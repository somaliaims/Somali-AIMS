using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IOrganizationMergeService
    {
        ICollection<OrganizationMergeRequests> GetForUser(int userId);
    }


    public class OrganizationMergeService
    {
        AIMSDbContext context;

        public OrganizationMergeService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public IEnumerable<OrganizationMergeRequests> GetForUser(int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationMergeRequests> mergeRequests = new List<OrganizationMergeRequests>();
                var userData = unitWork.UserRepository.GetOne(u => u.Id == userId);
                int organizationId = 0;
                if (userData != null)
                {
                    organizationId = userData.OrganizationId;
                }
                return mergeRequests;
            }
        }
    }
}
