using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightSignature
{
    public class DocumentObj
    {
        //Send
        public string type { get; set; }
        public string filename { get; set; }
        public string url { get; set; }
        public Dictionary<string, string> tags { get; set; }
        public int? expires_in { get; set; }
        public string description { get; set; }
        public List<Recipient> recipients { get; set; }

        public DocumentObj()
        {
            type = "base64"; //"url" or "base64"
            filename = "test"; // only if type is "base64"
            url = ""; //url to the file
            expires_in = null;
            description = null;
            tags = null;
            recipients = null;
        }

        public struct Recipient
        {
            public string name, email, role;
            public Boolean locked, is_sender;         // If true, not allow the redirected user to modify the value
            public Recipient(string uName, string uEmail, string uRole, Boolean isSender = false, Boolean isLocked = false)
            {
                name = uName;
                email = uEmail;
                role = uRole;
                is_sender = isSender;
                locked = isLocked;
            }
        }
    }
}
