using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

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
        ActionResponse Add(FinancialYearSettingModel model);
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

        public ActionResponse Add(FinancialYearSettingModel model)
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

                    var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(f => f.Id != 0);
                    if (fySettings != null)
                    {
                        fySettings.Day = model.Day;
                        fySettings.Month = model.Month;
                        unitWork.FinancialYearSettingsRepository.Update(fySettings);
                    }
                    else
                    {
                        unitWork.FinancialYearSettingsRepository.Insert(new EFFinancialYearSettings()
                        {
                            Month = dated.Month,
                            Day = dated.Day
                        });
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
    }
}
