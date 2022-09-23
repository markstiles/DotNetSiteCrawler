jQuery.noConflict();

jQuery(document).ready(function ()
{
    //form
    var indexingForm = ".indexing .form";
    var indexingFormSubmit = indexingForm + " .submit";
    var indexingProgressIndicator = indexingForm + " .progress-indicator";
    var indexingSubmitSuccess = indexingForm + " .submit-success";
    var indexingSubmitFailure = indexingForm + " .submit-failure";
    var jobMessages = ".job-messages";

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

        jQuery(jobMessages).html("");
        jQuery(indexingProgressIndicator).show();
        jQuery(indexingSubmitSuccess).hide();
        jQuery(indexingSubmitFailure).hide();
                
        jQuery.post(jQuery(indexingForm).attr("action"),{}
        ).done(function (jobResult)
        {
            var lastDate = new Date()
            lastDate.setDate(lastDate.getDate() - 1)

            CheckStatus(jobResult, jQuery(indexingForm).attr("status"), lastDate,
                function (jobStatus)
                {
                    var messages = jobStatus.Messages;
                    for (let i = 0; i < messages.length; i++)
                    {
                        var m = messages[i];
                        jQuery(jobMessages).append("<div class='message'>" + m + "</div>");
                    }
                    jQuery(jobMessages).scrollTop(jQuery(jobMessages).height());
                },
                function (jobStatus)
                {
                    jQuery(".progress-indicator").hide();
                }
            )
        });
    }
});

var internalLastDate;
function CheckStatus(jobResult, statusUrl, lastDate, statusFunction, finishFunction)
{
    var timer = setInterval(function ()
    {
        var useDate = internalLastDate == undefined ? lastDate : internalLastDate;
        jQuery.post(statusUrl, {
            handleName: jobResult.ReturnValue,
            lastDateReceived: useDate.toJSON()
        })
        .done(function (newJobStatus)
        {
            var jobStatus = newJobStatus.JobStatus;
            internalLastDate = parseJsonDate(jobStatus.LastReceived);
            statusFunction(jobStatus);

            if (jobStatus.IsFinished)
            {
                finishFunction(jobStatus);
                clearInterval(timer);
            }   
        });
    }, 1000);
}

function parseJsonDate(jsonDateString) {
    return new Date(parseInt(jsonDateString.replace('/Date(', '')));
}