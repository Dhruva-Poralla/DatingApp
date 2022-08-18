using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalIR
{
    [Authorize]
    public class PresenceHub:Hub
    {
        private readonly PresenceTacker _tacker;
        public PresenceHub(PresenceTacker tacker)
        {
            _tacker = tacker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline=await _tacker.UserConnected(Context.User.GetUserName(),Context.ConnectionId);
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

            var currentUsers = await _tacker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception){
            var isOffline = await _tacker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
            
            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            await base.OnDisconnectedAsync(exception);
        }
    }
}