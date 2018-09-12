using transporterQuote.Helper;
using transporterQuote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using transporterQuote.Services;
using transporterQuote.API;
using System.Web.Configuration;

namespace transporterQuote.API
{
    [RoutePrefix("api/sms"), IsMajama]
    public class smsApiController : ApiController
    {
        #region "Functions"

        /*
           - send_password()
           - Purpose: Send SMS with password to the given phone number.
           - Audience: transporter, executive and customer
           - In: class smsParams {
                          pwd
                       }
        */
        public static jResponse send_password(List<string> toList, dynamic smsParams)
        {
            smsDraft draft = new smsDraft();

            draft.msgText = "Your password is " + smsParams + ".";
            draft.toList = toList;

            return sendSMS(draft);
        }

        /*
          - send_newRFQ()
          - Purpose: Send SMS to tansporter with link.
          - Audience: transporter
          - In: class smsParams {
                        link,
                        sourceCity,
                        destinationCity
                      }
       */
        public static jResponse send_newRFQ(List<string> toList, dynamic smsParams)
        {
            smsDraft draft = new smsDraft();
            string displayName = WebConfigurationManager.AppSettings["MailDisplayName"];

            draft.msgText = "New RFQ "+ smsParams.rfqID + " from " + displayName +" "+ smsParams.link;
            draft.toList = toList;

            return sendSMS(draft);
        }

        /*
          - send_quoteNegotiation()
          - Purpose: Send SMS to tansporter with link.
          - Audience: transporter
          - In: class smsParams {
                        link,
                        sourceCity,
                        destinationCity
                      }
       */
        public static jResponse send_quoteNegotiation(List<string> toList, dynamic smsParams)
        {
            smsDraft draft = new smsDraft();
            string displayName = WebConfigurationManager.AppSettings["MailDisplayName"];

            draft.msgText = "Quotation negotiation request for " + smsParams.rfqID + " from " + displayName + " " + smsParams.link;
            draft.toList = toList;

            return sendSMS(draft);
        }

        /*
          - send_updatedRFQ()
          - Purpose: Send SMS to tansporter with link.
          - Audience: transporter
          - In: class smsParams {
                        link,
                        sourceCity,
                        destinationCity
                      }
       */
        public static jResponse send_updatedRFQ(List<string> toList, dynamic smsParams)
        {
            smsDraft draft = new smsDraft();
            string displayName = WebConfigurationManager.AppSettings["MailDisplayName"];

            draft.msgText = "Update RFQ " + smsParams.rfqID + " from " + displayName + " " + smsParams.link;
            draft.toList = toList;

            return sendSMS(draft);
        }

        /*
         - send_qtnExecutive()
         - Purpose: Send SMS to executive for transporter quatation.
         - Audience: executive
         - In: class smsParams {
                        link,
                        rfqID,
                        customerName,
                        transporterName,
                        charge
                     }
         */
        public static jResponse send_qtnExecutive(List<string> toList, dynamic smsParams)
        {
            smsDraft draft = new smsDraft();
            string displayName = WebConfigurationManager.AppSettings["MailDisplayName"];

            draft.msgText = "Qtns recd for RFQ " + smsParams.rfqID + " for " + smsParams.customerName + " from " + smsParams.transporterName;
            draft.toList = toList;

            return sendSMS(draft);
        }

        /*
        - send_acptToTransporter()
        - Purpose: Send SMS to executive for transporter quatation.
        - Audience: executive
        - In: class smsParams {
                       list,
                       rfqID,
                       customerName
                    }
        */
        public static jResponse send_acptToTransporter(List<string> toList, dynamic smsParams)
        {
            smsDraft draft = new smsDraft();
            string displayName = WebConfigurationManager.AppSettings["MailDisplayName"];

            draft.msgText = "Congrats! your quote for RFQ " + smsParams.rfqID + " for " + smsParams.customerName + " has been accepted";
            draft.toList = toList;

            return sendSMS(draft);
        }

        /*
         - send_Remider()
         - Purpose: Send SMS to transporte for reminder.
         - Audience: transporter
         - In: class smsParams {
                       link
                     }
      */
        public static jResponse send_Remider(List<string> toList, dynamic smsParams)
        {
            smsDraft draft = new smsDraft();
            string displayName = WebConfigurationManager.AppSettings["MailDisplayName"];

            draft.msgText = "RFQ " + smsParams.rfqID + " from " + displayName + " " + smsParams.link;
            draft.toList = toList;

            return sendSMS(draft);
        }


        /*
           - sendSMS()
           - Purpose: Send a sms with the given password to the given phoneNumber.
           - In: class myParams {
                           pwd
                       }
           - Out: Success / failure
        */
        private static jResponse sendSMS(smsDraft myParams)
        {
            // #region "Validations"

            // Basic validations
            if (myParams == null || myParams.toList == null || myParams.msgText == "")
            {
                return new jResponse();
            }

            // Extract parameters
            List<string> toList = myParams.toList;
            string smsText = myParams.msgText;
            List<string> validNumbers = new List<string>();

            if (toList.Count < 1)
            {
                return new jResponse();
            }

            // Check validation for phone number
            foreach (var phoneNumber in toList)
            {
                if (phoneNumber.Length == 10)
                {
                    validNumbers.Add(phoneNumber);
                }
            }

            // #endregion

            // #region "Send SMS"

            string SMSWorkingKey = WebConfigurationManager.AppSettings["SMSWorkingKey"];
            string SMSSenderID = WebConfigurationManager.AppSettings["SMSSenderID"];

            foreach (var phoneNumber in validNumbers)
            {

                string url = "http://alerts.prioritysms.com/api/web2sms.php?" +
                    "workingkey=" + SMSWorkingKey +
                    "&sender=" + SMSSenderID +
                    "&to=" + phoneNumber +
                    "&message=" + smsText;

                System.Net.WebClient web = new System.Net.WebClient();

                string result = web.DownloadString(url);

                if (result.Contains("Message GID"))
                {
                   // return new jResponse(false, "SMS will be sent!", true);
                }
                else
                {
                    return new jResponse(false, "SMS will be sent!", false);
                }
            }

            return new jResponse();

            // #endregion

        }

        #endregion

        #region "Classes"

        public class smsDraft
        {
            public List<string> toList { get; set; }
            public string msgText { get; set; }
        }

        #endregion

    }
}