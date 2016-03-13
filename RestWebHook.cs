using EPiServer.Data.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Zapier
{
    public class RestWebHook : IDynamicData
    {
        public EPiServer.Data.Identity Id
        {
            get;
            set;
        }

        public string Url { get; set; }

        public string EventName { get; set; }


        private static DynamicDataStore GetStore()
        {
            return DynamicDataStoreFactory.Instance.CreateStore(typeof(RestWebHook));
        }


        public static void InvokeRestHooks(string EventName, object Data)
        {
            HttpClient cli = new HttpClient();
            foreach (var r in GetStore().Items<RestWebHook>().Where(rh => rh.EventName == EventName))
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
                cli.PostAsJsonAsync(r.Url, jsonData);
            }
        }

        public Guid SaveRestHook()
        {
            var items = GetStore().Items<RestWebHook>().Where(rh => rh.Url == this.Url);
            if (items != null)
            {
                if (items.Count() <= 0)
                {
                    return GetStore().Save(this).ExternalId;
                }
                else
                {
                    return items.First().Id.ExternalId;
                }
            }
            else
                return Guid.Empty;
        }

        public static void DeleteRestHook(string ID)
        {
            GetStore().Delete(Guid.Parse(ID));
        }

        public static void CleanStore()
        {
            GetStore().DeleteAll();
        }
    }
}