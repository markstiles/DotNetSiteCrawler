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

        SearchIndex();
    });

    function SearchIndex()
    {
        var solrConnectionIdValue = jQuery(searchForm + " .solr-connection").val();
        var queryValue = jQuery(searchForm + " .query").val();

        jQuery(progressIndicator).show();
        jQuery(searchResults).html("");

        jQuery.post(jQuery(searchForm).attr("action"),
            {
                solrConnectionId: solrConnectionIdValue,
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
                        var output = "<div class='result'>";
                        output += "<div class='title'><a href='" + result.url[0] + "' target='_blank'>" + result.title[0] + "</a></div>";
                        output += "<div class='description'>" + result.content[0].slice(0, 300) + "...</div>";
                        output += "<div class='url'><a href='" + result.url[0] + "' target='_blank'>" + result.url[0] + "</a></div>";
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
});