jQuery.noConflict();

jQuery(document).ready(function ()
{
    //search form
    var searchForm = ".searching .search-form";
    var searchFormSubmit = searchForm + " .submit";
    var searchResults = ".search-results";
    var searchFormFailure = searchForm + " .search-failure";

    jQuery(searchFormSubmit).click(function (e)
    {
        e.preventDefault();

        ResetSearchForms();
        SearchIndex();
    });

    function SearchIndex()
    {
        var connectionIdValue = jQuery(searchForm + " .connection").val();
        var queryValue = jQuery(searchForm + " .query").val();

        jQuery(progressIndicator).show();
        
        jQuery.post(jQuery(searchForm).attr("action"),
            {
                connectionId: connectionIdValue,
                query: queryValue
            })
            .done(function (r)
            {
                jQuery(progressIndicator).hide();
                if (r.Succeeded)
                {
                    for (let i = 0; i < r.ReturnValue.length; i++)
                    {
                        var result = r.ReturnValue[i];
                        
                        var url = "";
                        try {
                            url = result.url[0];
                        }
                        catch (e) {
                            url = result.Url;
                        }

                        var title = "";
                        try {
                            title = result.title[0];
                        }
                        catch (e) {
                            title = result.Title;
                        }

                        var content = "";
                        try {
                            content = result.content[0];
                        }
                        catch (e) {
                            content = result.Content;
                        }
                        
                        var output = "<div class='result'>";
                        output += "<div class='title'><a href='" + url + "' target='_blank'>" + title + "</a></div>";
                        output += "<div class='description'>" + content.slice(0, 300) + "...</div>";
                        output += "<div class='url'><a href='" + url + "' target='_blank'>" + url + "</a></div>";
                        output += "</div>";
                        jQuery(searchResults).append(output);
                    }                    
                }
                else
                {
                    jQuery(searchFormFailure).show();
                }
            });
    }

    function ResetSearchForms()
    {
        jQuery(searchResults).html("");
    }
});