using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightSignature
{
    public interface IDocument
    {
        // Send document: 
        //document_data (required)
        //A node containing type and value nodes along with an optional filename node. Type can either be "base64" or "url". Value corresponds to the respective type chosen (either a URL where the document can be fetched or a base64 encoded string of the document). The filename node must be provided for "base64" types only.
        //recipients (required)
        //A node containing nodes for each recipient involved (including the sender). The recipient node for the sender should include is_sender set to true and role set to either "signer" or "cc". Each non-sender recipient node should specify a name, email, and role, where role is either "signer" or "cc".
        //subject (required)
        //A node specifying the subject of the document.
        //action (required)
        //A node specifying whether to validate and send the document, or whether to prefill the information and return a redirect token. The options are "send" or "redirect".
        //expires_in (optional)
        //A node specifying the length of time the document will be valid, in days. For example: "5 days" (Allowed values are {2, 5, 15, 30} days). If this node is absent, the document will expire in 30 days.
        //description (optional)
        //A node specifying the description of the document.
        //tags (optional)
        //A node specifying tags to attach to the document. Tags can be specified as simple tags (name only) or tuples (name/value) pairs. See example below.
        //callback_location (optional)
        //A URI encoded URL that specifies the location we will POST a callback notification to when the document has been created.
        //use_text_tags (optional)
        //(true/false) A node specifying that the Document should parse for Text Tags. 
        string SendDocument(string type, string url, string subject, string fileName = "", Dictionary<string, string> tags = null, List<Structs.Recipient> recipients = null, string description = null, int? expires = null, string action = "send", string callback_location = null, string use_text_tags = null, bool embedded_signing = false);
       
        //Get all documents created by the user for that account
        //page (optional)
        //Page number starting at 1
        //per_page (optional)
        //Number of documents to return per page. 10, 20, 30, 40, or 50. Default: 10
        //tags (optional)
        //Return only documents with the given tags and name/value tags. Format should be comma-separated with name/value pairs (if applicable) joined by a ':'. Example: '&tags=nda,organization_id:123' would return documents tagged with 'NDA' and have a name/value tag 'organization_id' with value '123'. See Tagging for more information.
        //search (optional)
        //A search term to assist in narrowing results. (URI encoded if necessary)
        //state (optional)
        //Filter documents by their state. Acceptable document states are 'completed', 'pending', 'trash' and should be joined comma-separated. 
        //recipient_email (optional)
        //Return only documents that include the oAuth calling user and a party with the given recipient_email
        string GetDocuments(string query = null, string docStates = null, int? page = null, int? perPage = null, string recipientEmail = null, Dictionary<string, string> tags = null);
        
        //Returns only the document that matches the given guid
        string GetDocumentDetails(string guid);

        //Moves a Document to the "Trash". The Document will no longer be available for signature.
        //Document guid to trash
        string DeleteDocument(string guid);

        
        string ResendReminder(string guid);
        
        //Extends the expiration date of a Document by 7 days.
        string ExtendExpiration(string guid);

        //Update the tags on a given document. All old tags are removed.
        //Params: Document guid to update the tags, Dictionary of new tags(name, value)
        string UpdateDocumentTags(string guid, Dictionary<string, string> tags);

    }

}
