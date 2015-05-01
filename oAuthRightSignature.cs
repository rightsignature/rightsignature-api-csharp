using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace RightSignature
{

    public class OAuthRightSignature : oAuthBase
    {

        public enum Method { GET, POST, PUT, DELETE };

        private static string _token;
        private static string _tokenSecret;

        #region PublicProperties
        public static string Token { get { return _token; } set { _token = value; } }
        public static string TokenSecret { get { return _tokenSecret; } set { _tokenSecret = value; } }
        #endregion

        public void _initialize()
        {
            String requestToken = this.getRequestToken();

            Console.WriteLine(this.AuthorizationLink);
            Console.WriteLine("Visit the link above to authorize and then paste the oauth_verifier value here and hit ENTER:");
            String verifier = Console.ReadLine();

            this.setVerifier(verifier);
            String accessToken = this.getAccessToken();
        }

        /// <summary>
        /// Get the linkedin request token using the consumer key and secret.  Also initializes tokensecret
        /// </summary>
        /// <returns>The request token.</returns>
        public String getRequestToken() {
            string ret = null;
            string response = oAuthWebRequest(Method.POST, Configuration.RequestTokenUrl, String.Empty);
            if (response.Length > 0)
            {
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    Token = qs["oauth_token"];
                    TokenSecret = qs["oauth_token_secret"];
                    ret = Token;
                }
            }
            return ret;
        }

        /// <summary>
        /// Get the access token
        /// </summary>
        /// <returns>The access token.</returns>
        public String getAccessToken() {
            if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Verifier))
            {
                Exception e = new Exception("The request token and verifier were not set");
                throw e;
            }

            string response = oAuthWebRequest(Method.POST, Configuration.AccessTokenUrl, string.Empty);

            if (response.Length > 0)
            {
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    Token = qs["oauth_token"];
                }
                if (qs["oauth_token_secret"] != null)
                {
                    TokenSecret = qs["oauth_token_secret"];
                }
            }

            return Token;
        }

        public void setVerifier(string _verifier)
        {
            Verifier = _verifier;
        }

        /// <summary>
        /// Get the link to Linked In's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLink
        {
            get { return Configuration.AuthorizeUrl + "?oauth_token=" + Token; }
        }

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public string oAuthWebRequest(Method method, string url, string postData)
        {
            string outUrl = "";
            string querystring = "";
            string ret = "";

            //Setup postData for signing.
            //Add the postData to the querystring.
            if (method == Method.POST)
            {
                if (postData.Length > 0)
                {
                    //Decode the parameters and re-encode using the oAuth UrlEncode method.
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }
                        qs[key] = HttpUtility.UrlDecode(qs[key]);
                        qs[key] = this.UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];

                    }
                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }

            Uri uri = new Uri(url);

            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            string callback = "";
            if (url.ToString().Contains(Configuration.RequestTokenUrl))
                callback = Configuration.CallbackUrl;

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                Configuration.ConsumerKey,
                Configuration.ConsumerSecret,
                Token,
                TokenSecret,
                method.ToString(),
                timeStamp,
                nonce,
                callback,
                out outUrl,
                out querystring);


            querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

            //Convert the querystring to postData
            if (method == Method.POST)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            if (method == Method.POST || method == Method.GET)
                ret = WebRequest(method, outUrl + querystring, postData);

            return ret;
        }

        /// <summary>
        /// WebRequestWithPut
        /// </summary>
        /// <param name="method">WebRequestWithPut</param>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string APIWebRequest(string method, string url, string postData = null)
        {
            HttpWebRequest webRequest = null;

            url = Configuration.BaseUrl + url;
            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (method == "POST") {
            webRequest.Method = method;
              webRequest.ContentType = "text/xml";
            }
            if (Configuration.AuthType == "securetoken")
            {
                webRequest.Headers.Add("api-token", Configuration.ApiToken);
            }
            else if (Configuration.AuthType == "oauthtoken")
            {
                webRequest.Credentials = CredentialCache.DefaultCredentials;
                webRequest.AllowWriteStreamBuffering = true;

                webRequest.PreAuthenticate = true;
                webRequest.ServicePoint.Expect100Continue = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                string header = _getHeader(url, method);
                webRequest.Headers.Add("Authorization", header);
            }

            if (postData != null)
            {
                byte[] fileToSend = Encoding.UTF8.GetBytes(postData);
                webRequest.ContentLength = fileToSend.Length;

                Stream reqStream = webRequest.GetRequestStream();

                reqStream.Write(fileToSend, 0, fileToSend.Length);
                reqStream.Close();
            }

            string returned = WebResponseGet(webRequest);

            return returned;
        }


        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(Method method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;

            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;

        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }

        public string _getHeader(string url, string method)
        {
            Uri uri = new Uri(url);
            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            string outUrl, querystring;

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                Configuration.ConsumerKey,
                Configuration.ConsumerSecret,
                Token,
                TokenSecret,
                method,
                timeStamp,
                nonce,
                null,
                out outUrl,
                out querystring);

            return ("OAuth realm=\"" + Configuration.BaseUrl + "\",oauth_consumer_key=\"" + Configuration.ConsumerKey + "\",oauth_token=\"" + Token + "\",oauth_signature_method=\"HMAC-SHA1\",oauth_signature=\"" + HttpUtility.UrlEncode(sig) + "\",oauth_timestamp=\"" + timeStamp + "\",oauth_nonce=\"" + nonce + "\",oauth_verifier=\"" + Verifier + "\", oauth_version=\"1.0\"");
        }
    }
}
