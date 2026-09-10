var $poweredByImg = document.querySelector('.bjs-powered-by img');
$poweredByImg.width = 80;
$poweredByImg.height = 80;
$poweredByImg.src = "";
var $poweredBy = document.querySelector('.bjs-powered-by');
$poweredBy.removeAttribute('href');
$poweredBy.title = "تحقیق و توسعه ارتباط";
var new_element = $poweredBy.cloneNode(true);
$poweredBy.parentNode.replaceChild(new_element, $poweredBy);