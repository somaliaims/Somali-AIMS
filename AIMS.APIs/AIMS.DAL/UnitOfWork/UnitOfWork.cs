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
        private GenericRepository<EFSector> sectorRepository;
        private GenericRepository<EFLocation> locationRepository;
        private GenericRepository<EFOrganization> organizationRepository;
        private GenericRepository<EFOrganizationTypes> organizationTypesRepository;
        private GenericRepository<EFProject> projectRepository;
        private GenericRepository<EFProjectSectors> projectSectorsRepository;
        private GenericRepository<EFProjectFunders> fundersRepository;
        private GenericRepository<EFProjectImplementers> implementersRepository;
        private GenericRepository<EFProjectFundings> fundingsRepository;
        private GenericRepository<EFUser> userRepository;

        public UnitOfWork(AIMSDbContext dbContext)
        {
            context = dbContext;
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

        public GenericRepository<EFOrganizationTypes> OrganizationTypesRepository
        {
            get
            {
                if (this.organizationTypesRepository == null)
                    this.organizationTypesRepository = new GenericRepository<EFOrganizationTypes>(context);
                return this.organizationTypesRepository;
            }
        }

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

        public GenericRepository<EFProjectFundings> ProjectFundingsRepository
        {
            get
            {
                if (this.fundingsRepository == null)
                    this.fundingsRepository = new GenericRepository<EFProjectFundings>(context);
                return this.fundingsRepository;
            }
        }

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
                System.IO.File.AppendAllLines(@"E:\errors.txt", lines);
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
