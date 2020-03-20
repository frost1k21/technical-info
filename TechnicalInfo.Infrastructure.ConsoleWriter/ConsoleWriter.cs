using SomeResult;
using System;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Infrastructure.Interfaces;

namespace TechnicalInfo.Infrastructure.ConsoleWriter
{
    public class ConsoleWriter : IInfoWriter
    {
        public Task Write(params Result<WorkStationModel, string>[] workStationModels)
        {
            foreach (var model in workStationModels)
            {
                if (model.Error != null)
                {
                    Console.WriteLine(model.Error);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"{model.Success.WsName}");
                    Console.WriteLine($"Процессор: {model.Success.Cpu.Name}. Частота: {model.Success.Cpu.Frequency} MHz");
                    Console.WriteLine($"Материнская плата: {model.Success.Motherboard.Model} - Производитель: {model.Success.Motherboard.Manufacturer}");
                    Console.WriteLine($"Пользователь: {model.Success.SystemUser.Login}");
                    Console.WriteLine($"ОС: {model.Success.OperatingSystem.Name} {model.Success.OperatingSystem.OsArchitecture} ServicePack: {model.Success.OperatingSystem.ServicePack}");
                    Console.WriteLine("Видео:");
                    model.Success.VideoAdapters.ForEach(x =>
                    {
                        Console.WriteLine($"\t{x.Name}");
                        Console.WriteLine($"\t{x.Memory / Math.Pow(1024, 2)} Mb");
                    });
                    Console.WriteLine("ОЗУ:");
                    model.Success.Rams.ForEach(x =>
                    {
                        Console.WriteLine($"\tОбъем: {x.Capacity / Math.Pow(1024, 2)} Mb");
                        Console.WriteLine($"\tСкорость: {x.Speed} MHz");
                    });
                    Console.WriteLine("Разделы:");
                    model.Success.DiskDrives.ForEach(x =>
                    {
                        Console.WriteLine($"\t{x.Name}");
                        Console.WriteLine($"\t{x.Size / Math.Pow(1024, 3):0.##} Gb");
                    });
                    Console.WriteLine("Мониторы:");
                    model.Success.Monitors?.ForEach(x =>
                    {
                        Console.WriteLine($"\t{x.Manufacturer}");
                        Console.WriteLine($"\t{x.Model}");
                        Console.WriteLine($"\t{x.MonitorConnectionPortName}");
                    });
                }
                Console.WriteLine();
                
            }

            return Task.CompletedTask;
        }
    }
}
