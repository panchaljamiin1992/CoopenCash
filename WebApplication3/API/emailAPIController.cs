using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Http;
using transporterQuote.Services;

namespace transporterQuote.API
{
    public class emailAPIController : ApiController
    {
        #region "Functions"

        /*
           - TranspoterQuote()
           - Purpose: Send mail to transporter
           - In: class emailParams {
                    product,
                    pickup,
                    delivery, 
                    quoteby,
                    emailAddres,
                    source, 
                    destination,
                    link, 
                    companyName
           } 
         */
        public static jResponse transpoterQuote(List<string> toList, dynamic emailParams)
        {
            emailDraft draft = new emailDraft();
            string productWeb = genApiController.getURL();
            string productName = genApiController.getProductName();
            string dynamicFields = "";

            draft.subject = "Ammann India Pvt. Ltd. - RFQ #" + emailParams.rfqID;
            draft.displayName = emailParams.displayName;
            
            try
            {
                draft.content = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/HTML/Emails/link_quoteToTransporter.html"));
            }
            catch (Exception ex)
            {
                return new jResponse(true, "No template found!", null);
            }

            draft.content = draft.content.Replace("{productName}", productName);
            draft.content = draft.content.Replace("{link}", emailParams.link);
          //  draft.content = draft.content.Replace("{destination}",emailParams.destination);
            draft.content = draft.content.Replace("{pickup}", emailParams.pickup);
            draft.content = draft.content.Replace("{delivery}", emailParams.delivery);
            draft.content = draft.content.Replace("{quoteby}", emailParams.quoteby);

            foreach (var k in emailParams.emailDynamicFields)
            {
                if (k.ShowVendors == true)
                {
                    dynamicFields += k.FieldValue == "" ? "" : " <p><b>" + k.FieldName + ": &nbsp;</b>" + k.FieldValue + "</p>";
                }
            }

            draft.content = draft.content.Replace("{dynamicFields}", dynamicFields);

            draft.toList = toList;

            return sendEmail(draft);
        }

        /*
           - transpoterNegotiationQuote()
           - Purpose: Send mail to transporter
           - In: class emailParams {
                    product,
                    pickup,
                    delivery, 
                    quoteby,
                    emailAddres,
                    source, 
                    destination,
                    link, 
                    companyName
           } 
         */
        public static jResponse transpoterNegotiationQuote(List<string> toList, dynamic emailParams)
        {
            emailDraft draft = new emailDraft();
            string productWeb = genApiController.getURL();
            string productName = genApiController.getProductName();
            string dynamicFields = "";

            draft.subject = "Ammann India Pvt. Ltd. - RFQ #" + emailParams.rfqID;
            draft.displayName = emailParams.displayName;

            try
            {
                draft.content = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/HTML/Emails/link_quoteNegotiationToTransporter.html"));
            }
            catch (Exception ex)
            {
                return new jResponse(true, "No template found!", null);
            }

            draft.content = draft.content.Replace("{productName}", productName);
            draft.content = draft.content.Replace("{link}", emailParams.link);
            //  draft.content = draft.content.Replace("{destination}",emailParams.destination);
            draft.content = draft.content.Replace("{pickup}", emailParams.pickup);
            draft.content = draft.content.Replace("{delivery}", emailParams.delivery);
            draft.content = draft.content.Replace("{quoteby}", emailParams.quoteby);
            draft.content = draft.content.Replace("{negotiationComments}", emailParams.negotiationComments);

            foreach (var k in emailParams.emailDynamicFields)
            {
                if (k.ShowVendors == true)
                {
                    dynamicFields += k.FieldValue == "" ? "" : " <p><b>" + k.FieldName + ": &nbsp;</b>" + k.FieldValue + "</p>";
                }
            }

            draft.content = draft.content.Replace("{dynamicFields}", dynamicFields);

            draft.toList = toList;

            return sendEmail(draft);
        }

