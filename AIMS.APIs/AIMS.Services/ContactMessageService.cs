using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AIMS.Services.Helpers;

namespace AIMS.Services
{
    public interface IContactMessageService
    {
        /// <summary>
        /// Gets all contact message views
        /// </summary>
        /// <returns></returns>
        IEnumerable<ContactMessageView> GetAll();

        /// <summary>
        /// Adds new contact message
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        ActionResponse Add(ContactMessageModel model);

        /// <summary>
        /// Approves the contact message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Approve(int id);

        /// <summary>
        /// Deletes the contact message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Delete(int id);
    }

    public class ContactMessageService
    {
        AIMSDbContext context;
        IMapper mapper;

        public ContactMessageService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<ContactMessageView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var contactMessages = unitWork.ContactMessagesRepository.GetAll();
                contactMessages = (from message in contactMessages
                                   orderby message.Dated descending
                                   select message);
                return mapper.Map<List<ContactMessageView>>(contactMessages);
            }
        }

        public ActionResponse Add(ContactMessageModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                var project = unitWork.ProjectRepository.GetOne(p => p.Id == model.ProjectId);
                if (project == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Project");
                    response.Success = false;
                    return response;
                }

                var user = unitWork.UserRepository.GetOne(u => u.Email == model.SenderEmail);
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User");
                    response.Success = false;
                    return response;
                }

                var newMessage = unitWork.ContactMessagesRepository.Insert(new EFContactMessages()
                {
                    SenderEmail = model.SenderEmail,
                    SenderName = model.SenderName,
                    ContactType = model.ContactType,
                    Subject = model.Subject,
                    Message = model.Message,
                    Dated = DateTime.Now,
                    IsViewed = false
                });
                unitWork.Save();
                response.ReturnedId = newMessage.Id;
                return response;
            }
        }
    }
}
