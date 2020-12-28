using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AIMS.APIs.Scheduler
{
    internal class CustomWebClient : WebClient
    {
        internal int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri Address)
        {
            WebRequest WebReq = base.GetWebRequest(Address);
            WebReq.Timeout = Timeout * 1000; // Seconds
            return WebReq;
        }
    }
}
