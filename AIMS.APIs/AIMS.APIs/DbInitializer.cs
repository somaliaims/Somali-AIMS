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

                if (context.HomePageSettings.Count() == 0)
                {
                    context.HomePageSettings.Add(new EFHomePageSettings()
                    {
                        AIMSTitle = "Somali AIMS",
                        IntroductionHeading = "Introduction",
                        IntroductionText = "Welcome to Somali AIMS"
                    });
                }

                if (context.EnvelopeTypes.Count() == 0)
                {
                    context.EnvelopeTypes.Add(new EFEnvelopeTypes() { TypeName = "Development" });
                    context.EnvelopeTypes.Add(new EFEnvelopeTypes() { TypeName = "Humanitarian" });
                }

                if (context.EmailMessages.Count() == 0)
                {
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewUser, TypeDefinition = "New user registration", Subject= "New user registration", Message = "New user registered" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewProjectToOrg, TypeDefinition = "New project to organization", Subject="Project added to organization", Message = "New project added to organanization" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.UserInactive, TypeDefinition = "User inactive", Subject="User account deactivated", Message = "User is inactive" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ChangedMappingEffectedProject, TypeDefinition = "Sector mapping updated", Subject="Sector mapping updated", Message = "Sector mapping updated" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewIATISector, TypeDefinition = "Sector added from IATI", Subject="Sector/s added from IATI", Message = "New sector/s added from IATI" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.OrganizationMerged, TypeDefinition = "Organization merged", Subject="Organizations merged", Message = "Organization merged" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewOrgToProject, TypeDefinition = "New organization request for project", Subject="Organization requesting to join project", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectPermissionGranted, TypeDefinition = "Project permission approved/granted", Subject="Project membership granted", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectPermissionDenied, TypeDefinition = "Project permission unapproved/denied", Subject="Project membership denied", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.UserApproved, TypeDefinition = "User account approved", Subject="User account approval confirmation", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.OrganizationRenamed, TypeDefinition = "Organization renamed", Subject="Organization renamed", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectDeletionRequest, TypeDefinition = "Project deletion request", Subject="Project deletion request", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectDeletionCancelled, TypeDefinition = "Project deletion cancelled", Subject="Project deletion request cancelled", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ProjectDeletionApproved, TypeDefinition = "Project deletion approved", Subject="Project deletion approved", Message = "" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.ResetPassword, TypeDefinition = "Reset password", Subject="User password reset", Message = "Your password is reset successfully" });
                    context.EmailMessages.Add(new EFEmailMessages() { MessageType = EmailMessageType.NewIATIOrganization, TypeDefinition = "New IATI organizations", Subject="New organization/s", Message = "Some new organizations are added through IATI to AIMS DB." });
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

                if (context.OrganizationTypes.Count() == 0)
                {
                    context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Default" });
                    context.SaveChanges();
                }

                if (context.SectorTypes.Count() == 0)
                {
                    var primary = context.SectorTypes.Add(new EFSectorTypes() 
                    { 
                        TypeName = "Somali Sectors", 
                        IsPrimary = true, 
                        IsSourceType = false, 
                        IATICode = 0 
                    });
                    context.SectorTypes.Add(new EFSectorTypes() { TypeName = "Default" });

                    var inclusivePolitics = context.Sectors.Add(new EFSector() 
                    { 
                        ParentSector = null, 
                        SectorType = primary.Entity, 
                        SectorName = "Pillar 1: Inclusive Politics", 
                        TimeStamp = DateTime.Now 
                    });
                    context.Sectors.Add(new EFSector() 
                    { 
                        ParentSector = inclusivePolitics.Entity, 
                        SectorType = primary.Entity, 
                        SectorName = "Inclusive Politics", 
                        TimeStamp = DateTime.Now 
                    });

                    var security = context.Sectors.Add(new EFSector() 
                    { 
                        ParentSector = null, 
                        SectorType = primary.Entity, 
                        SectorName = "Pillar 2: Security", 
                        TimeStamp = DateTime.Now 
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = security.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Security",
                        TimeStamp = DateTime.Now
                    });

                    var ruleOfLaw = context.Sectors.Add(new EFSector()
                    {
                        ParentSector = null,
                        SectorType = primary.Entity,
                        SectorName = "Pillar 3: Rule of Law",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = ruleOfLaw.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Rule of Law",
                        TimeStamp = DateTime.Now
                    });

                    var effectiveInstitutions = context.Sectors.Add(new EFSector()
                    {
                        ParentSector = null,
                        SectorType = primary.Entity,
                        SectorName = "Pillar 4: Effective, Efficient Institutions",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = effectiveInstitutions.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Civil Service Reform / Public Administration",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = effectiveInstitutions.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Planning, M&E and Statistics",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = effectiveInstitutions.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Public Financial Management",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = effectiveInstitutions.Entity,
                        SectorType = primary.Entity,
                        SectorName = "State and Local Governance",
                        TimeStamp = DateTime.Now
                    });

                    var economicalGrowth = context.Sectors.Add(new EFSector()
                    {
                        ParentSector = null,
                        SectorType = primary.Entity,
                        SectorName = "Pillar 5: Economic Growth",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = economicalGrowth.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Agriculture - Irrigated and rain-fed crops",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = economicalGrowth.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Employment and skills development",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = economicalGrowth.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Livestock",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = economicalGrowth.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Private Sector Development",
                        TimeStamp = DateTime.Now
                    });

                    var infrastructure = context.Sectors.Add(new EFSector()
                    {
                        ParentSector = null,
                        SectorType = primary.Entity,
                        SectorName = "Pillar 6: Infrastructure",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = infrastructure.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Energy and ICT",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = infrastructure.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Other infrastructure",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = infrastructure.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Transport",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = infrastructure.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Water and Sanitation (Urban)",
                        TimeStamp = DateTime.Now
                    });

                    var social = context.Sectors.Add(new EFSector()
                    {
                        ParentSector = null,
                        SectorType = primary.Entity,
                        SectorName = "Pillar 7: Social & Human Development",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = social.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Education",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = social.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Health",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = social.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Nutrition",
                        TimeStamp = DateTime.Now
                    });

                    var resilience = context.Sectors.Add(new EFSector()
                    {
                        ParentSector = null,
                        SectorType = primary.Entity,
                        SectorName = "Pillar 8: Resilience",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = resilience.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Disaster Risk Reduction",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = resilience.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Environment & Natural Resources Management",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = resilience.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Food Security",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = resilience.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Migration, Displacement, Refugees and Durable Solutions",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = resilience.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Social Protection & Safety Nets",
                        TimeStamp = DateTime.Now
                    });

                    var gender = context.Sectors.Add(new EFSector()
                    {
                        ParentSector = null,
                        SectorType = primary.Entity,
                        SectorName = "Pillar 9: Gender & Human Rights",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = gender.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Gender & Human Rights",
                        TimeStamp = DateTime.Now
                    });

                    var other = context.Sectors.Add(new EFSector()
                    {
                        ParentSector = null,
                        SectorType = primary.Entity,
                        SectorName = "Other",
                        TimeStamp = DateTime.Now
                    });
                    context.Sectors.Add(new EFSector()
                    {
                        ParentSector = other.Entity,
                        SectorType = primary.Entity,
                        SectorName = "Other",
                        TimeStamp = DateTime.Now
                    });
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
                        Location = "UNATTRIBUTED",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.Locations.Add(new EFLocation()
                    {
                        Location = "FGS",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.Locations.Add(new EFLocation()
                    {
                        Location = "BRA",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.Locations.Add(new EFLocation()
                    {
                        Location = "GALMUDUG",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.Locations.Add(new EFLocation()
                    {
                        Location = "HIIRSHABELLE",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.Locations.Add(new EFLocation()
                    {
                        Location = "JUBALAND",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.Locations.Add(new EFLocation()
                    {
                        Location = "PUNTLAND",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.Locations.Add(new EFLocation()
                    {
                        Location = "SOUTH WEST",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.Locations.Add(new EFLocation()
                    {
                        Location = "SOMALILAND",
                        Longitude = 0,
                        Latitude = 0
                    });

                    context.SaveChanges();
                }

                if (context.Markers.Count() == 0)
                {
                    context.Markers.Add(new EFMarkers()
                    {
                       FieldTitle = "GENDER MARKER",
                       FieldType = FieldTypes.Text,
                       Help = "",
                       Values = ""
                    });

                    context.Markers.Add(new EFMarkers()
                    {
                        FieldTitle = "CAPACITY DEVELOPMENT MARKER",
                        FieldType = FieldTypes.Text,
                        Help = "",
                        Values = ""
                    });

                    context.Markers.Add(new EFMarkers()
                    {
                        FieldTitle = "STABALIZATION/CRESTA",
                        FieldType = FieldTypes.Text,
                        Help = "",
                        Values = ""
                    });

                    context.Markers.Add(new EFMarkers()
                    {
                        FieldTitle = "DURABLE SOLUTIONS",
                        FieldType = FieldTypes.Text,
                        Help = "",
                        Values = ""
                    });

                    context.Markers.Add(new EFMarkers()
                    {
                        FieldTitle = "YOUTH MARKER",
                        FieldType = FieldTypes.Text,
                        Help = "",
                        Values = ""
                    });

                    context.Markers.Add(new EFMarkers()
                    {
                        FieldTitle = "RRF MARKER",
                        FieldType = FieldTypes.Text,
                        Help = "",
                        Values = ""
                    });

                    context.Markers.Add(new EFMarkers()
                    {
                        FieldTitle = "HUMANATARIAN",
                        FieldType = FieldTypes.Text,
                        Help = "",
                        Values = ""
                    });

                    context.Markers.Add(new EFMarkers()
                    {
                        FieldTitle = "PWG CONSULTATION",
                        FieldType = FieldTypes.Text,
                        Help = "",
                        Values = ""
                    });

                    context.SaveChanges();
                }

                if (context.Organizations.Count() == 0)
                {
                    //Funders & Implementers
                    var unAgency = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "UN Agency" });
                    var federalGovt = context.OrganizationTypes.Add(new EFOrganizationTypes() { TypeName = "Federal Government" });
                    var undp = context.Organizations.Add(new EFOrganization() { OrganizationName = "UNDP", OrganizationType = unAgency.Entity });
                    var mop = context.Organizations.Add(new EFOrganization() { OrganizationName = "Ministry of Planning, Somalia", OrganizationType = federalGovt.Entity });
                    
                    if (context.Users.Count() == 0)
                    {
                        context.Users.Add(new EFUser()
                        {
                            Email = "raashid.ahmad@gmail.com",
                            Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                            Organization = undp.Entity,
                            RegistrationDate = DateTime.Now,
                            IsApproved = true,
                            UserType = UserTypes.Manager
                        });

                        var superUser = context.Users.Add(new EFUser()
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
                            Organization = undp.Entity,
                            RegistrationDate = DateTime.Now,
                            IsApproved = true,
                            UserType = UserTypes.Standard
                        });

                        context.Users.Add(new EFUser()
                        {
                            Email = "ict@mop.gov.so",
                            Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                            Organization = mop.Entity,
                            RegistrationDate = DateTime.Now,
                            IsApproved = true,
                            UserType = UserTypes.Standard
                        });

                        context.Users.Add(new EFUser()
                        {
                            Email = "pau.blanquer@undp.org",
                            Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                            Organization = undp.Entity,
                            RegistrationDate = DateTime.Now,
                            IsApproved = true,
                            UserType = UserTypes.Standard
                        });

                        context.Users.Add(new EFUser()
                        {
                            Email = "sarah.cramer@one.un.org",
                            Password = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
                            Organization = undp.Entity,
                            RegistrationDate = DateTime.Now,
                            IsApproved = true,
                            UserType = UserTypes.Standard
                        });
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
