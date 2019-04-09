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
        /// Gets invalid sector type message
        /// </summary>
        /// <param name="sectorType"></param>
        /// <returns></returns>
        string InvalidSectorMapping(string sectorType);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectorCount"></param>
        /// <returns></returns>
        string NewIATISectorsAdded(int sectorCount);

        /// <summary>
        /// Gets invalid percentage message
        /// </summary>
        /// <returns></returns>
        string InvalidPercentage();

        /// <summary>
        /// Gets invalid date message
        /// </summary>
        /// <returns></returns>
        string InvalidDate();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string AlreadyExists(string entity);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string InvalidDisbursement();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string DeleteMessage(string entity);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string InvalidProjectMerge();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string InvalidOrganizationMerge();
    }

    public class MessageHelper : IMessageHelper
    {
        private readonly string NOT_FOUND = " not found with the provided ID";
        private readonly string USERNAME_TAKEN = " is already taken.";
        private readonly string NEW_RECORD = " added successfully.";
        private readonly string NEW = "New ";
        private readonly string DELETED = " deleted successfully";
        private readonly string INVALID_ATTEMPT = " made an invalid attempt to update data.";
        private readonly string INVALID_ACCOUNT_DELETION_ATTEMPT = "A super admin account cannot be deleted";
        private readonly string INVALID_SECTORTYPE_MAPPING = " cannot be used for sector mapping as it is a default sector type.";
        private readonly string INVALID_PROJECT_MERGE = "At least two projects must be provided for completing the project merge process";
        private readonly string INVALID_ORGANIZATION_MERGE = "At least two organizations must be provided for completing the project merge process";
        private readonly string EMAIL_NOT_FOUND = " did not match any of our records for registered users.";
        private readonly string INVALID_PERCENTAGE = "Invalid value provided for percentage.";
        private readonly string ALREADY_EXISTS = " provided is already entered once.";
        private readonly string INVALID_DATE = "Invalid value provided for date";
        private readonly string INVALID_DISBURSEMENTS = "You cannot add more disbursements than the project total value. Please increase the project funding amount before adding more disbursements.";

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

        public string NewIATISectorsAdded(int sectorCount)
        {
            return (sectorCount + " new IATI sectors loaded into the database. Please do mappings for new sectors if required");
        }

        public string EmailNotFound(string email)
        {
            return (email + EMAIL_NOT_FOUND);
        }

        public string InvalidPercentage()
        {
            return (INVALID_PERCENTAGE);
        }

        public string AlreadyExists(string entity)
        {
            return (entity + ALREADY_EXISTS);
        }

        public string InvalidDisbursement()
        {
            return (INVALID_DISBURSEMENTS);
        }

        public string DeleteMessage(string entity)
        {
            return (entity + DELETED);
        }

        public string InvalidDate()
        {
            return INVALID_DATE;
        }

        public string InvalidProjectMerge()
        {
            return INVALID_PROJECT_MERGE;
        }

        public string InvalidOrganizationMerge()
        {
            return INVALID_ORGANIZATION_MERGE;
        }

        public string InvalidSectorMapping(string sectorType)
        {
            return (sectorType + INVALID_SECTORTYPE_MAPPING);
        }
    }
}
