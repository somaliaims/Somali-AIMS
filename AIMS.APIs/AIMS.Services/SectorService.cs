using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        /// Gets child sectors of a sector
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SectorView> GetChildSectors(int id);

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

        /// <summary>
        /// Deletes a sector
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newId"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id, int newId);
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
                var sectors = unitWork.SectorRepository.GetWithInclude(c => c.Id != 0, new string[] { "ParentSector" });
                return mapper.Map<List<SectorView>>(sectors);
            }
        }

        public IEnumerable<SectorView> GetChildSectors(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectors = unitWork.SectorRepository.GetWithInclude(s => s.ParentSectorId == id);
                return mapper.Map<List<SectorView>>(sectors);
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
                    var isSectorCreated = unitWork.SectorRepository.GetOne(s => s.SectorName.ToLower() == model.SectorName.ToLower());
                    if (isSectorCreated != null)
                    {
                        response.ReturnedId = isSectorCreated.Id;
                    }
                    else
                    {
                        var parentSector = unitWork.SectorRepository.GetByID(model.ParentId);
                        EFSector newSector = null;

                        if (parentSector != null)
                        {
                            newSector = unitWork.SectorRepository.Insert(new EFSector()
                            {
                                ParentSector = parentSector,
                                SectorName = model.SectorName,
                                TimeStamp = DateTime.Now
                            });
                        }
                        else
                        {
                            newSector = unitWork.SectorRepository.Insert(new EFSector()
                            {
                                SectorName = model.SectorName,
                                TimeStamp = DateTime.Now
                            });
                        }
                        unitWork.Save();
                        response.ReturnedId = newSector.Id;
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

        public async Task<ActionResponse> DeleteAsync(int id, int newId)
        {
            ActionResponse response = new ActionResponse();
            IMessageHelper mHelper;
            using (var unitWork = new UnitOfWork(context))
            {
                EFSector newSector = null;
                var sector = await unitWork.SectorRepository.GetByIDAsync(id);
                if (sector == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector");
                    return response;
                }

                if (newId > 0)
                {
                    newSector = await unitWork.SectorRepository.GetByIDAsync(newId);
                    if (newSector == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Sector");
                        return response;
                    }
                }

                try
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            var projectSectors = await unitWork.ProjectSectorsRepository.GetManyQueryableAsync(s => s.SectorId == id);
                            if (projectSectors != null)
                            {
                                foreach (var projectSector in projectSectors)
                                {
                                    projectSector.Sector = newSector;
                                }
                            }
                            await unitWork.SaveAsync();

                            unitWork.SectorRepository.Delete(sector);
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
            }
            return response;
        }
    }
}
