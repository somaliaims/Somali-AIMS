using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IFinancialYearSettingsService
    {
        ActionResponse Add(FinancialYearSettingModel model);
    }

    public class FinancialYearSettingsService
    {
        AIMSDbContext context;

        public FinancialYearSettingsService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public ActionResponse Add(FinancialYearSettingModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                return response;
            }
        }
    }
}
