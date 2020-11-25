using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMS.APIs.Helpers
{
    public interface ICommonMessageHelper
    {
        /// <summary>
        /// Gets message for invalid id provided
        /// </summary>
        /// <returns></returns>
        string GetInvalidIdMessage();
    }

    public class CommonMessageHelper : ICommonMessageHelper
    {
        public CommonMessageHelper()
        {
        }

        public string GetInvalidIdMessage()
        {
            return ("Invalid id provided");
        }
    }
}
