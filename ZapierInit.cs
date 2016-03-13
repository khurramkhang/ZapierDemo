using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Zapier
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ZapierInit : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RestWebHook.CleanStore();
            var clients = GetRestHooks();
            foreach (var client in clients)
            {
                RegisterZapierHook(client.ZappierRestUrl, client.EventName);
                //For demo only - Send JSON format to Zapier hooks
                RestWebHook.InvokeRestHooks(ZapierConstants.NewProduct, new { ProductName = "SetupName", Brand = "SetupBrand", ProductUrl = "SetupUrl" });
            }

            var events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.PublishingContent += events_PublishingContent;
        }

        void events_PublishingContent(object sender, ContentEventArgs e)
        {
            var product = e.Content as FashionProduct;
            if (product != null && product.PublishItOnZapier)
            {
                RestWebHook.InvokeRestHooks(ZapierConstants.NewProduct, new { ProductName = product.Name, Brand = product.Brand, ProductUrl = string.Format("http://www.mysite.co.uk/{0}", product.SeoUri) });
            }
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }

        private void RegisterZapierHook(string urlAddress, string eventName)
        {
            string url = urlAddress;
            string evnt = eventName;
            RestWebHook rh = new RestWebHook();
            rh.Url = url;
            rh.EventName = evnt;
            rh.SaveRestHook();
        }

        private List<ZapierClient> GetRestHooks()
        {
            List<ZapierClient> clients = new List<ZapierClient>();
            string key = "zappierclients";
            if (System.Configuration.ConfigurationManager.AppSettings[key] != null)
            {
                foreach (var client in Convert.ToString(System.Configuration.ConfigurationManager.AppSettings[key]).Split(','))
                {
                    var zC = client.Split('|');
                    clients.Add(new ZapierClient()
                    {
                        ZappierRestUrl = zC[0],
                        EventName = zC[1]
                    });

                }
            }

            return clients;
        }
    }

}