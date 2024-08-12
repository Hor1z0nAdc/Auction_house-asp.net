using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace Auction_house {

    public class StateHub : Hub 
    {
        public async Task SendUpdate(string id, string ajánlat)
        {   
            Console.WriteLine("send registered: ajánlat = " + ajánlat);
            await Clients.All.SendAsync("ReceiveUpdate", id, ajánlat);
        }
    }
}