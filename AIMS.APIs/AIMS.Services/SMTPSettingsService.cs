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
        SMTPSettingsModelView Get();

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

    public class SMTPSettingsService : ISMTPSettingsService
    {
        AIMSDbContext context;
        //IMapper mapper;

        public SMTPSettingsService(AIMSDbContext cntxt)
        {
            context = cntxt;
            //mapper = new AutoMapper();
        }

        public SMTPSettingsModelView Get()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var settings = unitWork.SMTPSettingsRepository.GetOne(s => s.Id != 0);
                SMTPSettingsModelView view = new SMTPSettingsModelView();
                if (settings != null)
                {
                    view.Id = settings.Id;
                    view.AdminEmail = settings.AdminEmail;
                    view.Host = settings.Host;
                    view.Port = settings.Port;
                    view.Username = settings.Username;
                }
                return view;
            }
        }

        public ActionResponse Add(SMTPSettingsModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var smtpSettings = unitWork.SMTPSettingsRepository.GetOne(s => s.Id != 0);
                    if (smtpSettings != null)
                    {
                        smtpSettings.Host = model.Host;
                        smtpSettings.Port = model.Port;
                        smtpSettings.Username = model.Username;
                        smtpSettings.AdminEmail = model.AdminEmail;

                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            smtpSettings.Password = model.Password;
                        }

                        unitWork.SMTPSettingsRepository.Update(smtpSettings);
                        unitWork.Save();
                        response.ReturnedId = smtpSettings.Id;
                    }
                    else
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

                if (!string.IsNullOrEmpty(model.Password))
                {
                    settingsObj.Password = model.Password;
                }
                
                settingsObj.AdminEmail = model.AdminEmail;

                unitWork.SMTPSettingsRepository.Update(settingsObj);
                unitWork.Save();
                response.Message = true.ToString();
                return response;
            }
        }
    }
}
