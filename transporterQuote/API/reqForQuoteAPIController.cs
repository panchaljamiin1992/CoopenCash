using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using transporterQuote.Services;
using transporterQuote.API;
using transporterQuote.Models;
using transporterQuote.Helper;
using System.Data.Entity;
using System.Data;
using System.Web.Configuration;

namespace transporterQuote.API
{
    [RoutePrefix("api/reqForQuote")]
    public class reqForQuoteAPIController : ApiController
    {
        #region "Functions"

        /*
          - setReqForQuote()
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
        [HttpPost, HttpGet, IsMajama, Route("set")]
        public dynamic setReqForQuote(dynamic myParams)
        {
            // #region "Validations"

            if (myParams == null || myParams.executiveID < 1 
                || myParams.quoteBy == "" || myParams.details == "")
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int executiveID = Convert.ToInt32(myParams.executiveID);
           // int sourceID = Convert.ToInt32(myParams.sourceID);
            int customerID = Convert.ToInt32(myParams.customerID);
           // int zoneID = Convert.ToInt32(myParams.zoneID);
            string destination = myParams.destination;
           // string source = myParams.source;
            string product = myParams.product;
            string orderNo = myParams.orderNo;
            string fileName = myParams.fileName;
            //  int routeID = myParams.routeID;
            // string route = myParams.routeName; 
            //  DateTime pickUp = myParams.pickUpDT;
            DateTime pickUp = myParams.pickUpDT;
            DateTime deliveryBy = myParams.deliveryDT;
            DateTime quoteBy = myParams.quoteBy;
            string details = myParams.details;
            //string destination = myParams.destination;
            bool allowLaterDelivery = false;
            string transpoters = myParams.transpoters;
            string status = myParams.status;
            int insertedReqID = 0;
            int quoteTypeID = myParams.quoteTypeID;
            int paymentTerms = myParams.paymentTerms;
            string fixedTerm = myParams.fixedTerm;
            dynamic dynamicFields = myParams.dynamicFields;
            decimal budget = myParams.budget;
            int shipmentID = myParams.shipmentID;
            DateTime currentDT = genApiController.getDate();
            genApiController gen = new genApiController();
            //List<string> TranspoterMobileList = new List<string>();
            string showHistory = "false";
            string negotiationComments = "";
            try
            {
                negotiationComments = myParams.negotiationComments;
            }
            catch { }
            // Verify user
            if (myToken.userID != executiveID)
            {
                return new jResponse(true, "You're not allowed to access this API.", null);
            }

            // Check valid date
            //if (pickUp > deliveryBy)
            //{
            //    return new jResponse(true, "Plase select valid date.", null);
            //}

            //if (quoteBy > pickUp || quoteBy > deliveryBy)
            //{
            //    return new jResponse(true, "Plase select valid date.", null);
            //}

            // #endregion

            // Add new transporter
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                if (customerID > 0) { 
                    var ckeckCustomer = db.Customers.Where(i => i.CustomerID == customerID && !i.IsDeleted).FirstOrDefault();

                    if (ckeckCustomer == null)
                    {
                        return new jResponse(true, "This customer does not exist", null);
                    }

                }

                //var ckeckSource = db.Sources.Where(i => i.SourceID == sourceID && !i.IsDeleted).FirstOrDefault();

                //if (ckeckSource == null)
                //{
                //    return new jResponse(true, "This source does not exist", null);
                //}

                // Add Request for quote.
                RFQ addReqForQuote = new RFQ();
                addReqForQuote.CustomerID = customerID;
                addReqForQuote.SourceID = 0;
                addReqForQuote.ExecutiveID = executiveID;
                addReqForQuote.Source = "";
                addReqForQuote.Destination = "";
                addReqForQuote.Product = "";
                addReqForQuote.PickUp = pickUp;
                addReqForQuote.DeliveryBy = deliveryBy;
                addReqForQuote.QuoteBy = quoteBy;
                addReqForQuote.Details = details;
                addReqForQuote.Transporters = transpoters;
                addReqForQuote.AllowLaterDelivery = allowLaterDelivery;
                addReqForQuote.CloseEarly = false;
                addReqForQuote.Status = status;
                addReqForQuote.CreatedDT = currentDT;
                addReqForQuote.UpdatedDT = currentDT;
                addReqForQuote.IsDeleted = false;
                addReqForQuote.OrderNo = orderNo;
                addReqForQuote.FileName = fileName;
                addReqForQuote.RouteID = 0;
                addReqForQuote.Route = "";
                addReqForQuote.DispatchedByUserID = 0;
                addReqForQuote.DispatchedDT = null;
                addReqForQuote.ZoneID = 0;
                addReqForQuote.ForwardedByUserId = 0;
                addReqForQuote.ClearenceDT = null;
                addReqForQuote.ClearedByUserID = 0;
                addReqForQuote.TimeSlotFromDT = null;
                addReqForQuote.TimeSlotToDT = null;
                addReqForQuote.VehicleReadyDT = null;
                addReqForQuote.VehicleNumber = "";
                addReqForQuote.VehReadyByUserID = 0;
                addReqForQuote.VehicleEntryDT = null;
                addReqForQuote.LoadingByUserID = 0;
                addReqForQuote.ReadyByTransporterUserID = 0;
                addReqForQuote.QuoteTypeID = quoteTypeID;
                addReqForQuote.PaymentTerm = paymentTerms;
                addReqForQuote.FixedTerm = fixedTerm;
                addReqForQuote.ReminderSentDT = null;
                addReqForQuote.Budget = budget;
                addReqForQuote.ShipmentID = shipmentID;
                addReqForQuote.StartWorkDT = null;
                addReqForQuote.StartedByUserID = 0;

                db.Entry(addReqForQuote).State = EntityState.Added;
                db.SaveChanges();
                insertedReqID = addReqForQuote.RequestForQuoteID;

                foreach (var k in dynamicFields)
                {
                    RFQFieldValue addFieldValue = new RFQFieldValue();
                    addFieldValue.RFQID = insertedReqID;
                    addFieldValue.FieldID = k.FieldID;
                    addFieldValue.FieldValue = k.FieldValue;
                    addFieldValue.CreatedDT = currentDT;
                    addFieldValue.CreatedUserID = executiveID;
                    addFieldValue.UpdatedDT = null;
                    addFieldValue.UpdatedUserID = null;
                    addFieldValue.IsDeleted = false;

                    db.Entry(addFieldValue).State = EntityState.Added;
                    db.SaveChanges();
                }

                string sendEmail = WebConfigurationManager.AppSettings["SendEmails"];
                smsParams smsObj = new smsParams();
                if (sendEmail.ToLower() == "true")
                {
                    emailParams emailToTranspoterParams = new emailParams();

                    emailToTranspoterParams.rfqID = insertedReqID.ToString();
                    emailToTranspoterParams.product = product;
                    emailToTranspoterParams.source = ""; //ckeckSource.Location + ", " + ckeckSource.Address1 + "" + (ckeckSource.Address2 != "" ? ", " : "") + ckeckSource.City + " (" + ckeckSource.State + ")";
                   // emailToTranspoterParams.destination = ckeckCustomer.CompanyName +", "+ ckeckCustomer.Address1 + "" + (ckeckCustomer.Address2 != "" ? ", " : "") + ckeckCustomer.Address2 + ", " + ckeckCustomer.City + " (" + ckeckCustomer.State + ")";
                    emailToTranspoterParams.pickup = addReqForQuote.PickUp.ToString("d-MMM-yy");
                    emailToTranspoterParams.quoteby = addReqForQuote.QuoteBy.ToString("d-MMM-yy");
                    emailToTranspoterParams.delivery = addReqForQuote.DeliveryBy.ToString("d-MMM-yy");
                    emailToTranspoterParams.emailDynamicFields = dynamicFields;

                    if (addReqForQuote.Transporters == "~0~")
                    {
                        var allTranspoterInfo = (from dbt in db.Transporters
                                                 where !dbt.IsDeleted
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

                        foreach (var i in allTranspoterInfo)
                        {
                            Random rng1 = new Random();
                            Random rng2 = new Random();
                            int value = rng1.Next(1000);
                            int value1 = rng2.Next(1000);
                            string text = value.ToString("000");
                            string text1 = value1.ToString("000");

                            string parameters = "" + i.transpoterID * 2018 + text + "~" + insertedReqID * 2018 + text1 + "~" + executiveID.ToString()
                                                + "~" + customerID.ToString() +"~" + showHistory;

                            // string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                            // string encodedURL = genApiController.getEncryptedPW(parameters);
                            //string encodedURL = genApiController.encryptParam(parameters);
                            List<string> TranspoterList = new List<string>();
                            TranspoterList.Add(i.email);
                            emailToTranspoterParams.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                            int length = i.companyName.Length;
                            emailToTranspoterParams.displayName = i.companyName;
                            //emailToTranspoterParams.responseLink = genApiController.encryptURL(emailToTranspoterParams.responseLink, (WebConfigurationManager.AppSettings["TranspoterKey"]));

                            emailAPIController.transpoterQuote(TranspoterList, emailToTranspoterParams);

                            smsObj.rfqID = insertedReqID.ToString();
                            smsObj.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                            smsObj.sourceCity = "";//ckeckSource.City;
                          //  smsObj.destinationCity = ckeckCustomer.City;
                            List<string> TranspoterMobileList = new List<string>();
                            TranspoterMobileList.Add(i.mobile);
                            TranspoterMobileList.Add(i.alternateMobile);

                            smsApiController.send_newRFQ(TranspoterMobileList, smsObj);
                        }
                    }
                    else {
                        List<int> TranspoterIds = gen.splitString('~', transpoters).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

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

                        foreach (var i in TranspoterInfo)
                        {
                            //string parameters = "" + i.transpoterID.ToString() + "~" + insertedReqID.ToString() + "~" + executiveID.ToString()
                            //                    + "~" + customerID.ToString() + "~" + showHistory;

                            Random rng1 = new Random();
                            Random rng2 = new Random();
                            int value = rng1.Next(1000);
                            int value1 = rng2.Next(1000);
                            string text = value.ToString("000");
                            string text1 = value1.ToString("000");

                            string parameters = "" + i.transpoterID * 2018 + text  + "~" + insertedReqID * 2018 + text1 + "~" + executiveID.ToString()
                                               + "~" + customerID.ToString() + "~" + showHistory;



                            //   string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                            // string encodedURL = genApiController.getEncryptedPW(parameters);
                           // string encodedURL = genApiController.encryptParam(parameters);
                            List<string> TranspoterList = new List<string>();
                            TranspoterList.Add(i.email);
                            emailToTranspoterParams.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                            int length = i.companyName.Length;
                            emailToTranspoterParams.displayName = i.companyName;
                            //emailToTranspoterParams.responseLink = genApiController.encryptURL(emailToTranspoterParams.responseLink, (WebConfigurationManager.AppSettings["TranspoterKey"]));

                            emailAPIController.transpoterQuote(TranspoterList, emailToTranspoterParams);

                            smsObj.rfqID = insertedReqID.ToString();
                            smsObj.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                            smsObj.sourceCity = "";//ckeckSource.City;
                          //  smsObj.destinationCity = ckeckCustomer.City;
                            List<string> TranspoterMobileList = new List<string>();
                            TranspoterMobileList.Add(i.mobile);
                            TranspoterMobileList.Add(i.alternateMobile);

                            smsApiController.send_newRFQ(TranspoterMobileList, smsObj);
                        }
                    }
                   
                }

                return new jResponse(false, "RFQ created!", insertedReqID);
            }
        }

        /*
          - getReqForQuote()
          - Purpose: get list of reqForQuote List.
          - In: class myParams {
                          userID
                      }
          - Out: reqForQuoteDetails
       */
        [HttpPost, HttpGet, IsMajama, Route("get")]
        public dynamic getReqForQuote(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;
            List<int> zoneIds = new List<int>();
            bool isCheckZone = false;
            genApiController gen = new genApiController();

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // Get reqForQuote list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var dbExecutive = db.Executives.Where(i => i.ExecutiveID == userID && !i.IsDeleted).FirstOrDefault();

                if (dbExecutive == null)
                {
                    return new jResponse(true, "This executive does not exist", null);
                }

                if (dbExecutive.ZoneIDs != "~" && dbExecutive.ZoneIDs != "")
                {
                    zoneIds = gen.splitString('~', dbExecutive.ZoneIDs).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                    isCheckZone = true;
                }

                var reqForQuoteDetails = db.sp_getRFQList(userID).ToList();

                if (reqForQuoteDetails == null)
                {
                    return new jResponse(true, "No RFQ found!", null);
                }

                //code to show RFQs based on service rights
                var dbServiceTypesInfo = db.ServiceTypes.Where(r => r.IsDeleted == false);
                List<sp_getRFQList_Result> result = new List<sp_getRFQList_Result>();
                List<string> allowedServices = new List<string>();
                foreach (string serviceRight in dbExecutive.ServiceRights.Split('~'))
                {
                    if (serviceRight == "")
                        continue;
                    foreach (var serviceType in dbServiceTypesInfo)
                    {
                        if (serviceType.ServiceTypeID == int.Parse(serviceRight))
                            allowedServices.Add(serviceType.ServiceName);
                    }
                }
                foreach (var reqForQuoteDetail in reqForQuoteDetails)
                {
                    if (allowedServices.Contains(reqForQuoteDetail.serviceName))
                        result.Add(reqForQuoteDetail);
                }
                //end

                //return new jResponse(false, "", reqForQuoteDetails);
                return new jResponse(false, "", result);

            }
        }

