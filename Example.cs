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
  
  public class Example  {

      private static List<Structs.Recipient> recipients = null;
    private static Dictionary<string, string> tags = null;

    static void Main(string[] args)
    {
        recipients = new List<Structs.Recipient>();
        recipients.Add(new Structs.Recipient("RightSignature", "support@rightsignature.com", "cc", false, true));
        recipients.Add(new Structs.Recipient("John Bellingham", "john@rightsignature.com", "signer", true, true));

        // Optional create tags to associate with Document
        tags = new Dictionary<string, string>();
        tags.Add("test", "testUser");
        tags.Add("user", "Jonathan");

        ApiHelper.Initialize();
        IDocument doc = new DocumentApi();

        //Console.WriteLine(doc.SendDocument("base64", "url", "subject", "fileName", tags, recipients));
        Console.WriteLine(doc.GetDocumentDetails("Y9TLYJJ335DU5EXZLNBYG9"));
        
        
       ITemplate tempApi = new TemplateApi();
       Console.WriteLine(tempApi.GetTemplates());
    }
  }
}