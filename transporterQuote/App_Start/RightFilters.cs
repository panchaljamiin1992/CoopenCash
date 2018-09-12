using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using transporterQuote.Services;
using transporterQuote.API;


namespace transporterQuote.Helper
{
    public class IsMajama : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            token myToken = genApiController.getPersonTkn();

            if (myToken == null || myToken.tokenID == "")
            {
                // Token has expired, so unauthorized access.
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "Token Expired"
                };
            }
            else
            {
                // TODO: Update condition to check if the person is a user
                base.OnAuthorization(actionContext);
            }
        }
    }

    public class IsSuperAdmin : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            token myToken = genApiController.getPersonTkn();

            if (myToken == null || myToken.tokenID == "")
            {
                // Token has expired, so unauthorized access.
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "Token Expired"
                };
            }
            else
            {
                if (myToken.isSuperAdmin)
                {
                    base.OnAuthorization(actionContext);
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                    {
                        ReasonPhrase = "Not Admin"
                    };
                }
            }
        }
    }
}