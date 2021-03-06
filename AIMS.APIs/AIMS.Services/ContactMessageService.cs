﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AIMS.Services.Helpers;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        /// Gets list of contact messages un-replied
        /// </summary>
        /// <returns></returns>
        IEnumerable<PendingMessagesView> GetUnRepliedMessages();

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

        /// <summary>
        /// Sets contact messages as notified
        /// </summary>
        /// <returns></returns>
        Task<ActionResponse> SetMessagesNotifiedAsync();

        /// <summary>
        /// Gets list of emails for project users
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<EmailAddress> GetProjectUsersEmails(int id);

        /// <summary>
        /// Gets list of emails for manager/admin users
        /// </summary>
        /// <returns></returns>
        IEnumerable<EmailAddress> GetManagerUsersEmails();
    }

    public class ContactMessageService : IContactMessageService
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
                var contactMessages = unitWork.ContactMessagesRepository.GetWithInclude(m => m.Id != 0, new string[] { "Project" });
                contactMessages = (from message in contactMessages
                                   orderby message.Dated descending
                                   select message);
                return mapper.Map<List<ContactMessageView>>(contactMessages);
            }
        }

        public IEnumerable<PendingMessagesView> GetUnRepliedMessages()
        {
            var unitWork = new UnitOfWork(context);
            var dateTime = DateTime.Now.AddDays(-14);
            var contactMessages = unitWork.ContactMessagesRepository.GetWithInclude(m => m.Dated.Date <= dateTime.Date && !m.IsNotified, new string[] { "Project" });
            List<PendingMessagesView> messagesUnReplied = new List<PendingMessagesView>();
            foreach (var message in contactMessages)
            {
                messagesUnReplied.Add(new PendingMessagesView()
                {
                    SenderEmail = message.SenderEmail,
                    SenderName = message.SenderName,
                    ProjectTitle = message.Project.Title,
                    Subject = message.Subject,
                    Message = message.Message
                });
            }
            return messagesUnReplied;
        }

        public ActionResponse Add(ContactMessageModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();

                try
                {
                    var project = unitWork.ProjectRepository.GetOne(p => p.Id == model.ProjectId);
                    if (project == null)
                    {
                        mHelper = new MessageHelper();
                        response.Message = mHelper.GetNotFound("Project");
                        response.Success = false;
                        return response;
                    }

                    var newMessage = unitWork.ContactMessagesRepository.Insert(new EFContactMessages()
                    {
                        ProjectId = project.Id,
                        SenderEmail = model.SenderEmail,
                        SenderName = model.SenderName,
                        EmailType = model.EmailType,
                        Subject = model.Subject,
                        Message = model.Message,
                        Dated = DateTime.Now,
                        IsNotified = false
                    });
                    unitWork.Save();
                    response.ReturnedId = newMessage.Id;
                }
                catch (Exception ex)
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

        public ActionResponse Approve(int id)
        {
            var unitWork = new UnitOfWork(context);
            IMessageHelper mHelper;
            ActionResponse response = new ActionResponse();

            try
            {
                var contactMessage = unitWork.ContactMessagesRepository.GetWithInclude(m => m.Id == id, new string[] { "Project" }).FirstOrDefault();
                if (contactMessage == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Contact message");
                    return response;
                }

                ContactEmailRequestModel model = new ContactEmailRequestModel()
                {
                    SenderEmail = contactMessage.SenderEmail,
                    SenderName = contactMessage.SenderName,
                    Subject = contactMessage.Subject,
                    Message = contactMessage.Message,
                    ProjectId = contactMessage.ProjectId,
                    ProjectTitle = contactMessage.Project.Title
                };
                response.Message = JsonConvert.SerializeObject(model);
            }
            catch (Exception ex)
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

        public ActionResponse Delete(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();
                try
                {
                    var contactMessage = unitWork.ContactMessagesRepository.GetByID(id);
                    if (contactMessage == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Contact message");
                        return response;
                    }

                    unitWork.ContactMessagesRepository.Delete(contactMessage);
                    unitWork.Save();
                }
                catch (Exception ex)
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

        public async Task<ActionResponse> SetMessagesNotifiedAsync()
        {
            ActionResponse response = new ActionResponse();
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var unitWork = new UnitOfWork(context);
                    var dateTime = DateTime.Now.AddDays(-14);
                    var contactMessages = await unitWork.ContactMessagesRepository.GetManyQueryableAsync(m => m.Dated.Date <= dateTime.Date);
                    if (contactMessages.Any())
                    {
                        foreach (var message in contactMessages)
                        {
                            message.IsNotified = true;
                            unitWork.ContactMessagesRepository.Update(message);
                        }
                        await unitWork.SaveAsync();
                        transaction.Commit();
                    }
                }
            });
            return response;
        }

        public IEnumerable<EmailAddress> GetProjectUsersEmails(int id)
        {
            var unitWork = new UnitOfWork(context);
            List<EmailAddress> emailAddressList = new List<EmailAddress>();
            IEnumerable<int> projectFunderIds = unitWork.ProjectFundersRepository.GetProjection(p => p.ProjectId == id, p => p.FunderId);
            IEnumerable<int> projectImplementerIds = unitWork.ProjectImplementersRepository.GetProjection(p => p.ProjectId == id, p => p.ImplementerId);
            int userId = (int)unitWork.ProjectRepository.GetProjection(p => p.Id == id, p => p.CreatedById).FirstOrDefault();
            if (userId > 0)
            {
                var email = unitWork.UserRepository.GetProjection(u => u.Id == id, u => u.Email).FirstOrDefault();
                if (email != null)
                {
                    emailAddressList.Add(new EmailAddress()
                    {
                        Email = email
                    });
                }
            }

            projectFunderIds = projectFunderIds.Union(projectImplementerIds);
            var emails = unitWork.UserRepository.GetProjection(u => projectFunderIds.Contains(u.OrganizationId), u => u.Email);
            foreach (var email in emails)
            {
                var emailExists = (from e in emailAddressList
                                   where e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                   select e).FirstOrDefault();

                if (emailExists == null)
                {
                    emailAddressList.Add(new EmailAddress()
                    {
                        Email = email
                    });
                }
            }
            if (emailAddressList.Count == 0)
            {
                var project = unitWork.ProjectRepository.GetWithInclude(p => p.Id == id, new string[] { "CreatedBy" }).FirstOrDefault();
                if (project != null)
                {
                    if (project.CreatedBy != null)
                    {
                        emailAddressList.Add(new EmailAddress()
                        {
                            Email = project.CreatedBy.Email
                        });
                    }
                }
            }
            return emailAddressList;
        }

        public IEnumerable<EmailAddress> GetManagerUsersEmails()
        {
            var unitWork = new UnitOfWork(context);
            List<EmailAddress> emailAddressList = new List<EmailAddress>();
            var emails = unitWork.UserRepository.GetProjection(u => (u.UserType == UserTypes.Manager || u.UserType == UserTypes.SuperAdmin), u => u.Email);
            foreach (var email in emails)
            {
                emailAddressList.Add(new EmailAddress()
                {
                    Email = email
                });
            }
            return emailAddressList;
        }
    }

}
