using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HangfireSignalR.Hubs
{
    [Authorize]
    public class JobProgressHub : Hub
    {
        public string UserName => Context.User.Identity.Name;

        public override Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, UserName);
            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, UserName);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