        /*
        - getQuotationList()
        - Purpose: get list of Quote List.
        - In: class myParams {
                        userID
                    }
        - Out: qutationInfo
        */
        [HttpPost, HttpGet, Route("quote/get")]
        public dynamic getQuotationList(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;
            genApiController gen = new genApiController();

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // Get quotation list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var qutationInfo = (from dbq in db.Quotations
                                    join dbr in db.RFQs on dbq.RequestForQuoteID equals dbr.RequestForQuoteID
                                    join dbt in db.Transporters on dbq.TransporterID equals dbt.TransporterID
                                    where !dbq.IsDeleted && !dbr.IsDeleted
                                    select new qutationDetails
                                    {
                                        quoteID = dbq.QuoteID,
                                        reqForQuoteID = dbq.RequestForQuoteID,
                                        executiveID = dbr.ExecutiveID,
                                        transporterID = dbq.TransporterID,
                                        transpoterName = dbt.Title + " " + dbt.FirstName + " " + dbt.LastName,
                                        transCompanyName = dbt.CompanyName,
                                        mobile = dbt.Mobile,
                                        email = dbt.Email,
                                        rate = dbq.Charges,
                                        note = dbq.Notes,
                                        paymentTerms = dbq.PaymentTerms,
                                        //status = dbr.Status,
                                        status = dbq.Status,
                                        pickUpDT = dbr.PickUp,
                                        deliveryDT = dbr.DeliveryBy,
                                        extendDeliveryDT = dbq.ExtendDeliveryDT,
                                        components = dbq.Components,
                                        exchangeRate = dbq.ExchangeRate,
                                        createdDT = dbq.CreatedDT,
                                        roundNumber = dbq.RoundNumber
                                    }).OrderBy(i => i.rate).ToList();

