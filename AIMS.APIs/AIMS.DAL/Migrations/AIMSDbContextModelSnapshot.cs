﻿// <auto-generated />
using System;
using AIMS.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AIMS.DAL.Migrations
{
    [DbContext(typeof(AIMSDbContext))]
    partial class AIMSDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AIMS.Models.EFCurrency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Currency");

                    b.Property<bool>("IsDefault");

                    b.HasKey("Id");

                    b.HasIndex("Currency")
                        .IsUnique()
                        .HasFilter("[Currency] IS NOT NULL");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("AIMS.Models.EFCustomFields", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ActiveFrom");

                    b.Property<DateTime>("ActiveUpto");

                    b.Property<string>("FieldTitle");

                    b.Property<int>("FieldType");

                    b.HasKey("Id");

                    b.ToTable("CustomFields");
                });

            modelBuilder.Entity("AIMS.Models.EFExchangeRates", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Dated");

                    b.Property<string>("DefaultCurrency");

                    b.Property<string>("ExchangeRatesJson");

                    b.HasKey("Id");

                    b.ToTable("ExchangeRates");
                });

            modelBuilder.Entity("AIMS.Models.EFExchangeRatesAPIsCount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count");

                    b.Property<DateTime>("Dated");

                    b.HasKey("Id");

                    b.ToTable("ExchangeRatesAPIsCount");
                });

            modelBuilder.Entity("AIMS.Models.EFExchangeRatesSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("APIKeyOpenExchangeRates");

                    b.Property<bool>("IsAutomatic");

                    b.Property<string>("ManualExchangeRates");

                    b.HasKey("Id");

                    b.ToTable("ExchangeRatesSettings");
                });

            modelBuilder.Entity("AIMS.Models.EFFinancialYears", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FinancialYear");

                    b.HasKey("Id");

                    b.HasIndex("FinancialYear")
                        .IsUnique();

                    b.ToTable("FinancialYears");
                });

            modelBuilder.Entity("AIMS.Models.EFIATIData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Data");

                    b.Property<DateTime>("Dated");

                    b.Property<string>("Organizations");

                    b.HasKey("Id");

                    b.ToTable("IATIData");
                });

            modelBuilder.Entity("AIMS.Models.EFIATISettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BaseUrl");

                    b.HasKey("Id");

                    b.ToTable("IATISettings");
                });

            modelBuilder.Entity("AIMS.Models.EFLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal?>("Latitude")
                        .HasColumnType("decimal(9, 5)");

                    b.Property<string>("Location");

                    b.Property<decimal?>("Longitude")
                        .HasColumnType("decimal(9, 5)");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("AIMS.Models.EFLogs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActionPerformed");

                    b.Property<DateTime>("Dated");

                    b.Property<string>("NewValues");

                    b.Property<string>("OldValues");

                    b.Property<int>("ProjectId");

                    b.Property<int?>("UpdatedById");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("AIMS.Models.EFOrganization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsApproved");

                    b.Property<string>("OrganizationName");

                    b.Property<DateTime>("RegisteredOn");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("AIMS.Models.EFOrganizationTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TypeName");

                    b.HasKey("Id");

                    b.ToTable("OrganizationTypes");
                });

            modelBuilder.Entity("AIMS.Models.EFPasswordRecoveryRequests", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Dated");

                    b.Property<string>("Email");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("PasswordRecoveryRequests");
                });

            modelBuilder.Entity("AIMS.Models.EFProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDate");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Title")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectCustomFields", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("CustomFieldId");

                    b.Property<string>("Value");

                    b.HasKey("ProjectId", "CustomFieldId");

                    b.HasIndex("CustomFieldId");

                    b.ToTable("ProjectCustomFields");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDisbursements", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<DateTime>("Dated");

                    b.Property<int>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectDisbursements");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDocuments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DocumentTitle");

                    b.Property<string>("DocumentUrl");

                    b.Property<int>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectDocuments");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFunders", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("FunderId");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(9 ,2)");

                    b.Property<string>("Currency");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(9, 2)");

                    b.HasKey("ProjectId", "FunderId");

                    b.HasIndex("FunderId");

                    b.ToTable("ProjectFunders");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectImplementers", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("ImplementerId");

                    b.HasKey("ProjectId", "ImplementerId");

                    b.HasIndex("ImplementerId");

                    b.ToTable("ProjectImplementers");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectLocations", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("LocationId");

                    b.Property<decimal>("FundsPercentage")
                        .HasColumnType("decimal(9, 2)");

                    b.HasKey("ProjectId", "LocationId");

                    b.HasIndex("LocationId");

                    b.ToTable("ProjectLocations");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectMarkers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Marker");

                    b.Property<decimal>("Percentage")
                        .HasColumnType("decimal(9, 2)");

                    b.Property<int>("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectMarkers");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectSectors", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<int>("SectorId");

                    b.Property<decimal>("FundsPercentage")
                        .HasColumnType("decimal(9, 2)");

                    b.HasKey("ProjectId", "SectorId");

                    b.HasIndex("SectorId");

                    b.ToTable("ProjectSectors");
                });

            modelBuilder.Entity("AIMS.Models.EFReportSubscriptions", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("ReportId");

                    b.HasKey("UserId", "ReportId");

                    b.HasIndex("ReportId");

                    b.ToTable("ReportSubscriptions");
                });

            modelBuilder.Entity("AIMS.Models.EFSector", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ParentSectorId");

                    b.Property<string>("SectorName");

                    b.Property<int>("SectorTypeId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("Id");

                    b.HasIndex("ParentSectorId");

                    b.HasIndex("SectorTypeId");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorMappings", b =>
                {
                    b.Property<int>("SectorId");

                    b.Property<int>("MappedSectorId");

                    b.Property<int>("SectorTypeId");

                    b.HasKey("SectorId", "MappedSectorId");

                    b.ToTable("SectorMappings");
                });

            modelBuilder.Entity("AIMS.Models.EFSectorTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("IsDefault");

                    b.Property<bool?>("IsIATIType");

                    b.Property<string>("TypeName");

                    b.HasKey("Id");

                    b.ToTable("SectorTypes");
                });

            modelBuilder.Entity("AIMS.Models.EFSMTPSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdminEmail");

                    b.Property<string>("Host");

                    b.Property<string>("Password");

                    b.Property<int>("Port");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("SMTPSettings");
                });

            modelBuilder.Entity("AIMS.Models.EFStaticReports", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("StaticReports");
                });

            modelBuilder.Entity("AIMS.Models.EFUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ApprovedById");

                    b.Property<DateTime?>("ApprovedOn");

                    b.Property<string>("Email");

                    b.Property<bool>("IsApproved");

                    b.Property<DateTime?>("LastLogin");

                    b.Property<int>("OrganizationId");

                    b.Property<string>("Password");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<int>("UserType");

                    b.HasKey("Id");

                    b.HasIndex("ApprovedById");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AIMS.Models.EFUserNotifications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Dated");

                    b.Property<bool>("IsSeen");

                    b.Property<string>("Message");

                    b.Property<int>("NotificationType");

                    b.Property<int>("OrganizationId");

                    b.Property<int>("TreatmentId");

                    b.Property<int>("UserType");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("UserNotifications");
                });

            modelBuilder.Entity("AIMS.Models.EFLogs", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFUser", "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById");
                });

            modelBuilder.Entity("AIMS.Models.EFProjectCustomFields", b =>
                {
                    b.HasOne("AIMS.Models.EFCustomFields", "CustomField")
                        .WithMany("ProjectFieldsList")
                        .HasForeignKey("CustomFieldId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDisbursements", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Disbursements")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectDocuments", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Documents")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectFunders", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Funder")
                        .WithMany()
                        .HasForeignKey("FunderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Funders")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectImplementers", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Implementer")
                        .WithMany()
                        .HasForeignKey("ImplementerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Implementers")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectLocations", b =>
                {
                    b.HasOne("AIMS.Models.EFLocation", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Locations")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectMarkers", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFProjectSectors", b =>
                {
                    b.HasOne("AIMS.Models.EFProject", "Project")
                        .WithMany("Sectors")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFSector", "Sector")
                        .WithMany()
                        .HasForeignKey("SectorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFReportSubscriptions", b =>
                {
                    b.HasOne("AIMS.Models.EFStaticReports", "Report")
                        .WithMany()
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AIMS.Models.EFUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFSector", b =>
                {
                    b.HasOne("AIMS.Models.EFSector", "ParentSector")
                        .WithMany()
                        .HasForeignKey("ParentSectorId");

                    b.HasOne("AIMS.Models.EFSectorTypes", "SectorType")
                        .WithMany("Sectors")
                        .HasForeignKey("SectorTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFUser", b =>
                {
                    b.HasOne("AIMS.Models.EFUser", "ApprovedBy")
                        .WithMany()
                        .HasForeignKey("ApprovedById");

                    b.HasOne("AIMS.Models.EFOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AIMS.Models.EFUserNotifications", b =>
                {
                    b.HasOne("AIMS.Models.EFOrganization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
