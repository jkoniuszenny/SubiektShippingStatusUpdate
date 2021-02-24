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

        public async Task<IEnumerable<ShippingStatusDto>> GetPackagesStatus(IEnumerable<string> trackingList)
        {
            List<ShippingStatusDto> shippingStatus = new List<ShippingStatusDto>();

            for (int i = 0; i <= ((trackingList.Count() + 50) / 100 * 100)/100; i++)
            {
                string joined = String.Join("+",trackingList.Skip(100 * ((i+1) - 1)).Take(100));

                if (joined.Length < 1)
                    continue;
                
                _restClient = new RestClient($"https://gls-group.eu/app/service/open/rest/PL/pl/rstt001?match={joined}&type=&caller=witt002&millis={Convert.ToUInt64((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)}");
                _restClient.FollowRedirects = false;

                RestRequest restRequest = new RestRequest(Method.GET);


                var respons = await _restClient.ExecuteAsync(restRequest);

                JsonDto JsonDto = new JsonDto();

                try
                {
                    JsonDto = JsonSerializer.Deserialize<JsonDto>(respons.Content, _JsonSerializerOptions);
                }
                catch
                {
                }

                foreach (var item in JsonDto.TuStatus)
                {
                    try
                    {
                        shippingStatus.Add(new ShippingStatusDto()
                        {
                            TrackingNumber = item.TuNo,
                            ActualStatus = item.ProgressBar.StatusInfo.Contains("DELIVEREDPS") ? GLSStatus.DELIVERED : (GLSStatus)Enum.Parse(typeof(GLSStatus), item.ProgressBar.StatusInfo)
                        });
                    }
                    catch
                    {
                    }
                }

                await Task.Delay(500);
            }

            return shippingStatus.AsEnumerable();
        }
    }
}
