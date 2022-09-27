
//global
var progressIndicator = ".progress-indicator";
var contentContainer = ".content-container";

function parseJsonDate(jsonDateString)
{
    return new Date(parseInt(jsonDateString.replace('/Date(', '')));
}