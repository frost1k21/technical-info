using System;
using System.Linq;
using System.Threading.Tasks;
using TechnicalInfo.Infrastructure.Wmi;
using TechnicalInfo.Inrfastructure.Interfaces;
using TechnicalInfo.Domain.Models;

namespace TechnicalInfo.UIs.ConsoleApp
{
    static class Program
    {
        private static string wsName = "ws555a";

        static async Task Main(string[] args)
        {
            IInfoCollector infoCollector = new WmiInfoCollector();

            var motherboard = await infoCollector.Get<MotherboardModel>(wsName);
            var userName = await infoCollector.Get<SystemUserName>(wsName);
            var operatingSystem = await infoCollector.Get<OperatingSystemModel>(wsName);
            var videoAdapter = await infoCollector.Get<VideoAdapterModel>(wsName);

            Console.WriteLine($"Motherboard: {motherboard.FirstOrDefault().Model} - Manufacturer: {motherboard.FirstOrDefault().Manufacturer}");
            Console.WriteLine($"User Login: {userName.FirstOrDefault().Login}");
            Console.WriteLine($"OS: {operatingSystem.FirstOrDefault().Name}");
            Console.WriteLine($"Video: {videoAdapter.FirstOrDefault().Name}");
        }
    }
}
