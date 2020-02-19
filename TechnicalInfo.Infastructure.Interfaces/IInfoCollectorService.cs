using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;

namespace TechnicalInfo.Infrastructure.Interfaces
{
    public interface IInfoCollectorService
    {
        Task<List<WorkStationModel>> GetComputersInfo(string[] wsNames);
    }
}
