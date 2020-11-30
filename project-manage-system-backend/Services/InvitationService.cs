using Microsoft.EntityFrameworkCore;
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

        public Invitation CreateInvitation(User inviter, User applicant, Project project)
        {
            var invitation = new Invitation
            {
                Inviter = inviter,
                Applicant = applicant,
                InvitedProject = project,
                IsAgreed = true
            };

            return invitation;
        }

        public List<Invitation> GetInvitations(User user)
        {
            List<Invitation> invitations = _dbContext.Invitations
                                                     .Include(i => i.Inviter)
                                                     .Include(i => i.InvitedProject)
                                                     .Where(i => i.Applicant.Equals(user)).ToList();
            return invitations;
        }

        public void AddInvitation(Invitation invitation)
        {
            _dbContext.Invitations.Add(invitation);

            if (_dbContext.SaveChanges() == 0)
                throw new Exception("Create invitation fail!");
        }

        public bool IsInvitationExist(Invitation invitation)
        {
            var Invitation = _dbContext.Invitations.Where
                (i => 
                    i.Inviter.Equals(invitation.Inviter) && i.Applicant.Equals(invitation.Applicant) && i.InvitedProject.Equals(invitation.InvitedProject)
                );
            if (Invitation.Count() > 0) return true;
            return false;
        }
        
        public void DeleteInvitation(Invitation invitation)
        {
            _dbContext.Invitations.Remove(invitation);

            if (_dbContext.SaveChanges() == 0)
            {
                throw new Exception("Delete invitation fail!");
            }
        }

        public Invitation GetInvitation(int id)
        {
            var invitation = _dbContext.Invitations.Include(u => u.InvitedProject).Include(u => u.Inviter).Include(u => u.Applicant).Where(i => i.ID.Equals(id)).First();
            return invitation;
        }
    }
}
