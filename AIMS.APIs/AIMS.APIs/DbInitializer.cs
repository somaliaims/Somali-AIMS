using AIMS.DAL.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMS.APIs
{
    public class DbInitializer
    {
        public AIMSDbContext context;

        public DbInitializer()
        {

        }

        public void SetDbContext(AIMSDbContext ctx)
        {
            context = ctx;
        }

        public void Seed()
        {
            // Run Migrations
            context.Database.Migrate();
        }
    }
}
