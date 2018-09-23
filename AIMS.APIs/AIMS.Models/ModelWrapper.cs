using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AIMS.Models
{
    public class ActionResponse
    {
        public int ReturnedId { get; set; } = 0;
        public string Message { get; set; } = "";
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// Organization models
    /// </summary>
    public class OrganizationView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
    }

    public class OrganizationModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int TypeId { get; set; }
    }

    /// <summary>
    /// Sector models
    /// </summary>
    public class SectorView
    {
        public int Id { get; set; }
        public string SectorName { get; set; }
    }

    public class SectorModel
    {
        [Required]
        public string Name { get; set; }
    }

    /// <summary>
    /// Organization type models
    /// </summary>
    public class OrganizationTypeView
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class OrganizationTypeModel
    {
        [Required]
        public string TypeName { get; set; }
    }

    /// <summary>
    /// User models
    /// </summary>
    public class UserView
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public UserTypes UserType { get; set; }
        public int OrganizationId { get; set; }
        public string Organization { get; set; }
        public bool IsApproved { get; set; }
        public string RegistrationDate { get; set; }
    }

    public class AuthenticateModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserModel
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserTypes UserType { get; set; }
        public int OrganizationId { get; set; }
    }

    public class EditUserOrganization
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class EditUserPassword
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Location models
    /// </summary>
    public class LocationView
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class LocationModel
    {
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }


    /// <summary>
    /// Project models
    /// </summary>
    public class ProjectView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Objective { get; set; }
        public string DateStarted { get; set; }
        public string DateEnded { get; set; }
        public string ProjectType { get; set; }
    }

    public class ProjectModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Objective { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProjectTypeId { get; set; }
    }

    /// <summary>
    /// Project funders models
    /// </summary>
    public class ProjectFunderView
    {
        public int FunderId { get; set; }
        public string Funder { get; set; }
        public int ProjectId { get; set; }
        public string Project { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class ProjectFunderModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int FunderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }


    /// <summary>
    /// Project implementors models
    /// </summary>
    public class ProjectImplementorView
    {
        public int FunderId { get; set; }
        public string Funder { get; set; }
        public int ProjectId { get; set; }
        public string Project { get; set; }
    }

    public class ProjectImplementorModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int ImplementorId { get; set; }
    }

}
