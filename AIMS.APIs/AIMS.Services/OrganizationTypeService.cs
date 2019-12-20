using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace AIMS.Services
{
    public interface IOrganizationTypeService
    {
        /// <summary>
        /// Gets all organizationTypes
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrganizationTypeView> GetAll();

        /// <summary>
        /// Gets all organizationTypes async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OrganizationTypeView>> GetAllAsync();

        /// <summary>
        /// Gets organization type for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OrganizationTypeView Get(int id);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(OrganizationTypeModel organizationType);

        /// <summary>
        /// Import organizations with types
        /// </summary>
        /// <param name="organizationsList"></param>
        /// <returns></returns>
        Task<ActionResponse> ImportOrganizationAndTypes(List<ImportedOrganizations> organizationsList);

        /// <summary>
        /// Updates a organizationType
        /// </summary>
        /// <param name="organizationType"></param>
        /// <returns></returns>
        ActionResponse Update(int id, OrganizationTypeModel organizationType);

        /// <summary>
        /// Deletes organization type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id, int newId = 0);
    }

    public class OrganizationTypeService : IOrganizationTypeService
    {
        AIMSDbContext context;
        IMapper mapper;

        public OrganizationTypeService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<OrganizationTypeView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizationTypes = unitWork.OrganizationTypesRepository.GetAll();
                return mapper.Map<List<OrganizationTypeView>>(organizationTypes);
            }
        }

        public async Task<IEnumerable<OrganizationTypeView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizationTypes = await unitWork.OrganizationTypesRepository.GetAllAsync();
                return await Task<IEnumerable<OrganizationTypeView>>.Run(() => mapper.Map<List<OrganizationTypeView>>(organizationTypes)).ConfigureAwait(false);
            }
        }

        public OrganizationTypeView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var orgType = unitWork.OrganizationTypesRepository.GetByID(id);
                OrganizationTypeView orgTypeView = new OrganizationTypeView();
                if (orgType != null)
                {
                    orgTypeView.TypeName = orgType.TypeName;
                }
                return orgTypeView;
            }
        }

        public ActionResponse Add(OrganizationTypeModel organizationType)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var newOrganizationType = unitWork.OrganizationTypesRepository.Insert(new EFOrganizationTypes() 
                    { 
                        TypeName = organizationType.TypeName
                    });
                    response.ReturnedId = newOrganizationType.Id;
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

        public async Task<ActionResponse> ImportOrganizationAndTypes(List<ImportedOrganizations> organizationsList)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var organizationTypes = unitWork.OrganizationTypesRepository.GetManyQueryable(t => t.Id != 0);
                    var organizations = unitWork.OrganizationRepository.GetManyQueryable(o => o.Id != 0);

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            foreach(var org in organizationsList)
                            {
                                var typeName = org.OrganizationType;
                                var orgType = (from t in organizationTypes
                                                    where t.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase)
                                                    select t).FirstOrDefault();
                                
                                if (orgType == null)
                                {
                                    orgType = unitWork.OrganizationTypesRepository.Insert(new EFOrganizationTypes()
                                    {
                                        TypeName = typeName,
                                    });
                                    unitWork.Save();
                                }

                                var organizationName = org.Organization;
                                var organization = (from o in organizations
                                                    where o.OrganizationName.Equals(organizationName, StringComparison.OrdinalIgnoreCase)
                                                    select o).FirstOrDefault();

                                if (organization == null)
                                {
                                    unitWork.OrganizationRepository.Insert(new EFOrganization()
                                    {
                                        OrganizationType = orgType,
                                        OrganizationName = organizationName,
                                        IsApproved = true
                                    });
                                }
                            }
                            await unitWork.SaveAsync();
                            transaction.Commit();
                        }
                    });
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, OrganizationTypeModel organizationType)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var organizationTypeObj = unitWork.OrganizationTypesRepository.GetByID(id);
                if (organizationTypeObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Organization Type");
                    return response;
                }

                organizationTypeObj.TypeName = organizationType.TypeName;
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }

        public async Task<ActionResponse> DeleteAsync(int id, int newId = 0)
        {
            ActionResponse response = new ActionResponse();
            IMessageHelper mHelper;
            if (id < 1)
            {
                mHelper = new MessageHelper();
                response.Success = false;
                response.Message = mHelper.GetNotFound("Organization Type");
                return response;
            }
            using (var unitWork = new UnitOfWork(context))
            {
                mHelper = new MessageHelper();
                var organizationTypes = unitWork.OrganizationTypesRepository.GetManyQueryable(t => (t.Id == id || t.Id == newId));
                if ((id > 1 && organizationTypes.Count() < 1) || (id > 1 && newId > 1 && organizationTypes.Count() < 2))
                {
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Organization Type/s");
                    return response;
                }

                var orgToDelete = (from type in organizationTypes
                                   where type.Id == id
                                   select type).FirstOrDefault();

                var orgToMap = (from type in organizationTypes
                                where type.Id == newId
                                select type).FirstOrDefault();

                if (orgToMap == null && newId > 0)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Organization to map");
                    return response;
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        if (newId > 0)
                        {
                            var organizationsToChange = await unitWork.OrganizationRepository.GetManyQueryableAsync(o => o.OrganizationTypeId == orgToDelete.Id);
                            if (organizationsToChange.Any())
                            {
                                foreach (var org in organizationsToChange)
                                {
                                    org.OrganizationTypeId = orgToMap.Id;
                                    unitWork.OrganizationRepository.Update(org);
                                }
                                await unitWork.SaveAsync();
                            }
                        }
                        unitWork.OrganizationTypesRepository.Delete(orgToDelete);
                        await unitWork.SaveAsync();
                        transaction.Commit();
                    }
                });
            }
            return await Task.Run(() => response).ConfigureAwait(false);
        }
    }
}
