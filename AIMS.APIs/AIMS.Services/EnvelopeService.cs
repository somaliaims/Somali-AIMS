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
        EnvelopeView GetFunderEnvelope(int funderId);

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
        ActionResponse Delete(int funderId, int year);
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

        public EnvelopeView GetFunderEnvelope(int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                decimal projectValue = 0, actualFundingAmount = 0;
                int currentYear = DateTime.Now.Year;
                int previousYear = currentYear - 1;
                int upperThreeYearsLimit = currentYear + 3;
                EnvelopeView envelope = new EnvelopeView();
                List<EnvelopeBreakup> envelopeList = new List<EnvelopeBreakup>();
                List<EnvelopeSectorBreakup> sectorsList = new List<EnvelopeSectorBreakup>();
                List<int> projectIds = new List<int>();

                var envelopes = unitWork.EnvelopeRepository.GetManyQueryable(e => e.FunderId == funderId);
                if (envelopes != null)
                {
                    foreach(var e in envelopes)
                    {
                        envelope.Currency = e.Currency;
                        envelopeList.Add(new EnvelopeBreakup()
                        {
                            ActualAmount = e.TotalAmount,
                            ExpectedAmount = e.ExpectedAmount,
                            Year = e.Year
                        });
                    }
                }

                var funderProjects = unitWork.ProjectFundersRepository.GetManyQueryable(f => f.FunderId == funderId);
                if (funderProjects != null)
                {
                    if (envelope.Currency == null)
                    {
                        envelope.Currency = (from f in funderProjects
                                             select f.Currency).FirstOrDefault();
                    }
                    projectIds = (from p in funderProjects
                                  select p.ProjectId).ToList<int>();

                    projectValue = (from p in funderProjects
                                          select p.Amount).Sum();
                }

                var disbursements = unitWork.ProjectDisbursementsRepository.GetManyQueryable(d => projectIds.Contains(d.ProjectId));
                
                if (disbursements.Count() > 0)
                {
                    disbursements = (from disbursement in disbursements
                                     where (disbursement.Dated.Year >= previousYear && disbursement.Dated.Year <= upperThreeYearsLimit)
                                     orderby disbursement.Dated
                                     select disbursement);
                }

                int year = 0;
                int yearsLeft = 0;
                decimal disbursementsValue = 0;
                decimal expectedFunds = 0;
                var lastElement = disbursements.LastOrDefault();
                foreach (var disbursement in disbursements)
                {
                    yearsLeft = upperThreeYearsLimit - year;

                    if (year != disbursement.Dated.Year)
                    {
                        var isEnvelopeExists = (from e in envelopeList
                                                where e.Year == disbursement.Dated.Year
                                                select e).FirstOrDefault();

                        if (isEnvelopeExists == null)
                        {
                            envelopeList.Add(new EnvelopeBreakup()
                            {
                                Year = year,
                                ActualAmount = disbursement.Amount,
                            });
                        }
                        else
                        {
                            isEnvelopeExists.ActualAmount = disbursement.Amount;
                        }
                    }
                    year = disbursement.Dated.Year;

                    if (disbursement == lastElement)
                    {
                        envelopeList.Add(new EnvelopeBreakup()
                        {
                            Year = year,
                            ActualAmount = disbursement.Amount,
                        });
                    }
                }

                envelopeList = (from e in envelopeList
                                        orderby e.Year ascending
                                        select e).ToList();

                List<EnvelopeBreakup> requiredEnvelopeList = new List<EnvelopeBreakup>();
                for(int y = previousYear; y <= upperThreeYearsLimit; y++)
                {
                    var yearEnvelope = (from e in envelopeList
                                       where e.Year == y
                                       select e).FirstOrDefault();

                    disbursementsValue = (from r in requiredEnvelopeList
                                           where r.Year <= y
                                           select r.ExpectedAmount).Sum();

                    yearsLeft = upperThreeYearsLimit - y;
                    expectedFunds = 0;
                    if (yearsLeft > 1)
                    {
                        expectedFunds = Math.Round((projectValue - disbursementsValue) / yearsLeft);
                    }
                    else if (yearsLeft == 1)
                    {
                        expectedFunds = Math.Round((projectValue - disbursementsValue) / 2);
                    }
                    else if(yearsLeft == 0)
                    {
                        expectedFunds = Math.Round(projectValue - disbursementsValue);
                    }
                    

                    if (yearEnvelope == null)
                    {
                        decimal exFunds = 0;
                        if (y >= currentYear)
                        {
                            exFunds = expectedFunds;
                        }
                        requiredEnvelopeList.Add(new EnvelopeBreakup()
                        {
                            Year = y,
                            ActualAmount = 0,
                            ExpectedAmount = exFunds
                        });
                    }
                    else
                    {
                        if (yearEnvelope.ExpectedAmount == 0 && yearEnvelope.ActualAmount > 0)
                        {
                            yearEnvelope.ExpectedAmount = expectedFunds;
                        }
                        else if (yearEnvelope.ActualAmount == 0 && y < currentYear)
                        {
                            yearEnvelope.ExpectedAmount = 0;
                        }
                        requiredEnvelopeList.Add(yearEnvelope);
                    }
                }

                actualFundingAmount = (from e in requiredEnvelopeList
                                       select e.ActualAmount).Sum();

                var envelopeSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => projectIds.Contains(p.ProjectId), new string[] { "Sector" });
                foreach(var sector in envelopeSectors)
                {
                    decimal allocatedAmount = 0;

                    List<SectorYearlyAllocation> allocationList = new List<SectorYearlyAllocation>();
                    foreach(var yr in requiredEnvelopeList)
                    {
                        allocatedAmount = (from e in requiredEnvelopeList
                                            where e.Year == yr.Year
                                            select e.ActualAmount).First();

                        if (allocatedAmount > 0)
                        {
                            allocatedAmount = ((allocatedAmount / 100) * sector.FundsPercentage);
                        }
                        allocationList.Add(new SectorYearlyAllocation()
                        {
                            Year = yr.Year,
                            Amount = allocatedAmount
                        });
                    }

                    sectorsList.Add(new EnvelopeSectorBreakup()
                    {
                        Sector = sector.Sector.SectorName,
                        Percentage = sector.FundsPercentage,
                        YearlyAllocation = allocationList
                    });
                }
                envelope.EnvelopeBreakups = requiredEnvelopeList;
                envelope.ActualFunds = actualFundingAmount;
                envelope.ExpectedFunds = projectValue;
                envelope.Sectors = sectorsList;
                return envelope;
            }
        }


        public async Task<ActionResponse> AddAsync(EnvelopeModel model, int funderId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var years = (from y in model.EnvelopeBreakups
                                 select y.Year).ToList<int>();

                    List<EFEnvelope> envelopeList = new List<EFEnvelope>();
                    var envelopeFunds = unitWork.EnvelopeRepository.GetManyQueryable(e => e.FunderId == funderId);

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            foreach (var funds in model.EnvelopeBreakups)
                            {
                                var isEnvelopeExists = (from e in envelopeFunds
                                                        where e.Year == funds.Year
                                                        select e).FirstOrDefault();

                                if (isEnvelopeExists == null)
                                {
                                    envelopeList.Add(new EFEnvelope()
                                    {
                                        FunderId = funderId,
                                        Currency = model.Currency,
                                        TotalAmount = funds.ActualAmount,
                                        ExpectedAmount = funds.ExpectedAmount,
                                        Year = funds.Year
                                    });
                                }
                                else
                                {
                                    isEnvelopeExists.TotalAmount = funds.ActualAmount;
                                    isEnvelopeExists.ExpectedAmount = funds.ExpectedAmount;
                                    unitWork.EnvelopeRepository.Update(isEnvelopeExists);
                                }
                            }

                            unitWork.EnvelopeRepository.InsertMultiple(envelopeList);
                            await unitWork.SaveAsync();
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


        public ActionResponse Delete(int funderId, int year)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper = new MessageHelper();
                ActionResponse response = new ActionResponse();

                var envelope = unitWork.EnvelopeRepository.GetOne(e => e.FunderId == funderId && e.Year == year);
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
