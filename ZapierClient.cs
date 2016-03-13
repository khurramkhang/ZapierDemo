using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Zapier
{
    public static class ZapierConstants {
        public const string NewProduct = "newproduct";
    }

    public class ZapierClient
    {
        public string ZappierRestUrl { get; set; }
        public string EventName { get; set; }
    }
}