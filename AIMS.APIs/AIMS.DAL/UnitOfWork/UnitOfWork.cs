using AIMS.DAL.EF;
using AIMS.DAL.Repository;
using AIMS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.DAL.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private AIMSDbContext context = null;
        private GenericRepository<EFFundingTypes> fundingTypesRepository;
        private GenericRepository<EFSectorTypes> sectorTypesRepository;
        private GenericRepository<EFSector> sectorRepository;
        private GenericRepository<EFSectorMappings> sectorMappingsRepository;
        private GenericRepository<EFLocation> locationRepository;
        private GenericRepository<EFOrganization> organizationRepository;
        private GenericRepository<EFIATIOrganization> iatiOrganizationRepository;
        private GenericRepository<EFOrganizationTypes> organizationTypesRepository;
        private GenericRepository<EFProject> projectRepository;
        private GenericRepository<EFProjectSectors> projectSectorsRepository;
        private GenericRepository<EFProjectFunders> fundersRepository;
        private GenericRepository<EFProjectImplementers> implementersRepository;
        private GenericRepository<EFUser> userRepository;
        private GenericRepository<EFProjectDocuments> projectDocumentsRepository;
        private GenericRepository<EFProjectDisbursements> projectDisbursementsRepository;
        private GenericRepository<EFUserNotifications> notificationsRepository;
        private GenericRepository<EFIATIData> iatiDataRepository;
        private GenericRepository<EFIATICountryCodes> iatiCountryRepository;
        private GenericRepository<EFSMTPSettings> smtpSettingsRepository;
        private GenericRepository<EFIATISettings> iatiSettingsRepository;
        private GenericRepository<EFPasswordRecoveryRequests> passwordRecoveryRepository;
        private GenericRepository<EFProjectLocations> projectLocationsRepository;
        private GenericRepository<EFFinancialYearSettings> financialYearSettingsRepository;
        private GenericRepository<EFFinancialYears> financialYearRepository;
        private GenericRepository<EFStaticReports> reportsRepository;
        private GenericRepository<EFReportSubscriptions> reportSubscriptionsRepository;
        private GenericRepository<EFCurrency> currencyRepository;
        private GenericRepository<EFExchangeRates> exchangeRatesRepository;
        private GenericRepository<EFExchangeRatesSettings> exRatesSettingsRepository;
        private GenericRepository<EFExchangeRatesUsageSettings> exRatesUsageSettingsRepository;
        private GenericRepository<EFExchangeRatesAPIsCount> exRatesAPIsRepository;
        private GenericRepository<EFEnvelopeTypes> envelopeTypesRepository;
        private GenericRepository<EFEnvelope> envelopeDataRepository;
        private GenericRepository<EFEnvelopeYearlyBreakup> envelopeBreakupsRepository;
        private GenericRepository<EFMarkers> markerRepository;
        private GenericRepository<EFProjectMarkers> projectMarkersRepository;
        private GenericRepository<EFEmailMessages> emailMessagesRepository;
        private GenericRepository<EFManualExchangeRates> manualRatesRepository;
        private GenericRepository<EFProjectMembershipRequests> projectMembershipRepository;
        private GenericRepository<EFProjectDeletionRequests> projectDeletionRepository;
        private GenericRepository<EFHelp> helpRepository;
        private GenericRepository<EFHomePageSettings> homePageRepository;
        private GenericRepository<EFDropboxSettings> dropboxSettingsRepository;

        public UnitOfWork(AIMSDbContext dbContext)
        {
            context = dbContext;
        }


        public GenericRepository<EFHomePageSettings> HomePageRepository
        {
            get
            {
                if (this.homePageRepository == null)
                    this.homePageRepository = new GenericRepository<EFHomePageSettings>(context);
                return this.homePageRepository;
            }
        }

        public GenericRepository<EFDropboxSettings> DropboxSettingsRepository
        {
            get
            {
                if (this.dropboxSettingsRepository == null)
                    this.dropboxSettingsRepository = new GenericRepository<EFDropboxSettings>(context);
                return this.dropboxSettingsRepository;
            }
        }

        public GenericRepository<EFHelp> HelpRepository
        {
            get
            {
                if (this.helpRepository == null)
                    this.helpRepository = new GenericRepository<EFHelp>(context);
                return this.helpRepository;
            }
        }

        public GenericRepository<EFEmailMessages> EmailMessagesRepository
        {
            get
            {
                if (this.emailMessagesRepository == null)
                    this.emailMessagesRepository = new GenericRepository<EFEmailMessages>(context);
                return this.emailMessagesRepository;
            }
        }

        public GenericRepository<EFProjectDeletionRequests> ProjectDeletionRepository
        {
            get
            {
                if (this.projectDeletionRepository == null)
                    this.projectDeletionRepository = new GenericRepository<EFProjectDeletionRequests>(context);
                return this.projectDeletionRepository;
            }
        }

        public GenericRepository<EFProjectMembershipRequests> ProjectMembershipRepository
        {
            get
            {
                if (this.projectMembershipRepository == null)
                    this.projectMembershipRepository = new GenericRepository<EFProjectMembershipRequests>(context);
                return this.projectMembershipRepository;
            }
        }

        public GenericRepository<EFManualExchangeRates> ManualRatesRepository
        {
            get
            {
                if (this.manualRatesRepository == null)
                    this.manualRatesRepository = new GenericRepository<EFManualExchangeRates>(context);
                return this.manualRatesRepository;
            }
        }

        public GenericRepository<EFFundingTypes> FundingTypeRepository
        {
            get
            {
                if (this.fundingTypesRepository == null)
                    this.fundingTypesRepository = new GenericRepository<EFFundingTypes>(context);
                return this.fundingTypesRepository;
            }
        }

        public GenericRepository<EFCurrency> CurrencyRepository
        {
            get
            {
                if (this.currencyRepository == null)
                    this.currencyRepository = new GenericRepository<EFCurrency>(context);
                return this.currencyRepository;
            }
        }

        public GenericRepository<EFMarkers> MarkerRepository
        {
            get
            {
                if (this.markerRepository == null)
                    this.markerRepository = new GenericRepository<EFMarkers>(context);
                return this.markerRepository;
            }
        }

        public GenericRepository<EFProjectMarkers> ProjectMarkersRepository
        {
            get
            {
                if (this.projectMarkersRepository == null)
                    this.projectMarkersRepository = new GenericRepository<EFProjectMarkers>(context);
                return this.projectMarkersRepository;
            }
        }

        public GenericRepository<EFEnvelopeTypes> EnvelopeTypesRepository
        {
            get
            {
                if (this.envelopeTypesRepository == null)
                    this.envelopeTypesRepository = new GenericRepository<EFEnvelopeTypes>(context);
                return this.envelopeTypesRepository;
            }
        }

        public GenericRepository<EFEnvelope> EnvelopeRepository
        {
            get
            {
                if (this.envelopeDataRepository == null)
                    this.envelopeDataRepository = new GenericRepository<EFEnvelope>(context);
                return this.envelopeDataRepository;
            }
        }

        public GenericRepository<EFEnvelopeYearlyBreakup> EnvelopeYearlyBreakupRepository
        {
            get
            {
                if (this.envelopeBreakupsRepository == null)
                    this.envelopeBreakupsRepository = new GenericRepository<EFEnvelopeYearlyBreakup>(context);
                return this.envelopeBreakupsRepository;
            }
        }

        public GenericRepository<EFFinancialYears> FinancialYearRepository
        {
            get
            {
                if (this.financialYearRepository == null)
                    this.financialYearRepository = new GenericRepository<EFFinancialYears>(context);
                return this.financialYearRepository;
            }
        }

        public GenericRepository<EFFinancialYearSettings> FinancialYearSettingsRepository
        {
            get
            {
                if (this.financialYearSettingsRepository == null)
                    this.financialYearSettingsRepository = new GenericRepository<EFFinancialYearSettings>(context);
                return this.financialYearSettingsRepository;
            }
        }

        public GenericRepository<EFReportSubscriptions> ReportSubscriptionRepository
        {
            get
            {
                if (this.reportSubscriptionsRepository == null)
                    this.reportSubscriptionsRepository = new GenericRepository<EFReportSubscriptions>(context);
                return this.reportSubscriptionsRepository;
            }
        }

        public GenericRepository<EFStaticReports> ReportsRepository
        {
            get
            {
                if (this.reportsRepository == null)
                    this.reportsRepository = new GenericRepository<EFStaticReports>(context);
                return this.reportsRepository;
            }
        }

        public GenericRepository<EFSMTPSettings> SMTPSettingsRepository
        {
            get
            {
                if (this.smtpSettingsRepository == null)
                    this.smtpSettingsRepository = new GenericRepository<EFSMTPSettings>(context);
                return this.smtpSettingsRepository;
            }
        }

        public GenericRepository<EFIATISettings> IATISettingsRepository
        {
            get
            {
                if (this.iatiSettingsRepository == null)
                    this.iatiSettingsRepository = new GenericRepository<EFIATISettings>(context);
                return this.iatiSettingsRepository;
            }
        }

        public GenericRepository<EFIATICountryCodes> IATICountryRepository
        {
            get
            {
                if (this.iatiCountryRepository == null)
                    this.iatiCountryRepository = new GenericRepository<EFIATICountryCodes>(context);
                return this.iatiCountryRepository;
            }
        }

        public GenericRepository<EFOrganization> OrganizationRepository
        {
            get
            {
                if (this.organizationRepository == null)
                    this.organizationRepository = new GenericRepository<EFOrganization>(context);
                return this.organizationRepository;
            }
        }

        public GenericRepository<EFIATIOrganization> IATIOrganizationRepository
        {
            get
            {
                if (this.iatiOrganizationRepository == null)
                    this.iatiOrganizationRepository = new GenericRepository<EFIATIOrganization>(context);
                return this.iatiOrganizationRepository;
            }
        }

        public GenericRepository<EFOrganizationTypes> OrganizationTypesRepository
        {
            get
            {
                if (this.organizationTypesRepository == null)
                    this.organizationTypesRepository = new GenericRepository<EFOrganizationTypes>(context);
                return this.organizationTypesRepository;
            }
        }

        public GenericRepository<EFSectorTypes> SectorTypesRepository
        {
            get
            {
                if (this.sectorTypesRepository == null)
                    this.sectorTypesRepository = new GenericRepository<EFSectorTypes>(context);
                return this.sectorTypesRepository;
            }
        }

        public GenericRepository<EFSectorMappings> SectorMappingsRepository
        {
            get
            {
                if (this.sectorMappingsRepository == null)
                    this.sectorMappingsRepository = new GenericRepository<EFSectorMappings>(context);
                return this.sectorMappingsRepository;
            }
        }

        /*public GenericRepository<EFSectorCategory> SectorCategoryRepository
        {
            get
            {
                if (this.sectorCategoryRepository == null)
                    this.sectorCategoryRepository = new GenericRepository<EFSectorCategory>(context);
                return this.sectorCategoryRepository;
            }
        }

        public GenericRepository<EFSectorSubCategory> SectorSubCategoryRepository
        {
            get
            {
                if (this.sectorSubCategoryRepository == null)
                    this.sectorSubCategoryRepository = new GenericRepository<EFSectorSubCategory>(context);
                return this.sectorSubCategoryRepository;
            }
        }*/

        public GenericRepository<EFSector> SectorRepository
        {
            get
            {
                if (this.sectorRepository == null)
                    this.sectorRepository = new GenericRepository<EFSector>(context);
                return this.sectorRepository;
            }
        }

        public GenericRepository<EFProject> ProjectRepository
        {
            get
            {
                if (this.projectRepository == null)
                    this.projectRepository = new GenericRepository<EFProject>(context);
                return this.projectRepository;
            }
        }

        public GenericRepository<EFProjectSectors> ProjectSectorsRepository
        {
            get
            {
                if (this.projectSectorsRepository == null)
                    this.projectSectorsRepository = new GenericRepository<EFProjectSectors>(context);
                return this.projectSectorsRepository;
            }
        }

        public GenericRepository<EFProjectLocations> ProjectLocationsRepository
        {
            get
            {
                if (this.projectLocationsRepository == null)
                    this.projectLocationsRepository = new GenericRepository<EFProjectLocations>(context);
                return this.projectLocationsRepository;
            }
        }

        public GenericRepository<EFProjectFunders> ProjectFundersRepository
        {
            get
            {
                if (this.fundersRepository == null)
                    this.fundersRepository = new GenericRepository<EFProjectFunders>(context);
                return this.fundersRepository;
            }
        }

        public GenericRepository<EFProjectImplementers> ProjectImplementersRepository
        {
            get
            {
                if (this.implementersRepository == null)
                    this.implementersRepository = new GenericRepository<EFProjectImplementers>(context);
                return this.implementersRepository;
            }
        }

        /*public GenericRepository<EFProjectFundings> ProjectFundsRepository
        {
            get
            {
                if (this.fundingsRepository == null)
                    this.fundingsRepository = new GenericRepository<EFProjectFundings>(context);
                return this.fundingsRepository;
            }
        }*/

        public GenericRepository<EFLocation> LocationRepository
        {
            get
            {
                if (this.locationRepository == null)
                    this.locationRepository = new GenericRepository<EFLocation>(context);
                return this.locationRepository;
            }
        }

        public GenericRepository<EFUser> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                    this.userRepository = new GenericRepository<EFUser>(context);
                return this.userRepository;
            }
        }

        public GenericRepository<EFPasswordRecoveryRequests> PasswordRecoveryRepository
        {
            get
            {
                if (this.passwordRecoveryRepository == null)
                    this.passwordRecoveryRepository = new GenericRepository<EFPasswordRecoveryRequests>(context);
                return this.passwordRecoveryRepository;
            }
        }

        public GenericRepository<EFProjectDocuments> ProjectDocumentRepository
        {
            get
            {
                if (this.projectDocumentsRepository == null)
                    this.projectDocumentsRepository = new GenericRepository<EFProjectDocuments>(context);
                return this.projectDocumentsRepository;
            }
        }

        public GenericRepository<EFProjectDisbursements> ProjectDisbursementsRepository
        {
            get
            {
                if (this.projectDisbursementsRepository == null)
                    this.projectDisbursementsRepository = new GenericRepository<EFProjectDisbursements>(context);
                return this.projectDisbursementsRepository;
            }
        }

        public GenericRepository<EFUserNotifications> NotificationsRepository
        {
            get
            {
                if (this.notificationsRepository == null)
                    this.notificationsRepository = new GenericRepository<EFUserNotifications>(context);
                return this.notificationsRepository;
            }
        }

        public GenericRepository<EFIATIData> IATIDataRepository
        {
            get
            {
                if (this.iatiDataRepository == null)
                    this.iatiDataRepository = new GenericRepository<EFIATIData>(context);
                return this.iatiDataRepository;
            }
        }

        public GenericRepository<EFExchangeRates> ExchangeRatesRepository
        {
            get
            {
                if (this.exchangeRatesRepository == null)
                    this.exchangeRatesRepository = new GenericRepository<EFExchangeRates>(context);
                return this.exchangeRatesRepository;
            }
        }

        public GenericRepository<EFExchangeRatesSettings> ExRatesSettingsRepository
        {
            get
            {
                if (this.exRatesSettingsRepository == null)
                    this.exRatesSettingsRepository = new GenericRepository<EFExchangeRatesSettings>(context);
                return this.exRatesSettingsRepository;
            }
        }

        public GenericRepository<EFExchangeRatesUsageSettings> ExRatesUsageRepository
        {
            get
            {
                if (this.exRatesUsageSettingsRepository == null)
                    this.exRatesUsageSettingsRepository = new GenericRepository<EFExchangeRatesUsageSettings>(context);
                return this.exRatesUsageSettingsRepository;
            }
        }

        public GenericRepository<EFExchangeRatesAPIsCount> ExchangeRatesAPIsRepository
        {
            get
            {
                if (this.exRatesAPIsRepository == null)
                    this.exRatesAPIsRepository = new GenericRepository<EFExchangeRatesAPIsCount>(context);
                return this.exRatesAPIsRepository;
            }
        }

        /// <summary>
        /// Save method.
        /// </summary>
        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                List<string> lines = new List<string>();
                foreach (var error in e.Data)
                {
                    lines.Add(error.ToString());
                }
                //System.IO.File.AppendAllLines(@"E:\errors.txt", lines);
                throw e;
            }
        }

        /// <summary>
        /// Save Async
        /// </summary>
        public async Task<int> SaveAsync()
        {
            try
            {
                return await Task<int>.Run(() => context.SaveChangesAsync());
            }
            catch (Exception e)
            {
                List<string> lines = new List<string>();
                foreach (var error in e.Data)
                {
                    lines.Add(error.ToString());
                }
                System.IO.File.AppendAllLines(@"E:\errors.txt", lines);
                throw e;
            }
        }

        private bool disposed = false;
        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("UnitOfWork is being disposed");
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
