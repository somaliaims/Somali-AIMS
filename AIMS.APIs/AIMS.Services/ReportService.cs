﻿using AIMS.DAL.EF;
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
        Task<ProjectProfileReportBySector> GetProjectsBySector(ReportModelForProjectSectors model);
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

        public async Task<ProjectProfileReportBySector> GetProjectsBySector(ReportModelForProjectSectors model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ProjectProfileReportBySector sectorProjectsReport = new ProjectProfileReportBySector();
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

                List < ProjectProfileView > projectsList = new List<ProjectProfileView>();
                foreach(var project in projectProfileListObj)
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
                return await Task<ProjectProfileReportBySector>.Run(() => sectorProjectsReport).ConfigureAwait(false);
            }
                
        }
    }
}