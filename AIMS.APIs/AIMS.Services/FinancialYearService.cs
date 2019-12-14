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
                years = (from y in years
                         orderby y.FinancialYear
                         select y);
                return mapper.Map<List<FinancialYearView>>(years);
            }
        }

        public async Task<IEnumerable<FinancialYearView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var years = await unitWork.FinancialYearRepository.GetAllAsync();
                years = (from y in years
                         orderby y.FinancialYear
                         select y);
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
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                if (model.FinancialYear < 1900)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetInvalidFinancialYearMessage();
                    response.Success = false;
                    return response;
                }
                
                try
                {
                    List<int> years = new List<int>();
                    int previousYear = 0, yearToCreate = 0, nextYear = 0;
                    previousYear = (model.FinancialYear - 1);
                    nextYear = (model.FinancialYear + 1);

                    if (model.Month > 0 && model.Month <= 6)
                    {
                        years.Add(previousYear);
                    }
                    else if (model.Month > 6 && model.Month <= 12)
                    {
                        years.Add(nextYear);
                    } 
                    else if (model.Month == 0)
                    {
                        years.Add(previousYear);
                        years.Add(nextYear);
                    }

                    yearToCreate = model.FinancialYear;
                    years.Add(yearToCreate);

                    bool newYearsAdded = false;
                    var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(l => years.Contains(l.FinancialYear));
                    foreach(var year in years)
                    {
                        var isYearExist = (from fy in financialYears
                                           where fy.FinancialYear == year
                                           select fy).FirstOrDefault();

                        if (isYearExist == null)
                        {
                            unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                            {
                                FinancialYear = year,
                                Label = "FY " + (year - 1) + "/" + (year)
                            });
                            newYearsAdded = true;
                        }
                    }
                    if (newYearsAdded)
                    {
                        unitWork.Save();
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
                    if (model.StartingYear > model.EndingYear && model.EndingYear != 0)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.StartingYearGreaterThanEnding();
                        response.Success = false;
                        return response;
                    }
                    int startingYear = model.StartingYear, endingYear = model.EndingYear;
                    int startingMonth = model.StartingMonth, endingMonth = model.EndingMonth;
                    if (startingYear != 0)
                    {
                        if (startingMonth <= 6)
                        {
                            if (endingYear == 0)
                            {
                                endingYear = startingYear;
                            }
                            startingYear = (startingYear - 1);
                        }
                        else if (startingMonth > 6)
                        {
                            if (endingYear == 0)
                            {
                                endingYear = (startingYear + 1);
                            }
                        }
                    }

                    if (endingYear != 0)
                    {
                        if (startingYear == 0)
                        {
                            startingYear = endingYear;
                        }

                        if (endingMonth > 6)
                        {
                            endingYear = ++endingYear;
                        }
                        else if(endingMonth <= 6)
                        {
                            if (startingYear == endingYear)
                            {
                                startingYear = (endingYear - 1);
                            }
                        }
                    }

                    var financialYears = unitWork.FinancialYearRepository.GetProjection(y => y.Id != 0, y => y.FinancialYear);
                    for(int year = startingYear; year <= endingYear; year++)
                    {
                        var financialYearExists = (from fy in financialYears
                                                   where fy == year
                                                   select fy).FirstOrDefault();

                        if (financialYearExists < 1)
                        {
                            string yearLabel = "FY " + (year - 1) + "/" + (year); 
                            unitWork.FinancialYearRepository.Insert(new EFFinancialYears() { FinancialYear = year, Label = yearLabel });
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
                                    string label = "FY " + (year - 1) + "/" + year;
                                    unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                    {
                                        FinancialYear = year,
                                        Label = label
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
