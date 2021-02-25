using Core.Enums;
using Core.Models;
using Core.Repositories.Interfaces;
using Core.Selects;
using Infrastructure.Database;
using Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SqlQueryRepository : ISqlQueryRepository
    {
        private readonly DatabaseContext _databaseContext;

        public SqlQueryRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<TrackingNumbersSelect>> GetTrackingNumber(IEnumerable<MappingFlagToShippingStatus> mappingFlagToShippings, DateTime dateTime)
        {
            //Wybieram listę flag dla których wykonam pobranie danych z bazy
            List<int> flagsId = mappingFlagToShippings.Where(w=>w.GLSStatus == GLSStatus.DELIVERED).Select(s=>s.FlagId).ToList();

            //Query pobierające dane z bazy 
            string query = @$"
                SELECT [dok_Id]
                      ,[dok_Typ]
                      , pd.pwd_Tekst01 'GLS'
	                  , pd.pwd_Tekst05 'DHL'
	                  , flw_IdGrupyFlag
	                  , flw_IdFlagi
                  FROM [dbo].[dok__Dokument] dd with(nolock)
                  inner join [dbo].[pw_Dane] pd with(nolock) on pd.pwd_IdObiektu = dd.dok_Id
                  left join  [dbo].[fl_Wartosc] fw with(nolock) on fw.flw_IdObiektu = dd.dok_Id
                  where
                  dok_DataWyst >= '{dateTime.ToString("yyyy-MM-dd")}' and
                  dd.dok_typ in (2, 21) and
                  (pd.pwd_Tekst01 is not null or pd.pwd_Tekst05 is not null )
                  and (flw_IdFlagi not in ({String.Join(",", flagsId)}) or flw_IdFlagi is null)";

            //Wywołanie pobrania danych i zmapowanie na obiekt
            var result = CustomSqlQueryExtensions.RawSqlQuery<TrackingNumbersSelect>(
                _databaseContext,
                query,
                x => new TrackingNumbersSelect(){
                    dok_Id = (int?)x[0],
                    dok_Type = (int?)x[1],
                    GLS = CustomSqlQueryExtensions.ConvertFromDBVal<string>(x[2]),
                    DHL = CustomSqlQueryExtensions.ConvertFromDBVal<string>(x[3]),
                    flw_IdGrupyFlag = CustomSqlQueryExtensions.ConvertFromDBVal<int>(x[4]),
                    flw_IdFlagi = CustomSqlQueryExtensions.ConvertFromDBVal<int>(x[5]),
                    GLSStatus = mappingFlagToShippings.FirstOrDefault(f=>f.FlagId == CustomSqlQueryExtensions.ConvertFromDBVal<int>(x[5]))?.GLSStatus ?? GLSStatus.NULL
                }
            );

            //Jeżeli było kilka nr dla jednego dokumentu, zostawiam tylko jeden
            result.All(a=> 
            {
                a.GLS = a.GLS is { } && a.GLS.Contains(",") ? a.GLS.Split(",")[0] : a.GLS;
                return true;
            });
            

            return await Task.FromResult(result);
        }
    }
}
