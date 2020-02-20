
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using SomeResult;

namespace TechnicalInfo.Inrfastructure.Interfaces
{
    public interface IInfoCollector
    {
        Task<Result<IEnumerable<T>, string>> Get<T>(string wsName) where T: class, new();
        Task<Result<WorkStationModel, string>> GetWorkStationInfo(string wsName);
    }
}
