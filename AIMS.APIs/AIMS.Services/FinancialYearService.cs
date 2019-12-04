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
    public interface IFinancialYearService
    {
        /// <summary>
        /// Gets all locations
        /// </summary>
        /// <returns></returns>
        IEnumerable<FinancialYearView> GetAll();

        /// <summary>
        /// Gets the location for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        FinancialYearView Get(int id);
        /// <summary>
        /// Gets all locations async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FinancialYearView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(FinancialYearModel model);

        /// <summary>
        /// Adds range of years
        /// </summary>
        /// <param name="years"></param>
        /// <returns></returns>
        ActionResponse AddRange(FinancialYearRangeModel years);

        /// <summary>
        /// Adds multiple years, for internal use only
        /// </summary>
        /// <param name="years"></param>
        /// <returns></returns>
        Task<ActionResponse> AddMultipleAsync(List<int> years);

        /// <summary>
        /// Updates a location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        ActionResponse Update(int id, FinancialYearModel model);
    }

    public class FinancialYearService : IFinancialYearService
    {
        AIMSDbContext context;
        IMapper mapper;

        public FinancialYearService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<FinancialYearView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var years = unitWork.FinancialYearRepository.GetAll();
                return mapper.Map<List<FinancialYearView>>(years);
            }
        }

        public async Task<IEnumerable<FinancialYearView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var years = await unitWork.FinancialYearRepository.GetAllAsync();
                return await Task<IEnumerable<FinancialYearView>>.Run(() => mapper.Map<List<FinancialYearView>>(years)).ConfigureAwait(false);
            }
        }

        public FinancialYearView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var year = unitWork.FinancialYearRepository.GetByID(id);
                return mapper.Map<FinancialYearView>(year);
            }
        }

        public ActionResponse Add(FinancialYearModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isFinancialYearCreated = unitWork.FinancialYearRepository.GetOne(l => l.FinancialYear == model.FinancialYear);
                    if (isFinancialYearCreated != null)
                    {
                        response.ReturnedId = isFinancialYearCreated.Id;
                    }
                    else
                    {
                        var newFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                        {
                            FinancialYear = model.FinancialYear,
                        });
                        unitWork.Save();
                        response.ReturnedId = newFinancialYear.Id;
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

        public ActionResponse AddRange(FinancialYearRangeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                try
                {
                    if (model.StartingYear > model.EndingYear)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetInvalidEndingFinancialYearMessage();
                        response.Success = false;
                        return response;
                    }
                    var financialYears = unitWork.FinancialYearRepository.GetProjection(y => y.Id != 0, y => y.FinancialYear);
                    for(int year = model.StartingYear; year <= model.EndingYear; year++)
                    {
                        var financialYearExists = (from fy in financialYears
                                                   where fy == year
                                                   select fy).FirstOrDefault();

                        if (financialYearExists < 1)
                        {
                            unitWork.FinancialYearRepository.Insert(new EFFinancialYears() { FinancialYear = year });
                        }
                    }
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

        public async Task<ActionResponse> AddMultipleAsync(List<int> years)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            var financialYears = await unitWork.FinancialYearRepository.GetAllAsync();
                            List<int> yearsList = (from year in financialYears
                                                   select year.FinancialYear).ToList<int>();

                            foreach(int year in years)
                            {
                                if (!yearsList.Contains(year))
                                {
                                    unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                    {
                                        FinancialYear = year
                                    });
                                }
                            }
                            unitWork.Save();
                            transaction.Commit();
                        }
                    });
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, FinancialYearModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var yearObj = unitWork.FinancialYearRepository.GetByID(id);
                if (yearObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("FinancialYear");
                    return response;
                }

                yearObj.FinancialYear = model.FinancialYear;

                unitWork.FinancialYearRepository.Update(yearObj);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }
    }
}
