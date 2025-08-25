using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowService.Helpers
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this HttpSessionStateBase session, string key, object value)
        {
            session[key] = JsonConvert.SerializeObject(value);
        }

        public static T GetObjectFromJson<T>(this HttpSessionStateBase session, string key)
        {
            var value = session[key];
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value.ToString());
        }

        public static bool HasKey(this HttpSessionStateBase session, string key)
        {
            return session[key] != null;
        }
    }
}