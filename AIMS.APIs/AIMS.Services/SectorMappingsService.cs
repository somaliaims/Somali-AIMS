using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using AIMS.Services.Helpers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AIMS.Services
{
    public interface ISectorMappingsService
    {
        /// <summary>
        /// Gets list of mappings for provided sector id
        /// </summary>
        /// <returns></returns>
        SectorMappingsView GetForSector(int id);

        /// <summary>
        /// Gets sector mappings
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SectorView> GetSectorMappings(int id);

        /// <summary>
        /// Gets sector mappings
        /// </summary>
        /// <param name="sectorName"></param>
        /// <returns></returns>
        IEnumerable<SectorView> GetSectorMappings(string sectorName);

        /// <summary>
        /// Adds new sector mappings for the provided sector
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddAsync(SectorMappingsModel model);

        /// <summary>
        /// Deletes a mapping
        /// </summary>
        /// <param name="sectorId"></param>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        ActionResponse Delete(int sectorId, int mappingId);
    }

    public class SectorMappingsService : ISectorMappingsService
    {
        AIMSDbContext context;
        IMapper mapper;

        public SectorMappingsService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public SectorMappingsView GetForSector(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                string sectorName = "";
                int sectorTypeId = 0;
                SectorMappingsView mappingsView = new SectorMappingsView();
                var sectors = unitWork.SectorRepository.GetManyQueryable(s => s.Id != 0);
                var sectorTypes = unitWork.SectorTypesRepository.GetAll();
                var mappings = unitWork.SectorMappingsRepository.GetManyQueryable(m => m.MappedSectorId == id);
                MappingSectors mappedSectors = null;
                List<MappingSectors> mappingSectorsList = new List<MappingSectors>();
                List<SectorSimpleView> sectorsList = new List<SectorSimpleView>();

                if (mappings.Count() > 0)
                {
                    mappings = (from mapping in mappings
                                orderby mapping.SectorTypeId ascending
                                select mapping);

                    EFSectorMappings lastMapping = mappings.Last();
                    foreach (var mapping in mappings)
                    {
                        var sectorType = (from sType in sectorTypes
                                          where sType.Id == mapping.SectorTypeId
                                          select sType).FirstOrDefault();
                        if (sectorTypeId != mapping.SectorTypeId)
                        {
                            sectorTypeId = mapping.SectorTypeId;
                            if (mappedSectors != null)
                            {
                                mappedSectors.Sectors = sectorsList;
                                mappingSectorsList.Add(mappedSectors);
                            }
                            mappedSectors = new MappingSectors()
                            {
                                SectorTypeId = mapping.SectorTypeId,
                                SectorType = sectorType.TypeName
                            };
                            sectorsList = new List<SectorSimpleView>();
                        }

                        sectorName = (from s in sectors
                                      where s.Id == mapping.SectorId
                                      select s).FirstOrDefault().SectorName;
                        sectorsList.Add(new SectorSimpleView()
                        {
                            SectorId = mapping.SectorId,
                            Sector = sectorName
                        });

                        if (mapping == lastMapping && mappedSectors != null)
                        {
                            mappedSectors.Sectors = sectorsList;
                            mappingSectorsList.Add(mappedSectors);
                        }
                    }
                    mappingsView.Sector = sectorName;
                    mappingsView.MappedSectors = mappingSectorsList;
                }
                
                return mappingsView;
            }
        }

        public IEnumerable<SectorView> GetSectorMappings(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<SectorView> mappingsList = new List<SectorView>();
                var sectorType = unitWork.SectorTypesRepository.GetOne(s => s.IsPrimary == true);
                IEnumerable<EFSector> sectorsList = new List<EFSector>();

                if (sectorType != null)
                {
                    var mappings = unitWork.SectorMappingsRepository.GetManyQueryable(m => (m.SectorId == id && m.SectorTypeId == sectorType.Id));
                    List<int> mappingIds = new List<int>();
                    if (mappings != null)
                    {
                        mappingIds = (from s in mappings
                                      select s.MappedSectorId).ToList<int>();

                        sectorsList = unitWork.SectorRepository.GetManyQueryable(s => mappingIds.Contains(s.Id));
                    }
                }
                return mapper.Map<List<SectorView>>(sectorsList);
            }
        }

        public IEnumerable<SectorView> GetSectorMappings(string sectorName)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<SectorView> mappingsList = new List<SectorView>();
                var sectorType = unitWork.SectorTypesRepository.GetOne(s => s.IsPrimary == true);
                IEnumerable<EFSector> sectorsList = new List<EFSector>();

                if (sectorType != null)
                {
                    var sector = unitWork.SectorRepository.GetOne(s => s.SectorName.Trim().ToLower() == sectorName.Trim().ToLower());
                    if (sector != null)
                    {
                        var mappings = unitWork.SectorMappingsRepository.GetManyQueryable(m => (m.SectorId == sector.Id && m.SectorTypeId == sector.SectorTypeId));
                        List<int> mappingIds = new List<int>();
                        if (mappings != null)
                        {
                            mappingIds = (from s in mappings
                                          select s.MappedSectorId).ToList<int>();

                            sectorsList = unitWork.SectorRepository.GetManyQueryable(s => mappingIds.Contains(s.Id));
                        }
                    }
                }
                return mapper.Map<List<SectorView>>(sectorsList);
            }
        }

        public async Task<ActionResponse> AddAsync(SectorMappingsModel model)
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
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Sector Type");
                        return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                    }

                    if (sectorType.IsPrimary == true)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.InvalidSectorMapping(sectorType.TypeName);
                        return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                    }

                    var sectors = unitWork.SectorRepository.GetManyQueryable(s => (model.MappingIds.Contains(s.Id) || s.Id == model.SectorId));
                    int sectorCount = sectors.Count() - 1;
                    if (sectorCount < model.MappingIds.Count)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Sector/s");
                        return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                    }
                    var sector = (from s in sectors
                                  where s.Id == model.SectorId
                                  select s).FirstOrDefault();

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            var sectorMappings = await unitWork.SectorMappingsRepository.GetManyQueryableAsync(m => m.SectorId == model.SectorId);
                            List<MappingsKeyView> mappingsView = (from m in sectorMappings
                                                                 select new MappingsKeyView
                                                                 {
                                                                     SectorId = m.SectorId,
                                                                     MappingId = m.MappedSectorId
                                                                 }).ToList<MappingsKeyView>();

                            List<EFSectorMappings> mappingsList = new List<EFSectorMappings>();
                            MappingsKeyView mappingView = null;
                            foreach (var id in model.MappingIds)
                            {
                                mappingView = (from m in mappingsView
                                            where m.SectorId == sector.Id && id == m.MappingId
                                            select m).FirstOrDefault();

                                if (mappingView == null)
                                {
                                    mappingsList.Add(new EFSectorMappings()
                                    {
                                        SectorId = id,
                                        SectorTypeId = sectorType.Id,
                                        MappedSectorId = sector.Id
                                    });
                                }
                            }

                            if (mappingsList.Count > 0)
                            {
                                unitWork.SectorMappingsRepository.InsertMultiple(mappingsList);
                                await unitWork.SaveAsync();
                                transaction.Commit();
                            }
                        }
                    });
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public ActionResponse Delete(int sectorId, int mappingId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var mapping = unitWork.SectorMappingsRepository.GetOne(m => (m.SectorId == sectorId && m.MappedSectorId == mappingId));
                if (mapping == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector Mapping");
                    return response;
                }

                unitWork.SectorMappingsRepository.Delete(mapping);
                unitWork.Save();
                return response;
            }
        }
    }
}
