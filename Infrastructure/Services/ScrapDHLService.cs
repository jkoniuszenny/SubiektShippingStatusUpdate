using Infrastructure.Dto.GLS;
using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;
using System.Text.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Core.Enums;
using Core.Models;
using Infrastructure.Dto.DHL;

namespace Infrastructure.Services
{
    public class ScrapDHLService : IScrapDHLService
    {
        private RestClient _restClient;
        private readonly DHLSettings _dhlSettings;
        private JsonSerializerOptions _JsonSerializerOptions;

        public ScrapDHLService(DHLSettings dhlSettings)
        {
            _dhlSettings = dhlSettings;

            _JsonSerializerOptions = new JsonSerializerOptions();
            _JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        }

        public async Task<IEnumerable<DHLShippingStatusDto>> GetPackagesStatus(IEnumerable<string> trackingList)
        {
            List<DHLShippingStatusDto> shippingStatus = new List<DHLShippingStatusDto>();

            foreach (var item in trackingList)
            {
                try
                {
                    Convert.ToUInt64(item);
                }
                catch 
                {
                    continue;
                }

                    

                _restClient = new RestClient($"{_dhlSettings.MainUrl}?trackingNumber={item}&language=en&limit=5");
                _restClient.AddDefaultHeader("DHL-API-Key", _dhlSettings.ApiKey);
                _restClient.FollowRedirects = false;

                RestRequest restRequest = new RestRequest(Method.GET);


                var respons = await _restClient.ExecuteAsync(restRequest);


                DHLJsonDto jsonDto = new DHLJsonDto();

                try
                {
                    jsonDto = JsonSerializer.Deserialize<DHLJsonDto>(respons.Content, _JsonSerializerOptions);
                
                    shippingStatus.Add(new DHLShippingStatusDto()
                    {
                        TrackingNumber = jsonDto.Shipments.FirstOrDefault().Id,
                        ActualStatus = (DHLStatus)Enum.Parse(typeof(DHLStatus), jsonDto.Shipments.FirstOrDefault().Status.StatusCode)
                    });
                }
                catch
                {
                }

                await Task.Delay(100);
            }

            return shippingStatus.AsEnumerable();
        }
    }
}
