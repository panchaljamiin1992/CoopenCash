using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using transporterQuote.Services;
using transporterQuote.Models;
using System.Data;
using System.Web.Configuration;
using static transporterQuote.API.reqForQuoteAPIController;

namespace transporterQuote.API
{
    [RoutePrefix("api/customer")]
    public class customerApiController : ApiController
    {
        #region "Functions"

        /*
         - getRFQ()
         - Purpose: Get RFQ list with quatation.
         - In: class myParams {
                         link
                     }
         - Out: customerObj
       */
        [HttpPost, HttpGet, Route("get")]
        public dynamic verifyCustomerURL(dynamic myParams)
        {
            // #region "Validations"

            if (myParams == null || myParams.link == "")
            {
                return new jResponse();
            }

            // Extract parameters
            genApiController genApi = new genApiController();
            string link = myParams.link;
            int RFQID = 0; //Int32.Parse(parameters[0]);
            int customerID = 0; // Int32.Parse(parameters[1]);
            int isCustomer = 0;
            
            List<string> parameters = new List<string>();
            if (link.ToLower().Contains('~'))
            {
                parameters = genApi.splitString('~', link);
                //var a = parameters[0];
                parameters[0] = parameters[0].Remove(parameters[0].Length - 3, 3);
                parameters[1] = parameters[1].Remove(parameters[1].Length - 3, 3);

                RFQID = Int32.Parse(parameters[0]) / 2018;
                parameters[0] = RFQID.ToString();
                customerID = Int32.Parse(parameters[1]) / 2018;
                parameters[1] = customerID.ToString();
                isCustomer = Int32.Parse(parameters[2]) / 2018;
            }
            else
            {
                string decodeURL = genApiController.decryptURL(link, WebConfigurationManager.AppSettings["TransporterKey"]);
                parameters = genApi.splitString('~', decodeURL);
                RFQID = Int32.Parse(parameters[0]);
                customerID = Int32.Parse(parameters[1]);
            }
            

            DateTime currentDT = genApiController.getDate();
            customerInfo customerObj = new customerInfo();
            customerObj.quoteDetail = new List<quoteObj>();
            rfqDetail objDetail = new rfqDetail();

            // #endregion

            // Verify and get request with quataion
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // #region "Validate parameters"

                // Get customer info

                if (isCustomer == 1)
                {
                    var dbCustomer = (from dbc in db.Customers
                                      where dbc.CustomerID == customerID && !dbc.IsDeleted
                                      select dbc).FirstOrDefault();

                    // Verify customer info
                    if (dbCustomer == null)
                    {
                        return new jResponse(true, "Something looks off about your access.  Please contact the system administrator.", null);
                    }

                    // Get request info
                    objDetail = (from dbr in db.RFQs
                                  join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                  // join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                  join dbq in db.Quotations
                                  on new { dbr.RequestForQuoteID, isRFQDeleted = false }
                                  equals new { dbq.RequestForQuoteID, isRFQDeleted = dbq.IsDeleted } into dq
                                  //join dbfv in db.RFQFieldValues on dbr.RequestForQuoteID equals dbfv.RFQID 
                                  //join dbf in db.Fields on dbfv.FieldID equals dbf.FieldID
                                  where dbr.RequestForQuoteID == RFQID && (dbr.CustomerID == customerID) && !dbr.IsDeleted
                                  select new rfqDetail
                                  {
                                      //  sourceCity = dbr.Source,
                                      //  destinationCity = dbr.Destination,
                                      //  product = dbr.Product,
                                      detail = dbr.Details,
                                      requestForQuoteID = dbr.RequestForQuoteID,
                                      pickUp = dbr.PickUp,
                                      deliveryBy = dbr.DeliveryBy,
                                      quoteBy = dbr.QuoteBy,
                                      status = dbr.Status,
                                      //allowLaterDelivery = dbr.AllowLaterDelivery,
                                      isCloseEarly = dbr.CloseEarly,
                                      quotes = dq,
                                      // sourceAddress = dbs.Location +", "+ dbs.Address1 + " " + dbs.Address2,
                                      destinationAddress = dbc.CompanyName, //+", " + dbc.Address1 + " " + dbc.Address2,
                                      // sourceState = dbs.State,
                                      destinationState = dbc.State,
                                      orderNo = dbr.OrderNo,
                                      paymentTerm = dbr.PaymentTerm,
                                      quoteTypeID = dbr.QuoteTypeID,
                                      fixedTerm = dbr.FixedTerm,
                                      budget = dbr.Budget,
                                      shipmentID = dbr.ShipmentID,
                                      fields = (from dbfv in db.RFQFieldValues
                                                join dbf in db.Fields on dbfv.FieldID equals dbf.FieldID
                                                where dbfv.RFQID == dbr.RequestForQuoteID
                                                select new customerApiController.fieldObj
                                                {
                                                    FieldID = dbf.FieldID,
                                                    fieldName = dbf.FieldName,
                                                    fieldValue = dbfv.FieldValue,
                                                    showtoCustomers = dbf.ShowCustomer,
                                                    showtoVendors = dbf.ShowVendors
                                                }).AsEnumerable()
                                  }).FirstOrDefault();
                    customerObj.isCustomer = 1;
                } else
                {
                    var dbExecutive = (from dbe in db.Executives
                                       where dbe.ExecutiveID == customerID && dbe.CanSelectForCustomer == true && !dbe.IsDeleted
                                       select dbe).FirstOrDefault();

                    // Verify customer info
                    if (dbExecutive == null)
                    {
                        return new jResponse(true, "Something looks off about your access.  Please contact the system administrator.", null);
                    }

                    // Get request info
                    objDetail = (from dbr in db.RFQs
                                     // join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 join dbc in db.Customers 
                                 on new { dbr.CustomerID, isCustDeleted = false }
                                 equals new { dbc.CustomerID, isCustDeleted = dbc.IsDeleted } into tbc
                                 from rbc in tbc.DefaultIfEmpty()
                                     // join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                 join dbq in db.Quotations
                                 on new { dbr.RequestForQuoteID, isRFQDeleted = false }
                                 equals new { dbq.RequestForQuoteID, isRFQDeleted = dbq.IsDeleted } into dq
                                 //join dbfv in db.RFQFieldValues on dbr.RequestForQuoteID equals dbfv.RFQID 
                                 //join dbf in db.Fields on dbfv.FieldID equals dbf.FieldID
                                 where dbr.RequestForQuoteID == RFQID && !dbr.IsDeleted
                                 select new rfqDetail
                                 {
                                     //  sourceCity = dbr.Source,
                                     //  destinationCity = dbr.Destination,
                                     //  product = dbr.Product,
                                     detail = dbr.Details,
                                     requestForQuoteID = dbr.RequestForQuoteID,
                                     pickUp = dbr.PickUp,
                                     deliveryBy = dbr.DeliveryBy,
                                     quoteBy = dbr.QuoteBy,
                                     status = dbr.Status,
                                     //allowLaterDelivery = dbr.AllowLaterDelivery,
                                     isCloseEarly = dbr.CloseEarly,
                                     quotes = dq,
                                     // sourceAddress = dbs.Location +", "+ dbs.Address1 + " " + dbs.Address2,
                                     destinationAddress = rbc.CompanyName, //+ ", " + dbc.Address1 + " " + dbc.Address2,
                                     // sourceState = dbs.State,
                                     destinationState = rbc.State,
                                     orderNo = dbr.OrderNo,
                                     quoteTypeID = dbr.QuoteTypeID,
                                     paymentTerm = dbr.PaymentTerm,
                                     fixedTerm = dbr.FixedTerm,
                                     budget = dbr.Budget,
                                     shipmentID = dbr.ShipmentID,
                                     fields = (from dbfv in db.RFQFieldValues
                                               join dbf in db.Fields on dbfv.FieldID equals dbf.FieldID
                                               where dbfv.RFQID == dbr.RequestForQuoteID
                                               select new customerApiController.fieldObj
                                               {
                                                   fieldName = dbf.FieldName,
                                                   fieldValue = dbfv.FieldValue,
                                                   showtoCustomers = dbf.ShowCustomer,
                                                   showtoVendors = dbf.ShowVendors
                                               }).AsEnumerable()
                                 }).FirstOrDefault();

                    customerObj.isCustomer = 0;
                }
               

                // Verify request
                if (objDetail == null)
                {
                    return new jResponse(true, "RFQ does not exist.", null);
                }
                else
                {
                    foreach (var i in objDetail.quotes)
                    {
                        var dbQuote = (from dbt in db.Transporters
                                       where dbt.TransporterID == i.TransporterID && !dbt.IsDeleted
                                       select new quoteObj
                                       {
                                           charges = i.Charges,
                                           paymentTerms = i.PaymentTerms,
                                           quoteStatus = i.Status,
                                           notes = i.Notes,
                                           transporterID = i.TransporterID,
                                           transporterName = dbt.Title + " " + dbt.FirstName + " " + dbt.LastName,
                                           companyName = dbt.CompanyName,
                                           mobile = dbt.Mobile,
                                           email = dbt.Email,
                                           status = i.Status,
                                           quoteID = i.QuoteID,
                                           extendDeliveryDT = i.ExtendDeliveryDT,
                                           components = i.Components,
                                           exchangeRate = i.ExchangeRate,
                                           createdDT = i.CreatedDT,
                                           acceptNote = i.AcceptNote,
                                           roundNumber = i.RoundNumber,
                                           negotiationComments = i.NegotiationComments
                                       }).FirstOrDefault();

                        customerObj.quoteDetail.Add(dbQuote);

                    }
                }
                objDetail.quotes = null;
                customerObj.quoteDetail.OrderBy(i => i.charges).ToList();
                customerObj.requestDetail = objDetail;
                customerObj.customerID = customerID;
                customerObj.RFQID = RFQID;
                return new jResponse(false, "", customerObj);

                // #endregion
            }


        }

