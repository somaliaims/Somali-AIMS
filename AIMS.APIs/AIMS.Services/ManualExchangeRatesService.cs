﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using AIMS.Services.Helpers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AIMS.Services
{
    public interface IManualExchangeRatesService
    {
        /// <summary>
        /// Gets list of all manual exchange rates
        /// </summary>
        /// <returns></returns>
        IEnumerable<ManualRatesView> GetAll();

        /// <summary>
        /// Gets all rates for a single national currency
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ManualRatesView> GetForNationalCurrency(string currencyCode);

        /// <summary>
        /// Gets the rate for the specified date
        /// </summary>
        /// <returns></returns>
        ManualRatesView GetByYear(int year);

        /// <summary>
        /// Adds new manual rate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddAsync(ManualRateModel model);

        /// <summary>
        /// Updates the manual rate
        /// </summary>
        /// <param name="id"></param>
        /// <param name=""></param>
        /// <returns></returns>
        ActionResponse Update(int id, ManualRateModel model);

        /// <summary>
        /// Deletes the manual rate for provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
        
    }

    public class ManualExchangeRatesService : IManualExchangeRatesService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ManualExchangeRatesService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ManualRatesView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var rates = unitWork.ManualRatesRepository.GetAll();
                if (rates != null)
                {
                    rates = (from r in rates
                                  orderby r.Year descending
                                  select r);
                }
                return mapper.Map<List<ManualRatesView>>(rates);
            }
        }

        public IEnumerable<ManualRatesView> GetForNationalCurrency(string currencyCode)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var rates = unitWork.ManualRatesRepository.GetManyQueryable(e => e.Currency == currencyCode);
                if (rates != null)
                {
                    rates = (from r in rates
                             orderby r.Year descending
                             select r);
                }
                return mapper.Map<List<ManualRatesView>>(rates);
            }
        }

        public ManualRatesView GetByYear(int year)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var manualRate = unitWork.ManualRatesRepository.GetOne(r => r.Year == year);
                return mapper.Map<ManualRatesView>(manualRate);
            }
        }

        public async Task<ActionResponse> AddAsync(ManualRateModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var manualRate = await unitWork.ManualRatesRepository.GetOneAsync(r => r.Year == model.Year && r.Currency == model.Currency);

                if (manualRate != null)
                {
                    manualRate.ExchangeRate = model.ExchangeRate;
                    manualRate.IsEditedByUser = true;
                    unitWork.ManualRatesRepository.Update(manualRate);
                }
                else
                {
                    unitWork.ManualRatesRepository.Insert(new EFManualExchangeRates()
                    {
                        Currency = model.Currency,
                        DefaultCurrency = model.DefaultCurrency,
                        Year = model.Year,
                        ExchangeRate = model.ExchangeRate,
                        IsEditedByUser = true
                    });
                }

                try
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            await unitWork.SaveAsync();
                            var projectsInYear = unitWork.ProjectRepository.GetManyQueryable(p => p.StartDate.Year == model.Year && p.ProjectCurrency == model.Currency);
                            foreach (var project in projectsInYear)
                            {
                                project.ExchangeRate = model.ExchangeRate;
                                unitWork.ProjectRepository.Update(project);
                            }
                            await unitWork.SaveAsync();

                            transaction.Commit();
                        }
                    });
                    
                }
                catch(Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, ManualRateModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var manualRate = unitWork.ManualRatesRepository.GetOne(r => r.Id == id);

                if (manualRate ==  null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Manual exchange rate");
                    return response;
                }

                if (manualRate != null)
                {
                    manualRate.ExchangeRate = model.ExchangeRate;
                    manualRate.Currency = model.Currency;
                    manualRate.DefaultCurrency = model.DefaultCurrency;
                    manualRate.IsEditedByUser = true;
                    unitWork.ManualRatesRepository.Update(manualRate);
                }

                try
                {
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                }
                return response;
            }
        }

        public ActionResponse Delete(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var manualRate = unitWork.ManualRatesRepository.GetByID(id);
                if (manualRate == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Manual exchange rate");
                    return response;
                }

                unitWork.ManualRatesRepository.Delete(manualRate);
                unitWork.Save();
                return response;
            }
        }
    }
}
