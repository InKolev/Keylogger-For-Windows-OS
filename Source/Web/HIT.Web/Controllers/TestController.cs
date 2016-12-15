using System;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HIT.Web.Controllers
{
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        [HttpPost]
        [Route("CurrentDate")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetCurrentDate()
        {
            return this.Ok(DateTime.Now.ToShortDateString());
        }

        [HttpPost]
        [Route("CurrentTime")]
        public IHttpActionResult GetCurrentTime()
        {
            return this.Ok(DateTime.Now.ToShortTimeString());
        }
    }
}
