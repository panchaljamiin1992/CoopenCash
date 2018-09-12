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
    
    public partial class RFQ
    {
        public int RequestForQuoteID { get; set; }
        public int CustomerID { get; set; }
        public int SourceID { get; set; }
        public int ExecutiveID { get; set; }
        public string Destination { get; set; }
        public string Source { get; set; }
        public string Product { get; set; }
        public System.DateTime PickUp { get; set; }
        public System.DateTime DeliveryBy { get; set; }
        public bool AllowLaterDelivery { get; set; }
        public System.DateTime QuoteBy { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public string Transporters { get; set; }
        public System.DateTime CreatedDT { get; set; }
        public System.DateTime UpdatedDT { get; set; }
        public bool IsDeleted { get; set; }
        public bool CloseEarly { get; set; }
        public string OrderNo { get; set; }
        public string FileName { get; set; }
        public int RouteID { get; set; }
        public string Route { get; set; }
        public int DispatchedByUserID { get; set; }
        public Nullable<System.DateTime> DispatchedDT { get; set; }
        public int ZoneID { get; set; }
        public int ForwardedByUserId { get; set; }
        public Nullable<System.DateTime> ClearenceDT { get; set; }
        public int ClearedByUserID { get; set; }
        public Nullable<System.DateTime> TimeSlotFromDT { get; set; }
        public Nullable<System.DateTime> TimeSlotToDT { get; set; }
        public Nullable<System.DateTime> VehicleReadyDT { get; set; }
        public string VehicleNumber { get; set; }
        public int VehReadyByUserID { get; set; }
        public Nullable<System.DateTime> VehicleEntryDT { get; set; }
        public int LoadingByUserID { get; set; }
        public int ReadyByTransporterUserID { get; set; }
        public int QuoteTypeID { get; set; }
        public int PaymentTerm { get; set; }
        public string FixedTerm { get; set; }
        public Nullable<System.DateTime> ReminderSentDT { get; set; }
        public decimal Budget { get; set; }
        public int ShipmentID { get; set; }
        public Nullable<System.DateTime> StartWorkDT { get; set; }
        public int StartedByUserID { get; set; }
        public Nullable<System.DateTime> CompletionDT { get; set; }
        public int CompletionByUserID { get; set; }
    }
}
