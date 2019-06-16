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

        public EmailMessageService(AIMSDbContext cntxt)
        {
            context = cntxt;
        }

        public IEnumerable<EmailMessageView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<EmailMessageView> messagesList = new List<EmailMessageView>();
                var messages = unitWork.EmailMessagesRepository.GetAll();
                foreach(var message in messages)
                {
                    messagesList.Add(new EmailMessageView()
                    {
                        Id = message.Id,
                        TypeDefinition = message.TypeDefinition,
                        MessageType = message.MessageType,
                        Subject = message.Subject,
                        Message = message.Message
                    });
                }
                return messagesList;
            }
        }

        public async Task<IEnumerable<EmailMessageView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<EmailMessageView> messagesList = new List<EmailMessageView>();
                var emailMessages = await unitWork.EmailMessagesRepository.GetAllAsync();
                foreach (var message in emailMessages)
                {
                    messagesList.Add(new EmailMessageView()
                    {
                        Id = message.Id,
                        TypeDefinition = message.TypeDefinition,
                        MessageType = message.MessageType,
                        Subject = message.Subject,
                        Message = message.Message
                    });
                }
                return await Task<IEnumerable<EmailMessageView>>.Run(() => messagesList).ConfigureAwait(false);
            }
        }

        public EmailMessageView Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var emailMessage = unitWork.EmailMessagesRepository.GetByID(id);
                if (emailMessage == null)
                {
                    return new EmailMessageView() { };
                }
                return new EmailMessageView() { Id = emailMessage.Id, TypeDefinition = emailMessage.TypeDefinition, MessageType = emailMessage.MessageType, Subject = emailMessage.Subject, Message = emailMessage.Message };
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
                            Subject = model.Subject,
                            Message = model.Message
                        });
                        response.ReturnedId = newEmailMessage.Id;
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
                emailMessage.Subject = model.Subject;
                emailMessage.Message = model.Message;
                unitWork.EmailMessagesRepository.Update(emailMessage);
                unitWork.Save();
                return response;
            }
        }
    }
}
