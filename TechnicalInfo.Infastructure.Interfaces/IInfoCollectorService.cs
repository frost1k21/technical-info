using SomeResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;

namespace TechnicalInfo.Infrastructure.Interfaces
{
    public interface IInfoCollectorService
    {
        Task<List<Result<WorkStationModel, string>>> GetComputersInfo(string[] wsNames);
    }
}
