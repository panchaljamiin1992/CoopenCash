//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace transporterQuote.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Field
    {
        public int FieldID { get; set; }
        public string FieldName { get; set; }
        public int FieldType { get; set; }
        public bool IsRequired { get; set; }
        public System.DateTime CreatedDT { get; set; }
        public int CreatedByUserID { get; set; }
        public Nullable<System.DateTime> UpdatedDT { get; set; }
        public Nullable<int> UpdatedByUserID { get; set; }
        public bool IsDeleted { get; set; }
        public string Choices { get; set; }
        public int QuoteTypeID { get; set; }
        public bool ShowCustomer { get; set; }
        public bool ShowVendors { get; set; }
    }
}