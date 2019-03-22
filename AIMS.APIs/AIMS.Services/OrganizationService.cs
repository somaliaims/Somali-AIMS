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
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public OrganizationViewModel Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizationList = unitWork.OrganizationRepository.GetMany(o => o.Id.Equals(id));
                EFOrganization organization = null;
                foreach(var org in organizationList)
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
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public async Task<IEnumerable<OrganizationView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizations = await unitWork.OrganizationRepository.GetAsync(o => o.Id != 0);
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
                    var isOrganizationCreated = unitWork.OrganizationRepository.GetOne(o => o.OrganizationName.ToLower() == model.Name.ToLower());
                    if (isOrganizationCreated != null)
                    {
                        response.ReturnedId = isOrganizationCreated.Id;
                    }
                    else
                    {
                        var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                        {
                            OrganizationName = model.Name,
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

                organizationObj.OrganizationName = organization.Name;
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
                            RegisteredOn = DateTime.Now,
                            IsApproved = true
                        });
                        unitWork.Save();

                        foreach (var user in users)
                        {
                            user.Organization = newOrganization;
                            unitWork.UserRepository.Update(user);
                        }
                        unitWork.Save();

                        var projectFunders = unitWork.ProjectFundersRepository.GetManyQueryable(f => orgIds.Contains(f.FunderId));
                        List<EFProjectFunders> fundersList = new List<EFProjectFunders>();

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

                        foreach(var organization in organizations)
                        {
                            unitWork.OrganizationRepository.Delete(organization);
                        }

                        unitWork.Save();
                        transaction.Commit();
                        response.ReturnedId = newOrganization.Id;
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
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }
    }
}
