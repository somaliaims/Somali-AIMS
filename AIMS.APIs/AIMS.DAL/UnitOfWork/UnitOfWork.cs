using AIMS.DAL.EF;
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

        public UnitOfWork(AIMSDbContext dbContext)
        {
            context = dbContext;
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
