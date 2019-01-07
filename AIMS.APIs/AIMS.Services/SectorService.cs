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
    public interface ISectorService
    {
        /// <summary>
        /// Gets all sectorCategories
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorView> GetAll();

        /// <summary>
        /// Gets all sectorCategories async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SectorView>> GetAllAsync();

        /// <summary>
        /// Gets sector category view for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SectorViewModel Get(int id);

        /// <summary>
        /// Gets the matching categories for the provided criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<SectorView> GetMatching(string criteria);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(SectorModel sector);

        /// <summary>
        /// Updates a sector
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        ActionResponse Update(int id, SectorModel sector);
    }

    public class SectorService : ISectorService
    {
        AIMSDbContext context;
        IMapper mapper;

        public SectorService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<SectorView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorCategories = unitWork.SectorRepository.GetWithInclude(c => c.Id != 0, new string[] { "ParentSector" });
                return mapper.Map<List<SectorView>>(sectorCategories);
            }
        }

        public async Task<IEnumerable<SectorView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorCategories = await unitWork.SectorRepository.GetWithIncludeAsync(c => c.Id != 0, new string[] { "ParentSector"});
                return await Task<IEnumerable<SectorView>>.Run(() => mapper.Map<List<SectorView>>(sectorCategories)).ConfigureAwait(false);
            }
        }

        public SectorViewModel Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorObj = unitWork.SectorRepository.GetWithInclude(c => c.Id == id, new string[] { "ParentSector" });
                EFSector sector = null;
                foreach (var category in sectorObj)
                {
                    sector = category;
                }
                return mapper.Map<SectorViewModel>(sector);
            }
        }

        public IEnumerable<SectorView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<SectorView> sectorTypesList = new List<SectorView>();
                var sectorTypes = unitWork.SectorRepository.GetWithInclude(c => c.SectorName.Contains(criteria), new string[] { "ParentSector" });
                return mapper.Map<List<SectorView>>(sectorTypes);
            }
        }

        public ActionResponse Add(SectorModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var parentSector = unitWork.SectorRepository.GetByID(model.ParentId);
                    var newSector = unitWork.SectorRepository.Insert(new EFSector()
                    {
                        ParentSector = parentSector,
                        SectorName = model.SectorName,
                        TimeStamp = DateTime.Now
                    });
                    response.ReturnedId = newSector.Id;
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

        public ActionResponse Update(int id, SectorModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;

                var parentSector = unitWork.SectorRepository.GetByID(model.ParentId);
                var sectorObj = unitWork.SectorRepository.GetByID(id);
                if (sectorObj == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector");
                    return response;
                }

                try
                {
                    sectorObj.ParentSector = parentSector;
                    sectorObj.SectorName = model.SectorName;
                    unitWork.SectorRepository.Update(sectorObj);
                    unitWork.Save();
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
