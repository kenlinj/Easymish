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
    
    public partial class FacilityProperty
    {
        public int ID { get; set; }
        public int FacilityID { get; set; }
        public string PropertyName { get; set; }
        public byte Type { get; set; }
        public int Order { get; set; }
        public string PropertyValue { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    
        public virtual Facility Facility { get; set; }
    }
}