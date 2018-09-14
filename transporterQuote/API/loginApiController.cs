using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Configuration;
using System.Runtime.Caching;
using transporterQuote.Services;
using transporterQuote.API;
using transporterQuote.Models;
using transporterQuote.Helper;
using System.Data;

namespace transporterQuote.API
{
    [RoutePrefix("api/login")]
    public class loginApiController : ApiController
    {
        #region "Functions"

        /*
           - login()
           - Purpose: Log a user in.
           - In: class myParams {
                           userName,
                           password
                       }
           - Out: token
        */
        [HttpPost, HttpGet, Route("login")]
        public dynamic login(dynamic myParams)
        {
            // #region "Validations"

            // Basic validations
            if (myParams == null || myParams.userName == "" || myParams.password == "")
            {
                return new jResponse();
            }

            // Extract parameters
            string userName = myParams.userName;
            string password = myParams.password;
          
            // #endregion

            // #region "Generate token"

            MemoryCache personToken = MemoryCache.Default;
            genApiController gen = new genApiController();
            // Keep changing Guid until a unique one is found.
            // This is to ensure that Guid is not repeated.
            token retToken = new token();
            retToken.tokenID = Guid.NewGuid().ToString();
            while (personToken.Contains(retToken.tokenID))
            {
                retToken.tokenID = Guid.NewGuid().ToString();
            }
            retToken.canForward = false;
            retToken.canDispatchCreate = false;
            retToken.canUpdateStatus = false;
            retToken.canScheduled = false;
            retToken.canLoading = false;
            retToken.canDispatchView = false;
            retToken.canStart = false;
            retToken.canFinish = false;
            retToken.canFeedback = false;
            // #endregion

            // #region "Check for SuperAdmin"

            //string adminUserName = WebConfigurationManager.AppSettings["LoginID"];
            //string adminPassword = WebConfigurationManager.AppSettings["Password"];

          

            using (CashCoopanEntities db = new CashCoopanEntities())
            {

                var executiveInfo = (from dbe in db.Users
                                     where dbe.Email == userName && dbe.Password == password && !dbe.IsDeleted
                                     select new { dbe }).FirstOrDefault();



                if (executiveInfo != null)
                {
                   //List<int> rightIDs = gen.splitString('~', executiveInfo.dbe.Rights).Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                   

                    retToken.userID = executiveInfo.dbe.UserID;
                    retToken.name = executiveInfo.dbe.FName + ' ' + executiveInfo.dbe.LName;
                    //retToken.serviceRights = executiveInfo.dbe.ServiceRights;

                   

                    genApiController.setPersonTkn(retToken);

                } else
                {
                    return new jResponse(true, "Please enter valid credentials.", null);
                }
                return new jResponse(false, "Welcome, " + retToken.name + "!", retToken);
            }



            // #endregion

            // #endregion                           
        }

        /*
           - logout()
           - Purpose: Log a user out.
           - In: class myParams {
                           userID
                       }
           - Out: Success / failure
        */
        [HttpPost, HttpGet, Route("logout")]
        public dynamic logout(dynamic myParams)
        {
            // #region "Validations"

            // Basic Validations
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
            bool isSuperAdmin = myToken.isSuperAdmin;

            if ((isSuperAdmin && userID != myToken.userID) ||
                    (!isSuperAdmin && userID != myToken.userID))
            {
                return new jResponse(true, "You're not allowed to access this data.", null);
            }

            // #endregion

            // #region "Logout"

            // Delete token object from cache, if present.
            MemoryCache memCache = MemoryCache.Default;
            if (memCache.Contains(myToken.tokenID))
            {
                memCache.Remove(myToken.tokenID);
            }

            // #endregion

            return new jResponse(false, "Bye for now!", null);
        }

        /*
           - resetPassword()
           - Purpose: Forgot password.
           - In: class myParams {
                           userID
                       }
           - Out: Success / failure
        */
        [HttpPost, HttpGet, Route("password/forgot")]
        public dynamic resetPassword(dynamic myParams)
        {
            // #region "Validations"
            if (myParams == null || myParams.email == "")
            {
                return new jResponse();
            }

            // Extract parameters
            string email = myParams.email;
            string password = "";
            DateTime currentDT = genApiController.getDate();
            // Get executive list
            using (CashCoopanEntities db = new CashCoopanEntities())
            {
               

                // Check for valid mobile number
                var dbExecutiveInfo = db.Users.Where(i => i.UserName == email && !i.IsDeleted).FirstOrDefault();

                if (dbExecutiveInfo != null)
                {
                    //password = currentDT.Second.ToString("00").Substring(0, 1) + dbExecutiveInfo.Mobile.Substring(5, 1) + dbExecutiveInfo.Mobile.Substring(1, 1) + currentDT.Millisecond.ToString("000").Substring(0, 1);
                    //help_sendPassword(dbExecutiveInfo.Email, password);
                    //dbExecutiveInfo.Password = password;
                    //dbExecutiveInfo.UpdatedDT = currentDT;
                    //db.Entry(dbExecutiveInfo).State = EntityState.Modified;
                    //db.SaveChanges();
                }
                else
                {
                    return new jResponse(true, "Please enter valid email address.", null);
                }
                return new jResponse(false, "Your new password has been sent to you by SMS.", null);
            }
        }

        public static jResponse help_sendPassword(dynamic mobile, dynamic pwd)
        {
            // Send OTP
            List<string> toList = new List<string>();
            toList.Add(mobile);

            //var smsRetVal = smsApiController.send_password(toList, pwd);
            //if (smsRetVal.error)
            //{
            //    return new jResponse(true, "Something went wrong. Please try again later.", null);
            //}

            return new jResponse(false, "Sent successfully", null);
        }

        #endregion

        #region "Classes"

        #endregion
    }
}