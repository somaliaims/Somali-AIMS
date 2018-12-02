using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface ISectorCategoryService
    {
        /// <summary>
        /// Gets all sectorCategories
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorCategoryView> GetAll();

        /// <summary>
        /// Gets all sectorCategories async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SectorCategoryView>> GetAllAsync();

        /// <summary>
        /// Gets sector category view for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SectorCategoryViewModel Get(int id);

        /// <summary>
        /// Gets the matching categories for the provided criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<SectorCategoryView> GetMatching(string criteria);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(SectorCategoryModel sectorCategory);

        /// <summary>
        /// Updates a sectorCategory
        /// </summary>
        /// <param name="sectorCategory"></param>
        /// <returns></returns>
        ActionResponse Update(int id, SectorCategoryModel sectorCategory);
    }

    public class SectorCategoryService : ISectorCategoryService
    {
        AIMSDbContext context;
        IMapper mapper;

        public SectorCategoryService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<SectorCategoryView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorCategories = unitWork.SectorCategoryRepository.GetWithInclude(c => c.Id != 0, new string[] { "SectorType" });
                return mapper.Map<List<SectorCategoryView>>(sectorCategories);
            }
        }

        public async Task<IEnumerable<SectorCategoryView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorCategories = await unitWork.SectorCategoryRepository.GetAllAsync();
                return await Task<IEnumerable<SectorCategoryView>>.Run(() => mapper.Map<List<SectorCategoryView>>(sectorCategories)).ConfigureAwait(false);
            }
        }

        public SectorCategoryViewModel Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorCategoryObj = unitWork.SectorCategoryRepository.GetWithInclude(c => c.Id == id, new string[] { "SectorType" });
                EFSectorCategory sectorCategory = null;
                foreach(var category in sectorCategoryObj)
                {
                    sectorCategory = category;
                }
                return mapper.Map<SectorCategoryViewModel>(sectorCategory);
            }
        }

        public IEnumerable<SectorCategoryView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<SectorCategoryView> sectorTypesList = new List<SectorCategoryView>();
                var sectorTypes = unitWork.SectorCategoryRepository.GetWithInclude(c => c.Category.Contains(criteria), new string[] { "SectorType" });
                return mapper.Map<List<SectorCategoryView>>(sectorTypes);
            }
        }

        public ActionResponse Add(SectorCategoryModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper msgHelper;
                try
                {
                    var sectorType = unitWork.SectorTypesRepository.GetByID(model.SectorTypeId);
                    if (sectorType == null)
                    {
                        msgHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = msgHelper.GetNotFound("Sector Type");
                        return response;
                    }

                    var newSectorCategory = unitWork.SectorCategoryRepository.Insert(new EFSectorCategory()
                    {
                        SectorType = sectorType,
                        Category = model.Category
                    });
                    response.ReturnedId = newSectorCategory.Id;
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, SectorCategoryModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var sectorCategoryObj = unitWork.SectorCategoryRepository.GetByID(id);
                if (sectorCategoryObj == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector Category");
                    return response;
                }
                var sectoryType = unitWork.SectorTypesRepository.GetByID(model.SectorTypeId);
                if (sectoryType == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector Type");
                    return response;
                }

                sectorCategoryObj.Category = model.Category;
                sectorCategoryObj.SectorType = sectoryType;
                unitWork.SectorCategoryRepository.Update(sectorCategoryObj);
                unitWork.Save();
                return response;
            }
        }
    }
}
