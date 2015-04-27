using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightSignature
{
    public class Configuration
    {
        public static string Get(string key, string defaultValue = "")
        {
            string retVal = ConfigurationManager.AppSettings[key];
            return retVal ?? defaultValue;
        }
        public static string BaseUrl
        {
            get { return (Get("baseUrl")); }
        }
        public static string AuthType
        {
            get { return (Get("authType")); }
        }
        public static string RequestTokenUrl
        {
            get { return (Get("baseUrl") + "/oauth/request_token"); }
        }
        public static string AuthorizeUrl
        {
            get { return (Get("baseUrl") + "/oauth/authorize"); }
        }
        public static string AccessTokenUrl
        {
            get { return (Get("baseUrl") + "/oauth/access_token"); }
        }
        public static string CallbackUrl
        {
            get { return (Get("callback_url")); }
        }
        public static string ConsumerKey
        {
            get { return (Get("consumer_key")); }
        }
        public static string ConsumerSecret
        {
            get { return (Get("consumer_secret")); }
        }
        public static string ApiToken
        {
            get { return (Get("api-token")); }
        }
    }
}
