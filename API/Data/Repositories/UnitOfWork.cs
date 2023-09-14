using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;

namespace API.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ISensorsRepository Sensors { get; }

        public IRecordsRepository Records { get; }
        private readonly DataContext _context;

        public UnitOfWork(ISensorsRepository sensors, IRecordsRepository records, 
            DataContext context)
        {
            _context = context;
            Sensors = sensors;
            Records = records;
        }

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}