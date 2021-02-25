using Core.Models;
using Infrastructure.Dto.DHL;
using Infrastructure.Dto.GLS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Interfaces
{
    public interface IScrapDHLService : IService
    {
        Task<IEnumerable<DHLShippingStatusDto>> GetPackagesStatus(IEnumerable<string> trackingList);
    }
}
