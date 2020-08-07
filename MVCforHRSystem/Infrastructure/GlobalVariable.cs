using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace HR.Website
{
    public static class GlobalVariable
    {
        public static HttpClient ApiClient = new HttpClient();

        static GlobalVariable()
        {
            ApiClient.BaseAddress = new Uri("https://localhost:44352/api/");
            ApiClient.DefaultRequestHeaders.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }
        public static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    }
}