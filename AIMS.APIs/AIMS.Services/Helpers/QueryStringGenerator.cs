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
            string queryString = "";
            if (model.OrganizationIds.Count > 0)
            {
                queryString = "orgs" + string.Join(",", model.OrganizationIds);
            }

            if (model.SectorIds.Count > 0)
            {
                string sectorIdsStr = string.Join(",", model.SectorIds);
                queryString += queryString.Length > 0 ? ("&sectors=" + sectorIdsStr) : ("sectors=" + sectorIdsStr);
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                queryString += queryString.Length > 0 ? ("&title=" + model.Title) : ("title=" + model.Title);
            }

            if (model.StartingYear > 0)
            {
                queryString += queryString.Length > 0 ? ("&syear=" + model.StartingYear) : ("syear=" + model.StartingYear);
            }

            if (model.EndingYear > 0)
            {
                queryString += queryString.Length > 0 ? ("&eyear=" + model.EndingYear) : ("eyear=" + model.EndingYear);
            }
            return queryString;
        }

        public string GetQueryStringForLocationsReport(SearchProjectsByLocationModel model)
        {
            string queryString = "";
            if (model.OrganizationIds.Count > 0)
            {
                queryString = "orgs" + string.Join(",", model.OrganizationIds);
            }

            if (model.LocationIds.Count > 0)
            {
                string locationIdsStr = string.Join(",", model.LocationIds);
                queryString += queryString.Length > 0 ? ("&locations=" + locationIdsStr) : ("locations=" + locationIdsStr);
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                queryString += queryString.Length > 0 ? ("&title=" + model.Title) : ("title=" + model.Title);
            }

            if (model.StartingYear > 0)
            {
                queryString += queryString.Length > 0 ? ("&syear=" + model.StartingYear) : ("syear=" + model.StartingYear);
            }

            if (model.EndingYear > 0)
            {
                queryString += queryString.Length > 0 ? ("&eyear=" + model.EndingYear) : ("eyear=" + model.EndingYear);
            }
            return queryString;
        }
    }
}
