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
    public interface IProjectFundsService
    {
        /// <summary>
        /// Gets all fundings for the provided project id
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectFundsView> GetAll(int id);

        /// <summary>
        /// Gets all projectFundings async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProjectFundsView>> GetAllAsync(int id);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(ProjectFundsModel projectFunding);

        /// <summary>
        /// Updates a projectFunding
        /// </summary>
        /// <param name="projectFunding"></param>
        /// <returns></returns>
        ActionResponse Update(ProjectFundsModel projectFunding);
    }

    public class ProjectFundsService : IProjectFundsService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ProjectFundsService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ProjectFundsView> GetAll(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectFundings = unitWork.ProjectFundsRepository.Get(p => p.ProjectId.Equals(id));
                return mapper.Map<List<ProjectFundsView>>(projectFundings);
            }
        }

        public async Task<IEnumerable<ProjectFundsView>> GetAllAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var projectFundings = await unitWork.ProjectFundsRepository.GetAsync(p => p.ProjectId.Equals(id));
                return await Task<IEnumerable<ProjectFundsView>>.Run(() => mapper.Map<List<ProjectFundsView>>(projectFundings)).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(ProjectFundsModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    IMessageHelper mHelper;
                    var project = unitWork.ProjectRepository.GetByID(model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Project");
                        return response;
                    }

                    var funder = unitWork.OrganizationRepository.GetByID(model.FunderId);
                    if (funder == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Organization/Funder");
                        return response;
                    }

                    var newProjectFunds = unitWork.ProjectFundsRepository.Insert(new EFProjectFundings()
                    {
                        Project = project,
                        Funder = funder,
                        Amount = model.Amount,
                        Currency = model.Currency,
                        ExchangeRate = model.ExchangeRate
                    });
                    response.ReturnedId = newProjectFunds.ProjectId;
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

        public ActionResponse Update(ProjectFundsModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var projectFundingObj = unitWork.ProjectFundsRepository.Get(f => f.ProjectId.Equals(model.ProjectId) && (f.FunderId.Equals(model.FunderId)));
                if (projectFundingObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Project Funding");
                    return response;
                }

                projectFundingObj.Amount = model.Amount;
                projectFundingObj.Currency = model.Currency;
                projectFundingObj.ExchangeRate = model.ExchangeRate;

                unitWork.ProjectFundsRepository.Update(projectFundingObj);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }
    }
}
