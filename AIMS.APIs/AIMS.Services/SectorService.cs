using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        IEnumerable<SectorDetailedView> GetAll();

        /// <summary>
        /// Gets all sectorCategories async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SectorDetailedView>> GetAllAsync();

        /// <summary>
        /// Gets sectors list for the type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SectorView> GetSectorsForType(int id);

        /// <summary>
        /// Gets list of default sector types
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorView> GetDefaultSectors();

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
        IEnumerable<SectorView> GetChildren(int id);

        /// <summary>
        /// Gets the matching categories for the provided criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<SectorDetailedView> GetMatching(string criteria);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(SectorModel sector);

        /// <summary>
        /// Sets the provided sector as child
        /// </summary>
        /// <param name="sectorId"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        ActionResponse SetChildSector(int sectorId, int childId);

        /// <summary>
        /// Removes the provided sector as child
        /// </summary>
        /// <param name="sectorId"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        ActionResponse RemoveChildSector(int sectorId, int childId);

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

        public IEnumerable<SectorDetailedView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectors = unitWork.SectorRepository.GetWithInclude(c => c.Id != 0, new string[] { "ParentSector", "SectorType" });
                return mapper.Map<List<SectorDetailedView>>(sectors);
            }
        }

        public IEnumerable<SectorView> GetDefaultSectors()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectors = unitWork.SectorRepository.GetWithInclude(s => s.SectorType.IsDefault == true, new string[] { "SectorType" });
                return mapper.Map<List<SectorView>>(sectors);
            }
        }

        public IEnumerable<SectorView> GetSectorsForType(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectors = unitWork.SectorRepository.GetManyQueryable(s => s.SectorTypeId == id);
                return mapper.Map<List<SectorView>>(sectors);
            }
        }

        public IEnumerable<SectorView> GetChildren(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectors = unitWork.SectorRepository.GetWithInclude(s => s.ParentSectorId == id);
                return mapper.Map<List<SectorView>>(sectors);
            }
        }

        public async Task<IEnumerable<SectorDetailedView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorCategories = await unitWork.SectorRepository.GetWithIncludeAsync(c => c.Id != 0, new string[] { "ParentSector", "SectorType"});
                return await Task<IEnumerable<SectorDetailedView>>.Run(() => mapper.Map<List<SectorDetailedView>>(sectorCategories)).ConfigureAwait(false);
            }
        }

        public SectorViewModel Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorObj = unitWork.SectorRepository.GetWithInclude(c => c.Id == id, new string[] { "ParentSector", "SectorType" });
                EFSector sector = null;
                foreach (var sec in sectorObj)
                {
                    sector = sec;
                }
                return mapper.Map<SectorViewModel>(sector);
            }
        }

        public IEnumerable<SectorDetailedView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<SectorDetailedView> sectorTypesList = new List<SectorDetailedView>();
                var sectorTypes = unitWork.SectorRepository.GetWithInclude(c => c.SectorName.Contains(criteria), new string[] { "ParentSector", "SectorType" });
                return mapper.Map<List<SectorDetailedView>>(sectorTypes);
            }
        }

        public ActionResponse Add(SectorModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                try
                {
                    var sectorType = unitWork.SectorTypesRepository.GetByID(model.SectorTypeId);
                    if (sectorType == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Sector Type");
                        response.Success = false;
                        return response;
                    }

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
                                SectorType = sectorType,
                                ParentSector = parentSector,
                                SectorName = model.SectorName,
                                TimeStamp = DateTime.Now
                            });
                        }
                        else
                        {
                            newSector = unitWork.SectorRepository.Insert(new EFSector()
                            {
                                SectorType = sectorType,
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

                var sectorType = unitWork.SectorTypesRepository.GetByID(model.SectorTypeId);
                if (sectorType == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector Type");
                    return response;
                }

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
                    sectorObj.SectorType = sectorType;
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
                EFSector sector = null;

                var sectors = await unitWork.SectorRepository.GetManyQueryableAsync(s => (s.Id == id || s.Id == newId));
                if (sectors.Count() < 2 && newId != 0)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector");
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var projectSectors = await unitWork.ProjectSectorsRepository.GetManyQueryableAsync(s => (s.SectorId == id || s.SectorId == newId));
                var sectorsInDb = (from s in projectSectors
                                   select new SectorsKeyView()
                                   {
                                       SectorId = s.SectorId,
                                       ProjectId = s.ProjectId
                                   });
                projectSectors = (from s in projectSectors
                                  where s.SectorId == id
                                  select s);

                sector = (from s in sectors
                          where s.Id == id
                          select s).FirstOrDefault();
                try
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            List<EFProjectSectors> sectorsList = new List<EFProjectSectors>();
                            foreach(var projectSector in projectSectors)
                            {
                                var isExists = (from s in sectorsList
                                                        where s.SectorId == newId && s.ProjectId == projectSector.ProjectId
                                                        select s).FirstOrDefault();
                                var isExistsInDb = (from s in sectorsInDb
                                                    where s.SectorId == newId && s.ProjectId == projectSector.ProjectId
                                                    select s).FirstOrDefault();

                                if (isExists == null && isExistsInDb == null)
                                {
                                    sectorsList.Add(new EFProjectSectors()
                                    {
                                        SectorId = newId,
                                        ProjectId = projectSector.ProjectId,
                                        FundsPercentage = projectSector.FundsPercentage
                                    });
                                }
                                unitWork.ProjectSectorsRepository.Delete(projectSector);
                            }
                            await unitWork.SaveAsync();

                            if (newId == 0)
                            {
                                unitWork.SectorRepository.Delete(sector);
                            }
                            else
                            {
                                unitWork.ProjectSectorsRepository.InsertMultiple(sectorsList);
                            }
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
            return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
        }

        public ActionResponse SetChildSector(int sectorId, int childId)
        {
            ActionResponse response = new ActionResponse();
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                var sectors = unitWork.SectorRepository.GetManyQueryable(s => (s.Id == sectorId || s.Id == childId));
                if (sectors == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector");
                    return response;
                }

                var childSector = (from sector in sectors
                                  where sector.Id.Equals(childId)
                                  select sector).FirstOrDefault();

                if (childSector == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Child Sector");
                    return response;
                }

                var parentSector = (from sector in sectors
                                  where sector.Id.Equals(sectorId)
                                  select sector).FirstOrDefault();

                if (parentSector == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Parent Sector");
                    return response;
                }

                childSector.ParentSector = parentSector;
                unitWork.SectorRepository.Update(childSector);
                response.ReturnedId = childSector.Id;
                unitWork.Save();
            }
            return response;
        }

        public ActionResponse RemoveChildSector(int sectorId, int childId)
        {
            ActionResponse response = new ActionResponse();
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                var sectors = unitWork.SectorRepository.GetManyQueryable(s => (s.Id == sectorId || s.Id == childId));
                if (sectors == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector");
                    return response;
                }

                var childSector = (from sector in sectors
                                   where sector.Id.Equals(childId)
                                   select sector).FirstOrDefault();

                if (childSector == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Child Sector");
                    return response;
                }

                var parentSector = (from sector in sectors
                                    where sector.Id.Equals(sectorId)
                                    select sector).FirstOrDefault();

                if (parentSector == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Parent Sector");
                    return response;
                }

                childSector.ParentSector = null;
                unitWork.SectorRepository.Update(childSector);
                unitWork.Save();
            }
            return response;
        }
    }
}
