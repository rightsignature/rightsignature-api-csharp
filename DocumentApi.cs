using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RightSignature
{
    public class DocumentApi : IDocument
    {
        private OAuthRightSignature _oauth = new OAuthRightSignature();
        // Send document: 
        //filename, url and type are non optional params and must be set in the document object

        public string SendDocument(string type, string url, string subject, string fileName = "", Dictionary<string, string> tags = null, List<Structs.Recipient> recipients = null, string description = null, int? expires = null, string action = "send", string callback_location = null, string use_text_tags = null)
        {
            XElement rootNode = new XElement("document");
            XDocument xml = new XDocument(rootNode);
            rootNode.Add(new XElement("subject", subject));
            XElement document_data = new XElement("document_data");

            document_data.Add(new XElement("filename", fileName));//i/p param

            //if type = url => value the same Url would be sent as value to be downloaded and processed by the API 
            //else if type = base64 => value is base64 of the file 
            document_data.Add(new XElement("type", type));//i/p param
            if (type == "base64")
            {               
                Byte[] bytes = File.ReadAllBytes(url);
                String file_contents = Convert.ToBase64String(bytes);
                document_data.Add(new XElement("value", file_contents));
            }
            else if (type == "url")
            {
                document_data.Add(new XElement("value", url));
            }
            rootNode.Add(document_data);

            if (recipients != null)
                rootNode.Add(ApiHelper.createRolesXML(recipients));
            if (tags != null)
                rootNode.Add(ApiHelper.CreateTagsXML(tags));
           
            rootNode.Add(new XElement("action", action));               // Action can be 'send' or 'redirect' 
            if (description != null)
                rootNode.Add(new XElement("description", description));
            if (expires != null)
                rootNode.Add(new XElement("expires_in", expires.ToString()));
            if (callback_location != null)
                rootNode.Add(new XElement("callback_location", callback_location));
            if (use_text_tags != null)
                rootNode.Add(new XElement("use_text_tags", use_text_tags));         
           
            return _oauth.APIWebRequest("POST", "/api/documents.xml", xml.ToString());
        }

        public string GetDocuments(string query = null, string docStates = null, int? page = null, int? perPage = null, string recipientEmail = null, Dictionary<string, string> tags = null)
        {
            string urlPath = "/api/documents.xml";
            string requestPath;
            List<string> queryParams = new List<string>();

            // Build up the URL request path parameters
            if (query != null)
                queryParams.Add("search=" + query);
            if (docStates != null)
                queryParams.Add("state=" + docStates);
            if (page.HasValue)
                queryParams.Add("page=" + page.ToString());
            if (perPage.HasValue)
                queryParams.Add("per_page=" + perPage.ToString());
            if (recipientEmail != null)
                queryParams.Add("recipient_email=" + recipientEmail.ToString());

            // Creates parameter string for tags
            if (tags != null)
                queryParams.Add(ApiHelper.CreateTagsParameter(tags));

            // Creates URL path with query parameters in it
            requestPath = ApiHelper.CreateRequestPath(urlPath, queryParams);

            return _oauth.APIWebRequest("GET",  requestPath , null);
        }
        
        //Get a document using GUID
        public string GetDocumentDetails(string guid)
        {
            return _oauth.APIWebRequest("GET", "/api/documents/" + guid + ".xml");
        }

        //Send document to trash - will be no longer available for signature
        public string DeleteDocument(string guid)
        {
            return _oauth.APIWebRequest("POST", "/api/documents/" + guid + "/trash.xml");
        }
        //Resend reminder emails for document
        public string ResendReminder(string guid)
        {
            return _oauth.APIWebRequest("POST", "/api/documents/" + guid + "/send_reminders.xml");
        }
        //Extend expiration for document
        public string ExtendExpiration(string guid)
        {
            return _oauth.APIWebRequest("POST","/api/documents/" + guid + "/extend_expiration.xml");
        }
        //Extend expiration for document
        public string UpdateDocumentTags(string guid, Dictionary<string, string> tags)
        {
            string tagsXml = null;
            if (tags != null)
            {
                tagsXml = ApiHelper.CreateTagsXML(tags).ToString();
            }
            return _oauth.APIWebRequest("POST", "/api/documents/" + guid + "/update_tags.xml", tagsXml);
        }
    }
}
