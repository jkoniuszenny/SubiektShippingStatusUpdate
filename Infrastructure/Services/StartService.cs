using Core.Enums;
using Core.Models;
using Core.Repositories.Interfaces;
using Infrastructure.Dto;
using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StartService : IStartService
    {
        private readonly ExtraConfigurationSettings _extraConfigurationSettings;
        private readonly IScrapGLSService _scrapGLSService;
        private readonly ISqlQueryRepository _sqlQueryRepository;
        private readonly IFlagValueRepository _flagValueRepository;
        private readonly IFlagDictionaryRepository _flagDictionaryRepository;

        public StartService(
            ExtraConfigurationSettings extraConfigurationSettings,
            IScrapGLSService scrapGLSService,
            ISqlQueryRepository sqlQueryRepository,
            IFlagValueRepository flagValueRepository,
            IFlagDictionaryRepository flagDictionaryRepository)
        {
            _extraConfigurationSettings = extraConfigurationSettings;
            _scrapGLSService = scrapGLSService;
            _sqlQueryRepository = sqlQueryRepository;
            _flagValueRepository = flagValueRepository;
            _flagDictionaryRepository = flagDictionaryRepository;
        }

        public async Task RunProgram()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileNamePath = @$"{Path.GetDirectoryName(strExeFilePath)}\Logs\Log_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt";
            Directory.CreateDirectory(@$"{Path.GetDirectoryName(strExeFilePath)}\Logs");

            //Zbieram info o tym co zaktualizowałem do klasy żeby zapisać w pliku
            List<ForLogDto> forLogs = new List<ForLogDto>();

            //Pobieram słownik flag
            var flagDictionaries = await _flagDictionaryRepository.GetFlagDictionary();

            //Przygotowuje mapowanie statusów GLS do flag z bazy
            var mappedFlag = GetGLSMappedFlag(flagDictionaries);

            //Pobieram dane sprzed n dni z appsettings w celu sparwdzenia zmian statusu
            var trackingNumbers = await _sqlQueryRepository.GetTrackingNumber(mappedFlag, DateTime.Now.Date.AddDays((-1 * _extraConfigurationSettings.DaysBack)));

            //Pobieram statusy ze strony GLS
            var result = await _scrapGLSService.GetPackagesStatus(trackingNumbers.Where(w => w.GLS is { }).Select(s => s.GLS).Where(w => w.Length > 1));

            //Zmianiem statusy na nowe dla wpisów pobranych z bazy i tworzę listę obiektów do aktualizacji (flag przypisanych do faktur)
            List<int> objectId = new List<int>();
            foreach (var item in result)
            {
                if (trackingNumbers.Any(f => f.GLS == item.TrackingNumber && f.GLSStatus != item.ActualStatus))
                {
                    trackingNumbers.FirstOrDefault(f => f.GLS == item.TrackingNumber && f.GLSStatus != item.ActualStatus).GLSStatus = item.ActualStatus;
                    objectId.Add(Convert.ToInt32(trackingNumbers.FirstOrDefault(f => f.GLS == item.TrackingNumber).dok_Id));
                }
            }

            //Pobieram wpisy do aktualizacji
            var flagValueListToUpdate = await _flagValueRepository.GetFlagValue(objectId);

            //Aktualizuje dane na pobranych wpisach
            flagValueListToUpdate.All(a =>
            {
                forLogs.Add(new ForLogDto()
                {
                    DocumentId = a.flw_IdObiektu,
                    Operation = "Aktualizacja",
                    OldFlag = Convert.ToInt32(a.flw_IdFlagi),
                    NewFlag = mappedFlag.FirstOrDefault(f => f.GLSStatus == trackingNumbers.FirstOrDefault(w => w.dok_Id == a.flw_IdObiektu).GLSStatus).FlagId
                });

                a.flw_IdFlagi = mappedFlag.FirstOrDefault(f => f.GLSStatus == trackingNumbers.FirstOrDefault(w => w.dok_Id == a.flw_IdObiektu).GLSStatus).FlagId;
                a.flw_CzasOstatniejZmiany = DateTime.Now;
                return true;
            });

            //Wykonuję aktualizację
            await _flagValueRepository.UpdateFlagValue(flagValueListToUpdate);

            using (StreamWriter streamNewFile = File.CreateText(fileNamePath))
            {
                foreach (var item in forLogs.Where(w => w.Operation == "Aktualizacja"))
                {
                    await streamNewFile.WriteLineAsync(item.ToString());
                }
            }


            //Wybieram elementy z bazy dla których należy dodać nowe Flagi do kodumentów
            var trackingNumersToInsert = trackingNumbers.Where(a => a is { } && a.flw_IdFlagi == 0 && !flagValueListToUpdate.Select(s => s.flw_IdObiektu).Contains(Convert.ToInt32(a.dok_Id)));

            //Przechodzę po elementach do dodania i tworzę dla nich flagi
            List<FlagValue> flagValueToInsert = new List<FlagValue>();
            trackingNumersToInsert.All(a =>
            {
                if (mappedFlag.Any(f => f.GLSStatus == a.GLSStatus))
                {
                    forLogs.Add(new ForLogDto()
                    {
                        DocumentId = Convert.ToInt32(a.dok_Id),
                        Operation = "Dodanie",
                        OldFlag = 0,
                        NewFlag = Convert.ToInt32(mappedFlag.FirstOrDefault(f => f.GLSStatus == a.GLSStatus)?.FlagId ?? 0)
                    });

                    flagValueToInsert.Add(new FlagValue()
                    {
                        flw_IdFlagi = mappedFlag.FirstOrDefault(f => f.GLSStatus == a.GLSStatus).FlagId,
                        flw_TypObiektu = 0,
                        flw_Komentarz = "",
                        flw_IdUzytkownika = 1,
                        flw_CzasOstatniejZmiany = DateTime.Now,
                        flw_IdObiektu = Convert.ToInt32(a.dok_Id),
                        flw_IdGrupyFlag = mappedFlag.FirstOrDefault(f => f.GLSStatus == a.GLSStatus).FlagGroup
                    });
                }


                return true;
            });

            //Zapisuję nowe wartości flag
            await _flagValueRepository.InsertFlagValue(flagValueToInsert);

            using (StreamWriter streamOldFile = File.AppendText(fileNamePath))
            {
                foreach (var item in forLogs.Where(w => w.Operation == "Dodanie"))
                {
                    await streamOldFile.WriteLineAsync(item.ToString());
                }
            }
        }


        private IEnumerable<MappingFlagToShippingStatus> GetGLSMappedFlag(IEnumerable<FlagDictionary> flagDictionaries)
        {

            List<MappingFlagToShippingStatus> mappingFlagToShippings = new List<MappingFlagToShippingStatus>();


            flagDictionaries.OrderBy(a => a.flg_IdGrupy).All(a =>
            {
                switch (a.flg_Numer)
                {
                    case 6:
                        mappingFlagToShippings.Add(new MappingFlagToShippingStatus()
                        {
                            FlagId = a.flg_Id,
                            FlagGroup = a.flg_IdGrupy,
                            GLSStatus = GLSStatus.PREADVICE
                        });
                        break;
                    case 7:
                        mappingFlagToShippings.Add(new MappingFlagToShippingStatus()
                        {
                            FlagId = a.flg_Id,
                            FlagGroup = a.flg_IdGrupy,
                            GLSStatus = GLSStatus.INTRANSIT
                        });
                        break;
                    case 1:
                        mappingFlagToShippings.Add(new MappingFlagToShippingStatus()
                        {
                            FlagId = a.flg_Id,
                            FlagGroup = a.flg_IdGrupy,
                            GLSStatus = GLSStatus.INWAREHOUSE
                        });
                        break;
                    case 8:
                        mappingFlagToShippings.Add(new MappingFlagToShippingStatus()
                        {
                            FlagId = a.flg_Id,
                            FlagGroup = a.flg_IdGrupy,
                            GLSStatus = GLSStatus.INDELIVERY
                        });
                        break;
                    case 10:
                        mappingFlagToShippings.Add(new MappingFlagToShippingStatus()
                        {
                            FlagId = a.flg_Id,
                            FlagGroup = a.flg_IdGrupy,
                            GLSStatus = GLSStatus.DELIVERED
                        });
                        break;
                    default:
                        break;
                }

                return true;
            });

            return mappingFlagToShippings;
        }
    }
}
