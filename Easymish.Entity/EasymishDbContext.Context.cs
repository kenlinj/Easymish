﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EasymishEntities : DbContext
    {
        public EasymishEntities()
            : base("name=EasymishEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccountTransaction> AccountTransactions { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<BinaryData> BinaryDatas { get; set; }
        public virtual DbSet<Continent> Continents { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Facility> Facilities { get; set; }
        public virtual DbSet<FacilityAccount> FacilityAccounts { get; set; }
        public virtual DbSet<FacilityAccountProperty> FacilityAccountProperties { get; set; }
        public virtual DbSet<FacilityProperty> FacilityProperties { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PersonProfile> PersonProfiles { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersInRole> UsersInRoles { get; set; }
    }
}