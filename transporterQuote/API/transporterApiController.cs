using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using transporterQuote.Models;
using transporterQuote.Helper;
using System.Data;
using transporterQuote.Services;
using System.Web.Configuration;

namespace transporterQuote.API
{
    [RoutePrefix("api/transporter")]
    public class transporterApiController : ApiController
    {
        #region "Functions"

        /*
           - verifyTransporterURL()
           - Purpose: Verify transporter URL.
           - In: class myParams {
                           link
                       }
           - Out: Success / failure
       */
        [HttpPost, HttpGet, Route("verify/transporterURL")]
        public dynamic verifyTransporterURL(dynamic myParams)
        {
            // #region "Validations"

            // Basic validations
            if (myParams == null || myParams.link == "")
            {
                return new jResponse();
            }

            // Extract parameters
            genApiController genApi = new genApiController();
            string link = myParams.link;
            int trasporterID = 0;
            int RFQID = 0;
            // string decodeURL = genApiController.decryptURL(link, WebConfigurationManager.AppSettings["TransporterKey"]);
            // string decodeURL = genApiController.getEncryptedPW(link);
            //string decodeURL = genApiController.encryptParam(link);
            List<string> parameters = new List<string>();
            if (link.ToLower().Contains('~'))
            {
                parameters = genApi.splitString('~', link);
                //var a = parameters[0];
                parameters[0] = parameters[0].Remove(parameters[0].Length - 3,3);
                parameters[1] = parameters[1].Remove(parameters[1].Length - 3, 3);
              
                trasporterID = Int32.Parse(parameters[0]) / 2018;
                parameters[0] = trasporterID.ToString();
                RFQID = Int32.Parse(parameters[1]) / 2018;
                parameters[1] = RFQID.ToString();
            } else
            {
                string decodeURL = genApiController.getEncryptedPW(link);
                parameters = genApi.splitString('~', decodeURL);
                trasporterID = Int32.Parse(parameters[0]);
                RFQID = Int32.Parse(parameters[1]);
            }
            
            
            DateTime currentDT = genApiController.getDate();

            getRFQs requestDetail = new getRFQs();
            requestDetail.quoteAns = false;
            // #endregion

            // Verify and get request list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // #region "Validate parameters"

                var dbtInfo = (from dbt in db.Transporters
                               where dbt.TransporterID == trasporterID && !dbt.IsDeleted
                               select dbt).FirstOrDefault();

                // Verify transporter
                if (dbtInfo == null)
                {
                    return new jResponse(true, "Something looks off about your access.  Please contact the system administrator.", null);
                }

                //var dbtrInfo = (from dbr in db.RFQs
                //                join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                //                join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                //                join dbqtc in db.QuoteTypeComponents on new {dbr.QuoteTypeID, isComponent = false }
                //                equals new {dbqtc.QuoteTypeID, isComponent = dbqtc.IsDeleted} into tqc
                //                //from rqc in tqc.DefaultIfEmpty()
                //                where dbr.RequestForQuoteID == RFQID && !dbr.IsDeleted
                //                select new rfqDetails
                //                {
                //                    reqForQuoteID = dbr.RequestForQuoteID,
                //                    customerID = dbr.CustomerID,
                //                    customerName = dbc.CompanyName,
                //                    sourceName = dbr.Source,
                //                    destination = dbr.Destination,
                //                    pickUpDT = dbr.PickUp,
                //                    deliveryDT = dbr.DeliveryBy,
                //                    quoteByDT = dbr.QuoteBy,
                //                    product = dbr.Product,
                //                    details = dbr.Details,
                //                    allowLaterDelivery = dbr.AllowLaterDelivery,
                //                    status = dbr.Status,
                //                    sourceAddress = dbs.Location + ", " + dbs.Address1 + " " + dbs.Address2,
                //                    destinationAddress = dbc.CompanyName + ", " + dbc.Address1 + " " + dbc.Address2,
                //                    isCloseEarly = dbr.CloseEarly,
                //                    sourceState = dbs.State,
                //                    destinationState = dbc.State,
                //                    orderNo = dbr.OrderNo,
                //                    routeName = dbr.Route,
                //                    fileName = dbr.FileName,
                //                    components = tqc,
                //                    paymentTerm = dbr.PaymentTerm,
                //                    fixedTerm = dbr.FixedTerm
                //                }).FirstOrDefault();

                var dbtrInfo = db.getTransporterQuoteDetail(RFQID).FirstOrDefault();

                // Verify request
                if (dbtrInfo == null)
                {
                    return new jResponse(true, "Request does not exist.", null);
                }

                Quotations dbQuoteInfo;
                int maxRoundNumber = 1;
                try
                {
                    maxRoundNumber = db.Quotations.Where(r => r.TransporterID == trasporterID && r.RequestForQuoteID == RFQID)
                                               .Max(r => r.RoundNumber);

                    dbQuoteInfo = (from dbq in db.Quotations
                                       where dbq.RequestForQuoteID == RFQID && dbq.TransporterID == trasporterID && dbq.RoundNumber == maxRoundNumber
                                       select dbq).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    dbQuoteInfo = (from dbq in db.Quotations
                                       where dbq.RequestForQuoteID == RFQID && dbq.TransporterID == trasporterID
                                       select dbq).FirstOrDefault();
                }

                string canChangeQuote = "canChangeQuote"; 
                if (dbQuoteInfo != null && dbQuoteInfo.NegotiationComments != null && dbQuoteInfo.NegotiationComments == "")
                {
                    IQueryable<Quotations> allQuotationsForRFQ = db.Quotations.Where(r => r.RequestForQuoteID == RFQID && r.RoundNumber == maxRoundNumber);
                    foreach (Quotations quote in allQuotationsForRFQ)
                    {
                        if (quote.NegotiationComments != "")
                        {
                            canChangeQuote = "canNotChangeQuote";
                            break;
                        }
                    }
                }

                parameters.Add(canChangeQuote);
                    
                if (dbQuoteInfo != null)
                {
                    requestDetail.quoteAns = true;
                }

                // #endregion

                //requestList.RFQList = db.sp_getRequest(trasporterID, 0).ToList(); //trasporterID

                requestDetail.quoteObj = dbQuoteInfo;
                requestDetail.parameter = parameters;
                requestDetail.RFQobj = dbtrInfo;

                return new jResponse(false, "", requestDetail);
            }
        }

