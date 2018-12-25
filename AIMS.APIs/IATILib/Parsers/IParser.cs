using AIMS.Models;
using IATILib.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AIMS.APIs.IATILib.Parsers
{
    public interface IParser
    {
        ICollection<IATIActivity> ExtractAcitivities(XDocument xmlDoc);
    }
}
