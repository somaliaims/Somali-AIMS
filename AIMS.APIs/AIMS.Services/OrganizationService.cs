﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        /// Updates a organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        ActionResponse Update(int id, OrganizationModel organization);
    }

    public class OrganizationService
    {
        AIMSDbContext context;

        public OrganizationService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public IEnumerable<OrganizationView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetAll();
                foreach(var organization in organizations)
                {

                }
                return organizationsList;
            }
        }

        public async Task<IEnumerable<OrganizationView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = await unitWork.OrganizationRepository.GetAllAsync();
                
                foreach (var organization in organizations)
                {

                }
                return await Task<IEnumerable<OrganizationView>>.Run(() => organizationsList).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(OrganizationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    IMessageHelper mHelper;
                    var organizationType = unitWork.OrganizationTypesRepository.GetByID(model.TypeId);
                    if (organizationType == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Organization Type");
                        return response;
                    }
                    var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                    {
                        OrganizationName = model.Name,
                        OrganizationType = organizationType
                    });
                    response.ReturnedId = newOrganization.Id;
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

                var organizationType = unitWork.organizationTypesRepository.GetByID(organization.TypeId);
                if (organizationType == null)
                {
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Organization Type");
                    return response;
                }

                organizationObj.OrganizationName = organization.Name;
                organizationObj.OrganizationType = organizationType;

                unitWork.OrganizationRepository.Update(organizationObj);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }
    }
}