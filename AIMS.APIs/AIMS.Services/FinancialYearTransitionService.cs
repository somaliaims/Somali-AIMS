using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IFinancialYearTransitionService
    {
        /// <summary>
        /// Adds transition for the provided year
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Add(int year);

        /// <summary>
        /// Checks if financial transition applied
        /// </summary>
        /// <returns></returns>
        FinancialYearTransitionView IsFinancialTransitionApplied();
    }

    public class FinancialYearTransitionService : IFinancialYearTransitionService
    {
        AIMSDbContext context;
        IMapper mapper;

        public FinancialYearTransitionService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public FinancialYearTransitionView IsFinancialTransitionApplied()
        {
            var unitWork = new UnitOfWork(context);
            FinancialYearTransitionView tView = new FinancialYearTransitionView();
            int fyDay = 1, fyMonth = 1, currentYear = DateTime.Now.Year, currentDay = DateTime.Now.Day,
                currentMonth = DateTime.Now.Month;
            var fySettings = unitWork.FinancialYearSettingsRepository.GetOne(fy => fy.Id != 0);
            if (fySettings != null)
            {
                fyDay = fySettings.Day;
                fyMonth = fySettings.Month;
            }

            if (fyDay != 1 && fyMonth != 1)
            {
                if (fyMonth < currentMonth)
                {
                    --currentYear;
                }
                else if (fyMonth == currentMonth && fyDay < currentDay)
                {
                    --currentYear;
                }
            }

            tView.Year = currentYear;
            var transitionExists = unitWork.FinancialTransitionRepository.GetOne(t => t.Year == currentYear);
            if (transitionExists != null)
            {
                tView.Exists = true;
            }
            return tView;
        }

        public ActionResponse Add(int year)
        {
            ActionResponse response = new ActionResponse();
            var unitWork = new UnitOfWork(context);
            var transition = unitWork.FinancialTransitionRepository.GetOne(t => t.Year == year);
            if (transition == null)
            {
                transition = unitWork.FinancialTransitionRepository.Insert(new EFFinancialYearTransition()
                {
                    Year = year,
                    AppliedOn = DateTime.Now
                });
            }
            return response;
        }

    }
}
