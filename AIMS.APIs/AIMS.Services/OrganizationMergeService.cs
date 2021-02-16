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
using System.Security.Cryptography;

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
        /// Gets list of organizations that are currently requested for merge
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrganizationMiniView> GetOrganizationsAppliedForMerge();

        /// <summary>
        /// Adds new request for merging organizations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddAsync(MergeOrganizationModel model, int userId);

        /// <summary>
        /// Approves merge request for organizations
        /// </summary>
        /// <returns></returns>
        Task<ActionResponse> ApproveMergeRequestAsync(int requestId, int userOrgId);

        /// <summary>
        /// Rejects merge request for organizations
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userOrgId"></param>
        /// <returns></returns>
        ActionResponse RejectRequest(int requestId, int userOrgId, string email);

        /// <summary>
        /// 

        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Task<ActionResponse> MergeOrganizations(int requestId);

        /// <summary>
        /// Merge all the provided organizations
        /// </summary>
        /// <returns></returns>
        Task<ActionResponse> MergeOrganizationsAuto(List<int> ids);

        /// <summary>
        /// Approves requests older two weeks
        /// </summary>
        /// <returns></returns>
        List<MergeOrganizationsRequest> GetTwoWeeksOlderRequests();
    }


    public class OrganizationMergeService : IOrganizationMergeService
    {
        AIMSDbContext context;

        public OrganizationMergeService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public IEnumerable<OrganizationMiniView> GetOrganizationsAppliedForMerge()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var orgIds = unitWork.OrganizationsToMergeRepository.GetProjection(o => o.OrganizationId != 0, o => o.OrganizationId).Distinct();
                List<OrganizationMiniView> organizationsList = new List<OrganizationMiniView>();
                foreach(var id in orgIds)
                {
                    organizationsList.Add(new OrganizationMiniView()
                    {
                        Id= id
                    });
                }
                return organizationsList;
            }
        }

        public async Task<ActionResponse> AddAsync(MergeOrganizationModel model, int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                EFOrganization envelopeOrganization = null;

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

                var requestedBy = unitWork.UserRepository.GetOne(u => u.Id == userId);
                if (requestedBy == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User");
                    response.Success = false;
                    return response;
                }

                var envelopeIds = unitWork.EnvelopeRepository.GetProjection(e => e.Id != 0, e => e.FunderId);
                bool hasEnvelope = false;
                foreach (int orgId in model.Ids)
                {
                    if (envelopeIds.Contains(orgId))
                    {
                        hasEnvelope = true;
                        break;
                    }
                }

                if (hasEnvelope && (model.EnvelopeOrganizationId == null || model.EnvelopeOrganizationId <= 0))
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Provided Organization for Envelope");
                    response.Success = false;
                    return response;
                }

                if (model.EnvelopeOrganizationId > 0)
                {
                    envelopeOrganization = (from o in organizations
                                            where o.Id == model.EnvelopeOrganizationId
                                            select o).FirstOrDefault();

                    if (envelopeOrganization == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Provided Organization for Envelope");
                        response.Success = false;
                        return response;
                    }

                    var envelopeExists = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == model.EnvelopeOrganizationId);
                    if (envelopeExists == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Envelope");
                        response.Success = false;
                        return response;
                    }

                    string orgsJson = string.Join("-", orgIds);
                    var requestExists = unitWork.OrganizationMergeRequestsRepository.GetOne(r => r.EnvelopeOrganizationId == model.EnvelopeOrganizationId);
                    if (requestExists != null)
                    {
                        var storedOrgIds = requestExists.OrganizationIdsJson.Split("-");
                        bool unMatchFound = false;
                        foreach(var o in storedOrgIds)
                        {
                            int orgId = Convert.ToInt32(o);
                            if (!orgIds.Contains(orgId))
                            {
                                unMatchFound = true;
                                break;
                            }
                        }

                        if (!unMatchFound)
                        {
                            response.Message = "Merge request already exists";
                            return response;
                        }
                    }
                }

                try
                {
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
                                    OrganizationTypeId = model.OrganizationTypeId,
                                    NewName = model.NewName,
                                    IsApproved = false,
                                    Dated = DateTime.Now,
                                    OrganizationIdsJson = string.Join("-", orgIds),
                                    RequestedBy = requestedBy,
                                    EnvelopeOrganization = envelopeOrganization
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
                                transaction.Commit();

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
                                var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.MergeOrganizationRequest);
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
                }
                catch(Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        response.Message = ex.InnerException.Message;
                    }
                    response.Success = false;
                }
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
                    var requests = unitWork.OrganizationMergeRequestsRepository.GetManyQueryable(r => r.RequestedById != userId);
                    List<int> requestIds = new List<int>();
                    foreach(var request in requests)
                    {
                        var orgIds = request.OrganizationIdsJson.Split("-").ToList();
                        if (orgIds.Contains(organizationId.ToString()))
                        {
                            requestIds.Add(request.Id);
                        }
                    }
                    var userRequests = unitWork.OrganizationsToMergeRepository.GetManyQueryable(m => requestIds.Contains(m.RequestId));
                    
                    if (userRequests.Any())
                    {
                        var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => o.Id != 0);
                        foreach(int requestId in requestIds)
                        {
                            var organizationIds = (from rq in userRequests
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

        public async Task<ActionResponse> ApproveMergeRequestAsync(int requestId, int userOrgId)
        {
            var unitWork = new UnitOfWork(context);
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

            var orgIds = request.Organizations.Select(o => o.OrganizationId);
            if (orgIds.Count() < 2)
            {
                mHelper = new MessageHelper();
                response.Message = mHelper.InvalidOrganizationMerge();
                response.Success = false;
                return response;
            }

            var organizationType = unitWork.OrganizationTypesRepository.GetOne(t => t.Id == request.OrganizationTypeId);
            if (organizationType == null)
            {
                mHelper = new MessageHelper();
                response.Message = mHelper.GetNotFound("Organization type");
                response.Success = false;
                return response;
            }

            try
            {
                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => orgIds.Contains(o.Id));
                        var organizationNames = (from org in organizations
                                                 select org.OrganizationName).ToList<string>();

                        var users = unitWork.UserRepository.GetManyQueryable(u => (orgIds.Contains(u.OrganizationId)));
                        var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                        {
                            OrganizationType = organizationType,
                            OrganizationName = request.NewName,
                            IsApproved = true
                        });
                        await unitWork.SaveAsync();

                        List<string> emailsList = new List<string>();
                        foreach (var user in users)
                        {
                            emailsList.Add(user.Email);
                            user.Organization = newOrganization;
                            unitWork.UserRepository.Update(user);
                        }
                        await unitWork.SaveAsync();

                        if (request.EnvelopeOrganizationId != null)
                        {
                            var envelopeToUpdate = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == request.EnvelopeOrganizationId);
                            envelopeToUpdate.Funder = newOrganization;
                            unitWork.EnvelopeRepository.Update(envelopeToUpdate);
                            await unitWork.SaveAsync();
                        }

                        var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => n.OrganizationId != null && orgIds.Contains((int)n.OrganizationId));
                        foreach (var notification in notifications)
                        {
                            notification.OrganizationId = newOrganization.Id;
                            unitWork.NotificationsRepository.Update(notification);
                            await unitWork.SaveAsync();
                        }

                        var projectFunders = unitWork.ProjectFundersRepository.GetManyQueryable(f => orgIds.Contains(f.FunderId));
                        var funderProjectIds = (from p in projectFunders
                                                select p.ProjectId).Distinct();
                        List<EFProjectFunders> fundersList = new List<EFProjectFunders>();
                        foreach (var projectId in funderProjectIds)
                        {
                            fundersList.Add(new EFProjectFunders()
                            {
                                ProjectId = projectId,
                                FunderId = newOrganization.Id,
                            });
                        }
                        if (fundersList.Any())
                        {
                            unitWork.ProjectFundersRepository.InsertMultiple(fundersList);
                            await unitWork.SaveAsync();
                        }

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetManyQueryable(i => orgIds.Contains(i.ImplementerId));
                        var implementerProjectIds = (from p in projectImplementers
                                                     select p.ProjectId).Distinct();
                        List<EFProjectImplementers> implementersList = new List<EFProjectImplementers>();
                        foreach (var projectId in implementerProjectIds)
                        {
                            implementersList.Add(new EFProjectImplementers()
                            {
                                ImplementerId = newOrganization.Id,
                                ProjectId = projectId
                            });
                        }

                        if (implementersList.Any())
                        {
                            unitWork.ProjectImplementersRepository.InsertMultiple(implementersList);
                            await unitWork.SaveAsync();
                        }

                        var updateProjectMembershipRequests = unitWork.ProjectMembershipRepository.GetManyQueryable(m => orgIds.Contains((int)m.OrganizationId));
                        if (updateProjectMembershipRequests.Any())
                        {
                            foreach (var membership in updateProjectMembershipRequests)
                            {
                                membership.OrganizationId = newOrganization.Id;
                                unitWork.ProjectMembershipRepository.Update(membership);
                            }
                            await unitWork.SaveAsync();
                        }

                        var updateProjectUpdatedByOrgs = unitWork.ProjectRepository.GetManyQueryable(p => p.UpdatedByOrganizationId != null && orgIds.Contains((int)p.UpdatedByOrganizationId));
                        if (updateProjectUpdatedByOrgs.Any())
                        {
                            foreach (var proj in updateProjectUpdatedByOrgs)
                            {
                                proj.UpdatedByOrganizationId = newOrganization.Id;
                                unitWork.ProjectRepository.Update(proj);
                            }
                            await unitWork.SaveAsync();
                        }

                        var orgIdsToDelete = (from o in organizations
                                      select o.Id).ToList<int>();
                        var deleteRequestsForDeleted = unitWork.OrganizationMergeRequestsRepository.GetManyQueryable(r => r.EnvelopeOrganizationId != null && orgIdsToDelete.Contains((int)r.EnvelopeOrganizationId));
                        foreach(var requestToDelete in deleteRequestsForDeleted)
                        {
                            unitWork.OrganizationMergeRequestsRepository.Delete(requestToDelete);
                        }
                        if (deleteRequestsForDeleted.Any())
                        {
                            await unitWork.SaveAsync();
                        }

                        response.ReturnedId = newOrganization.Id;
                        foreach (var organization in organizations)
                        {
                            unitWork.OrganizationRepository.Delete(organization);
                        }
                        await unitWork.SaveAsync();

                        
                        transaction.Commit();

                        string subject = "", message = "", footerMessage = "";
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.OrganizationMerged);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }

                        mHelper = new MessageHelper();
                        message += mHelper.OrganizationsMergedMessage(organizationNames, newOrganization.OrganizationName, emailMessage.Message, emailMessage.FooterMessage);

                        //Send email
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

                        if (emailsList.Count > 0)
                        {
                            List<EmailAddress> emailAddresses = new List<EmailAddress>();
                            foreach (var email in emailsList)
                            {
                                emailAddresses.Add(new EmailAddress()
                                {
                                    Email = email
                                });
                            }
                            IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                            emailHelper.SendEmailToUsers(emailAddresses, subject, "", message, footerMessage);
                        }
                    }
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                });
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    response.Message = ex.InnerException.Message;
                }
                response.Success = false;
            }
            return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
        }

        public ActionResponse RejectRequest(int requestId, int userOrgId, string userEmail)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var request = unitWork.OrganizationMergeRequestsRepository.GetWithInclude(r => r.Id == requestId && r.IsApproved == false, new string[] { "Organizations", "Organizations.Organization" }).FirstOrDefault();
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

                unitWork.OrganizationMergeRequestsRepository.Delete(request);
                unitWork.Save();

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
                return response;
            }
        }

        public List<MergeOrganizationsRequest> GetTwoWeeksOlderRequests()
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
                    OrganizationTypeId = request.OrganizationTypeId,
                    NewName = request.NewName,
                    OrganizationIds = (request.Organizations.Count > 0 ) ? request.Organizations.Select(o => o.OrganizationId).ToList() : new List<int>()
                });
            }
            return mergeRequestOlderTwoWeeks;
        }

        public async Task<ActionResponse> MergeOrganizationsAuto(List<int> ids)
        {
            var unitWork = new UnitOfWork(context);
            IMessageHelper mHelper;
            ActionResponse response = new ActionResponse();
            var allOrganizations = await unitWork.OrganizationRepository.GetManyQueryableAsync(o => o.Id != 0);
            var allOrganizationNames = (from org in allOrganizations
                                        select org.OrganizationName).ToList<string>();
            var organizationTypes = await unitWork.OrganizationTypesRepository.GetManyQueryableAsync(t => t.Id != 0);

            try
            {
                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        foreach (int requestId in ids)
                        {
                            var request = unitWork.OrganizationMergeRequestsRepository.GetWithInclude(r => r.Id == requestId, new string[] { "Organizations" }).FirstOrDefault();
                            if (request == null)
                            {
                                continue;
                            }

                            var orgIds = request.Organizations.Select(o => o.OrganizationId);
                            if (orgIds.Count() < 2)
                            {
                                continue;
                            }

                            var organizations = (from org in allOrganizations
                                                 where orgIds.Contains(org.Id)
                                                 select org);
                            var organizationNames = (from org in allOrganizations
                                                     where orgIds.Contains(org.Id)
                                                     select org.OrganizationName).ToList();

                            var organizationType = (from t in organizationTypes
                                                    where t.Id == request.OrganizationTypeId
                                                    select t).FirstOrDefault();

                            if (organizationType == null)
                            {
                                continue;
                            }

                            var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                            {
                                OrganizationType = organizationType,
                                OrganizationName = request.NewName,
                                IsApproved = true
                            });
                            await unitWork.SaveAsync();

                            var users = unitWork.UserRepository.GetManyQueryable(u => u.IsApproved == true && orgIds.Contains(u.OrganizationId));
                            List<string> emailsList = new List<string>();
                            foreach (var user in users)
                            {
                                emailsList.Add(user.Email);
                                user.Organization = newOrganization;
                                unitWork.UserRepository.Update(user);
                            }

                            if (users.Any())
                            {
                                await unitWork.SaveAsync();
                            }

                            var projectFunders = unitWork.ProjectFundersRepository.GetManyQueryable(f => orgIds.Contains(f.FunderId));
                            var funderProjectIds = (from f in projectFunders
                                                    select f.ProjectId).Distinct();
                            List<EFProjectFunders> fundersList = new List<EFProjectFunders>();
                            foreach (var projectId in funderProjectIds)
                            {
                                fundersList.Add(new EFProjectFunders()
                                {
                                    ProjectId = projectId,
                                    FunderId = newOrganization.Id,
                                });
                            }
                            if (fundersList.Any())
                            {
                                unitWork.ProjectFundersRepository.InsertMultiple(fundersList);
                                await unitWork.SaveAsync();
                            }

                            var projectImplementers = unitWork.ProjectImplementersRepository.GetManyQueryable(i => orgIds.Contains(i.ImplementerId));
                            var implementerProjectIds = (from i in projectImplementers
                                                     select i.ProjectId).Distinct();
                            List<EFProjectImplementers> implementersList = new List<EFProjectImplementers>();
                            foreach (var projectId in implementerProjectIds)
                            {
                                implementersList.Add(new EFProjectImplementers()
                                {
                                    ImplementerId = newOrganization.Id,
                                    ProjectId = projectId
                                });
                            }
                            if (implementersList.Any())
                            {
                                unitWork.ProjectImplementersRepository.InsertMultiple(implementersList);
                                await unitWork.SaveAsync();
                            }

                            var updateProjectMembershipRequests = unitWork.ProjectMembershipRepository.GetManyQueryable(m => orgIds.Contains((int)m.OrganizationId));
                            if (updateProjectMembershipRequests.Any())
                            {
                                foreach (var membership in updateProjectMembershipRequests)
                                {
                                    membership.OrganizationId = newOrganization.Id;
                                    unitWork.ProjectMembershipRepository.Update(membership);
                                }
                                await unitWork.SaveAsync();
                            }

                            if (request.EnvelopeOrganizationId != null)
                            {
                                var envelopeToUpdate = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == request.EnvelopeOrganizationId);
                                if (envelopeToUpdate != null)
                                {
                                    envelopeToUpdate.Funder = newOrganization;
                                    unitWork.EnvelopeRepository.Update(envelopeToUpdate);
                                    await unitWork.SaveAsync();
                                }
                            }

                            //Update notifications
                            var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => n.OrganizationId != null && orgIds.Contains((int)n.OrganizationId));
                            foreach (var notification in notifications)
                            {
                                notification.OrganizationId = newOrganization.Id;
                                unitWork.NotificationsRepository.Update(notification);
                                unitWork.Save();
                            }

                            foreach (var organization in organizations)
                            {
                                unitWork.OrganizationRepository.Delete(organization);
                                unitWork.Save();
                            }

                            //Now delete the request
                            unitWork.OrganizationMergeRequestsRepository.Delete(request);
                            unitWork.Save();

                            string subject = "", message = "", footerMessage = "";
                            var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.OrganizationMerged);
                            if (emailMessage != null)
                            {
                                subject = emailMessage.Subject;
                                message = emailMessage.Message;
                                footerMessage = emailMessage.FooterMessage;
                            }

                            mHelper = new MessageHelper();
                            message += mHelper.OrganizationsMergedMessage(organizationNames, newOrganization.OrganizationName, emailMessage.Message, emailMessage.FooterMessage);

                            //Send email
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

                            transaction.Commit();
                            response.ReturnedId = newOrganization.Id;

                            if (emailsList.Count > 0)
                            {
                                List<EmailAddress> emailAddresses = new List<EmailAddress>();
                                foreach (var email in emailsList)
                                {
                                    emailAddresses.Add(new EmailAddress()
                                    {
                                        Email = email
                                    });
                                }
                                IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                                emailHelper.SendEmailToUsers(emailAddresses, subject, "", message, footerMessage);
                            }
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                if (ex.InnerException != null)
                {
                    response.Message = ex.InnerException.Message;
                }
            }
            return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
        }

        /*public async Task<ActionResponse> MergeOrganizations(int requestId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var request = unitWork.OrganizationMergeRequestsRepository.GetWithInclude(r => r.Id == requestId && r.IsApproved == true, new string[] { "Organizations" }).FirstOrDefault();
                if (request == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Merge Request");
                    response.Success = false;
                    return response;
                }

                var orgIds = request.Organizations.Select(o => o.OrganizationId);
                if (orgIds.Count() < 2)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.InvalidOrganizationMerge();
                    response.Success = false;
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var organizationType = unitWork.OrganizationTypesRepository.GetOne(t => t.Id == request.OrganizationTypeId);
                if (organizationType == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization type");
                    response.Success = false;
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => orgIds.Contains(o.Id));
                        var organizationNames = (from org in organizations
                                                 select org.OrganizationName).ToList<string>();

                        var users = unitWork.UserRepository.GetManyQueryable(u => (orgIds.Contains(u.OrganizationId)));
                        var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                        {
                            OrganizationType = organizationType,
                            OrganizationName = request.NewName,
                            IsApproved = true
                        });
                        unitWork.Save();

                        List<string> emailsList = new List<string>();
                        foreach (var user in users)
                        {
                            emailsList.Add(user.Email);
                            user.Organization = newOrganization;
                            unitWork.UserRepository.Update(user);
                            unitWork.Save();
                        }

                        var projectFunders = unitWork.ProjectFundersRepository.GetManyQueryable(f => orgIds.Contains(f.FunderId));
                        List<EFProjectFunders> fundersList = new List<EFProjectFunders>();
                        foreach (var funder in projectFunders)
                        {
                            fundersList.Add(new EFProjectFunders()
                            {
                                ProjectId = funder.ProjectId,
                                FunderId = newOrganization.Id,
                            });
                            unitWork.ProjectFundersRepository.Delete(funder);
                        }

                        if (fundersList.Count > 0)
                        {
                            unitWork.Save();
                            unitWork.ProjectFundersRepository.InsertMultiple(fundersList);
                            unitWork.Save();
                        }

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetManyQueryable(i => orgIds.Contains(i.ImplementerId));
                        List<EFProjectImplementers> implementersList = new List<EFProjectImplementers>();
                        foreach (var implementer in projectImplementers)
                        {
                            implementersList.Add(new EFProjectImplementers()
                            {
                                ImplementerId = newOrganization.Id,
                                ProjectId = implementer.ProjectId
                            });
                            unitWork.ProjectImplementersRepository.Delete(implementer);
                        }

                        if (implementersList.Count > 0)
                        {
                            unitWork.Save();
                            unitWork.ProjectImplementersRepository.InsertMultiple(implementersList);
                            unitWork.Save();
                        }

                        //Update notifications
                        var notifications = unitWork.NotificationsRepository.GetManyQueryable(n => n.OrganizationId != null && orgIds.Contains((int)n.OrganizationId));
                        foreach (var notification in notifications)
                        {
                            notification.OrganizationId = newOrganization.Id;
                            unitWork.NotificationsRepository.Update(notification);
                            unitWork.Save();
                        }

                        foreach (var organization in organizations)
                        {
                            unitWork.OrganizationRepository.Delete(organization);
                            unitWork.Save();
                        }
                        //Now delete the request
                        unitWork.OrganizationMergeRequestsRepository.Delete(request);
                        unitWork.Save();

                        string subject = "", message = "", footerMessage = "";
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.OrganizationMerged);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }

                        mHelper = new MessageHelper();
                        message += mHelper.OrganizationsMergedMessage(organizationNames, newOrganization.OrganizationName, emailMessage.Message, emailMessage.FooterMessage);

                        //Send email
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

                        transaction.Commit();
                        response.ReturnedId = newOrganization.Id;

                        if (emailsList.Count > 0)
                        {
                            List<EmailAddress> emailAddresses = new List<EmailAddress>();
                            foreach (var email in emailsList)
                            {
                                emailAddresses.Add(new EmailAddress()
                                {
                                    Email = email
                                });
                            }
                            IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                            emailHelper.SendEmailToUsers(emailAddresses, subject, "", message, footerMessage);
                        }
                    }
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }*/
    }
}
