using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Inrfastructure.Interfaces;
using SomeResult;
using System.Text;

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
        private string _logicalDisk = "Win32_DiskDrive";

        private string _monitor = "WmiMonitorID";
        private string _monitorConnection = "WmiMonitorConnectionParams";

        private Dictionary<string, string> _manufacturers = new Dictionary<string, string>()
        {
            { "AAC","AcerView" },
            { "ACR", "Acer" },
            { "AOC", "AOC" },
            { "AIC","AG Neovo" },
            { "APP", "Apple Computer"},
            { "AST", "AST Research" },
            { "AUO", "Asus"},
            { "BNQ", "BenQ" },
            { "CMO", "Acer"},
            {"CPL", "Compal" },
            {"CPQ", "Compaq" },
            {"CPT", "Chunghwa Pciture Tubes, Ltd." },
            {"CTX", "CTX" },
            {"DEC", "DEC" },
            {"DEL", "Dell" },
            {"DPC", "Delta" },
            {"DWE", "Daewoo" },
            {"EIZ", "EIZO" },
            {"ELS", "ELSA" },
            {"ENC", "EIZO" },
            {"EPI", "Envision" },
            {"FCM", "Funai" },
            {"FUJ", "Fujitsu" },
            {"FUS", "Fujitsu-Siemens" },
            {"GSM", "LG Electronics" },
            {"GWY", "Gateway 2000" },
            {"HEI", "Hyundai" },
            {"HIT", "Hyundai" },
            {"HSL", "Hansol" },
            {"HTC", "Hitachi/Nissei" },
            {"HWP", "HP" },
            {"IBM", "IBM" },
            {"ICL", "Fujitsu ICL" },
            {"IVM", "Iiyama" },
            {"KDS", "Korea Data Systems" },
            {"LEN", "Lenovo" },
            {"LGD", "Asus" },
            {"LPL", "Fujitsu" },
            {"MAX", "Belinea" },
            {"MEI", "Panasonic" },
            {"MEL", "Mitsubishi Electronics" },
            {"MS_", "Panasonic" },
            {"NAN", "Nanao" },
            {"NEC", "NEC" },
            {"NOK", "Nokia Data" },
            {"NVD", "Fujitsu" },
            {"OPT", "Optoma" },
            {"PHL", "Philips" },
            {"REL", "Relisys" },
            {"SAN", "Samsung" },
            {"SAM", "Samsung" },
            {"SBI", "Smarttech" },
            {"SGI", "SGI" },
            {"SNY", "Sony" },
            {"SRC", "Shamrock" },
            {"SUN", "Sun Microsystems" },
            {"SEC", "Hewlett-Packard" },
            {"TAT", "Tatung" },
            {"TOS", "Toshiba" },
            {"TSB", "Toshiba" },
            {"VSC", "ViewSonic" },
            {"ZCM", "Zenith" },
            {"UNK", "Unknown" },
            {"_YV", "Fujitsu" }
        };
        private Dictionary<MonitorConnectionPorts, string> _monitorPorts = new Dictionary<MonitorConnectionPorts, string>()
        {
            //TODO add all ports
            { MonitorConnectionPorts.D3DKMDT_VOT_HD15, "VGA" },
            { MonitorConnectionPorts.D3DKMDT_VOT_HDMI, "HDMI" },
            { MonitorConnectionPorts.D3DKMDT_VOT_DVI, "DVI" }
        };

        #endregion

        #region Public Method(s)

        public Task<Result<WorkStationModel, string>> GetWorkStationInfo(string wsName)
        {
            var motherboard = Get<MotherboardModel>(wsName);
            var cpu = Get<CpuModel>(wsName);
            var operatingSystem = Get<OperatingSystemModel>(wsName);
            var videoAdapter = Get<VideoAdapterModel>(wsName);
            var userName = Get<SystemUserName>(wsName);
            var memory = Get<RamModel>(wsName);
            var partitions = Get<DiskDriveModel>(wsName);
            var monitors = GetMonitors(wsName);
            var memoryTypeStrings = GetMemoryType(wsName);

            Task.WaitAll(motherboard, cpu, operatingSystem, videoAdapter, userName, memory, partitions, monitors);

            if (motherboard.Result.Error != null)
                return Task.FromResult(new Result<WorkStationModel, string>() { Error = motherboard.Result.Error });

            var workstation2 = new WorkStationModel();
            workstation2.WsName = wsName;
            workstation2.Cpu = cpu.Result.Success.FirstOrDefault();
            workstation2.Motherboard = motherboard.Result.Success.FirstOrDefault();
            workstation2.OperatingSystem = operatingSystem.Result.Success.FirstOrDefault();
            workstation2.SystemUser = userName.Result.Success.FirstOrDefault();
            workstation2.VideoAdapters = videoAdapter.Result.Success.Where(v => v.Memory != 0).ToList();
            workstation2.Rams = memory.Result.Success.ToList();
            workstation2.Rams.ForEach(x => x.MemType = memoryTypeStrings.Result.Success);
            workstation2.Monitors = monitors.Result.Success?.ToList();
            workstation2.DiskDrives = partitions.Result.Success.Where(p => p.Size != 0).ToList();

            return Task.FromResult(new Result<WorkStationModel, string>() { Success = workstation2 });

        }

        public Task<Result<IEnumerable<T>, string>> Get<T>(string wsName) where T : class, new()
        {
            return Task.Run(() =>
            {
                try
                {
                    var scope = new ManagementScope(GetWmiPath(wsName));
                    var path = GetManagementPath<T>();
                    var options = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
                    var managementClass = new ManagementClass(scope, path, options);

                    var successResult = managementClass.GetInstances()
                        .Cast<ManagementBaseObject>()
                        .Select(x => x.MapTo<T>());
                    return Task.FromResult(new Result<IEnumerable<T>, string>() { Success = successResult });
                }
                catch (Exception e)
                {
                    return Task.FromResult(new Result<IEnumerable<T>, string>() { Error = $"{wsName} {e.Message}" });
                }
                
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

                case DiskDriveModel partitionDiskDrive:
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


        // TODO: complete memory type collection
        private Task<Result<string, string>> GetMemoryType(string wsName)
        {
            return Task.Run(() => 
            {
                try
                {
                    var scope = new ManagementScope($@"\\{wsName}\root\wmi");
                    scope.Connect();

                    var selectQuery = new SelectQuery("Select * from MSSmBios_RawSMBiosTables");

                    var searcher = new ManagementObjectSearcher(scope, selectQuery);
                    var memoryTypeStrings = new List<string>();

                    foreach (var data in searcher.Get())
                    {
                        var dataByte = data["SMBiosData"] as byte[];

                        for (int i = 0; i < dataByte.Length; i++)
                        {
                            if (dataByte[i] == 17 && dataByte[i - 1] == 00 && dataByte[i - 2] == 00)
                            {
                                var memType = dataByte[i + 0x12];

                                switch (memType)
                                {
                                    case 26:
                                        memoryTypeStrings.Add("DDR4");
                                        break;
                                    case 20:
                                        memoryTypeStrings.Add("DDR");
                                        break;
                                    case 21:
                                        memoryTypeStrings.Add("DDR2");
                                        break;
                                    case 24:
                                        memoryTypeStrings.Add("DDR3");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    if(memoryTypeStrings.Count > 0)
                        return Task.FromResult(new Result<string, string>() { Success = memoryTypeStrings.First() });

                    return Task.FromResult(new Result<string, string>() { Success = "" });
                }
                catch (Exception e)
                {

                    return Task.FromResult(new Result<string, string>() { Error = $"{wsName} {e.Message}" });
                }
            });
            
        }

        private Task<Result<IEnumerable<MonitorModel>, string>> GetMonitors(string wsName)
        {
            return Task.Run(() =>
            {
                try
                {
                    var monitors = new List<MonitorModel>();
                    var scope = new ManagementScope($@"\\{wsName}\root\wmi");
                    var monitorpath = new ManagementPath(_monitor);
                    var monitorPortPath = new ManagementPath(_monitorConnection);
                    var options = new ObjectGetOptions(null, TimeSpan.MaxValue, true);

                    var monitorMangementClass = new ManagementClass(scope, monitorpath, options);
                    var monitorPortManagementClass = new ManagementClass(scope, monitorPortPath, options);

                    var result = monitorMangementClass.GetInstances();
                    foreach (var item in result)
                    {
                        var monitorByteArr = ((UInt16[])item["UserFriendlyName"]).ToList().Select(Convert.ToByte).ToArray();
                        var monitorModel = Encoding.UTF8.GetString(monitorByteArr);
                        var manufactByteArr = ((UInt16[])item["ManufacturerName"]).ToList().Take(3).Select(Convert.ToByte).ToArray();
                        var manufacturerShort = Encoding.UTF8.GetString(manufactByteArr);
                        var instanceName = item["InstanceName"].ToString();
                        if (_manufacturers.TryGetValue(manufacturerShort, out string manufacturer))
                            monitors.Add(new MonitorModel { Instancename = instanceName, Model = monitorModel, Manufacturer = manufacturer });
                    }

                    var resultInputTech = monitorPortManagementClass.GetInstances();
                    foreach (var item in resultInputTech)
                    {
                        var resultNumber = (MonitorConnectionPorts)int.Parse(item["VideoOutputTechnology"].ToString());
                        var instanceName = item["InstanceName"].ToString();
                        if (_monitorPorts.TryGetValue(resultNumber, out string portName))
                            monitors = monitors.Select(x => 
                            {
                                if (x.Instancename == instanceName)
                                {
                                    x.MonitorConnectionPort = resultNumber;
                                    x.MonitorConnectionPortName = portName;
                                }
                                return x;
                            }).ToList();
                    }


                    return Task.FromResult(
                        new Result<IEnumerable<MonitorModel>, string>()
                        {
                            Success = monitors
                        }
                    );
                }
                catch (Exception e)
                {
                    return Task.FromResult(new Result<IEnumerable<MonitorModel>, string>() { Error = $"{wsName} {e.Message}" });
                }
            });
            
        }

        #endregion

    }
}
