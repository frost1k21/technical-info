using System;
using System.Linq;
using System.Threading.Tasks;
using TechnicalInfo.Infrastructure.Wmi;
using TechnicalInfo.Inrfastructure.Interfaces;
using TechnicalInfo.Domain.Models;
using System.Diagnostics;

namespace TechnicalInfo.UIs.ConsoleApp
{
    static class Program
    {
        private static string wsNameWithXp = "ws1261";
        private static string wsNameWithWin7 = "ws555a";
        private static string wsNameWithWin10 = "ws1674";

        private static IInfoCollector infoCollector = new WmiInfoCollector();

        static async Task Main(string[] args)
        {
            //await GetData(wsNameWithXp);
            //await GetData(wsNameWithWin7);
            //await GetData(wsNameWithWin10);

            await GetWorkstationInfo(wsNameWithWin7);
        }

        static async Task GetData(string wsName)
        {
            var cpu = await infoCollector.Get<CpuModel>(wsName);
            var motherboard = await infoCollector.Get<MotherboardModel>(wsName);
            var userName = await infoCollector.Get<SystemUserName>(wsName);
            var operatingSystem = await infoCollector.Get<OperatingSystemModel>(wsName);
            var videoAdapter = await infoCollector.Get<VideoAdapterModel>(wsName);
            var ram = await infoCollector.Get<RamModel>(wsName);

            Console.WriteLine($"{wsName}:"); 
            Console.WriteLine($"Процессор: {cpu.FirstOrDefault()?.Name}. Частота: {cpu.FirstOrDefault()?.Frequency} MHz");
            Console.WriteLine($"Материнская плата: {motherboard.FirstOrDefault()?.Model} - Производитель: {motherboard.FirstOrDefault()?.Manufacturer}");
            Console.WriteLine($"Пользователь: {userName.FirstOrDefault()?.Login}");
            Console.WriteLine($"ОС: {operatingSystem.FirstOrDefault()?.Name}");
            Console.WriteLine($"Видео: {videoAdapter.FirstOrDefault()?.Name}");
            Console.WriteLine();
        }

        static async Task GetWorkstationInfo(string wsName)
        {
            var result = await infoCollector.GetWorkStationInfo(wsName);
            Console.WriteLine($"{wsName}");
            Console.WriteLine($"Процессор: {result.Cpu.Name}. Частота: {result.Cpu.Frequency} MHz");
            Console.WriteLine($"Материнская плата: {result.Motherboard.Model} - Производитель: {result.Motherboard.Manufacturer}");
            Console.WriteLine($"Пользователь: {result.SystemUser.Login}");
            Console.WriteLine($"ОС: {result.OperatingSystem.Name}");
            Console.WriteLine("Видео:");
            result.VideoAdapters.ForEach(x =>
            {
                Console.WriteLine($"\t{x.Name}");
                Console.WriteLine($"\t{x.Memory / Math.Pow(1024, 2)} Mb");
            });
            Console.WriteLine("ОЗУ:");
            result.Rams.ForEach(x =>
            {
                Console.WriteLine($"\t{x.Capacity / Math.Pow(1024, 2)} Mb");
            });
            Console.WriteLine("Разделы:");
            result.PartitionDisks.ForEach(x =>
            {
                Console.WriteLine($"\t{x.Name}");
                Console.WriteLine($"\t{x.Size / Math.Pow(1024, 3):0.##} Gb");
            });

            Console.WriteLine();
        }
    }
}
