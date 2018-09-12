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


namespace transporterQuote.API
{
    [RoutePrefix("api/admin")]
    public class adminApiController : ApiController
    {
        #region "Functions"

        /*
           - setTransporter()
           - Purpose: Add transporter.
           - In: class myParams {
                           userID,
                           transporterID,
                           companyName,
                           email,
                           city,
                           state,
                           title,
                           fname,
                           lname,
                           mobile,
                           alternateMobile,
                           notes,
                           isDeleted,
                           isExport,
                           isDomestic,
                           serviceTypeID
                       }
           - Out: Success/Failure 
        */
        [HttpPost, HttpGet, IsSuperAdmin, Route("transporter/set")]
        public dynamic setTransporter(dynamic myParams)
        {
            // #region "Validations"

            // Basic validations
            if(myParams == null || myParams.companyName == "" || myParams.email == "" || myParams.city  == "" || myParams.state == ""
                || myParams.title == "" || myParams.fname == "" || myParams.lname == "" || myParams.mobile == "")
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
            int adminID = Convert.ToInt32(myParams.userID);
            int transporterID = Convert.ToInt32(myParams.transporterID);
            string companyName = myParams.companyName;
            string email = myParams.email;
            string city = myParams.city;
            string state = myParams.state;
            string title = myParams.title;
            string fname = myParams.fname;
            string lname = myParams.lname;
            string mobile = myParams.mobile;
            string alternateMobile = myParams.alternateNobile;
            string notes = myParams.notes;
            int insertedTransporterID = 0;
            bool isDeleted = myParams.isDeleted;
            bool isDomestic = myParams.isDomestic;
            bool isExport = myParams.isExport;
            string serviceTypeID = myParams.serviceTypeID;
            DateTime currentDT = genApiController.getDate();
            genApiController gen = new genApiController();

            // Verify user
            if (myToken.userID != adminID)
            {
                return new jResponse(true, "You're not allowed to access this API.", null);
            }

            // Verify mobile number
            if (mobile.Length != 10)
            {
                return new jResponse(true, "Please enter a valid mobile number.", true);
            }

            // Verify alternate mobile number
            if (alternateMobile != null && alternateMobile != "" && alternateMobile.Length != 10)
            {
                return new jResponse(true, "Please enter a valid alternate mobile number.", true);
            }

            // #endregion

            // Add new transporter
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                

                if (transporterID == 0)
                {
                    // Check for duplicate value
                    if (db.Transporters.Any(i => i.Email == email && i.CompanyName == companyName && !i.IsDeleted))
                    {
                        return new jResponse(true, "Another transporter of this name already exists.", null);
                    }

                    // Add trasporter
                    Transporter addTransporter = new Transporter();
                    addTransporter.CompanyName = companyName;
                    addTransporter.Email = email;
                    addTransporter.Mobile = mobile;
                    addTransporter.Title = title;
                    addTransporter.FirstName = fname;
                    addTransporter.LastName = lname;
                    addTransporter.City = city;
                    addTransporter.State = state;
                    addTransporter.AlternateMobile = alternateMobile == null ? "" : alternateMobile;
                    addTransporter.Notes = notes;
                    addTransporter.CreatedDT = currentDT;
                    addTransporter.UpdatedDT = currentDT;
                    addTransporter.IsDeleted = false;
                    addTransporter.IsDomestic = isDomestic;
                    addTransporter.IsExport = isExport;
                    addTransporter.Password = currentDT.Second.ToString("00").Substring(0, 1) + mobile.Substring(5, 1) + mobile.Substring(1, 1) + currentDT.Millisecond.ToString("000").Substring(0, 1);
                    addTransporter.ServiceTypeID = serviceTypeID;

                    db.Entry(addTransporter).State = EntityState.Added;
                    db.SaveChanges();
                    insertedTransporterID = addTransporter.TransporterID;
                   // help_sendPassword(mobile, addTransporter.Password);
                }
                else
                {
                    // Update transporter
                    var transporterDB = db.Transporters.Where(i => i.TransporterID == transporterID && !i.IsDeleted).FirstOrDefault();

                    if (transporterDB == null)
                    {
                        return new jResponse(true, "Transporter does not exist.", null);
                    }

                    if (!isDeleted) { 

                        transporterDB.CompanyName = companyName;
                        transporterDB.Email = email;
                        transporterDB.Mobile = mobile;
                        transporterDB.Title = title;
                        transporterDB.FirstName = fname;
                        transporterDB.LastName = lname;
                        transporterDB.City = city;
                        transporterDB.State = state;
                        transporterDB.AlternateMobile = alternateMobile == null ? "" : alternateMobile;
                        transporterDB.Notes = notes;
                        transporterDB.UpdatedDT = currentDT;
                        transporterDB.IsDomestic = isDomestic;
                        transporterDB.IsExport = isExport;
                        transporterDB.ServiceTypeID = serviceTypeID;
                    } else
                    {
                        //string tid = Convert.ToString(transporterID);
                        //if (db.RFQs.Any(i => i.Transporters.Contains("~"+ tid + "~") && i.DeliveryBy >= currentDT && !i.IsDeleted))
                        //{
                        //    return new jResponse(true, "This transporter already in use.", null);
                        //}
                        transporterDB.IsDeleted = isDeleted;
                        transporterDB.UpdatedDT = currentDT;
                    }

                    db.Entry(transporterDB).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Trasnsporter successfully " + (transporterID == 0 ? "added." : isDeleted ? "deleted." : "updated."), insertedTransporterID);

            }
        }

