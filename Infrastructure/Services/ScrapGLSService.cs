
using Core.Enums;
using Infrastructure.Dto.GLS;
using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ScrapGLSService : IScrapGLSService
    {
        private RestClient _restClient;
        private readonly GLSSettings _glsSettings;
        private JsonSerializerOptions _JsonSerializerOptions;

        public ScrapGLSService(GLSSettings glsSettings)
        {
            _glsSettings = glsSettings;

            _JsonSerializerOptions = new JsonSerializerOptions();
            _JsonSerializerOptions.PropertyNameCaseInsensitive = true;


        }

        public async Task<IEnumerable<GLSShippingStatusDto>> GetPackagesStatus(IEnumerable<string> trackingList)
        {
            List<GLSShippingStatusDto> shippingStatus = new List<GLSShippingStatusDto>();

            for (int i = 0; i <= ((trackingList.Count() + 50) / 100 * 100) / 100; i++)
            {
                string joined = String.Join("+", trackingList.Skip(100 * ((i + 1) - 1)).Take(100));

                if (joined.Length < 1)
                    continue;

                _restClient = new RestClient($"{_glsSettings.MainUrl}rstt001?match={joined}&type=&caller=witt002&millis={Convert.ToUInt64((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)}");
                _restClient.FollowRedirects = false;

                RestRequest restRequest = new RestRequest(Method.GET);


                var respons = await _restClient.ExecuteAsync(restRequest);

                GLSJsonDto JsonDto = new GLSJsonDto();

                try
                {
                    JsonDto = JsonSerializer.Deserialize<GLSJsonDto>(respons.Content, _JsonSerializerOptions);
                }
                catch
                {
                }

                foreach (var item in JsonDto.TuStatus)
                {
                    try
                    {
                        shippingStatus.Add(new GLSShippingStatusDto()
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
