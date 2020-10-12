using System.Management;

namespace TechnicalInfo.Infrastructure.Wmi
{
    public static class WmiPropertyReceiverGuard
    {
        public static string SafeGetProperty(this ManagementBaseObject managementBaseObject, string propertyName)
        {
			try
			{
				return managementBaseObject[propertyName]?.ToString() ?? "";
			}
			catch (System.Exception)
			{
				return "";
			}
        }
    }
}
