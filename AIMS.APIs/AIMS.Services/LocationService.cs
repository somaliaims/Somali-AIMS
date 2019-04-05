using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface ILocationService
    {
        /// <summary>
        /// Gets all locations
        /// </summary>
        /// <returns></returns>
        IEnumerable<LocationView> GetAll();

        /// <summary>
        /// Get matching locations for the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<LocationView> GetMatching(string criteria);

        /// <summary>
        /// Gets the location for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        LocationView Get(int id);
        /// <summary>
        /// Gets all locations async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<LocationView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(LocationModel location);

        /// <summary>
        /// Updates a location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        ActionResponse Update(int id, LocationModel location);

        /// <summary>
        /// Deletes a location
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id, int newId);
    }

    public class LocationService : ILocationService
    {
        AIMSDbContext context;
        IMapper mapper;

        public LocationService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<LocationView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var locations = unitWork.LocationRepository.GetAll();
                return mapper.Map<List<LocationView>>(locations);
            }
        }

        public async Task<IEnumerable<LocationView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var locations = await unitWork.LocationRepository.GetAllAsync();
                return await Task<IEnumerable<LocationView>>.Run(() => mapper.Map<List<LocationView>>(locations)).ConfigureAwait(false);
            }
        }

        public LocationView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var location = unitWork.LocationRepository.GetByID(id);
                return mapper.Map<LocationView>(location);
            }
        }

        public IEnumerable<LocationView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<LocationView> locationsList = new List<LocationView>();
                var locations = unitWork.LocationRepository.GetMany(o => o.Location.Contains(criteria));
                return mapper.Map<List<LocationView>>(locations);
            }
        }

        public ActionResponse Add(LocationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isLocationCreated = unitWork.LocationRepository.GetOne(l => l.Location.ToLower() == model.Location.ToLower());
                    if (isLocationCreated != null)
                    {
                        isLocationCreated.Latitude = model.Latitude;
                        isLocationCreated.Longitude = model.Longitude;
                        unitWork.LocationRepository.Update(isLocationCreated);

                        response.ReturnedId = isLocationCreated.Id;
                    }
                    else
                    {
                        var newLocation = unitWork.LocationRepository.Insert(new EFLocation()
                        {
                            Location = model.Location,
                            Latitude = model.Latitude,
                            Longitude = model.Longitude
                        });
                        unitWork.Save();
                        response.ReturnedId = newLocation.Id;
                    }
                    
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, LocationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var locationObj = unitWork.LocationRepository.GetByID(id);
                if (locationObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Location");
                    return response;
                }

                locationObj.Location = model.Location;
                locationObj.Latitude = model.Latitude;
                locationObj.Longitude = model.Longitude;

                unitWork.LocationRepository.Update(locationObj);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }

        public async Task<ActionResponse> DeleteAsync(int id, int newId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper = new MessageHelper();
                ActionResponse response = new ActionResponse();
                var locations = await unitWork.LocationRepository.GetManyQueryableAsync(l => (l.Id == id || l.Id == newId));
                if (locations.Count() < 2 && newId != 0)
                {
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Location");
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                }

                var projectLocations = await unitWork.ProjectLocationsRepository.GetManyQueryableAsync(l => (l.LocationId == id || l.LocationId == newId));
                var locationsInDb = (from l in projectLocations
                                     select new LocationKeyPreview
                                     {
                                         ProjectId = l.ProjectId,
                                         LocationId = l.LocationId
                                     }).ToList<LocationKeyPreview>();

                var location = (from l in locations
                                where l.Id == id
                                select l).FirstOrDefault();

                List<EFProjectLocations> locationsList = new List<EFProjectLocations>();
                    foreach(var pLocation in projectLocations)
                    {
                        var isLocationExists = (from l in locationsList
                                                where l.ProjectId == pLocation.ProjectId && l.LocationId == newId
                                                select l).FirstOrDefault();

                        var isLocationInDB = (from l in locationsInDb
                                              where l.LocationId == newId && l.ProjectId == pLocation.ProjectId
                                              select l).FirstOrDefault();

                        if (isLocationExists == null && isLocationInDB == null)
                        {
                            locationsList.Add(new EFProjectLocations()
                            {
                                LocationId = newId,
                                ProjectId = pLocation.ProjectId,
                                FundsPercentage = pLocation.FundsPercentage
                            });
                        }

                        unitWork.ProjectLocationsRepository.Delete(pLocation);
                    }
                    await unitWork.SaveAsync();

                if (newId == 0)
                {
                    var locationToDelete = (from l in locations
                                            where l.Id == id
                                            select l).FirstOrDefault();
                    unitWork.LocationRepository.Delete(locationToDelete);
                }
                else
                {
                    unitWork.ProjectLocationsRepository.InsertMultiple(locationsList);
                }

                await unitWork.SaveAsync();
                response.ReturnedId = newId;
                response.Message = mHelper.DeleteMessage("Location");
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }
    }
}