        /*
          - quot_accept()
          - Purpose: Send mail to transporter for quote accept
          - In: class emailParams {
                  pickup,
                  delivery,
                  AcceptedbyName
          } 
        */
        public static jResponse quot_accept(List<string> toList, dynamic emailParams)
        {
            emailDraft draft = new emailDraft();
            string productWeb = genApiController.getURL();
            string productName = genApiController.getProductName();

            draft.subject = "Ammann India Pvt. Ltd. - RFQ #" + emailParams.rfqID + "- selected";
            //draft.displayName = emailParams.displayName;

            try
            {
                draft.content = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/HTML/Emails/acpt_quotationOfTransporter.html"));
            }
            catch (Exception ex)
            {
                return new jResponse(true, "No template found!", null);
            }

            draft.content = draft.content.Replace("{rfqID}", emailParams.rfqID);
            draft.content = draft.content.Replace("{pickup}", emailParams.pickup);
            draft.content = draft.content.Replace("{delivery}", emailParams.delivery);
         //   draft.content = draft.content.Replace("{product}", emailParams.product);
            draft.content = draft.content.Replace("{customerCompany}", emailParams.companyName);
            draft.content = draft.content.Replace("{link}", emailParams.link);

            draft.toList = toList;

            return sendEmail(draft);
        }

        /*
         - link_Customer()
         - Purpose: Sent link to customer for
         - In: class emailParams {
                    product, 
                   // pickup, 
                   // delivery, 
                   // quoteby,
                    emailAddres,
                   // source, 
                   // destination,
                    link, 
                    companyName,
                    quoteCount
            } 
       */
        public static jResponse link_Customer(List<string> toList, dynamic emailParmas)
        {
            emailDraft draft = new emailDraft();
            string productWeb = genApiController.getURL();
            string productName = genApiController.getProductName();

            draft.subject = "Ammann India Pvt. Ltd. - Vendor Quote on RFQ #" + emailParmas.rfqID;
        
            draft.displayName = emailParmas.displayName;
            try
            {
                draft.content = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/HTML/Emails/link_quotationToCustomer.html"));
            }
            catch (Exception ex)
            {
                return new jResponse(true, "No template found!", null);
            }

            draft.content = draft.content.Replace("{productName}", productName);
            draft.content = draft.content.Replace("{link}", emailParmas.link);
            draft.content = draft.content.Replace("{rfqID}", emailParmas.rfqID);
            draft.content = draft.content.Replace("{quoteCount}", emailParmas.quoteCount);
            //draft.content = draft.content.Replace("{source}", emailParmas.source);
            //draft.content = draft.content.Replace("{destination}", emailParmas.destination);
            //draft.content = draft.content.Replace("{pickup}", emailParmas.pickup);
           // draft.content = draft.content.Replace("{delivery}", emailParmas.delivery);
           // draft.content = draft.content.Replace("{charges}", emailParmas.charges);
           // draft.content = draft.content.Replace("{payment}", emailParmas.payment);
           // string notesDiv = emailParmas.notes == "" ? "" : " <p><b>Notes: &nbsp;</b>"+ emailParmas.notes +"</p>";
           // draft.content = draft.content.Replace("{notesDiv}", notesDiv);
          //  draft.content = draft.content.Replace("{product}", emailParmas.product);
            draft.content = draft.content.Replace("{customerName}", emailParmas.displayName);
            draft.content = draft.content.Replace("{userName}", emailParmas.userName);
            draft.toList = toList;

            return sendEmail(draft);
        }

        /*
         - quot_Received()
         - Purpose: Sent quote received to executive
         - In: class emailParams {
                    product, 
                   // pickup, 
                   // delivery, 
                   // quoteby,
                    emailAddres,
                   // source, 
                   // destination,
                    link, 
                    companyName,
                    quoteCount
            } 
       */
        public static jResponse quot_Received(List<string> toList, dynamic emailParmas)
        {
            emailDraft draft = new emailDraft();
            string productWeb = genApiController.getURL();
            string productName = genApiController.getProductName();

            draft.subject = "Quote received from " + emailParmas.companyName;

          
            try
            {
                draft.content = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/HTML/Emails/quot_received.html"));
            }
            catch (Exception ex)
            {
                return new jResponse(true, "No template found!", null);
            }
            
            draft.content = draft.content.Replace("{rfqID}", emailParmas.rfqID);
            draft.content = draft.content.Replace("{companyName}", emailParmas.companyName);
            draft.toList = toList;

            return sendEmail(draft);
        }

