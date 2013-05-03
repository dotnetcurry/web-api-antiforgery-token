using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace WebApiAuthorizationToken.Handlers
{
    public class AntiForgeryTokenHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            bool isCsrf = true;
            CookieHeaderValue cookie = request.Headers
                                          .GetCookies(AntiForgeryConfig.CookieName)
                                          .FirstOrDefault();
            if (cookie != null)
            {
                Stream requestBufferedStream = request.Content.ReadAsStreamAsync().Result;
                requestBufferedStream.Position = 0;
                NameValueCollection myform = request.Content.ReadAsFormDataAsync().Result;
                requestBufferedStream.Position = 0;
                try
                {
                    AntiForgery.Validate(cookie[AntiForgeryConfig.CookieName].Value,
                     myform[AntiForgeryConfig.CookieName]);
                    isCsrf = false;
                }
                catch (Exception ex)
                {
                    return request.CreateResponse(HttpStatusCode.Forbidden);
                }
            }
            if (isCsrf)
            {
                return request.CreateResponse(HttpStatusCode.Forbidden);
            }
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}