using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AIMS.Services
{
    public interface ISubLocationService
    {
        /// <summary>
        /// Gets list of all sub locations
        /// </summary>
        /// <returns></returns>
        IEnumerable<SubLocationView> GetAll();

        /// <summary>
        /// Get sub locations for the provided location
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<SubLocationView> GetForLocation(int id);

        /// <summary>
        /// Get sub location for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SubLocationView Get(int id);

        /// <summary>
        /// Adds new sub location
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Add(SubLocationModel model);

        /// <summary>
        /// Updates the sub location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse Update(int id, SubLocationModel model);

        /// <summary>
        /// Deletes the provided sub location
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }
    public class SubLocationService : ISubLocationService
    {
        AIMSDbContext context;
        IMapper autoMapper;

        public SubLocationService(AIMSDbContext cntxt, IMapper mapper)
        {
            context = cntxt;
            autoMapper = mapper;
        }

        public IEnumerable<SubLocationView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                return autoMapper.Map<List<SubLocationView>>(unitWork.SubLocationRepository.GetManyQueryable(s => s.Id != 0));
            }
        }

        public IEnumerable<SubLocationView> GetForLocation(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                return autoMapper.Map<List<SubLocationView>>(unitWork.SubLocationRepository.GetManyQueryable(s => s.LocationId == id));
            }
        }

        public SubLocationView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                return autoMapper.Map<SubLocationView>(unitWork.SubLocationRepository.GetOne(s => s.Id == id));
            }
        }

        public ActionResponse Add(SubLocationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                try
                {
                    var location = unitWork.LocationRepository.GetByID(model.LocationId);
                    if (location == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Location");
                        response.Success = false;
                        return response;
                    }

                    var subLocation = unitWork.SubLocationRepository.Insert(new EFSubLocation()
                    {
                        Location = location,
                        SubLocation = model.SubLocation
                    });
                    unitWork.Save();
                    response.ReturnedId = subLocation.Id;
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        response.Message = ex.InnerException.Message;
                    }
                }
                return response;
            }
        }

        public ActionResponse Update(int id, SubLocationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                try
                {
                    var subLocation = unitWork.SubLocationRepository.GetByID(id);
                    if (subLocation == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Sub-Location");
                        response.Success = false;
                        return response;
                    }

                    var location = unitWork.LocationRepository.GetByID(model.LocationId);
                    if (location == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Location");
                        response.Success = false;
                        return response;
                    }

                    subLocation.Location = location;
                    subLocation.SubLocation = model.SubLocation;
                    unitWork.SubLocationRepository.Update(subLocation);
                    unitWork.Save();
                }
                catch(Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                    if (ex.InnerException != null)
                    {
                        response.Message = ex.InnerException.Message;
                    }
                }
                return response;
            }
        }

        public ActionResponse Delete(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                try
                {
                    var subLocation = unitWork.SubLocationRepository.GetByID(id);
                    if (subLocation == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Sub-Location");
                        response.Success = false;
                        return response;
                    }

                    var isSubLocationReferenced = unitWork.ProjectLocationsRepository.GetManyQueryable(l => (l.SubLocationIds != null && (l.SubLocationIds.Split("-")).Contains(id.ToString())));
                    if (isSubLocationReferenced.Any())
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetDependentProjectsForSublocationMessage();
                        response.Success = false;
                        return response;
                    }
                    unitWork.SubLocationRepository.Delete(subLocation);
                    unitWork.Save();
                    response.ReturnedId = id;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                    if (ex.InnerException != null)
                    {
                        response.Message = ex.InnerException.Message;
                    }
                }
                return response;
            }
        }
    }
}
