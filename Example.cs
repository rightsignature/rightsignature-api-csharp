using System;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;

namespace RightSignature
{
  
  public class Example {

    private static List<DocumentObj.Recipient> recipients = null;
    private static Dictionary<string, string> tags = null;

    static void Main(string[] args)
    {
        recipients = new List<DocumentObj.Recipient>();
        recipients.Add(new DocumentObj.Recipient("RightSignature", "support@rightsignature.com", "cc", false, true));
        recipients.Add(new DocumentObj.Recipient("John Bellingham", "john@rightsignature.com", "signer", true, true));

        // Optional create tags to associate with Document
        tags = new Dictionary<string, string>();
        tags.Add("test", "testUser");
        tags.Add("user", "Jonathan");

        ApiHelper.Initialize();
        DocumentApi docapi = new DocumentApi();
        
        DocumentObj docObj = new DocumentObj(){
                        type = "base64",
                        url = "filepath",
                        filename = "FileName",
                        tags = tags,
                        recipients = recipients
        };
        Console.WriteLine(docapi.SendDocument(docObj, "SampleApiOauthDoc"));
        Console.WriteLine(docapi.GetDocumentDetails("Y9TLYJJ335DU5EXZLNBYG9"));
        
        
        TemplateApi tempApi = new TemplateApi();
        TemplateObj templateObj = new TemplateObj()
        {
            guid = "", // enter the guid of prepackaged template.
            roles = recipients,
            subject = "Test Templates"

        };
       Console.WriteLine(tempApi.GetTemplates());
    }
  }
}