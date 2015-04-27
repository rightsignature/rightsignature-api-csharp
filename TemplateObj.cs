using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightSignature
{
    public class TemplateObj
    {

        public Dictionary<string, string> tags { get; set; }

        //Send params
        //required fields
        public string guid { get; set; }
        public string subject { get; set; }
        public List<DocumentObj.Recipient> roles { get; set; }
        public int? expires_in { get; set; }
        public string description { get; set; }
        public string callback_location { get; set; }
        
        public List<MergeField> mergeFields { get; set; }

        public TemplateObj()
        {
            tags = null;
            expires_in = null;
            description = null;
        }

        public struct MergeField
        {
            public string name, value;
            public Boolean locked;         // If true, not allow the redirected user to modify the value
            public MergeField(string mName, string mValue, Boolean isLocked)
            {
                name = mName;
                value = mValue;
                locked = isLocked;
            }

        }
    }
}
