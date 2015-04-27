using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightSignature
{
    public interface ITemplate
    {
       
        // Get all templates 
        //page (optional)
        //Page number starting at 1
        //per_page (optional)
        //Number of templates to return per page. 10, 20, 30, 40, or 50. Default: 10
        //tags (optional)
        //Return only templates with the given tags and name/value tags. Format should be comma-separated with name/value pairs (if applicable) joined by a ':'. Example: '&tags=nda,business' would return templates tagged with 'NDA' and 'Business'. See Tagging for more information.
        //search (optional)
        //A search term to assist in narrowing results. (URI encoded if necessary)
        string GetTemplates(string query = null, int? page = 1, int? perPage = 10, Dictionary<string, string> tags = null);
        
        //Get the template details for a given guid
        string GetTemplateDetails(string guid);
        
        //This allows you to build a template 
        //callback_location (optional)
        //A URI encoded URL that specifies the location we will POST a callback notification to when the template has been created.
        //redirect_location (optional)
        //A URI encoded URL that specifies the location we will redirect the user to once they have created a template.
        //tags (optional)
        //These tags will be applied to the template that is created via the RedirectToken generated. This can be used to reference the Template in the future.
        //acceptable_role_names (optional)
        //The user creating the Template will be forced to select one of the values provided. There will be no free-form name entry when adding roles to the Template.
        //acceptable_merge_field_names (optional)
        //The user creating the Template will be forced to select one of the values provided. There will be no free-form name entry when adding merge fields to the Template.
        string BuildTemplate(Dictionary<string, string> tags = null, List<string> acceptableRoleNames = null, List<string> acceptableMergeFieldNames = null, string callback_location = null, string redirect_location = null);
        
        //provide a single or comma seperated string of guids to prepackage those templates into a single document
        string PrepackageTemplate(string guidsString);
        
        //Given the output from PrePackageTemplate function, this returns the guid created for the template.
        string GetPrepackagedGuid(string document);

        //Send/Prefill the prepackaged template as a document using the guid that was generated during the prepackage process.
        string PreFillORSendAsDocument(string guid, string subject, string action = "send", List<Structs.Recipient> roles = null, List<Structs.MergeField> mergeFields = null, Dictionary<string, string> tags = null, int? expires_in = null, string description = null, string callbackURL = null);

    }
}
