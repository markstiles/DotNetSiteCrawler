jQuery.noConflict();

jQuery(document).ready(function ()
{
    //form
    var indexingForm = ".indexing .form";
    var indexingFormSubmit = indexingForm + " .submit";
    var indexingProgressIndicator = ".indexing .progress-indicator";
    var jobMessages = ".job-messages";

    //empty index
    var emptyFormSubmit = indexingForm + " .empty-submit";
    var emptyFormSuccess = indexingForm + " .empty-success";
    var emptyFormFailure = indexingForm + " .empty-failure";

    //search form
    var searchForm = ".indexing .search-form";
    var searchFormSubmit = searchForm + " .submit";
    var searchResults = ".search-results";

    jQuery(indexingFormSubmit).click(function (e)
    {
        e.preventDefault();

        StartIndexing();
    });

    function StartIndexing()
    {
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

    jQuery(emptyFormSubmit).click(function (e)
    {
        e.preventDefault();

        EmptyIndex();
    });

    function EmptyIndex() {
        jQuery(indexingProgressIndicator).show();
        jQuery(emptyFormSuccess).hide();
        jQuery(emptyFormFailure).hide();

        jQuery.post(jQuery(indexingForm).attr("empty"), {})
        .done(function (r)
        {
            jQuery(indexingProgressIndicator).hide();
            if (r.Succeeded)
            {
                jQuery(emptyFormSuccess).show();
            }
            else {
                jQuery(emptyFormFailure).show();
            }
        });
    } 

    jQuery(searchFormSubmit).click(function (e)
    {
        e.preventDefault();

        SearchIndex();
    });

    function SearchIndex()
    {
        var queryValue = jQuery(searchForm + " .query").val();
        jQuery(indexingProgressIndicator).show();
        jQuery(searchResults).html("");

        jQuery.post(jQuery(searchForm).attr("action"),
            {
                query: queryValue
            })
            .done(function (r)
            {
                jQuery(indexingProgressIndicator).hide();
                if (r.Succeeded)
                {
                    for (let i = 0; i < r.ReturnValue.length; i++)
                    {
                        var result = r.ReturnValue[i];
                        var output = "<div class='result'>";
                        output += "<div class='title'>" + result.title[0] + "</div>";
                        output += "<div class='description'>" + result.content[0].slice(0, 100) + "...</div>";
                        output += "<div class='url'><a href='" + result.url[0] + "' target='_blank'>" + result.url[0] + "</a></div>";
                        output += "</div>";
                        jQuery(searchResults).append(output);
                    }                    
                }
                else
                {
                    jQuery(indexingEmptyFailure).show();
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

function parseJsonDate(jsonDateString) {
    return new Date(parseInt(jsonDateString.replace('/Date(', '')));
}