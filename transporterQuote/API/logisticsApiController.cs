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
using System.Data.SqlClient;
using System.Globalization;

namespace transporterQuote.API
{
    [RoutePrefix("api/logistics")]
    public class logisticsApiController : ApiController
    {
        #region 'Functions'

        /*
       - getLogisticsRFQ()
       - Purpose: Get list of rfq for logistics.
       - In: class myParams {
                       userID,
                       serviceRights,
                       status
                   }
       - Out: logisticsRFQList
    */
        [HttpPost, HttpGet, IsMajama, Route("get")]
        public dynamic getLogisticsRFQ(dynamic myParams)
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
            string serviceRights = myParams.serviceRights;
            int status = myParams.status;

            genApiController gen = new genApiController();

            if (myToken.userID != userID)
            {
                return new jResponse(true, "Something went wrong", null);
            }

            // #endregion

            // Get reqForQuote list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var dbExecutive = db.Executives.Where(i => i.ExecutiveID == userID && !i.IsDeleted).FirstOrDefault();

                if (dbExecutive == null)
                {
                    return new jResponse(true, "This executive does not exist", null);
                }

                var logisticsRFQList = db.sp_getTransportRFQ(userID, serviceRights, status).ToList();

                if (logisticsRFQList == null)
                {
                    return new jResponse(true, "No RFQ found!", null);
                }

