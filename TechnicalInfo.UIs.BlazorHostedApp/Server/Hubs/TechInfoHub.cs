using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TechnicalInfo.Infrastructure.Interfaces;

namespace TechnicalInfo.UIs.BlazorHostedApp.Server.Hubs
{
    public class TechInfoHub : Hub
    {
        private readonly IInfoCollectorService infoCollectorService;

        public TechInfoHub(IInfoCollectorService infoCollectorService)
        {
            this.infoCollectorService = infoCollectorService ?? throw new NullReferenceException(nameof(infoCollectorService));
        }

        public async Task GetInfo(string[] wsNames)
        {
            var result = await infoCollectorService.GetComputersInfo(wsNames);
            await Clients.Caller.SendAsync("ReceiveInfo", result);
        }
    }
}
