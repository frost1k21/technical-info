using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Inrfastructure.Interfaces;

namespace TechnicalInfo.Infrastructure.Wmi
{
    /// <summary>
    /// Class that implements <see cref="IInfoCollector"> wmi specific
    /// </summary>
    public class WmiInfoCollector : IInfoCollector
    {
        #region Private Field(s)

        private string _baseBoard = "Win32_BaseBoard";
        private string _videoController = "Win32_VideoController";
        private string _physicalMemory = "Win32_PhysicalMemory";
        private string _computerSystem = "Win32_ComputerSystem";
        private string _operatingSystem = "Win32_OperatingSystem";
        private string _processor = "Win32_Processor";
        private string _logicalDisk = "Win32_LogicalDisk";

        #endregion

        #region Public Method(s)

        public Task<WorkStationModel> GetWorkStationInfo(string wsName)
        {
            var motherboard = Get<MotherboardModel>(wsName);
            var cpu = Get<CpuModel>(wsName);
            var operatingSystem = Get<OperatingSystemModel>(wsName);
            var videoAdapter = Get<VideoAdapterModel>(wsName);
            var userName = Get<SystemUserName>(wsName);
            var memory = Get<RamModel>(wsName);
            var partitions = Get<PartitionDiskDriveModel>(wsName);

            Task.WaitAll(motherboard, cpu, operatingSystem, videoAdapter, userName);

            var workStation = new WorkStationModel()
            {
                Cpu = cpu.Result.FirstOrDefault(),
                Motherboard = motherboard.Result.FirstOrDefault(),
                OperatingSystem = operatingSystem.Result.FirstOrDefault(),
                SystemUser = userName.Result.FirstOrDefault(),
                VideoAdapters = videoAdapter.Result.Where(v => v.Memory != 0).ToList(),
                Rams = memory.Result.ToList(),
                PartitionDisks = partitions.Result.ToList()
            };

            return Task.FromResult(workStation);

        }

        public Task<IEnumerable<T>> Get<T>(string wsName) where T : class, new()
        {
            return Task.Run(() =>
            {
                var scope = new ManagementScope(GetWmiPath(wsName));
                var path = GetManagementPath<T>();
                var options = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
                var managementClass = new ManagementClass(scope, path, options);

                return managementClass.GetInstances()
                    .Cast<ManagementBaseObject>()
                    .Select(x => x.MapTo<T>());
            });
        }

        #endregion

        #region Private Method(s)

        private ManagementPath GetManagementPath<T>() where T: class, new ()
        {
            var path = _baseBoard;
            switch(new T())
            {
                case MotherboardModel motherboard:
                    path = _baseBoard;
                    break;

                case CpuModel cpuModel:
                    path = _processor;
                    break;

                case VideoAdapterModel videoAdapter:
                    path = _videoController;
                    break;

                case RamModel ramModel:
                    path = _physicalMemory;
                    break;

                case OperatingSystemModel operatingSystem:
                    path = _operatingSystem;
                    break;

                case SystemUserName systemUserName:
                    path = _computerSystem;
                    break;

                case PartitionDiskDriveModel partitionDiskDrive:
                    path = _logicalDisk;
                    break;

                default:
                    throw new InvalidCastException();
            }

            return new ManagementPath(path);
        }

        private string GetWmiPath(string wsName)
        {
            return $@"\\{wsName}\root\cimv2";
        }

        #endregion

    }
}
