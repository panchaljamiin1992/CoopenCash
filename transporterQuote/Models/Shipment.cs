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
    
    public partial class Shipment
    {
        public int ShipmentID { get; set; }
        public int QuoteID { get; set; }
        public int RFQID { get; set; }
        public int TransporterID { get; set; }
        public int QuoteTypeID { get; set; }
        public Nullable<System.DateTime> PickupDT { get; set; }
        public string Product { get; set; }
        public string SourceLocation { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public int Containers { get; set; }
        public string PortofLoading { get; set; }
        public string PortofDischarge { get; set; }
        public string EmptyContainer { get; set; }
        public System.DateTime CreatedDT { get; set; }
        public int CreatedByUserID { get; set; }
        public bool IsDeleted { get; set; }
        public string ServiceIDs { get; set; }
        public System.DateTime DeliveryDT { get; set; }
        public int CustomerID { get; set; }
        public string ShipmentRFQIDs { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public int UpdatedByUserID { get; set; }
        public Nullable<System.DateTime> UpdatedDT { get; set; }
    }
}