        /*
           - setQuote()
           - Purpose: Set quote.
           - In: class myParams {
                           transporterID,
                           RFQID,
                           customerID,
                           charges,
                           paymentTerms,
                           notes
                       }
           - Out: insertedQuoteID 
        */
        [HttpPost, HttpGet, Route("quote/set")]
        public dynamic setQuote(dynamic myParams)
        {
            // #region "Validations"

            if (myParams == null || myParams.transporterID < 1 || myParams.RFQID < 1 || myParams.charges == null
                 || myParams.components == null || myParams.components == "")
            {
                return new jResponse();
            }

            // Extract parameters
            int transporterID = myParams.transporterID;
            int RFQID = myParams.RFQID;
            int customerID = myParams.customerID;
            string paymentTerms = myParams.paymentTerms;
            string notes = myParams.notes;
            string components = myParams.components;
            int quoteID = myParams.quoteID;
            // DateTime extendDeliveryDT = myParams.deliveryDT;
            DateTime currentDT = genApiController.getDate();
            var insertedQuoteID = 0;
            decimal exchangeRate = myParams.exchangeRate;
            string negotiationComments = myParams.NegotiationComments;
            decimal charges = myParams.charges;
            // #endregion

            // Add quotation
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Get transporter detail 
                var dbTransporterInfo = (from dbt in db.Transporters
                                         where dbt.TransporterID == transporterID && !dbt.IsDeleted
                                         select new
                                         {
                                             dbt
                                         }).FirstOrDefault();

                // Check for transporter
                if (dbTransporterInfo == null)
                {
                    return new jResponse(true, "Something looks off about your access.  Please contact the system administrator.", null);
                }

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                 //join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                 join dbe in db.Executives on dbr.ExecutiveID equals dbe.ExecutiveID
                                // join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 where dbr.RequestForQuoteID == RFQID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr,
                                   //  dbs,
                                   //  dbc,
                                     dbe
                                 }).FirstOrDefault();

                // Check for RFQ
                if (dbRFQInfo == null)
                {
                    return new jResponse(true, "This request for quote does not exist.", null);
                }

                var quotes = (from dbq in db.Quotations
                           where dbq.TransporterID == transporterID && dbq.RequestForQuoteID == RFQID && dbq.IsDeleted == false
                           select dbq).ToList();
                int roundNumber = quotes.Count() + 1;

                if (quoteID == 0 || negotiationComments != "") { 

                    // Add quote
                    Quotations addQuote = new Quotations();
                    addQuote.RequestForQuoteID = RFQID;
                    addQuote.TransporterID = transporterID;
                    addQuote.PaymentTerms = paymentTerms;
                    addQuote.Status = "0";
                    addQuote.Notes = notes;
                    addQuote.ExtendDeliveryDT = null;
                    addQuote.CreatedDT = currentDT;
                    addQuote.UpdatedDT = currentDT;
                    addQuote.AcceptanceStatusOn = currentDT;
                    addQuote.AcceptID = 0;
                    addQuote.IsDeleted = false;
                    addQuote.Components = components;
                    addQuote.ExchangeRate = exchangeRate;
                    addQuote.AcceptNote = "";
                    addQuote.RoundNumber = roundNumber;
                    addQuote.NegotiationComments = "";
                    addQuote.Charges = charges;

                    db.Entry(addQuote).State = EntityState.Added;
                    db.SaveChanges();
                    insertedQuoteID = addQuote.QuoteID;

                } else
                {
                    // Get transporter detail 
                    var dbQuote= (from dbq in db.Quotations
                                             where dbq.QuoteID == quoteID && !dbq.IsDeleted
                                             select new
                                             {
                                                 dbq
                                             }).FirstOrDefault();

                    // Check quote available or not
                    if (dbQuote == null)
                    {
                        return new jResponse(true, "This quote does not exist.", null);
                    }

                    dbQuote.dbq.Components = components;
                    dbQuote.dbq.ExchangeRate = exchangeRate;

                    db.Entry(dbQuote.dbq).State = EntityState.Modified;
                    db.SaveChanges();

                }

                //var dbcInfo = (from dbc in db.Customers
                //               where dbc.CustomerID == customerID && !dbc.IsDeleted
                //               select new
                //               {
                //                   dbc
                //               }).FirstOrDefault();

                string sendEmail = WebConfigurationManager.AppSettings["SendEmails"];
               // string sendEmail = "false";
                if (sendEmail.ToLower() == "true")
                {
                    emailDetail emailToTransporterParams = new emailDetail();
                   
                    //string parameters = "" + RFQID.ToString() + "~" + dbRFQInfo.dbr.CustomerID.ToString();
                   // string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                    List<string> emailList = new List<string>();
                    emailList.Add(dbRFQInfo.dbe.Email);
                    emailToTransporterParams.rfqID = dbRFQInfo.dbr.RequestForQuoteID.ToString();
                    emailToTransporterParams.companyName = dbTransporterInfo.dbt.CompanyName;
                    emailAPIController.quot_Received(emailList, emailToTransporterParams);
                }

                smsParams smsObj = new smsParams();
                if (customerID > 0)
                {
                    var dbCustomer = db.Customers.Where(i => i.CustomerID == dbRFQInfo.dbr.CustomerID && !i.IsDeleted).FirstOrDefault();
                    if (dbCustomer == null)
                    {
                        return new jResponse(false, "Customer does not exist!", dbRFQInfo.dbr.RequestForQuoteID);
                    }
                    smsObj.customerName = dbCustomer.CompanyName;
                } else
                {
                    smsObj.customerName = "Ammann";
                }

              
                smsObj.rfqID = dbRFQInfo.dbr.RequestForQuoteID.ToString();
                
                smsObj.transporterName = dbTransporterInfo.dbt.CompanyName;
                smsObj.charge = "0";
                List<string> TranspoterMobileList = new List<string>();
                TranspoterMobileList.Add(dbRFQInfo.dbe.Mobile);

                if (dbRFQInfo.dbe.Mobile != null || dbRFQInfo.dbe.Mobile != "")
                {
                    smsApiController.send_qtnExecutive(TranspoterMobileList, smsObj);
                }

                return new jResponse(false, "Quote issued!", insertedQuoteID);

            }
        }


        /*
         - readyRFQ()
         - Purpose: Ready for loading.
         - In: class myParams {
                          userID,
                          reqForQuoteID,  
                          vehicleNumber
                     }
         - Out: reqForQuoteID
    */
        [HttpPost, HttpGet, Route("req/readyVeh")]
        public dynamic readyRFQ(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1 || myParams.vehicleNumber == "")
            {
                return new jResponse();
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            string vehicleNumber = myParams.vehicleNumber;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                //var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                //                    .FirstOrDefault();

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                 join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                 join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr,
                                     dbs,
                                     dbc
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "5";
                dbRFQInfo.dbr.ReadyByTransporterUserID = userID;
                dbRFQInfo.dbr.VehicleReadyDT = currentDT;
                dbRFQInfo.dbr.VehicleNumber = vehicleNumber;
                dbRFQInfo.dbr.UpdatedDT = currentDT;
                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Vehicle is ready for loading!", reqForQuoteID);

            }

        }


        /*
          - getHistory()
          - Purpose: Get history.
          - In: class myParams {
                         transporterID
                      }
          - Out: Success / failure
      */
        [HttpPost, HttpGet, Route("rfqHistory/get")]
        public dynamic getHistory(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.transporterID == "" || myParams.transporterID == 0)
            {
                return new jResponse();
            }

            // Extract parameters
            int transporterID = myParams.transporterID;
            getRFQs requestList = new getRFQs();
            // #endregion

            // Get history
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // #region "Validate parameters"

                var dbtInfo = (from dbt in db.Transporters
                               where dbt.TransporterID == transporterID && !dbt.IsDeleted
                               select dbt).FirstOrDefault();

                // Verify transporter
                if (dbtInfo == null)
                {
                    return new jResponse(true, "Something looks off about your access.  Please contact the system administrator.", null);
                }

                //requestList.RFQList = db.sp_getRequest(transporterID, 1).ToList(); //trasporterID

                return new jResponse(false, "", requestList);
            }
        }
        #endregion

        #region "Classes"

        public class smsParams
        {
            public string rfqID { get; set; }
            public string customerName { get; set; }
            public string transporterName { get; set; }
            public string charge { get; set; }
        }

        public class rfqDetails
        {
            public int reqForQuoteID { get; set; }
            public int customerID { get; set; }
            public string customerName { get; set; }
            public string product { get; set; }
            public string sourceName { get; set; }
            public string destination { get; set; }
            public DateTime pickUpDT { get; set; }
            public DateTime deliveryDT { get; set; }
            public DateTime quoteByDT { get; set; }
            public string details { get; set; }
            public bool allowLaterDelivery { get; set; }
            public string status { get; set; }
            public string sourceAddress { get; set; }
            public string destinationAddress { get; set; }
            public dynamic response { get; set; }
            public bool isCloseEarly { get; set; }
            public string sourceState { get; set; }
            public string destinationState { get; set; }
            public string orderNo { get; set; }
            public string routeName { get; set; }
            public string fileName { get; set; }
            public string createdBy { get; set; }
            public dynamic components { get; set; }
            public int paymentTerm { get; set; }
            public string fixedTerm { get; set; }
        }              

        public class getRFQs
        {
            public dynamic quoteObj { get; set; }
            public dynamic RFQobj { get; set; }
            public dynamic parameter { get; set; }
            public bool quoteAns { get; set; }
        }

        public class emailDetail
        {
            public string rfqID { get; set; }
            public string companyName { get; set; }
          
        }

        #endregion

    }
}