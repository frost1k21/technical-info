using System;
using System.Collections.Generic;

namespace TechnicalInfo.Domain.Models
{
    public class WorkStationModel
    {
        public CpuModel Cpu { get; set; }
        public MotherboardModel motherboard { get; set; }
        public SystemUserName SystemUser { get; set; }
        public OperatingSystem OperatingSystem { get; set; }
        public List<RamModel> Rams { get; set; }
        public List<PartitionDiskDriveModel> PartitionDisks { get; set; }
        public List<VideoAdapterModel> VideoAdapters { get; set; }
    }
}
