using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
    }
}
