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

            modelBuilder.Entity<EFProjectImplementers>()
                .HasKey(i => new { i.ProjectId, i.ImplementerId });

            modelBuilder.Entity<EFUserSubscriptions>()
                .HasKey(s => new { s.UserId, s.ReportId });

            modelBuilder.Entity<EFUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        //Creating DB Tables for the Objects
        public DbSet<EFTimeIntervals> TimeIntervals { get; set; }
        public DbSet<EFOrganization> Organizations { get; set; }
        public DbSet<EFUser> Users { get; set; }
        public DbSet<EFSector> Sectors { get; set; }
        public DbSet<EFLocation> Locations { get; set; }
        public DbSet<EFProject> Projects { get; set; }
        public DbSet<EFProjectFunders> ProjectFunders { get; set; }
        public DbSet<EFProjectImplementers> ProjectImplementers { get; set; }
        public DbSet<EFProjectFundings> ProjectFundings { get; set; }
        public DbSet<EFUserSubscriptions> UserSubscriptions { get; set; }
        public DbSet<EFUserNotifications> UserNotifications { get; set; }
        public DbSet<EFProjectLogs> ProjectLogs { get; set; }
        public DbSet<EFProjectCustomFields> ProjectCustomFields { get; set; }
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
