using Core.Models;
using Core.Repositories.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FlagValueRepository : IFlagValueRepository
    {
        private readonly DatabaseContext _databaseContext;

        public FlagValueRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<FlagValue>> GetFlagValue(IEnumerable<int> objectIds)
        {
            return await _databaseContext.FlagValue.Where(w => objectIds.Contains(w.flw_IdObiektu)).AsNoTracking().ToListAsync();
        }

        public async Task UpdateFlagValue(IEnumerable<FlagValue> flagValues)
        {
            _databaseContext.FlagValue.UpdateRange(flagValues);

            await _databaseContext.SaveChangesAsync();

        }

        public async Task InsertFlagValue(IEnumerable<FlagValue> flagValues)
        {
            await _databaseContext.FlagValue.AddRangeAsync(flagValues);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
