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
    public interface IGrantTypeService
    {
        /// <summary>
        /// Gets all GrantTypes
        /// </summary>
        /// <returns></returns>
        IEnumerable<GrantTypeView> GetAll();

        /// <summary>
        /// Get matching GrantTypes for the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<GrantTypeView> GetMatching(string criteria);

        /// <summary>
        /// Gets the GrantType for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        GrantTypeView Get(int id);
        /// <summary>
        /// Gets all GrantTypes async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<GrantTypeView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(GrantTypeModel GrantType);

        /// <summary>
        /// Updates a GrantType
        /// </summary>
        /// <param name="GrantType"></param>
        /// <returns></returns>
        ActionResponse Update(int id, GrantTypeModel GrantType);

        /// <summary>
        /// Deletes a GrantType
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id, int newId);
    }

    public class GrantTypeService
    {
        AIMSDbContext context;
        IMapper mapper;

        public GrantTypeService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<GrantTypeView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var GrantTypes = unitWork.GrantTypeRepository.GetAll();
                return mapper.Map<List<GrantTypeView>>(GrantTypes);
            }
        }

        public async Task<IEnumerable<GrantTypeView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var GrantTypes = await unitWork.GrantTypeRepository.GetAllAsync();
                return await Task<IEnumerable<GrantTypeView>>.Run(() => mapper.Map<List<GrantTypeView>>(GrantTypes)).ConfigureAwait(false);
            }
        }

        public GrantTypeView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var GrantType = unitWork.GrantTypeRepository.GetByID(id);
                return mapper.Map<GrantTypeView>(GrantType);
            }
        }

        public IEnumerable<GrantTypeView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<GrantTypeView> GrantTypesList = new List<GrantTypeView>();
                var GrantTypes = unitWork.GrantTypeRepository.GetMany(o => o.GrantType.Contains(criteria));
                return mapper.Map<List<GrantTypeView>>(GrantTypes);
            }
        }

        public ActionResponse Add(GrantTypeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isGrantTypeCreated = unitWork.GrantTypeRepository.GetOne(l => l.GrantType.ToLower() == model.GrantType.ToLower());
                    if (isGrantTypeCreated != null)
                    {
                        response.ReturnedId = isGrantTypeCreated.Id;
                    }
                    else
                    {
                        var newGrantType = unitWork.GrantTypeRepository.Insert(new EFGrantTypes()
                        {
                            GrantType = model.GrantType,
                        });
                        unitWork.Save();
                        response.ReturnedId = newGrantType.Id;
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

        public ActionResponse Update(int id, GrantTypeModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var GrantTypeObj = unitWork.GrantTypeRepository.GetByID(id);
                if (GrantTypeObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("GrantType");
                    return response;
                }

                GrantTypeObj.GrantType = model.GrantType;
                unitWork.GrantTypeRepository.Update(GrantTypeObj);
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
                var GrantTypes = await unitWork.GrantTypeRepository.GetManyQueryableAsync(l => (l.Id == id || l.Id == newId));
                if (GrantTypes.Count() < 2 && newId != 0)
                {
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("GrantType");
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                ///TODO: Implement all the dependent child update/delete logic
                await unitWork.SaveAsync();

                if (newId == 0)
                {
                    var GrantTypeToDelete = (from l in GrantTypes
                                            where l.Id == id
                                            select l).FirstOrDefault();
                    unitWork.GrantTypeRepository.Delete(GrantTypeToDelete);
                }
                else
                {
                    //Save all the new dependent children
                }

                await unitWork.SaveAsync();
                response.ReturnedId = newId;
                response.Message = mHelper.DeleteMessage("GrantType");
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }
    }
}
