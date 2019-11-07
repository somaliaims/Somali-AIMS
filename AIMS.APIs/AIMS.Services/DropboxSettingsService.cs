using AIMS.DAL.EF;
using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services
{
    public interface IDropboxSettingsService
    {
        ActionResponse UpdateToken(DropboxSettingsModel model);
    }

    public class DropboxSettingsService
    {
        AIMSDbContext context;

        public DropboxSettingsService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public ActionResponse UpdateToken(DropboxSettingsModel model)
        {
            ActionResponse response = new ActionResponse();
            return response;
        }
    }
}
