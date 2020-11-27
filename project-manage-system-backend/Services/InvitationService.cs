using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class InvitationService : BaseService
    {
        public InvitationService(PMSContext dbContext) : base(dbContext) { }

        public void CreateInvitation(User inviter, User applicant, Project project)
        {
            var invitation = new Invitation
            {
                Inviter = inviter,
                Applicant = applicant,
                InvitedProject = project,
                IsAgreed = false
            };

            _dbContext.invitations.Add(invitation);

            if (_dbContext.SaveChanges() == 0)
                throw new Exception("Create invitation fail!");
        }
    }
}
