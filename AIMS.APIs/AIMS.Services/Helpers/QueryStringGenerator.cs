using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services.Helpers
{
    public interface IQueryStringGenerator
    {
        /// <summary>
        /// Prepares a querystring for sectors reports ui
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetQueryStringForSectorsReport(SearchProjectsBySectorModel model);

        /// <summary>
        /// Prepares a querystring for locations reports ui
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetQueryStringForLocationsReport(SearchProjectsByLocationModel model);
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

            if (!string.IsNullOrEmpty(model.Title))
            {
                queryString += ("&title=" + model.Title);
            }

            if (model.StartingYear > 0)
            {
                queryString += ("&syear=" + model.StartingYear);
            }

            if (model.EndingYear > 0)
            {
                queryString += ("&eyear=" + model.EndingYear);
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

            if (!string.IsNullOrEmpty(model.Title))
            {
                queryString += ("&title=" + model.Title);
            }

            if (model.StartingYear > 0)
            {
                queryString += ("&syear=" + model.StartingYear);
            }

            if (model.EndingYear > 0)
            {
                queryString += ("&eyear=" + model.EndingYear);
            }
            return queryString;
        }
    }
}
