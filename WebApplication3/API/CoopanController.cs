using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication3.Models;
using WebApplication3.Repository;
using WebApplication3.ViewModels;

namespace WebApplication3.API
{
    [RoutePrefix("api/coopan")]
    public class CoopanController : ApiController
    {
        CoopanRepository objCoopanRepo = new CoopanRepository();
        public CoopanController()
        {
           // CoopanRepository objCoopanRepo = new CoopanRepository();
        }

        // GET: api/Coopan
        public IHttpActionResult Get()
        {
            int userId = 3;
            var data = objCoopanRepo.GetAllTickets(userId);
            return Ok(data);
        }

        // GET: api/Coopan/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Coopan
        public IHttpActionResult Post([FromBody]AddUserCoopanModel model)
        {
            int userId = 3;
            var res = objCoopanRepo.SubmitCoopan(model);
            if (res) {
                var data = objCoopanRepo.GetAllTickets(userId);
                return Ok(data);
            }
            return Ok(false);
        }

        // PUT: api/Coopan/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Coopan/5
        public void Delete(int id)
        {
        }
    }
}
