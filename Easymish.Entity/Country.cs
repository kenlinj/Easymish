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
    
    public partial class Country
    {
        public Country()
        {
            this.Addresses = new HashSet<Address>();
        }
    
        public string Code { get; set; }
        public string Alpha3Code { get; set; }
        public Nullable<short> NumericCode { get; set; }
        public string Name { get; set; }
        public string PhonePrefix { get; set; }
        public string DomainExtension { get; set; }
        public Nullable<byte> ContinentID { get; set; }
        public bool Active { get; set; }
    
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual Continent Continent { get; set; }
    }
}
