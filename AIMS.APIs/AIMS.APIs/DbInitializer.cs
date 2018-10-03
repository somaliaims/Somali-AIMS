using AIMS.DAL.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;

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

            if (context.OrganizationTypes.Count() == 0)
            {
                var type1 = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Govt" });
                var type2 = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Private" });
                var type3 = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Semi-Govt" });

                var orgMop = context.Organizations.Add(new EFOrganization() { OrganizationName = "MoPIED", OrganizationType = type1.Entity });
                var adminUser = context.Users.Add(new EFUser() { Email = "admin@aims.org", DisplayName = "Super Admin",
                    Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                    Organization = orgMop.Entity, RegistrationDate =  DateTime.Now, IsApproved = true, UserType = UserTypes.SuperAdmin});

                context.SaveChanges();
            }
        }
    }
}
