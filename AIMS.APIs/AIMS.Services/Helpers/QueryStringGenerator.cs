using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Net;
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

            if (model.SubLocationIds.Count > 0)
            {
                string sublocationsIdsStr = string.Join(",", model.SubLocationIds);
                queryString += ("&slocations=" + sublocationsIdsStr);
            }

            if (model.SectorOption == NoSectorOptions.ProjectsWithoutSectors)
            {
                queryString += ("&noSectors=true");
            }

            if (model.SectorLevel == SectorLevels.Parent)
            {
                queryString += ("&level=" + (int)SectorLevels.Parent);
            }
            else if(model.SectorLevel == SectorLevels.Child)
            {
                queryString += ("&level=" + (int)SectorLevels.Child);
            }

            if (model.ChartType > 0)
            {
                queryString += ("&ctype=" + model.ChartType);
            }

            if (model.MarkerId > 0)
            {
                queryString += ("&mid=" + model.MarkerId);
            }

            if (model.MarkerValues.Count > 0)
            {
                for(int m=0; m < model.MarkerValues.Count; m++)
                {
                    model.MarkerValues[m] = WebUtility.UrlEncode(model.MarkerValues[m]);
                }
                queryString += ("&mvalue=" + string.Join(",", model.MarkerValues));
            }

            if (model.MarkerId2 > 0)
            {
                queryString += ("&mid2=" + model.MarkerId2);
            }

            if (model.MarkerValues2.Count > 0)
            {
                for (int m = 0; m < model.MarkerValues2.Count; m++)
                {
                    model.MarkerValues2[m] = WebUtility.UrlEncode(model.MarkerValues2[m]);
                }
                queryString += ("&mvalue2=" + string.Join(",", model.MarkerValues2));
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

            if (model.SubLocationIds.Count > 0)
            {
                string subLocationIdsStr = string.Join(",", model.SubLocationIds);
                queryString += ("&slocations=" + subLocationIdsStr);
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

            if (model.MarkerId > 0)
            {
                queryString += ("&mid=" + model.MarkerId);
            }

            if (model.MarkerValues.Count > 0)
            {
                for (int m = 0; m < model.MarkerValues.Count; m++)
                {
                    model.MarkerValues[m] = WebUtility.UrlEncode(model.MarkerValues[m]);
                }
                queryString += ("&mvalue=" + string.Join(",", model.MarkerValues));
            }

            if (model.MarkerId2 > 0)
            {
                queryString += ("&mid2=" + model.MarkerId2);
            }

            if (model.MarkerValues2.Count > 0)
            {
                for (int m = 0; m < model.MarkerValues2.Count; m++)
                {
                    model.MarkerValues2[m] = WebUtility.UrlEncode(model.MarkerValues2[m]);
                }
                queryString += ("&mvalue2=" + string.Join(",", model.MarkerValues2));
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

            if (model.SubLocationIds.Count > 0)
            {
                queryString += "&slocations=" + string.Join(",", model.SubLocationIds);
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

            if (model.MarkerId > 0)
            {
                queryString += ("&mid=" + model.MarkerId);
            }

            if (model.MarkerValues.Count > 0)
            {
                for (int m = 0; m < model.MarkerValues.Count; m++)
                {
                    model.MarkerValues[m] = WebUtility.UrlEncode(model.MarkerValues[m]);
                }
                queryString += ("&mvalue=" + string.Join(",", model.MarkerValues));
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

            if (model.FunderTypeIds.Count > 0)
            {
                string funderTypeIdsStr = string.Join(",", model.FunderTypeIds);
                queryString += ("&ftypes=" + funderTypeIdsStr);
            }

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
