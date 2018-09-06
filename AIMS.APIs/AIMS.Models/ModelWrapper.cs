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
    }

    public class NewOrganization
    {
        [Required]
        public string Name { get; set; }
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

}
