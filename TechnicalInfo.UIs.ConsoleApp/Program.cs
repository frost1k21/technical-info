using SomeResult;
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
        private static string notExistsWs = "ws9999";
        private static string anotherAdminPassword = "ws2032";

        //private static string[] wsNames = new string[] { wsNameWithXp, wsNameWithWin7, wsNameWithWin10, ws007, notExistsWs, anotherAdminPassword };
        private static string[] wsNames = new string[] { wsNameWithWin7, ws007 };

        private static IInfoCollectorService wmiInfoCollectorService = new WmiInfoCollectorService();

        static async Task Main(string[] args)
        {
            await GetInfoFromService(wsNames);
        }
        
        static async Task GetInfoFromService(string[] wsNames)
        {
            var result = await wmiInfoCollectorService.GetComputersInfo(wsNames);
            result.ForEach(PrintInfo);
        }

        private static void PrintInfo(Result<WorkStationModel, string> model)
        {
            if(model.Error != null)
            {
                Console.WriteLine(model.Error);
            }
            else
            {
                Console.WriteLine($"{model.Success.WsName}");
                Console.WriteLine($"Процессор: {model.Success.Cpu.Name}. Частота: {model.Success.Cpu.Frequency} MHz");
                Console.WriteLine($"Материнская плата: {model.Success.Motherboard.Model} - Производитель: {model.Success.Motherboard.Manufacturer}");
                Console.WriteLine($"Пользователь: {model.Success.SystemUser.Login}");
                Console.WriteLine($"ОС: {model.Success.OperatingSystem.Name}");
                Console.WriteLine("Видео:");
                model.Success.VideoAdapters.ForEach(x =>
                {
                    Console.WriteLine($"\t{x.Name}");
                    Console.WriteLine($"\t{x.Memory / Math.Pow(1024, 2)} Mb");
                });
                Console.WriteLine("ОЗУ:");
                model.Success.Rams.ForEach(x =>
                {
                    Console.WriteLine($"\t{x.Capacity / Math.Pow(1024, 2)} Mb");
                });
                Console.WriteLine("Разделы:");
                model.Success.PartitionDisks.ForEach(x =>
                {
                    Console.WriteLine($"\t{x.Name}");
                    Console.WriteLine($"\t{x.Size / Math.Pow(1024, 3):0.##} Gb");
                });
                Console.WriteLine("Мониторы:");
                model.Success.Monitors.ForEach(x =>
                {
                    Console.WriteLine($"\t{x.Manufacturer}");
                    Console.WriteLine($"\t{x.Model}");
                    Console.WriteLine($"\t{x.MonitorConnectionPortName}");
                });
            }
            Console.WriteLine();
        }
    }
}
