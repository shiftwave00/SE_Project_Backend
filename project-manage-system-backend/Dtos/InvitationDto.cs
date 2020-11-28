using project_manage_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Dtos
{
    public class InvitationDto
    {
        public string ApplicantId { get; set; } 
        public int ProjectId { get; set; }
    }
    public class ReplyToInvitationDto
    {
        public int InvitationId { get; set; }
    }
}