        /*
        - reqCloseEarly()
        - Purpose: request close.
        - In: class myParams {
                   userID,
                   reqForQuoteID,
           }
        - Out: 
     */
        [HttpPost, HttpGet, Route("req/closeEarly")]
        public dynamic reqCloseEarly(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.customerID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Extract parameter
            int customerID = myParams.customerID;
            int reqForQuoteID = myParams.reqForQuoteID;
            bool isCloseEarly = true;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // #endregion

            // RFQ close
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                                    .FirstOrDefault();

                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (reqForQuoteInfo == null || reqForQuoteInfo.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                reqForQuoteInfo.CloseEarly = isCloseEarly;
                reqForQuoteInfo.UpdatedDT = currentDT;

                db.Entry(reqForQuoteInfo).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Your quote request has been close.", reqForQuoteID);
            }
        }

        /*
         - acceptQuote()
         - Purpose: Accept an quote resposne.
         - In: class myParams {
                    userID,
                    reqForQuoteID,
                    quoteID,
                    transporterID,
                    isCustomer,
                    note
            }
         - Out: Success/failure
      */
        [HttpPost, HttpGet, Route("quote/accept")]
        public dynamic acceptQuote(dynamic myParams)
        {
            // #region "Validations"

            if (myParams == null || myParams.reqForQuoteID < 1
                || myParams.quoteID < 1)
            {
                return new jResponse();
            }

            // Extract parameter
            int customerID = myParams.customerID;
            int reqForQuoteID = myParams.reqForQuoteID;
            int quoteID = myParams.quoteID;
            int executiveID = 0;
            int isCustomer = myParams.isCustomer;
            string showHistory = "true";
            string note = myParams.note;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();
            Quotations quoteInfo = new Quotations();

            // #endregion

            // #region "Quote accept"

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // #region "More validation"

                var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                                    .FirstOrDefault();


                // Check if inquiryInfo is null or inquiryID same or not
                if (reqForQuoteInfo == null || reqForQuoteInfo.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                // #endregion

                // #region "Update status"

                //rfqInfo.ExecutiveID = quoteInfo.ExecutiveID;
                reqForQuoteInfo.Status = "1";
                reqForQuoteInfo.UpdatedDT = currentDT;

                db.Entry(reqForQuoteInfo).State = EntityState.Modified;

                var quoteDetails = db.Quotations.Where(i => i.QuoteID == quoteID && !i.IsDeleted)
                                  .FirstOrDefault();

                quoteDetails.Status = "1";
                quoteDetails.AcceptID = customerID;
                quoteDetails.AcceptNote = note;                
                if (isCustomer == 1)
                {
                    quoteDetails.IsAcceptedByExeOrCust = true; // true - customer and false - executive.
                } else
                {
                    quoteDetails.IsAcceptedByExeOrCust = false;
                }
               
                quoteDetails.AcceptanceStatusOn = currentDT;
                quoteDetails.UpdatedDT = currentDT;

                db.Entry(quoteDetails).State = EntityState.Modified;

                db.SaveChanges();

                var shipmentDetail = db.sp_MapShipFields(quoteID, customerID, reqForQuoteInfo.ShipmentID).FirstOrDefault();

                if (shipmentDetail == null)
                {
                    return new jResponse(false, "Shipment can't added successfully.", true);
                }

                // Send mail to transporter

                var transporterDetail = db.Transporters.Where(i => i.TransporterID == quoteDetails.TransporterID && !i.IsDeleted).FirstOrDefault();

                if (transporterDetail == null)
                {
                    return new jResponse(false, "Transporter does not exist.", true);
                }

                string sendEmail = WebConfigurationManager.AppSettings["SendEmails"];
                if (sendEmail.ToLower() == "true")
                {
                    dynamic customerDetail = null;

                    if (customerID > 0) {
                        customerDetail = db.Customers.Where(i => i.CustomerID == customerID && !i.IsDeleted).FirstOrDefault();

                        if (customerDetail == null)
                        {
                            return new jResponse(false, "Customer does not exist.", true);
                        }
                    }

                    Random rng2 = new Random();
                    int value = rng2.Next(1000);
                    string text = value.ToString("000");

                    string parameters = "" + transporterDetail.TransporterID * 2018 + text + "~" + reqForQuoteID * 2018 + text + "~" + executiveID.ToString()
                                               + "~" + customerID.ToString() +"~" + showHistory;

                    //   string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                    //string encodedURL = genApiController.getEncryptedPW(parameters);
                    List<string> emailList = new List<string>();
                    emailList.Add(transporterDetail.Email);
                    emailParams emailToTransporterParams = new emailParams();
                    emailToTransporterParams.rfqID = reqForQuoteInfo.RequestForQuoteID.ToString();
                    emailToTransporterParams.quoteID = quoteID.ToString();
                    emailToTransporterParams.companyName = customerDetail.CompanyName;
                    emailToTransporterParams.pickup = reqForQuoteInfo.PickUp.ToString("d-MMM-yy");
                    emailToTransporterParams.product = reqForQuoteInfo.Product.ToString();
                    emailToTransporterParams.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                    emailToTransporterParams.delivery = quoteDetails.ExtendDeliveryDT == null ? reqForQuoteInfo.DeliveryBy.ToString("d-MMM-yy") : quoteDetails.ExtendDeliveryDT.GetValueOrDefault().ToString("d-MMM-yy");
                    emailAPIController.quot_accept(emailList, emailToTransporterParams);

                    // SMS send
                    smsToTransporterParams smsObj = new smsToTransporterParams();
                    smsObj.rfqID = reqForQuoteInfo.RequestForQuoteID.ToString();
                    smsObj.customerName = customerDetail.CompanyName;

                    List<string> TranspoterMobileList = new List<string>();
                    TranspoterMobileList.Add(transporterDetail.Mobile);

                    if (transporterDetail.Mobile != null || transporterDetail.Mobile != "")
                    {
                        smsApiController.send_acptToTransporter(TranspoterMobileList, smsObj);
                    }
                }

                return new jResponse(false, "Quote marked as accepted.", true);

                // #endregion
            }

            // #endregion
        }

