//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Easymish.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Address
    {
        public Address()
        {
            this.People = new HashSet<Person>();
        }
    
        public int ID { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual ICollection<Person> People { get; set; }
    }
}
