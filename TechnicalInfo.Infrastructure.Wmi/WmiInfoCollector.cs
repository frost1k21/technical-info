using System;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Infastructure.Interfaces;

namespace TechnicalInfo.Infrastructure.Wmi
{

    public class WmiInfoCollector : IInfoCollector
    {
        public Task<MotherboardModel> GetMotherboard(string wsName)
        {
            return Task.Run(() =>
            {
                var scope = new ManagementScope($@"\\{wsName}\root\cimv2");
                var path = new ManagementPath("Win32_BaseBoard");
                var options = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
                var managementClass = new ManagementClass(scope, path, options);

                return managementClass.GetInstances()
                    .Cast<ManagementBaseObject>()
                    .Select(x => WmiToModelMapper.MapTo<MotherboardModel>(x))
                    .FirstOrDefault();
            });
        }
    }
}
