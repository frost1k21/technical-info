using System;
using System.Threading.Tasks;
using TechnicalInfo.Infastructure.Interfaces;
using TechnicalInfo.Infrastructure.Wmi;

namespace TechnicalInfo.UIs.ConsoleApp
{
    static class Program
    {
        private static string wsName = "ws555a";

        static async Task Main(string[] args)
        {
            IInfoCollector infoCollector = new WmiInfoCollector();

            var motherboard = await infoCollector.GetMotherboard(wsName);

            Console.WriteLine($"Motherboard: {motherboard.Model} - Manufacturer: {motherboard.Manufacturer}");
        }
    }
}
