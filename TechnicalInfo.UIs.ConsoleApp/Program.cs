using System;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Infrastructure.Interfaces;
using TechnicalInfo.Infrastructure.Wmi;

namespace TechnicalInfo.UIs.ConsoleApp
{
    static class Program
    {
        private static string wsNameWithXp = "ws1261";
        private static string wsNameWithWin7 = "ws555a";
        private static string wsNameWithWin10 = "ws1674";
        private static string ws007 = "ws007";

        private static string[] wsNames = new string[] { wsNameWithXp, wsNameWithWin7, wsNameWithWin10, ws007 };

        private static IInfoCollectorService wmiInfoCollectorService = new WmiInfoCollectorService();

        static async Task Main(string[] args)
        {
            Console.Write("Сейчас будет собираться информация\nНажмите на клавишу...");
            Console.ReadLine();
            await GetInfoFromService(wsNames);
        }
        
        static async Task GetInfoFromService(string[] wsNames)
        {
            var result = await wmiInfoCollectorService.GetComputersInfo(wsNames);
            result.ForEach(PrintInfo);
        }

        private static void PrintInfo(WorkStationModel model)
        {
            Console.WriteLine($"{model.WsName}");
            Console.WriteLine($"Процессор: {model.Cpu.Name}. Частота: {model.Cpu.Frequency} MHz");
            Console.WriteLine($"Материнская плата: {model.Motherboard.Model} - Производитель: {model.Motherboard.Manufacturer}");
            Console.WriteLine($"Пользователь: {model.SystemUser.Login}");
            Console.WriteLine($"ОС: {model.OperatingSystem.Name}");
            Console.WriteLine("Видео:");
            model.VideoAdapters.ForEach(x =>
            {
                Console.WriteLine($"\t{x.Name}");
                Console.WriteLine($"\t{x.Memory / Math.Pow(1024, 2)} Mb");
            });
            Console.WriteLine("ОЗУ:");
            model.Rams.ForEach(x =>
            {
                Console.WriteLine($"\t{x.Capacity / Math.Pow(1024, 2)} Mb");
            });
            Console.WriteLine("Разделы:");
            model.PartitionDisks.ForEach(x =>
            {
                Console.WriteLine($"\t{x.Name}");
                Console.WriteLine($"\t{x.Size / Math.Pow(1024, 3):0.##} Gb");
            });

            Console.WriteLine();
        }
    }
}
