using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface ISectorTypesService
    {
        /// <summary>
        /// Gets all sectorTypes
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorTypesView> GetAll();

        /// <summary>
        /// Gets sector type for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SectorTypesView Get(int id);

        /// <summary>
        /// Gets sector type for IATI
        /// </summary>
        /// <returns></returns>
        SectorTypesView GetSectorTypeForIATI();

        /// <summary>
        /// Sets a sector as default
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResponse> SetAsDefaultAsync(int id);

        /// <summary>
        /// Sets sector type as iati sector default type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResponse> SetAsIATIAsync(int id);

        /// <summary>
        /// Gets default sector type
        /// </summary>
        /// <returns></returns>
        SectorTypesView GetDefault();

        /// <summary>
        /// Gets sectors other than default
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorTypesView> GetOtherSectors();

        /// <summary>
        /// Gets matching sector types for the provided criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<SectorTypesView> GetMatching(string criteria);

        /// <summary>
        /// Gets all sectorTypes async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SectorTypesView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(SectorTypesModel sectorType);

        /// <summary>
        /// Updates a sectorType
        /// </summary>
        /// <param name="sectorType"></param>
        /// <returns></returns>
        ActionResponse Update(int id, SectorTypesModel sectorType);

        /// <summary>
        /// Deletes the sector type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }

    public class SectorTypesService : ISectorTypesService
    {
        AIMSDbContext context;
        IMapper mapper;

        public SectorTypesService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<SectorTypesView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorTypes = unitWork.SectorTypesRepository.GetAll();
                return mapper.Map<List<SectorTypesView>>(sectorTypes);
            }
        }

        public IEnumerable<SectorTypesView> GetOtherSectors()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorTypes = unitWork.SectorTypesRepository.GetManyQueryable(s => s.IsPrimary != true);
                return mapper.Map<List<SectorTypesView>>(sectorTypes);
            }
        }

        public SectorTypesView GetSectorTypeForIATI()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                SectorTypesView view = new SectorTypesView();
                var sectorType = unitWork.SectorTypesRepository.GetOne(s => s.IsSourceType == true);
                if (sectorType != null)
                {
                    view.Id = sectorType.Id;
                    view.TypeName = sectorType.TypeName;
                }
                return view;
            }
        }

        public SectorTypesView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorType = unitWork.SectorTypesRepository.GetByID(id);
                return mapper.Map<SectorTypesView>(sectorType);
            }
        }

        public SectorTypesView GetDefault()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorType = unitWork.SectorTypesRepository.GetOne(s => (s.IsPrimary == true));
                return mapper.Map<SectorTypesView>(sectorType);
            }
        }

        public async Task<IEnumerable<SectorTypesView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorTypes = await unitWork.SectorTypesRepository.GetAllAsync();
                return await Task<IEnumerable<SectorTypesView>>.Run(() => mapper.Map<List<SectorTypesView>>(sectorTypes)).ConfigureAwait(false);
            }
        }

        public IEnumerable<SectorTypesView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<SectorTypesView> sectorTypesList = new List<SectorTypesView>();
                var sectorTypes = unitWork.SectorTypesRepository.GetMany(o => o.TypeName.Contains(criteria));
                return mapper.Map<List<SectorTypesView>>(sectorTypes);
            }
        }

        public ActionResponse Add(SectorTypesModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var newSectorTypes = unitWork.SectorTypesRepository.Insert(new EFSectorTypes()
                    {
                        TypeName = model.TypeName,
                        IsPrimary = model.IsPrimary,
                        IsSourceType = model.IsSourceType
                    });
                    response.ReturnedId = newSectorTypes.Id;
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

        public async Task<ActionResponse> SetAsDefaultAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var sectorTypes = unitWork.SectorTypesRepository.GetAll();
                var sectorType = (from s in sectorTypes
                                  where s.Id == id
                                  select s).FirstOrDefault();

                if (sectorType == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector type");
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        foreach(var sType in sectorTypes)
                        {
                            sType.IsPrimary = false;
                            unitWork.SectorTypesRepository.Update(sType);
                        }
                        sectorType.IsPrimary = true;
                        await unitWork.SaveAsync();
                    }
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public async Task<ActionResponse> SetAsIATIAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var sectorTypes = unitWork.SectorTypesRepository.GetAll();
                var sectorType = (from s in sectorTypes
                                  where s.Id == id
                                  select s).FirstOrDefault();

                if (sectorType == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector type");
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        foreach (var sType in sectorTypes)
                        {
                            sType.IsSourceType = false;
                            unitWork.SectorTypesRepository.Update(sType);
                        }
                        sectorType.IsSourceType = true;
                        await unitWork.SaveAsync();
                    }
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public ActionResponse Update(int id, SectorTypesModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var sectorTypeObj = unitWork.SectorTypesRepository.GetByID(id);
                if (sectorTypeObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("SectorTypes");
                    return response;
                }

                sectorTypeObj.TypeName = model.TypeName;
                sectorTypeObj.IsPrimary = model.IsPrimary;
                sectorTypeObj.IsSourceType = model.IsSourceType;
                unitWork.Save();
                response.Message = true.ToString();
                return response;
            }
        }

        public ActionResponse Delete(int id)
        {
            ActionResponse response = new ActionResponse();
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                var sectors = unitWork.SectorRepository.GetManyQueryable(s => s.SectorTypeId == id);
                if (sectors.Count() > 0)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetInvalidDeletionAttemptSectorType();
                    response.Success = false;
                    return response;
                }

                var sectorType = unitWork.SectorTypesRepository.GetByID(id);
                if (sectorType == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Sector type");
                    response.Success = false;
                    return response;
                }
                unitWork.SectorTypesRepository.Delete(sectorType);
                unitWork.Save();
            }
            return response;
        }
    }
}
