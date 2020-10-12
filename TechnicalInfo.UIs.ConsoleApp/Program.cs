using SomeResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Infrastructure.ConsoleWriter;
using TechnicalInfo.Infrastructure.ExcelWriter;
using TechnicalInfo.Infrastructure.Interfaces;
using TechnicalInfo.Infrastructure.Wmi;

namespace TechnicalInfo.UIs.ConsoleApp
{
    static class Program
    {
        //private static string wsNameWithXp = "ws1261";
        //private static string wsNameWithWin7 = "ws555a";
        //private static string wsNameWithWin10 = "ws1674";
        //private static string ws007 = "ws007";
        //private static string notExistsWs = "ws9999";
        //private static string anotherAdminPassword = "ws2032";

        //private static string[] wsNames = new string[] { wsNameWithXp, wsNameWithWin7, wsNameWithWin10, ws007, notExistsWs, anotherAdminPassword };
        //private static string[] wsNames = new string[] { ws007 };

        private static IInfoCollectorService wmiInfoCollectorService = new WmiInfoCollectorService();
        private static IInfoWriter infoWriter = new ConsoleWriter();
        private static IInfoWriter excelInfoWriter = new ExcelInfoWriter();
        private static string[] wsNames;

        // max Ws2731
        private static int maxWsName = 170;

        static async Task Main(string[] args)
        {
            if(args.Length == 0)
            {
                InitialDialog();
            }
            else
            {
                wsNames = args;
            }

            await GetInformation();

            Console.WriteLine();
            Console.Write("Для выхода нажмите любую клавишу...");
            Console.ReadKey(false);
        }
        
        static async Task GetInfoFromService(string[] wsNames)
        {
            if (wsNames != null && wsNames.Length > 0)
            {
                Console.WriteLine("Подождите идет сбор данных");
                var result = await wmiInfoCollectorService.GetComputersInfo(wsNames);

                if (result.Count >= 10)
                {
                    result.Take(10).ToList().ForEach(PrintInfo);
                }
                else
                {
                    result.ForEach(PrintInfo);
                }


                await excelInfoWriter.Write(result.ToArray());
                Console.WriteLine("Данные собраны");
            }
            else
            {
                Console.WriteLine("Нет имен компьютеров");
            }
            
        }

        private static async void PrintInfo(Result<WorkStationModel, string> model)
        {
            await infoWriter.Write(model);
        }

        private static void GetWsNamesFromUserInput()
        {
            Console.Clear();
            Console.Write("Введите через пробел имена компьютеров: ");
            wsNames = Console.ReadLine().Split(' ');
        }

        private static void GetAllWsNames()
        {
            var newListWsNames = new List<string>();
            for (int i = 0; i <= maxWsName; i++)
            {
                if (i < 10) 
                {
                    newListWsNames.Add($"ws00{i}");
                }

                if (i >= 10 && i < 100)
                {
                    newListWsNames.Add($"ws0{i}");
                }

                if (i >= 100)
                {
                    newListWsNames.Add($"ws{i}");
                }
            }

            wsNames = newListWsNames.ToArray();
        }

        private static async Task GetInformation()
        {
            await GetInfoFromService(wsNames);
        }

        private static void InitialDialog()
        {
            Console.WriteLine("1. Самостоятельный ввод имен компьютеров.");
            Console.WriteLine($"2. Сбор сведений компьютеров с Ws000 по Ws{maxWsName}.");
            Console.Write("Ваш выбор: ");
            switch (Console.ReadLine().ToLower())
            {
                case "1":
                    GetWsNamesFromUserInput();
                    break;
                case "2":
                    GetAllWsNames();
                    break;
                default:
                    Console.WriteLine("Нет такого пункта.");
                    break;
            }
        }
    }
}
