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
    public interface ICustomFieldsService
    {
        /// <summary>
        /// Gets all customFields
        /// </summary>
        /// <returns></returns>
        IEnumerable<CustomFieldView> GetAll();

        /// <summary>
        /// Gets list of active fields
        /// </summary>
        /// <returns></returns>
        IEnumerable<CustomFieldView> GetActiveFields();

        /// <summary>
        /// Get matching customFields for the criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<CustomFieldView> GetMatching(string criteria);

        /// <summary>
        /// Gets the customField for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        CustomFieldView Get(int id);
        /// <summary>
        /// Gets all customFields async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CustomFieldView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(CustomFieldModel customField);

        /// <summary>
        /// Updates a customField
        /// </summary>
        /// <param name="customField"></param>
        /// <returns></returns>
        ActionResponse Update(int id, CustomFieldModel customField);

        /// <summary>
        /// Deletes a customField
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResponse> DeleteAsync(int id);
    }

    public class CustomFieldsService : ICustomFieldsService
    {
        AIMSDbContext context;
        IMapper mapper;

        public CustomFieldsService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<CustomFieldView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var customFields = unitWork.CustomFieldRepository.GetAll();
                if (customFields != null)
                {
                    customFields = (from c in customFields
                                  orderby c.FieldTitle ascending
                                  select c);
                }
                return mapper.Map<List<CustomFieldView>>(customFields);
            }
        }

        public IEnumerable<CustomFieldView> GetActiveFields()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var dated = DateTime.Now;
                var customFields = unitWork.CustomFieldRepository.GetManyQueryable(c => dated.Date >= c.ActiveFrom.Date && dated.Date <= c.ActiveUpto);
                if (customFields != null)
                {
                    customFields = (from c in customFields
                                    orderby c.FieldTitle ascending
                                    select c);
                }
                return mapper.Map<List<CustomFieldView>>(customFields);
            }
        }

        public async Task<IEnumerable<CustomFieldView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var customFields = await unitWork.CustomFieldRepository.GetAllAsync();
                if (customFields != null)
                {
                    customFields = (from c in customFields
                                  orderby c.FieldTitle ascending
                                  select c);
                }
                return await Task<IEnumerable<CustomFieldView>>.Run(() => mapper.Map<List<CustomFieldView>>(customFields)).ConfigureAwait(false);
            }
        }

        public CustomFieldView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var customField = unitWork.CustomFieldRepository.GetByID(id);
                return mapper.Map<CustomFieldView>(customField);
            }
        }

        public IEnumerable<CustomFieldView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<CustomFieldView> customFieldsList = new List<CustomFieldView>();
                var customFields = unitWork.CustomFieldRepository.GetMany(o => o.FieldTitle.Contains(criteria, StringComparison.OrdinalIgnoreCase));
                if (customFields != null)
                {
                    customFields = (from c in customFields
                                  orderby c.FieldTitle ascending
                                  select c);
                }
                return mapper.Map<List<CustomFieldView>>(customFields);
            }
        }

        public ActionResponse Add(CustomFieldModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    IMessageHelper mHelper;
                    List<CustomFieldValues> valuesList = new List<CustomFieldValues>();
                    if (!string.IsNullOrEmpty(model.Values))
                    {
                       valuesList = JsonConvert.DeserializeObject<List<CustomFieldValues>>(model.Values);
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
                    
                    var customFields = unitWork.CustomFieldRepository.GetManyQueryable(l => (l.FieldTitle.ToLower() == model.FieldTitle.ToLower()));
                    var isCustomFieldCreated = (from c in customFields
                                             where c.FieldTitle.ToLower() == model.FieldTitle
                                             select c).FirstOrDefault();

                    if (isCustomFieldCreated != null)
                    {
                        isCustomFieldCreated.FieldTitle = model.FieldTitle;
                        isCustomFieldCreated.FieldType = model.FieldType;
                        isCustomFieldCreated.Values = model.Values;
                        isCustomFieldCreated.ActiveFrom = model.ActiveFrom;
                        isCustomFieldCreated.ActiveUpto = model.ActiveUpto;
                        unitWork.CustomFieldRepository.Update(isCustomFieldCreated);
                        unitWork.Save();
                        response.ReturnedId = isCustomFieldCreated.Id;
                    }
                    else
                    {
                        var newCustomField = unitWork.CustomFieldRepository.Insert(new EFCustomFields()
                        {
                           FieldTitle = model.FieldTitle,
                           FieldType = model.FieldType,
                           Values = model.Values,
                           ActiveFrom = model.ActiveFrom,
                           ActiveUpto = model.ActiveUpto,
                        });
                        unitWork.Save();
                        response.ReturnedId = newCustomField.Id;
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

        public ActionResponse Update(int id, CustomFieldModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var customField = unitWork.CustomFieldRepository.GetByID(id);

                if (customField == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("CustomField");
                    response.Success = false;
                    return response;
                }

                List<CustomFieldValues> valuesList = new List<CustomFieldValues>();
                if (!string.IsNullOrEmpty(model.Values))
                {
                    valuesList = JsonConvert.DeserializeObject<List<CustomFieldValues>>(model.Values);
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

                customField.FieldTitle = model.FieldTitle;
                customField.FieldType = model.FieldType;
                customField.Values = model.Values;
                customField.ActiveFrom = model.ActiveFrom;
                customField.ActiveUpto = model.ActiveUpto;
                unitWork.CustomFieldRepository.Update(customField);
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
                var customField = await unitWork.CustomFieldRepository.GetByIDAsync(id);
                if (customField == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("CustomField");
                    return response;
                }

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var projectCustomFields = unitWork.ProjectCustomFieldsRepository.GetManyQueryable(c => c.CustomFieldId == id);
                        foreach(var cField in projectCustomFields)
                        {
                            unitWork.ProjectCustomFieldsRepository.Delete(cField);
                        }
                        unitWork.CustomFieldRepository.Delete(customField);
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
