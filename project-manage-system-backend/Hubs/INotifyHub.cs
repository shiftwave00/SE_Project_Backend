using Microsoft.AspNetCore.Authorization;
using project_manage_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Hubs
{
    public interface INotifyHub
    {
        Task ReceiveNotification();

        Task SubscribeNotification();

        Task UnsubscribeNotification();
    }
}
