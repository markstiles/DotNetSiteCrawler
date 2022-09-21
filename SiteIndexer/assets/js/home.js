jQuery.noConflict();

jQuery(document).ready(function ()
{
    //form
    var indexingForm = ".indexing .form";
    var indexingFormSubmit = indexingForm + " .submit";
    var indexingProgressIndicator = indexingForm + " .progress-indicator";
    var indexingSubmitSuccess = indexingForm + " .submit-success";
    var indexingSubmitFailure = indexingForm + " .submit-failure";

    jQuery(indexingFormSubmit).click(function (e)
    {
        e.preventDefault();

        StartIndexing();
    });

    function StartIndexing()
    {
        //var projectNameValue = jQuery(indexingForm + " .project-name").val();
        //var jiraUrlValue = jQuery(configForm + " .jira-url").val();
        //var jiraProjectCodeValue = jQuery(configForm + " .jira-project-code").val();
        //var mavenlinkWorkspaceIdValue = jQuery(configForm + " .mavenlink-workspace-id").val();

        jQuery(indexingProgressIndicator).show();
        jQuery(indexingSubmitSuccess).hide();
        jQuery(indexingSubmitFailure).hide();

        jQuery.post(
            jQuery(indexingForm).attr("action"),
            {
                
            }
        ).done(function (r)
        {
            //jQuery(indexingForm + " .project-name").val("");
            //jQuery(indexingForm + " .jira-project-code").val("");
            //jQuery(indexingForm + " .mavenlink-workspace-id").val("");
            jQuery(indexingProgressIndicator).hide();
           
            if (r.Succeeded)
            {
                jQuery(indexingSubmitSuccess).show();
            }
            else
            {
                jQuery(indexingSubmitFailure).show();
            }
        });
    }
});