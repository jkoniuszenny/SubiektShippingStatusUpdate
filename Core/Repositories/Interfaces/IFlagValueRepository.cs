using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Interfaces
{
    public interface IFlagValueRepository : IRepository
    {
        Task<IEnumerable<FlagValue>> GetFlagValue(IEnumerable<int> objectIds);
        Task UpdateFlagValue(IEnumerable<FlagValue> flagValues);
        Task InsertFlagValue(IEnumerable<FlagValue> flagValues);
    }
}
