using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Services.Helpers
{
    public interface IMessageHelper
    {
        /// <summary>
        /// Gets not found message for the provided entity name
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string GetNotFound(string entity);

        /// <summary>
        /// Gets username taken message
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        string UserNameAlreadyTaken(string userName);

        /// <summary>
        /// Gets invalid attempt message
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string GetInvalidAttempt(string entity);

        /// <summary>
        /// Shows message for new entity insertion
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string NewRecord(string entity);

        /// <summary>
        /// Show message for attempting the deletion of super admin account
        /// </summary>
        /// <returns></returns>
        string InvalidAccountDeletionAttempt();

        /// <summary>
        /// Show a message for email not found
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        string EmailNotFound(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        string NewUserForOrganization(string organization);
    }

    public class MessageHelper : IMessageHelper
    {
        private readonly string NOT_FOUND = " not found with the provided ID";
        private readonly string USERNAME_TAKEN = " is already taken.";
        private readonly string NEW_RECORD = " added successfully.";
        private readonly string NEW = "New ";
        private readonly string INVALID_ATTEMPT = " made an invalid attempt to update data.";
        private readonly string INVALID_ACCOUNT_DELETION_ATTEMPT = "A super admin account cannot be deleted";
        private readonly string EMAIL_NOT_FOUND = " did not match any of our records for registered users.";

        public string GetNotFound(string entity)
        {
            return (entity + NOT_FOUND);
        }

        public string UserNameAlreadyTaken(string userName)
        {
            return (userName + USERNAME_TAKEN);
        }

        public string NewRecord(string entity)
        {
            return (NEW + entity + NEW_RECORD);
        }

        public string GetInvalidAttempt(string entity)
        {
            return (entity + INVALID_ATTEMPT);
        }

        public string InvalidAccountDeletionAttempt()
        {
            return INVALID_ACCOUNT_DELETION_ATTEMPT;
        }

        public string NewUserForOrganization(string organization)
        {
            return ("A new user has submitted the request to register for the organization " +  organization);
        }

        public string EmailNotFound(string email)
        {
            return (email + EMAIL_NOT_FOUND);
        }
    }
}
