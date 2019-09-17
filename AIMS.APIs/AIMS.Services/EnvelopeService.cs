using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AIMS.Services
{
    public interface IEnvelopeService
    {
        /// <summary>
        /// Gets all envelopes
        /// </summary>
        /// <returns></returns>
        IEnumerable<EnvelopeView> GetAll();

        /// <summary>
        /// Gets envelope data for funder
        /// </summary>
        /// <param name="funderId"></param>
        /// <returns></returns>
        EnvelopeYearlyView GetFunderEnvelope(int funderId);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        Task<ActionResponse> AddAsync(EnvelopeModel envelope, int funderId);

        /// <summary>
        /// Deletes a relevant funder row from envelope data
        /// </summary>
        /// <param name="funderId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        ActionResponse Delete(int funderId);
    }

    public class EnvelopeService : IEnvelopeService
    {
        AIMSDbContext context;
        IMapper mapper;

        public EnvelopeService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<EnvelopeView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var envelopes = unitWork.EnvelopeRepository.GetAll();
                return mapper.Map<List<EnvelopeView>>(envelopes);
            }
        }

        public EnvelopeYearlyView GetFunderEnvelope(int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                int currentYear = DateTime.Now.Year;
                int previousYear = currentYear - 1;
                int upperYearLimit = currentYear + 1;
                EnvelopeYearlyView envelopeView = new EnvelopeYearlyView();
                envelopeView.EnvelopeBreakupsByType = new List<EnvelopeBreakupView>();
                List<int> projectIds = new List<int>();

                var envelopeTypes = unitWork.EnvelopeTypesRepository.GetManyQueryable(e => e.Id != 0);
                var envelope = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == funderId);
                IQueryable<EFEnvelopeYearlyBreakup> yearlyBreakup = null;
                if (envelope != null)
                {
                    envelopeView.Currency = envelope.Currency;
                    envelopeView.FunderId = funderId;

                    yearlyBreakup = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(
                        f => f.EnvelopeId == envelope.Id && f.Year.FinancialYear >= previousYear, new string[] { "Year", "EnvelopeType" });
                }

                if (yearlyBreakup != null)
                {
                    yearlyBreakup = (from yb in yearlyBreakup
                                     orderby yb.Year.FinancialYear ascending
                                     select yb);
                }


                IQueryable<EFEnvelopeYearlyBreakup> yearBreakup = null;

                foreach (var type in envelopeTypes)
                {
                    EnvelopeBreakupView breakupView = new EnvelopeBreakupView();
                    breakupView.EnvelopeType = type.TypeName;
                    breakupView.EnvelopeTypeId = type.Id;

                    List<EnvelopeYearlyBreakUp> yearlyBreakupList = new List<EnvelopeYearlyBreakUp>();
                    for (int year = previousYear; year <= upperYearLimit; year++)
                    {
                        if (yearlyBreakup != null)
                        {
                            yearBreakup = (from yb in yearlyBreakup
                                           where yb.Year.FinancialYear == year
                                           select yb);
                        }

                        EFEnvelopeYearlyBreakup isBreakupExists = null;
                        if (yearBreakup != null)
                        {
                            isBreakupExists = (from typeBreakup in yearBreakup
                                               where typeBreakup.EnvelopeTypeId == type.Id
                                               select typeBreakup).FirstOrDefault();
                        }

                        if (isBreakupExists != null)
                        {
                            yearlyBreakupList.Add(new EnvelopeYearlyBreakUp()
                            {
                                Amount = isBreakupExists.Amount,
                                Year = year
                            });
                        }
                        else
                        {
                            yearlyBreakupList.Add(new EnvelopeYearlyBreakUp()
                            {
                                Amount = 0,
                                Year = year,
                            });
                        }
                    }
                    breakupView.YearlyBreakup = yearlyBreakupList;
                    envelopeView.EnvelopeBreakupsByType.Add(breakupView);
                }
                return envelopeView;
            }
        }


        public async Task<ActionResponse> AddAsync(EnvelopeModel model, int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var envelope = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == funderId);
                    int currentYear = DateTime.Now.Year;
                    var financialYears = unitWork.FinancialYearRepository.GetManyQueryable(y => y.FinancialYear >= (currentYear - 1) && y.FinancialYear <= (currentYear + 1));
                    var envelopeTypes = unitWork.EnvelopeTypesRepository.GetManyQueryable(e => e.Id != 0);

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            if (envelope == null)
                            {
                                envelope = unitWork.EnvelopeRepository.Insert(new EFEnvelope()
                                {
                                    FunderId = funderId,
                                    Currency = model.Currency,
                                    ExchangeRate = model.ExchangeRate,
                                });
                            }
                            else
                            {
                                envelope.Currency = model.Currency;
                                envelope.ExchangeRate = model.ExchangeRate;
                                unitWork.EnvelopeRepository.Update(envelope);
                            }
                            await unitWork.SaveAsync();

                            var yearlyBreakups = unitWork.EnvelopeYearlyBreakupRepository.GetWithInclude(e => e.EnvelopeId == envelope.Id, new string[] { "Year" });
                            List<EFEnvelopeYearlyBreakup> newYearlyBreakups = new List<EFEnvelopeYearlyBreakup>();

                            foreach(var breakup in model.EnvelopeBreakups)
                            {
                                var isBreakupExists = (from b in yearlyBreakups
                                                       where b.Year.FinancialYear == breakup.Year && envelope.Id == b.EnvelopeId 
                                                       && b.EnvelopeTypeId == breakup.EnvelopeTypeId 
                                                       select b).FirstOrDefault();

                                if (isBreakupExists == null)
                                {
                                    var financialYear = (from fy in financialYears
                                                         where fy.FinancialYear == breakup.Year
                                                         select fy).FirstOrDefault();
                                    if (financialYear == null)
                                    {
                                        financialYear = unitWork.FinancialYearRepository.Insert(new EFFinancialYears()
                                        {
                                            FinancialYear = breakup.Year
                                        });
                                        unitWork.Save();
                                    }
                                    var envelopeType = (from t in envelopeTypes
                                                        where t.Id == breakup.EnvelopeTypeId
                                                        select t).FirstOrDefault();

                                    if (envelopeType != null)
                                    {
                                        newYearlyBreakups.Add(new EFEnvelopeYearlyBreakup()
                                        {
                                            EnvelopeType = envelopeType,
                                            Year = financialYear,
                                            Amount = breakup.Amount,
                                            Envelope = envelope
                                        });
                                    }
                                }
                                else
                                {
                                    isBreakupExists.Amount = breakup.Amount;
                                    unitWork.EnvelopeYearlyBreakupRepository.Update(isBreakupExists);
                                    unitWork.Save();
                                }
                            }
                            if (newYearlyBreakups.Count > 0)
                            {
                                unitWork.EnvelopeYearlyBreakupRepository.InsertMultiple(newYearlyBreakups);
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


        public ActionResponse Delete(int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper = new MessageHelper();
                ActionResponse response = new ActionResponse();

                var envelope = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == funderId);
                if (envelope == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Envelope Data");
                    response.Success = false;
                    return response;
                }

                unitWork.EnvelopeRepository.Delete(envelope);
                unitWork.Save();
                return response;
            }
        }
    }
}
