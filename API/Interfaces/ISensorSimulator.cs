using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.DTOs;
using API.Data.Entities;

namespace API.Interfaces
{
    public interface ISensorSimulator
    {
        Task<Record> Simulate(int id, RecordDto properties);
    }
}