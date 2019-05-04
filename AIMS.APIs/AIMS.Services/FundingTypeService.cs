using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IFundingTypeService
    {
        /// <summary>
        /// Gets all FundingTypes
        /// </summary>
        /// <returns></returns>
        IEnumerable<FundingTypeView> GetAll();

        /// <summary>
        /// Get matching FundingTypes for the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<FundingTypeView> GetMatching(string criteria);

        /// <summary>
        /// Gets the FundingType for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        FundingTypeView Get(int id);
        /// <summary>
        /// Gets all FundingTypes async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FundingTypeView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(FundingTypeModel FundingType);

        /// <summary>
        /// Updates a FundingType
        /// </summary>
        /// <param name="FundingType"></param>
        /// <returns></returns>
        ActionResponse Update(int id, FundingTypeModel FundingType);

        /// <summary>
        /// Deletes a FundingType
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id, int newId);
    }

    public class FundingTypeService : IFundingTypeService
    {
        AIMSDbContext context;
        IMapper mapper;

        public FundingTypeService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<FundingTypeView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var FundingTypes = unitWork.FundingTypeRepository.GetAll();
                return mapper.Map<List<FundingTypeView>>(FundingTypes);
            }
        }

        public async Task<IEnumerable<FundingTypeView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var FundingTypes = await unitWork.FundingTypeRepository.GetAllAsync();
                return await Task<IEnumerable<FundingTypeView>>.Run(() => mapper.Map<List<FundingTypeView>>(FundingTypes)).ConfigureAwait(false);
            }
        }

        public FundingTypeView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var FundingType = unitWork.FundingTypeRepository.GetByID(id);
                return mapper.Map<FundingTypeView>(FundingType);
            }
        }

        public IEnumerable<FundingTypeView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<FundingTypeView> FundingTypesList = new List<FundingTypeView>();
                var FundingTypes = unitWork.FundingTypeRepository.GetMany(o => o.FundingType.Contains(criteria));
                return mapper.Map<List<FundingTypeView>>(FundingTypes);
            }
        }

        public ActionResponse Add(FundingTypeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isFundingTypeCreated = unitWork.FundingTypeRepository.GetOne(l => l.FundingType.ToLower() == model.FundingType.ToLower());
                    if (isFundingTypeCreated != null)
                    {
                        response.ReturnedId = isFundingTypeCreated.Id;
                    }
                    else
                    {
                        var newFundingType = unitWork.FundingTypeRepository.Insert(new EFFundingTypes()
                        {
                            FundingType = model.FundingType,
                        });
                        unitWork.Save();
                        response.ReturnedId = newFundingType.Id;
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

        public ActionResponse Update(int id, FundingTypeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var FundingTypeObj = unitWork.FundingTypeRepository.GetByID(id);
                if (FundingTypeObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("FundingType");
                    return response;
                }

                FundingTypeObj.FundingType = model.FundingType;
                unitWork.FundingTypeRepository.Update(FundingTypeObj);
                unitWork.Save();
                response.Message = true.ToString();
                return response;
            }
        }

        public async Task<ActionResponse> DeleteAsync(int id, int newId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper = new MessageHelper();
                ActionResponse response = new ActionResponse();
                var FundingTypes = await unitWork.FundingTypeRepository.GetManyQueryableAsync(l => (l.Id == id || l.Id == newId));
                if (FundingTypes.Count() < 2 && newId != 0)
                {
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("FundingType");
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                ///TODO: Implement all the dependent child update/delete logic
                await unitWork.SaveAsync();

                if (newId == 0)
                {
                    var FundingTypeToDelete = (from l in FundingTypes
                                            where l.Id == id
                                            select l).FirstOrDefault();
                    unitWork.FundingTypeRepository.Delete(FundingTypeToDelete);
                }
                else
                {
                    //Save all the new dependent children
                }

                await unitWork.SaveAsync();
                response.ReturnedId = newId;
                response.Message = mHelper.DeleteMessage("FundingType");
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }
    }
}
