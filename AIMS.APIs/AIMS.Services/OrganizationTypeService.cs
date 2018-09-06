using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(OrganizationTypeModel organizationType);

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
        ActionResponse Delete(int id);
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

        public ActionResponse Add(OrganizationTypeModel organizationType)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var newOrganizationType = unitWork.OrganizationTypesRepository.Insert(new EFOrganizationTypes() { TypeName = organizationType.TypeName });
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
                return response;
            }
        }

        public ActionResponse Delete(int id)
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

                unitWork.organizationTypesRepository.Delete(organizationTypeObj);
                unitWork.Save();
                return response;
            }
        }
    }
}
