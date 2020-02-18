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
        private static string wsNameWithXp = "ws1261";
        private static string wsNameWithWin7 = "ws555a";
        private static string wsNameWithWin10 = "ws1674";

        private static IInfoCollector infoCollector = new WmiInfoCollector();

        static async Task Main(string[] args)
        {
            await GetData(wsNameWithXp);
            await GetData(wsNameWithWin7);
            await GetData(wsNameWithWin10);
        }

        static async Task GetData(string wsName)
        {
            var cpu = await infoCollector.Get<CpuModel>(wsName);
            var motherboard = await infoCollector.Get<MotherboardModel>(wsName);
            var userName = await infoCollector.Get<SystemUserName>(wsName);
            var operatingSystem = await infoCollector.Get<OperatingSystemModel>(wsName);
            var videoAdapter = await infoCollector.Get<VideoAdapterModel>(wsName);

            Console.WriteLine($"{wsName}:"); 
            Console.WriteLine($"Процессор: {cpu.FirstOrDefault()?.Name}. Частота: {cpu.FirstOrDefault()?.Frequency} MHz");
            Console.WriteLine($"Материнская плата: {motherboard.FirstOrDefault()?.Model} - Производитель: {motherboard.FirstOrDefault()?.Manufacturer}");
            Console.WriteLine($"Пользователь: {userName.FirstOrDefault()?.Login}");
            Console.WriteLine($"ОС: {operatingSystem.FirstOrDefault()?.Name}");
            Console.WriteLine($"Видео: {videoAdapter.FirstOrDefault()?.Name}");
            Console.WriteLine();
        }
    }
}
