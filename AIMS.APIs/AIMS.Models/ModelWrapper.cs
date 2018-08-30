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

    public class OrganizationView
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class NewOrganization
    {
        public string Name { get; set; }
    }

    public class SectorView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DateCreated { get; set; }
    }

    public class NewSector
    {
        [Required]
        public string Name { get; set; }
    }

    public class UpdateSector
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
