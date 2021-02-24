using Core.Models;
using Core.Selects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Interfaces
{
    public interface ISqlQueryRepository : IRepository
    {
        Task<IEnumerable<TrackingNumbersSelect>> GetTrackingNumber(IEnumerable<MappingFlagToShippingStatus> mappingFlagToShippings ,DateTime dateTime);
    }
}