        /*
          - setSource()
          - Purpose: Add source.
          - In: class myParams {
                          userID,
                          sourceID,
                          location,
                          type,
                          address1,
                          address2,
                          city,
                          state,
                          notes,
                          isDeleted
                      }
          - Out: Success/Failure 
       */
        [HttpPost, HttpGet, IsSuperAdmin, Route("source/set")]
        public dynamic setSource (dynamic myParams)
        {
            // #region "Validations"

            if (myParams == null || myParams.location == "" || myParams.type == "" || myParams.address1 == "" 
                 || myParams.city == "" || myParams.state == "")
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
            int sourceID = myParams.sourceID;
            string location = myParams.location;
            string type = myParams.type;
            string address1 = myParams.address1;
            string address2 = myParams.address2;
            string city = myParams.city;
            string state = myParams.state;
            string notes = myParams.notes;
            int insertedSourceID = 0;
            bool isDeleted = myParams.isDeleted;
            DateTime currentDT = genApiController.getDate();
            

            // #endregion

            // Add source
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                

                if (sourceID == 0)
                {
                    // Check for duplicate value
                    if (db.Sources.Any(i => i.Location == location && i.Type == type && !i.IsDeleted))
                    {
                        return new jResponse(true, "Another source of this name already exists.", null);
                    }

                    // Add source
                    Source addSource = new Source();
                    addSource.Location = location;
                    addSource.Type = type;
                    addSource.Address1 = address1;
                    addSource.Address2 = address2 == null ? "" : address2;
                    addSource.City = city;
                    addSource.State = state;
                    addSource.Notes = notes == null ? "" : notes;
                    addSource.CreatedDT = currentDT;
                    addSource.UpdatedDT = currentDT;
                    addSource.IsDeleted = false;
               
                    db.Entry(addSource).State = EntityState.Added;
                    db.SaveChanges();
                    insertedSourceID = addSource.SourceID;
                   
                } else
                {
                    // Update source
                    var sourceDB = db.Sources.Where(i => i.SourceID == sourceID && !i.IsDeleted).FirstOrDefault();

                    if (sourceDB == null)
                    {
                        return new jResponse(true, "Source does not exist.",null);
                    }

                    if (!isDeleted) { 
                    sourceDB.Location = location;
                    sourceDB.Type = type;
                    sourceDB.Address1 = address1;
                    sourceDB.Address2 = address2 == null ? "" : address2;
                    sourceDB.City = city;
                    sourceDB.State = state;
                    sourceDB.Notes = notes == null ? "" : notes;
                    sourceDB.UpdatedDT = currentDT;
                    } else
                    {
                        //if (db.RFQs.Any(i => i.SourceID == sourceID && i.DeliveryBy >= currentDT && !i.IsDeleted))
                        //{
                        //    return new jResponse(true, "This source already in use.", null);
                        //}
                        sourceDB.IsDeleted = isDeleted;
                        sourceDB.UpdatedDT = currentDT;
                    }

                    db.Entry(sourceDB).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Source successfully " + (sourceID == 0 ? "added." : isDeleted ? "deleted." : "updated.") , insertedSourceID);
            }
        }

        /*
         - setExecutive()
         - Purpose: Add executive.
         - In: class myParams {
                         userID,
                         executiveID,
                         executiveName,
                         email,
                         mobile,
                         canSelectTransporter,
                         canCreateRFQ,
                         canCloseRFQ,
                         canSelectforCustomer,
                         designation,
                         notes,
                         isDeleted,
                         zoneIDs,
                         rights
                     }
         - Out: Success/Failure 
       */
        [HttpPost, HttpGet, IsSuperAdmin, Route("executive/set")]
        public dynamic setExecutive (dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.executiveName == "" || myParams.email == "" || myParams.mobile  == "" || myParams.designation == "")
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
            int executiveID = myParams.executiveID;
            string executiveName = myParams.executiveName;
            string email = myParams.email;
            string mobile = myParams.mobile;
            string designation = myParams.designation;
            bool canSelectTransporter = myParams.canSelectTransporter;
            bool canCreateRFQ = myParams.canCreateRFQ;
            bool canCloseRFQ = myParams.canCloseRFQ;
            bool canSelectforCustomer = myParams.canSelectforCustomer;
            string notes = myParams.notes;
            DateTime currentDT = genApiController.getDate();
            int insertedExecutiveID = 0;
            bool isDeleted = myParams.isDeleted;
            string zoneIDs = myParams.zoneIDs;
            string rights = myParams.rights;
            string serviceRights = myParams.serviceRights;
            // #endregion

            // Add executive
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                if (executiveID == 0)
                {
                    // Check for duplicate value
                    if (db.Executives.Any(i => i.ExecutiveName == executiveName && i.Email == email && !i.IsDeleted))
                    {
                        return new jResponse(true, "Another executive of this name already exists.", null);
                    }

                    // Add executive
                    Executive addExecutive = new Executive();
                    addExecutive.ExecutiveName = executiveName;
                    addExecutive.Email = email;
                    addExecutive.Mobile = mobile;
                    addExecutive.Designation = designation;
                    addExecutive.CanSelectTransporter = canSelectTransporter;
                    addExecutive.CanSelectForCustomer = canSelectforCustomer;
                    addExecutive.CanCloseRFQ = canCloseRFQ;
                    addExecutive.CanCreateRFQ = canCreateRFQ;
                    addExecutive.Notes = notes == null ? "" : notes;
                    addExecutive.IsDeleted = false;
                    addExecutive.CreatedDT = currentDT;
                    addExecutive.UpdatedDT = currentDT;
                    addExecutive.Password = currentDT.Second.ToString("00").Substring(0, 1) + mobile.Substring(5, 1) + mobile.Substring(1, 1) + currentDT.Millisecond.ToString("000").Substring(0, 1);
                    addExecutive.ZoneIDs = zoneIDs;
                    addExecutive.Rights = rights;
                    addExecutive.ServiceRights = serviceRights;

                    db.Entry(addExecutive).State = EntityState.Added;
                    db.SaveChanges();
                    insertedExecutiveID = addExecutive.ExecutiveID;

                    help_sendPassword(mobile, addExecutive.Password);
                }
                else
                {
                    var executiveDB = db.Executives.Where(i => i.ExecutiveID == executiveID && !i.IsDeleted).FirstOrDefault();

                    if (executiveDB == null)
                    {
                        return new jResponse(true, "Executive does not exist.", null);
                    }

                    if (!isDeleted) { 
                        executiveDB.ExecutiveName = executiveName;
                        executiveDB.Email = email;
                        executiveDB.Mobile = mobile;
                        executiveDB.Designation = designation;
                        executiveDB.CanSelectTransporter = canSelectTransporter;
                        executiveDB.CanSelectForCustomer = canSelectforCustomer;
                        executiveDB.CanCloseRFQ = canCloseRFQ;
                        executiveDB.CanCreateRFQ = canCreateRFQ;
                        executiveDB.Notes = notes == null ? "" : notes;
                        executiveDB.UpdatedDT = currentDT;
                        executiveDB.ZoneIDs = zoneIDs;
                        executiveDB.Rights = rights;
                        executiveDB.ServiceRights = serviceRights;
                    } else
                    {
                        //if (db.RFQs.Any(i => i.ExecutiveID == executiveID && i.DeliveryBy >= currentDT && !i.IsDeleted))
                        //{
                        //    return new jResponse(true, "This executive already in use.", null);
                        //}
                        executiveDB.IsDeleted = isDeleted;
                        executiveDB.UpdatedDT = currentDT;
                    }

                    db.Entry(executiveDB).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Executive successfully " + (executiveID == 0 ? "added." : isDeleted ? "deleted." : "updated.") , insertedExecutiveID);
            }
        }

