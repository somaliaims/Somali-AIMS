﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IFinancialYearSettingsService
    {
        /// <summary>
        /// Gets financial year settings
        /// </summary>
        /// <returns></returns>
        FinancialYearSettingView Get();

        /// <summary>
        /// Adds or updates month and day settings for financial year
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddAsync(FinancialYearSettingModel model, int userId);
    }

    public class FinancialYearSettingsService : IFinancialYearSettingsService
    {
        AIMSDbContext context;

        public FinancialYearSettingsService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public FinancialYearSettingView Get()
        {
            var unitWork = new UnitOfWork(context);
            int currentYear = DateTime.Now.Year, 
                secondPreviousYear = (currentYear - 2),
                previousYear = (currentYear - 1),
                nextYear = (currentYear + 1),
                currentMonth = DateTime.Now.Month,
                currentDay = DateTime.Now.Day;
            int settingsMonth = 0, settingsDay = 0;
            FinancialYearSettingView view = new FinancialYearSettingView();
            var settings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
            if (settings != null)
            {
                settingsMonth = settings.Month;
                settingsDay = settings.Day;

                if (currentMonth < settingsMonth)
                {
                    --secondPreviousYear;
                    --previousYear;
                    --currentYear;
                    --nextYear;
                }
                else if (currentMonth == settingsMonth && currentDay < settingsDay)
                {
                    --secondPreviousYear;
                    --previousYear;
                    --currentYear;
                    --nextYear;
                }
            }
            bool isSimilarToCalendarYear = ((settings.Month == 1 && settings.Day == 1)) ? true : false;
            var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(y => (y.FinancialYear == secondPreviousYear) || (y.FinancialYear == previousYear) ||
                (y.FinancialYear == currentYear) || (y.FinancialYear == nextYear));

            var secondPreviousFinancialYear = (from y in financialYears
                                               where y.FinancialYear == secondPreviousYear
                                               select y).FirstOrDefault();

            var previousFinancialYear = (from y in financialYears
                                        where y.FinancialYear == previousYear
                                        select y).FirstOrDefault();

            var currentFinancialYear = (from y in financialYears
                                        where y.FinancialYear == currentYear
                                        select y).FirstOrDefault();

            var nextFinancialYear = (from y in financialYears
                                        where y.FinancialYear == nextYear
                                        select y).FirstOrDefault();

            if (secondPreviousFinancialYear == null)
            {
                string label = (isSimilarToCalendarYear) ? "FY " + secondPreviousYear : "FY " + (secondPreviousYear) + "/" + (secondPreviousYear + 1);
                secondPreviousFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                {
                    FinancialYear = secondPreviousYear,
                    Label = label
                });
                unitWork.Save();
            }

            if (previousFinancialYear == null)
            {
                string label = (isSimilarToCalendarYear) ? "FY " + previousYear : "FY " + (previousYear) + "/" + (previousYear + 1);
                previousFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                {
                    FinancialYear = previousYear,
                    Label = label
                });
                unitWork.Save();
            }

            if (currentFinancialYear == null)
            {
                string label = (isSimilarToCalendarYear) ? "FY " + currentYear : "FY " + (currentYear) + "/" + (currentYear + 1);
                currentFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                {
                    FinancialYear = currentYear,
                    Label = label
                });
                unitWork.Save();
            }

            if (nextFinancialYear == null)
            {
                string label = (isSimilarToCalendarYear) ? "FY " + nextYear : "FY " + (nextYear) + "/" + (nextYear + 1);
                nextFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                {
                    FinancialYear = currentYear,
                    Label = label
                });
                unitWork.Save();
            }

            view.Day = settingsDay;
            view.Month = settingsMonth;
            view.SecondPreviousFinancialYear = secondPreviousFinancialYear.FinancialYear;
            view.SecondPreviousFinancialYearLabel = secondPreviousFinancialYear.Label;
            view.PreviousFinancialYear = previousFinancialYear.FinancialYear;
            view.PreviousFinancialYearLabel = previousFinancialYear.Label;
            view.CurrentFinancialYear = currentFinancialYear.FinancialYear;
            view.CurrentFinancialYearLabel = currentFinancialYear.Label;
            view.NextFinancialYear = nextFinancialYear.FinancialYear;
            view.NextFinancialYearLabel = nextFinancialYear.Label;
            return view;
        }

        public async Task<ActionResponse> AddAsync(FinancialYearSettingModel model, int userId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    IMessageHelper mHelper;
                    DateTime dated = DateTime.Now;
                    int year = dated.Year;
                    string dateStr = year + "-" + model.Month + "-" + model.Day;
                    bool isValidDate = DateTime.TryParse(dateStr, out dated);
                    if (!isValidDate)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.InvalidMonthDayFound();
                        response.Success = false;
                        return response;
                    }

                    var user = unitWork.UserRepository.GetOne(u => u.Id == userId);
                    if (user == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("User");
                        response.Success = false;
                        return response;
                    }
                    var fySettings = await unitWork.FinancialYearSettingsRepository.GetOneAsync(f => f.Id != 0);
                    if (fySettings != null)
                    {
                        if (fySettings.Month == model.Month && fySettings.Day == model.Day)
                        {
                            response.Success = true;
                            return response;
                        }
                    }

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            if (fySettings != null)
                            {
                                fySettings.Day = model.Day;
                                fySettings.Month = model.Month;
                                unitWork.FinancialYearSettingsRepository.Update(fySettings);
                            }
                            else
                            {
                                fySettings = unitWork.FinancialYearSettingsRepository.Insert(new EFFinancialYearSettings()
                                {
                                    Month = dated.Month,
                                    Day = dated.Day
                                });
                            }
                            unitWork.Save();

                            //Update financial year labels
                            var years = unitWork.FinancialYearRepository.GetManyQueryable(y => y.Id != 0);
                            bool isSimilarToCalendarYear = (fySettings.Month == 1 && fySettings.Day == 1) ? true : false;
                            foreach (var fy in years)
                            {
                                int yr = fy.FinancialYear;
                                fy.Label = (isSimilarToCalendarYear) ? "FY " + yr : "FY " + yr + "/" + (yr + 1);
                                unitWork.FinancialYearRepository.Update(fy);
                            }
                            unitWork.Save();

                            //Update project financial years
                            int month = 1, day = 1;
                            if (fySettings != null)
                            {
                                month = fySettings.Month;
                                day = fySettings.Day;
                            }

                            EFFinancialYears startingFinancialYear, endingFinancialYear = null;
                            var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(y => y.Id != 0);
                            var projects = unitWork.ProjectRepository.GetWithInclude(p => p.Id != 0, new string[] { "StartingFinancialYear", "EndingFinancialYear" });
                            foreach (var project in projects)
                            {
                                int fyStartingYear = project.StartDate.Year;
                                int fyEndingYear = project.EndDate.Year;
                                if (isSimilarToCalendarYear)
                                {
                                    startingFinancialYear = (from fy in financialYears
                                                             where fy.FinancialYear == fyStartingYear
                                                             select fy).FirstOrDefault();

                                    if (startingFinancialYear == null)
                                    {
                                        startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                        {
                                            FinancialYear = fyStartingYear,
                                            Label = "FY " + fyStartingYear
                                        });
                                    }
                                    else
                                    {
                                        startingFinancialYear.Label = "FY " + fyStartingYear;
                                    }
                                    unitWork.Save();

                                    endingFinancialYear = (from fy in financialYears
                                                           where fy.FinancialYear == fyEndingYear
                                                           select fy).FirstOrDefault();
                                    if (endingFinancialYear == null)
                                    {
                                        endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                        {
                                            FinancialYear = fyEndingYear,
                                            Label = "FY " + fyEndingYear
                                        });
                                    }
                                    else
                                    {
                                        endingFinancialYear.Label = "FY " + fyEndingYear;
                                    }
                                    project.StartingFinancialYear = startingFinancialYear;
                                    project.EndingFinancialYear = endingFinancialYear;
                                    unitWork.ProjectRepository.Update(project);
                                    unitWork.Save();
                                }
                                else
                                {
                                    int startingMonth = project.StartDate.Month;
                                    int startingDay = project.StartDate.Day;
                                    int endingMonth = project.EndDate.Month;
                                    int endingDay = project.EndDate.Day;

                                    if (startingMonth < month)
                                    {
                                        --fyStartingYear;
                                    }
                                    else if (startingMonth == month && startingDay < day)
                                    {
                                        --fyStartingYear;
                                    }

                                    startingFinancialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == fyStartingYear);
                                    if (startingFinancialYear == null)
                                    {
                                        startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                        {
                                            FinancialYear = fyStartingYear,
                                            Label = "FY " + fyStartingYear + "/" + (fyStartingYear + 1)
                                        });
                                        unitWork.Save();
                                    }

                                    if (endingMonth < month)
                                    {
                                        --fyEndingYear;
                                    }
                                    else if (endingMonth == month && endingDay < day)
                                    {
                                        --fyEndingYear;
                                    }

                                    endingFinancialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == fyEndingYear);
                                    if (endingFinancialYear == null)
                                    {
                                        endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                        {
                                            FinancialYear = fyEndingYear,
                                            Label = "FY " + fyEndingYear + "/" + (fyStartingYear + 1)
                                        });
                                        unitWork.Save();
                                    }

                                    project.StartingFinancialYear = startingFinancialYear;
                                    project.EndingFinancialYear = endingFinancialYear;
                                    unitWork.ProjectRepository.Update(project);
                                    unitWork.Save();
                                }
                            }

                            if (projects.Count() > 0)
                            {
                                unitWork.Save();
                            }

                            //Adjust disbursements
                            int fyMonth = model.Month, fyDay = model.Day;
                            int startingYear = 0, endingYear = 0, startMonth = 0, startDay = 0, currentActiveYear = DateTime.Now.Year;
                            if (!isSimilarToCalendarYear)
                            {
                                if (startMonth < fyMonth)
                                {
                                    --currentActiveYear;
                                }
                                else if (startMonth == fyMonth && startDay < fyDay)
                                {
                                    --currentActiveYear;
                                }
                            }

                            var adjustProjects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.EndDate.Year >= currentActiveYear, new string[] { "StartingFinancialYear", "EndingFinancialYear", "Disbursements", "Disbursements.Year" });
                            foreach (var project in adjustProjects)
                            {
                                startMonth = project.StartDate.Month;
                                startDay = project.StartDate.Day;
                                startingYear = project.StartingFinancialYear.FinancialYear;
                                endingYear = project.EndingFinancialYear.FinancialYear;
                                
                                var disbursementsToDelete = (from disbursement in project.Disbursements
                                                             where ((disbursement.Year.FinancialYear < startingYear) ||
                                                             (disbursement.Year.FinancialYear > endingYear)) ||
                                                             (disbursement.Year.FinancialYear < currentActiveYear &&
                                                             disbursement.DisbursementType == DisbursementTypes.Planned) ||
                                                             (disbursement.Year.FinancialYear > currentActiveYear &&
                                                                disbursement.DisbursementType == DisbursementTypes.Actual)
                                                             select disbursement);
                                decimal deletedActualDisbursements = 0, deletedPlannedDisbursements = 0;
                                foreach (var disbursement in disbursementsToDelete)
                                {
                                    if (disbursement.DisbursementType == DisbursementTypes.Actual && disbursement.Year.FinancialYear > currentActiveYear)
                                    {
                                        deletedActualDisbursements += disbursement.Amount;
                                        unitWork.ProjectDisbursementsRepository.Delete(disbursement);
                                    }
                                    else if (disbursement.DisbursementType == DisbursementTypes.Planned && disbursement.Year.FinancialYear < currentActiveYear)
                                    {
                                        deletedPlannedDisbursements += disbursement.Amount;
                                        unitWork.ProjectDisbursementsRepository.Delete(disbursement);
                                    }
                                }
                                if (disbursementsToDelete.Any())
                                {
                                    unitWork.Save();
                                }

                                if (project.StartingFinancialYear.FinancialYear <= currentActiveYear)
                                {
                                    var disbursementsForCurrentYear = (from disbursement in project.Disbursements
                                                                       where (disbursement.Year.FinancialYear == currentActiveYear)
                                                                       select disbursement);
                                    var actualDisbursementCurrentYear = (from disbursement in disbursementsForCurrentYear
                                                                         where disbursement.DisbursementType == DisbursementTypes.Actual
                                                                         select disbursement).FirstOrDefault();
                                    var plannedDisbursementCurrentYear = (from disbursement in disbursementsForCurrentYear
                                                                          where disbursement.DisbursementType == DisbursementTypes.Planned
                                                                          select disbursement).FirstOrDefault();
                                    var currentFinancialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == currentActiveYear);
                                    if (currentFinancialYear == null)
                                    {
                                        string label = (fyMonth == 1 && fyDay == 1) ? "FY " + currentActiveYear : "FY " + currentActiveYear + "/" + (currentActiveYear + 1);
                                        currentFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                        {
                                            FinancialYear = currentActiveYear,
                                            Label = label
                                        });
                                        unitWork.Save();
                                    }

                                    if (actualDisbursementCurrentYear == null)
                                    {
                                        actualDisbursementCurrentYear = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = currentFinancialYear,
                                            Currency = project.ProjectCurrency,
                                            Amount = deletedActualDisbursements,
                                            ExchangeRate = project.ExchangeRate,
                                            DisbursementType = DisbursementTypes.Actual
                                        });
                                        unitWork.Save();
                                    }
                                    else
                                    {
                                        actualDisbursementCurrentYear.Amount += deletedActualDisbursements;
                                        unitWork.ProjectDisbursementsRepository.Update(actualDisbursementCurrentYear);
                                        unitWork.Save();
                                    }
                                    if (plannedDisbursementCurrentYear == null)
                                    {
                                        plannedDisbursementCurrentYear = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                        {
                                            Project = project,
                                            Year = currentFinancialYear,
                                            Currency = project.ProjectCurrency,
                                            Amount = deletedPlannedDisbursements,
                                            ExchangeRate = project.ExchangeRate,
                                            DisbursementType = DisbursementTypes.Planned
                                        });
                                        unitWork.Save();
                                    }
                                    else
                                    {
                                        plannedDisbursementCurrentYear.Amount += deletedPlannedDisbursements;
                                        unitWork.ProjectDisbursementsRepository.Update(plannedDisbursementCurrentYear);
                                        unitWork.Save();
                                    }
                                }

                                if (project.EndingFinancialYear.FinancialYear >= currentActiveYear)
                                {
                                    var projectDisbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(p => p.ProjectId == project.Id, new string[] { "Year" });
                                    for(int yr = (currentActiveYear); yr <= project.EndingFinancialYear.FinancialYear; yr++)
                                    {
                                        var yearDisbursement = (from d in projectDisbursements
                                                                where d.Year.FinancialYear == yr &&
                                                                d.DisbursementType == DisbursementTypes.Planned
                                                                select d).FirstOrDefault();
                                        var financialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == yr);
                                        if (financialYear == null)
                                        {
                                            string label = (isSimilarToCalendarYear) ? "FY " + yr : "FY " + yr + "/" + (yr + 1);
                                            financialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                            {
                                                Label = label,
                                                FinancialYear = yr
                                            });
                                            unitWork.Save();
                                        }

                                        if (yearDisbursement == null)
                                        {
                                            yearDisbursement = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                            {
                                                Project = project,
                                                Year = financialYear,
                                                Amount = 0,
                                                DisbursementType = DisbursementTypes.Planned
                                            });
                                            unitWork.Save();
                                        }
                                    }
                                }

                                var transition = unitWork.FinancialTransitionRepository.Insert(new EFFinancialYearTransition()
                                {
                                    Year = currentActiveYear,
                                    AppliedOn = DateTime.Now,
                                    AppliedBy = user,
                                    IsAutomated = false
                                });
                                await unitWork.SaveAsync();

                            }
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
    }
}
