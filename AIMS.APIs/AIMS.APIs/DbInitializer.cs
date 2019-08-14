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
            try
            {
                // Run Migrations
                context.Database.Migrate();
                if (context.EmailMessages.Count() == 0)
                {
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewUser, TypeDefinition = "New user registration", Message = "New user registered" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewProjectToOrg, TypeDefinition = "New project to organization", Message = "New project added to organanization" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.UserInactive, TypeDefinition = "User inactive", Message = "User is inactive" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ChangedMappingEffectedProject, TypeDefinition = "Sector mapping updated", Message = "Sector mapping updated" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewIATISector, TypeDefinition = "Sector added from IATI", Message = "New sector/s added from IATI" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.OrganizationMerged, TypeDefinition = "Organization merged", Message = "Organization merged" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewOrgToProject, TypeDefinition = "New organization request for project", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectPermissionGranted, TypeDefinition = "Project permission approved/granted", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectPermissionDenied, TypeDefinition = "Project permission unapproved/denied", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.UserApproved, TypeDefinition = "User account approved", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.OrganizationRenamed, TypeDefinition = "Organization renamed", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectDeletionRequest, TypeDefinition = "Project deletion request", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectDeletionCancelled, TypeDefinition = "Project deletion cancelled", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectDeletionApproved, TypeDefinition = "Project deletion approved", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectDeleted, TypeDefinition = "Project deleted", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ResetPassword, TypeDefinition = "Reset password", Message = "Your password is reset successfully" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewIATIOrganization, TypeDefinition = "New IATI organizations", Message = "Some new organizations are added through IATI to AIMS DB." });
                    context.SaveChanges();
                }

                if (context.IATISettings.Count() == 0)
                {
                    context.IATISettings.Add(new EFIATISettings() { BaseUrl = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=SO&stream=true" });
                }

                if (context.FundingTypes.Count() == 0)
                {
                    context.FundingTypes.Add(new EFFundingTypes() { FundingType = "Grant" });
                    context.FundingTypes.Add(new EFFundingTypes() { FundingType = "Loan" });
                    context.SaveChanges();
                }

                if (context.StaticReports.Count() == 0)
                {
                    context.StaticReports.Add(new EFStaticReports() { Title = "Projects report" });
                    context.StaticReports.Add(new EFStaticReports() { Title = "Locations report" });
                    context.StaticReports.Add(new EFStaticReports() { Title = "Sectors report" });
                    context.StaticReports.Add(new EFStaticReports() { Title = "Budget report" });
                    context.StaticReports.Add(new EFStaticReports() { Title = "Project profile" });
                    context.StaticReports.Add(new EFStaticReports() { Title = "Excel report" });
                    context.SaveChanges();
                }

                if (context.FinancialYears.Count() == 0)
                {
                    context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2015 });
                    context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2016 });
                    context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2017 });
                    context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2018 });
                    context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2019 });
                    context.FinancialYears.Add(new EFFinancialYears() { FinancialYear = 2020 });

                    context.SaveChanges();
                }

                if (context.ExchangeRatesSettings.Count() == 0)
                {
                    context.ExchangeRatesSettings.Add(new EFExchangeRatesSettings()
                    {
                        APIKeyOpenExchangeRates = "ce2f27af4d414969bfe05b7285a01dec",
                        IsAutomatic = true,
                        ManualExchangeRates = null,
                        ManualExchangeRateSource = "Central Bank"
                    });
                    context.SaveChanges();
                }

                if (context.SMTPSettings.Count() == 0)
                {
                    context.SMTPSettings.Add(new EFSMTPSettings()
                    {
                        AdminEmail = "aims.developer18@gmail.com",
                        Host = "smtp.gmail.com",
                        Port = 587,
                        Username = "aims.developer18@gmail.com",
                        Password = "aims@123secure!"
                    });
                    context.SaveChanges();
                }

                EFSectorTypes somaliSectorType = null;
                EFSector otherSector = null;
                if (context.SectorTypes.Count() == 0)
                {
                    somaliSectorType = context.SectorTypes.Add(new EFSectorTypes() { TypeName = "Somali Sectors", IsPrimary = true }).Entity;
                    otherSector = context.Sectors.Add(new EFSector()
                    {
                        SectorType = somaliSectorType,
                        SectorName = "Other",
                        ParentSector = null
                    }).Entity;
                    context.SaveChanges();
                }

                if (context.Locations.Count() == 0)
                {
                    context.Locations.Add(new EFLocation()
                    {
                        Location = "Other",
                        Longitude = 0,
                        Latitude = 0
                    });
                }

                if (context.Organizations.Count() == 0)
                {
                    //Funders & Implementers
                    var undp = context.Organizations.Add(new EFOrganization() { OrganizationName = "UNDP" });
                    var mgec = context.Organizations.Add(new EFOrganization() { OrganizationName = "MGEC" });
                    var mop = context.Organizations.Add(new EFOrganization() { OrganizationName = "Ministry of Planning, Somalia" });
                    /*context.Organizations.Add(new EFOrganization() { OrganizationName = "DFID" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "USA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "UK" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "GEF" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "EU" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Netherlands" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "African Water Facility" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Netherlands" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "AFDB" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Japan" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "QATAR" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "USAID" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "UNEP" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "DANIDA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Switzerland" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "FAO" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "CHF" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Finland" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Italy" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "The Global Fund" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "FCO" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "UNICEF - GC" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Norway" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Sweden" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "UNODC" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Denmark" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "WB SPF" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "WORLD VISION" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Government Counterparts" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "HIRDA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "NCA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Bancroft Global Development" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "SRDA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "INTERSOS" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "SEDHURO" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "SHRA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "CEDA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "RAAS" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "GEWDO" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "HARD" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "EDRO" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "UNOPS" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "UNODC" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "UNICEF" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "MOE" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "IMWSC" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "MOWR" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "MOH" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Ministries of Health" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "OSPAD" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "AIDS Commissions" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Progressio" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "SCRCS" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Swisso Kalmo" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "CISP" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Mercy USA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Galkayo Medical Foundation" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "SOS Village Children" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Human Development Concern" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "SAHAN" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "WAWA" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "PLHIV networks" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Physician Across Continents" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "CCS" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "PUNCHAD" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Armament Policy Support" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "ASI" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "MUSLIM AID" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "SWISSO KALMO" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "ICRC" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Arma dei Carabinieri" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "ADAM SMITH INTERNATIONAL LTD" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "FAO" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "TERRE SOLIDALI ONLUS ASSOCIAZIONE" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "MdM France" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "OXFAM" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Population Services International" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "NORWEGIAN REFUGEE COUNCIL" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "CONCERN WORLDWIDE" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Save the Children International" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Action Against Hunger (ACF)" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Concern World Wide" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Care International" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "ASSOCIATION OF EUROPEAN PARLIAMENTARIANS WITH AFRICA VERENIGING" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "IRG" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Stichting Care Nederland" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "AUIBAR" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "CARE" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "HALO Trust" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "RED BARNET FORENING" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "MERCY CORPS" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "STIFTELSEN FLYKTNINGHJELPEN (NRC)" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "THE INTERNATIONAL CENTRE OF INSECTPHYSIOLOGY AND ECOLOGY LBG" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "INTERPEACE EUROPE AISBL" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Oxfam Novib" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "NRC" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "SPARK" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Fairfishing" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "ADRA DEUTSCHLAND EV" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "ACTED" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "International Organization for Migration" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "IDLO" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Ministry of Finance" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Conflict Dynamics" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "WORLD VISION DEUTSCHLAND EV" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "African Water Facility" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Saferworld" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "AFRICAN DEVELOPMENT SOLUTIONS (ADESO) ASSOCIATION" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "WORLD VISION DEUTSCHLAND EV" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "STIFTELSEN FLYKTNINGHJELPEN (NRC)" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Danish Demming Group" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Mott Macdonald" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Creative Associates International Inc." });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "DAI" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "CESVI" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "International Rescue Committee" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Concern World Wide" });
                    context.Organizations.Add(new EFOrganization() { OrganizationName = "Norwegian Refugee Council" });*/

                    var managerUser = context.Users.Add(new EFUser()
                    {
                        Email = "aims.developer18@gmail.com",
                        Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                        Organization = undp.Entity,
                        RegistrationDate = DateTime.Now,
                        IsApproved = true,
                        UserType = UserTypes.Manager
                    });

                    context.Users.Add(new EFUser()
                    {
                        Email = "work@mattgeddes.co.uk",
                        Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                        Organization = mgec.Entity,
                        RegistrationDate = DateTime.Now,
                        IsApproved = true,
                        UserType = UserTypes.Manager
                    });

                    context.Users.Add(new EFUser()
                    {
                        Email = "mohammedgele22@qq.com",
                        Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                        Organization = mop.Entity,
                        RegistrationDate = DateTime.Now,
                        IsApproved = true,
                        UserType = UserTypes.Manager
                    });

                    context.Users.Add(new EFUser()
                    {
                        Email = "pau.blanquer@undp.org",
                        Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                        Organization = undp.Entity,
                        RegistrationDate = DateTime.Now,
                        IsApproved = true,
                        UserType = UserTypes.Manager
                    });

                    context.Users.Add(new EFUser()
                    {
                        Email = "sarah.cramer@one.un.org",
                        Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                        Organization = undp.Entity,
                        RegistrationDate = DateTime.Now,
                        IsApproved = true,
                        UserType = UserTypes.Manager
                    });

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