        /*
        - getExecutives()
        - Purpose: get list of executive.
        - In: class myParams {
                        userID
                    }
        - Out: executives
      */
        [HttpPost, HttpGet, IsSuperAdmin, Route("executives/get")]
        public dynamic getExecutives(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID != 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            genApiController gen = new genApiController();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;

            // Get executive list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var executives = (from dbe in db.Executives
                                  where !dbe.IsDeleted && dbe.ExecutiveID > 0
                                  select new executive
                                  {
                                     executiveID = dbe.ExecutiveID,
                                     executiveName = dbe.ExecutiveName,
                                     designation = dbe.Designation,
                                     email = dbe.Email,
                                     mobile = dbe.Mobile,
                                     canCreateRFQ = dbe.CanCreateRFQ,
                                     canCloseRFQ = dbe.CanCloseRFQ,
                                     canSelectForCustomer = dbe.CanSelectForCustomer,
                                     canSelectTransporter = dbe.CanSelectTransporter,
                                     notes = dbe.Notes,
                                     zoneIDs = dbe.ZoneIDs,
                                     rights = dbe.Rights,
                                     serviceRights = dbe.ServiceRights
                                  }).OrderBy(i => i.executiveName).ToList();

                foreach(var i in executives)
                {
                    List<int> rightIDs = gen.splitString('~', i.rights).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                    
                   foreach (var j in rightIDs)
                    {
                        if (j == (int)executiveRights.forwarded)
                        {
                            i.canForward = true;
                        }

                        if (j == (int)executiveRights.dispatchCreate)
                        {
                            i.canDispatchCreate = true;
                        }

                        if (j == (int)executiveRights.updateStatus)
                        {
                            i.canUpdateStatus = true;
                        }

                        if (j == (int)executiveRights.scheduled)
                        {
                            i.canScheduled = true;
                        }

                        if (j == (int)executiveRights.loading)
                        {
                            i.canLoading = true;
                        }
                        if (j == (int)executiveRights.dispatchView)
                        {
                           i.canDispatchView = true;
                        }

                        if (j == (int)executiveRights.isSupervisor)
                        {
                            i.isSupervisor = true;
                        }

                        if(j == (int)executiveRights.isDomestic)
                        {
                            i.isDomestic = true;
                        }

                        if (j == (int)executiveRights.isExport)
                        {
                            i.isExport = true;
                        }

                        if (j == (int) executiveRights.canStart)
                        {
                            i.canStart = true;
                        }

                        if (j == (int) executiveRights.canFinish)
                        {
                            i.canFinish = true;
                        }

                        if (j == (int)executiveRights.canFeedback)
                        {
                            i.canFeedback = true;
                        }
                    }
                }

