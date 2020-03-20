using SomeResult;
using System;
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
        private static string wsNameWithXp = "ws1261";
        private static string wsNameWithWin7 = "ws555a";
        private static string wsNameWithWin10 = "ws1674";
        private static string ws007 = "ws007";
        private static string notExistsWs = "ws9999";
        private static string anotherAdminPassword = "ws2032";

        private static string[] wsNames = new string[] { wsNameWithXp, wsNameWithWin7, wsNameWithWin10, ws007, notExistsWs, anotherAdminPassword };
        //private static string[] wsNames = new string[] { wsNameWithXp };

        private static IInfoCollectorService wmiInfoCollectorService = new WmiInfoCollectorService();
        private static IInfoWriter infoWriter = new ConsoleWriter();
        private static IInfoWriter excelInfoWriter = new ExcelInfoWriter();

        static async Task Main(string[] args)
        {
            await GetInfoFromService(wsNames);
        }
        
        static async Task GetInfoFromService(string[] wsNames)
        {
            var result = await wmiInfoCollectorService.GetComputersInfo(wsNames);
            result.ForEach(PrintInfo);
            await excelInfoWriter.Write(result.ToArray());
        }

        private static async void PrintInfo(Result<WorkStationModel, string> model)
        {
            await infoWriter.Write(model);
        }
    }
}
