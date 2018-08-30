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
}
