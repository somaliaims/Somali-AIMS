using AIMS.DAL.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.DAL.Repository
{
    /// <summary>
    /// Generic Repository class for Entity Operations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal AIMSDbContext dbContext;
        internal DbSet<TEntity> DbSet;

        /// <summary>
        /// Public Constructor,initializes privately declared local variables.
        /// </summary>
        /// <param name="context"></param>
        public GenericRepository(AIMSDbContext context)
        {
            this.dbContext = context;
            this.DbSet = dbContext.Set<TEntity>();
        }

        /// <summary>
        /// generic Get method for Entities
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get()
        {
            IQueryable<TEntity> query = DbSet;
            return query.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync()
        {
            IQueryable<TEntity> query = DbSet;
            return await Task<IEnumerable<TEntity>>.Run(() => query.ToList()).ConfigureAwait(false);
        }

        /// <summary>
        /// Generic get method on the basis of id for Entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetByID(object id)
        {
            return DbSet.Find(id);
        }

        public virtual async Task<TEntity> GetByIDAsync(object id)
        {
            return await Task<TEntity>.Run(() => DbSet.Find(id)).ConfigureAwait(false);
        }

        /// <summary>
        /// generic Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public virtual TEntity Insert(TEntity entity)
        {
            DbSet.Add(entity);
            return entity;
        }

        /// <summary>
        /// Inserts multiple instances of an entity
        /// </summary>
        /// <param name="entityList"></param>
        public virtual List<TEntity> InsertMultiple(List<TEntity> entityList)
        {
            List<TEntity> dbEntities = new List<TEntity>();
            foreach (var entity in entityList)
            {
                var dbEntity = DbSet.Add(entity);
                dbEntities.Add(entity);
            }
            return dbEntities;
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Generic update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public virtual void Update(TEntity entityToUpdate)
        {
            DbSet.Attach(entityToUpdate);
            dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetMany(Func<TEntity, bool> where)
        {
            return DbSet.Where(where).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetManyAsync(Func<TEntity, bool> where)
        {
            return await Task<IEnumerable<TEntity>>.Run(() => DbSet.Where(where).ToList()).ConfigureAwait(false);
        }

        public virtual IEnumerable<TType> GetProjection<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select)
        {
            return DbSet.Where(where).Select(select).ToList();
        }

        public virtual int GetProjectionCount<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select)
        {
            return DbSet.Where(where).Select(select).AsQueryable().Count();
        }

        /// <summary>
        /// generic method to get a single entity on the basis of a condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual TEntity GetOne(Func<TEntity, bool> where)
        {
            return DbSet.Where(where).FirstOrDefault();
        }

        public virtual TEntity GetOneOrderByDescending(Func<TEntity, bool> orderBy)
        {
            return DbSet.OrderByDescending(orderBy).FirstOrDefault();
        }

        public virtual TEntity GetOneOrderByAscending(Func<TEntity, bool> orderBy)
        {
            return DbSet.OrderBy(orderBy).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetOneAsync(Func<TEntity, bool> where)
        {
            return await Task<TEntity>.Run(() => DbSet.Where(where).FirstOrDefault()).ConfigureAwait(false);
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetManyQueryable(Func<TEntity, bool> where)
        {
            return DbSet.Where(where).AsQueryable();
        }

        public virtual IQueryable<TEntity> GetManyQueryableOrderBy(Func<TEntity, bool> where, Func<TEntity, bool> orderBy)
        {
            return DbSet.Where(where).OrderBy(orderBy).AsQueryable();
        }

        public virtual async Task<IQueryable<TEntity>> GetManyQueryableAsync(Func<TEntity, bool> where)
        {
            return await Task<IQueryable<TEntity>>.Run(() => DbSet.Where(where).AsQueryable()).ConfigureAwait(false);
        }

        public virtual async Task<IQueryable<TEntity>> GetManyQueryableOrderByAsync(Func<TEntity, bool> where, Func<TEntity, bool> orderBy)
        {
            return await Task<IQueryable<TEntity>>.Run(() => DbSet.Where(where).OrderBy(orderBy).AsQueryable()).ConfigureAwait(false);
        }

        /// <summary>
        /// generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public TEntity Get(Func<TEntity, Boolean> where)
        {
            if (DbSet.Any())
            {
                return DbSet.Where(where).FirstOrDefault<TEntity>();
            }
            else
            {
                return null;
            }
            
        }

        public async Task<TEntity> GetAsync(Func<TEntity, Boolean> where)
        {
            return await Task<TEntity>.Run(() => DbSet.Where(where).FirstOrDefault<TEntity>()).ConfigureAwait(false);
        }

        /// <summary>
        /// generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Delete(Func<TEntity, Boolean> where)
        {
            IQueryable<TEntity> objects = DbSet.Where<TEntity>(where).AsQueryable();
            foreach (TEntity obj in objects)
                DbSet.Remove(obj);
        }

        /// <summary>
        /// generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        /// <summary>
        /// Async for getall
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Task<IEnumerable<TEntity>>.Run(() => DbSet.ToListAsync()).ConfigureAwait(false);
        }
        /// <summary>
        /// Inclue multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetWithInclude(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            IQueryable<TEntity> query = this.DbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate).AsQueryable();
        }

        public IQueryable<TEntity> GetWithIncludeOrderBy(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, Func<TEntity, string> orderBy, params string[] include)
        {
            IQueryable<TEntity> query = this.DbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate).OrderBy(orderBy).AsQueryable();
        }

        /// <summary>
        /// Async for Include multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public async Task<IQueryable<TEntity>> GetWithIncludeAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            IQueryable<TEntity> query = this.DbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return await Task<IQueryable<TEntity>>.Run(() => query.Where(predicate).AsQueryable()).ConfigureAwait(false);
        }

        /// <summary>
        /// Generic method to check if entity exists
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public bool Exists(object primaryKey)
        {
            return DbSet.Find(primaryKey) != null;
        }

        public async Task<bool> ExistsAsync(object primaryKey)
        {
            return await Task<bool>.Run(() => DbSet.Find(primaryKey) != null).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public TEntity GetSingle(Func<TEntity, bool> predicate)
        {
            return DbSet.Single<TEntity>(predicate);
        }

        public async Task<TEntity> GetSingleAsync(Func<TEntity, bool> predicate)
        {
            return await Task<TEntity>.Run(() => DbSet.Single<TEntity>(predicate));
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public TEntity GetFirst(Func<TEntity, bool> predicate)
        {
            return DbSet.First<TEntity>(predicate);
        }

        public async Task<TEntity> GetFirstAsync(Func<TEntity, bool> predicate)
        {
            return await Task<TEntity>.Run(() => DbSet.First<TEntity>(predicate)).ConfigureAwait(false);
        }
    }
}
