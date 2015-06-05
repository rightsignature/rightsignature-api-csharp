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
        recipients.Add(new Structs.Recipient("RightSignature", "noemail@rightsignature.com", "cc", false, true));
        recipients.Add(new Structs.Recipient("John Bellingham", "noemail@rightsignature.com", "signer", true, true));

        // Optional create tags to associate with Document
        tags = new Dictionary<string, string>();
        tags.Add("test", "testUser");
        tags.Add("user", "Jonathan");

        ApiHelper.Initialize();
        IDocument doc = new DocumentApi();

        Console.WriteLine(doc.SendDocument("base64", "filepath", "test", "test", tags, recipients, null, null, "send", null, null, true));
        //Console.WriteLine(doc.GetDocumentDetails("guid"));
        
        
       ITemplate tempApi = new TemplateApi();
       Console.WriteLine(tempApi.GetTemplates());
    }
  }
}