using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightSignature
{
    public class Structs
    {
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
