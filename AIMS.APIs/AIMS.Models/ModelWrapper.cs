using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Models
{
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
        public string Name { get; set; }
    }

    public class UpdateSector
    {
        public int EffectedSectorId { get; set; }
        public string Name { get; set; }
    }
}