        /*
         - sent_ReminderTransporter()
         - Purpose: Send mail to transporter
         - In: class emailParams {
                  product,
                  pickup,
                  delivery, 
                  quoteby,
                  emailAddres,
                  source, 
                  destination,
                  link, 
                  companyName
         } 
       */
        public static jResponse sent_ReminderTransporter(List<string> toList, dynamic emailParams)
        {
            emailDraft draft = new emailDraft();
            string productWeb = genApiController.getURL();
            string productName = genApiController.getProductName();
            string dynamicFields = "";

            draft.subject = "Reminder - Ammann India Pvt. Ltd. - RFQ #" + emailParams.rfqID;
            draft.displayName = emailParams.displayName;

            try
            {
                draft.content = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/HTML/Emails/sent_reminderToTrans.html"));
            }
            catch (Exception ex)
            {
                return new jResponse(true, "No template found!", null);
            }

            draft.content = draft.content.Replace("{productName}", productName);
            draft.content = draft.content.Replace("{link}", emailParams.link);
           // draft.content = draft.content.Replace("{source}", emailParams.source);
           // draft.content = draft.content.Replace("{destination}", emailParams.destination);
            draft.content = draft.content.Replace("{pickup}", emailParams.pickup);
            draft.content = draft.content.Replace("{delivery}", emailParams.delivery);
            draft.content = draft.content.Replace("{quoteby}", emailParams.quoteby);
            //  draft.content = draft.content.Replace("{product}", emailParams.product);
            foreach (var k in emailParams.emailDynamicFields)
            {
               dynamicFields += k.fieldValue == "" ? "" : " <p><b>" + k.fieldName + ": &nbsp;</b>" + k.fieldValue + "</p>";
            }

            draft.content = draft.content.Replace("{dynamicFields}", dynamicFields);


            draft.toList = toList;

            return sendEmail(draft);
        }

        //#region "Core"

        public static jResponse sendEmail(emailDraft myParams)
        {
            // #region "Validations"

            // Basic validations
            if (myParams == null || myParams.toList == null
                || myParams.subject == "" || myParams.content == "")
            {
                return new jResponse(true, "Invalid parameters", null);
            }

            // Extract parameters
            List<string> toList = myParams.toList;
            string subject = myParams.subject;
            string content = myParams.content;
            string webConDisplayName = WebConfigurationManager.AppSettings["MailDisplayName"];
           
            string displayName = myParams.displayName == null || myParams.displayName == "" ? webConDisplayName : myParams.displayName;
            string fromEmailAddress = WebConfigurationManager.AppSettings["MailAccount"];
            string fromEmailPassword = WebConfigurationManager.AppSettings["MailPassword"];
            MailAddress fromAddr = new MailAddress(fromEmailAddress, webConDisplayName);

            if (subject.Length > 80 || content.Length < 10 || toList.Count < 1)
            {
                return new jResponse(true, "Invalid parameters", null);
            }

            // #endregion

            // #region "Send email"

            foreach (var recipient in toList)
            {
                //string fromEmailAddress = WebConfigurationManager.AppSettings["MailAccount"];
                //string fromEmailPassword = WebConfigurationManager.AppSettings["MailPassword"];
                //MailMessage mail = new MailMessage(fromEmailAddress, recipient); //apdcomproject
                MailAddress toAddr = new MailAddress(recipient, displayName);
                MailMessage mail = new MailMessage(fromAddr, toAddr);
                mail.Subject = subject;
                mail.IsBodyHtml = true; // Raw HTML code will be displayed otherwise.
                mail.Body = content;

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(fromEmailAddress, fromEmailPassword);

                System.Threading.ThreadPool.QueueUserWorkItem(p =>
                {
                    try
                    {
                        client.Send(mail);
                    }
                    catch (Exception e)
                    {
                        var str = e;
                    }
                });
            }

            // #endregion

            return new jResponse(false, "Email(s) will be sent!", null);
        }

        //#endregion

        #endregion

        #region "Classes"

        public class emailDraft
        {
            public List<string> toList { get; set; }
            public string subject { get; set; }
            public string content { get; set; }
            public string displayName { get; set; }
        }

        #endregion
    }
}
