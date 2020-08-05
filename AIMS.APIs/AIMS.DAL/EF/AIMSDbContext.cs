using AIMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.DAL.EF
{
    public class AIMSDbContext : DbContext
    {
        public string ConnectionString { get; set; }
        public AIMSDbContext()
        {
            Database.SetCommandTimeout(60);
        }

        public AIMSDbContext(DbContextOptions options) : base(options)
        {
            var sqlServerOptionsExtension =
                   options.FindExtension<SqlServerOptionsExtension>();
            if (sqlServerOptionsExtension != null)
            {
                ConnectionString = sqlServerOptionsExtension.ConnectionString;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Composite keys and Unique index configuration while creating the model
            modelBuilder.Entity<EFProjectFunders>()
                .HasKey(f => new { f.ProjectId, f.FunderId });

            modelBuilder.Entity<EFSector>()
                .HasOne(s => s.ParentSector);

            modelBuilder.Entity<EFProjectImplementers>()
                .HasKey(i => new { i.ProjectId, i.ImplementerId });

            modelBuilder.Entity<EFProjectSectors>()
                .HasKey(s => new { s.ProjectId, s.SectorId });

            modelBuilder.Entity<EFProjectLocations>()
                .HasKey(l => new { l.ProjectId, l.LocationId });

            modelBuilder.Entity<EFFinancialYears>()
                .HasIndex(f => f.FinancialYear)
                .IsUnique();

            modelBuilder.Entity<EFReportSubscriptions>()
                .HasKey(r => new { r.UserId, r.ReportId });

            modelBuilder.Entity<EFProjectMarkers>()
                .HasKey(m => new { m.ProjectId, m.MarkerId });

            modelBuilder.Entity<EFSectorMappings>()
                .HasKey(m => new { m.SectorId, m.MappedSectorId });

            modelBuilder.Entity<EFUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<EFCurrency>()
                .HasIndex(c => c.Currency)
                .IsUnique();

            modelBuilder.Entity<EFSectorMappings>()
                .HasKey(m => new { m.SectorId, m.MappedSectorId });

            modelBuilder.Entity<EFProjectMembershipRequests>()
                .HasKey(m => new { m.ProjectId, m.UserId, m.MembershipType });

            modelBuilder.Entity<EFOrganizationsToMerge>()
                .HasKey(o => new { o.RequestId, o.OrganizationId });

            modelBuilder.Entity<EFProjectDeletionRequests>()
                .HasKey(d => new { d.ProjectId, d.RequestedById });

            modelBuilder.Entity<EFExchangeRatesUsageSettings>()
                .HasIndex(e => new { e.Source, e.UsageSection })
                .IsUnique();

            modelBuilder.Entity<EFEnvelopeYearlyBreakup>()
                .HasKey(e => new { e.EnvelopeTypeId, e.EnvelopeId, e.YearId });

            modelBuilder.Entity<EFProjectDisbursements>()
                .HasIndex(d => new { d.ProjectId, d.YearId, d.DisbursementType })
                .IsUnique();
        }

        //Creating DB Tables for the Objects
        public DbSet<EFFundingTypes> FundingTypes { get; set; }
        public DbSet<EFOrganizationTypes> OrganizationTypes { get; set; }
        public DbSet<EFOrganization> Organizations { get; set; }
        public DbSet<EFIATIOrganization> IATIOrganizations { get; set; }
        public DbSet<EFIATICountryCodes> IATICountryCodes { get; set; }
        public DbSet<EFUser> Users { get; set; }
        public DbSet<EFSectorTypes> SectorTypes { get; set; }
        public DbSet<EFSectorMappings> SectorMappings { get; set; }
        public DbSet<EFSector> Sectors { get; set; }
        public DbSet<EFLocation> Locations { get; set; }
        public DbSet<EFMarkers> Markers { get; set; }
        public DbSet<EFProject> Projects { get; set; }
        public DbSet<EFProjectFunders> ProjectFunders { get; set; }
        public DbSet<EFProjectImplementers> ProjectImplementers { get; set; }
        public DbSet<EFProjectLocations> ProjectLocations { get; set; }
        public DbSet<EFProjectSectors> ProjectSectors { get; set; }
        public DbSet<EFProjectDisbursements> ProjectDisbursements { get; set; }
        public DbSet<EFProjectDocuments> ProjectDocuments { get; set; }
        public DbSet<EFProjectMarkers> ProjectMarkers { get; set; }
        public DbSet<EFReportSubscriptions> ReportSubscriptions { get; set; }
        public DbSet<EFUserNotifications> UserNotifications { get; set; }
        public DbSet<EFLogs> Logs { get; set; }
        public DbSet<EFStaticReports> StaticReports { get; set; }
        public DbSet<EFIATIData> IATIData { get; set; }
        public DbSet<EFSMTPSettings> SMTPSettings { get; set; }
        public DbSet<EFIATISettings> IATISettings { get; set; }
        public DbSet<EFPasswordRecoveryRequests> PasswordRecoveryRequests { get; set; }
        public DbSet<EFFinancialYearSettings> FinancialYearSettings { get; set; }
        public DbSet<EFFinancialYears> FinancialYears { get; set; }
        public DbSet<EFCurrency> Currencies { get; set; }
        public DbSet<EFExchangeRates> ExchangeRates { get; set; }
        public DbSet<EFManualExchangeRates> ManualExchangeRates { get; set; }
        public DbSet<EFExchangeRatesSettings> ExchangeRatesSettings { get; set; }
        public DbSet<EFExchangeRatesAPIsCount> ExchangeRatesAPIsCount { get; set; }
        public DbSet<EFEnvelopeTypes> EnvelopeTypes { get; set; }
        public DbSet<EFEnvelope> Envelope { get; set; }
        public DbSet<EFEnvelopeYearlyBreakup> EnvelopeYearlyBreakups { get; set; }
        public DbSet<EFEmailMessages> EmailMessages { get; set; }
        public DbSet<EFProjectMembershipRequests> ProjectMembershipRequests { get; set; }
        public DbSet<EFProjectDeletionRequests> ProjectDeletionRequests { get; set; }
        public DbSet<EFExchangeRatesUsageSettings> ExchangeRatesUsageSettings { get; set; }
        public DbSet<EFHelp> Help { get; set; }
        public DbSet<EFHomePageSettings> HomePageSettings { get; set; }
        public DbSet<EFDropboxSettings> DropboxSettings { get; set; }
        public DbSet<EFOrganizationMergeRequests> OrganizationMergeRequests { get; set; }
        public DbSet<EFOrganizationsToMerge> OrganizationsToMerge { get; set; }
        public DbSet<EFUserRoleSettlementRequests> UserRoleSettlementRequests { get; set; }
        public DbSet<EFFinancialYearTransition> FinancialTransitions { get; set; }

        //Overridden SaveChanges to catch full exception details about
        //EntityValidation Exceptions instead of attaching debugger everytime
        public override int SaveChanges()
        {
            try
            {
                var entities = from e in ChangeTracker.Entries()
                               where e.State == EntityState.Added
                                   || e.State == EntityState.Modified
                               select e.Entity;
                foreach (var entity in entities)
                {
                    var validationContext = new ValidationContext(entity);
                    Validator.ValidateObject(entity, validationContext);
                }

                return base.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException(ex.Message, ex);
            }
        }

        private static readonly Dictionary<int, string> _sqlErrorTextDict = new Dictionary<int, string>
        {
                {547, "This operation failed because another data entry uses this entry."},
                {2601,"One of the properties is marked as Unique index and there is already an entry with that value."}
        };
    }
}
