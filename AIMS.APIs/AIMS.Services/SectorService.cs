using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace AIMS.Services
{
    public interface ISectorService
    {
        /// <summary>
        /// Gets all sectors
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorView> GetAll();
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
                var sectors = unitWork.SectorRepository.GetAll();
                return mapper.Map<List<SectorView>>(sectors);
            }
        }
    }
}
