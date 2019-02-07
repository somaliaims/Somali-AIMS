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

            if (context.FinancialYears.Count() == 0)
            {
                context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2015 } );
                context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2016 });
                context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2017 });
                context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2018 });
                context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2019 });
                context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2020 });

                context.SaveChanges();
            }

            if (context.Organizations.Count() == 0)
            {
                /*var typeDefault = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Default" });
                var type1 = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Govt" });
                var type2 = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Private" });
                var type3 = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Semi-Govt" });*/

                var orgMop = context.Organizations.Add(new EFOrganization() { OrganizationName = "MoPIED" });
                var adminUser = context.Users.Add(new EFUser() { Email = "admin@aims.org", Name = "Super Admin",
                    Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                    Organization = orgMop.Entity, RegistrationDate =  DateTime.Now, IsApproved = true, UserType = UserTypes.SuperAdmin});

                var managerUser = context.Users.Add(new EFUser()
                {
                    Email = "manager@aims.org",
                    Name = "Manager",
                    Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                    Organization = orgMop.Entity,
                    RegistrationDate = DateTime.Now,
                    IsApproved = true,
                    UserType = UserTypes.Manager
                });

                var standardUser = context.Users.Add(new EFUser()
                {
                    Email = "standard@aims.org",
                    Name = "Manager",
                    Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                    Organization = orgMop.Entity,
                    RegistrationDate = DateTime.Now,
                    IsApproved = true,
                    UserType = UserTypes.Standard
                });

                context.SaveChanges();
            }
        }
    }
}
