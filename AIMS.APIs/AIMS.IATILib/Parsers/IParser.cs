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
        ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc, string criteria);

        /// <summary>
        /// Extract activities for ids
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        ICollection<IATIActivity> ExtractAcitivitiesForIds(XDocument xmlDoc, IEnumerable<string> Ids);

        /// <summary>
        /// Parses short version of projects
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        ICollection<IATIProject> ExtractProjects(XDocument xmlDoc);
    }
}
