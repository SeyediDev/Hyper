/*Xhanguler to Fix chrome's b*u*g about BIDIrectional texts including parentheses*/

$(function () {
    $(document).on('DOMNodeInserted', /*'.teta-bidi',*/ function () {
        var $bidiElements = $(this).find('.teta-bidi:not(.teta-bidified)');
        if (!$bidiElements.length)
            return;        

        $bidiElements.each(function () {            
            var bidirectionalText = $(this).html();                         
            var correctedText = bidirectionalText.replace(/\)/g, "&rlm;)");            
            $(this).addClass('teta-bidified');
            $(this).html(correctedText);
        });                           
    });    
});