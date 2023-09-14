using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        ISensorsRepository Sensors { get; }
        IRecordsRepository Records { get;}
        Task<bool> Complete();
    }
}