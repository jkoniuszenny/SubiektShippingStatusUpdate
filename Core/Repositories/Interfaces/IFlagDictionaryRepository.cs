using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Interfaces
{
    public interface IFlagDictionaryRepository : IRepository
    {
        Task<IEnumerable<FlagDictionary>> GetFlagDictionary();
    }
}
