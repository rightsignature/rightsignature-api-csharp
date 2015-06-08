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
        recipients.Add(new Structs.Recipient("RightSignature", "noemail@rightsignature.com", "Document Sender", false, true));
        recipients.Add(new Structs.Recipient("John Bellingham", "noemail@rightsignature.com", "Receiver", true, true));

        // Optional create tags to associate with Document
        tags = new Dictionary<string, string>();
        tags.Add("test", "testUser");
        tags.Add("user", "Jonathan");

        ApiHelper.Initialize();
        IDocument doc = new DocumentApi();

        //Console.WriteLine(doc.SendDocument("base64", "C:\\Users\\deepikaas\\Desktop\\test.txt", "test", "test", tags, recipients, null, null, "send", null, null, true));
        //Console.WriteLine(doc.GetDocumentDetails("Y9TLYJJ335DU5EXZLNBYG9"));
        
        
       ITemplate tempApi = new TemplateApi();
       //Console.WriteLine(tempApi.GetTemplates());
       //Console.WriteLine(tempApi.BuildTemplate());
       //Console.WriteLine(tempApi.PrepackageTemplate("a_2915_20cd89c8ab494d5f9edb3c34dd05f1c2"));
       Console.WriteLine(tempApi.PreFillORSendAsDocument(ApiHelper.getGuid(tempApi.PrepackageTemplate("a_2915_20cd89c8ab494d5f9edb3c34dd05f1c2"), "template"), "test", "send", recipients, null, null, null, null, null, true));
       //Console.WriteLine(ApiHelper.GetSignerLinks("TN4ZLPIZ24HBTYIZ7BZF77"));
    }
  }
}