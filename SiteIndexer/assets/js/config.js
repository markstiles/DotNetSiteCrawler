jQuery.noConflict();

jQuery(document).ready(function ()
{
    //form
    var configForm = ".configurations .form";
    var configFormSubmit = configForm + " .config-submit";
    var progressIndicator = ".progress-indicator";
    var submitSuccess = ".submit-success";
    var submitFailure = ".submit-failure";

    jQuery(configFormSubmit).click(function (e)
    {
        e.preventDefault();

        CreateConfig();
    });

    function CreateConfig()
    {
        var projectNameValue = jQuery(configForm + " .project-name").val();
        var jiraUrlValue = jQuery(configForm + " .jira-url").val();
        var jiraProjectCodeValue = jQuery(configForm + " .jira-project-code").val();
        var mavenlinkWorkspaceIdValue = jQuery(configForm + " .mavenlink-workspace-id").val();

        jQuery(progressIndicator).show();
        jQuery(submitSuccess).hide();
        jQuery(submitFailure).hide();

        jQuery.post(
            jQuery(configForm).attr("action"),
            {
                ProjectName: projectNameValue,
                JiraUrl: jiraUrlValue,
                JiraProjectCode: jiraProjectCodeValue,
                MavenlinkWorkspaceId: mavenlinkWorkspaceIdValue
            }
        ).done(function (r)
        {
            jQuery(configForm + " .project-name").val("");
            jQuery(configForm + " .jira-project-code").val("");
            jQuery(configForm + " .mavenlink-workspace-id").val("");
            jQuery(progressIndicator).hide();
           
            if (r.Succeeded)
            {
                jQuery(submitSuccess).show();
            }
            else
            {
                jQuery(submitFailure).show();
            }
        });
    }
});