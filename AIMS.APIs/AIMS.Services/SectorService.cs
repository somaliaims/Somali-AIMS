using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface ISectorService
    {
        /// <summary>
        /// Gets all sectors
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorView> GetAll();

        /// <summary>
        /// Gets all sectors async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SectorView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(NewSector sector);

        /// <summary>
        /// Updates a sector
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        ActionResponse Update(UpdateSector sector);
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

        public async Task<IEnumerable<SectorView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectors = await unitWork.SectorRepository.GetAllAsync();
                return await Task<IEnumerable<SectorView>>.Run(() => mapper.Map<List<SectorView>>(sectors)).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(NewSector sector)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var newSector = unitWork.SectorRepository.Insert(new EFSector() { SectorName = sector.Name });
                    response.ReturnedId = newSector.Id;
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

        public ActionResponse Update(UpdateSector sector)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var sectorObj = unitWork.SectorRepository.GetByID(sector.Id);
                if (sectorObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Sector");
                    return response;
                }


               /* var linkedProjects = unitWork.ProjectRepository.GetWithInclude(p => p.SectorId == sector.Id && p.DateEnded == null, new string[] { "Sector" });
                EFSector newSector = null;
                if (linkedProjects != null)
                {
                    newSector = unitWork.sectorRepository.Insert(new EFSector()
                    {
                        SectorName = sector.Name,
                    });
                }
                
                foreach (var project in linkedProjects)
                {
                }*/

                unitWork.Save();
                return response;
            }
        }
    }
}
