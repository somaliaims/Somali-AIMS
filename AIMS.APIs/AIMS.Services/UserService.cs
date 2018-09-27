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
        /// Authenticates a user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserView AuthenticateUser(string userName, string password);

        /// <summary>
        /// Checks availability of email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        ActionResponse CheckEmailAvailability(string email);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(UserModel user);

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

        public UserView AuthenticateUser(string userName, string password)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                UserView foundUser = new UserView();
                var findUser = unitWork.UserRepository.GetWithInclude(u => u.DisplayName.Equals(userName) && u.Password.Equals(password),
                    new string[] { "Organization" });

                if (findUser != null)
                {
                    foreach (var user in findUser)
                    {
                        foundUser.DisplayName = user.DisplayName;
                        foundUser.UserType = user.UserType;
                        foundUser.Organization = user.Organization.OrganizationName;
                        break;
                    }
                }
                return foundUser;
            }
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

        public ActionResponse Add(UserModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    //EFOrganization organization = unitWork.OrganizationRepository.GetByID(model.OrganizationId);
                    //For testing only, need to retain the one above again after testing
                    EFOrganization organization = unitWork.OrganizationRepository.GetByID(1);
                    ISecurityHelper sHelper = new SecurityHelper();
                    IMessageHelper mHelper;
                    if (organization == null)
                    {
                        mHelper = new MessageHelper();
                        response.Success = false;
                        response.Message = mHelper.GetNotFound("Organization");
                        return response;
                    }

                    string passwordHash = sHelper.GetPasswordHash(model.Password);
                    var newUser = unitWork.UserRepository.Insert(new EFUser()
                    {
                        DisplayName = model.DisplayName,
                        Email = model.Email,
                        UserType = model.UserType,
                        Organization = organization,
                        Password = passwordHash,
                        IsApproved = false,
                        RegistrationDate = DateTime.Now

                    });
                    unitWork.Save();
                    response.ReturnedId = newUser.Id;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
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
                response.Message = "1";
                return response;
            }
        }
    }
}
