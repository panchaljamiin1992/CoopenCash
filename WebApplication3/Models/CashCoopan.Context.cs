﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication3.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CashCoopanEntities : DbContext
    {
        public CashCoopanEntities()
            : base("name=CashCoopanEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tblTicketMaster> tblTicketMasters { get; set; }
        public DbSet<tblUserTicket> tblUserTickets { get; set; }
        public DbSet<User_Addresses> User_Addresses { get; set; }
        public DbSet<User> Users { get; set; }
    }
}