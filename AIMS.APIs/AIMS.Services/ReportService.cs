using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AIMS.Services.Helpers;

namespace AIMS.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Get projects report by sectors and title
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Task<ProjectProfileReportBySector> GetProjectsBySector(ReportModelForProjectSectors model);

        /// <summary>
        /// Search matching projects for the provided criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model);
    }

    public class ReportService : IReportService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ReportService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        /*public async Task<ProjectProfileReportBySector> GetProjectsBySector(ReportModelForProjectSectors model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectProfileReportBySector sectorProjectsReport = new ProjectProfileReportBySector();

                try
                {
                    sectorProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_SECTOR_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_SECTOR_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_SECTOR_FOOTER,
                        Dated = DateTime.Now.ToLongDateString()
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileListObj = null;
                    IQueryable<EFProjectSectors> projectSectors = null;

                    if (model.Year != 0)
                    {
                        projectProfileListObj = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartDate.Year == model.Year)),
                            new string[] { "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents" });
                    }
                    else
                    {
                        projectProfileListObj = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartDate.Year == year && p.StartDate.Month >= month)
                            || (p.EndDate.Year >= year && p.EndDate.Month >= month)),
                            new string[] { "Locations", "Locations.Location", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer", "Documents" });
                    }

                    if (model.SectorIds != null)
                    {
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => model.SectorIds.Contains(p.SectorId), new string[] { "Sector" });
                    }
                    else
                    {
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => p.ProjectId != 0, new string[] { "Sector" });
                    }

                    projectSectors = from pSector in projectSectors
                                     orderby pSector.Sector.SectorName
                                     select pSector;

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileListObj)
                    {
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Locations = mapper.Map<List<ProjectLocationDetailView>>(project.Locations);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        profileView.Documents = mapper.Map<List<ProjectDocumentView>>(project.Documents);
                        decimal projectCost = 0;
                        if (profileView.Funders.Count > 0)
                        {
                            projectCost = profileView.Funders.Select(f => (f.Amount)).Sum();
                            profileView.ProjectCost = projectCost;
                        }
                        if (profileView.Disbursements.Count > 0)
                        {
                            decimal totalDisbursements = profileView.Disbursements.Select(d => (d.Amount)).Sum();
                            UtilityHelper helper = new UtilityHelper();
                            var endDate = Convert.ToDateTime(profileView.EndDate);
                            var startDate = DateTime.Now;
                            int months = helper.GetMonthDifference(startDate, endDate);

                            profileView.ActualDisbursements = totalDisbursements;
                            profileView.PlannedDisbursements = Math.Round((projectCost - totalDisbursements) / months);
                        }
                        projectsList.Add(profileView);
                    }

                    string currentSector = null;
                    List<int> projectIds = new List<int>();
                    List<ProjectsBySector> sectorProjectsList = new List<ProjectsBySector>();
                    ProjectsBySector projectsBySector = null;

                    int totalSectors = projectSectors.Count();
                    int counter = 0;
                    foreach (var sector in projectSectors)
                    {
                        if (sector.Sector.SectorName != currentSector)
                        {
                            if (currentSector != null)
                            {
                                var sectorProjects = (from project in projectsList
                                                      where projectIds.Contains(project.Id)
                                                      select project).ToList<ProjectProfileView>();

                                projectsBySector.Projects = sectorProjects;
                                sectorProjectsList.Add(projectsBySector);
                                projectIds.Clear();
                            }
                            projectsBySector = new ProjectsBySector();
                            projectsBySector.SectorName = sector.Sector.SectorName;
                        }
                        currentSector = sector.Sector.SectorName;
                        projectIds.Add(sector.ProjectId);
                        ++counter;

                        if (totalSectors == counter)
                        {
                            var sectorProjects = (from project in projectsList
                                                  where projectIds.Contains(project.Id)
                                                  select project).ToList<ProjectProfileView>();

                            projectsBySector.Projects = sectorProjects;
                            sectorProjectsList.Add(projectsBySector);
                            projectIds.Clear();
                        }
                    }
                    sectorProjectsReport.SectorProjectsList = sectorProjectsList;
                }
                catch(Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportBySector>.Run(() => sectorProjectsReport).ConfigureAwait(false);
            }     
        }*/


        public async Task<ProjectProfileReportBySector> GetProjectsBySectors(SearchProjectsBySectorModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectProfileReportBySector sectorProjectsReport = new ProjectProfileReportBySector();
                try
                {
                    sectorProjectsReport.ReportSettings = new Report()
                    {
                        Title = ReportConstants.PROJECTS_BY_SECTOR_TITLE,
                        SubTitle = ReportConstants.PROJECTS_BY_SECTOR_SUBTITLE,
                        Footer = ReportConstants.PROJECTS_BY_SECTOR_FOOTER,
                        Dated = DateTime.Now.ToLongDateString()
                    };

                    DateTime dated = new DateTime();
                    int year = dated.Year;
                    int month = dated.Month;
                    IQueryable<EFProject> projectProfileList = null;
                    IQueryable<EFProjectSectors> projectSectors = null;

                    if (!string.IsNullOrEmpty(model.Title))
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => p.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase),
                            new string[] {  "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.StartingYear >= 2000 && model.EndingYear >= 2000)
                    {
                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => ((p.StartDate.Year >= model.StartingYear && p.EndDate.Year <= model.EndingYear)),
                            new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where project.StartDate.Year >= model.StartingYear
                                                 && project.EndDate.Year <= model.EndingYear
                                                 select project;
                        }
                    }

                    if (model.OrganizationIds.Count > 0)
                    {
                        var projectFunders = unitWork.ProjectFundersRepository.GetMany(f => model.OrganizationIds.Contains(f.FunderId));
                        var projectIdsFunders = (from pFunder in projectFunders
                                                 select pFunder.ProjectId).ToList<int>().Distinct();

                        var projectImplementers = unitWork.ProjectImplementersRepository.GetMany(f => model.OrganizationIds.Contains(f.ImplementerId));
                        var projectIdsImplementers = (from pImplementer in projectImplementers
                                                      select pImplementer.ProjectId).ToList<int>().Distinct();


                        var projectIdsList = projectIdsFunders.Union(projectIdsImplementers);

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }

                    /*if (model.LocationIds.Count > 0)
                    {
                        var projectLocations = unitWork.ProjectLocationsRepository.GetMany(l => model.LocationIds.Contains(l.LocationId));
                        var projectIdsList = (from pLocation in projectLocations
                                              select pLocation.ProjectId).ToList<int>().Distinct();

                        if (projectProfileList == null)
                        {
                            projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => projectIdsList.Contains(p.Id)
                            , new string[] { "Sectors", "Sectors.Sector", "Locations", "Locations.Location", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                        }
                        else
                        {
                            projectProfileList = from project in projectProfileList
                                                 where projectIdsList.Contains(project.Id)
                                                 select project;
                        }
                    }*/

                    if (projectProfileList == null)
                    {
                        projectProfileList = await unitWork.ProjectRepository.GetWithIncludeAsync(p => (p.EndDate.Year >= year && p.EndDate.Month >= month),
                            new string[] { "Sectors", "Sectors.Sector", "Disbursements", "Funders", "Funders.Funder", "Implementers", "Implementers.Implementer" });
                    }

                    if (model.SectorIds.Count > 0)
                    {
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => model.SectorIds.Contains(p.SectorId), new string[] { "Sector" });
                    }
                    else
                    {
                        projectSectors = unitWork.ProjectSectorsRepository.GetWithInclude(p => p.ProjectId != 0, new string[] { "Sector" });
                    }

                    projectSectors = from pSector in projectSectors
                                     orderby pSector.Sector.SectorName
                                     select pSector;

                    List<ProjectProfileView> projectsList = new List<ProjectProfileView>();
                    foreach (var project in projectProfileList)
                    {
                        ProjectProfileView profileView = new ProjectProfileView();
                        profileView.Id = project.Id;
                        profileView.Title = project.Title;
                        profileView.Description = project.Description;
                        profileView.StartDate = project.StartDate.ToLongDateString();
                        profileView.EndDate = project.EndDate.ToLongDateString();
                        profileView.Sectors = mapper.Map<List<ProjectSectorView>>(project.Sectors);
                        profileView.Funders = mapper.Map<List<ProjectFunderView>>(project.Funders);
                        profileView.Implementers = mapper.Map<List<ProjectImplementerView>>(project.Implementers);
                        profileView.Disbursements = mapper.Map<List<ProjectDisbursementView>>(project.Disbursements);
                        projectsList.Add(profileView);
                    }

                    string currentSector = null;
                    List<int> projectIds = new List<int>();
                    List<ProjectsBySector> sectorProjectsList = new List<ProjectsBySector>();
                    ProjectsBySector projectsBySector = null;

                    int totalSectors = projectSectors.Count();
                    int counter = 0;
                    List<ProjectViewForSector> projectsListForSector = null;
                    foreach (var sector in projectSectors)
                    {
                        if (sector.Sector.SectorName != currentSector)
                        {
                            if (currentSector != null)
                            {
                                projectsListForSector = new List<ProjectViewForSector>();
                                var sectorProjects = (from project in projectsList
                                                      where projectIds.Contains(project.Id)
                                                      select project).ToList<ProjectProfileView>();

                                decimal totalFunding = 0, totalDisbursements = 0, totalFundingPercentage = 0, totalDisbursementsPercentage = 0;
                                foreach (var project in sectorProjects)
                                {
                                    if (project.Funders.Count() > 0)
                                    {
                                        var fundingTotal = project.Funders.Select(f => (f.Amount)).Sum();
                                        project.ProjectCost = ((fundingTotal / 100) * sector.FundsPercentage);
                                        totalFunding += fundingTotal;
                                    }
                                }

                                if (totalFunding > 0)
                                {
                                    totalFundingPercentage += ((totalFunding / 100) * sector.FundsPercentage);
                                }

                                foreach (var project in sectorProjects)
                                {
                                    if (project.Disbursements.Count() > 0)
                                    {
                                        decimal projectDisbursements = project.Disbursements.Select(d=> (d.Amount)).Sum();
                                        totalDisbursements += projectDisbursements;

                                        UtilityHelper helper = new UtilityHelper();
                                        var endDate = Convert.ToDateTime(project.EndDate);
                                        var startDate = DateTime.Now;
                                        int months = helper.GetMonthDifference(startDate, endDate);

                                        project.ActualDisbursements = ((projectDisbursements / 100) * sector.FundsPercentage);
                                        if (months > 0)
                                        {
                                            project.PlannedDisbursements = Math.Round((project.ProjectCost - projectDisbursements) / months);
                                            if (project.PlannedDisbursements < 0)
                                            {
                                                project.PlannedDisbursements = 0;
                                            }
                                        }
                                    }
                                }

                                if (totalDisbursements > 0)
                                {
                                    totalDisbursementsPercentage = ((totalDisbursements / 100) * sector.FundsPercentage);
                                }

                                foreach (var project in sectorProjects)
                                {
                                    projectsListForSector.Add(new ProjectViewForSector()
                                    {
                                        Title = project.Title,
                                        StartDate = project.StartDate,
                                        EndDate = project.EndDate,
                                        Funders = string.Join(", ", project.Funders.Select(f => f.Funder)),
                                        Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                        ProjectCost = project.ProjectCost,
                                        ActualDisbursements = project.ActualDisbursements,
                                        PlannedDisbursements = project.PlannedDisbursements,
                                    });
                                }

                                projectsBySector.TotalFunding = totalFundingPercentage;
                                projectsBySector.TotalDisbursements = totalDisbursementsPercentage;
                                projectsBySector.Projects = projectsListForSector;
                                sectorProjectsList.Add(projectsBySector);
                                projectIds.Clear();
                            }
                            projectsBySector = new ProjectsBySector();
                            projectsBySector.SectorName = sector.Sector.SectorName;
                        }
                        currentSector = sector.Sector.SectorName;
                        projectIds.Add(sector.ProjectId);
                        ++counter;

                        if (totalSectors == counter)
                        {
                            projectsListForSector = new List<ProjectViewForSector>();
                            var sectorProjects = (from project in projectsList
                                                  where projectIds.Contains(project.Id)
                                                  select project).ToList<ProjectProfileView>();

                            decimal totalFunding = 0, totalFundingPercentage = 0, totalDisbursements = 0, totalDisbursementsPercentage = 0, sectorPercentage = 0;
                            foreach (var project in sectorProjects)
                            {
                                if (project.Sectors != null)
                                {
                                    sectorPercentage = (from s in project.Sectors
                                                        where s.SectorId == sector.SectorId
                                                        select s.FundsPercentage).FirstOrDefault();

                                    if (project.Funders.Count() > 0)
                                    {
                                        var fundingTotal = project.Funders.Select(f => (f.Amount)).Sum();
                                        project.ProjectCost = ((fundingTotal / 100) * sector.FundsPercentage);
                                        totalFunding += fundingTotal;
                                    }
                                }
                            }

                            if (totalFunding > 0)
                            {
                                totalFundingPercentage = ((totalFunding / 100) * sector.FundsPercentage);
                            }

                            foreach (var project in sectorProjects)
                            {
                                if (project.Sectors != null)
                                {
                                    sectorPercentage = (from s in project.Sectors
                                                        where s.SectorId == sector.SectorId
                                                        select s.FundsPercentage).FirstOrDefault();
                                    if (project.Disbursements.Count() > 0)
                                    {
                                        decimal projectDisbursements = project.Disbursements.Select(d => (d.Amount)).Sum();
                                        totalDisbursements += projectDisbursements;

                                        UtilityHelper helper = new UtilityHelper();
                                        var endDate = Convert.ToDateTime(project.EndDate);
                                        var startDate = DateTime.Now;
                                        int months = helper.GetMonthDifference(startDate, endDate);

                                        project.ActualDisbursements = ((projectDisbursements / 100) * sector.FundsPercentage);
                                        if (months > 0)
                                        {
                                            project.PlannedDisbursements = Math.Round((project.ProjectCost - project.ActualDisbursements) / months);
                                            if (project.PlannedDisbursements < 0)
                                            {
                                                project.PlannedDisbursements = 0;
                                            }
                                        }
                                    }
                                }
                            }

                            if (totalDisbursements > 0)
                            {
                                totalDisbursementsPercentage = ((totalDisbursements / 100) *  sector.FundsPercentage);
                            }

                            foreach(var project in sectorProjects)
                            {
                                projectsListForSector.Add(new ProjectViewForSector()
                                {
                                    Title = project.Title,
                                    StartDate = project.StartDate,
                                    EndDate = project.EndDate,
                                    Funders = string.Join(",", project.Funders.Select(f => f.Funder)),
                                    Implementers = string.Join(", ", project.Implementers.Select(i => i.Implementer)),
                                    ProjectCost = project.ProjectCost,
                                    ActualDisbursements = project.ActualDisbursements,
                                    PlannedDisbursements = project.PlannedDisbursements,
                                }) ;
                            }

                            projectsBySector.TotalFunding = totalFundingPercentage;
                            projectsBySector.TotalDisbursements = totalDisbursementsPercentage;
                            projectsBySector.Projects = projectsListForSector;
                            sectorProjectsList.Add(projectsBySector);
                            projectIds.Clear();
                        }
                    }
                    sectorProjectsReport.SectorProjectsList = sectorProjectsList;
                }
                catch(Exception ex)
                {
                    string error = ex.Message;
                }
                return await Task<ProjectProfileReportBySector>.Run(() => sectorProjectsReport).ConfigureAwait(false);
            }
        }
    }
}