                return new jResponse(false, "", qutationInfo);
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
                    note
            }
         - Out: success/failure
      */
        [HttpPost, HttpGet, Route("quote/accept")]
        public dynamic acceptQuote(dynamic myParams)
        {
            // #region "Validations"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1
                || myParams.quoteID < 1 || myParams.transporterID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            int quoteID = myParams.quoteID;
            int transporterID = myParams.transporterID;
            int executiveID = 0;
            string note = myParams.note;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();
            Quotations quoteInfo = new Quotations();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // #region "request response"

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
                quoteDetails.AcceptID = userID;
                quoteDetails.AcceptanceStatusOn = currentDT;
                quoteDetails.UpdatedDT = currentDT;
                quoteDetails.AcceptNote = note;

                db.Entry(quoteDetails).State = EntityState.Modified;

                db.SaveChanges();

               var shipmentDetail =  db.sp_MapShipFields(quoteID, userID, reqForQuoteInfo.ShipmentID).FirstOrDefault();

                if (shipmentDetail == null)
                {
                    return new jResponse(false, "Shipment can't added successfully.", true);
                }


                string sendEmail = WebConfigurationManager.AppSettings["SendEmails"];
                if (sendEmail.ToLower() == "true")
                {
                    var transporterDetail = db.Transporters.Where(i => i.TransporterID == quoteDetails.TransporterID && !i.IsDeleted).FirstOrDefault();

                    if (transporterDetail == null)
                    {
                        return new jResponse(false, "Transporter does not exist.", true);
                    }

                    dynamic customerDetail = null;
                    if (reqForQuoteInfo.CustomerID > 0) { 

                        customerDetail = db.Customers.Where(i => i.CustomerID == reqForQuoteInfo.CustomerID && !i.IsDeleted).FirstOrDefault();

                        if (customerDetail == null)
                        {
                            return new jResponse(false, "Customer does not exist.", true);
                        }

                    }

                    Random rng2 = new Random();
                    int value = rng2.Next(1000);
                    string text = value.ToString("000");

                    string parameters = "" + transporterDetail.TransporterID * 2018 + text + "~" + reqForQuoteID * 2018 + text + "~" + executiveID.ToString()
                                              + "~" + (reqForQuoteInfo.CustomerID > 0 ? customerDetail.CustomerID.ToString() : "0") ;

                    //   string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                   // string encodedURL = genApiController.getEncryptedPW(parameters);
                    List<string> emailList = new List<string>();
                    emailList.Add(transporterDetail.Email);
                    emailParams emailToTransporterParams = new emailParams();
                    emailToTransporterParams.rfqID = reqForQuoteInfo.RequestForQuoteID.ToString();
                    //emailToTransporterParams.quoteID = quoteID.ToString();

                    emailToTransporterParams.companyName = reqForQuoteInfo.CustomerID > 0 ?  customerDetail.CompanyName : "";
                    emailToTransporterParams.pickup = reqForQuoteInfo.PickUp.ToString("d-MMM-yy");
                    emailToTransporterParams.product = reqForQuoteInfo.Product.ToString();
                    emailToTransporterParams.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                    emailToTransporterParams.delivery = quoteDetails.ExtendDeliveryDT == null ? reqForQuoteInfo.DeliveryBy.ToString("d-MMM-yy") : quoteDetails.ExtendDeliveryDT.GetValueOrDefault().ToString("d-MMM-yy");
                    emailAPIController.quot_accept(emailList, emailToTransporterParams);

                    // SMS send
                    smsToTransporterParams smsObj = new smsToTransporterParams();
                    smsObj.rfqID = reqForQuoteInfo.RequestForQuoteID.ToString();
                    smsObj.customerName = reqForQuoteInfo.CustomerID > 0 ? customerDetail.CompanyName : "";
                   
                    List<string> TranspoterMobileList = new List<string>();
                    TranspoterMobileList.Add(transporterDetail.Mobile);
                    TranspoterMobileList.Add(transporterDetail.AlternateMobile);

                    if (transporterDetail.Mobile != null || transporterDetail.Mobile != "")
                    {
                        smsApiController.send_acptToTransporter(TranspoterMobileList, smsObj);
                    }
                }

                return new jResponse(false, "Quote is accepted!", quoteDetails.Status);
                // #endregion
            }

            // #endregion
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

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1 || myParams.quoteByDT == null)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime quoteByDT = myParams.quoteByDT;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            if (currentDT > quoteByDT)
            {
                return new jResponse(true, "Plase select valid date.", null);
            }

            // #endregion

            // #region "request response"

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                                    .FirstOrDefault();

                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (reqForQuoteInfo == null || reqForQuoteInfo.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                reqForQuoteInfo.QuoteBy = quoteByDT;
                reqForQuoteInfo.UpdatedDT = currentDT;

                db.Entry(reqForQuoteInfo).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "RFQ extended!", true);
            }

            // #endregion

        }

        /*
           - reqCloseEarly()
           - Purpose: request close.
           - In: class myParams {
                      userID,
                      reqForQuoteID,
              }
           - Out: Success/failure
        */
        [HttpPost, HttpGet, Route("req/closeEarly")]
        public dynamic reqCloseEarly(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            bool isCloseEarly = true;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                //var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                //                    .FirstOrDefault();
                
                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                               //  join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                 join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted 
                                 select new
                                 {
                                     dbr,
                                  //   dbs,
                                     dbc
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.CloseEarly = isCloseEarly;
                dbRFQInfo.dbr.UpdatedDT = currentDT;

                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();
                string sendEmail = "false"; //WebConfigurationManager.AppSettings["SendEmails"];
                if (sendEmail.ToLower() == "true")
                {
                    var dbqCount = db.Quotations.Where(i => i.RequestForQuoteID == dbRFQInfo.dbr.RequestForQuoteID && !i.IsDeleted).Count();

                    emailDetail emailToTransporterParams = new emailDetail();

                    string parameters = "" + reqForQuoteID.ToString() + "~" + dbRFQInfo.dbr.CustomerID.ToString();
                    string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                    List<string> emailList = new List<string>();
                    emailList.Add(dbRFQInfo.dbc.Email);
                    emailToTransporterParams.rfqID = dbRFQInfo.dbr.RequestForQuoteID.ToString();
                    emailToTransporterParams.link = genApiController.getURL() + "customer.html?q=" + encodedURL;
                    emailToTransporterParams.quoteCount = dbqCount.ToString();
                    //emailToTransporterParams.source = dbRFQInfo.dbs.Address1 + " " + (dbRFQInfo.dbs.Address2 != "" ? ", " : "") + dbRFQInfo.dbs.Address2 + ", " + dbRFQInfo.dbs.City + " (" + dbRFQInfo.dbs.State + ")";
                    // emailToTransporterParams.destination = dbRFQInfo.dbc.Address1 + " " + (dbRFQInfo.dbc.Address2 != "" ? ", " : "") + dbRFQInfo.dbc.Address2 + ", " + dbRFQInfo.dbc.City + " (" + dbRFQInfo.dbc.State + ")";
                    // emailToTransporterParams.pickup = dbRFQInfo.dbr.PickUp.ToString("d-MMM-yy");
                    //emailToTransporterParams.delivery = dbRFQInfo.dbr.DeliveryBy.ToString("d-MMM-yy");
                    //emailToTransporterParams.charges = charges.ToString();
                    //emailToTransporterParams.payment = paymentTerms.ToString();
                    // emailToTransporterParams.product = dbRFQInfo.dbr.Product;
                    //emailToTransporterParams.companyName = dbTransporterInfo.dbt.CompanyName;
                    //emailToTransporterParams.notes = notes;
                    emailToTransporterParams.displayName = dbRFQInfo.dbc.FirstName + " " + dbRFQInfo.dbc.LastName;
                    emailToTransporterParams.userName = myToken.name;
                    emailAPIController.link_Customer(emailList, emailToTransporterParams);

                }

                return new jResponse(false, "RFQ closed!", reqForQuoteID);

            }
        }

        /*
           - changePassword()
           - Purpose: change password.
           - In: class myParams {
                      userID,
                      oldPassword,
                      newPassword
              }
           - Out: Success/failure
        */
        [HttpPost, HttpGet, Route("password/change")]
        public dynamic changePassword(dynamic myParams)
        {
            // #region "Validation"
            if (myParams == null || myParams.userID < 0 || myParams.oldPassword == null || myParams.oldPassword == "" || 
                myParams.newPassword == null || myParams.newPassword == "")
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            string oldPassword = myParams.oldPassword;
            string newPassword = myParams.newPassword;
            DateTime currentDT = genApiController.getDate();


            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Change password
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Check for valid mobile number
                var dbExecutiveInfo = db.Executives.Where(i => i.ExecutiveID == userID && !i.IsDeleted).FirstOrDefault();

                // Check executive 
                if (dbExecutiveInfo == null)
                {
                    return new jResponse(false, "Executive does not exist.", null);
                }

                if(dbExecutiveInfo.Password != oldPassword)
                {
                    return new jResponse(true, "Your old password is not correct.", null);
                }

                dbExecutiveInfo.Password = newPassword;
                dbExecutiveInfo.CreatedDT = currentDT;

                db.Entry(dbExecutiveInfo).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Password changed.", null);
            }
        }

        /*
          - getProductList()
          - Purpose: get list of product.
          - In: class myParams {
                            userID
                      }
          - Out: dbProductList
      */
        [HttpPost, HttpGet, Route("product/get")]
        public dynamic getProductList(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;

            List<string> dbProductList = new List<string>();

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                dbProductList = db.RFQs.Select(i => i.Product).Distinct().ToList();
                return new jResponse(false, "", dbProductList);
            }
        }

        /*
        - getRouteList()
        - Purpose: get list of route.
        - In: class myParams {
                        userID
                    }
        - Out: dbRouteList
    */
        [HttpPost, HttpGet, Route("route/get")]
        public dynamic getRouteList(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;

            List<string> dbRouteList = new List<string>();

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                dbRouteList = db.RFQs.Select(i => i.Route).Distinct().ToList();
                return new jResponse(false, "", dbRouteList);
            }
        }

        /*
             - forwardToCustomer()
             - Purpose: forward to customer.
             - In: class myParams {
                              userID,
                              reqForQuoteID,  
                         }
             - Out: reqForQuoteID
        */
        [HttpPost, HttpGet, Route("req/forward")]
        public dynamic forwardToCustomer(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            bool isCloseEarly = true;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                //var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                //                    .FirstOrDefault();

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                // join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                               //  join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 join dbq in db.QuoteTypes on new {dbr.QuoteTypeID, isQuoteType = false}
                                 equals new { dbq.QuoteTypeID, isQuoteType = dbq.IsDeleted } into tqc
                                 from rqc in tqc.DefaultIfEmpty()
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr,
                                     //dbs,
                                    // dbc,
                                     rqc
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "2";
                dbRFQInfo.dbr.CloseEarly = isCloseEarly;
                dbRFQInfo.dbr.ForwardedByUserId = userID;
                dbRFQInfo.dbr.UpdatedDT = currentDT;

                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();
                string sendEmail = WebConfigurationManager.AppSettings["SendEmails"];
                if (sendEmail.ToLower() == "true")
                {
                    var dbqCount = db.Quotations.Where(i => i.RequestForQuoteID == dbRFQInfo.dbr.RequestForQuoteID && !i.IsDeleted).Count();
                    if (dbRFQInfo.rqc.PaidBy == "Company")
                    {
                        var dbExecutive = db.Executives.Where(i => i.CanSelectForCustomer == true && !i.IsDeleted).ToList();
                        if (dbExecutive == null)
                        {
                            return new jResponse(false, "Executives does not exist!", reqForQuoteID);
                        }


                        foreach (var i in dbExecutive)
                        {
                            //string parameters = "" + i.transpoterID.ToString() + "~" + insertedReqID.ToString() + "~" + executiveID.ToString()
                            //                    + "~" + customerID.ToString() + "~" + showHistory;

                            emailDetail emailToTransporterParams = new emailDetail();

                            Random rng1 = new Random();
                            Random rng2 = new Random();
                            int value = rng1.Next(1000);
                            int value1 = rng2.Next(1000);
                            string text = value.ToString("000");
                            string text1 = value1.ToString("000");

                            string parameters = "" + reqForQuoteID * 2018 + text + "~" + i.ExecutiveID * 2018 + text1 + "~0";
                            // string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                            List<string> emailList = new List<string>();
                            emailList.Add(i.Email);
                            emailToTransporterParams.rfqID = dbRFQInfo.dbr.RequestForQuoteID.ToString();
                            emailToTransporterParams.link = genApiController.getURL() + "customer.html?q=" + parameters;
                            emailToTransporterParams.quoteCount = dbqCount.ToString();
                            emailToTransporterParams.displayName = i.ExecutiveName;
                            emailToTransporterParams.userName = myToken.name;
                            emailAPIController.link_Customer(emailList, emailToTransporterParams);
                        }

                    }
                    else
                    {

                        var dbCustomer = db.Customers.Where(i => i.CustomerID == dbRFQInfo.dbr.CustomerID && !i.IsDeleted).FirstOrDefault();
                        if (dbCustomer == null)
                        {
                            return new jResponse(false, "Customer does not exist!", reqForQuoteID);
                        }

                        emailDetail emailToTransporterParams = new emailDetail();

                        Random rng1 = new Random();
                        Random rng2 = new Random();
                        int value = rng1.Next(1000);
                        int value1 = rng2.Next(1000);
                        string text = value.ToString("000");
                        string text1 = value1.ToString("000");

                        string parameters = "" + reqForQuoteID * 2018 + text + "~" + dbRFQInfo.dbr.CustomerID * 2018 + text1 + "~2018";
                       // string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                        List<string> emailList = new List<string>();
                        emailList.Add(dbCustomer.Email);
                        emailToTransporterParams.rfqID = dbRFQInfo.dbr.RequestForQuoteID.ToString();
                        emailToTransporterParams.link = genApiController.getURL() + "customer.html?q=" + parameters;
                        emailToTransporterParams.quoteCount = dbqCount.ToString();
                        emailToTransporterParams.displayName = dbCustomer.FirstName + " " + dbCustomer.LastName;
                        emailToTransporterParams.userName = myToken.name;
                        emailAPIController.link_Customer(emailList, emailToTransporterParams);
                    }
                }

                return new jResponse(false, "RFQ forwarded to customer!", reqForQuoteID);

            }

        }

        /*
            - dispatchRFQ()
            - Purpose: Dispatch to rfq.
            - In: class myParams {
                             userID,
                             reqForQuoteID,  
                        }
            - Out: reqForQuoteID
       */
        [HttpPost, HttpGet, Route("req/dispatch")]
        public dynamic dispatchRFQ(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                //var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                //                    .FirstOrDefault();

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                // join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                 join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr,
                                  //   dbs,
                                     dbc
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "3";
                dbRFQInfo.dbr.DispatchedByUserID = userID;
                dbRFQInfo.dbr.DispatchedDT = currentDT;
                dbRFQInfo.dbr.UpdatedDT = currentDT;
                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();
                string sendEmail = "False";
                if (sendEmail.ToLower() == "true")
                {
                    var dbqCount = db.Quotations.Where(i => i.RequestForQuoteID == dbRFQInfo.dbr.RequestForQuoteID && !i.IsDeleted).Count();

                    emailDetail emailToTransporterParams = new emailDetail();

                    string parameters = "" + reqForQuoteID.ToString() + "~" + dbRFQInfo.dbr.CustomerID.ToString();
                    string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                    List<string> emailList = new List<string>();
                    emailList.Add(dbRFQInfo.dbc.Email);
                    emailToTransporterParams.rfqID = dbRFQInfo.dbr.RequestForQuoteID.ToString();
                    emailToTransporterParams.link = genApiController.getURL() + "customer.html?q=" + encodedURL;
                    emailToTransporterParams.quoteCount = dbqCount.ToString();
                    emailToTransporterParams.displayName = dbRFQInfo.dbc.FirstName + " " + dbRFQInfo.dbc.LastName;
                    emailToTransporterParams.userName = myToken.name;
                    emailAPIController.link_Customer(emailList, emailToTransporterParams);

                }

                return new jResponse(false, "RFQ is dispatched!", reqForQuoteID);

            }

        }

        /*
           - clearRFQ()
           - Purpose: Clear to rfq.
           - In: class myParams {
                            userID,
                            reqForQuoteID,  
                       }
           - Out: reqForQuoteID
      */
        [HttpPost, HttpGet, Route("req/clear")]
        public dynamic clearRFQ(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                //var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                //                    .FirstOrDefault();

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                // join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                // join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr,
                                    // dbs,
                                    // dbc
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "4";
                dbRFQInfo.dbr.ClearedByUserID = userID;
                dbRFQInfo.dbr.ClearenceDT = currentDT;
                dbRFQInfo.dbr.UpdatedDT = currentDT;
                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Done!", reqForQuoteID);

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

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            string vehicleNumber = myParams.vehicleNumber;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                //var reqForQuoteInfo = db.RFQs.Where(i => i.RequestForQuoteID == reqForQuoteID && !i.IsDeleted)
                //                    .FirstOrDefault();

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                // join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                 join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr,
                                  //   dbs,
                                     dbc
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "5";
                dbRFQInfo.dbr.VehReadyByUserID = userID;
                dbRFQInfo.dbr.VehicleReadyDT = currentDT;
                dbRFQInfo.dbr.VehicleNumber = vehicleNumber;
                dbRFQInfo.dbr.UpdatedDT = currentDT;
                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Done!", reqForQuoteID);

            }

        }

        /*
         - setTimeSlot()
         - Purpose: Set time slot for loading.
         - In: class myParams {
                          userID,
                          reqForQuoteID,  
                          timeSlotFromDT,
                          timeSlotToDT
                     }
         - Out: reqForQuoteID
    */
        [HttpPost, HttpGet, Route("req/setTimeSlot")]
        public dynamic setTimeSlot(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1 || myParams.timeSlotFromDT == "" || myParams.timeSlotToDT == "")
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime timeSlotFromDT = myParams.timeSlotFromDT;
            DateTime timeSlotToDT = myParams.timeSlotToDT;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                // join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                 join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr,
                                  //   dbs,
                                     dbc
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "6";
                dbRFQInfo.dbr.TimeSlotFromDT = timeSlotFromDT;
                dbRFQInfo.dbr.TimeSlotToDT = timeSlotToDT;
                dbRFQInfo.dbr.UpdatedDT = currentDT;
                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Done!", reqForQuoteID);

            }

        }

        /*
         - entryVehicle()
         - Purpose: Set entry of vehicle.
         - In: class myParams {
                          userID,
                          reqForQuoteID
                     }
         - Out: reqForQuoteID
    */
        [HttpPost, HttpGet, Route("req/entryVehicle")]
        public dynamic entryVehicle(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                // join dbs in db.Sources on dbr.SourceID equals dbs.SourceID
                                 join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr,
                                  //   dbs,
                                     dbc
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "7";
                dbRFQInfo.dbr.VehicleEntryDT = currentDT;
                dbRFQInfo.dbr.LoadingByUserID = userID;
                dbRFQInfo.dbr.UpdatedDT = currentDT;
                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Done!", reqForQuoteID);

            }

        }

        /*
       - getExecZone()
       - Purpose: Get zone.
       - In: class myParams {
                       userID
                   }
       - Out: zones
        */
        [HttpPost, HttpGet, Route("execZone/get")]
        public dynamic getExecZone(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;
            genApiController gen = new genApiController();
            List<int> zoneIds = new List<int>();
            dynamic zones;

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // Get zones
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var dbExecutive = db.Executives.Where(i => i.ExecutiveID == userID && !i.IsDeleted).FirstOrDefault();

                if (dbExecutive == null)
                {
                    return new jResponse(true, "This executive does not exist", null);
                }

                if (dbExecutive.ZoneIDs != "~" && dbExecutive.ZoneIDs != "")
                {
                    zoneIds = gen.splitString('~', dbExecutive.ZoneIDs).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                }

                if (dbExecutive.ZoneIDs == "~" || dbExecutive.ZoneIDs == "")
                {
                 zones = (from dbz in db.Zones
                                 where !dbz.IsDeleted
                                 select new zone
                                 {
                                     zoneID = dbz.ZoneID,
                                     zoneName = dbz.ZoneName
                                 }).OrderBy(i => i.zoneName).ToList();
                } else { 

                  zones = (from dbz in db.Zones
                                 where !dbz.IsDeleted 
                                 && zoneIds.Contains(dbz.ZoneID)
                                 select new zone
                                 {
                                     zoneID = dbz.ZoneID,
                                     zoneName = dbz.ZoneName
                                 }).OrderBy(i => i.zoneName).ToList();
                }

                return new jResponse(false, "", zones);
            }
        }

        /*
          - getScheduleList()
          - Purpose: Get schedule.
          - In: class myParams {
                          userID,
                          date
                      }
          - Out: list
       */
        [HttpPost, HttpGet, Route("schedule/get")]
        public dynamic getScheduleList(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 0 || myParams.date == null || myParams.date == "")
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;
            DateTime date = myParams.date;
           //int zoneID = myParams.zoneID;

            genApiController gen = new genApiController();
            schedule schedules = new schedule();

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // #endregion

            // Get scheduled list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                schedules.scheduleList = db.sp_getScheduleList(date, userID).ToList();
                return new jResponse(false, "", schedules);
            }
         }

        /*
         - getStatusReport()
         - Purpose: Get report .
         - In: class myParams {
                         userID
                     }
         - Out: list
      */
        [HttpPost, HttpGet, Route("report/get")]
        public dynamic getStatusReport(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;
          
            //int zoneID = myParams.zoneID;

            genApiController gen = new genApiController();
            report rpt = new report();

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // #endregion

            // Get scheduled list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var currentStatusList = (from dbr in db.RFQs
                                         where !dbr.IsDeleted
                                         group dbr by new { dbr.Status } into rbd
                                         select new
                                         {
                                           count = rbd.Where(i => i.Status == rbd.Key.Status).Count(),
                                          // selected = rbd.Where(i => i.Status == "1").Count(),
                                           status =  rbd.Key.Status
                                         //  name = Enum.GetName(typeof(executiveRights), Int32.Parse(rbd.Key.Status))
                                         }).ToList();

                var openRFQ = (from dbr in db.RFQs
                               where !dbr.IsDeleted && dbr.Status == "0"
                               group dbr by new { dbr.Status } into or
                               select new
                               {
                                   count = or.Where(j => j.Status == "0").Count(),
                                   status = or.Key.Status
                               }).FirstOrDefault();

                rpt.currentWorkLoadList = currentStatusList;
                rpt.openRFQCount = openRFQ;

               
                return new jResponse(false, "", rpt);
            }
        }

        /*
        - getQuoteType()
        - Purpose: Get quote type list.
        - In: class myParams {
                     userID,
                     serviceID,
                     isDispatchPlan,
                     serviceRights
                    }
        - Out: Success/Failure 
        */
        [HttpPost, HttpGet, Route("execQType/get")]
        public dynamic getQuoteType(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            genApiController gen = new genApiController();
            int userID = myParams.userID;
            int serviceID = myParams.serviceID;
            bool isDispatchPlan = myParams.isDispatchPlan;
            string serviceRights = myParams.serviceRights;
            List<int> serviceTypeIDs = gen.splitString('~', serviceRights).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

         //   bool isDomestic = myParams.isDomestic;
         //   bool isExport = myParams.isExport;

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }


            // Get executive list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var quoteTypeList = (from dbq in db.QuoteTypes
                                     join dbs in db.ServiceTypes on dbq.ServiceTypeID equals dbs.ServiceTypeID
                                     where !dbq.IsDeleted && serviceTypeIDs.Contains(dbq.ServiceTypeID) && (!isDispatchPlan || (isDispatchPlan && dbq.ServiceTypeID == serviceID))
                                     // &&  (dbq.Type == "Domestic" && isDomestic) || (dbq.Type == "Export" && isExport)
                                     select new quoteTypeDetail
                                     {
                                         quoteTypeID = dbq.QuoteTypeID,
                                         quoteTypeName = dbq.QuoteTypeName,
                                         paidBy = dbq.PaidBy,
                                         type = dbq.Type,
                                         serviceID = dbs.ServiceTypeID,
                                         serviceName = dbs.ServiceName
                                     }).OrderBy(i => i.quoteTypeName).ToList();

                return new jResponse(false, "", quoteTypeList);
            }
        }

        /*
    - getTrasporters()
    - Purpose: get list of transporter.
    - In: class myParams {
                     userID,
                     serviceTypeID
                }
    - Out: transporters
    */
        [HttpPost, HttpGet, Route("execTransporters/get")]
        public dynamic getTransporters(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }


            // Extract parameters
            int userID = myParams.userID;
            string serviceTypeID = myParams.serviceTypeID;
          //  bool isDomestic = myParams.isDomestic;
          //  bool isExport = myParams.isExport;

            if (myToken.userID != userID)
            {
                return new jResponse(true, "User does not exist.", null);
            }

            // Get transporter list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var transporters = (from dbt in db.Transporters
                                    where !dbt.IsDeleted && dbt.ServiceTypeID.Contains("~"+ serviceTypeID +"~")
                                    select new transporters
                                    {
                                        transporterID = dbt.TransporterID,
                                        companyName = dbt.CompanyName,
                                        city = dbt.City,
                                        state = dbt.State,
                                        title = dbt.Title,
                                        firstName = dbt.FirstName,
                                        lastName = dbt.LastName,
                                        email = dbt.Email,
                                        mobile = dbt.Mobile,
                                        alternateMobile = dbt.AlternateMobile,
                                        notes = dbt.Notes,
                                        isDomestic = dbt.IsDomestic,
                                        isExport = dbt.IsExport
                                    }).OrderBy(i => i.companyName).ToList();

                return new jResponse(false, "", transporters);
            }
        }

        /*
           - setReminder()
           - Purpose: remider for pending transporter for quotation.
           - In: class myParams {
                            userID,
                            rfqTD,
                            transporters,
                            dynamicFields
                       }
           - Out: transporters
       */
        [HttpPost, HttpGet, Route("reminder/set")]
        public dynamic setReminder(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            genApiController gen = new genApiController();
            DateTime currentDT = genApiController.getDate();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }


            // Extract parameters
            int userID = myParams.userID;
            int rfqID = myParams.rfqID;
            int customerID = 1;
            string transporters = myParams.transporters;
            string showHistory = "false";
            dynamic dynamicFields = myParams.dynamicFields;

            if (myToken.userID != userID)
            {
                return new jResponse(true, "User does not exist.", null);
            }

            // Get transporter list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var dbRFQ = db.RFQs.Where(i => i.RequestForQuoteID == rfqID && !i.IsDeleted).FirstOrDefault();

                if (dbRFQ == null)
                {
                    return new jResponse(true, "RFQ does not exist.", null);
                }

                dbRFQ.ReminderSentDT = currentDT;
                dbRFQ.UpdatedDT = currentDT;

                db.Entry(dbRFQ).State = EntityState.Modified;
                db.SaveChanges();

                //var ckeckCustomer = db.Customers.Where(i => i.CustomerID == dbRFQ.CustomerID && !i.IsDeleted).FirstOrDefault();

                //if (ckeckCustomer == null)
                //{
                //    return new jResponse(true, "This customer does not exist", null);
                //}

                string sendEmail = WebConfigurationManager.AppSettings["SendEmails"];
                smsParams smsObj = new smsParams();
                if (sendEmail.ToLower() == "true")
                {
                    emailParams emailToTranspoterParams = new emailParams();

                    emailToTranspoterParams.rfqID = rfqID.ToString();
                    emailToTranspoterParams.product = dbRFQ.Product;
                   // emailToTranspoterParams.source = ckeckSource.Location + ", " + ckeckSource.Address1 + "" + (ckeckSource.Address2 != "" ? ", " : "") + ckeckSource.City + " (" + ckeckSource.State + ")";
                   // emailToTranspoterParams.destination = ckeckCustomer.CompanyName + ", " + ckeckCustomer.Address1 + "" + (ckeckCustomer.Address2 != "" ? ", " : "") + ckeckCustomer.Address2 + ", " + ckeckCustomer.City + " (" + ckeckCustomer.State + ")";
                    emailToTranspoterParams.pickup = dbRFQ.PickUp.ToString("d-MMM-yy");
                    emailToTranspoterParams.quoteby = dbRFQ.QuoteBy.ToString("d-MMM-yy");
                    emailToTranspoterParams.delivery = dbRFQ.DeliveryBy.ToString("d-MMM-yy");
                    emailToTranspoterParams.emailDynamicFields = dynamicFields;

                    List<int> TranspoterIds = gen.splitString('~', transporters).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
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

                    foreach (var i in TranspoterInfo)
                    {
                        //string parameters = "" + i.transpoterID.ToString() + "~" + insertedReqID.ToString() + "~" + executiveID.ToString()
                        //                    + "~" + customerID.ToString() + "~" + showHistory;

                        Random rng1 = new Random();
                        Random rng2 = new Random();
                        int value = rng1.Next(1000);
                        int value1 = rng2.Next(1000);
                        string text = value.ToString("000");
                        string text1 = value1.ToString("000");

                        string parameters = "" + i.transpoterID * 2018 + text + "~" + rfqID * 2018 + text1 + "~" + userID.ToString()
                                           + "~" + customerID.ToString() + "~" + showHistory;

                        //   string encodedURL = genApiController.encryptURL(parameters, (WebConfigurationManager.AppSettings["TransporterKey"]));
                        // string encodedURL = genApiController.getEncryptedPW(parameters);
                        // string encodedURL = genApiController.encryptParam(parameters);
                        List<string> TranspoterList = new List<string>();
                        TranspoterList.Add(i.email);
                        emailToTranspoterParams.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                        int length = i.companyName.Length;
                        emailToTranspoterParams.displayName = i.companyName;
                        //emailToTranspoterParams.responseLink = genApiController.encryptURL(emailToTranspoterParams.responseLink, (WebConfigurationManager.AppSettings["TranspoterKey"]));

                        emailAPIController.sent_ReminderTransporter(TranspoterList, emailToTranspoterParams);

                        smsObj.rfqID = rfqID.ToString();
                        smsObj.link = genApiController.getURL() + "transporter.html?q=" + parameters;
                      //  smsObj.sourceCity = ckeckSource.City;
                      //  smsObj.destinationCity = ckeckCustomer.City;
                        List<string> TranspoterMobileList = new List<string>();
                        TranspoterMobileList.Add(i.mobile);
                        TranspoterMobileList.Add(i.alternateMobile);

                        smsApiController.send_Remider(TranspoterMobileList, smsObj);
                    }
                }

                return new jResponse(false, "Reminder sent.", null);
            }
        }

        /*
          - getField()
          - Purpose: Get field.
          - In: class myParams {
                           userID,
                           quoteTypeID
                      }
          - Out: Field
      */
        [HttpPost, HttpGet, Route("fields/get")]
        public dynamic getField(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 1 || myParams.quoteTypeID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            genApiController gen = new genApiController();

            // Extract parameter
            int quoteTypeID = myParams.quoteTypeID;
            DateTime currentDT = genApiController.getDate();

            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Get field type
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var dynamicFields = (from dbq in db.QuoteTypes
                                    join dbf in db.Fields
                                    on new { dbq.QuoteTypeID, isField = false }
                                    equals new { dbf.QuoteTypeID, isField = dbf.IsDeleted } into df
                                    join dbs in db.ServiceTypes on dbq.ServiceTypeID equals dbs.ServiceTypeID
                                    where !dbq.IsDeleted && dbq.QuoteTypeID == quoteTypeID
                                    select new fieldType
                                    {
                                        quoteTypeID = dbq.QuoteTypeID,
                                        coreFields= dbq.CoreField,
                                        fields = df,
                                        completionText = dbs.CompletionDateText,
                                        pickupText = dbs.PickupDateText,
                                        serviceTypeID = dbq.ServiceTypeID
                                    }).FirstOrDefault();

              
                    List<int> coreIDs = gen.splitString('~', dynamicFields.coreFields).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

                    foreach (var j in coreIDs)
                    {
                        if (j == (int)coreFields.customer)
                        {
                            dynamicFields.customer = true;
                        }

                        if (j == (int)coreFields.term)
                        {
                            dynamicFields.term = true;
                        }

                        if (j == (int)coreFields.note)
                        {
                            dynamicFields.note = true;
                        }

                        if (j == (int)coreFields.quoteby)
                        {
                            dynamicFields.quoteby = true;
                        }

                        if (j == (int)coreFields.vendors)
                        {
                            dynamicFields.vendors = true;
                        }
                        if (j == (int)coreFields.file)
                        {
                            dynamicFields.file = true;
                        }

                        if (j == (int)coreFields.erpRef)
                        {
                            dynamicFields.erpRef = true;
                        }

                        if (j == (int)coreFields.deliveryDT)
                        {
                            dynamicFields.deliveryDT = true;
                        }
                  
                }


                return new jResponse(false, "", dynamicFields);
            }

        }

        /*
       - dispatchPalnGet()
       - Purpose: Dispatch plan get.
       - In: class myParams {
                        userID
                   }
       - Out: 
      */
        [HttpPost, HttpGet, Route("dispatchPlan/get")]
        public dynamic dispatchPlanGet(dynamic myParams)
        {
            // #region "Validation"
            if (myParams == null || myParams.userID == null || myParams.userID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            DateTime currentDT = genApiController.getDate();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Get dispatch plan
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Check for valid executive
                var dbExecutive = db.Executives.Where(i => i.ExecutiveID == userID && !i.IsDeleted).FirstOrDefault();

                if (dbExecutive == null)
                {
                    return new jResponse(true, "This executive does not exist", null);
                }

                var dbDispatchPlan = db.sp_getDispatchPlanList(userID).ToList();
                //var dbDispatchPlan = (from dbs in db.Shipments
                //                      join dbc in db.Customers on dbs.CustomerID equals dbc.CustomerID
                //                      join dbe in db.Executives on dbs.CreatedByUserID equals dbe.ExecutiveID
                //                      join dbst in db.ServiceTypes on dbs.ServiceIDs.Contains('~' + dbst.ServiceTypeID +)
                //                      where !dbs.IsDeleted && !dbc.IsDeleted && !dbe.IsDeleted
                //                      select new dispatchDetail
                //                      {
                //                          shipmentID = dbs.ShipmentID,
                //                          pickup = dbs.PickupDT,
                //                          delivery = dbs.DeliveryDT,
                //                          customerID = dbc.CustomerID,
                //                          companyName = dbc.CompanyName,
                //                          container = dbs.Containers,
                //                          fileName = dbs.FileName,
                //                          serviceIDs = dbs.ServiceIDs
                //                      }).OrderBy(i => i.pickup).ToList();

                //code to show disptach plan based on service rights
                var dbServiceTypesInfo = db.ServiceTypes.Where(r => r.IsDeleted == false);
                List<sp_getDispatchPlanList_Result> result = new List<sp_getDispatchPlanList_Result>();
                List<string> allowedServices = new List<string>();
                foreach (string serviceRight in dbExecutive.ServiceRights.Split('~'))
                {
                    if (serviceRight == "")
                        continue;
                    foreach (var serviceType in dbServiceTypesInfo)
                    {
                        if (serviceType.ServiceTypeID == int.Parse(serviceRight))
                            allowedServices.Add(serviceType.ServiceName);
                    }
                }
                foreach (var plan in dbDispatchPlan)
                {
                    if (allowedServices.Contains(plan.serviceName))
                        result.Add(plan);
                }
                //end

                //return new jResponse(false, "", dbDispatchPlan);
                return new jResponse(false, "", result);
                
            }
        }

        /*
            - startWorkonRFQ()
            - Purpose: start work on rfq.
            - In: class myParams {
                             userID,
                             reqForQuoteID  
                        }
            - Out: reqForQuoteID
       */
        [HttpPost, HttpGet, Route("rfq/start")]
        public dynamic startWorkonRFQ(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Start rfq
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                
                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "3";
                dbRFQInfo.dbr.StartWorkDT = currentDT;
                dbRFQInfo.dbr.StartedByUserID = userID;
                dbRFQInfo.dbr.UpdatedDT = currentDT;
                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "RFQ is started!", reqForQuoteID);

            }

        }

        /*
           - completeRFQ()
           - Purpose: complete rfq.
           - In: class myParams {
                            userID,
                            reqForQuoteID  
                       }
           - Out: reqForQuoteID
        */
        [HttpPost, HttpGet, Route("rfq/complete")]
        public dynamic completeRFQ(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Completion of rfq
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                // Get RFQ detail
                var dbRFQInfo = (from dbr in db.RFQs
                                 where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted
                                 select new
                                 {
                                     dbr
                                 }).FirstOrDefault();


                // Check if reqForQuoteInfo is null or inquiryID same or not
                if (dbRFQInfo == null || dbRFQInfo.dbr.RequestForQuoteID != reqForQuoteID)
                {
                    return new jResponse();
                }

                dbRFQInfo.dbr.Status = "4";
                dbRFQInfo.dbr.DispatchedByUserID = userID;
                dbRFQInfo.dbr.DispatchedDT = currentDT;
                dbRFQInfo.dbr.UpdatedDT = currentDT;
                db.Entry(dbRFQInfo.dbr).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "RFQ is completed!", reqForQuoteID);

            }

        }

        /*
           - getQuoteComponents()
           - Purpose: Get quote component.
           - In: class myParams {
                            userID,
                            reqForQuoteID,
                            status  
                       }
           - Out: reqForQuoteID
        */
        [HttpPost, HttpGet, Route("quoteComponent/get")]
        public dynamic getQuoteComponents(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.reqForQuoteID < 1)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameter
            int userID = myParams.userID;
            int reqForQuoteID = myParams.reqForQuoteID;
            string status = myParams.status;
            DateTime currentDT = genApiController.getDate();
            RFQ rfqInfo = new RFQ();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Get component for RFQ
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                componentDetail detail =  new componentDetail();                                
                detail.Rates = (from dbfr in db.FinalRates
                                    where dbfr.RFQID == reqForQuoteID
                                    select dbfr.Rates
                                    ).FirstOrDefault();

                // Get components
                detail.Components +=  (from dbr in db.RFQs
                                        join dbq in db.Quotations on dbr.RequestForQuoteID equals dbq.RequestForQuoteID
                                        where dbr.RequestForQuoteID == reqForQuoteID && !dbr.IsDeleted && dbq.Status == status && !dbq.IsDeleted
                                        select dbq.Components
                                        ).FirstOrDefault();

                detail.RatingData += (from dbr in db.Rating
                                      where dbr.RFQID == reqForQuoteID
                                      select dbr.RatingData
                                      ).FirstOrDefault();

                return new jResponse(false, "", detail);
            }

        }

        /*
           - setFinalRates()
           - Purpose: Set final rates.
           - In: class myParams {
                            userID,                                       
                            rating,
                            reason,
                            components,
                            componentIDs,
                            componentAcceptedCosts,
                            componentActualCosts
                       }
           - Out: Success/Failure 
        */
        [HttpPost, HttpGet, Route("finalRates/set")]
        public dynamic setFinalRates(dynamic myParams)
        {
            #region "Validations"

            if (myParams == null || myParams.userID < 1 || myParams.components == 0 
                || myParams.reason == "")
            {
                return new jResponse();
            }

            int userID = myParams.userID;
            int rateValue = myParams.rating;
            string reason = myParams.reason;
            int components = myParams.components;
            DateTime currentDT = genApiController.getDate();

            string rates = myParams.rates;
            string ratingData = myParams.ratingData;
            dynamic componentIDs = myParams.componentIDs;
            dynamic componentAcceptedCosts = myParams.componentAcceptedCosts;
            dynamic componentActualCosts = myParams.componentActualCosts;
            int RFQID = myParams.RFQID;
            #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {   

                if (components > 0)
                {
                    try
                    {
                        for (int i = 0; i < components; i++)
                        {
                            FinalRates finalRate = new FinalRates();
                            finalRate.RFQID = RFQID;
                            finalRate.ComponentID = componentIDs[i];
                            finalRate.AcceptedCost = componentAcceptedCosts[i];
                            finalRate.ActualCost = componentActualCosts[i];
                            finalRate.IsDeleted = false;
                            finalRate.CreatedDT = currentDT;
                            finalRate.CreatedByUserID = userID;
                            finalRate.Rates = rates;

                            db.Entry(finalRate).State = EntityState.Added;
                        }

                        Rating rating = new Rating();
                        rating.RFQID = RFQID;
                        rating.Stars = rateValue;
                        rating.Reason = reason;
                        rating.RatingData = ratingData;

                        db.Entry(rating).State = EntityState.Added;

                        // Change RFQ status to 5
                        var dbRFQ = db.RFQs.Where(RFQ => RFQ.RequestForQuoteID == RFQID).FirstOrDefault();
                        dbRFQ.Status = "5";
                        dbRFQ.UpdatedDT = currentDT;

                        db.Entry(dbRFQ).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return new jResponse(true, ex.Message, null);
                    }
                }
                return new jResponse(false, "Feedback & Actual costs received!", null);
            }
        }

        #endregion

        #region "Classes"

        public class smsParams
        {
            public string rfqID { get; set; }
            public string link { get; set; }
            public string sourceCity { get; set; }
            public string destinationCity { get; set; }
        }

        public class smsToTransporterParams
        {
            public string rfqID { get; set; }
            public string customerName { get; set; }
        }

        public class emailParams
        {
            public string rfqID { get; set; }
            public string product { get; set; }
            public string pickup { get; set; }
            public string delivery { get; set; }
            public string quoteby { get; set; }
            public string emailAddress { get; set; }
            public string source { get; set; }
            public string destination { get; set; }
            public string link { get; set; }
            public string companyName { get; set; }
            public string displayName { get; set; }
            public dynamic emailDynamicFields { get; set; }
        }

        public class transpoterDetails
        {
            public int transpoterID { get; set; }
            public string email { get; set; }
            public string fName { get; set; }
            public string lName { get; set; }
            public string title { get; set; }
            public string sendCustomerName { get; set; }
            public string responseLink { get; set; }
            public string sourceName { get; set; }
            public string destination { get; set; }
            public string companyName { get; set; }
            public string mobile { get; set; }
            public string alternateMobile { get; set; }
        }

        public class rfqDetails
        {
            public int reqForQuoteID { get; set; }
            public int customerID { get; set; }
            public string customerName { get; set; }
            //public List<customerDetails> customerDetail { get; set; }
            public string sourceName { get; set; }
            public string destination { get; set; }
            public string product { get; set; }
            public DateTime pickUpDT { get; set; }
            public DateTime deliveryDT { get; set; }
            public DateTime quoteByDT { get; set; }
            public string details { get; set; }
            public bool allowLaterDelivery { get; set; }
            public string status { get; set; }
            public string transporterIDs { get; set; }
            public List<transporterDetails> transporterDetail { get; set; }
            public string sourceAddress { get; set; }
            public string destinationAddress { get; set; }
            public dynamic response { get; set; }
            public bool isCloseEarly { get; set; }
            public string sourceState { get; set; }
            public string destinationState { get; set; }
            public string orderNo { get; set; }
            public string routeName { get; set; }
            public string fileName { get; set; }
            public DateTime? dispatchDT { get; set; }
            public string createdBy { get; set; }
            public string dispatchedBy { get; set; }
            public string acceptedBy { get; set; }
            public int zoneID { get; set; }
            public string zoneName { get; set; }
            public DateTime? timeSlotFromDT { get; set; }
            public DateTime? timeSlotToDT { get; set; }
            public DateTime? vehicleEntryDT { get; set; }
            public DateTime? vehicleReadyDT { get; set; }
            public DateTime? clearanceDT { get; set; }
            public string vehicleNumber { get; set; }
            public DateTime? reminderSentDate { get; set; }
            public int paymentTerm { get; set; }
            public string fixedTerm { get; set; }
        }

        public class transporterDetails
        {
            public int transporterID { get; set; }
            public string transporterName { get; set; }
            public string transporterCompany { get; set; }
        }

        public class qutationDetails
        {
            public int quoteID { get; set; }
            public int reqForQuoteID { get; set; }
            public int transporterID { get; set; }
            public int executiveID { get; set; }
            public string paymentTerms { get; set; }
            public string status { get; set; }
            public string note { get; set; }
            public string transpoterName { get; set; }
            public decimal rate { get; set; }
            public string transCompanyName { get; set; }
            public string mobile { get; set; }
            public string email { get; set; }
            public DateTime pickUpDT { get; set; }
            public DateTime deliveryDT { get; set; }
            public DateTime? extendDeliveryDT { get; set; }
            public string components { get; set; }
            public decimal exchangeRate { get; set; }
            public DateTime createdDT { get; set; }
            public int roundNumber { get; set; }
        }

        public class emailDetail
        {
            public string rfqID { get; set; }
            public string product { get; set; }
            public string pickup { get; set; }
            public string delivery { get; set; }
            public string charges { get; set; }
            public string emailAddress { get; set; }
            public string source { get; set; }
            public string destination { get; set; }
            public string link { get; set; }
            public string companyName { get; set; }
            public string payment { get; set; }
            public string notes { get; set; }
            public string displayName { get; set; }
            public string quoteCount { get; set; }
            public string userName { get; set; }
        }

        public class zone
        {
            public int zoneID { get; set; }
            public string zoneName { get; set; }
        }

        public class schedule
        {
            public dynamic scheduleList { get; set; }
        }

        public class report
        {
            public dynamic currentWorkLoadList { get; set; }
            public dynamic openRFQCount { get; set; }
        }

        public class quoteTypeDetail
        {
            public int quoteTypeID { get; set; }
            public string quoteTypeName { get; set; }
            public string paidBy { get; set; }
            public string type { get; set; }
            public int serviceID { get; set; }
            public string serviceName { get; set; }
        }

        public class transporters
        {
            public int transporterID { get; set; }
            public string companyName { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string title { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string mobile { get; set; }
            public string alternateMobile { get; set; }
            public string notes { get; set; }
            public bool isDomestic { get; set; }
            public bool isExport { get; set; }
        }

        public class fieldType
        {
            public int quoteTypeID { get; set; }
            public dynamic fields { get; set; }
            public string  coreFields { get; set; }
            public bool customer { get; set; }
            public bool quoteby { get; set; }
            public bool term { get; set; }
            public bool file { get; set; }
            public bool vendors { get; set; }
            public bool note { get; set; }
            public bool erpRef { get; set; }
            public bool deliveryDT { get; set; }
            public string completionText { get; set; }
            public int serviceTypeID { get; set; }
            public string pickupText { get; set; }
        }

        public class dispatchDetail
        {
            public int shipmentID { get; set; }
            public DateTime? pickup { get; set; }
            public DateTime delivery { get; set; }
            public string sourece { get; set; }
            public int customerID { get; set; }
            public string companyName { get; set; }
            public int container { get; set; }
            public string fileName { get; set; }
            public string serviceIDs { get; set; }
        }

        public class componentDetail
        {
            public string Components { get; set; }
            public string Rates { get; set; }
            public string RatingData { get; set; }
        }


        #endregion

    }
}