jQuery.noConflict();

jQuery(document).ready(function ()
{
    //solr
    var solrConfig = ".solr-config";
    var solrConfigForm = solrConfig + " .form";
    var solrTestFormSubmit = solrConfigForm + " .solr-test";
    var solrTestSuccess = solrConfig + " .test-success";
    var solrTestFailure = solrConfig + " .test-failure";
    var solrConfigFormSubmit = solrConfigForm + " .solr-submit";
    var solrSubmitSuccess = solrConfig + " .submit-success";
    var solrSubmitFailure = solrConfig + " .submit-failure";

    //site
    var siteConfig = ".site-config";
    var siteConfigForm = siteConfig + " .form";
    var siteConfigFormSubmit = siteConfigForm + " .site-submit";
    var siteSubmitSuccess = siteConfig + " .submit-success";
    var siteSubmitFailure = siteConfig + " .submit-failure";

    //crawler
    var crawlerConfig = ".crawler-config";
    var crawlerConfigForm = crawlerConfig + " .form";
    var crawlerConfigFormSubmit = crawlerConfigForm + " .crawler-submit";
    var crawlerSubmitSuccess = crawlerConfig + " .submit-success";
    var crawlerSubmitFailure = crawlerConfig + " .submit-failure";

    jQuery(solrTestFormSubmit).click(function (e)
    {
        e.preventDefault();

        ResetConfigForms();
        TestSolr();
    });

    function TestSolr()
    {
        var solrUrlValue = jQuery(solrConfigForm + " .solr-url").val();
        var solrCoreValue = jQuery(solrConfigForm + " .solr-core").val();

        jQuery(progressIndicator).show();
        
        jQuery.post(
            jQuery(solrConfigForm).attr("test"),
            {
                SolrUrl: solrUrlValue,
                SolrCore: solrCoreValue
            }
        ).done(function (r)
        {
            jQuery(progressIndicator).hide();

            if (r.Succeeded)
            {
                jQuery(solrTestSuccess).show();
            }
            else {
                jQuery(solrTestFailure).show();
            }
        });
    }

    jQuery(solrConfigFormSubmit).click(function (e)
    {
        e.preventDefault();

        ResetConfigForms();
        CreateSolrConfig();
    });

    function CreateSolrConfig()
    {
        var solrUrlValue = jQuery(solrConfigForm + " .solr-url").val();
        var solrCoreValue = jQuery(solrConfigForm + " .solr-core").val();

        jQuery(progressIndicator).show();
        
        jQuery.post(
            jQuery(solrConfigForm).attr("action"),
            {
                SolrUrl: solrUrlValue,
                SolrCore: solrCoreValue
            }
        ).done(function (r)
        {
            jQuery(progressIndicator).hide();
           
            if (r.Succeeded)
            {
                jQuery(solrConfigForm + " .solr-url").val("");
                jQuery(solrConfigForm + " .solr-core").val("");
                jQuery(solrSubmitSuccess).show();
            }
            else
            {
                jQuery(solrSubmitFailure).show();
            }
        });
    }

    jQuery(siteConfigFormSubmit).click(function (e)
    {
        e.preventDefault();

        ResetConfigForms();
        CreateSiteConfig();
    });

    function CreateSiteConfig()
    {
        var siteUrlValue = jQuery(siteConfigForm + " .site-url").val();
        var parserValue = jQuery(siteConfigForm + " .parser").val();

        jQuery(progressIndicator).show();
        
        jQuery.post(
            jQuery(siteConfigForm).attr("action"),
            {
                SiteUrl: siteUrlValue,
                Parser: parserValue
            }
        ).done(function (r)
        {
            jQuery(progressIndicator).hide();

            if (r.Succeeded)
            {
                jQuery(siteConfigForm + " .site-url").val("");
                jQuery(siteSubmitSuccess).show();
            }
            else {
                jQuery(siteSubmitFailure).show();
            }
        });
    }

    jQuery(crawlerConfigFormSubmit).click(function (e)
    {
        e.preventDefault();

        ResetConfigForms();
        CreateCrawlerConfig();
    });

    function CreateCrawlerConfig()
    {
        var crawlerNameValue = jQuery(crawlerConfigForm + " .crawler-name").val();
        var solrConnectionValue = jQuery(crawlerConfigForm + " .solr-connection").val();
        var sitesValue = [];
        jQuery(crawlerConfigForm + " .sites input[type=checkbox]").each(function ()
        {
            if (jQuery(this).is(":checked"))
                sitesValue.push(jQuery(this).val());
        });
        
        jQuery(progressIndicator).show();

        jQuery.post(
            jQuery(crawlerConfigForm).attr("action"),
            {
                CrawlerName: crawlerNameValue,
                SolrConnection: solrConnectionValue,
                Sites: sitesValue
            }
        ).done(function (r)
        {
            jQuery(progressIndicator).hide();

            if (r.Succeeded)
            {
                jQuery(crawlerConfigForm + " .site-url").val("");
                jQuery(crawlerSubmitSuccess).show();
            }
            else {
                jQuery(crawlerSubmitFailure).show();
            }
        });
    }

    function ResetConfigForms()
    {
        jQuery(solrTestSuccess).hide();
        jQuery(solrTestFailure).hide();

        jQuery(solrSubmitSuccess).hide();
        jQuery(solrSubmitFailure).hide();

        jQuery(siteSubmitSuccess).hide();
        jQuery(siteSubmitFailure).hide();

        jQuery(crawlerSubmitSuccess).hide();
        jQuery(crawlerSubmitFailure).hide();
    }
});