
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;

namespace TechnicalInfo.Inrfastructure.Interfaces
{
    public interface IInfoCollector
    {
        Task<IEnumerable<T>> Get<T>(string wsName) where T: class, new();
        Task<WorkStationModel> GetWorkStationInfo(string wsName);
    }
}
