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
        IEnumerable<OrganizationMergeRequests> GetForUser(int userId);

        /// <summary>
        /// Adds new request for merging organizations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddAsync(MergeOrganizationModel model);

        /// <summary>
        /// Approves merge request for organizations
        /// </summary>
        /// <returns></returns>
        ActionResponse ApproveMergeRequest(int requestId, int userOrgId);

        /// <summary>
        /// Rejects merge request for organizations
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userOrgId"></param>
        /// <returns></returns>
        ActionResponse RejectRequest(int requestId, int userOrgId, string email);

        /// <summary>
        /// Approves requests older two weeks
        /// </summary>
        /// <returns></returns>
        List<MergeOrganizationsRequest> GetRequestOlderTwoWeeks();
    }


    public class OrganizationMergeService : IOrganizationMergeService
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
                                NewName = model.NewName,
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
                            message += mHelper.OrganizationsMergeRequest(organizationNames, model.NewName);
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
                    var requests = unitWork.OrganizationsToMergeRepository.GetWithInclude(r => r.OrganizationId != 0 && r.Request.IsApproved == false, new string[] { "Organization", "Request" });
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

        public ActionResponse ApproveMergeRequest(int requestId, int userOrgId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var request = unitWork.OrganizationMergeRequestsRepository.GetWithInclude(r => r.Id == requestId && r.IsApproved == false, new string[] { "Organizations" }).FirstOrDefault();
                if (request == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization merge request");
                    response.Success = false;
                    return response;
                }

                var orgId = (from o in request.Organizations
                              where o.OrganizationId == userOrgId
                              select o.OrganizationId).FirstOrDefault();

                if (orgId == 0)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.UnAuthorizedOrganizationsMerge();
                    response.Success = false;
                    return response;
                }

                request.IsApproved = true;
                unitWork.OrganizationMergeRequestsRepository.Update(request);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse RejectRequest(int requestId, int userOrgId, string userEmail)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var request = unitWork.OrganizationMergeRequestsRepository.GetWithInclude(r => r.Id == requestId && r.IsApproved == false, new string[] { "Organizations" }).FirstOrDefault();
                if (request == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization merge request");
                    response.Success = false;
                    return response;
                }

                var orgId = (from o in request.Organizations
                             where o.OrganizationId == userOrgId
                             select o.OrganizationId).FirstOrDefault();

                if (orgId == 0)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.UnAuthorizedOrganizationsMerge();
                    response.Success = false;
                    return response;
                }

                var organizationNames = (from o in request.Organizations
                                         select o.Organization.OrganizationName).ToList();

                var orgIds = (from o in request.Organizations
                              select o.OrganizationId);

                string subject = "", message = "", footerMessage = "";
                var users = unitWork.UserRepository.GetManyQueryable(u => orgIds.Contains(u.OrganizationId));
                if (users != null)
                {
                    List<EmailAddress> emailsList = (from u in users
                                                     select new EmailAddress()
                                                     {
                                                         Email = u.Email
                                                     }).ToList();

                    if (emailsList.Count > 0)
                    {
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.MergeOrganizationRejected);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }

                        mHelper = new MessageHelper();
                        message = "<p>Request rejected by: " + userEmail + "</p>";
                        message += mHelper.OrganizationsMergeRequest(organizationNames, request.NewName);
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
                        IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                        emailHelper.SendEmailToUsers(emailsList, subject, "", message, footerMessage);
                    }
                }

                unitWork.OrganizationMergeRequestsRepository.Delete(request);
                unitWork.Save();
                return response;
            }
        }

        public List<MergeOrganizationsRequest> GetRequestOlderTwoWeeks()
        {
            List<MergeOrganizationsRequest> mergeRequestOlderTwoWeeks = new List<MergeOrganizationsRequest>();
            var unitWork = new UnitOfWork(context);
            DateTime dateTwoWeeksBefore = DateTime.Now.AddDays(-14);
            var requests = unitWork.OrganizationMergeRequestsRepository.GetWithInclude(r => r.Dated.Date <= dateTwoWeeksBefore.Date && r.IsApproved == false, new string[] { "Organizations" });
            foreach(var request in requests)
            {
                mergeRequestOlderTwoWeeks.Add(new MergeOrganizationsRequest()
                {
                    RequestId = request.Id,
                    OrganizationIds = (request.Organizations.Count > 0 ) ? request.Organizations.Select(o => o.OrganizationId).ToList() : new List<int>()
                });
            }
            return mergeRequestOlderTwoWeeks;
        }
    }
}
