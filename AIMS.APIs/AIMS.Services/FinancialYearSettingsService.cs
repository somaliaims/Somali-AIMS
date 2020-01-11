using AIMS.DAL.EF;
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
        FinancialYearSettingModel Get();

        /// <summary>
        /// Adds or updates month and day settings for financial year
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddAsync(FinancialYearSettingModel model);
    }

    public class FinancialYearSettingsService : IFinancialYearSettingsService
    {
        AIMSDbContext context;

        public FinancialYearSettingsService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public FinancialYearSettingModel Get()
        {
            var unitWork = new UnitOfWork(context);
            FinancialYearSettingModel view = new FinancialYearSettingModel();
            var settings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
            if (settings != null)
            {
                view.Month = settings.Month;
                view.Day = settings.Day;
            }
            return view;
        }

        public async Task<ActionResponse> AddAsync(FinancialYearSettingModel model)
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
                        return response;
                    }

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            var fySettings = await unitWork.FinancialYearSettingsRepository.GetOneAsync(f => f.Id != 0);
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
                            int month = 0, day = 1;
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
                                    unitWork.Save();
                                }
                            }

                            //Adjust disbursements
                            int currentActiveYear = DateTime.Now.Year, currentMonth = DateTime.Now.Month, currentDay = DateTime.Now.Day;
                            int fyMonth = 0, fyDay = 0;
                            if (fySettings != null)
                            {
                                fyMonth = fySettings.Month;
                                fyDay = fySettings.Day;
                                if (currentMonth < fyMonth)
                                {
                                    --currentActiveYear;
                                }
                                else if (currentMonth == fyMonth && currentDay <= fyDay)
                                {
                                    --currentActiveYear;
                                }
                            }

                            int endingYear = 0;
                            var adjustProjects = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.EndDate.Year >= currentActiveYear, new string[] { "EndingFinancialYear", "Disbursements", "Disbursements.Year" });

                            foreach (var project in adjustProjects)
                            {
                                endingYear = project.EndingFinancialYear.FinancialYear;
                                if (currentMonth > fyMonth || (currentMonth == fyMonth && currentDay > fyDay))
                                {
                                    //Delete any planned disbursements for previous year
                                    var disbursementsToDelete = (from disbursement in project.Disbursements
                                                                 where disbursement.Year.FinancialYear == currentActiveYear &&
                                                                 disbursement.DisbursementType == DisbursementTypes.Planned
                                                                 select disbursement);
                                    int deleted = 0;
                                    decimal deletedAmount = 0;
                                    if (disbursementsToDelete.Any())
                                    {
                                        foreach (var disbursement in disbursementsToDelete)
                                        {
                                            deletedAmount += disbursement.Amount;
                                            unitWork.ProjectDisbursementsRepository.Delete(disbursement);
                                            deleted++;
                                        }

                                        if (deleted > 0)
                                        {
                                            unitWork.Save();
                                            var nextPlannedDisbursements = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.ProjectId == project.Id && d.Year.FinancialYear > currentActiveYear
                                        && d.DisbursementType == DisbursementTypes.Planned, new string[] { "Year" });
                                            if (nextPlannedDisbursements.Any())
                                            {
                                                var adjustedAmount = (deletedAmount / nextPlannedDisbursements.Count());
                                                foreach (var planned in nextPlannedDisbursements)
                                                {
                                                    planned.Amount += adjustedAmount;
                                                    unitWork.ProjectDisbursementsRepository.Update(planned);
                                                }
                                                unitWork.Save();
                                            }
                                            else
                                            {
                                                if (endingYear > currentActiveYear)
                                                {
                                                    var financialYear = unitWork.FinancialYearRepository.GetOne(y => y.FinancialYear == (currentActiveYear + 1));
                                                    if (financialYear == null)
                                                    {
                                                        unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                                        {
                                                            FinancialYear = currentActiveYear,
                                                            Label = "FY " + currentActiveYear + "/" + (currentActiveYear + 1)
                                                        });
                                                        unitWork.Save();
                                                    }

                                                    var plannedDisbursement = unitWork.ProjectDisbursementsRepository.GetWithInclude(d => d.Year.FinancialYear == (currentActiveYear + 1),
                                                        new string[] { "Year" }).FirstOrDefault();

                                                    if (plannedDisbursement == null)
                                                    {
                                                        plannedDisbursement = unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                                        {
                                                            Project = project,
                                                            Amount = deletedAmount,
                                                            DisbursementType = DisbursementTypes.Planned,
                                                            Year = financialYear
                                                        });
                                                    }
                                                    else
                                                    {
                                                        plannedDisbursement.Amount += deletedAmount;
                                                        unitWork.ProjectDisbursementsRepository.Update(plannedDisbursement);
                                                    }
                                                    unitWork.Save();
                                                }
                                            }
                                        }
                                    }
                                }
                                else if ((currentMonth <= fyMonth && currentDay <= fyDay) && endingYear >= currentActiveYear)
                                {
                                    var plannedDisbursement = unitWork.ProjectDisbursementsRepository.GetWithInclude(p => p.ProjectId == project.Id &&
                                        p.Year.FinancialYear == currentActiveYear && p.DisbursementType == DisbursementTypes.Planned,
                                        new string[] { "Year" }).FirstOrDefault();

                                    if (plannedDisbursement == null)
                                    {
                                        var fYear = unitWork.FinancialYearRepository.GetOne(fy => fy.FinancialYear == currentActiveYear);

                                        if (fYear != null)
                                        {
                                            unitWork.ProjectDisbursementsRepository.Insert(new EFProjectDisbursements()
                                            {
                                                Year = fYear,
                                                Project = project,
                                                Amount = 0,
                                                DisbursementType = DisbursementTypes.Planned
                                            });
                                            unitWork.Save();
                                        }
                                    }
                                }
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
