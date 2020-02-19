using System;
using System.Threading.Tasks;
using TechnicalInfo.Infrastructure.Wmi;
using TechnicalInfo.Inrfastructure.Interfaces;

namespace TechnicalInfo.UIs.ConsoleApp
{
    static class Program
    {
        private static string wsNameWithXp = "ws1261";
        private static string wsNameWithWin7 = "ws555a";
        private static string wsNameWithWin10 = "ws1674";
        private static string ws007 = "ws007";
        private static string ws2473 = "ws2473";

        private static IInfoCollector infoCollector = new WmiInfoCollector();

        static async Task Main(string[] args)
        {

            await GetWorkstationInfo(wsNameWithXp);
            await GetWorkstationInfo(wsNameWithWin7);
            await GetWorkstationInfo(wsNameWithWin10);
            await GetWorkstationInfo(ws007);
            await GetWorkstationInfo(ws2473);
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
