using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IReportNamesService
    {
        /// <summary>
        /// Gets all reportNames
        /// </summary>
        /// <returns></returns>
        IEnumerable<ReportNameView> GetAll();

        /// <summary>
        /// Get matching reportNames for the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<ReportNameView> GetMatching(string criteria);

        /// <summary>
        /// Gets the report for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        ReportNameView Get(int id);
        
        /// <summary>
        /// Gets all reportNames async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ReportNameView>> GetAllAsync();
    }

    public class ReportNamesService : IReportNamesService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ReportNamesService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ReportNameView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ReportNameView> namesList = new List<ReportNameView>();
                var reportNames = unitWork.ReportsRepository.GetAll();
                return mapper.Map<List<ReportNameView>>(reportNames);
            }
        }

        public async Task<IEnumerable<ReportNameView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var reportNames = await unitWork.ReportsRepository.GetAllAsync();
                return await Task<IEnumerable<ReportNameView>>.Run(() => mapper.Map<List<ReportNameView>>(reportNames)).ConfigureAwait(false);
            }
        }

        public ReportNameView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var report = unitWork.ReportsRepository.GetByID(id);
                return mapper.Map<ReportNameView>(report);
            }
        }

        public IEnumerable<ReportNameView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<ReportNameView> reportNamesList = new List<ReportNameView>();
                var reportNames = unitWork.ReportsRepository.GetMany(r => r.Title.Contains(criteria));
                return mapper.Map<List<ReportNameView>>(reportNames);
            }
        }
    }
}
