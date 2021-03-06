﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMS.Services
{
    public interface IContactService
    {
        /// <summary>
        /// Gets list of emails for project users
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<EmailAddress> GetProjectUsersEmails(int id);

        /// <summary>
        /// Gets list of emails for manager/admin users
        /// </summary>
        /// <returns></returns>
        IEnumerable<EmailAddress> GetManagerUsersEmails();
    }

    public class ContactService : IContactService
    {
        AIMSDbContext context;

        public ContactService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public IEnumerable<EmailAddress> GetProjectUsersEmails(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<EmailAddress> emailAddressList = new List<EmailAddress>();
                IEnumerable<int> projectFunderIds = unitWork.ProjectFundersRepository.GetProjection(p => p.ProjectId == id, p => p.FunderId);
                IEnumerable<int> projectImplementerIds = unitWork.ProjectImplementersRepository.GetProjection(p => p.ProjectId == id, p => p.ImplementerId);
                int userId = (int)unitWork.ProjectRepository.GetProjection(p => p.Id == id, p => p.CreatedById).FirstOrDefault();
                if (userId > 0)
                {
                    var email = unitWork.UserRepository.GetProjection(u => u.Id == id, u => u.Email).FirstOrDefault();
                    if (email != null)
                    {
                        emailAddressList.Add(new EmailAddress()
                        {
                            Email = email
                        });
                    }
                }

                projectFunderIds = projectFunderIds.Union(projectImplementerIds);
                var emails = unitWork.UserRepository.GetProjection(u => projectFunderIds.Contains(u.OrganizationId), u => u.Email);
                foreach (var email in emails)
                {
                    var emailExists = (from e in emailAddressList
                                       where e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                       select e).FirstOrDefault();

                    if (emailExists == null)
                    {
                        emailAddressList.Add(new EmailAddress()
                        {
                            Email = email
                        });
                    }
                }
                if (emailAddressList.Count == 0)
                {
                    var project = unitWork.ProjectRepository.GetWithInclude(p => p.Id == id, new string[] { "CreatedBy" }).FirstOrDefault();
                    if (project != null)
                    {
                        if (project.CreatedBy != null)
                        {
                            emailAddressList.Add(new EmailAddress()
                            {
                                Email = project.CreatedBy.Email
                            });
                        }
                    }
                }
                return emailAddressList;
            }
        }

        public IEnumerable<EmailAddress> GetManagerUsersEmails()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<EmailAddress> emailAddressList = new List<EmailAddress>();
                var emails = unitWork.UserRepository.GetProjection(u => (u.UserType == UserTypes.Manager || u.UserType == UserTypes.SuperAdmin), u => u.Email);
                foreach (var email in emails)
                {
                    emailAddressList.Add(new EmailAddress()
                    {
                        Email = email
                    });
                }
                return emailAddressList;
            }
        }
    }
}
