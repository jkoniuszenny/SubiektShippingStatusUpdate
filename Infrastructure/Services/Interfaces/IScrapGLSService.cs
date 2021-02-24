
using Infrastructure.Dto.GLS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Interfaces
{
    public interface IScrapGLSService : IService
    {
        Task<IEnumerable<ShippingStatusDto>> GetPackagesStatus(IEnumerable<string> trackingList);
    }
}
