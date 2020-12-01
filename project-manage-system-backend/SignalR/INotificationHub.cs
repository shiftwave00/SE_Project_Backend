using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using project_manage_system_backend.Models;

namespace project_manage_system_backend.SignalR
{
    public interface INotificationHub
    {
        Task sendInvitationToClient();
    }
}
