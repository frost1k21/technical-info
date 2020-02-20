using System.Collections.Concurrent;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using System.Linq;
using System.Collections.Generic;
using TechnicalInfo.Infrastructure.Interfaces;
using SomeResult;
using NetworkWsCheck;

namespace TechnicalInfo.Infrastructure.Wmi
{
    public class WmiInfoCollectorService : IInfoCollectorService
    {
        private ConcurrentDictionary<string, Result<WorkStationModel, string>> _computers = new ConcurrentDictionary<string, Result<WorkStationModel, string>>();
        private WmiInfoCollector _infoCollector = new WmiInfoCollector();

        public Task<List<Result<WorkStationModel, string>>> GetComputersInfo(string[] wsNames)
        {
            var tasks = wsNames.ToList().Select(x =>
            {
                return Task.Run(async () =>
                {
                    if(await NetworkWsChecker.WsCheck(x))
                    {
                        var result = await _infoCollector.GetWorkStationInfo(x);
                        if (result.Error == null)
                            _computers.TryAdd(x, new Result<WorkStationModel, string>() { Success = result.Success });
                        else
                            _computers.TryAdd(x, new Result<WorkStationModel, string>() { Error = result.Error });
                    }
                    else
                    {
                        _computers.TryAdd(x, new Result<WorkStationModel, string>() { Error = $"{x} не в сети" });
                    }
                });
            }).ToArray();

            Task.WaitAll(tasks);

            return Task.FromResult(_computers.Select(x => x.Value).ToList());
        }
    }
}
