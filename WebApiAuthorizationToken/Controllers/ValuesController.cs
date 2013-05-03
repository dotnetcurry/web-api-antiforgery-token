using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Helpers;
using System.Web.Http;

namespace WebApiAuthorizationToken.Controllers
{
    public class ValuesController : ApiController
    {

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [Authorize]
        // POST api/values
        public void Post([FromBody]string value)
        {
            CookieHeaderValue cookie = Request.Headers
                .GetCookies(AntiForgeryConfig.CookieName)
                .FirstOrDefault();
            if (cookie != null)
            {
                Stream requestBufferedStream = Request.Content.ReadAsStreamAsync().Result;
                requestBufferedStream.Position = 0;
                NameValueCollection myform = Request.Content.ReadAsFormDataAsync().Result;
                try
                {
                    AntiForgery.Validate(cookie[AntiForgeryConfig.CookieName].Value, myform[AntiForgeryConfig.CookieName]);
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}