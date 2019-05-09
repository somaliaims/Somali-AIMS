using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AIMS.IATILib.Parsers
{
    public interface IParser
    {
        /// <summary>
        /// Parses IATI Activities
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc, string criteria, List<IATITransactionTypes> transactionTypes = null);

        /// <summary>
        /// Extract activities for ids
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        ICollection<IATIActivity> ExtractAcitivitiesForIds(XDocument xmlDoc, IEnumerable<string> Ids, List<IATITransactionTypes> transactionTypes = null);

        /// <summary>
        /// Parses short version of projects
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        ICollection<IATIProject> ExtractProjects(XDocument xmlDoc);

        /// <summary>
        /// Parses short version of sectors
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        ICollection<IATISectorModel> ExtractSectors(XDocument xmlDoc);

        /// <summary>
        /// Parses organizations from the IATI document
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        ICollection<IATIOrganizationModel> ExtractOrganizations(XDocument xmlDoc);
    }
}
