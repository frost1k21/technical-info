using System;
using System.Management;
using TechnicalInfo.Domain.Models;

namespace TechnicalInfo.Infrastructure.Wmi
{
    public static class WmiToModelMapper
    {
        public static T MapTo<T>(ManagementBaseObject managementBaseObject) where T: class, new()
        {
            switch (new T())
            {
                case MotherboardModel motherboard:
                    motherboard.Manufacturer = managementBaseObject["Manufacturer"].ToString();
                    motherboard.Model = managementBaseObject["Product"].ToString();
                    return motherboard as T;
                case CpuModel cpu:
                    return cpu as T;
                case RamModel ram:
                    return ram as T;
                case PartitionDiskDriveModel partitionDiskDrive:
                    return partitionDiskDrive as T;
                case OperatingSystemModel operatingSystem:
                    return operatingSystem as T;
                case SystemUserName systemUserName:
                    return systemUserName as T;
                case VideoAdapterModel videoAdapter:
                    return videoAdapter as T;

                default:
                    throw new InvalidCastException();
            }
        }
    }
}
