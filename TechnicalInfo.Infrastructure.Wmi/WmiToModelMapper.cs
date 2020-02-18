using System;
using System.Management;
using TechnicalInfo.Domain.Models;

namespace TechnicalInfo.Infrastructure.Wmi
{
    public static class WmiToModelMapper
    {
        public static T MapTo<T>(this ManagementBaseObject managementBaseObject) where T: class, new()
        {
            switch (new T())
            {
                case MotherboardModel motherboard:
                    motherboard.Manufacturer = managementBaseObject.SafeGetProperty("Manufacturer");
                    motherboard.Model = managementBaseObject.SafeGetProperty("Product");
                    return motherboard as T;

                case CpuModel cpu:
                    cpu.Name = managementBaseObject.SafeGetProperty("Name");
                    return cpu as T;

                case RamModel ram:
                    return ram as T;

                case PartitionDiskDriveModel partitionDiskDrive:
                    partitionDiskDrive.Name = managementBaseObject.SafeGetProperty("Name");
                    return partitionDiskDrive as T;

                case OperatingSystemModel operatingSystem:
                    operatingSystem.Name = managementBaseObject.SafeGetProperty("Caption");
                    operatingSystem.OsArchitecture = managementBaseObject.SafeGetProperty("OSArchitecture");
                    operatingSystem.ServicePack = managementBaseObject.SafeGetProperty("ServicePackMajorVersion");
                    return operatingSystem as T;

                case SystemUserName systemUserName:
                    systemUserName.Login = managementBaseObject.SafeGetProperty("UserName");
                    return systemUserName as T;

                case VideoAdapterModel videoAdapter:
                    videoAdapter.Name = managementBaseObject.SafeGetProperty("Name");
                    return videoAdapter as T;

                default:
                    throw new InvalidCastException();
            }
        }
    }
}