        /*
          - updateReqForQuote()
          - Purpose: Add transporter.
          - In: class myParams {
                          CustomerID,
                          SourceID,
                          Destination,
                          Source,
                          Product,
                          PickUp,
                          DeliveryBy,
                          AllowLaterDelivery,
                          QuoteBy,
                          Details,
                          Status,
                          Transporters,
                          orderNo,
                          fileName,
                          dynamicFields,
                          budget,
                          shipmentID
                      }
          - Out: Success/Failure 
        */
        [HttpPost, HttpGet, Route("req/negotiation")]
        public dynamic updateReqForQuote(dynamic myParams)
        {
            // #region "Validations"

            if (myParams == null || myParams.executiveID < 1
                || myParams.quoteBy == "" || myParams.details == "")
            {
                return new jResponse();
            }

            // Extract parameters
            int executiveID = Convert.ToInt32(myParams.executiveID);
            int customerID = Convert.ToInt32(myParams.customerID);
            string destination = myParams.destination;
            string product = myParams.product;
            string orderNo = myParams.orderNo;
            string fileName = myParams.fileName;
            DateTime pickUp = myParams.pickUpDT;
            DateTime deliveryBy = myParams.deliveryDT;
            DateTime quoteBy = myParams.quoteBy;
            string details = myParams.details;
            string transpoters = myParams.transpoters;
            string status = myParams.status;
            int insertedReqID = myParams.RFQID;
            int quoteTypeID = myParams.quoteTypeID;
            int paymentTerms = myParams.paymentTerms;
            string fixedTerm = myParams.fixedTerm;
            dynamic dynamicFields = myParams.dynamicFields;
            decimal budget = myParams.budget;
            int shipmentID = myParams.shipmentID;
            DateTime currentDT = genApiController.getDate();
            genApiController gen = new genApiController();
            string showHistory = "false";
            string negotiationComments = myParams.negotiationComments;
           
            // Add new transporter
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                
                smsParams smsObj = new smsParams();
                
                emailParams emailToTranspoterParams = new emailParams();

                emailToTranspoterParams.rfqID = insertedReqID.ToString();
                emailToTranspoterParams.product = product;
                                                         // emailToTranspoterParams.destination = ckeckCustomer.CompanyName +", "+ ckeckCustomer.Address1 + "" + (ckeckCustomer.Address2 != "" ? ", " : "") + ckeckCustomer.Address2 + ", " + ckeckCustomer.City + " (" + ckeckCustomer.State + ")";
                emailToTranspoterParams.pickup = pickUp.ToString("d-MMM-yy");
                emailToTranspoterParams.delivery = deliveryBy.ToString("d-MMM-yy");
                emailToTranspoterParams.quoteby = quoteBy.ToString("d-MMM-yy");
                emailToTranspoterParams.emailDynamicFields = dynamicFields;

                List<int> TranspoterIds = gen.splitString('~', transpoters).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                List<string> negotiationCommentsList = gen.splitString('~', negotiationComments).Where(x => !string.IsNullOrEmpty(x)).ToList();
                var TranspoterInfo = (from dbt in db.Transporters
                                        where TranspoterIds.Contains(dbt.TransporterID)
                                        select new transpoterDetails
                                        {
                                            transpoterID = dbt.TransporterID,
                                            email = dbt.Email,
                                            fName = dbt.FirstName,
                                            lName = dbt.LastName,
                                            title = dbt.Title,
                                            companyName = dbt.CompanyName,
                                            mobile = dbt.Mobile,
                                            alternateMobile = dbt.AlternateMobile
                                        }).ToList();

                int j = 0;
                foreach (var i in TranspoterInfo)
                {
                    Random rng1 = new Random();
                    Random rng2 = new Random();
                    int value = rng1.Next(1000);
                    int value1 = rng2.Next(1000);
                    string text = value.ToString("000");
                    string text1 = value1.ToString("000");

                    string parameters = "" + i.transpoterID * 2018 + text + "~" + insertedReqID * 2018 + text1 + "~" + executiveID.ToString()
                                        + "~" + customerID.ToString() + "~" + showHistory;



                    List<string> TranspoterList = new List<string>();
                    TranspoterList.Add(i.email);
                    emailToTranspoterParams.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                    int length = i.companyName.Length;
                    emailToTranspoterParams.displayName = i.companyName;
                    emailToTranspoterParams.negotiationComments = negotiationCommentsList[j];
                    //emailToTranspoterParams.responseLink = genApiController.encryptURL(emailToTranspoterParams.responseLink, (WebConfigurationManager.AppSettings["TranspoterKey"]));

                    emailAPIController.transpoterNegotiationQuote(TranspoterList, emailToTranspoterParams);

                    smsObj.rfqID = insertedReqID.ToString();
                    smsObj.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                    smsObj.sourceCity = "";//ckeckSource.City;
                                            //  smsObj.destinationCity = ckeckCustomer.City;
                    List<string> TranspoterMobileList = new List<string>();
                    TranspoterMobileList.Add(i.mobile);
                    TranspoterMobileList.Add(i.alternateMobile);

                    smsApiController.send_quoteNegotiation(TranspoterMobileList, smsObj);

                    //Update NegotiationComments into 'Quotations' table
                    Quotations quotationsInfo;
                    try
                    {
                        int maxRoundNumber = db.Quotations.Where(r => r.TransporterID == i.transpoterID && r.RequestForQuoteID == insertedReqID)
                                               .Max(r => r.RoundNumber);
                        quotationsInfo = db.Quotations.Where(r => r.TransporterID == i.transpoterID && r.RequestForQuoteID == insertedReqID
                                                                && r.RoundNumber == maxRoundNumber).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        quotationsInfo = db.Quotations.Where(r => r.TransporterID == i.transpoterID && r.RequestForQuoteID == insertedReqID).FirstOrDefault();
                    }
                    quotationsInfo.NegotiationComments = negotiationCommentsList[j++];
                    db.Entry(quotationsInfo).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return new jResponse(false, "Negotiation requests sent.", insertedReqID);
        }


        /*
         - extendQuoteBy()
         - Purpose: Extend quoteby date.
         - In: class myParams {
                    userID,
                    reqForQuoteID,
                    quoteByDT
            }
         - Out: Success/failure
      */
        [HttpPost, HttpGet, Route("extend/QuoteBy")]
        public dynamic extendQuoteBy(dynamic myParams)
        {

            // #region "Validations"

            if (myParams == null || myParams.customerID < 1 || myParams.reqForQuoteID < 1 || myParams.quoteBy == null)
            {
                return new jResponse();
            }

            // Extract parameter
            int customerID = myParams.customerID;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime quoteBy = myParams.quoteBy;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            if (currentDT > quoteBy)
            {
                return new jResponse(true, "Plase select valid date.", null);
            }

            // #endregion

            // #region "Extend quote by date"

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                                    .FirstOrDefault();

                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (reqForQuoteInfo == null || reqForQuoteInfo.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                reqForQuoteInfo.QuoteBy = quoteBy;
                reqForQuoteInfo.UpdatedDT = currentDT;

                db.Entry(reqForQuoteInfo).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "QuoteBy date extended.", true);
            }

            // #endregion

        }

