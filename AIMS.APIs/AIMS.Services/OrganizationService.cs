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
        /// Gets all organizations async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OrganizationView>> GetAllAsync();

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
        /// Merges two organizations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> MergeOrganizations(MergeOrganizationModel model);

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
    }

    public class OrganizationService : IOrganizationService
    {
        AIMSDbContext context;
        IMapper mapper;

        public OrganizationService(AIMSDbContext cntxt, IMapper mappr)
        {
            context = cntxt;
            this.mapper = mappr;
        }

        public IEnumerable<OrganizationView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetMany(o => o.Id != 0);
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
                var organizationList = unitWork.OrganizationRepository.GetMany(o => o.Id.Equals(id));
                EFOrganization organization = null;
                foreach (var org in organizationList)
                {
                    organization = org;
                }
                return mapper.Map<OrganizationViewModel>(organization);
            }
        }

        public IEnumerable<OrganizationView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetMany(o => o.OrganizationName.Contains(criteria));
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
                var organizations = await unitWork.OrganizationRepository.GetAllAsync();
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
                        var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                        {
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

                organizationObj.OrganizationName = organization.Name.Trim();
                unitWork.OrganizationRepository.Update(organizationObj);
                unitWork.Save();
                response.Message = "1";
                return response;
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

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
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
                            OrganizationName = model.NewName,
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

                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.OrganizationMerged);
                        foreach (var funder in projectFunders)
                        {
                            fundersList.Add(new EFProjectFunders()
                            {
                                ProjectId = funder.ProjectId,
                                FunderId = newOrganization.Id,
                                Amount = funder.Amount,
                                ExchangeRate = funder.ExchangeRate,
                                Currency = funder.Currency
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

                        string message = "";
                        if (emailMessage != null)
                        {
                            message = emailMessage.Message;
                        }

                        mHelper = new MessageHelper();
                        message += mHelper.OrganizationsMergedMessage(organizationNames, newOrganization.OrganizationName);
                        //Send notifications and email
                        /*unitWork.NotificationsRepository.Insert(new EFUserNotifications()
                        {
                            NotificationType = NotificationTypes.OrganizationMerged,
                            Message = message,
                            Dated = DateTime.Now,
                            OrganizationId = newOrganization.Id,
                            IsSeen = false,
                            TreatmentId = 0,
                        });
                        unitWork.Save();*/

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
                        }

                        transaction.Commit();
                        response.ReturnedId = newOrganization.Id;

                        if (emailsList.Count > 0)
                        {
                            List<EmailAddress> emailAddresses = new List<EmailAddress>();
                            foreach(var email in emailsList)
                            {
                                emailAddresses.Add(new EmailAddress()
                                {
                                    Email = email
                                });
                            }
                            IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettingsModel);
                            emailHelper.SendEmailToUsers(emailAddresses, "Organizations merged", "Organizations merged", message);
                        }
                        
                        
                    }
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

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

                EFOrganization newOrganization = null;
                EFOrganization oldOrganization = (from org in organizations
                                                  where org.Id == id
                                                  select org).FirstOrDefault();

                newOrganization = (from org in organizations
                                   where org.Id == newId
                                   select org).FirstOrDefault();

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var projectFundersList = await unitWork.ProjectFundersRepository.GetManyQueryableAsync(p => (p.FunderId == id || p.FunderId == newId));
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
                                    Amount = funder.Amount,
                                    ExchangeRate = funder.ExchangeRate,
                                    Currency = funder.Currency
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
                        await unitWork.SaveAsync();

                        if (newId == 0)
                        {
                            unitWork.OrganizationRepository.Delete(oldOrganization);
                        }
                        else
                        {
                            unitWork.ProjectFundersRepository.InsertMultiple(fundersList);
                            unitWork.ProjectImplementersRepository.InsertMultiple(implementersList);
                        }
                        await unitWork.SaveAsync();
                        transaction.Commit();
                    }
                });

                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }
    }
}
