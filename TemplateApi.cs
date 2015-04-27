using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RightSignature
{
    class TemplateApi : TemplatesInterface
    {
        private OAuthRightSignature _oauth = new OAuthRightSignature();

        // Get all templates created in your account
        public string GetTemplates(string query = null, int? page = 1, int? perPage = 10, Dictionary<string, string> tags = null)
        {
            string urlPath = "/api/templates.xml";
            string requestPath;
            List<string> queryParams = new List<string>();

            // Build up the URL request path parameters
            if (query != null)
                queryParams.Add("search=" + query);
            if (page.HasValue)
                queryParams.Add("page=" + page.ToString());
            if (perPage.HasValue)
                queryParams.Add("per_page=" + perPage.ToString());
            // Creates parameter string for tags
            if (tags != null)
                queryParams.Add(ApiHelper.CreateTagsParameter(tags));

            // Creates URL path with query parameters in it
            requestPath = ApiHelper.CreateRequestPath(urlPath, queryParams);
            return _oauth.APIWebRequest("GET", requestPath);
        }
        public string GetTemplateDetails(string guid)
        {
            return _oauth.APIWebRequest("GET", "/api/templates" + guid + ".xml");
        }
        //This allows you to build a template 
        //The output document contains a redirect token which should be used in the RightSignature API to complete building the template

        public string BuildTemplate(Dictionary<string, string> tags = null, List<string> acceptableRoleNames = null, List<string> acceptableMergeFieldNames = null, string callback_location = null, string redirect_location = null)
        {
            XElement templateNode = new XElement("template");
            if (acceptableRoleNames != null)
            {
                templateNode.Add(ApiHelper.acceptableRoleNamesXML(acceptableRoleNames));
            }
            if (acceptableMergeFieldNames != null)
            {
                templateNode.Add(ApiHelper.acceptableMergeFieldsXML(acceptableMergeFieldNames));
            }
            if (tags != null)
            {
                templateNode.Add(ApiHelper.CreateTagsXML(tags));
            }
            if (callback_location != null)
            {
                templateNode.Add(new XElement("callback_location", callback_location));
            }
            if (redirect_location != null)
            {
                templateNode.Add(new XElement("redirect_location", redirect_location));
            }
            return _oauth.APIWebRequest("POST", "/api/templates/generate_build_token.xml", templateNode.ToString());
        }
        //provide a single or comma seperated string of guids to prepackage the templates into a single document to send
        public string PrepackageTemplate(string guidsString)
        {
            return(_oauth.APIWebRequest("POST", "/api/templates/" + guidsString + "/prepackage.xml", null));
        }

        public string GetPrepackagedGuid(string document)
        {
            XDocument xmlDocument = new XDocument(document);
            return xmlDocument.Element("template").Element("guid").Value;
        }

        //Send/Prefill the prepackaged template as a document using the guid that was generated during the prepackage process.
        public string PreFillORSendAsDocument(TemplateObj templateObj, string subject, string action = "send", string description = null, string callbackURL = null)
        {
            string urlPath = "/api/templates.xml";
            XElement rootNode = new XElement("template");
            XDocument xml = new XDocument(rootNode);

            // Creates the xml body to send to API
            rootNode.Add(new XElement("guid", templateObj.guid));
            rootNode.Add(new XElement("subject", subject));
            rootNode.Add(new XElement("action", action));               // Action can be 'send' or 'prefill' 
            if (description != null)
                rootNode.Add(new XElement("description", description));
            if (templateObj.expires_in != null)
                rootNode.Add(new XElement("expires_in", templateObj.expires_in.ToString()));        // Must be 2, 5, 15, or 30. Otherwise, API will default it to 30 days

            // Create Roles XML
            XElement rolesNode = new XElement("roles");
            foreach (DocumentObj.Recipient role in templateObj.roles)
            {
                XElement roleNode = new XElement("role");
                roleNode.SetAttributeValue("role_name", role.role);
                roleNode.Add(new XElement("name", role.name));
                roleNode.Add(new XElement("email", role.email));
                roleNode.Add(new XElement("locked", role.locked.ToString().ToLower()));
                rolesNode.Add(roleNode);
            }
            rootNode.Add(rolesNode);

            // Create mergefields XML
            if (templateObj.mergeFields != null)
            {
                XElement mfsNode = new XElement("merge_fields");
                foreach (TemplateObj.MergeField mergeField in templateObj.mergeFields)
                {
                    XElement mfNode = new XElement("merge_field");
                    mfNode.SetAttributeValue("merge_field_name", mergeField.name);
                    mfNode.Add(new XElement("value", mergeField.value));
                    mfNode.Add(new XElement("locked", mergeField.locked.ToString().ToLower()));
                    mfsNode.Add(mfNode);
                }
                rootNode.Add(mfsNode);
            }

            if (templateObj.tags != null)
                rootNode.Add(ApiHelper.CreateTagsXML(templateObj.tags));
            if (callbackURL != null)
                rootNode.Add(new XElement("callback_location", callbackURL));


            // Creates HTTP Request and parses it as XDocument
            return _oauth.APIWebRequest("POST", urlPath, xml.ToString());
        }
    }
}
