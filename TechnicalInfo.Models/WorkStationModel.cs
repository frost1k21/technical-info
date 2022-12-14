using System;
using System.Collections.Generic;

namespace TechnicalInfo.Domain.Models
{
    public class WorkStationModel
    {
        public string WsName { get; set; }
        public CpuModel Cpu { get; set; }
        public MotherboardModel Motherboard { get; set; }
        public SystemUserName SystemUser { get; set; }
        public OperatingSystemModel OperatingSystem { get; set; }
        public List<RamModel> Rams { get; set; }
        public List<DiskDriveModel> DiskDrives { get; set; }
        public List<VideoAdapterModel> VideoAdapters { get; set; }
        public List<MonitorModel> Monitors { get; set; }
    }
}
