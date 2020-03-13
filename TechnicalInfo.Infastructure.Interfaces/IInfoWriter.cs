using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;

namespace TechnicalInfo.Infrastructure.Interfaces
{
    public interface IInfoWriter
    {
        Task Write(params WorkStationModel[] workStationModels);
    }
}
