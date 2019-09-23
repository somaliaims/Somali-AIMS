using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AIMS.Services
{
    public interface IEnvelopeTypeService
    {
        /// <summary>
        /// Gets all the data for all envelopes
        /// </summary>
        /// <returns></returns>
        ICollection<EnvelopeTypeView> GetAll();

        /// <summary>
        /// Gets envelope type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EnvelopeTypeView GetById(int id);

        /// <summary>
        /// Adds new envelope type
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        ActionResponse Add(EnvelopeTypeModel model);

        /// <summary>
        /// Updates envelope type for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Update(int id, EnvelopeTypeModel model);

        /// <summary>
        /// Deletes the envelope type and map it to another
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id, int mappingId);
    }

    public class EnvelopeTypeService : IEnvelopeTypeService
    {
        AIMSDbContext context;
        IMapper mapper;

        public EnvelopeTypeService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public ICollection<EnvelopeTypeView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                return mapper.Map<List<EnvelopeTypeView>>(unitWork.EnvelopeTypesRepository.GetAll());
            }
        }

        public EnvelopeTypeView GetById(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                return mapper.Map<EnvelopeTypeView>(unitWork.EnvelopeTypesRepository.GetByID(id));
            }
        }

        public ActionResponse Add(EnvelopeTypeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var envelopeType = unitWork.EnvelopeTypesRepository.GetOne(e => e.TypeName.Equals(model.TypeName, StringComparison.OrdinalIgnoreCase));
                if (envelopeType == null)
                {
                    var newType = unitWork.EnvelopeTypesRepository.Insert(new EFEnvelopeTypes() { TypeName = model.TypeName });
                    unitWork.Save();
                    response.ReturnedId = newType.Id;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, EnvelopeTypeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var envelopeType = unitWork.EnvelopeTypesRepository.GetOne(e => e.Id.Equals(id));
                if (envelopeType == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Envelope Type");
                    return response;
                }

                envelopeType.TypeName = model.TypeName;
                unitWork.EnvelopeTypesRepository.Update(envelopeType);
                unitWork.Save();
                return response;
            }
        }

        public async Task<ActionResponse> DeleteAsync(int id, int mappingId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var envelopeTypes = unitWork.EnvelopeTypesRepository.GetManyQueryable(e => (e.Id == id || e.Id == mappingId));

                var isDeletedOkay = (from e in envelopeTypes
                                     where e.Id == id
                                     select e).FirstOrDefault();

                if (isDeletedOkay == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Envelope Type to delete");
                    response.Success = false;
                    return response;
                }

                var isMappingOkay = (from e in envelopeTypes
                                     where e.Id == mappingId
                                     select e).FirstOrDefault();

                if (isMappingOkay == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Envelope Type to map");
                    response.Success = false;
                    return response;
                }

                try
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            var envelopeBreakups = await unitWork.EnvelopeYearlyBreakupRepository.GetWithIncludeAsync(e => (e.EnvelopeTypeId == id || e.EnvelopeTypeId == mappingId), new string[] { "Year" });
                            var deletingBreakups = (from b in envelopeBreakups
                                                    where b.EnvelopeTypeId == id
                                                    select b);
                            var mappingBreakups = (from b in envelopeBreakups
                                                   where b.EnvelopeTypeId == mappingId
                                                   select b);

                            int amountsUpdated = 0;
                            foreach (var breakup in deletingBreakups)
                            {
                                int year = breakup.Year.FinancialYear;
                                var mappingBreakupExists = (from b in mappingBreakups
                                                            where b.Year.FinancialYear == year
                                                            select b).FirstOrDefault();

                                if (mappingBreakupExists != null)
                                {
                                    mappingBreakupExists.Amount += breakup.Amount;
                                    mappingBreakupExists.Amount = Math.Round(mappingBreakupExists.Amount, MidpointRounding.AwayFromZero);
                                    unitWork.EnvelopeYearlyBreakupRepository.Update(mappingBreakupExists);
                                    amountsUpdated++;
                                }
                            }

                            if (amountsUpdated > 0)
                            {
                                unitWork.Save();
                            }
                            unitWork.EnvelopeTypesRepository.Delete(isDeletedOkay);
                            await unitWork.SaveAsync();
                            transaction.Commit();
                        }
                    });
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
