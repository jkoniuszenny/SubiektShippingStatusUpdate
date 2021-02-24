using Core.Models;
using Core.Repositories.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FlagDictionaryRepository : IFlagDictionaryRepository
    {
        private readonly DatabaseContext _databaseContext;

        public FlagDictionaryRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<FlagDictionary>> GetFlagDictionary()
        {
            return await _databaseContext.FlagDictionary.Where(w=>new[] {4, 6}.Contains(w.flg_IdGrupy)).AsNoTracking().ToListAsync();
        }
    }
}
