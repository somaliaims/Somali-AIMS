using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AIMS.Services.Helpers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace AIMS.Services
{
    public interface IMarkersService
    {
        /// <summary>
        /// Gets all markers
        /// </summary>
        /// <returns></returns>
        IEnumerable<MarkerView> GetAll();

        /// <summary>
        /// Gets list of active fields
        /// </summary>
        /// <returns></returns>
        IEnumerable<MarkerView> GetActiveFields();

        /// <summary>
        /// Get matching markers for the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<MarkerView> GetMatching(string criteria);

        /// <summary>
        /// Gets the marker for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        MarkerView Get(int id);
        /// <summary>
        /// Gets all markers async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MarkerView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(MarkerModel marker);

        /// <summary>
        /// Updates a marker
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        ActionResponse Update(int id, MarkerModel marker);

        /// <summary>
        /// Deletes a marker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id);
    }

    public class MarkersService : IMarkersService
    {
        AIMSDbContext context;
        IMapper mapper;

        public MarkersService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<MarkerView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var markers = unitWork.MarkerRepository.GetAll();
                if (markers != null)
                {
                    markers = (from c in markers
                                  orderby c.FieldTitle ascending
                                  select c);
                }
                return mapper.Map<List<MarkerView>>(markers);
            }
        }

        public IEnumerable<MarkerView> GetActiveFields()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var dated = DateTime.Now;
                var markers = unitWork.MarkerRepository.GetManyQueryable(c => c.Id != 0);
                if (markers != null)
                {
                    markers = (from c in markers
                                    orderby c.FieldTitle ascending
                                    select c);
                }
                return mapper.Map<List<MarkerView>>(markers);
            }
        }

        public async Task<IEnumerable<MarkerView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var markers = await unitWork.MarkerRepository.GetAllAsync();
                if (markers != null)
                {
                    markers = (from c in markers
                                  orderby c.FieldTitle ascending
                                  select c);
                }
                return await Task<IEnumerable<MarkerView>>.Run(() => mapper.Map<List<MarkerView>>(markers)).ConfigureAwait(false);
            }
        }

        public MarkerView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var marker = unitWork.MarkerRepository.GetByID(id);
                return mapper.Map<MarkerView>(marker);
            }
        }

        public IEnumerable<MarkerView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<MarkerView> markersList = new List<MarkerView>();
                var markers = unitWork.MarkerRepository.GetMany(o => o.FieldTitle.Contains(criteria, StringComparison.OrdinalIgnoreCase));
                if (markers != null)
                {
                    markers = (from c in markers
                                  orderby c.FieldTitle ascending
                                  select c);
                }
                return mapper.Map<List<MarkerView>>(markers);
            }
        }

        public ActionResponse Add(MarkerModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    IMessageHelper mHelper;
                    List<MarkerValues> valuesList = new List<MarkerValues>();
                    if (!string.IsNullOrEmpty(model.Values))
                    {
                       valuesList = JsonConvert.DeserializeObject<List<MarkerValues>>(model.Values);
                    }

                    switch (model.FieldType)
                    {
                        case FieldTypes.DropDown:
                        case FieldTypes.CheckBox:
                            if (valuesList.Count < 1)
                            {
                                mHelper = new MessageHelper();
                                response.Message = mHelper.GetInvalidOptionsMessage();
                                response.Success = false;
                                return response;
                            }
                            break;

                        case FieldTypes.Radio:
                            if (valuesList.Count < 2)
                            {
                                mHelper = new MessageHelper();
                                response.Message = mHelper.GetInvalidOptionsMessage();
                                response.Success = false;
                                return response;
                            }
                            break;

                        default:
                            break;
                    }
                    
                    var markers = unitWork.MarkerRepository.GetManyQueryable(l => (l.FieldTitle.ToLower() == model.FieldTitle.ToLower()));
                    var isMarkerCreated = (from c in markers
                                             where c.FieldTitle.ToLower() == model.FieldTitle
                                             select c).FirstOrDefault();

                    if (isMarkerCreated != null)
                    {
                        isMarkerCreated.FieldTitle = model.FieldTitle;
                        isMarkerCreated.FieldType = model.FieldType;
                        isMarkerCreated.Values = model.Values;
                        isMarkerCreated.Help = model.Help;
                        unitWork.MarkerRepository.Update(isMarkerCreated);
                        unitWork.Save();
                        response.ReturnedId = isMarkerCreated.Id;
                    }
                    else
                    {
                        var newMarker = unitWork.MarkerRepository.Insert(new EFMarkers()
                        {
                           FieldTitle = model.FieldTitle,
                           FieldType = model.FieldType,
                           Values = model.Values,
                           Help = model.Help
                        });
                        unitWork.Save();
                        response.ReturnedId = newMarker.Id;
                    }
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, MarkerModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var marker = unitWork.MarkerRepository.GetByID(id);

                if (marker == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Marker");
                    response.Success = false;
                    return response;
                }

                List<MarkerValues> valuesList = new List<MarkerValues>();
                if (!string.IsNullOrEmpty(model.Values))
                {
                    valuesList = JsonConvert.DeserializeObject<List<MarkerValues>>(model.Values);
                }

                switch (model.FieldType)
                {
                    case FieldTypes.DropDown:
                    case FieldTypes.CheckBox:
                        if (valuesList.Count < 1)
                        {
                            mHelper = new MessageHelper();
                            response.Message = mHelper.GetInvalidOptionsMessage();
                            response.Success = false;
                            return response;
                        }
                        break;

                    case FieldTypes.Radio:
                        if (valuesList.Count < 2)
                        {
                            mHelper = new MessageHelper();
                            response.Message = mHelper.GetInvalidOptionsMessage();
                            response.Success = false;
                            return response;
                        }
                        break;

                    default:
                        break;
                }

                marker.FieldTitle = model.FieldTitle;
                marker.FieldType = model.FieldType;
                marker.Values = model.Values;
                marker.Help = model.Help;
                unitWork.MarkerRepository.Update(marker);
                unitWork.Save();
                response.Message = true.ToString();
                response.ReturnedId = id;
                return response;
            }
        }

        public async Task<ActionResponse> DeleteAsync(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var marker = await unitWork.MarkerRepository.GetByIDAsync(id);
                if (marker == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Marker");
                    return response;
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var projectMarkers = unitWork.ProjectMarkersRepository.GetManyQueryable(c => c.MarkerId == id);
                        foreach(var cField in projectMarkers)
                        {
                            unitWork.ProjectMarkersRepository.Delete(cField);
                        }
                        unitWork.MarkerRepository.Delete(marker);
                        await unitWork.SaveAsync();
                        transaction.Commit();
                    }
                });
                response.Message = true.ToString();
                return response;
            }
        }
    }
}
