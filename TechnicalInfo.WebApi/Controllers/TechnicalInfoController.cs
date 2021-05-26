using Microsoft.AspNetCore.Mvc;
using SomeResult;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Infrastructure.ExcelWriter;
using TechnicalInfo.Infrastructure.Interfaces;

namespace TechnicalInfo.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/techinfo")]
    public class TechnicalInfoController
    {
        private readonly IInfoCollectorService infoCollectorService;
        private readonly IWebHostEnvironment _appEnvironment;

        public TechnicalInfoController(IInfoCollectorService infoCollectorService, IWebHostEnvironment appEnvironment)
        {
            this.infoCollectorService = infoCollectorService;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public async Task<Result<WorkStationModel, string>> Get([FromQuery(Name = "name")]string wsname)
        {
            return (await infoCollectorService.GetComputersInfo(new string[] { wsname })).FirstOrDefault();
        }

        [HttpGet("many")]
        public Task<List<Result<WorkStationModel, string>>> GetMany([FromQuery(Name = "name")]string[] wsnames)
        {
            return infoCollectorService.GetComputersInfo(wsnames);
        }

        [HttpGet("excel")]
        public async Task<IActionResult> GetInfoInExcelFile([FromQuery(Name = "name")]string[] wsnames)
        {
            var result =  await infoCollectorService.GetComputersInfo(wsnames);
            var excelWriter = new ExcelInfoWriter();
            await excelWriter.Write(result.ToArray());
            var fileInfo = excelWriter.GetFileInfo();
            string filePath = fileInfo.FullName;

            var bytes = await File.ReadAllBytesAsync(filePath);
            File.Delete(filePath);
            
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return new FileContentResult(bytes, contentType)
            {
                FileDownloadName = fileInfo.Name
            };
        }
    }
}
