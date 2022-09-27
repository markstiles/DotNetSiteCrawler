jQuery.noConflict();

jQuery(document).ready(function ()
{
    //form
    var indexingForm = ".indexing .form";
    var indexingFormSubmit = indexingForm + " .submit";
    var jobMessages = ".job-messages";

    //empty index
    var emptyFormSubmit = indexingForm + " .empty-submit";
    var emptyFormSuccess = indexingForm + " .empty-success";
    var emptyFormFailure = indexingForm + " .empty-failure";

    jQuery(indexingFormSubmit).click(function (e)
    {
        e.preventDefault();

        StartIndexing();
    });

    function StartIndexing()
    {
        var crawlerIdValue = jQuery(indexingForm + " .crawler").val();
        jQuery(jobMessages).html("");
        jQuery(progressIndicator).show();
                
        jQuery.post(jQuery(indexingForm).attr("action"),
        {
            CrawlerId: crawlerIdValue
        }
        ).done(function (jobResult)
        {
            jQuery(progressIndicator).hide();
       
            var lastDate = new Date()
            lastDate.setDate(lastDate.getDate() - 1)

            CheckStatus(jobResult, jQuery(indexingForm).attr("status"), lastDate,
                function (jobStatus)
                {
                    var messages = jobStatus.Messages;
                    for (let i = 0; i < messages.length; i++)
                    {
                        var m = messages[i];
                            jQuery(jobMessages).prepend("<div class='message'>" + m + "</div>");
                    }
                },
                function (jobStatus)
                {
                    jQuery(".progress-indicator").hide();
                }
            )
        });
    }

    jQuery(emptyFormSubmit).click(function (e)
    {
        e.preventDefault();

        EmptyIndex();
    });

    function EmptyIndex()
    {
        jQuery(progressIndicator).show();
        jQuery(emptyFormSuccess).hide();
        jQuery(emptyFormFailure).hide();

        jQuery.post(jQuery(indexingForm).attr("empty"), {})
        .done(function (r)
        {
            jQuery(progressIndicator).hide();
            if (r.Succeeded)
            {
                jQuery(emptyFormSuccess).show();
            }
            else {
                jQuery(emptyFormFailure).show();
            }
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