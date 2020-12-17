using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using project_manage_system_backend.Models;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace project_manage_system_backend.Hubs
{
    public class NotifyHub: Hub<INotifyHub>
    {
        public async Task SubscribeNotification(string userAccount)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userAccount);
        }

        public async Task UnsubscribeNotification(string userAccount)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userAccount);
        }
    }
}
