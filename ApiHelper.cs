using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RightSignature
{
    public class ApiHelper
    {

        public static void Initialize()
        {
            OAuthRightSignature _oauth = new OAuthRightSignature();
            if (Configuration.AuthType == "oauthtoken")
                _oauth._initialize();
        }


        public static XElement createRolesXML(List<Structs.Recipient> recipients)
        {
            // Create Roles XML
            XElement rolesNode = new XElement("recipients");
            foreach (Structs.Recipient recipient in recipients)
            {
                XElement roleNode = new XElement("recipient");
                roleNode.Add(new XElement("role", recipient.role));
                roleNode.Add(new XElement("name", recipient.name));
                roleNode.Add(new XElement("email", recipient.email));

                if (recipient.is_sender)
                {
                    roleNode.Add(new XElement("is_sender", "true"));
                }
                rolesNode.Add(roleNode);
            }
            return rolesNode;
        }

        // Converts Dictionary into tags XML Element
        public static XElement CreateTagsXML(Dictionary<string, string> tags)
        {
            XElement tagsNode = new XElement("tags");
            foreach (KeyValuePair<string, string> tag in tags)
            {
                XElement tagNode = new XElement("tag");
                tagNode.Add(new XElement("name", tag.Key));
                if (tag.Value != null)
                    tagNode.Add(new XElement("value", tag.Value));
                tagsNode.Add(tagNode);
            }
            return tagsNode;
        }
        // Converts a List and request path into one request path with paramters
        public static string CreateRequestPath(string path, List<string> parameters)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                path += i == 0 ? "?" : "&";
                path += parameters[i];
            }
            return path;
        }
        public static string CreateTagsParameter(Dictionary<string, string> tags)
        {
            string tagsParam = "tags=";
            int i = 0;
            foreach (KeyValuePair<string, string> tag in tags)
            {
                if (i > 0)
                    tagsParam += ',';
                if (tag.Value == null)
                    tagsParam += tag.Key;
                else
                    tagsParam += tag.Key + ":" + tag.Value;
                i++;
            }
            return tagsParam;
        }

        public static XElement acceptableRoleNamesXML(List<string> roleNames)
        {
            XElement roleNamesXml = new XElement("acceptable_role_names");
            foreach (string roleName in roleNames)
            {
                roleNamesXml.Add(new XElement("name", roleName));
            }
            return roleNamesXml;
        }

        public static XElement acceptableMergeFieldsXML(List<string> mergeFields)
        {
            XElement mergeFieldNamesXml = new XElement("acceptable_merge_field_names");
            foreach (string mergeField in mergeFields)
            {
                mergeFieldNamesXml.Add(new XElement("name", mergeField));
            }
            return mergeFieldNamesXml;
        }
        //extract the guid from the document
        //type = document or template
        public static string getGuid(string document, string type)
        {
            XDocument xmlDocument = XDocument.Parse(document);
            return xmlDocument.Element(type).Element("guid").Value;
        }
        public static string GetSignerLinks(string guid)
        {
            string urlPath = "/api/documents/" + guid + "/signer_links.xml";
            OAuthRightSignature _oauth = new OAuthRightSignature();

            string signerLinksXml = _oauth.APIWebRequest("GET", urlPath, null);

            XDocument doc = XDocument.Parse(signerLinksXml);
            XElement rootNode = new XElement("document");
            XDocument returnXml = new XDocument(rootNode);
            rootNode.Add(new XElement("guid", guid));
            XElement signer_links = new XElement("signer-links");

            foreach (XElement element in doc.Element("document").Element("signer-links").Elements("signer-link"))
            {
                Console.WriteLine("Name: {0}; Value: {1}",
                    (string)element.Attribute("name"),
                    (string)element.Element("role"));
                XElement signer_link = new XElement("signer-link");
                signer_link.Add(new XElement("name", (string)element.Element("name")));
                signer_link.Add(new XElement("role", (string)element.Element("role")));
                signer_link.Add(new XElement("link", Configuration.BaseUrl + "/signatures/embedded?rt=" + (string)element.Element("signer-token")));
                signer_links.Add(signer_link);
            }
            rootNode.Add(signer_links);
            return rootNode.ToString();
        }
    }
}
