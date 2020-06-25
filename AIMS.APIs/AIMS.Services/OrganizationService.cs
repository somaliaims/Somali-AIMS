using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AIMS.Services
{
    public interface IOrganizationService
    {
        /// <summary>
        /// Gets all organizations
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrganizationView> GetAll();

        /// <summary>
        /// Gets list of organizations having type
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrganizationView> GetWithType();

        /// <summary>
        /// Gets all organizations entered by user
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrganizationView> GetUserOrganizations();

        /// <summary>
        /// Gets all iati organizations
        /// </summary>
        /// <returns></returns>
        IEnumerable<IATIOrganizationView> GetIATIOrganizations();

        /// <summary>
        /// Gets organization by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OrganizationViewModel Get(int id);

        /// <summary>
        /// Gets all organizations matching the name with criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<OrganizationView> GetMatching(string criteria);

        /// <summary>
        /// Gets organizations for the provided type id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<OrganizationView> GetOrganizationsForType(int id);

        /// <summary>
        /// Gets all organizations async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OrganizationView>> GetAllAsync();

        /// <summary>
        /// Gets list of organizations that have envelope data available
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrganizationView> GetOrganizationsHavingEnvelope();

        /// <summary>
        /// Gets organizations ids only having envelope data available
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrganizationMiniView> GetOrganizationIdsHavingEnvelope();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(OrganizationModel organization);

        /// <summary>
        /// Deletes an organization and assigns new one if required
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newId"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id, int newId = 0);

        /// <summary>
        /// Renames an organization
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        Task<ActionResponse> RenameOrganization(int id, string newName);

        /// <summary>
        /// Merges two organizations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> MergeOrganizations(MergeOrganizationModel model);

        /// <summary>
        /// Merges organizations multiple requests
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Task<ActionResponse> MergeOrganizationsAutoAsync(List<MergeOrganizationsRequest> mergeRequests);

        /// <summary>
        /// Updates a organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        ActionResponse Update(int id, OrganizationModel organization);

        /// <summary>
        /// Approves an organization with the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Approve(int id);

        /// <summary>
        /// Returns true or false if an organization is registered with a user
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        ActionResponse CheckIfOrganizationsHaveUsers(List<int> ids);

        /// <summary>
        /// Gets list of organizations
        /// </summary>
        /// <returns></returns>
        int GetOrganizationsCount();
    }

    public class OrganizationService : IOrganizationService
    {
        AIMSDbContext context;
        IMapper mapper;

        public OrganizationService(AIMSDbContext cntxt, IMapper mappr)
        {
            context = cntxt;
            mapper = mappr;
        }

        public IEnumerable<OrganizationView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetWithInclude(o => o.Id != 0, new string[] { "OrganizationType" });
                if (organizations.Count() > 0)
                {
                    organizations = (from org in organizations
                                     orderby org.OrganizationName ascending
                                     select org);
                }
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public int GetOrganizationsCount()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                return unitWork.OrganizationRepository.GetProjectionCount(o => o.IsApproved == true, o => o.Id);
            }
        }

        public IEnumerable<OrganizationView> GetWithType()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetWithInclude(o => o.OrganizationTypeId != 0, new string[] { "OrganizationType" });
                if (organizations.Count() > 0)
                {
                    organizations = (from org in organizations
                                     orderby org.OrganizationName ascending
                                     select org);
                }
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public IEnumerable<OrganizationView> GetUserOrganizations()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetWithInclude(o => o.Id != 0, new string[] { "OrganizationType" });
                if (organizations.Count() > 0)
                {
                    organizations = (from org in organizations
                                     orderby org.OrganizationName ascending
                                     select org);
                }
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public ActionResponse CheckIfOrganizationsHaveUsers(List<int> ids)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();
            var users = unitWork.UserRepository.GetProjection(u => ids.Contains(u.OrganizationId), u => u.Id);
            response.ReturnedId = (users.Any()) ? users.Count() : 0;
            return response;
        }

        public IEnumerable<IATIOrganizationView> GetIATIOrganizations()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<IATIOrganizationView> organizationsList = new List<IATIOrganizationView>();
                var organizations = unitWork.IATIOrganizationRepository.GetManyQueryable(o => o.Id != 0);
                if (organizations.Count() > 0)
                {
                    organizations = (from org in organizations
                                     orderby org.OrganizationName ascending
                                     select org);
                }
                return mapper.Map<List<IATIOrganizationView>>(organizations);
            }
        }

        public IEnumerable<OrganizationView> GetOrganizationsHavingEnvelope()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizationIds = unitWork.EnvelopeRepository.GetProjection(e => e.Id != 0, e => e.FunderId);
                var organizations = unitWork.OrganizationRepository.GetWithInclude(o =>  
                organizationIds.Contains(o.Id), new string[] { "OrganizationType" });

                if (organizations.Count() > 0)
                {
                    organizations = (from org in organizations
                                     orderby org.OrganizationName ascending
                                     select org);
                }
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public IEnumerable<OrganizationMiniView> GetOrganizationIdsHavingEnvelope()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationMiniView> organizationsList = new List<OrganizationMiniView>();
                var organizationIds = unitWork.EnvelopeRepository.GetProjection(e => e.Id != 0, e => e.FunderId);
                foreach(var id in organizationIds)
                {
                    organizationsList.Add(new OrganizationMiniView()
                    {
                        Id = id
                    });
                }
                return organizationsList;
            }
        }

        public IEnumerable<OrganizationView> GetSourceOrganizations()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.IATIOrganizationRepository.GetWithInclude(o => o.Id != 0, new string[] { "OrganizationType" });
                if (organizations.Count() > 0)
                {
                    organizations = (from org in organizations
                                     orderby org.OrganizationName ascending
                                     select org);
                }
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public OrganizationViewModel Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizationList = unitWork.OrganizationRepository.GetWithInclude(o => o.Id.Equals(id), new string[] { "OrganizationType" });
                EFOrganization organization = null;
                foreach (var org in organizationList)
                {
                    organization = org;
                }
                return mapper.Map<OrganizationViewModel>(organization);
            }
        }

        public IEnumerable<OrganizationView> GetOrganizationsForType(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizationList = unitWork.OrganizationRepository.GetManyQueryable(o => o.OrganizationTypeId.Equals(id));
                return mapper.Map<List<OrganizationView>>(organizationList);
            }
        }

        public IEnumerable<OrganizationView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetWithInclude(o => o.OrganizationName.Contains(criteria), new string[] { "OrganizationType" });
                if (organizations.Count() > 0)
                {
                    organizations = (from org in organizations
                                     orderby org.OrganizationName ascending
                                     select org);
                }
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public async Task<IEnumerable<OrganizationView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizations = await unitWork.OrganizationRepository.GetWithIncludeAsync(o => o.Id != 0, new string[] { "OrganizationType" });
                if (organizations.Count() > 0)
                {
                    organizations = (from org in organizations
                                     orderby org.OrganizationName ascending
                                     select org);
                }
                return await Task<IEnumerable<OrganizationView>>.Run(() => mapper.Map<List<OrganizationView>>(organizations)).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(OrganizationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isOrganizationCreated = unitWork.OrganizationRepository.GetOne(o => o.OrganizationName.ToLower() == model.Name.ToLower().Trim());
                    if (isOrganizationCreated != null)
                    {
                        response.ReturnedId = isOrganizationCreated.Id;
                    }
                    else
                    {
                        var organizationType = unitWork.OrganizationTypesRepository.GetOne(o => o.Id == model.OrganizationTypeId);
                        var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                        {
                            OrganizationType = organizationType,
                            OrganizationName = model.Name.Trim(),
                        });
                        unitWork.Save();
                        response.ReturnedId = newOrganization.Id;
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, OrganizationModel organization)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var organizationObj = unitWork.OrganizationRepository.GetByID(id);
                IMessageHelper mHelper = new MessageHelper();

                if (organizationObj == null)
                {
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Organization");
                    return response;
                }

                var organizationType = unitWork.OrganizationTypesRepository.GetOne(o => o.Id == organization.OrganizationTypeId);
                if (organizationType == null)
                {
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Organization Type");
                    return response;
                }

                organizationObj.OrganizationName = organization.Name.Trim();
                organizationObj.OrganizationType = organizationType;
                unitWork.OrganizationRepository.Update(organizationObj);
                unitWork.Save();
                response.ReturnedId = id;
                return response;
            }
        }

        public async Task<ActionResponse> RenameOrganization(int id, string newName)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                if (string.IsNullOrEmpty(newName))
                {
                    response.Success = false;
                    response.Message = "Invalid name provided for organization";
                    return response;
                }

                var organization = unitWork.OrganizationRepository.GetByID(id);
                if (organization == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization");
                    response.Success = false;
                    return response;
                }

                string oldName = organization.OrganizationName;
                organization.OrganizationName = newName;
                unitWork.OrganizationRepository.Update(organization);
                unitWork.Save();

                string subject = "", message = "", footerMessage = "";
                var users = unitWork.UserRepository.GetManyQueryable(u => u.OrganizationId == id);
                if (users != null)
                {
                    List<EmailAddress> emailsList = (from u in users
                                                     select new EmailAddress()
                                                     {
                                                         Email = u.Email
                                                     }).ToList();

                    if (emailsList.Count > 0)
                    {
                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.OrganizationRenamed);
                        if (emailMessage != null)
                        {
                            subject = emailMessage.Subject;
                            message = emailMessage.Message;
                            footerMessage = emailMessage.FooterMessage;
                        }

                        mHelper = new MessageHelper();
                        message = mHelper.OrganizationRenamedMessage(oldName, newName, emailMessage.Message, emailMessage.FooterMessage);
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
                
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public async Task<ActionResponse> MergeOrganizations(MergeOrganizationModel model)
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

                var envelopeIds = unitWork.EnvelopeRepository.GetProjection(e => e.Id != 0, e => e.FunderId);
                bool hasEnvelope = false;
                foreach(int orgId in model.Ids)
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

                try
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
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
                                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                                }

                                var users = unitWork.UserRepository.GetManyQueryable(u => (orgIds.Contains(u.OrganizationId)));
                                var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                                {
                                    OrganizationType = organizationType,
                                    OrganizationName = model.NewName,
                                    IsApproved = true
                                });
                                await unitWork.SaveAsync();

                                List<string> emailsList = new List<string>();
                                foreach (var user in users)
                                {
                                    emailsList.Add(user.Email);
                                    user.Organization = newOrganization;
                                    unitWork.UserRepository.Update(user);
                                    await unitWork.SaveAsync();
                                }

                                var projectFunders = unitWork.ProjectFundersRepository.GetManyQueryable(f => orgIds.Contains(f.FunderId));
                                var funderProjectIds = (from f in projectFunders
                                                        select f.ProjectId).Distinct();
                                List<EFProjectFunders> fundersList = new List<EFProjectFunders>();
                                foreach (var funder in projectFunders)
                                {
                                    fundersList.Add(new EFProjectFunders()
                                    {
                                        ProjectId = funder.ProjectId,
                                        FunderId = newOrganization.Id,
                                    });
                                }

                                if (fundersList.Count > 0)
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

                                if (implementersList.Count > 0)
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

                                var updateEnvelope = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == model.EnvelopeOrganizationId);
                                if (updateEnvelope != null)
                                {
                                    updateEnvelope.FunderId = newOrganization.Id;
                                    unitWork.EnvelopeRepository.Update(updateEnvelope);
                                    await unitWork.SaveAsync();
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
                                }
                                await unitWork.SaveAsync();

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
                            catch(Exception ex)
                            {
                                response.Success = false;
                                response.Message = ex.Message;
                            }
                        }
                        return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                    });
                }
                catch(Exception ex)
                {
                    string message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        /*public async Task<ActionResponse> MergeOrganizationsAutoAsync(List<MergeOrganizationsRequest> mergeRequests)
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();
            IMessageHelper mHelper;
            EFOrganizationTypes organizationType = null;
            var strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
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

                    var organizations = await unitWork.OrganizationRepository.GetWithIncludeAsync(o => o.Id != 0, new string[] { "OrganizationType" });
                    foreach (var request in mergeRequests)
                    {
                        var orgIds = request.OrganizationIds;
                        var organizationNames = (from org in organizations
                                                 where orgIds.Contains(org.Id)
                                                 select org.OrganizationName).ToList<string>();


                        if (organizationNames.Count < orgIds.Count)
                        {
                            continue;
                        }

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

                        var orgsToDelete = (from org in organizations
                                            where orgIds.Contains(org.Id)
                                            select org);
                        foreach (var organization in orgsToDelete)
                        {
                            unitWork.OrganizationRepository.Delete(organization);
                            unitWork.Save();
                        }

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
            return await Task.Run<ActionResponse>(() => response).ConfigureAwait(false);
        }*/

        public ActionResponse Approve(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var organization = unitWork.OrganizationRepository.GetByID(id);
                if (organization == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization");
                    response.Success = false;
                    return response;
                }

                try
                {
                    organization.IsApproved = true;
                    unitWork.OrganizationRepository.Update(organization);
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public async Task<ActionResponse> DeleteAsync(int id, int newId = 0)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var organizations = await unitWork.OrganizationRepository.GetManyQueryableAsync(o => (o.Id == id || o.Id == newId));
                if (organizations.Count() < 2 && newId != 0)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization");
                    response.Success = false;
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var userAccounts = unitWork.UserRepository.GetManyQueryable(u => u.OrganizationId == id);
                if (userAccounts.Any() && newId == 0)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetUserAccountsUnderOrgMessage();
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var projectFundersList = await unitWork.ProjectFundersRepository.GetManyQueryableAsync(f => ( f.FunderId == id || f.FunderId == newId));
                var projectIds = (from s in projectFundersList
                                  select s.ProjectId).Distinct().ToList<int>();

                if (projectIds.Count() > 0 && newId == 0)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetDependentProjectsOnOrganizationMessage();
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                EFOrganization newOrganization = null;
                EFOrganization oldOrganization = (from org in organizations
                                                  where org.Id == id
                                                  select org).FirstOrDefault();

                newOrganization = (from org in organizations
                                   where org.Id == newId
                                   select org).FirstOrDefault();

                var projects = unitWork.ProjectRepository.GetWithInclude(p => projectIds.Contains(p.Id), new string[] { "CreatedBy" });
                List<string> projectNames = (from p in projects
                                             select p.Title).ToList<string>();
                var emails = (from p in projects
                              select p.CreatedBy.Email);


                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        //var projectFundersList = await unitWork.ProjectFundersRepository.GetManyQueryableAsync(p => (p.FunderId == id || p.FunderId == newId));
                        var projectImplementersList = await unitWork.ProjectImplementersRepository.GetManyQueryableAsync(i => (i.ImplementerId == id || i.ImplementerId == newId));
                        List<EFProjectFunders> fundersList = new List<EFProjectFunders>();
                        List<EFProjectImplementers> implementersList = new List<EFProjectImplementers>();

                        var projectFunders = (from f in projectFundersList
                                              where f.FunderId == id
                                              select f);
                        var projectImplementers = (from i in projectImplementersList
                                                   where i.ImplementerId == id
                                                   select i);

                        List<FundersKeyView> fundersInDb = (from f in projectFundersList
                                                            where f.FunderId == newId
                                          select new FundersKeyView{
                                              FunderId = f.FunderId,
                                              ProjectId = f.ProjectId
                                          }).ToList<FundersKeyView>();

                        List<ImplementersKeyView> implementersInDb = (from i in projectImplementersList
                                                                      where i.ImplementerId == newId
                                                            select new ImplementersKeyView
                                                            {
                                                                ImplementerId = i.ImplementerId,
                                                                ProjectId = i.ProjectId
                                                            }).ToList<ImplementersKeyView>();

                        foreach (var funder in projectFunders)
                        {
                            var funderExists = (from f in fundersList
                                                where f.FunderId == newId && f.ProjectId == funder.ProjectId
                                                select f).FirstOrDefault();
                            var funderInDb = (from f in fundersInDb
                                              where f.FunderId == newId && f.ProjectId == funder.ProjectId
                                              select f).FirstOrDefault();

                            if (funderExists == null && funderInDb == null)
                            {
                                fundersList.Add(new EFProjectFunders()
                                {
                                    ProjectId = funder.ProjectId,
                                    FunderId = newId,
                                });
                            }
                            unitWork.ProjectFundersRepository.Delete(funder);
                        }
                        await unitWork.SaveAsync();

                        foreach(var implementer in projectImplementers)
                        {
                            var implementerExists = (from i in implementersList
                                                     where (i.ImplementerId == implementer.ImplementerId && i.ProjectId == implementer.ProjectId)
                                                     select i).FirstOrDefault();

                            var implementerInDb = (from i in implementersInDb
                                                   where i.ImplementerId == implementer.ImplementerId && i.ProjectId == implementer.ProjectId
                                                   select i).FirstOrDefault();

                            if (implementerExists == null && implementerInDb == null)
                            {
                                implementersList.Add(new EFProjectImplementers()
                                {
                                    ProjectId = implementer.ProjectId,
                                    ImplementerId = newId
                                });
                            }
                            unitWork.ProjectImplementersRepository.Delete(implementer);
                        }

                        
                        if (newId != 0)
                        {
                            foreach(var userAccount in userAccounts)
                            {
                                userAccount.OrganizationId = newId;
                                unitWork.UserRepository.Update(userAccount);
                            }
                        }
                        await unitWork.SaveAsync();
                            
                        unitWork.ProjectFundersRepository.InsertMultiple(fundersList);
                        unitWork.ProjectImplementersRepository.InsertMultiple(implementersList);
                        unitWork.OrganizationRepository.Delete(oldOrganization);
                        await unitWork.SaveAsync();
                        transaction.Commit();

                        if (projectNames.Count > 0)
                        {
                            var users = unitWork.UserRepository.GetManyQueryable(u => u.UserType == UserTypes.Manager || u.UserType == UserTypes.SuperAdmin);
                            List<EmailAddress> emailAddresses = new List<EmailAddress>();
                            foreach (var user in users)
                            {
                                emailAddresses.Add(new EmailAddress()
                                {
                                    Email = user.Email
                                });
                            }

                            foreach (var email in emails)
                            {
                                var isEmailExists = (from e in emailAddresses
                                                     where e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                                     select e).FirstOrDefault();

                                if (isEmailExists == null)
                                {
                                    emailAddresses.Add(new EmailAddress()
                                    {
                                        Email = email
                                    });
                                }
                            }

                            if (emailAddresses.Count > 0)
                            {
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
                                var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ChangedMappingEffectedProject);
                                if (emailMessage != null)
                                {
                                    subject = emailMessage.Subject;
                                    message = emailMessage.Message;
                                    footerMessage = emailMessage.FooterMessage;
                                }

                                mHelper = new MessageHelper();
                                string oldSectorName = oldOrganization != null ? oldOrganization.OrganizationName : null;
                                string newSectorName = newOrganization != null ? newOrganization.OrganizationName : null;
                                message += mHelper.ChangedMappingAffectedProjectsMessage(projectNames, oldSectorName, newSectorName);
                                IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                                emailHelper.SendEmailToUsers(emailAddresses, subject, "", message);
                            }
                        }
                    }
                });

                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }
    }
}
