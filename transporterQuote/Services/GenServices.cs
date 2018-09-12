using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace transporterQuote.Services
{
   
    public class jResponse
        {
            public bool error { get; set; }
            public string message { get; set; }
            public dynamic data { get; set; }

            public jResponse()
            {
                this.error = true;
                this.message = "Invalid parameters";
                this.data = null;
            }

            public jResponse(bool error, string message, dynamic data)
            {
                this.error = error;
                this.message = message;
                this.data = data;
            }
        }

    public enum executiveRights
    {
        pending = 0,
        selected = 1,
        forwarded = 2,
        dispatchView = 3,
        dispatchCreate = 4,
        updateStatus = 5,
        scheduled = 6,
        loading = 7,
        isSupervisor = 8,
        isDomestic = 9,
        isExport = 10, 
        canStart = 11,
        canFinish = 12,
        canFeedback = 13
    }

    public enum coreFields {
        customer = 1,
        quoteby = 2,
        term = 3,
        file = 4,
        vendors = 5,
        note = 6,
        erpRef = 7,
        deliveryDT = 8
    }

    public class token
    {
        public string tokenID { get; set; }
        public int userID { get; set; }
        public string name { get; set; }
        public bool isSuperAdmin { get; set; }
        public bool canCreateRFQ { get; set; }
        public bool canSelectTransporter { get; set; }
        public bool canCloseRFQ { get; set; }
        public bool canSelectForCustomer { get; set; }
        public bool canForward { get; set; }
        public bool canDispatchCreate { get; set; }
        public bool canUpdateStatus { get; set; }
        public bool canScheduled { get; set; }
        public bool canLoading { get; set; }
        public bool canDispatchView { get; set; }
        public bool canStart { get; set; }
        public bool canFinish { get; set; }
        public bool canFeedback { get; set; }
        public bool isSupervisor { get; set; }
        public bool isDomestic { get; set; }
        public bool isExport { get; set; }
        public string serviceRights { get; set; } 

    }

  

}