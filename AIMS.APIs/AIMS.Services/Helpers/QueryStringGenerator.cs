using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services.Helpers
{
    public interface IQueryStringGenerator
    {
        /// <summary>
        /// Prepares a querystring for sectors reports using UI
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetQueryStringForSectorsReport(SearchProjectsBySectorModel model);

        /// <summary>
        /// Prepares a querystring for locations reports using UI
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetQueryStringForLocationsReport(SearchProjectsByLocationModel model);

        /// <summary>
        /// Prepares a querystring for time series reports using UI
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetQueryStringForTimeSeriesReport(SearchProjectsByYearModel model);

        /// <summary>
        /// Prepares a querystring for envelope report reload using UI
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetQueryStringForEnvelopeReport(SearchEnvelopeModel model);
    }

    public class QueryStringGenerator : IQueryStringGenerator
    {
        public QueryStringGenerator()
        {

        }

        public string GetQueryStringForSectorsReport(SearchProjectsBySectorModel model)
        {
            string queryString = "?load=true";
            if (model.OrganizationIds.Count > 0)
            {
                queryString += "&orgs=" + string.Join(",", model.OrganizationIds);
            }

            if (model.SectorIds.Count > 0)
            {
                string sectorIdsStr = string.Join(",", model.SectorIds);
                queryString += ("&sectors=" + sectorIdsStr);
            }

            if (model.ProjectIds.Count > 0)
            {
                string projectIdsStr = string.Join(",", model.ProjectIds);
                queryString += ("&projects=" + projectIdsStr);
            }

            if (model.StartingYear > 0)
            {
                queryString += ("&syear=" + model.StartingYear);
            }

            if (model.EndingYear > 0)
            {
                queryString += ("&eyear=" + model.EndingYear);
            }

            if (model.LocationId > 0)
            {
                queryString += ("&locationId=" + model.LocationId);
            }

            if (model.SectorOption == NoSectorOptions.ProjectsWithoutSectors)
            {
                queryString += ("&noSectors=true");
            }

            if (model.ChartType > 0)
            {
                queryString += ("&ctype=" + model.ChartType);
            }
            return queryString;
        }

        public string GetQueryStringForLocationsReport(SearchProjectsByLocationModel model)
        {
            string queryString = "?load=true";
            if (model.OrganizationIds.Count > 0)
            {
                queryString += "&orgs=" + string.Join(",", model.OrganizationIds);
            }

            if (model.LocationIds.Count > 0)
            {
                string locationIdsStr = string.Join(",", model.LocationIds);
                queryString += ("&locations=" + locationIdsStr);
            }

            if (model.ProjectIds.Count > 0)
            {
                string projectIdsStr = string.Join(",", model.ProjectIds);
                queryString += ("&projects=" + projectIdsStr);
            }

            if (model.StartingYear > 0)
            {
                queryString += ("&syear=" + model.StartingYear);
            }

            if (model.EndingYear > 0)
            {
                queryString += ("&eyear=" + model.EndingYear);
            }

            if (model.SectorId > 0)
            {
                queryString += ("&sectorId=" + model.SectorId);
            }

            if (model.LocationOption == NoLocationOptions.ProjectsWithoutLocations)
            {
                queryString += ("&noLocations=true");
            }

            if (model.ChartType > 0)
            {
                queryString += ("&ctype=" + model.ChartType);
            }
            return queryString;
        }

        public string GetQueryStringForTimeSeriesReport(SearchProjectsByYearModel model)
        {
            string queryString = "?load=true";
            if (model.OrganizationIds.Count > 0)
            {
                queryString += "&orgs=" + string.Join(",", model.OrganizationIds);
            }

            if (model.LocationId > 0)
            {
                queryString += ("&locationId=" + model.LocationId);
            }

            if (model.SectorIds.Count > 0)
            {
                string sectorIdsStr = string.Join(",", model.SectorIds);
                queryString += ("&sectors=" + sectorIdsStr);
            }

            if (model.ProjectIds.Count > 0)
            {
                string projectIdsStr = string.Join(",", model.ProjectIds);
                queryString += ("&projects=" + projectIdsStr);
            }

            if (model.StartingYear > 0)
            {
                queryString += ("&syear=" + model.StartingYear);
            }

            if (model.EndingYear > 0)
            {
                queryString += ("&eyear=" + model.EndingYear);
            }

            if (model.ChartType > 0)
            {
                queryString += ("&ctype=" + model.ChartType);
            }
            return queryString;
        }

        public string GetQueryStringForEnvelopeReport(SearchEnvelopeModel model)
        {
            string queryString = "?load=true";
            if (model.FunderIds.Count > 0)
            {
                queryString += "&funders=" + string.Join(",", model.FunderIds);
            }

            if (model.EnvelopeTypeIds.Count > 0)
            {
                string envelopeTypeIdsStr = string.Join(",", model.EnvelopeTypeIds);
                queryString += ("&envelopeTypes=" + envelopeTypeIdsStr);
            }

            if (model.StartingYear > 0)
            {
                queryString += ("&syear=" + model.StartingYear);
            }

            if (model.EndingYear > 0)
            {
                queryString += ("&eyear=" + model.EndingYear);
            }

            if (model.ChartType > 0)
            {
                queryString += ("&ctype=" + model.ChartType);
            }
            return queryString;
        }
    }
}
