using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface ISMTPSettingsService
    {
        /// <summary>
        /// Gets settings details for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SMTPSettingsModel Get();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(SMTPSettingsModel settings);

        /// <summary>
        /// Updates a settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        ActionResponse Update(int id, SMTPSettingsModel settings);
    }

    public class SMTPSettingsService
    {
        AIMSDbContext context;
        IMapper mapper;

        public SMTPSettingsService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public SMTPSettingsModelView Get()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var settings = unitWork.SMTPSettingsRepository.GetFirst(s => s.Host != null);
                return mapper.Map<SMTPSettingsModelView>(settings);
            }
        }

        public ActionResponse Add(SMTPSettingsModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var newSMTPSettings = unitWork.SMTPSettingsRepository.Insert(new EFSMTPSettings()
                    {
                        Host = model.Host,
                        Port = model.Port,
                        Username = model.Username,
                        Password = model.Password,
                        AdminEmail = model.AdminEmail
                    });
                    response.ReturnedId = newSMTPSettings.Id;
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

        public ActionResponse Update(int id, SMTPSettingsModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var settingsObj = unitWork.SMTPSettingsRepository.GetByID(id);
                if (settingsObj == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("SMTPSettings");
                    return response;
                }

                settingsObj.Host = model.Host;
                settingsObj.Port = model.Port;
                settingsObj.Username = model.Username;
                settingsObj.Password = model.Password;
                settingsObj.AdminEmail = model.AdminEmail;

                unitWork.SMTPSettingsRepository.Update(settingsObj);
                unitWork.Save();
                response.Message = true.ToString();
                return response;
            }
        }
    }
}