                return new jResponse(false, "", logisticsRFQList);

            }
        }

        /*
         - getCountRFQL()
         - Purpose: Get count of rfq status wise.
         - In: class myParams {
                        userID
                    }
         - Out: countList
         */
        [HttpPost, HttpGet, IsMajama, Route("count/get")]
        public dynamic getCountRFQL(dynamic myParams)
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

            // #endregion

            // Get rfq count for logistics
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var dbCountList = db.sp_getCountRFQ(userID).ToList();

                return new jResponse(false, "", dbCountList);
            }
        }

        /*
         - dispatchPalnSet()
         - Purpose: New dispatch plan set.
         - In: class myParams {
                          userID,
                          customerID,
                          source,
                          pickUpDT,
                          deliveryDT,
                          containers,
                          serviceIDs,
                          fileName,
                          sourceAddress,
                          destination,
                          product,
                          portofLoading,
                          portofDispatch,
                          emptyContainerPick
                     }
         - Out: 
        */
        [HttpPost, HttpGet, Route("dispatchPlan/set")]
        public dynamic dispatchPlanSet(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.customerID < 1 || myParams.source == null || myParams.source == ""
               || myParams.containers == 0 || myParams.serviceIDs == null || myParams.serviceIDs == "~")
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
            int customerID = myParams.customerID;
            DateTime pickupDT = myParams.pickUpDT;
            DateTime deliveryDT = myParams.deliveryDT;
            int containers = myParams.containers;
            string file = myParams.file;
            string serviceIDs = myParams.serviceIDs;
            string source = myParams.source;
            string sourceAddress = myParams.sourceAddress;
            string destination = myParams.destination;
            string product = myParams.product;
            string portofLoading = myParams.portofLoading;
            string portofDispatch = myParams.portofDispatch;
            string emptyContainerPick = myParams.emptyContainerPick;
            //List<string> containerNames = new List<string>(myParams.containersNames);
            dynamic containerNames = myParams.containersNames;
           // containerNames = List<string>(myParams.containersNames);
            DateTime currentDT = genApiController.getDate();
            int insertedShipmentID = 0;

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Add dispatch plan
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                Shipment addShipment = new Shipment();

                addShipment.CustomerID = customerID;
                addShipment.PickupDT = pickupDT;
                addShipment.DeliveryDT = deliveryDT;
                addShipment.Containers = containers;
                addShipment.SourceLocation = source;
                addShipment.ServiceIDs = serviceIDs;
                addShipment.FileName = file == null ? "" : file;
                addShipment.CreatedByUserID = userID;
                addShipment.CreatedDT = currentDT;
                addShipment.DestinationAddress = destination;
                addShipment.SourceAddress = sourceAddress;
                addShipment.RFQID = 0;
                addShipment.Product = product;
                addShipment.QuoteID = 0;
                addShipment.TransporterID = 0;
                addShipment.QuoteTypeID = 0;
                addShipment.PortofDischarge = portofDispatch;
                addShipment.PortofLoading = portofLoading;
                addShipment.EmptyContainer = emptyContainerPick;
                addShipment.IsDeleted = false;
                addShipment.ShipmentRFQIDs = "";
                addShipment.Status = "0";
                addShipment.UpdatedDT = null;
                addShipment.UpdatedByUserID = 0;

                db.Entry(addShipment).State = EntityState.Added;
                db.SaveChanges();
                insertedShipmentID = addShipment.ShipmentID;

                // Add containers
                if (containers > 0)
                {
                    for (var i = 0; i < containers; i++)
                    {
                        Container addContainer = new Container();
                        addContainer.ShipmentID = insertedShipmentID;
                        addContainer.ScheduleFromDT = null;
                        addContainer.ScheduleTime = null;
                        addContainer.SlotID = 0;
                        addContainer.Notes = "";
                        addContainer.IsDeleted = false;
                        addContainer.CreatedDT = currentDT;
                        addContainer.CreatedByUserID = userID;
                        addContainer.UpdatedByUserID = 0;
                        addContainer.UpdatedDT = null;
                        addContainer.Status = "0";
                        addContainer.VehicleNumber = "";
                        addContainer.VehicleArrivalDT = null;
                        addContainer.VehicleArrivalByUserID = 0;
                        addContainer.LoadingDT = null;
                        addContainer.LoadingByUserID = 0;
                        addContainer.DispatchedDT = null;
                        addContainer.DispatchedByUserID = 0;
                        addContainer.DeliveryDT = null;
                        addContainer.DeliveryByUserID = 0;
                        addContainer.CtrName = containerNames[i];

                        db.Entry(addContainer).State = EntityState.Added;
                        db.SaveChanges();
                    }
                }

                return new jResponse(false, "Dispatch plan generated!", null);
            }
        }

        /*
         - dispatchPalnGet()
         - Purpose: Dispatch plan get.
         - In: class myParams {
                          userID,
                          status
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
            string status = myParams.status;
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

                var dbDispatchPlan = (from dbs in db.Shipments
                                      join dbc in db.Customers on dbs.CustomerID equals dbc.CustomerID
                                      join dbe in db.Executives on dbs.CreatedByUserID equals dbe.ExecutiveID                                      
                                      where !dbs.IsDeleted && !dbc.IsDeleted && !dbe.IsDeleted && dbs.Status == status
                                      select new dispatchDetail
                                      {
                                          shipmentID = dbs.ShipmentID,
                                          pickup = dbs.PickupDT,
                                          delivery = dbs.DeliveryDT,
                                          customerID = dbc.CustomerID,
                                          companyName = dbc.CompanyName,
                                          container = dbs.Containers,
                                          fileName = dbs.FileName,
                                          serviceIDs = dbs.ServiceIDs,
                                          product = dbs.Product,
                                          source = dbs.SourceLocation,
                                          sourceAddress = dbs.SourceAddress,
                                          destinationAddress = dbs.DestinationAddress,
                                          portofLoading = dbs.PortofLoading,
                                          portofDischarge = dbs.PortofDischarge,
                                          status = dbs.Status
                                      }).OrderBy(i => i.pickup).ToList();

                return new jResponse(false, "", dbDispatchPlan);
            }
        }

        /*
         - dispatchPalnRFQGet()
         - Purpose: Dispatch plan get.
         - In: class myParams {
                          userID,
                          shipmentID
                     }
         - Out: RFQList
        */
        [HttpPost, HttpGet, Route("dispatchPlanRFQs/get")]
        public dynamic dispatchPalnRFQGet(dynamic myParams)
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
            int shipmentID = myParams.shipmentID;
            DateTime currentDT = genApiController.getDate();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Get dispatch plan rfqlist
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Check for valid executive
                var dbExecutive = db.Executives.Where(i => i.ExecutiveID == userID && !i.IsDeleted).FirstOrDefault();

                if (dbExecutive == null)
                {
                    return new jResponse(true, "This executive does not exist", null);
                }

                var dbDispatchPlan = (from dbr in db.RFQs
                                      join dbc in db.Customers on dbr.CustomerID equals dbc.CustomerID
                                      join dbe in db.Executives on dbr.ExecutiveID equals dbe.ExecutiveID
                                      join dbqt in db.QuoteTypes on dbr.QuoteTypeID equals dbqt.QuoteTypeID
                                      join dbs in db.ServiceTypes on dbqt.ServiceTypeID equals dbs.ServiceTypeID
                                      where !dbr.IsDeleted && !dbc.IsDeleted && !dbe.IsDeleted && dbr.ShipmentID == shipmentID
                                      select new planRFQsDetail
                                      {
                                          RFQID = dbr.RequestForQuoteID,
                                          customerID = dbc.CustomerID,
                                          customerName = dbc.CompanyName,
                                          createdBy = dbe.ExecutiveName,
                                          createdDT = dbr.CreatedDT,
                                          quoteBy = dbr.QuoteBy,
                                          status = dbr.Status,
                                          quoteTypeName = dbqt.QuoteTypeName,
                                          serviceName = dbs.ServiceName
                                      }).OrderBy(i => i.RFQID).ToList();

                return new jResponse(false, "", dbDispatchPlan);
            }
        }

        /*
             - readyForDispatch()
             - Purpose: Ready for dispatch.
             - In: class myParams {
                              userID,
                              shipmentID 
                         }
             - Out: 
        */
        [HttpPost, HttpGet, Route("dispatch/ready")]
        public dynamic readyForDispatch(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.shipmentID < 1)
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
            int shipmentID = myParams.shipmentID;
            DateTime currentDT = genApiController.getDate();


            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Get shipment detail
                var dbShipment = (from dbs in db.Shipments
                                  where !dbs.IsDeleted && dbs.ShipmentID == shipmentID
                                  select new {
                                      dbs
                                  }).FirstOrDefault();


                // Check if shipment is null or shipmentID same or not
                if (dbShipment == null || dbShipment.dbs.ShipmentID != shipmentID)
                {
                    return new jResponse();
                }

                dbShipment.dbs.Status = "1";
                dbShipment.dbs.UpdatedByUserID = userID;
                dbShipment.dbs.UpdatedDT = currentDT;

                db.Entry(dbShipment.dbs).State = EntityState.Modified;
                db.SaveChanges();


                return new jResponse(false, "Ready for dispatch!", shipmentID);

            }

        }

        /*
            - getScheduleSlot()
            - Purpose: Get scehdule slot.
            - In: class myParams {
                            userID,
                            serviceTypeID
                        }
            - Out: 
        */
        [HttpPost, HttpGet, Route("schedule/get")]
        public dynamic getScheduleSlot(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.serviceTypeID < 1)
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
            int serviceTypeID = myParams.serviceTypeID;
            DateTime currentDT = genApiController.getDate();


            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Get schedule slot
            string con = System.Configuration.ConfigurationManager.ConnectionStrings["transporter_QuoteEntities"].ConnectionString;
            if (con.ToLower().StartsWith("metadata="))
            {
                System.Data.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.EntityClient.EntityConnectionStringBuilder(con);
                con = efBuilder.ProviderConnectionString;
            }

            // Make connection object
            using (SqlConnection conn2 = new SqlConnection(con))
            {
                DataSet ds = new DataSet();
                try
                {
                    // Connection open
                    conn2.Open();
                    SqlDataAdapter da2 = new SqlDataAdapter("EXEC sp_getScheduleSlot 1", conn2);
                    da2.Fill(ds);
                }
                finally
                {
                    conn2.Close();
                }

                return new jResponse(false, "", Newtonsoft.Json.JsonConvert.SerializeObject(ds));
                //return Newtonsoft.Json.JsonConvert.SerializeObject(ds);
            }

        }

        /*
            - getSlots()
            - Purpose: Get slots.
            - In: class myParams {
                            userID,
                            serviceTypeID
                        }
            - Out: dbSlots
        */
        [HttpPost, HttpGet, Route("slots/get")]
        public dynamic getSlots(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.serviceTypeID < 1)
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
            int serviceTypeID = myParams.serviceTypeID;
            DateTime currentDT = genApiController.getDate();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Get slots
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                var dbSlots = (from dbs in db.Slots
                               where !dbs.IsDeleted && dbs.ServiceTypeID == serviceTypeID
                               select new
                               {
                                   slotID = dbs.SlotID,
                                   slotFrom = dbs.SlotFrom,
                                   slotTo = dbs.SlotTo,
                                   capacity = dbs.Capacity
                               }).ToList()
                               .Select(x => new slotDetail()
                               {
                                   slotID = x.slotID,
                                   slotFrom = (x.slotFrom).ToString(@"hh\:mm"),
                                   slotTo = (x.slotTo).ToString(@"hh\:mm"), 
                                   capacity = x.capacity
                               }).ToList();

                return new jResponse(false, "", dbSlots);
            }
        }

        /*
            - setSchedule()
            - Purpose:
            - In: class myParams {
                            userID,
                            serviceTypeID,
                            containers
                        }
            - Out: dbSlots
        */
        [HttpPost, HttpGet, Route("scheduleSlot/set")]
        public dynamic setSchedule(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1)
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
            string containers = myParams.containers;
            int shipmentID = myParams.shipmentID;
            List<containerSet> containerList = new List<containerSet>();
            containerList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<containerSet>>(containers);
            DateTime currentDT = genApiController.getDate();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Set containers
                foreach (var k in containerList)
                {
                    if (k.scheduleDT != null) { 
                    var dbContainer = db.Containers.Where(j => j.ContainerID == k.containerID).FirstOrDefault();
                    string a = Convert.ToDateTime(Convert.ToString(k.scheduleDT)).ToString("dd-MMM-yyyy");
                    //string b = Convert.ToDateTime(Convert.ToString(k.scheduleTime)).ToString("HH:mm:ss");
                    TimeSpan b = TimeSpan.Parse(Convert.ToString(k.scheduleTime));
                    dbContainer.ScheduleFromDT = Convert.ToDateTime(a); // (a.Date + TimeSpan.Parse((k.scheduleTime)));
                    dbContainer.ScheduleTime = b;
                    dbContainer.SlotID = k.slotID;
                    dbContainer.Notes = k.notes;
                    dbContainer.UpdatedByUserID = userID;
                    dbContainer.UpdatedDT = currentDT;

                    db.Entry(dbContainer).State = EntityState.Modified;
                    db.SaveChanges();
                    }
                }

                var dbShipment = db.Shipments.Where(i => i.ShipmentID == shipmentID).FirstOrDefault();

                dbShipment.Status = "2";
                db.Entry(dbShipment).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Successfully container added.", null);
            }
        }

        /*
            - getContainer()
            - Purpose: Get conatiner list for time slot allocate
            - In: class myParams {
                            userID,
                            shipmentID
                        }
            - Out: dbContainer
        */
        [HttpPost, HttpGet, Route("container/get")]
        public dynamic getContainer(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.shipmentID < 1)
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
            int shipmentID = myParams.shipmentID;
            DateTime currentDT = genApiController.getDate();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Get container
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Conatiner list
                var dbContainers = (from dbc in db.Containers
                                    where !dbc.IsDeleted && dbc.ShipmentID == shipmentID
                                    select new containerInfo
                                    {
                                        containerID = dbc.ContainerID,
                                        shipmentID = dbc.ShipmentID,
                                        slotID = dbc.SlotID,
                                        scheduleDT = dbc.ScheduleFromDT,
                                        scheduleTime = dbc.ScheduleTime,
                                        notes = dbc.Notes,
                                        status = dbc.Status,
                                        ctrName = dbc.CtrName
                                    }).ToList();

                return new jResponse(false, "", dbContainers);
            }
        }

        /*
           - getAllocateSlot()
           - Purpose: Get slot allocate 
           - In: class myParams {
                           userID,
                           slotID,
                           scheduleDate
                       }
           - Out: dbSlotAllocate
       */
        [HttpPost, HttpGet, Route("allocateSlot/get")]
        public dynamic getAllocateSlot(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.slotID < 1)
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
            int slotID = myParams.slotID;
            DateTime scheduleDT = myParams.scheduleDate;
            DateTime currentDT = genApiController.getDate();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Container Allocated list
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Allocated list
                var dbSlotAllocate = db.sp_getAllocatedSlotDetail(scheduleDT, slotID).ToList();

                return new jResponse(false, "", dbSlotAllocate);
            }
        }

        /*
           - setVehicalArrival()
           - Purpose: Set vehical arrival
           - In: class myParams {
                            userID,
                            containerID,
                            vehicalNumber
                       }
           - Out: success/failure
       */
        [HttpPost, HttpGet, Route("vehicalArrival/set")]
        public dynamic setVehicalArrival(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.containerID < 1)
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
            int containerID = myParams.containerID;
            string vehicleNumber = myParams.vehicleNumber;
            DateTime currentDT = genApiController.getDate();
            //extra parameters to check whether vehicle is the first ever arrived one
            bool firstVehicleArrived = myParams.firstVehicleArrived;
            int RFQID = myParams.RFQID;

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Update container status
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Get container 
                var dbContainer = db.Containers.Where(i => i.ContainerID == containerID && !i.IsDeleted).FirstOrDefault();

                if (dbContainer == null)
                {
                    return new jResponse(true, "Container does not exist", null);
                }

                // Update for vehical arrival
                dbContainer.Status = "1";
                dbContainer.VehicleArrivalDT = currentDT;
                dbContainer.VehicleNumber = vehicleNumber;
                dbContainer.VehicleArrivalByUserID = userID;
                dbContainer.UpdatedByUserID = userID;
                dbContainer.UpdatedDT = currentDT;

                db.Entry(dbContainer).State = EntityState.Modified;
                db.SaveChanges();

                //Update for first ever vehicle arrival
                if (firstVehicleArrived)
                {
                    var dbRFQ = db.RFQs.Where(RFQ => RFQ.RequestForQuoteID == RFQID).FirstOrDefault();
                    dbRFQ.Status = "3";
                    dbRFQ.StartWorkDT = currentDT;
                    dbRFQ.StartedByUserID = userID;
                    dbRFQ.UpdatedDT = currentDT;

                    db.Entry(dbRFQ).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new jResponse(false, "Vehical arrived!", dbContainer.ContainerID);

            }
        }

        /*
           - setLoading()
           - Purpose: status change for container load start
           - In: class myParams {
                            userID,
                            containerID
                       }
           - Out: success/failure
       */
        [HttpPost, HttpGet, Route("load/set")]
        public dynamic setLoading(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.containerID < 1)
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
            int containerID = myParams.containerID;
            DateTime currentDT = genApiController.getDate();

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Update container status
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Get container 
                var dbContainer = db.Containers.Where(i => i.ContainerID == containerID && !i.IsDeleted).FirstOrDefault();

                if (dbContainer == null)
                {
                    return new jResponse(true, "Container does not exist", null);
                }

                // Update for load
                dbContainer.Status = "2";
                dbContainer.LoadingDT = currentDT;
                dbContainer.LoadingByUserID = userID;
                dbContainer.UpdatedByUserID = userID;
                dbContainer.UpdatedDT = currentDT;

                db.Entry(dbContainer).State = EntityState.Modified;
                db.SaveChanges();

                return new jResponse(false, "Loading started!", dbContainer.ContainerID);

            }
        }

        /*
           - setDispatch()
           - Purpose: status change for dispatch
           - In: class myParams {
                            userID,
                            containerID
                       }
           - Out: success/failure
       */
        [HttpPost, HttpGet, Route("dispatch/set")]
        public dynamic setDispatch(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.containerID < 1)
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
            int containerID = myParams.containerID;
            DateTime currentDT = genApiController.getDate();
            //Extra parameters to check whether all vehicles got dispatched
            bool allVehicleDispatched = myParams.allVehicleDispatched;
            int RFQID = myParams.RFQID;            

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Update container status
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Get container 
                var dbContainer = db.Containers.Where(i => i.ContainerID == containerID && !i.IsDeleted).FirstOrDefault();

                if (dbContainer == null)
                {
                    return new jResponse(true, "Container does not exist", null);
                }

                // Update for load
                dbContainer.Status = "3";
                dbContainer.DispatchedDT = currentDT;
                dbContainer.DispatchedByUserID= userID;
                dbContainer.UpdatedByUserID = userID;
                dbContainer.UpdatedDT = currentDT;

                db.Entry(dbContainer).State = EntityState.Modified;
                db.SaveChanges();

                //Update for all vehicles dispatched
                if (allVehicleDispatched)
                {
                    var dbRFQ = db.RFQs.Where(RFQ => RFQ.RequestForQuoteID == RFQID).FirstOrDefault();
                    dbRFQ.Status = "4";
                    dbRFQ.UpdatedDT = currentDT;
                    dbRFQ.DispatchedDT = currentDT;
                    dbRFQ.DispatchedByUserID = userID;

                    db.Entry(dbRFQ).State = EntityState.Modified;                    
                    db.SaveChanges();
                }

                return new jResponse(false, "Dispatched!", dbContainer.ContainerID);

            }
        }

        /*
           - setDelivery()
           - Purpose: Delivery of container to destination
           - In: class myParams {
                            userID,
                            containerID
                       }
           - Out: success/failure
       */
        [HttpPost, HttpGet, Route("delivery/set")]
        public dynamic setDelivery(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1 || myParams.containerID < 1)
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
            int containerID = myParams.containerID;
            DateTime currentDT = genApiController.getDate();
            bool allVehicleDelivered = myParams.allVehicleDelivered;
            int shipmentID = myParams.shipmentID;

            // Check if valid user, if not then kickout
            if (myToken.userID != userID)
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // Update container status
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {
                // Get container 
                var dbContainer = db.Containers.Where(i => i.ContainerID == containerID && !i.IsDeleted).FirstOrDefault();

                if (dbContainer == null)
                {
                    return new jResponse(true, "Container does not exist", null);
                }

                // Update for load
                dbContainer.Status = "4";
                dbContainer.DispatchedDT = currentDT;
                dbContainer.DispatchedByUserID = userID;
                dbContainer.UpdatedByUserID = userID;
                dbContainer.UpdatedDT = currentDT;

                db.Entry(dbContainer).State = EntityState.Modified;                

                if (allVehicleDelivered)
                {
                    var dbShipment = db.Shipments.Where(shipment => shipment.ShipmentID == shipmentID).FirstOrDefault();
                    dbShipment.Status = "3";

                    db.Entry(dbShipment).State = EntityState.Modified;                    
                }

                db.SaveChanges();
                return new jResponse(false, "Delivered!", dbContainer.ContainerID);

            }
        }

        #region 'Customer wise container'

        /*
           - getCustContainer()
           - Purpose: Get list of container customer wise  
           - In: class myParams {
                           userID
                         }
           - Out: dbCustContainers
       */
        [HttpPost, HttpGet, Route("custWiseContainers/get")]
        public dynamic getCustContainer(dynamic myParams)
        {
            // #region "Validation"

            if (myParams == null || myParams.userID < 1)
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

            // Container list in schedule time order
            using (transporter_QuoteEntities db = new transporter_QuoteEntities())
            {

                var dbCustContainers = (from dbc in db.Containers
                                        join dbs in db.Shipments on dbc.ShipmentID equals dbs.ShipmentID
                                        join dbct in db.Customers on dbs.CustomerID equals dbct.CustomerID
                                        join dbt in db.Transporters on dbs.TransporterID equals dbt.TransporterID
                                        where dbc.ScheduleFromDT != null && dbc.SlotID != 0 && new[] { "0", "1", "2" }.Contains(dbc.Status)
                                        select new getContainerCust
                                        {
                                           containerID = dbc.ContainerID,
                                           containerName = dbc.CtrName,
                                           customerName = dbct.CompanyName,
                                           status = dbc.Status,
                                           shipmentID = dbs.ShipmentID,
                                           scheduleDT = dbc.ScheduleFromDT,
                                           scheduleTime = dbc.ScheduleTime,
                                           transporterName = dbt.CompanyName
                                        }).OrderBy(i =>  i.scheduleDT)
                                        .ThenBy(i => i.scheduleTime).ToList();

                return new jResponse(false, "", dbCustContainers);
            }
        }   
        #endregion

        #endregion

        #region 'Classes'

        public class emailDetail
        {
            public string rfqID { get; set; }
            public string link { get; set; }
            public string quoteCount { get; set; }
            public string displayName { get; set; }
            public string userName { get; set; }
        }

        public class dispatchDetail
        {
            public int shipmentID { get; set; }
            public DateTime? pickup { get; set; }
            public DateTime delivery { get; set; }
            public string source { get; set; }
            public int customerID { get; set; }
            public string companyName { get; set; }
            public int container { get; set; }
            public string fileName { get; set; }
            public string serviceIDs { get; set; }
            public string product { get; set; }
            public string sourceAddress { get; set; }
            public string destinationAddress { get; set; }
            public string portofLoading { get; set; }
            public string portofDischarge { get; set; }
            public string status { get; set; }
        }

        public class planRFQsDetail
        {
            public int RFQID { get; set; }
            public int customerID { get; set; }
            public string customerName { get; set; }
            public string status { get; set; }
            public string createdBy { get; set; }
            public string quoteTypeName { get; set; }
            public string serviceName { get; set; }
            public DateTime createdDT { get; set; }
            public DateTime quoteBy { get; set; }
        }

        public class slotDetail
        {
            public int slotID { get; set; }
            public string slotFrom { get; set; }
            public string slotTo { get; set; }
            public int capacity { get; set; }
        }

        public class containerInfo
        {
            public int shipmentID { get; set; }
            public int containerID { get; set; }
            public DateTime? scheduleDT { get; set; }
            public TimeSpan? scheduleTime { get; set; }
            public int slotID { get; set; }
            public string notes { get; set; }
            public string status { get; set; }
            public string ctrName { get; set; }
        }

        public class containerSet
        {
            public int shipmentID { get; set; }
            public int containerID { get; set; }
            public DateTime? scheduleDT { get; set; }
            public String scheduleTime { get; set; }
            public int slotID { get; set; }
            public string notes { get; set; }
        }

        public class slotAllocateInfo
        {
            public int containerID { get; set; }
            public int shipmentID { get; set; }
            public TimeSpan? scheduleTime { get; set; }
            public string customerName { get; set; }
            public string transporterName { get; set; }
        }

        public class getContainerCust
        {
            public int containerID { get; set; }
            public int shipmentID { get; set; }
            public string customerName { get; set; }
            public string status { get; set; }
            public TimeSpan? scheduleTime { get; set; }
            public string containerName { get; set; }
            public string transporterName { get; set; }
            public DateTime? scheduleDT { get; set; }
        }

        #endregion

    }
}