                return new jResponse(false, "", executives);
            }
        }

        /*
       - getTrasporters()
       - Purpose: get list of transporter.
       - In: class myParams {
                       userID
                   }
       - Out: transporters
       */
        [HttpPost, HttpGet, Route("transporters/get")]
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
            if (myToken.userID != userID)
            {
                return new jResponse(true, "User does not exist.", null);
            }

            // Get transporter list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var transporters = (from dbt in db.Transporters
                                  where !dbt.IsDeleted
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
                                      isExport = dbt.IsExport,
                                      serviceTypeID = dbt.ServiceTypeID
                                  }).OrderBy(i => i.companyName).ToList();

                return new jResponse(false, "", transporters);
            }
        }

        /*
          - getSources()
          - Purpose: get list of transporter.
          - In: class myParams {
                          userID
                      }
          - Out: sources
       */
        [HttpPost, HttpGet, Route("sources/get")]
        public dynamic getSources(dynamic myParams)
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

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // Get source list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var sources = (from dbs in db.Sources
                                    where !dbs.IsDeleted
                                    select new source
                                    {
                                        sourceID = dbs.SourceID,
                                        location = dbs.Location,
                                        type = dbs.Type,
                                        address1 = dbs.Address1,
                                        address2 = dbs.Address2,
                                        city = dbs.City,
                                        state = dbs.State,
                                        notes = dbs.Notes
                                    }).OrderBy(i => i.location).ToList();

                return new jResponse(false, "", sources);
            }
        }

        /*
         - resetPassword()
         - Purpose: reset password.
         - In: class myParams {
                         userID,
                         mobile,
                         email
                     }
         - Out: Send sms
      */
        [HttpPost, HttpGet, IsSuperAdmin, Route("password/reset")]
        public dynamic resetPassword(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID != 0 || (myParams.mobile == "" && myParams.email == ""))
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
            string mobile = myParams.mobile;
            string email = myParams.email;
            string password = "";
            DateTime currentDT = genApiController.getDate();

            // Password change and send
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                password = currentDT.Second.ToString("00").Substring(0, 1) + mobile.Substring(5, 1) + mobile.Substring(1, 1) + currentDT.Millisecond.ToString("000").Substring(0, 1);

                // Check for valid mobile number
                var dbExecutiveInfo = db.Executives.Where(i => (i.Mobile == mobile || i.Email == email) && !i.IsDeleted).FirstOrDefault();

                if (dbExecutiveInfo != null)
                {
                    help_sendPassword(dbExecutiveInfo.Mobile, password);
                    dbExecutiveInfo.Password = password;
                    dbExecutiveInfo.CreatedDT = currentDT;
                    db.Entry(dbExecutiveInfo).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    return new jResponse(true, "Please enter valid mobile number.", null);
                }
                return new jResponse(false, "Your new password has been sent to you by SMS.", null);
            }
        }

        /*
        - setCustomer()
        - Purpose: Add customer.
        - In: class myParams {
                        userID,
                        customerID,
                        companyName,
                        email,
                        city,
                        state,
                        title,
                        fname,
                        lname,
                        mobile,
                        alternateMobile,
                        notes,
                        isDeleted,
                        country
                    }
        - Out: Success/Failure 
     */
        [HttpPost, HttpGet, Route("customer/set")]
        public dynamic setCustomer(dynamic myParams)
        {
            // #region "Validations"

            // Basic validations
            if (myParams == null || myParams.companyName == "" || myParams.email == "" || myParams.city == "" || myParams.state == ""
                || myParams.title == "" || myParams.fname == "" || myParams.lname == "" || myParams.mobile == "")
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
            int adminID = Convert.ToInt32(myParams.userID);
            int customerID = Convert.ToInt32(myParams.customerID);
            string companyName = myParams.companyName;
            string email = myParams.email;
            string city = myParams.city;
            string state = myParams.state;
            string title = myParams.title;
            string fname = myParams.fname;
            string lname = myParams.lname;
            string mobile = myParams.mobile;
            string alternateMobile = myParams.alternateNobile;
            string notes = myParams.notes;
            string address1 = myParams.address1;
            string address2 = myParams.address2;
            string country = myParams.country;
            int insertedCustomerID = 0;
            bool isDeleted = myParams.isDeleted;
            DateTime currentDT = genApiController.getDate();

            // Verify user
            if (myToken.userID != adminID)
            {
                return new jResponse(true, "You're not allowed to access this API.", null);
            }

            // Verify mobile number
            if (mobile.Length != 10)
            {
                return new jResponse(true, "Please enter a valid mobile number.", true);
            }

            // Verify alternate mobile number
            if (alternateMobile != null && alternateMobile != "" && alternateMobile.Length != 10)
            {
                return new jResponse(true, "Please enter a valid alternate mobile number.", true);
            }

            // #endregion

            // Add new customer
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                if (customerID == 0) { 

                // Check for duplicate value
                if (db.Customers.Any(i => i.Email == email && i.CompanyName == companyName && !i.IsDeleted))
                {
                    return new jResponse(true, "Another customer of this name already exists.", null);
                }

                // Add customer
                Customer addCustomer = new Customer();
                addCustomer.CompanyName = companyName;
                addCustomer.Email = email;
                addCustomer.Mobile = mobile;
                addCustomer.Title = title;
                addCustomer.FirstName = fname;
                addCustomer.LastName = lname;
                addCustomer.City = city;
                addCustomer.State = state;
                addCustomer.AlternateMobile = alternateMobile == null ? "" : alternateMobile;
                addCustomer.Password = currentDT.Second.ToString("00").Substring(0, 1) + mobile.Substring(5, 1) + mobile.Substring(1, 1) + currentDT.Millisecond.ToString("000").Substring(0, 1); ;
                addCustomer.Notes = notes;
                addCustomer.CreatedDT = currentDT;
                addCustomer.UpdatedDT = currentDT;
                addCustomer.IsDeleted = false;
                addCustomer.Address1 = address1;
                addCustomer.Address2 = address2 == null ? "" : address2;
                addCustomer.Country = country;

                db.Entry(addCustomer).State = EntityState.Added;
                db.SaveChanges();
                insertedCustomerID = addCustomer.CustomerID;
                // help_sendPassword(mobile, addCustomer.Password);
                } else
                {
                    var customerDB = db.Customers.Where(i => i.CustomerID == customerID && !i.IsDeleted).FirstOrDefault();

                    if (customerDB == null)
                    {
                        return new jResponse(true, "customer does not exist.", null);
                    }

                    if (!isDeleted) { 
                        customerDB.CompanyName = companyName;
                        customerDB.Email = email;
                        customerDB.Mobile = mobile;
                        customerDB.Title = title;
                        customerDB.FirstName = fname;
                        customerDB.LastName = lname;
                        customerDB.City = city;
                        customerDB.State = state;
                        customerDB.AlternateMobile = alternateMobile == null ? "" : alternateMobile;
                        customerDB.Notes = notes;
                        customerDB.CreatedDT = currentDT;
                        customerDB.UpdatedDT = currentDT;
                        customerDB.Address1 = address1;
                        customerDB.Address2 = address2;
                        customerDB.Country = country;
                    } else
                    {
                        //if (db.RFQs.Any(i => i.CustomerID == customerID && i.DeliveryBy >= currentDT && !i.IsDeleted))
                        //{
                        //    return new jResponse(true, "This customer already in use.", null);
                        //}
                        customerDB.IsDeleted = isDeleted;
                        customerDB.UpdatedDT = currentDT;
                    }
                    db.Entry(customerDB).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Customer successfully " + (customerID == 0 ? "added." : isDeleted ? "deleted." : "updated."), insertedCustomerID);

            }
        }

        /*
          - getCustomers()
          - Purpose: get list of customer.
          - In: class myParams {
                          userID
                      }
          - Out: customers
       */
        [HttpPost, HttpGet, Route("customers/get")]
        public dynamic getCustomers(dynamic myParams)
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

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }


            // Get executive list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var customers = (from dbc in db.Customers
                               where !dbc.IsDeleted
                               select new customer
                               {
                                   customerID = dbc.CustomerID,
                                   companyName = dbc.CompanyName,
                                   city = dbc.City,
                                   state = dbc.State,
                                   title = dbc.Title,
                                   fname = dbc.FirstName,
                                   lname = dbc.LastName,
                                   email = dbc.Email,
                                   mobile = dbc.Mobile,
                                   alternateMobile = dbc.AlternateMobile,
                                   notes = dbc.Notes,
                                   address1 = dbc.Address1,
                                   address2 = dbc.Address2,
                                   country = dbc.Country
                               }).OrderBy(i => i.companyName).ToList();

                return new jResponse(false, "", customers);
            }
        }

        /*
          - setRoute()
          - Purpose: set Route.
          - In: class myParams {
                          userID,
                          routeID,
                          code,
                          detail,
                          isDeleted
                      }
          - Out: insertedRouteID
       */
        [HttpPost, HttpGet, IsSuperAdmin, Route("route/set")]
        public dynamic setRoute(dynamic myParams)
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
            int routeID = myParams.routeID;
            int insertedRouteID = 0;
            string routeCode = myParams.code;
            string routeDetail = myParams.detail;
            bool isDeleted = myParams.isDeleted;
            DateTime currentDT = genApiController.getDate();

            if (myToken.userID != userID)
            {
                return new jResponse(true, "User does not exist.", null);
            }

            // Set route
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                if (routeID == 0) { 
                // Check for duplicate value
                if (db.Routes.Any(i => i.RouteCode == routeCode  && !i.IsDeleted))
                {
                    return new jResponse(true, "Another route of this name already exists.", null);
                }

                // Add source
                Route addRoute = new Route();
                addRoute.RouteCode = routeCode;
                addRoute.RouteDetail = routeDetail;
                addRoute.CreatedDT = currentDT;
                addRoute.IsDeleted = false;

                db.Entry(addRoute).State = EntityState.Added;
                db.SaveChanges();
                insertedRouteID = addRoute.RouteID;
                }
                else
                {
                    // Update source
                    var routeDB = db.Routes.Where(i => i.RouteID == routeID && !i.IsDeleted).FirstOrDefault();

                    if (routeDB == null)
                    {
                        return new jResponse(true, "Route does not exist.", null);
                    }

                    if (!isDeleted) { 
                    routeDB.RouteCode = routeCode;
                    routeDB.RouteDetail = routeDetail;
                    routeDB.CreatedDT = currentDT;
                    }  else
                    {
                        //if (db.RFQs.Any(i => i.RouteID == routeID && i.DeliveryBy >= currentDT && !i.IsDeleted))
                        //{
                        //    return new jResponse(true, "This route already in use.", null);
                        //}
                        routeDB.IsDeleted = true;
                    }

                    db.Entry(routeDB).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Route successfully " + (routeID == 0 ? "added." : isDeleted ? "deleted." : "updated."), insertedRouteID);

            }
       }

        /*
         - getRoute()
         - Purpose: Get Route.
         - In: class myParams {
                         userID
                     }
         - Out: routes
       */
        [HttpPost, HttpGet, Route("routes/get")]
        public dynamic getRoute(dynamic myParams)
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

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // Get route
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var routes = (from dbr in db.Routes
                              where !dbr.IsDeleted
                              select new route
                              {
                                  routeID = dbr.RouteID,
                                  routeCode = dbr.RouteCode,
                                  routeDetail = dbr.RouteDetail,
                                  routeInfo = dbr.RouteCode + " - " + dbr.RouteDetail
                                 }).OrderBy(i => i.routeInfo).ToList();

                return new jResponse(false, "", routes);
            }
        }

        /*
        - getZone()
        - Purpose: Get zone.
        - In: class myParams {
                        userID
                    }
        - Out: zones
      */
        [HttpPost, HttpGet, Route("zone/get")]
        public dynamic getZone(dynamic myParams)
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

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // Get zones
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var zones = (from dbz in db.Zones
                              where !dbz.IsDeleted
                              select new zone
                              {
                                  zoneID = dbz.ZoneID,
                                  zoneName = dbz.ZoneName
                              }).OrderBy(i => i.zoneName).ToList();

                return new jResponse(false, "", zones);
            }
        }

        /*
       - setQuoteType()
       - Purpose: Add quote type.
       - In: class myParams {
                    userID,
                    quoteTypeID,
                    quoteTypeName,
                    paidBy,
                    type,
                    components,
                    isDeleted
                   }
       - Out: Success/Failure 
    */
        [HttpPost, HttpGet, Route("quoteType/set")]
        public dynamic setQuoteType(dynamic myParams)
        {
            // #region "Validations"

            // Basic validations
            if (myParams == null || myParams.quoteTypeName == "" || myParams.paidBy == "" 
                || myParams.components == null)
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
            int userID = Convert.ToInt32(myParams.userID);
            int quoteTypeID = myParams.quoteTypeID;
            string quoteTypeName = myParams.quoteTypeName;
            string paidBy = myParams.paidBy;
            string type = "";
            dynamic components = myParams.components;
            int insertedQuoteTypeID = 0;
            bool isDeleted = myParams.isDeleted;
            bool isAllowNegotiation = myParams.isAllowNegotiation;
            DateTime currentDT = genApiController.getDate();
            int serviceTypeID = myParams.serviceTypeID;
            string coreField = myParams.coreField;
            dynamic customFields = myParams.customFields;

            // Verify user
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this API.", null);
            }

            // #endregion

            // Add new customer
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                if (quoteTypeID == 0)
                {
                    // Check for duplicate value
                    if (db.QuoteTypes.Any(i => i.QuoteTypeName == quoteTypeName && !i.IsDeleted))
                    {
                        return new jResponse(true, "Another quote type of this name already exists.", null);
                    }

                    QuoteType qt = new QuoteType();
                    qt.QuoteTypeName = quoteTypeName;
                    qt.PaidBy = paidBy;
                    qt.Type = "";
                    qt.CreatedByUserID = userID;
                    qt.CreatedDT = currentDT;
                    qt.LastUpdatedByUserID = null;
                    qt.LastUpdatedDT = null;
                    qt.IsDeleted = false;
                    qt.IsAllowNegotiation = isAllowNegotiation;
                    qt.ServiceTypeID = serviceTypeID;
                    qt.CoreField = coreField;

                    db.Entry(qt).State = EntityState.Added;
                    db.SaveChanges();
                    insertedQuoteTypeID = qt.QuoteTypeID;

                    foreach (var j in customFields)
                    {
                        Field addField = new Field();
                        addField.FieldName = j.fieldName;
                        addField.FieldType = j.typeID;
                        addField.IsRequired = false;
                        addField.IsDeleted = false;
                        addField.Choices = j.choice;
                        addField.CreatedDT = currentDT;
                        addField.CreatedByUserID = userID;
                        addField.UpdatedByUserID = null;
                        addField.UpdatedDT = null;
                        addField.QuoteTypeID = insertedQuoteTypeID;
                        addField.ShowCustomer = j.showCustomers;
                        addField.ShowVendors = j.showVendors;

                        db.Entry(addField).State = EntityState.Added;
                        db.SaveChanges();

                    }

                    foreach (var i in components)
                    {
                        QuoteTypeComponent addComponent = new QuoteTypeComponent();
                        addComponent.ComponentName = i.ComponentName;
                        addComponent.Type = i.Type;
                        addComponent.PaidBy = i.PaidBy;
                        addComponent.Currency = i.Currency;
                        addComponent.QuoteTypeID = insertedQuoteTypeID;
                        addComponent.CreatedByUserID = userID;
                        addComponent.CreatedDT = currentDT;
                        addComponent.LastUpdatedByUserID = null;
                        addComponent.LastUpdatedDT = null;
                        addComponent.IsDeleted = false;

                        db.Entry(addComponent).State = EntityState.Added;
                        db.SaveChanges();
                    }
                } else
                {
                    var quoteTypeDB = db.QuoteTypes.Where(i => i.QuoteTypeID == quoteTypeID && !i.IsDeleted).FirstOrDefault();
                    if (quoteTypeDB == null)
                    {
                        return new jResponse(true, "QuoteType does not exist.", null);
                    }

                    if (!isDeleted)
                    {
                        quoteTypeDB.QuoteTypeName = quoteTypeName;
                        quoteTypeDB.PaidBy = paidBy;
                        quoteTypeDB.Type = "";
                        quoteTypeDB.IsAllowNegotiation = isAllowNegotiation;
                        quoteTypeDB.ServiceTypeID = serviceTypeID;
                        quoteTypeDB.CoreField = coreField;
                        quoteTypeDB.LastUpdatedDT = currentDT;
                        quoteTypeDB.LastUpdatedByUserID = userID;


                        foreach (var j in customFields)
                        {
                            if (j.isNew == true) { 

                                Field addField = new Field();
                                addField.FieldName = j.fieldName;
                                addField.FieldType = j.typeID;
                                addField.IsRequired = false;
                                addField.IsDeleted = false;
                                addField.Choices = j.choice;
                                addField.CreatedDT = currentDT;
                                addField.CreatedByUserID = userID;
                                addField.UpdatedByUserID = null;
                                addField.UpdatedDT = null;
                                addField.QuoteTypeID = quoteTypeDB.QuoteTypeID;
                                addField.ShowCustomer = j.showCustomers;
                                addField.ShowVendors = j.showVendors;

                                db.Entry(addField).State = EntityState.Added;
                                db.SaveChanges();

                            }

                        }

                        foreach (var i in components)
                        {
                            if (i.IsNew == true) { 
                                QuoteTypeComponent addComponent = new QuoteTypeComponent();
                                addComponent.ComponentName = i.ComponentName;
                                addComponent.Type = i.Type;
                                addComponent.PaidBy = i.PaidBy;
                                addComponent.Currency = i.Currency;
                                addComponent.QuoteTypeID = quoteTypeDB.QuoteTypeID;
                                addComponent.CreatedByUserID = userID;
                                addComponent.CreatedDT = currentDT;
                                addComponent.LastUpdatedByUserID = null;
                                addComponent.LastUpdatedDT = null;
                                addComponent.IsDeleted = false;

                                db.Entry(addComponent).State = EntityState.Added;
                                db.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        //if (db.RFQs.Any(i => i.CustomerID == customerID && i.DeliveryBy >= currentDT && !i.IsDeleted))
                        //{
                        //    return new jResponse(true, "This customer already in use.", null);
                        //}
                        quoteTypeDB.IsDeleted = isDeleted;
                        quoteTypeDB.LastUpdatedDT = currentDT;
                        quoteTypeDB.LastUpdatedByUserID = userID;
                    }
                    db.Entry(quoteTypeDB).State = EntityState.Modified;
                    db.SaveChanges();

                }


                return new jResponse(false, "Quote type successfully added.", null);
            }
        }

        /*
         - getQuoteType()
         - Purpose: Get quote type list.
         - In: class myParams {
                      userID
                     }
         - Out: Success/Failure 
        */
        [HttpPost, HttpGet, Route("quoteType/get")]
        public dynamic getQuoteType(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.userID < 0)
            {
                return new jResponse();
            }

            // Get user info from cached token.
            token myToken = genApiController.getPersonTkn();
            genApiController gen = new genApiController();
            if (myToken == null)
            {
                return new jResponse(true, "Your session has expired.", null);
            }

            // Extract parameters
            int userID = myParams.userID;

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }


            // Get executive list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var quoteTypeList = (from dbq in db.QuoteTypes
                                 join dbqc in db.QuoteTypeComponents
                                 on new { dbq.QuoteTypeID, isQuoteType = false }
                                 equals new { dbqc.QuoteTypeID, isQuoteType = dbqc.IsDeleted } into dq
                                 join dbf in db.Fields
                                 on new { dbq.QuoteTypeID, isField = false }
                                 equals new {dbf.QuoteTypeID, isField = dbf.IsDeleted} into df
                                 where !dbq.IsDeleted
                                 select new quoteTypeDetail
                                 {
                                     quoteTypeID = dbq.QuoteTypeID,
                                     quoteTypeName = dbq.QuoteTypeName,
                                     paidBy = dbq.PaidBy,
                                     type = dbq.Type,
                                     isDeleted = dbq.IsDeleted,
                                     isAllowNegotiation = dbq.IsAllowNegotiation,
                                     components = dq,
                                     fields = df,
                                     serviceTypeID = dbq.ServiceTypeID,
                                     coreFields = dbq.CoreField
                                 }).OrderBy(i => i.quoteTypeName).ToList();

                foreach (var i in quoteTypeList)
                {
                    List<int> coreIDs = gen.splitString('~', i.coreFields).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

                    foreach (var j in coreIDs)
                    {
                        if (j == (int)coreFields.customer)
                        {
                            i.customer = true;
                        }

                        if (j == (int)coreFields.term)
                        {
                            i.term = true;
                        }

                        if (j == (int)coreFields.note)
                        {
                           i.note = true;
                        }

                        if (j == (int)coreFields.quoteby)
                        {
                            i.quoteby = true;
                        }

                        if (j == (int)coreFields.vendors)
                        {
                            i.vendors = true;
                        }
                        if (j == (int)coreFields.file)
                        {
                            i.file = true;
                        }

                        if (j == (int)coreFields.erpRef)
                        {
                           i.erpRef = true;
                        }

                        if (j == (int)coreFields.deliveryDT)
                        {
                            i.deliveryDT = true;
                        }
                    }
                }

                return new jResponse(false, "", quoteTypeList);
            }
        }

        /*
          - updateComponent()
          - Purpose: Delete component.
          - In: class myParams {
                       userID,
                       componentID,
                       componentName,
                       type,
                       paidBy,
                       isDeleted
                      }
          - Out: Success/Failure 
       */
        [HttpPost, HttpGet, Route("component/set")]
        public dynamic updateComponent(dynamic myParams)
        {
            // #region "Validations"

            // Basic validations
            if (myParams == null || myParams.componentID < 1 || myParams.componentName == "" || myParams.paidBy == "" || myParams.type == "")
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
            int userID = Convert.ToInt32(myParams.userID);
            int componentID = myParams.componentID;
            bool isDeleted = myParams.isDeleted;
            string componentName = myParams.componentName;
            string currency = myParams.currency;
            string paidBy = myParams.paidBy;
            string type = myParams.type;

            DateTime currentDT = genApiController.getDate();

            // Verify user
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this API.", null);
            }

            // #endregion

            // Add new customer
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
               if (isDeleted) { 

                    var QuoteComponent = db.QuoteTypeComponents.Where(j => j.ComponentID == componentID && !j.IsDeleted).FirstOrDefault();

                    if (QuoteComponent == null)
                    {
                        return new jResponse(false, "Component not available.", null);
                    }

                    QuoteComponent.IsDeleted = true;
                    QuoteComponent.LastUpdatedByUserID = userID;
                    QuoteComponent.LastUpdatedDT = currentDT;
                    db.Entry(QuoteComponent).State = EntityState.Modified;
                    db.SaveChanges();
                } else
                {
                    var QuoteComponent = db.QuoteTypeComponents.Where(j => j.ComponentID == componentID && !j.IsDeleted).FirstOrDefault();

                    if (QuoteComponent == null)
                    {
                        return new jResponse(false, "Component not available.", null);
                    }

                    QuoteComponent.IsDeleted = false;
                    QuoteComponent.Currency = currency;
                    QuoteComponent.PaidBy = paidBy;
                    QuoteComponent.Type = type;
                    QuoteComponent.ComponentName = componentName;
                    QuoteComponent.LastUpdatedByUserID = userID;
                    QuoteComponent.LastUpdatedDT = currentDT;
                    db.Entry(QuoteComponent).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Component successfully updated.", null);
            }
        }

        /*
          - getServiceType()
          - Purpose: Get service list.
          - In: class myParams {
                       userID
                      }
          - Out: Success/Failure 
        */
        [HttpPost, HttpGet, Route("serviceType/get")]
        public dynamic getServiceType(dynamic myParams)
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

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }


            // Get executive list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var serviceList = (from dbs in db.ServiceTypes
                                     join dbq in db.QuoteTypes
                                     on new { dbs.ServiceTypeID, isServiceType = false }
                                     equals new { dbq.ServiceTypeID, isServiceType = dbq.IsDeleted } into dqc
                                     where !dbs.IsDeleted
                                     select new serviceTypeDetail
                                     {
                                         serviceTypeID = dbs.ServiceTypeID,
                                         serviceTypeName = dbs.ServiceName,
                                         completionText = dbs.CompletionDateText,
                                         quoteTypes = dqc
                                     }).OrderBy(i => i.serviceTypeName).ToList();

                return new jResponse(false, "", serviceList);
            }
        }

        /*
        - updateCustomField()
        - Purpose: Update and Delete custom field.
        - In: class myParams {
                     userID,
                     fieldID,
                     fieldName,
                     typeID,
                     choice,
                     isDeleted,
                     quoteTypeID
                    }
        - Out: Success/Failure 
        */
        [HttpPost, HttpGet, Route("customField/set")]
        public dynamic updateCustomField(dynamic myParams)
        {
            // #region "Validations"

            // Basic validations
            if (myParams == null || myParams.fieldID < 1 || myParams.fieldName == "" || myParams.typeID == "")
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
            int userID = Convert.ToInt32(myParams.userID);
            int fieldID = myParams.fieldID;
            string fieldName = myParams.fieldName;
            string choice = myParams.choice;
            int typeID = myParams.typeID;
            bool isDeleted = myParams.isDeleted;
            bool showCustomers = myParams.showCustomers;
            bool showVendors = myParams.showVendors;
            int quoteTypeID = myParams.quoteTypeID;
            DateTime currentDT = genApiController.getDate();

            // Verify user
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this API.", null);
            }

            // #endregion

            // Update or delete field
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                if (isDeleted)
                {

                    var fieldDB = db.Fields.Where(j => j.FieldID == fieldID && !j.IsDeleted).FirstOrDefault();

                    if (fieldDB == null)
                    {
                        return new jResponse(false, "Field not available.", null);
                    }

                    fieldDB.IsDeleted = true;
                    fieldDB.UpdatedByUserID = userID;
                    fieldDB.UpdatedDT = currentDT;
                    db.Entry(fieldDB).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var fieldDB = db.Fields.Where(j => j.FieldID == fieldID && !j.IsDeleted).FirstOrDefault();

                    if (fieldDB == null)
                    {
                        return new jResponse(false, "Field not available.", null);
                    }

                    var dbShipFiled = db.ShipFields.Where(k => k.QuoteTypeID == quoteTypeID && !k.IsDeleted).FirstOrDefault();

                    if (dbShipFiled != null)
                    {
                        return new jResponse(false, "You can not change components.", null);
                    }

                    fieldDB.IsDeleted = false;
                    fieldDB.FieldName = fieldName;
                    fieldDB.FieldType = typeID;
                    fieldDB.Choices = choice;
                    fieldDB.UpdatedByUserID = userID;
                    fieldDB.UpdatedDT = currentDT;
                    fieldDB.ShowCustomer = showCustomers;
                    fieldDB.ShowVendors = showVendors;
                    db.Entry(fieldDB).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Field successfully updated.", null);
            }
        }

        /*
           - setSlot()
           - Purpose: Set slot.
           - In: class myParams {
                        userID,
                        slotID
                        slotFrom,
                        slotTo,
                        capacity,
                        isDeleted
                       }
           - Out: Success/Failure 
        */
        [HttpPost, HttpGet, IsSuperAdmin, Route("slot/set")]
        public dynamic setSlot(dynamic myParams)
        {
            // #region "Validations"

            if (myParams == null || myParams.slotFrom == null || myParams.slotFrom == "" || myParams.slotTo == null || myParams.slotTo == ""
                 || myParams.capacity == null || myParams.capacity == 0)
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
            int slotID = myParams.slotID;
            TimeSpan slotFrom = TimeSpan.Parse(Convert.ToString(myParams.slotFrom));
            TimeSpan slotTo = TimeSpan.Parse(Convert.ToString(myParams.slotTo));
            int serviceTypeID = 1;
            int capacity = myParams.capacity;
            bool isDeleted = myParams.isDeleted;
            int insertedSlotID = 0;
            DateTime currentDT = genApiController.getDate();


            // #endregion

            // Add slot
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
              
                if (slotID == 0)
                {
                    // Check for duplicate value 
                    //if (db.Slots.Any(i => ((TimeSpan.Compare(i.SlotFrom, slotFrom) == -1 && TimeSpan.Compare(i.SlotTo, slotFrom) == 1) 
                    //    || (i.SlotFrom < slotFrom && i.SlotTo > slotTo))
                    //    && !i.IsDeleted))
                    //{
                    //    return new jResponse(true, "This slot already exists.", null);
                    //}
                    if (db.Slots.Any(i => ((i.SlotFrom < slotFrom && i.SlotTo > slotFrom) || (i.SlotFrom < slotTo && i.SlotTo > slotTo))
                        && !i.IsDeleted))
                    {
                        return new jResponse(true, "This slot already exists.", null);
                    }


                    // Add slot
                    Slot dbSlot = new Slot();
                    dbSlot.SlotFrom = slotFrom;
                    dbSlot.SlotTo = slotTo;
                    dbSlot.ServiceTypeID = 1;
                    dbSlot.CreatedDT = currentDT;
                    dbSlot.CreatedByUserID = userID;
                    dbSlot.UpdatedByUserID = 0;
                    dbSlot.UpdatedDT = null;
                    dbSlot.Capacity = capacity;
                    dbSlot.IsDeleted = false;

                    db.Entry(dbSlot).State = EntityState.Added;
                    db.SaveChanges();
                    insertedSlotID = dbSlot.SlotID;

                }
                else
                {
                    // Update slot
                    var slotDB = db.Slots.Where(i => i.SlotID == slotID && !i.IsDeleted).FirstOrDefault();

                    if (slotDB == null)
                    {
                        return new jResponse(true, "Slot does not exist.", null);
                    }

                  
                    if (!isDeleted)
                    {
                        // Check for duplicate value
                        if (db.Slots.Any(i => i.SlotID != slotID && ((i.SlotFrom < slotFrom && i.SlotTo > slotFrom) 
                            || (i.SlotFrom < slotTo && i.SlotTo > slotTo)) && !i.IsDeleted))
                        {
                            return new jResponse(true, "This slot already exists.", null);
                        }


                        slotDB.SlotFrom = slotFrom;
                        slotDB.SlotTo = slotTo;
                        slotDB.Capacity = capacity;
                        slotDB.ServiceTypeID = serviceTypeID;
                        slotDB.UpdatedByUserID = userID;
                        slotDB.UpdatedDT = currentDT;
                    }
                    else
                    {
                        slotDB.IsDeleted = isDeleted;
                        slotDB.UpdatedDT = currentDT;
                    }

                    db.Entry(slotDB).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Slot successfully " + (slotID == 0 ? "added." : isDeleted ? "deleted." : "updated."), insertedSlotID);
            }
        }

        /*
          - getSlot()
          - Purpose: Get slot.
          - In: class myParams {
                       userID
                      }
          - Out: Success/Failure 
       */
        [HttpPost, HttpGet, IsSuperAdmin, Route("slot/get")]
        public dynamic getSlot(dynamic myParams)
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

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // Get slot
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var slots = (from dbs in db.Slots
                              join dbst in db.ServiceTypes on dbs.ServiceTypeID equals dbst.ServiceTypeID
                              where !dbs.IsDeleted && !dbst.IsDeleted
                              select new slot
                              {
                                  slotID = dbs.SlotID,
                                  fromTime = dbs.SlotFrom,
                                  toTime = dbs.SlotTo, //DateTime.Today.Add(dbs.SlotTo).ToString("hh:mm tt"),
                                  capacity = dbs.Capacity,
                                  serviceTypeID = dbs.ServiceTypeID,
                                  serviceName = dbst.ServiceName
                              }).OrderBy(i => i.slotID).ToList();

                return new jResponse(false, "", slots);
            }
        }

        /*
         - help_sendPassword()
         - Purpose: Send password.
         - In: class myParams {
                         mobile,
                         pwd
                     }
         - Out: Success/Failure 
        */
        public static jResponse help_sendPassword(dynamic mobile, dynamic pwd)
        {
            // Send OTP
            List<string> toList = new List<string>();
            toList.Add(mobile);

            var smsRetVal = smsApiController.send_password(toList, pwd);
            if (smsRetVal.error)
            {
                return new jResponse(true, "Something went wrong. Please try again later.", null);
            }

            return new jResponse(false, "Sent successfully", null);
        }

        #endregion

        #region "Classes"

        public class route
        {
            public int routeID { get; set; }
            public string routeCode { get; set; }
            public string routeDetail { get; set; }
            public string routeInfo { get; set; }
        }

        public class executive
        {
            public int executiveID { get; set; }
            public string executiveName { get; set; }
            public string designation { get; set; }
            public string email { get; set; }
            public string mobile { get; set; }
            public bool canCreateRFQ { get; set; }
            public bool canSelectTransporter { get; set; }
            public bool canCloseRFQ { get; set; }
            public bool canStart { get; set; }
            public bool canFinish { get; set; }
            public bool canFeedback { get; set; }
            public bool canSelectForCustomer { get; set; }
            public string notes { get; set; }
            public string zoneIDs { get; set; }
            public string rights { get; set; }
            public bool canForward { get; set; }
            public bool canDispatchCreate { get; set; }
            public bool canUpdateStatus { get; set; }
            public bool canScheduled { get; set; }
            public bool canLoading { get; set; }
            public bool canDispatchView { get; set; }
            public bool isSupervisor { get; set; }
            public bool isDomestic { get; set; }
            public bool isExport { get; set; }
            public string serviceRights { get; set; }

            public executive()
            {
                this.canDispatchCreate = false;
                this.canUpdateStatus = false;
                this.canLoading = false;
                this.canScheduled = false;
                this.canDispatchView = false;
                this.canForward = false;
                this.isSupervisor = false;
                this.isDomestic = false;
                this.isExport = false;
            }
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
            public string serviceTypeID { get; set; }
        }

        public class source
        {
            public int sourceID { get; set; }
            public string location { get; set; }
            public string type { get; set; }
            public string address1 { get; set; }
            public string address2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string notes { get; set; }
        }

        public class customer
        {
            public int customerID { get; set; }
            public string companyName { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string title { get; set; }
            public string fname { get; set; }
            public string lname { get; set; }
            public string email { get; set; }
            public string mobile { get; set; }
            public string alternateMobile { get; set; }
            public string notes { get; set; }
            public string address1 { get; set; }
            public string address2 { get; set; }
            public string country { get; set; }
        }

        public class zone
        {
            public int zoneID { get; set; }
            public string zoneName { get; set; }
        }

        public class quoteTypeDetail
        {
            public int quoteTypeID { get; set; }
            public string quoteTypeName { get; set; }
            public string paidBy { get; set; }
            public string type { get; set; }
            public bool isDeleted { get; set; }
            public bool isAllowNegotiation { get; set; }
            public dynamic components { get; set; }
            public dynamic fields { get; set; }
            public int serviceTypeID { get; set; }
            public string coreFields { get; set; }
            public bool customer { get; set; }
            public bool quoteby { get; set; }
            public bool term { get; set; }
            public bool file { get; set; }
            public bool vendors { get; set; }
            public bool note { get; set; }
            public bool erpRef { get; set; }
            public bool deliveryDT { get; set; }
        }

        public class serviceTypeDetail
        {
            public int serviceTypeID { get; set; }
            public string serviceTypeName { get; set; }
            public string completionText { get; set; }
            public dynamic quoteTypes { get; set; }
        }

        public class slot
        {
            public int slotID { get; set; }
            public TimeSpan fromTime { get; set; }
            public TimeSpan toTime { get; set; }
            public int capacity { get; set; }
            public int serviceTypeID { get; set; }
            public string serviceName { get; set; }
          
        }

        #endregion
    }
}