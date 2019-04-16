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
        Task<ActionResponse> AddAsync(EnvelopeModel envelope);

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
                EnvelopeView envelope = new EnvelopeView();
                List<EnvelopeBreakup> envelopeList = new List<EnvelopeBreakup>();
                List<EnvelopeSectorBreakup> sectorsList = new List<EnvelopeSectorBreakup>();
                List<int> projectIds = new List<int>();
                var envelopes = unitWork.EnvelopeRepository.GetManyQueryable(e => e.FunderId == funderId);
                if (envelopes != null)
                {
                    foreach(var e in envelopes)
                    {
                        envelopeList.Add(new EnvelopeBreakup()
                        {
                            TotalAmount = e.TotalAmount,
                            Year = e.Year
                        });
                    }
                }
                var funding = unitWork.ProjectFundersRepository.GetManyQueryable(f => f.FunderId == funderId);
                decimal totalFunding = 0;
                foreach (var fund in funding)
                {
                    totalFunding += fund.Amount;
                }

                var envelopeSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => projectIds.Contains(p.ProjectId), new string[] { "Sector" });
                foreach(var sector in envelopeSectors)
                {
                    sectorsList.Add(new EnvelopeSectorBreakup()
                    {
                        Sector = sector.Sector.SectorName,
                        Percentage = sector.FundsPercentage
                    });
                }
                envelope.Sectors = sectorsList;
                return envelope;
            }
        }


        public async Task<ActionResponse> AddAsync(EnvelopeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var years = (from y in model.FundsBreakup
                                 select y.Year).ToList<int>();

                    List<EFEnvelope> envelopeList = new List<EFEnvelope>();
                    var envelopeFunds = unitWork.EnvelopeRepository.GetManyQueryable(e => e.FunderId == model.FunderId);

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            foreach (var funds in model.FundsBreakup)
                            {
                                var isEnvelopeExists = (from e in envelopeFunds
                                                        where e.Year == funds.Year
                                                        select e).FirstOrDefault();

                                if (isEnvelopeExists == null)
                                {
                                    envelopeList.Add(new EFEnvelope()
                                    {
                                        FunderId = model.FunderId,
                                        TotalAmount = funds.TotalAmount,
                                        Year = funds.Year
                                    });
                                }
                                else
                                {
                                    isEnvelopeExists.TotalAmount = funds.TotalAmount;
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
