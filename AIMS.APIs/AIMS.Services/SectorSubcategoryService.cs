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
    public interface ISectorSubcategoryService
    {
        /// <summary>
        /// Gets all sectorCategories
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorSubCategoryView> GetAll();

        /// <summary>
        /// Gets all sectorCategories async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SectorSubCategoryView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(SectorSubCategoryModel sectorCategory);

        /// <summary>
        /// Updates a sectorCategory
        /// </summary>
        /// <param name="sectorCategory"></param>
        /// <returns></returns>
        ActionResponse Update(int id, SectorSubCategoryModel sectorCategory);
    }

    public class SectorSubcategoryService
    {
        AIMSDbContext context;
        IMapper mapper;

        public SectorSubcategoryService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<SectorSubCategoryView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorCategories = unitWork.SectorSubCategoryRepository.GetAll();
                return mapper.Map<List<SectorSubCategoryView>>(sectorCategories);
            }
        }

        public async Task<IEnumerable<SectorSubCategoryView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorCategories = await unitWork.SectorSubCategoryRepository.GetAllAsync();
                return await Task<IEnumerable<SectorSubCategoryView>>.Run(() => mapper.Map<List<SectorSubCategoryView>>(sectorCategories)).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(SectorSubCategoryModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper msgHelper;
                try
                {
                    var category = unitWork.SectorCategoryRepository.GetByID(model.CategoryId);
                    if (category == null)
                    {
                        msgHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = msgHelper.GetNotFound("Category");
                    }
                    var newSectorSubCategory = unitWork.SectorSubCategoryRepository.Insert(new EFSectorSubCategory()
                    {
                        SectorCategory = category,
                        SubCategory = model.SubCategory
                    });
                    response.ReturnedId = newSectorSubCategory.Id;
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

        public ActionResponse Update(int id, SectorSubCategoryModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var sectorSubCategoryObj = unitWork.SectorSubCategoryRepository.GetByID(id);
                if (sectorSubCategoryObj == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector SubCategory");
                    return response;
                }
                var sectorCategory = unitWork.SectorCategoryRepository.GetByID(model.CategoryId);
                if (sectorCategory == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector Category");
                    return response;
                }

                sectorSubCategoryObj.SectorCategory = sectorCategory;
                sectorSubCategoryObj.SubCategory = model.SubCategory;
                unitWork.SectorSubCategoryRepository.Update(sectorSubCategoryObj);
                unitWork.Save();
                return response;
            }
        }
    }
}
