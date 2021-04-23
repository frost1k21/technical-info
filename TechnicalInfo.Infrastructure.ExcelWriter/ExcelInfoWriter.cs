using OfficeOpenXml;
using SomeResult;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Infrastructure.Interfaces;
using OfficeOpenXml.Style;
using System.Collections.Generic;

namespace TechnicalInfo.Infrastructure.ExcelWriter
{
    public class ExcelInfoWriter : IInfoWriter
    {
        private FileInfo _xlFile;

        public Task Write(params Result<WorkStationModel, string>[] workStationModels)
        {
            var newLine = Environment.NewLine;
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Технические характеристики");
                ExcelWorksheet worksheetError = package.Workbook.Worksheets.Add("Ошибки");
                //Add the headers
                worksheet.Cells[1, 1].Value = "Имя компьютера";
                worksheet.Cells[1, 2].Value = "Материнская плата";
                worksheet.Cells[1, 3].Value = "Процессор";
                worksheet.Cells[1, 4].Value = "Память";
                worksheet.Cells[1, 5].Value = "Видео";
                worksheet.Cells[1, 6].Value = "HDD";
                worksheet.Cells[1, 7].Value = "Мониторы";
                worksheet.Cells[1, 8].Value = "Пользователи";
                worksheet.Cells[1, 9].Value = "ОС";

                //установка стилей
                worksheet.Cells["A1:I1"].Style.Font.Bold = true;
                worksheet.Cells["A1:I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:I1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:I1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:I1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                var currnetRow = 2;
                var errorCount = 1;
            
                foreach (var item in workStationModels)
                {
                    if(string.IsNullOrEmpty(item.Error))
                    {
                        worksheet.Cells[$"A{currnetRow}"].Value = item.Success.WsName;
                        worksheet.Cells[$"B{currnetRow}"].Value = $"{item.Success.Motherboard.Model}{newLine}Производитель:{newLine}{item.Success.Motherboard.Manufacturer}";
                        worksheet.Cells[$"C{currnetRow}"].Value = $"{item.Success.Cpu.Name}{newLine}Частота: {item.Success.Cpu.Frequency} MHz";

                        worksheet.Cells[$"D{currnetRow}"].Value = MemoryFormatString(newLine, item);

                        worksheet.Cells[$"E{currnetRow}"].Value = string.Join($"{newLine}", item.Success.VideoAdapters.Select(va => $"{va.Name}{newLine}{va.Memory / Math.Pow(1024, 2)} Mb"));

                        worksheet.Cells[$"F{currnetRow}"].Value = string.Join($"{newLine}", item.Success.DiskDrives.Select(dd => $"{dd.Name}{newLine}{dd.Size / Math.Pow(1024, 3):0.##} Gb"));

                        if (item.Success.Monitors != null)
                            worksheet.Cells[$"G{currnetRow}"].Value = string.Join($"{newLine}", item.Success.Monitors.Select(mon => $"{mon.Manufacturer}{newLine}{mon.Model}{newLine}{mon.MonitorConnectionPortName}"));

                        worksheet.Cells[$"H{currnetRow}"].Value = $"{item.Success.SystemUser.Login}";
                        worksheet.Cells[$"I{currnetRow}"].Value = $"{item.Success.OperatingSystem.Name}{newLine}{item.Success.OperatingSystem.OsArchitecture}{newLine}SP{item.Success.OperatingSystem.ServicePack}";

                        worksheet.Cells[$"A{currnetRow}:I{currnetRow}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[$"A{currnetRow}:I{currnetRow}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{currnetRow}:I{currnetRow}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{currnetRow}:I{currnetRow}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{currnetRow}:I{currnetRow}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[$"A{currnetRow}:I{currnetRow}"].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

                        worksheet.Cells[$"A{currnetRow}:G{currnetRow}"].Style.WrapText = true;
                        worksheet.Cells[$"I{currnetRow}"].Style.WrapText = true;
                        currnetRow++;
                    }
                    else
                    {
                        worksheetError.Cells[$"A{errorCount}"].Value = item.Error;

                        worksheetError.Cells[$"A{errorCount}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheetError.Cells[$"A{errorCount}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheetError.Cells[$"A{errorCount}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheetError.Cells[$"A{errorCount}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheetError.Cells[$"A{errorCount}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheetError.Cells[$"A{errorCount}"].Style.Fill.BackgroundColor.SetColor(Color.LightPink);

                        worksheetError.Cells[$"A{errorCount}"].Style.WrapText = true;
                        errorCount++;
                    }
                }

                worksheet.Cells.AutoFitColumns(30);
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 32;
                worksheet.Column(3).Width = 32;
                worksheet.Column(4).Width = 22;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 35;
                worksheet.Column(7).Width = 30;
                worksheet.Column(9).Width = 30;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                worksheetError.Column(1).Width = 30;
                worksheetError.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheetError.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                var name = DateTime.Now.ToString("dd-MM-yyyy HH.mm.ss");
                var xlFile = new FileInfo($"{name}.xlsx");
                // save our new workbook in the output directory and we are done!
                package.SaveAs(xlFile);
                _xlFile = xlFile;
            }

            return Task.CompletedTask;
        }

        private static string MemoryFormatString(string separator, Result<WorkStationModel, string> item)
        {
            var totalMemory = item.Success.Rams.Sum(r => r.Capacity / Math.Pow(1024, 2));
            var totalMemoryString = $"Весь объем: {totalMemory}Mb";
            var ramsStrings = item.Success.Rams.Select(r => 
            {
                var memType = "";
                if (!string.IsNullOrEmpty(r.MemType)) memType = $"Тип памяти: {r.MemType}{separator}";
                return $"Объем: {r.Capacity / Math.Pow(1024, 2)}Mb{separator}{memType}Частота: {r.Speed} MHz";
            });
            var stringsList = new List<string>();
            stringsList.Add(totalMemoryString);
            stringsList.AddRange(ramsStrings);
            return string.Join($"{separator}", stringsList);
        }

        public FileInfo GetFileInfo()
        {
            Console.WriteLine(_xlFile.FullName);
            return _xlFile;
        }
    }

}
