using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AIMS.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AIMS.Services
{
    public interface IOrganizationMergeService
    {
        /// <summary>
        /// Gets merge organization requests for current user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ICollection<OrganizationMergeRequests> GetForUser(int userId);

        /// <summary>
        /// Adds new request for merging organizations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddAsync(MergeOrganizationModel model);
    }


    public class OrganizationMergeService
    {
        AIMSDbContext context;

        public OrganizationMergeService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public async Task<ActionResponse> AddAsync(MergeOrganizationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();

                if (model.Ids.Count < 2)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.InvalidOrganizationMerge();
                    response.Success = false;
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var organizationType = unitWork.OrganizationTypesRepository.GetOne(t => t.Id == model.OrganizationTypeId);
                if (organizationType == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization type");
                    response.Success = false;
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => model.Ids.Contains(o.Id));
                var orgIds = (from org in organizations
                              select org.Id).ToList<int>();
                var organizationNames = (from org in organizations
                                         select org.OrganizationName).ToList<string>();

                if (model.Ids.Count < orgIds.Count)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization/s");
                    response.Success = false;
                    return response;
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var users = await unitWork.UserRepository.GetManyQueryableAsync(u => (orgIds.Contains(u.OrganizationId)));
                        List<EmailAddress> emailsList = new List<EmailAddress>();
                        foreach (var user in users)
                        {
                            emailsList.Add(new EmailAddress() { Email = user.Email });
                        }

                        if (emailsList.Count == 0)
                        {
                            response.ReturnedId = 1;
                            response.Message = "Merge";
                        }
                        else
                        {
                            var request = unitWork.OrganizationMergeRequestsRepository.Insert(new EFOrganizationMergeRequests()
                            {
                                IsApproved = false,
                                Dated = DateTime.Now,
                                OrganizationIdsJson = string.Join("-", orgIds)
                            });
                            unitWork.Save();

                            foreach (var organization in organizations)
                            {
                                unitWork.OrganizationsToMergeRepository.Insert(new EFOrganizationsToMerge()
                                {
                                    Request = request,
                                    Organization = organization
                                });    
                            }
                            unitWork.Save();

                            ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                            var smtpSettings = smtpService.GetPrivate();
                            SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();
                            if (smtpSettings != null)
                            {
                                smtpSettingsModel.Host = smtpSettings.Host;
                                smtpSettingsModel.Port = smtpSettings.Port;
                                smtpSettingsModel.Username = smtpSettings.Username;
                                smtpSettingsModel.Password = smtpSettings.Password;
                                smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                                smtpSettingsModel.SenderName = smtpSettings.SenderName;
                            }

                            string subject = "", message = "", footerMessage = "";
                            var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.NewProjectToOrg);
                            if (emailMessage != null)
                            {
                                subject = emailMessage.Subject;
                                message = emailMessage.Message;
                                footerMessage = emailMessage.FooterMessage;
                            }

                            mHelper = new MessageHelper();
                            message += mHelper.OrganizationsMergeRequest(organizationNames);
                            IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                            emailHelper.SendEmailToUsers(emailsList, subject, "", message, footerMessage);
                        }
                    }
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
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
                    var requests = unitWork.OrganizationsToMergeRepository.GetWithInclude(r => r.OrganizationId != 0, new string[] { "Organization" });
                    var userRequests = (from req in requests
                                        where req.OrganizationId == organizationId
                                        select req.RequestId);
                    if (userRequests.Any())
                    {
                        var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => o.Id != 0);
                        foreach(int requestId in userRequests)
                        {
                            var organizationIds = (from rq in requests
                                                   where rq.RequestId == requestId
                                                   select rq.OrganizationId);
                            var organizationsToMerge = (from org in organizations
                                                        where organizationIds.Contains(org.Id)
                                                        select new OrganizationsToMerge()
                                                        {
                                                             Id = org.Id,
                                                             OrganizationName = org.OrganizationName
                                                        }).ToList();

                            mergeRequests.Add(new OrganizationMergeRequests()
                            {
                                Id = requestId,
                                Organizations = organizationsToMerge
                            });
                        }
                    }
                }
                return mergeRequests;
            }
        }
    }
}
