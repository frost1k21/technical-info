using Microsoft.AspNetCore.Mvc;
using SomeResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalInfo.Domain.Models;
using TechnicalInfo.Infrastructure.Interfaces;

namespace TechnicalInfo.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/techinfo")]
    public class TechnicalInfoController
    {
        private readonly IInfoCollectorService infoCollectorService;

        public TechnicalInfoController(IInfoCollectorService infoCollectorService)
        {
            this.infoCollectorService = infoCollectorService;
        }

        [HttpGet]
        public async Task<Result<WorkStationModel, string>> Get([FromQuery]string wsname)
        {
            return (await infoCollectorService.GetComputersInfo(new string[] { wsname })).FirstOrDefault();
        }
    }
}