        #endregion

        #region "Classes"

        public class fieldObj
        {
            public int FieldID { get; set; }
            public string fieldName { get; set; }
            public string fieldValue { get; set; }
            public bool showtoCustomers { get; set; }
            public bool showtoVendors { get; set; }
        }

        public class customerInfo
        {
            public dynamic requestDetail { get; set; }
            public List<quoteObj> quoteDetail { get; set; }
            public int customerID { get; set; }
            public int RFQID { get; set; }
            public int isCustomer { get; set; }
        }

        public class rfqDetail
        {
            public string sourceCity { get; set; }
            public string destinationCity { get; set; }
            public string product { get; set; }
            public string detail { get; set; }
            public int requestForQuoteID { get; set; }
            public DateTime pickUp { get; set; }
            public DateTime deliveryBy { get; set; }
            public DateTime quoteBy { get; set; }
            public string status { get; set; }
            public bool allowLaterDelivery { get; set; }
            public IEnumerable<Quotations> quotes { get; set; }
            public bool isCloseEarly { get; set; }
            public string sourceAddress { get; set; }
            public string destinationAddress { get; set; }
            public string sourceState { get; set; }
            public string destinationState { get; set; }
            public IEnumerable<customerApiController.fieldObj> fields { get; set; }
            public string orderNo { get; set; }
            public int quoteTypeID { get; set; }
            public int paymentTerm { get; set; }
            public string fixedTerm { get; set; }
            public decimal budget { get; set; }
            public int shipmentID { get; set; }
        }

        public class smsToTransporterParams
        {
            public string rfqID { get; set; }
            public string customerName { get; set; }
        }

        public class quoteObj
        {
            public int quoteID { get; set; }
            public decimal charges { get; set; }
            public string paymentTerms { get; set; }
            public string quoteStatus { get; set; }
            public string notes { get; set; }
            public int transporterID { get; set; }
            public string transporterName { get; set; }
            public string companyName { get; set; }
            public string mobile { get; set; }
            public string email { get; set; }
            public string status { get; set; }
            public DateTime? extendDeliveryDT { get; set; }
            public string components { get; set; }
            public decimal exchangeRate { get; set; }
            public DateTime createdDT { get; set; }
            public string acceptNote { get; set; }
            public int? roundNumber { get; set; }
            public string negotiationComments { get; set; }
        }

        public class emailParams
        {
            public string rfqID { get; set; }
            public string quoteID { get; set; } 
            public string pickup { get; set; }
            public string delivery { get; set; }
            public string companyName { get; set; }
            public string product { get; set; } 
            public string link { get; set; }
            public dynamic emailDynamicFields { get; set; }
            public string displayName { get; set; }
            public string quoteby { get; set; }
            public string negotiationComments { get; set; }
        }

        #endregion

    }
}