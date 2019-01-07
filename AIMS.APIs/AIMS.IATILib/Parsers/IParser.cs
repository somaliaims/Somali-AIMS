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
        /// Parses short version of projects
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        ICollection<IATIProject> ExtractProjects(XDocument xmlDoc);
    }
}
