using Microsoft.AspNetCore.SignalR;
using project_manage_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.SignalR
{
    public class NotificationHub : Hub<INotificationHub>
    {
        public async Task SendNotification(User applicant)
        {
            await Clients.All.sendInvitationToClient();
        }
    }
}
