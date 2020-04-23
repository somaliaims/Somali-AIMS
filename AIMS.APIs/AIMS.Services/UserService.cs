using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserView> GetAll();

        /// <summary>
        /// Gets all users async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserView>> GetAllAsync();

        /// <summary>
        /// Gets list of standard users
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserView> GetStandardUsers();

        /// <summary>
        /// Gets list of manager users
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserView> GetManagerUsers();

        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAuthenticationView AuthenticateUser(string email, string password);

        /// <summary>
        /// Gets user information for the provided email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        UserAuthenticationView GetUserByEmail(string email);

        /// <summary>
        /// Checks availability of email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        ActionResponse CheckEmailAvailability(string email);

        /// <summary>
        /// Sends an email for password recovery for the provided email address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ActionResponse ResetPasswordRequest(PasswordResetEmailModel model, DateTime dated, string adminEmail);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        Task<ActionResponse> AddAsync(UserModel user, string adminEmail);

        /// <summary>
        /// Updates a user's organization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ActionResponse UpdateOrganization(int userId, int organizationId);

        /// <summary>
        /// Updates a user's organization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ActionResponse UpdatePassword(int userId, string newPassword);

        /// <summary>
        /// Resets password for the user provided token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> ResetPasswordAsync(PasswordResetModel model, DateTime tokenTime);

        //ActionResponse SendEmailToUsers();

        /// <summary>
        /// Deletes the account for the provided user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ActionResponse Delete(int userId, string password);

        /// <summary>
        /// Activates account for the provided user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> ActivateUserAccountAsync(UserApprovalModel model, string loginUrl, string defaultUserEmail);

        /// <summary>
        /// Promotes user to management
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ActionResponse PromoteUser(int userId, int loggedInUserId);

        /// <summary>
        /// Demotes user to standard
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ActionResponse DemoteUser(int userId, int loggedInUserId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ActionResponse SetNotificationsForUsers();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<ActionResponse> SetNotificationsForUsersAsync();

        /// <summary>
        /// Gets count for active users
        /// </summary>
        /// <returns></returns>
        int GetActiveUserCount();
    }

    public class UserService : IUserService
    {
        AIMSDbContext context;
        IMapper mapper;

        public UserService(AIMSDbContext cntxt, IMapper mappr)
        {
            this.context = cntxt;
            this.mapper = mappr;
        }

        public IEnumerable<UserView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<UserView> usersList = new List<UserView>();
                var users = unitWork.UserRepository.GetWithInclude(u => u.Id != 0, new string[] { "Organization" });
                usersList = mapper.Map<List<UserView>>(users);
                return usersList;
            }
        }

        public int GetActiveUserCount()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                return unitWork.UserRepository.GetProjection(u => u.IsApproved == true, u => u.Id).Count();
            }
        }

        public IEnumerable<UserView> GetStandardUsers()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var users = unitWork.UserRepository.GetWithInclude(u => u.Id != 0 && u.UserType == UserTypes.Standard, new string[] { "Organization" });
                if (users.Any())
                {
                    users = (from user in users
                             orderby user.Email
                             select user);
                }
                return mapper.Map<List<UserView>>(users);
            }
        }

        public IEnumerable<UserView> GetManagerUsers()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var users = unitWork.UserRepository.GetWithInclude(u => u.Id != 0 && u.UserType == UserTypes.Manager, new string[] { "Organization" });
                if (users.Any())
                {
                    users = (from user in users
                             orderby user.Email
                             select user);
                }
                return mapper.Map<List<UserView>>(users);
            }
        }

        public async Task<IEnumerable<UserView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<UserView> usersList = new List<UserView>();
                var users = await unitWork.UserRepository.GetAllAsync();
                usersList = mapper.Map<List<UserView>>(users);
                return await Task<IEnumerable<UserView>>.Run(() => usersList).ConfigureAwait(false);
            }
        }

        public UserAuthenticationView AuthenticateUser(string email, string password)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                UserAuthenticationView foundUser = new UserAuthenticationView();
                var findUser = unitWork.UserRepository.GetWithInclude(u => u.Email.Equals(email) && u.Password.Equals(password), new string[] { "Organization" }).FirstOrDefault();

                if (findUser != null)
                {
                    if (!findUser.IsApproved)
                    {
                        foundUser.OrganizationId = findUser.OrganizationId;
                        foundUser.IsApproved = false;
                    }
                    else
                    {
                        foundUser.Id = findUser.Id;
                        foundUser.Email = findUser.Email;
                        foundUser.UserType = findUser.UserType;
                        foundUser.OrganizationName = findUser.Organization.OrganizationName;
                        foundUser.OrganizationId = findUser.OrganizationId;
                        foundUser.IsApproved = true;

                        findUser.LastLogin = DateTime.Now;
                        unitWork.UserRepository.Update(findUser);
                        unitWork.Save();
                    }
                }
                return foundUser;
            }
        }

        public ActionResponse PromoteUser(int userId, int loggedInUserId)
        {
            using (var unitwork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var users = unitwork.UserRepository.GetManyQueryable(u => u.Id == userId || u.Id == loggedInUserId);
                var userToPromote = (from u in users
                                     where u.Id == userId
                                     select u).FirstOrDefault();

                var managerUser = (from u in users
                                   where u.Id == loggedInUserId
                                   && u.UserType == UserTypes.Manager
                                   select u).FirstOrDefault();

                if (userToPromote == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User to promote");
                    response.Success = false;
                    return response;
                }

                if (managerUser == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.UnAuthorizedUserAccountChange();
                    response.Success = false;
                    return response;
                }

                userToPromote.UserType = UserTypes.Manager;
                unitwork.UserRepository.Update(userToPromote);
                unitwork.Save();
                return response;
            }
        }

        public ActionResponse DemoteUser(int userId, int loggedInUserId)
        {
            using (var unitwork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var users = unitwork.UserRepository.GetManyQueryable(u => u.Id == userId || u.UserType == UserTypes.Manager);
                var userToDemote = (from u in users
                                     where u.Id == userId
                                     select u).FirstOrDefault();

                var managerUser = (from u in users
                                   where u.Id == loggedInUserId
                                   && u.UserType == UserTypes.Manager
                                   select u).FirstOrDefault();

                if (userToDemote == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User to promote");
                    response.Success = false;
                    return response;
                }

                if (managerUser == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.UnAuthorizedUserAccountChange();
                    response.Success = false;
                    return response;
                }

                var managerUserCount = (from u in users
                                        where u.UserType == UserTypes.Manager
                                        select u).Count();

                if (managerUserCount <= 1)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.ManagerAccountCannotBeDemoted();
                    response.Success = false;
                    return response;
                }

                userToDemote.UserType = UserTypes.Standard;
                unitwork.UserRepository.Update(userToDemote);
                unitwork.Save();
                return response;
            }
        }

        public UserAuthenticationView GetUserByEmail(string email)
        {
            var unitWork = new UnitOfWork(context);
            UserAuthenticationView foundUser = new UserAuthenticationView();
            var findUser = unitWork.UserRepository.GetWithInclude(u => u.Email.Equals(email),
                new string[] { "Organization" });

            if (findUser != null)
            {
                foreach (var user in findUser)
                {
                    foundUser.Id = user.Id;
                    foundUser.Email = user.Email;
                    foundUser.UserType = user.UserType;
                    foundUser.OrganizationId = user.Organization.Id;
                    break;
                }
            }
            return foundUser;
        }

        public ActionResponse CheckEmailAvailability(string email)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var user = unitWork.UserRepository.Get(u => u.Email.Equals(email));
                if (user != null)
                {
                    response.Success = false;
                    return response;
                }
                return response;
            }
        }

        public ActionResponse ResetPasswordRequest(PasswordResetEmailModel model, DateTime dated, string adminEmail)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                    var smtpSettings = smtpService.GetPrivate();
                    SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();

                    if (smtpSettings != null)
                    {
                        smtpSettingsModel.Host = smtpSettings.Host;
                        smtpSettingsModel.Port = smtpSettings.Port;
                        smtpSettingsModel.Username = smtpSettings.Username;
                        smtpSettingsModel.Password = smtpSettings.Password;
                        smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                        smtpSettingsModel.SenderName = smtpSettings.SenderName;
                    }

                    IEmailHelper emailHelper = new EmailHelper(smtpSettings.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                    response = emailHelper.SendPasswordRecoveryEmail(model);
                    if (!response.Success)
                    {
                        response.Message = response.Message;
                        response.Success = false;
                    }
                    else
                    {
                        unitWork.PasswordRecoveryRepository.Insert(new EFPasswordRecoveryRequests()
                        {
                            Email = model.Email,
                            Token = model.Token,
                            Dated = dated
                        });
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

        public async Task<ActionResponse> AddAsync(UserModel model, string adminEmail)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    EFOrganization organization = null;
                    EFOrganizationTypes organizationType = null;
                    ISecurityHelper sHelper = new SecurityHelper();
                    IMessageHelper mHelper;

                    if (!model.IsNewOrganization)
                    {
                        organization = unitWork.OrganizationRepository.GetByID(model.OrganizationId);
                        if (organization == null)
                        {
                            mHelper = new MessageHelper();
                            response.Success = false;
                            response.Message = mHelper.GetNotFound("Organization");
                            return response;
                        }
                    }
                    else
                    {
                        if (model.IsNewOrganization)
                        {
                            organizationType = unitWork.OrganizationTypesRepository.GetOne(t => t.Id == model.OrganizationTypeId);
                            if (organizationType == null)
                            {
                                mHelper = new MessageHelper();
                                response.Success = false;
                                response.Message = mHelper.GetNotFound("Organization Type");
                                return response;
                            }
                        }
                    }

                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            if (model.IsNewOrganization)
                            {
                                organization = new EFOrganization()
                                {
                                    OrganizationType = organizationType,
                                    OrganizationName = model.OrganizationName,
                                };
                                await unitWork.SaveAsync();
                                model.OrganizationId = organization.Id;
                            }
                            string passwordHash = sHelper.GetPasswordHash(model.Password);
                            var newUser = unitWork.UserRepository.Insert(new EFUser()
                            {
                                Email = model.Email,
                                UserType = UserTypes.Standard,
                                Organization = organization,
                                Password = passwordHash,
                                IsApproved = false,
                                RegistrationDate = DateTime.Now
                            });
                            await unitWork.SaveAsync();
                            //Get emails for all the users
                            var users = unitWork.UserRepository.GetMany(u => u.OrganizationId.Equals(organization.Id) && u.IsApproved == true);
                            List<EmailsModel> usersEmailList = new List<EmailsModel>();
                            foreach (var user in users)
                            {
                                if (user.Email != model.Email)
                                {
                                    usersEmailList.Add(new EmailsModel()
                                    {
                                        Email = user.Email,
                                        UserType = user.UserType
                                    });
                                }
                            }

                            var managerUsers = unitWork.UserRepository.GetMany(u => u.UserType == UserTypes.Manager || u.UserType == UserTypes.SuperAdmin);
                            foreach (var user in managerUsers)
                            {
                                if (user.Email != model.Email)
                                {
                                    usersEmailList.Add(new EmailsModel()
                                    {
                                        Email = user.Email,
                                        UserType = user.UserType
                                    });
                                }
                            }

                            if (usersEmailList.Count > 0)
                            {
                                //Send emails
                                ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                                var smtpSettings = smtpService.GetPrivate();
                                SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();
                                if (smtpSettings != null)
                                {
                                    smtpSettingsModel.Host = smtpSettings.Host;
                                    smtpSettingsModel.Port = smtpSettings.Port;
                                    smtpSettingsModel.Username = smtpSettings.Username;
                                    smtpSettingsModel.Password = smtpSettings.Password;
                                    smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                                    smtpSettingsModel.SenderName = smtpSettings.SenderName;
                                }

                                string message = "", subject = "", footerMessage = "";
                                var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.NewUser);
                                if (emailMessage != null)
                                {
                                    subject = emailMessage.Subject;
                                    message = emailMessage.Message;
                                    footerMessage = emailMessage.FooterMessage;
                                }
                                mHelper = new MessageHelper();
                                //Add notification
                                unitWork.NotificationsRepository.Insert(new EFUserNotifications()
                                {
                                    UserType = UserTypes.Standard,
                                    Organization = organization,
                                    Message = message,
                                    TreatmentId = newUser.Id,
                                    Email = newUser.Email,
                                    Dated = DateTime.Now,
                                    IsSeen = false,
                                    NotificationType = NotificationTypes.NewUser
                                });
                                await unitWork.SaveAsync();
                                transaction.Commit();
                                message += mHelper.NewUserForOrganization(organization.OrganizationName, model.Email);
                                IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                                emailHelper.SendNewRegistrationEmail(usersEmailList, organization.OrganizationName, subject, message, footerMessage);
                            }
                            response.ReturnedId = newUser.Id;
                        }
                    });
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    if (ex.InnerException != null)
                    {
                        response.Message = ex.InnerException.Message;
                    }
                    else
                    {
                        response.Message = ex.Message;
                    }
                }
                return response;
            }
        }

        public async Task<ActionResponse> ActivateUserAccountAsync(UserApprovalModel model, string loginUrl, string defaultUserEmail)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                EFUser userAccount = null;
                EFUser approvedByAccount = null;

                var userAccounts = unitWork.UserRepository.GetMany(u => u.Id.Equals(model.ApprovedById) || u.Id.Equals(model.UserId));
                foreach (var user in userAccounts)
                {
                    if (user.Id.Equals(model.ApprovedById))
                    {
                        approvedByAccount = user;
                    }
                    else if (user.Id.Equals(model.UserId))
                    {
                        userAccount = user;
                    }
                }
                if (approvedByAccount == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Approved By");
                    response.Success = false;
                    return response;
                }
                if (userAccount == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("User");
                    response.Success = false;
                    return response;
                }

                var notification = unitWork.NotificationsRepository.GetByID(model.NotificationId);
                if (notification == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Notification");
                    return response;
                }

                try
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            userAccount.IsApproved = true;
                            userAccount.ApprovedOn = DateTime.Now;

                            unitWork.UserRepository.Update(userAccount);
                            unitWork.NotificationsRepository.Delete(notification);
                            await unitWork.SaveAsync();

                            var userCount = unitWork.UserRepository.GetProjection(u => u.OrganizationId == userAccount.OrganizationId && u.IsApproved == true, u => u.Id).Count();
                            if (userCount == 0)
                            {
                                var funderProjectIds = unitWork.ProjectFundersRepository.GetProjection(f => f.FunderId == userAccount.OrganizationId, f => f.ProjectId);
                                var implementerProjectIds = unitWork.ProjectImplementersRepository.GetProjection(i => i.ImplementerId == userAccount.OrganizationId, i => i.ProjectId);
                                var projectIds = funderProjectIds.Union(implementerProjectIds);
                                if (projectIds.Any())
                                {
                                    var defaultUser = unitWork.UserRepository.GetOne(u => u.Email == defaultUserEmail);
                                    if (defaultUser != null)
                                    {
                                        var projects = unitWork.ProjectRepository.GetManyQueryable(p => projectIds.Contains(p.Id) && p.CreatedById == defaultUser.Id);
                                        if (projects.Any())
                                        {
                                            foreach (var project in projects)
                                            {
                                                project.CreatedBy = userAccount;
                                                unitWork.ProjectRepository.Update(project);
                                            }
                                            await unitWork.SaveAsync();
                                        }
                                    }
                                }
                            }
                        }
                        List<EmailAddress> usersEmailList = new List<EmailAddress>();
                        usersEmailList.Add(new EmailAddress()
                        {
                            Email = userAccount.Email
                        });
                        if (usersEmailList.Count > 0)
                        {
                            //Send emails
                            ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                            var smtpSettings = smtpService.GetPrivate();
                            SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();
                            if (smtpSettings != null)
                            {
                                smtpSettingsModel.Host = smtpSettings.Host;
                                smtpSettingsModel.Port = smtpSettings.Port;
                                smtpSettingsModel.Username = smtpSettings.Username;
                                smtpSettingsModel.Password = smtpSettings.Password;
                                smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                                smtpSettingsModel.SenderName = smtpSettings.SenderName;
                            }

                            string message = "", subject = "", footerMessage = "";
                            var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.UserApproved);
                            if (emailMessage != null)
                            {
                                subject = emailMessage.Subject;
                                message = emailMessage.Message;
                                footerMessage = emailMessage.FooterMessage;
                            }
                            mHelper = new MessageHelper();
                            message = mHelper.FormUserApprovedMessage(emailMessage.Message, loginUrl, emailMessage.FooterMessage);
                            IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                            emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                        }
                        response.ReturnedId = userAccount.Id;
                    });
                }
                catch(Exception ex)
                {
                    response.Message = ex.Message;
                    response.Success = false;
                }
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public ActionResponse UpdateOrganization(int userId, int organizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var organization = unitWork.OrganizationRepository.GetByID(organizationId);
                IMessageHelper msgHelper;
                if (organization == null)
                {
                    msgHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = msgHelper.GetNotFound("Organization");
                    return response;
                }

                var user = unitWork.UserRepository.GetByID(userId);
                if (user == null)
                {
                    msgHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = msgHelper.GetNotFound("User");
                    return response;
                }

                user.Organization = organization;
                unitWork.UserRepository.Update(user);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }

        public ActionResponse UpdatePassword(int userId, string password)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper msgHelper;
                var user = unitWork.UserRepository.GetByID(userId);
                if (user == null)
                {
                    msgHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = msgHelper.GetNotFound("User");
                    return response;
                }

                ISecurityHelper sHelper = new SecurityHelper();
                user.Password = sHelper.GetPasswordHash(password);
                unitWork.UserRepository.Update(user);
                unitWork.Save();

                //Send emails
                ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                var smtpSettings = smtpService.GetPrivate();
                SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();
                List<EmailAddress> usersEmailList = new List<EmailAddress>();
                usersEmailList.Add(new EmailAddress()
                {
                    Email = user.Email
                });
                if (smtpSettings != null)
                {
                    smtpSettingsModel.Host = smtpSettings.Host;
                    smtpSettingsModel.Port = smtpSettings.Port;
                    smtpSettingsModel.Username = smtpSettings.Username;
                    smtpSettingsModel.Password = smtpSettings.Password;
                    smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                    smtpSettingsModel.SenderName = smtpSettings.SenderName;
                }
                string message = "", subject = "", footerMessage = "";
                var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ResetPassword);
                if (emailMessage != null)
                {
                    subject = emailMessage.Subject;
                    message = emailMessage.Message;
                    footerMessage = emailMessage.FooterMessage;
                }
                IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                emailHelper.SendEmailToUsers(usersEmailList, subject, "", message, footerMessage);
                response.Message = true.ToString();
                return response;
            }
        }

        public async Task<ActionResponse> ResetPasswordAsync(PasswordResetModel model, DateTime tokenTime)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var isTokenExists = await unitWork.PasswordRecoveryRepository.GetOneAsync(r => r.Token == model.Token && r.Dated.Date == tokenTime.Date);
                        if (isTokenExists != null)
                        {
                            DateTime expirationTime = isTokenExists.Dated.AddHours(12);
                            if (expirationTime >= DateTime.Now)
                            {
                                try
                                {
                                    var user = await unitWork.UserRepository.GetOneAsync(u => u.Email == isTokenExists.Email);
                                    if (user != null)
                                    {
                                        ISecurityHelper sHelper = new SecurityHelper();
                                        var passwordHash = sHelper.GetPasswordHash(model.NewPassword);
                                        user.Password = passwordHash;

                                        unitWork.UserRepository.Update(user);
                                        unitWork.PasswordRecoveryRepository.Delete(isTokenExists);
                                        await unitWork.SaveAsync();

                                        var resetRequests = unitWork.PasswordRecoveryRepository.GetManyQueryable(r => r.Email == user.Email);
                                        foreach(var request in resetRequests)
                                        {
                                            unitWork.PasswordRecoveryRepository.Delete(request);
                                        }
                                        if (resetRequests.Any())
                                        {
                                            await unitWork.SaveAsync();
                                        }
                                        transaction.Commit();

                                        //Send emails
                                        ISMTPSettingsService smtpService = new SMTPSettingsService(context);
                                        var smtpSettings = smtpService.GetPrivate();
                                        SMTPSettingsModel smtpSettingsModel = new SMTPSettingsModel();
                                        List<EmailAddress> usersEmailList = new List<EmailAddress>();
                                        usersEmailList.Add(new EmailAddress()
                                        {
                                            Email = user.Email
                                        });
                                        if (smtpSettings != null)
                                        {
                                            smtpSettingsModel.Host = smtpSettings.Host;
                                            smtpSettingsModel.Port = smtpSettings.Port;
                                            smtpSettingsModel.Username = smtpSettings.Username;
                                            smtpSettingsModel.Password = smtpSettings.Password;
                                            smtpSettingsModel.AdminEmail = smtpSettings.AdminEmail;
                                            smtpSettingsModel.SenderName = smtpSettings.SenderName;
                                        }

                                        string message = "", subject = "", footerMessage = "";
                                        var emailMessage = unitWork.EmailMessagesRepository.GetOne(m => m.MessageType == EmailMessageType.ResetPassword);
                                        if (emailMessage != null)
                                        {
                                            subject = emailMessage.Subject;
                                            message = emailMessage.Message;
                                            footerMessage = emailMessage.FooterMessage;
                                        }
                                        IEmailHelper emailHelper = new EmailHelper(smtpSettingsModel.AdminEmail, smtpSettings.SenderName, smtpSettingsModel);
                                        emailHelper.SendEmailToUsers(usersEmailList, emailMessage.Subject, "", message, footerMessage);
                                    }
                                    else
                                    {
                                        response.Success = false;
                                        response.Message = "AIMS could not find any account with the provided email";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    response.Success = false;
                                    response.Message = ex.Message;
                                }
                            }
                            else
                            {
                                response.Success = false;
                                response.Message = "Password reset token is expired";
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Password reset token expired or not found";
                        }
                    }
                });
                
                return response;
            }
        }

        public ActionResponse Delete(int userId, string password)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                ISecurityHelper sHelper = new SecurityHelper();
                string passwordHash = sHelper.GetPasswordHash(password);
                var user = unitWork.UserRepository.Get(u => u.Id.Equals(userId) && u.Password.Equals(passwordHash));

                IMessageHelper mHelper;
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.IncorrectAccountInformation();
                    return response;
                }

                if (user.UserType == UserTypes.SuperAdmin || user.UserType == UserTypes.Manager)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.InvalidAccountDeletionAttempt();
                    return response;
                }
                //user.ApprovedBy = null;
                unitWork.UserRepository.Update(user);
                unitWork.Save();

                unitWork.UserRepository.Delete(user);
                unitWork.Save();
                return response;
            }
        }

        public ActionResponse SetNotificationsForUsers()
        {
            var unitWork = new UnitOfWork(context);
            ActionResponse response = new ActionResponse();

            try
            {
                var todaysDate = DateTime.Now;
                var dateForDeletion = todaysDate.AddDays(-365);
                var dateForInactive = todaysDate.AddDays(-350);
                IMessageHelper mHelper = new MessageHelper();
                var userNotifications = unitWork.NotificationsRepository.GetManyQueryable(n => n.NotificationType == NotificationTypes.UserInactive && n.Dated.Date < DateTime.Now.Date);
                var users = unitWork.UserRepository.GetWithInclude(u => u.LastLogin <= dateForInactive, new string[] { "Organization" });
                List<EFUserNotifications> notificationsList = new List<EFUserNotifications>();
                foreach (var user in users)
                {
                    if (user.LastLogin <= dateForDeletion)
                    {
                        unitWork.UserRepository.Delete(user);
                        unitWork.Save();
                    }
                    else
                    {
                        var isNotificationExists = (from notification in userNotifications
                                                    where notification.OrganizationId == user.OrganizationId
                                                    && notification.TreatmentId == user.Id
                                                    select notification).FirstOrDefault();

                        if (isNotificationExists == null)
                        {
                            notificationsList.Add(new EFUserNotifications()
                            {
                                OrganizationId = user.OrganizationId,
                                NotificationType = NotificationTypes.UserInactive,
                                Message = mHelper.InactiveUserMessage(user.Email, user.Organization.OrganizationName),
                                Dated = DateTime.Now,
                                UserType = UserTypes.Standard
                            });
                        }
                    }
                }
                if (notificationsList.Count > 0)
                {
                    unitWork.NotificationsRepository.InsertMultiple(notificationsList);
                    unitWork.Save();
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        public async Task<ActionResponse> SetNotificationsForUsersAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var todaysDate = DateTime.Now;
                        var dateForDeletion = todaysDate.AddDays(-365);
                        var dateForInactive = todaysDate.AddDays(-350);
                        IMessageHelper mHelper = new MessageHelper();
                        var userNotifications = unitWork.NotificationsRepository.GetManyQueryable(n => n.NotificationType == NotificationTypes.UserInactive && n.Dated.Date < DateTime.Now.Date);
                        var users = await unitWork.UserRepository.GetWithIncludeAsync(u => u.LastLogin <= dateForInactive, new string[] { "Organization" });
                        List<EFUserNotifications> notificationsList = new List<EFUserNotifications>();
                        foreach (var user in users)
                        {
                            if (user.LastLogin <= dateForDeletion)
                            {
                                unitWork.UserRepository.Delete(user);
                            }
                            else
                            {
                                var isNotificationExists = (from notification in userNotifications
                                                            where notification.OrganizationId == user.OrganizationId
                                                            && notification.TreatmentId == user.Id
                                                            select notification).FirstOrDefault();

                                if (isNotificationExists == null)
                                {
                                    notificationsList.Add(new EFUserNotifications()
                                    {
                                        OrganizationId = user.OrganizationId,
                                        NotificationType = NotificationTypes.UserInactive,
                                        Message = mHelper.InactiveUserMessage(user.Email, user.Organization.OrganizationName),
                                        Dated = DateTime.Now,
                                        UserType = UserTypes.Standard
                                    });
                                }
                            }
                        }
                        if (notificationsList.Count > 0)
                        {
                            unitWork.NotificationsRepository.InsertMultiple(notificationsList);
                        }
                        await unitWork.SaveAsync();
                        transaction.Commit();
                    }
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }
    }
}
