using AIMS.Models;
using Microsoft.EntityFrameworkCore;
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
        public AIMSDbContext()
        {
        }

        public AIMSDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Composite keys and Unique index configuration while creating the model
            modelBuilder.Entity<EFProjectFunders>()
                .HasKey(f => new { f.ProjectId, f.FunderId });

            modelBuilder.Entity<EFProjectImplementors>()
                .HasKey(i => new { i.ProjectId, i.ImplementorId });

            modelBuilder.Entity<EFProjectDisbursements>()
                .HasKey(d => new { d.ProjectId, d.StartingYear });

            modelBuilder.Entity<EFProjectSectors>()
                .HasKey(s => new { s.ProjectId, s.SectorId });

            modelBuilder.Entity<EFProjectLocations>()
                .HasKey(l => new { l.ProjectId, l.LocationId });

            /*modelBuilder.Entity<EFProjectFundings>()
                .HasKey(fu => new { fu.ProjectId, fu.FunderId });*/

            modelBuilder.Entity<EFReportSubscriptions>()
                .HasKey(r => new { r.UserId, r.ReportId });

            modelBuilder.Entity<EFProjectCustomFields>()
                .HasKey(c => new { c.ProjectId, c.CustomFieldId });

            modelBuilder.Entity<EFUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        //Creating DB Tables for the Objects
        public DbSet<EFOrganizationTypes> OrganizationTypes { get; set; }
        public DbSet<EFProjectTypes> ProjectTypes { get; set; }
        public DbSet<EFOrganization> Organizations { get; set; }
        public DbSet<EFUser> Users { get; set; }
        public DbSet<EFSectorTypes> SectorTypes { get; set; }
        public DbSet<EFSectorCategory> SectorCategories { get; set; }
        public DbSet<EFSectorSubCategory> SectorSubCategories { get; set; }
        public DbSet<EFSector> Sectors { get; set; }
        public DbSet<EFLocation> Locations { get; set; }
        public DbSet<EFCustomFields> CustomFields { get; set; }
        public DbSet<EFProject> Projects { get; set; }
        public DbSet<EFProjectFunders> ProjectFunders { get; set; }
        public DbSet<EFProjectImplementors> ProjectImplementors { get; set; }
        public DbSet<EFProjectFundings> ProjectFundings { get; set; }
        public DbSet<EFProjectLocations> ProjectLocations { get; set; }
        public DbSet<EFProjectSectors> ProjectSectors { get; set; }
        public DbSet<EFProjectDisbursements> ProjectDisbursements { get; set; }
        public DbSet<EFProjectDocuments> ProjectDocuments { get; set; }
        public DbSet<EFProjectMarkers> ProjectMarkers { get; set; }
        public DbSet<EFProjectCustomFields> ProjectCustomFields { get; set; }
        public DbSet<EFReportSubscriptions> ReportSubscriptions { get; set; }
        public DbSet<EFUserNotifications> UserNotifications { get; set; }
        public DbSet<EFLogs> Logs { get; set; }
        public DbSet<EFStaticReports> StaticReports { get; set; }

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
