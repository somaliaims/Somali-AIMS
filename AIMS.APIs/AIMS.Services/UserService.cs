﻿using AIMS.DAL.EF;
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

        public ActionResponse Add(UserModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    EFOrganization organization = unitWork.OrganizationRepository.GetByID(model.OrganizationId);
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
                        UserName = model.UserName,
                        Email = model.Email,
                        UserType = model.UserType,
                        Organization = organization,
                        Password = passwordHash,
                        IsApproved = false,
                        RegistrationDate = DateTime.Now

                    });
                    response.ReturnedId = newUser.Id;
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