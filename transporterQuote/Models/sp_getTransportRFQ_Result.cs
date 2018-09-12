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
    
    public partial class sp_getTransportRFQ_Result
    {
        public int shipmentID { get; set; }
        public int rfqID { get; set; }
        public int quoteID { get; set; }
        public System.DateTime pickupDate { get; set; }
        public string product { get; set; }
        public string sourceAddress { get; set; }
        public string sourceLocation { get; set; }
        public string destinationAddress { get; set; }
        public string emptyContainer { get; set; }
        public int containers { get; set; }
        public string portofDischarge { get; set; }
        public string portodLoading { get; set; }
        public System.DateTime deliveryDate { get; set; }
        public string detail { get; set; }
        public int executiveID { get; set; }
        public string fixedTerm { get; set; }
        public string fileName { get; set; }
        public int customerID { get; set; }
        public string orderNo { get; set; }
        public string status { get; set; }
        public string transportCompany { get; set; }
        public string customerCompany { get; set; }
        public string components { get; set; }
        public string paymentTerms { get; set; }
        public System.DateTime quoteBy { get; set; }
        public int loadingbyUserID { get; set; }
        public int clearedByUserID { get; set; }
        public Nullable<System.DateTime> clearenceDT { get; set; }
        public Nullable<System.DateTime> timeFromDT { get; set; }
        public Nullable<System.DateTime> timeToDT { get; set; }
        public int vehReadyByUserID { get; set; }
        public Nullable<System.DateTime> vehicleEntryDT { get; set; }
        public string vehicleNumber { get; set; }
        public Nullable<System.DateTime> vehicleReadyDT { get; set; }
        public Nullable<System.DateTime> dispatchedDT { get; set; }
        public int dispatchedByUserID { get; set; }
        public string transporterDetail { get; set; }
        public string response { get; set; }
        public string createdBy { get; set; }
        public string vendorName { get; set; }
        public decimal exchangeRate { get; set; }
    }
}