using System.Collections.Concurrent;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using System.Linq;
using System.Collections.Generic;
using TechnicalInfo.Infrastructure.Interfaces;

namespace TechnicalInfo.Infrastructure.Wmi
{
    public class WmiInfoCollectorService : IInfoCollectorService
    {
        private ConcurrentDictionary<string, WorkStationModel> _computers = new ConcurrentDictionary<string, WorkStationModel>();
        private WmiInfoCollector _infoCollector = new WmiInfoCollector();

        public Task<List<WorkStationModel>> GetComputersInfo(string[] wsNames)
        {
            var tasks = wsNames.ToList().Select(x =>
            {
                return Task.Run(async () =>
                {
                    var result = await _infoCollector.GetWorkStationInfo(x);
                    _computers.TryAdd(x, result);
                });
            }).ToArray();

            Task.WaitAll(tasks);

            return Task.FromResult(_computers.Select(x => x.Value).ToList());
        }
    }
}
