﻿using AIMS.DAL.EF;
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
        /// Adds new sector mappings for the provided sector
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> AddAsync(SectorMappingsModel model);
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
                SectorMappingsView mappingsView = new SectorMappingsView();
                var mappings = unitWork.SectorMappingsRepository.GetWithInclude(m => m.SectorId == id, new string[] { "Sector", "MappedSector" });
                string sectorName = "";
                int sectorTypeId = 0;
                MappingSectors mappedSectors = null;
                List<MappingSectors> mappingSectorsList = new List<MappingSectors>();
                List<SectorSimpleView> sectorsList = new List<SectorSimpleView>();
                mappings = (from mapping in mappings
                            orderby mapping.SectorTypeId ascending
                            select mapping);

                foreach (var mapping in mappings)
                {
                    if (sectorTypeId != mapping.SectorTypeId)
                    {
                        if (mappedSectors != null)
                        {
                            mappedSectors.Sectors = sectorsList;
                            mappingSectorsList.Add(mappedSectors);
                        }
                        mappedSectors = new MappingSectors()
                        {
                            SectorTypeId = mapping.SectorTypeId,
                            SectorType = mapping.SectorType.SectorType.TypeName
                        };
                        sectorsList = new List<SectorSimpleView>();
                    }
                    sectorsList.Add(new SectorSimpleView()
                    {
                        SectorId = mapping.Sector.Id,
                        Sector = mapping.Sector.SectorName
                    });
                }
                mappingsView.Sector = sectorName;
                return mappingsView;
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
                            foreach (var mapping in sectorMappings)
                            {
                                unitWork.SectorMappingsRepository.Delete(mapping);
                            }
                            await unitWork.SaveAsync();

                            List<EFSectorMappings> mappingsList = new List<EFSectorMappings>();
                            foreach (var id in model.MappingIds)
                            {
                                mappingsList.Add(new EFSectorMappings()
                                {
                                    SectorId = sector.Id,
                                    SectorTypeId = sectorType.Id,
                                    MappedSectorId = id
                                });
                            }
                            await unitWork.SaveAsync();
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
    }
}
