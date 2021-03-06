﻿using AIMS.DAL.EF;
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
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<FinancialYearView> GetYearsForEnvelope();

        /// <summary>
        /// Get all years for envelope
        /// </summary>
        /// <returns></returns>
        IEnumerable<FinancialYearView> GetForEnvelope();

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

        /// <summary>
        /// Amend the financial year labels, temporary api
        /// </summary>
        /// <returns></returns>
        ActionResponse AmendLabels();
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

        public IEnumerable<FinancialYearView> GetYearsForEnvelope()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var envelopeYears = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(y => y.EnvelopeId != 0, new string[] { "Year" });
                var yearsList = (from y in envelopeYears
                                      select y.Year.FinancialYear).Distinct();

                var years = unitWork.FinancialYearRepository.GetManyQueryable(y => yearsList.Contains(y.FinancialYear));
                years = (from y in years
                         orderby y.FinancialYear
                         select y);
                return mapper.Map<List<FinancialYearView>>(years);
            }
        }

        public IEnumerable<FinancialYearView> GetForEnvelope()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                int fyMonth = 1, fyDay = 1;
                var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(fy => fy.Id != 0);
                if (fySettings != null)
                {
                    fyMonth = fySettings.Month;
                    fyDay = fySettings.Day;
                }

                int currentYear = DateTime.Now.Year, currentMonth = DateTime.Now.Month, currentDay = DateTime.Now.Day;
                bool isSimilarToCalendarYear = (fyMonth == 1 && fyDay == 1) ? true : false;
                if (!isSimilarToCalendarYear)
                {
                    if (currentMonth < fyMonth)
                    {
                        --currentYear;
                    }
                    else if (currentMonth == fyMonth && currentDay < fyDay)
                    {
                        --currentYear;
                    }
                }
                
                int previousYear = (currentYear - 2), nextYear = (currentYear + 1);
                var years = unitWork.FinancialYearRepository.GetManyQueryable(y => y.FinancialYear >= previousYear && y.FinancialYear <= nextYear).ToList();
                var previousFinancialYear = (from y in years
                                            where y.FinancialYear == previousYear
                                            select y).FirstOrDefault();
                if (previousFinancialYear == null)
                {
                    string label = (isSimilarToCalendarYear) ? "FY " + previousYear : ("FY " + previousYear + "/" + (previousYear + 1));
                    previousFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                    {
                        Label = label,
                        FinancialYear = previousYear,
                    });
                    unitWork.Save();
                    years.Add(previousFinancialYear);
                }

                var currentFinancialYear = (from y in years
                                             where y.FinancialYear == currentYear
                                             select y).FirstOrDefault();
                if (currentFinancialYear == null)
                {
                    string label = (isSimilarToCalendarYear) ? "FY" + currentYear : ("FY " + currentYear + "/" + (currentYear + 1));
                    currentFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                    {
                        Label = label,
                        FinancialYear = previousYear,
                    });
                    unitWork.Save();
                    years.Add(currentFinancialYear);
                }

                var nextFinancialYear = (from y in years
                                            where y.FinancialYear == nextYear
                                            select y).FirstOrDefault();
                if (nextFinancialYear == null)
                {
                    string label = (isSimilarToCalendarYear) ? "FY" + nextYear : ("FY " + nextYear + "/" + (nextYear + 1));
                    nextFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                    {
                        Label = label,
                        FinancialYear = previousYear,
                    });
                    unitWork.Save();
                    years.Add(nextFinancialYear);
                }
                years = (from y in years
                         orderby y.FinancialYear
                         select y).ToList();
                return mapper.Map<List<FinancialYearView>>(years);
            }
        }

        public ActionResponse AmendLabels()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var years = unitWork.FinancialYearRepository.GetManyQueryable(y => y.Id != 0);
                int currentMonth = DateTime.Now.Month;
                int currentDay = DateTime.Now.Day;
                var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(s => s.Id != 0);
                int settingsMonth = 1, settingsDay = 1;
                if (fySettings != null)
                {
                    settingsMonth = fySettings.Month;
                    settingsDay = fySettings.Day;
                }
                foreach (var year in years)
                {
                    int yr = year.FinancialYear;
                    string label = (settingsMonth == 1 && settingsDay == 1) ? "FY " + yr : "FY " + yr + "/" + (yr + 1);
                    year.Label = label;
                    unitWork.FinancialYearRepository.Update(year);
                }
                unitWork.Save();
                return response;
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
                
                try
                {
                    List<int> years = new List<int>();
                    var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(y => y.Id != 0);
                    int settingsMonth = 0, settingsDay = 0;
                    if (fySettings == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.FinancialYearSettingsMissing();
                        return response;
                    }
                    settingsMonth = fySettings.Month;
                    settingsDay = fySettings.Day;
                    int currentYear = DateTime.Now.Year, yearToCreate = currentYear;
                    if (model.Month < settingsMonth)
                    {
                        years.Add((currentYear - 1));
                        years.Add(currentYear);
                    }
                    else
                    {
                        years.Add(currentYear);
                    }
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
                                Label = "FY " + year + "/" + (year + 1)
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
                /*var yearObj = unitWork.FinancialYearRepository.GetByID(id);
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
                response.Message = "1";*/
                return response;
            }
        }
    }
}
