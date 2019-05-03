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
    public interface IEmailMessageService
    {
        /// <summary>
        /// Gets all emailMessages
        /// </summary>
        /// <returns></returns>
        IEnumerable<EmailMessageView> GetAll();

        /// <summary>
        /// Gets the emailMessage for the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        EmailMessageView Get(int id);
        /// <summary>
        /// Gets all emailMessages async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<EmailMessageView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(EmailMessageModel emailMessage);

        /// <summary>
        /// Updates a emailMessage
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        ActionResponse Update(int id, EmailMessageModel emailMessage);
    }

    public class EmailMessageService : IEmailMessageService
    {
        AIMSDbContext context;
        IMapper mapper;

        public EmailMessageService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<EmailMessageView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var messages = unitWork.EmailMessagesRepository.GetAll();
                return mapper.Map<List<EmailMessageView>>(messages);
            }
        }

        public async Task<IEnumerable<EmailMessageView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var emailMessages = await unitWork.EmailMessagesRepository.GetAllAsync();
                return await Task<IEnumerable<EmailMessageView>>.Run(() => mapper.Map<List<EmailMessageView>>(emailMessages)).ConfigureAwait(false);
            }
        }

        public EmailMessageView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var emailMessage = unitWork.EmailMessagesRepository.GetByID(id);
                return mapper.Map<EmailMessageView>(emailMessage);
            }
        }

        public ActionResponse Add(EmailMessageModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isEmailMessageCreated = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == model.MessageType);
                    if (isEmailMessageCreated != null)
                    {
                        isEmailMessageCreated.Message = model.Message;
                        unitWork.EmailMessagesRepository.Update(isEmailMessageCreated);
                        response.ReturnedId = isEmailMessageCreated.Id;
                    }
                    else
                    {
                        var newEmailMessage = unitWork.EmailMessagesRepository.Insert(new EFEmailMessages()
                        {
                            MessageType = model.MessageType,
                            Message = model.Message
                        });
                        unitWork.Save();
                        response.ReturnedId = newEmailMessage.Id;
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

        public ActionResponse Update(int id, EmailMessageModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var emailMessage = unitWork.EmailMessagesRepository.GetByID(id);
                if (emailMessage == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("EmailMessage");
                    return response;
                }

                response.Message = true.ToString();
                emailMessage.Message = model.Message;
                unitWork.EmailMessagesRepository.Update(emailMessage);
                unitWork.Save();
                return response;
            }
        }
    }
}
