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
        public string UserName { get; set; }
        public string Email { get; set; }
        public UserTypes UserType { get; set; }
        public int OrganizationId { get; set; }
        public string Organization { get; set; }
        public bool IsApproved { get; set; }
        public string RegistrationDate { get; set; }
    }

    public class UserModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserTypes UserType { get; set; }
        public int OrganizationId { get; set; }
    }

}
