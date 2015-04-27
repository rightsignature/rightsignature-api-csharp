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
    }
}
