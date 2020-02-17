
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;

namespace TechnicalInfo.Infastructure.Interfaces
{
    public interface IInfoCollector
    {
        Task<MotherboardModel> GetMotherboard(string wsName);
    }
}
