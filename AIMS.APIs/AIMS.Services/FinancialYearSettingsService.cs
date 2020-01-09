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
            using (var unitWork = new UnitOfWork(context))
            {
                FinancialYearSettingModel view = new FinancialYearSettingModel();
                var settings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
                if (settings != null)
                {
                    view.Month = settings.Month;
                    view.Day = settings.Day;
                }
                return view;
            }
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
                        }
                    });

                        var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
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

                    //Update labels
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
                        int startingYear = project.StartDate.Year;
                        int endingYear = project.EndDate.Year;
                        if (month == 1 && day == 1)
                        {
                            startingFinancialYear = (from fy in financialYears
                                                     where fy.FinancialYear == startingYear
                                                     select fy).FirstOrDefault();

                            if (startingFinancialYear == null)
                            {
                                startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = startingYear,
                                    Label = "FY " + startingYear
                                });
                            }
                            else
                            {
                                startingFinancialYear.Label = "FY " + startingYear;
                            }
                            unitWork.Save();

                            endingFinancialYear = (from fy in financialYears
                                                   where fy.FinancialYear == endingYear
                                                   select fy).FirstOrDefault();
                            if (endingFinancialYear == null)
                            {
                                endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = endingYear,
                                    Label = "FY " + endingYear
                                });
                            }
                            else
                            {
                                endingFinancialYear.Label = "FY " + endingYear;
                            }
                            unitWork.Save();
                        }
                        else if (month >= 1 && day > 1)
                        {
                            int startingMonth = project.StartDate.Month;
                            int startingDay = project.StartDate.Day;
                            int endingMonth = project.EndDate.Month;
                            int endingDay = project.EndDate.Day;

                            if (startingMonth < month)
                            {
                                --startingYear;
                            }
                            else if (startingMonth == month && startingDay < day)
                            {
                                --startingYear;
                            }

                            startingFinancialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == startingYear);
                            if (startingFinancialYear == null)
                            {
                                startingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = startingYear,
                                    Label = "FY " + startingYear + "/" + (startingYear + 1)
                                });
                                unitWork.Save();
                            }

                            if (endingMonth < month)
                            {
                                --endingYear;
                            }
                            else if (endingMonth == month && endingDay < day)
                            {
                                --endingYear;
                            }

                            endingFinancialYear = unitWork.FinancialYearRepository.GetOne(f => f.FinancialYear == endingYear);
                            if (endingFinancialYear == null)
                            {
                                endingFinancialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                {
                                    FinancialYear = endingYear,
                                    Label = "FY " + endingYear + "/" + (startingYear + 1)
                                });
                                unitWork.Save();
                            }

                            project.StartingFinancialYear = startingFinancialYear;
                            project.EndingFinancialYear = endingFinancialYear;
                            unitWork.Save();

                        }
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
    }
}
