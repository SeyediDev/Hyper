/*! jQuery v3.6.3 | (c) OpenJS Foundation and other contributors | jquery.org/license */
!function(e,t){"use strict";"object"==typeof module&&"object"==typeof module.exports?module.exports=e.document?t(e,!0):function(e){if(!e.document)throw new Error("jQuery requires a window with a document");return t(e)}:t(e)}("undefined"!=typeof window?window:this,function(C,e){"use strict";var t=[],r=Object.getPrototypeOf,s=t.slice,g=t.flat?function(e){return t.flat.call(e)}:function(e){return t.concat.apply([],e)},u=t.push,i=t.indexOf,n={},o=n.toString,y=n.hasOwnProperty,a=y.toString,l=a.call(Object),v={},m=function(e){return"function"==typeof e&&"number"!=typeof e.nodeType&&"function"!=typeof e.item},x=function(e){return null!=e&&e===e.window},S=C.document,c={type:!0,src:!0,nonce:!0,noModule:!0};function b(e,t,n){var r,i,o=(n=n||S).createElement("script");if(o.text=e,t)for(r in c)(i=t[r]||t.getAttribute&&t.getAttribute(r))&&o.setAttribute(r,i);n.head.appendChild(o).parentNode.removeChild(o)}function w(e){return null==e?e+"":"object"==typeof e||"function"==typeof e?n[o.call(e)]||"object":typeof e}var f="3.6.3",E=function(e,t){return new E.fn.init(e,t)};function p(e){var t=!!e&&"length"in e&&e.length,n=w(e);return!m(e)&&!x(e)&&("array"===n||0===t||"number"==typeof t&&0<t&&t-1 in e)}E.fn=E.prototype={jquery:f,constructor:E,length:0,toArray:function(){return s.call(this)},get:function(e){return null==e?s.call(this):e<0?this[e+this.length]:this[e]},pushStack:function(e){var t=E.merge(this.constructor(),e);return t.prevObject=this,t},each:function(e){return E.each(this,e)},map:function(n){return this.pushStack(E.map(this,function(e,t){return n.call(e,t,e)}))},slice:function(){return this.pushStack(s.apply(this,arguments))},first:function(){return this.eq(0)},last:function(){return this.eq(-1)},even:function(){return this.pushStack(E.grep(this,function(e,t){return(t+1)%2}))},odd:function(){return this.pushStack(E.grep(this,function(e,t){return t%2}))},eq:function(e){var t=this.length,n=+e+(e<0?t:0);return this.pushStack(0<=n&&n<t?[this[n]]:[])},end:function(){return this.prevObject||this.constructor()},push:u,sort:t.sort,splice:t.splice},E.extend=E.fn.extend=function(){var e,t,n,r,i,o,a=arguments[0]||{},s=1,u=arguments.length,l=!1;for("boolean"==typeof a&&(l=a,a=arguments[s]||{},s++),"object"==typeof a||m(a)||(a={}),s===u&&(a=this,s--);s<u;s++)if(null!=(e=arguments[s]))for(t in e)r=e[t],"__proto__"!==t&&a!==r&&(l&&r&&(E.isPlainObject(r)||(i=Array.isArray(r)))?(n=a[t],o=i&&!Array.isArray(n)?[]:i||E.isPlainObject(n)?n:{},i=!1,a[t]=E.extend(l,o,r)):void 0!==r&&(a[t]=r));return a},E.extend({expando:"jQuery"+(f+Math.random()).replace(/\D/g,""),isReady:!0,error:function(e){throw new Error(e)},noop:function(){},isPlainObject:function(e){var t,n;return!(!e||"[object Object]"!==o.call(e))&&(!(t=r(e))||"function"==typeof(n=y.call(t,"constructor")&&t.constructor)&&a.call(n)===l)},isEmptyObject:function(e){var t;for(t in e)return!1;return!0},globalEval:function(e,t,n){b(e,{nonce:t&&t.nonce},n)},each:function(e,t){var n,r=0;if(p(e)){for(n=e.length;r<n;r++)if(!1===t.call(e[r],r,e[r]))break}else for(r in e)if(!1===t.call(e[r],r,e[r]))break;return e},makeArray:function(e,t){var n=t||[];return null!=e&&(p(Object(e))?E.merge(n,"string"==typeof e?[e]:e):u.call(n,e)),n},inArray:function(e,t,n){return null==t?-1:i.call(t,e,n)},merge:function(e,t){for(var n=+t.length,r=0,i=e.length;r<n;r++)e[i++]=t[r];return e.length=i,e},grep:function(e,t,n){for(var r=[],i=0,o=e.length,a=!n;i<o;i++)!t(e[i],i)!==a&&r.push(e[i]);return r},map:function(e,t,n){var r,i,o=0,a=[];if(p(e))for(r=e.length;o<r;o++)null!=(i=t(e[o],o,n))&&a.push(i);else for(o in e)null!=(i=t(e[o],o,n))&&a.push(i);return g(a)},guid:1,support:v}),"function"==typeof Symbol&&(E.fn[Symbol.iterator]=t[Symbol.iterator]),E.each("Boolean Number String Function Array Date RegExp Object Error Symbol".split(" "),function(e,t){n["[object "+t+"]"]=t.toLowerCase()});var d=function(n){var e,d,b,o,i,h,f,g,w,u,l,T,C,a,S,y,s,c,v,E="sizzle"+1*new Date,p=n.document,k=0,r=0,m=ue(),x=ue(),A=ue(),N=ue(),j=function(e,t){return e===t&&(l=!0),0},D={}.hasOwnProperty,t=[],q=t.pop,L=t.push,H=t.push,O=t.slice,P=function(e,t){for(var n=0,r=e.length;n<r;n++)if(e[n]===t)return n;return-1},R="checked|selected|async|autofocus|autoplay|controls|defer|disabled|hidden|ismap|loop|multiple|open|readonly|required|scoped",M="[\\x20\\t\\r\\n\\f]",I="(?:\\\\[\\da-fA-F]{1,6}"+M+"?|\\\\[^\\r\\n\\f]|[\\w-]|[^\0-\\x7f])+",W="\\["+M+"*("+I+")(?:"+M+"*([*^$|!~]?=)"+M+"*(?:'((?:\\\\.|[^\\\\'])*)'|\"((?:\\\\.|[^\\\\\"])*)\"|("+I+"))|)"+M+"*\\]",F=":("+I+")(?:\\((('((?:\\\\.|[^\\\\'])*)'|\"((?:\\\\.|[^\\\\\"])*)\")|((?:\\\\.|[^\\\\()[\\]]|"+W+")*)|.*)\\)|)",$=new RegExp(M+"+","g"),B=new RegExp("^"+M+"+|((?:^|[^\\\\])(?:\\\\.)*)"+M+"+$","g"),_=new RegExp("^"+M+"*,"+M+"*"),z=new RegExp("^"+M+"*([>+~]|"+M+")"+M+"*"),U=new RegExp(M+"|>"),X=new RegExp(F),V=new RegExp("^"+I+"$"),G={ID:new RegExp("^#("+I+")"),CLASS:new RegExp("^\\.("+I+")"),TAG:new RegExp("^("+I+"|[*])"),ATTR:new RegExp("^"+W),PSEUDO:new RegExp("^"+F),CHILD:new RegExp("^:(only|first|last|nth|nth-last)-(child|of-type)(?:\\("+M+"*(even|odd|(([+-]|)(\\d*)n|)"+M+"*(?:([+-]|)"+M+"*(\\d+)|))"+M+"*\\)|)","i"),bool:new RegExp("^(?:"+R+")$","i"),needsContext:new RegExp("^"+M+"*[>+~]|:(even|odd|eq|gt|lt|nth|first|last)(?:\\("+M+"*((?:-\\d)?\\d*)"+M+"*\\)|)(?=[^-]|$)","i")},Y=/HTML$/i,Q=/^(?:input|select|textarea|button)$/i,J=/^h\d$/i,K=/^[^{]+\{\s*\[native \w/,Z=/^(?:#([\w-]+)|(\w+)|\.([\w-]+))$/,ee=/[+~]/,te=new RegExp("\\\\[\\da-fA-F]{1,6}"+M+"?|\\\\([^\\r\\n\\f])","g"),ne=function(e,t){var n="0x"+e.slice(1)-65536;return t||(n<0?String.fromCharCode(n+65536):String.fromCharCode(n>>10|55296,1023&n|56320))},re=/([\0-\x1f\x7f]|^-?\d)|^-$|[^\0-\x1f\x7f-\uFFFF\w-]/g,ie=function(e,t){return t?"\0"===e?"\ufffd":e.slice(0,-1)+"\\"+e.charCodeAt(e.length-1).toString(16)+" ":"\\"+e},oe=function(){T()},ae=be(function(e){return!0===e.disabled&&"fieldset"===e.nodeName.toLowerCase()},{dir:"parentNode",next:"legend"});try{H.apply(t=O.call(p.childNodes),p.childNodes),t[p.childNodes.length].nodeType}catch(e){H={apply:t.length?function(e,t){L.apply(e,O.call(t))}:function(e,t){var n=e.length,r=0;while(e[n++]=t[r++]);e.length=n-1}}}function se(t,e,n,r){var i,o,a,s,u,l,c,f=e&&e.ownerDocument,p=e?e.nodeType:9;if(n=n||[],"string"!=typeof t||!t||1!==p&&9!==p&&11!==p)return n;if(!r&&(T(e),e=e||C,S)){if(11!==p&&(u=Z.exec(t)))if(i=u[1]){if(9===p){if(!(a=e.getElementById(i)))return n;if(a.id===i)return n.push(a),n}else if(f&&(a=f.getElementById(i))&&v(e,a)&&a.id===i)return n.push(a),n}else{if(u[2])return H.apply(n,e.getElementsByTagName(t)),n;if((i=u[3])&&d.getElementsByClassName&&e.getElementsByClassName)return H.apply(n,e.getElementsByClassName(i)),n}if(d.qsa&&!N[t+" "]&&(!y||!y.test(t))&&(1!==p||"object"!==e.nodeName.toLowerCase())){if(c=t,f=e,1===p&&(U.test(t)||z.test(t))){(f=ee.test(t)&&ve(e.parentNode)||e)===e&&d.scope||((s=e.getAttribute("id"))?s=s.replace(re,ie):e.setAttribute("id",s=E)),o=(l=h(t)).length;while(o--)l[o]=(s?"#"+s:":scope")+" "+xe(l[o]);c=l.join(",")}try{if(d.cssSupportsSelector&&!CSS.supports("selector(:is("+c+"))"))throw new Error;return H.apply(n,f.querySelectorAll(c)),n}catch(e){N(t,!0)}finally{s===E&&e.removeAttribute("id")}}}return g(t.replace(B,"$1"),e,n,r)}function ue(){var r=[];return function e(t,n){return r.push(t+" ")>b.cacheLength&&delete e[r.shift()],e[t+" "]=n}}function le(e){return e[E]=!0,e}function ce(e){var t=C.createElement("fieldset");try{return!!e(t)}catch(e){return!1}finally{t.parentNode&&t.parentNode.removeChild(t),t=null}}function fe(e,t){var n=e.split("|"),r=n.length;while(r--)b.attrHandle[n[r]]=t}function pe(e,t){var n=t&&e,r=n&&1===e.nodeType&&1===t.nodeType&&e.sourceIndex-t.sourceIndex;if(r)return r;if(n)while(n=n.nextSibling)if(n===t)return-1;return e?1:-1}function de(t){return function(e){return"input"===e.nodeName.toLowerCase()&&e.type===t}}function he(n){return function(e){var t=e.nodeName.toLowerCase();return("input"===t||"button"===t)&&e.type===n}}function ge(t){return function(e){return"form"in e?e.parentNode&&!1===e.disabled?"label"in e?"label"in e.parentNode?e.parentNode.disabled===t:e.disabled===t:e.isDisabled===t||e.isDisabled!==!t&&ae(e)===t:e.disabled===t:"label"in e&&e.disabled===t}}function ye(a){return le(function(o){return o=+o,le(function(e,t){var n,r=a([],e.length,o),i=r.length;while(i--)e[n=r[i]]&&(e[n]=!(t[n]=e[n]))})})}function ve(e){return e&&"undefined"!=typeof e.getElementsByTagName&&e}for(e in d=se.support={},i=se.isXML=function(e){var t=e&&e.namespaceURI,n=e&&(e.ownerDocument||e).documentElement;return!Y.test(t||n&&n.nodeName||"HTML")},T=se.setDocument=function(e){var t,n,r=e?e.ownerDocument||e:p;return r!=C&&9===r.nodeType&&r.documentElement&&(a=(C=r).documentElement,S=!i(C),p!=C&&(n=C.defaultView)&&n.top!==n&&(n.addEventListener?n.addEventListener("unload",oe,!1):n.attachEvent&&n.attachEvent("onunload",oe)),d.scope=ce(function(e){return a.appendChild(e).appendChild(C.createElement("div")),"undefined"!=typeof e.querySelectorAll&&!e.querySelectorAll(":scope fieldset div").length}),d.cssSupportsSelector=ce(function(){return CSS.supports("selector(*)")&&C.querySelectorAll(":is(:jqfake)")&&!CSS.supports("selector(:is(*,:jqfake))")}),d.attributes=ce(function(e){return e.className="i",!e.getAttribute("className")}),d.getElementsByTagName=ce(function(e){return e.appendChild(C.createComment("")),!e.getElementsByTagName("*").length}),d.getElementsByClassName=K.test(C.getElementsByClassName),d.getById=ce(function(e){return a.appendChild(e).id=E,!C.getElementsByName||!C.getElementsByName(E).length}),d.getById?(b.filter.ID=function(e){var t=e.replace(te,ne);return function(e){return e.getAttribute("id")===t}},b.find.ID=function(e,t){if("undefined"!=typeof t.getElementById&&S){var n=t.getElementById(e);return n?[n]:[]}}):(b.filter.ID=function(e){var n=e.replace(te,ne);return function(e){var t="undefined"!=typeof e.getAttributeNode&&e.getAttributeNode("id");return t&&t.value===n}},b.find.ID=function(e,t){if("undefined"!=typeof t.getElementById&&S){var n,r,i,o=t.getElementById(e);if(o){if((n=o.getAttributeNode("id"))&&n.value===e)return[o];i=t.getElementsByName(e),r=0;while(o=i[r++])if((n=o.getAttributeNode("id"))&&n.value===e)return[o]}return[]}}),b.find.TAG=d.getElementsByTagName?function(e,t){return"undefined"!=typeof t.getElementsByTagName?t.getElementsByTagName(e):d.qsa?t.querySelectorAll(e):void 0}:function(e,t){var n,r=[],i=0,o=t.getElementsByTagName(e);if("*"===e){while(n=o[i++])1===n.nodeType&&r.push(n);return r}return o},b.find.CLASS=d.getElementsByClassName&&function(e,t){if("undefined"!=typeof t.getElementsByClassName&&S)return t.getElementsByClassName(e)},s=[],y=[],(d.qsa=K.test(C.querySelectorAll))&&(ce(function(e){var t;a.appendChild(e).innerHTML="<a id='"+E+"'></a><select id='"+E+"-\r\\' msallowcapture=''><option selected=''></option></select>",e.querySelectorAll("[msallowcapture^='']").length&&y.push("[*^$]="+M+"*(?:''|\"\")"),e.querySelectorAll("[selected]").length||y.push("\\["+M+"*(?:value|"+R+")"),e.querySelectorAll("[id~="+E+"-]").length||y.push("~="),(t=C.createElement("input")).setAttribute("name",""),e.appendChild(t),e.querySelectorAll("[name='']").length||y.push("\\["+M+"*name"+M+"*="+M+"*(?:''|\"\")"),e.querySelectorAll(":checked").length||y.push(":checked"),e.querySelectorAll("a#"+E+"+*").length||y.push(".#.+[+~]"),e.querySelectorAll("\\\f"),y.push("[\\r\\n\\f]")}),ce(function(e){e.innerHTML="<a href='' disabled='disabled'></a><select disabled='disabled'><option/></select>";var t=C.createElement("input");t.setAttribute("type","hidden"),e.appendChild(t).setAttribute("name","D"),e.querySelectorAll("[name=d]").length&&y.push("name"+M+"*[*^$|!~]?="),2!==e.querySelectorAll(":enabled").length&&y.push(":enabled",":disabled"),a.appendChild(e).disabled=!0,2!==e.querySelectorAll(":disabled").length&&y.push(":enabled",":disabled"),e.querySelectorAll("*,:x"),y.push(",.*:")})),(d.matchesSelector=K.test(c=a.matches||a.webkitMatchesSelector||a.mozMatchesSelector||a.oMatchesSelector||a.msMatchesSelector))&&ce(function(e){d.disconnectedMatch=c.call(e,"*"),c.call(e,"[s!='']:x"),s.push("!=",F)}),d.cssSupportsSelector||y.push(":has"),y=y.length&&new RegExp(y.join("|")),s=s.length&&new RegExp(s.join("|")),t=K.test(a.compareDocumentPosition),v=t||K.test(a.contains)?function(e,t){var n=9===e.nodeType&&e.documentElement||e,r=t&&t.parentNode;return e===r||!(!r||1!==r.nodeType||!(n.contains?n.contains(r):e.compareDocumentPosition&&16&e.compareDocumentPosition(r)))}:function(e,t){if(t)while(t=t.parentNode)if(t===e)return!0;return!1},j=t?function(e,t){if(e===t)return l=!0,0;var n=!e.compareDocumentPosition-!t.compareDocumentPosition;return n||(1&(n=(e.ownerDocument||e)==(t.ownerDocument||t)?e.compareDocumentPosition(t):1)||!d.sortDetached&&t.compareDocumentPosition(e)===n?e==C||e.ownerDocument==p&&v(p,e)?-1:t==C||t.ownerDocument==p&&v(p,t)?1:u?P(u,e)-P(u,t):0:4&n?-1:1)}:function(e,t){if(e===t)return l=!0,0;var n,r=0,i=e.parentNode,o=t.parentNode,a=[e],s=[t];if(!i||!o)return e==C?-1:t==C?1:i?-1:o?1:u?P(u,e)-P(u,t):0;if(i===o)return pe(e,t);n=e;while(n=n.parentNode)a.unshift(n);n=t;while(n=n.parentNode)s.unshift(n);while(a[r]===s[r])r++;return r?pe(a[r],s[r]):a[r]==p?-1:s[r]==p?1:0}),C},se.matches=function(e,t){return se(e,null,null,t)},se.matchesSelector=function(e,t){if(T(e),d.matchesSelector&&S&&!N[t+" "]&&(!s||!s.test(t))&&(!y||!y.test(t)))try{var n=c.call(e,t);if(n||d.disconnectedMatch||e.document&&11!==e.document.nodeType)return n}catch(e){N(t,!0)}return 0<se(t,C,null,[e]).length},se.contains=function(e,t){return(e.ownerDocument||e)!=C&&T(e),v(e,t)},se.attr=function(e,t){(e.ownerDocument||e)!=C&&T(e);var n=b.attrHandle[t.toLowerCase()],r=n&&D.call(b.attrHandle,t.toLowerCase())?n(e,t,!S):void 0;return void 0!==r?r:d.attributes||!S?e.getAttribute(t):(r=e.getAttributeNode(t))&&r.specified?r.value:null},se.escape=function(e){return(e+"").replace(re,ie)},se.error=function(e){throw new Error("Syntax error, unrecognized expression: "+e)},se.uniqueSort=function(e){var t,n=[],r=0,i=0;if(l=!d.detectDuplicates,u=!d.sortStable&&e.slice(0),e.sort(j),l){while(t=e[i++])t===e[i]&&(r=n.push(i));while(r--)e.splice(n[r],1)}return u=null,e},o=se.getText=function(e){var t,n="",r=0,i=e.nodeType;if(i){if(1===i||9===i||11===i){if("string"==typeof e.textContent)return e.textContent;for(e=e.firstChild;e;e=e.nextSibling)n+=o(e)}else if(3===i||4===i)return e.nodeValue}else while(t=e[r++])n+=o(t);return n},(b=se.selectors={cacheLength:50,createPseudo:le,match:G,attrHandle:{},find:{},relative:{">":{dir:"parentNode",first:!0}," ":{dir:"parentNode"},"+":{dir:"previousSibling",first:!0},"~":{dir:"previousSibling"}},preFilter:{ATTR:function(e){return e[1]=e[1].replace(te,ne),e[3]=(e[3]||e[4]||e[5]||"").replace(te,ne),"~="===e[2]&&(e[3]=" "+e[3]+" "),e.slice(0,4)},CHILD:function(e){return e[1]=e[1].toLowerCase(),"nth"===e[1].slice(0,3)?(e[3]||se.error(e[0]),e[4]=+(e[4]?e[5]+(e[6]||1):2*("even"===e[3]||"odd"===e[3])),e[5]=+(e[7]+e[8]||"odd"===e[3])):e[3]&&se.error(e[0]),e},PSEUDO:function(e){var t,n=!e[6]&&e[2];return G.CHILD.test(e[0])?null:(e[3]?e[2]=e[4]||e[5]||"":n&&X.test(n)&&(t=h(n,!0))&&(t=n.indexOf(")",n.length-t)-n.length)&&(e[0]=e[0].slice(0,t),e[2]=n.slice(0,t)),e.slice(0,3))}},filter:{TAG:function(e){var t=e.replace(te,ne).toLowerCase();return"*"===e?function(){return!0}:function(e){return e.nodeName&&e.nodeName.toLowerCase()===t}},CLASS:function(e){var t=m[e+" "];return t||(t=new RegExp("(^|"+M+")"+e+"("+M+"|$)"))&&m(e,function(e){return t.test("string"==typeof e.className&&e.className||"undefined"!=typeof e.getAttribute&&e.getAttribute("class")||"")})},ATTR:function(n,r,i){return function(e){var t=se.attr(e,n);return null==t?"!="===r:!r||(t+="","="===r?t===i:"!="===r?t!==i:"^="===r?i&&0===t.indexOf(i):"*="===r?i&&-1<t.indexOf(i):"$="===r?i&&t.slice(-i.length)===i:"~="===r?-1<(" "+t.replace($," ")+" ").indexOf(i):"|="===r&&(t===i||t.slice(0,i.length+1)===i+"-"))}},CHILD:function(h,e,t,g,y){var v="nth"!==h.slice(0,3),m="last"!==h.slice(-4),x="of-type"===e;return 1===g&&0===y?function(e){return!!e.parentNode}:function(e,t,n){var r,i,o,a,s,u,l=v!==m?"nextSibling":"previousSibling",c=e.parentNode,f=x&&e.nodeName.toLowerCase(),p=!n&&!x,d=!1;if(c){if(v){while(l){a=e;while(a=a[l])if(x?a.nodeName.toLowerCase()===f:1===a.nodeType)return!1;u=l="only"===h&&!u&&"nextSibling"}return!0}if(u=[m?c.firstChild:c.lastChild],m&&p){d=(s=(r=(i=(o=(a=c)[E]||(a[E]={}))[a.uniqueID]||(o[a.uniqueID]={}))[h]||[])[0]===k&&r[1])&&r[2],a=s&&c.childNodes[s];while(a=++s&&a&&a[l]||(d=s=0)||u.pop())if(1===a.nodeType&&++d&&a===e){i[h]=[k,s,d];break}}else if(p&&(d=s=(r=(i=(o=(a=e)[E]||(a[E]={}))[a.uniqueID]||(o[a.uniqueID]={}))[h]||[])[0]===k&&r[1]),!1===d)while(a=++s&&a&&a[l]||(d=s=0)||u.pop())if((x?a.nodeName.toLowerCase()===f:1===a.nodeType)&&++d&&(p&&((i=(o=a[E]||(a[E]={}))[a.uniqueID]||(o[a.uniqueID]={}))[h]=[k,d]),a===e))break;return(d-=y)===g||d%g==0&&0<=d/g}}},PSEUDO:function(e,o){var t,a=b.pseudos[e]||b.setFilters[e.toLowerCase()]||se.error("unsupported pseudo: "+e);return a[E]?a(o):1<a.length?(t=[e,e,"",o],b.setFilters.hasOwnProperty(e.toLowerCase())?le(function(e,t){var n,r=a(e,o),i=r.length;while(i--)e[n=P(e,r[i])]=!(t[n]=r[i])}):function(e){return a(e,0,t)}):a}},pseudos:{not:le(function(e){var r=[],i=[],s=f(e.replace(B,"$1"));return s[E]?le(function(e,t,n,r){var i,o=s(e,null,r,[]),a=e.length;while(a--)(i=o[a])&&(e[a]=!(t[a]=i))}):function(e,t,n){return r[0]=e,s(r,null,n,i),r[0]=null,!i.pop()}}),has:le(function(t){return function(e){return 0<se(t,e).length}}),contains:le(function(t){return t=t.replace(te,ne),function(e){return-1<(e.textContent||o(e)).indexOf(t)}}),lang:le(function(n){return V.test(n||"")||se.error("unsupported lang: "+n),n=n.replace(te,ne).toLowerCase(),function(e){var t;do{if(t=S?e.lang:e.getAttribute("xml:lang")||e.getAttribute("lang"))return(t=t.toLowerCase())===n||0===t.indexOf(n+"-")}while((e=e.parentNode)&&1===e.nodeType);return!1}}),target:function(e){var t=n.location&&n.location.hash;return t&&t.slice(1)===e.id},root:function(e){return e===a},focus:function(e){return e===C.activeElement&&(!C.hasFocus||C.hasFocus())&&!!(e.type||e.href||~e.tabIndex)},enabled:ge(!1),disabled:ge(!0),checked:function(e){var t=e.nodeName.toLowerCase();return"input"===t&&!!e.checked||"option"===t&&!!e.selected},selected:function(e){return e.parentNode&&e.parentNode.selectedIndex,!0===e.selected},empty:function(e){for(e=e.firstChild;e;e=e.nextSibling)if(e.nodeType<6)return!1;return!0},parent:function(e){return!b.pseudos.empty(e)},header:function(e){return J.test(e.nodeName)},input:function(e){return Q.test(e.nodeName)},button:function(e){var t=e.nodeName.toLowerCase();return"input"===t&&"button"===e.type||"button"===t},text:function(e){var t;return"input"===e.nodeName.toLowerCase()&&"text"===e.type&&(null==(t=e.getAttribute("type"))||"text"===t.toLowerCase())},first:ye(function(){return[0]}),last:ye(function(e,t){return[t-1]}),eq:ye(function(e,t,n){return[n<0?n+t:n]}),even:ye(function(e,t){for(var n=0;n<t;n+=2)e.push(n);return e}),odd:ye(function(e,t){for(var n=1;n<t;n+=2)e.push(n);return e}),lt:ye(function(e,t,n){for(var r=n<0?n+t:t<n?t:n;0<=--r;)e.push(r);return e}),gt:ye(function(e,t,n){for(var r=n<0?n+t:n;++r<t;)e.push(r);return e})}}).pseudos.nth=b.pseudos.eq,{radio:!0,checkbox:!0,file:!0,password:!0,image:!0})b.pseudos[e]=de(e);for(e in{submit:!0,reset:!0})b.pseudos[e]=he(e);function me(){}function xe(e){for(var t=0,n=e.length,r="";t<n;t++)r+=e[t].value;return r}function be(s,e,t){var u=e.dir,l=e.next,c=l||u,f=t&&"parentNode"===c,p=r++;return e.first?function(e,t,n){while(e=e[u])if(1===e.nodeType||f)return s(e,t,n);return!1}:function(e,t,n){var r,i,o,a=[k,p];if(n){while(e=e[u])if((1===e.nodeType||f)&&s(e,t,n))return!0}else while(e=e[u])if(1===e.nodeType||f)if(i=(o=e[E]||(e[E]={}))[e.uniqueID]||(o[e.uniqueID]={}),l&&l===e.nodeName.toLowerCase())e=e[u]||e;else{if((r=i[c])&&r[0]===k&&r[1]===p)return a[2]=r[2];if((i[c]=a)[2]=s(e,t,n))return!0}return!1}}function we(i){return 1<i.length?function(e,t,n){var r=i.length;while(r--)if(!i[r](e,t,n))return!1;return!0}:i[0]}function Te(e,t,n,r,i){for(var o,a=[],s=0,u=e.length,l=null!=t;s<u;s++)(o=e[s])&&(n&&!n(o,r,i)||(a.push(o),l&&t.push(s)));return a}function Ce(d,h,g,y,v,e){return y&&!y[E]&&(y=Ce(y)),v&&!v[E]&&(v=Ce(v,e)),le(function(e,t,n,r){var i,o,a,s=[],u=[],l=t.length,c=e||function(e,t,n){for(var r=0,i=t.length;r<i;r++)se(e,t[r],n);return n}(h||"*",n.nodeType?[n]:n,[]),f=!d||!e&&h?c:Te(c,s,d,n,r),p=g?v||(e?d:l||y)?[]:t:f;if(g&&g(f,p,n,r),y){i=Te(p,u),y(i,[],n,r),o=i.length;while(o--)(a=i[o])&&(p[u[o]]=!(f[u[o]]=a))}if(e){if(v||d){if(v){i=[],o=p.length;while(o--)(a=p[o])&&i.push(f[o]=a);v(null,p=[],i,r)}o=p.length;while(o--)(a=p[o])&&-1<(i=v?P(e,a):s[o])&&(e[i]=!(t[i]=a))}}else p=Te(p===t?p.splice(l,p.length):p),v?v(null,t,p,r):H.apply(t,p)})}function Se(e){for(var i,t,n,r=e.length,o=b.relative[e[0].type],a=o||b.relative[" "],s=o?1:0,u=be(function(e){return e===i},a,!0),l=be(function(e){return-1<P(i,e)},a,!0),c=[function(e,t,n){var r=!o&&(n||t!==w)||((i=t).nodeType?u(e,t,n):l(e,t,n));return i=null,r}];s<r;s++)if(t=b.relative[e[s].type])c=[be(we(c),t)];else{if((t=b.filter[e[s].type].apply(null,e[s].matches))[E]){for(n=++s;n<r;n++)if(b.relative[e[n].type])break;return Ce(1<s&&we(c),1<s&&xe(e.slice(0,s-1).concat({value:" "===e[s-2].type?"*":""})).replace(B,"$1"),t,s<n&&Se(e.slice(s,n)),n<r&&Se(e=e.slice(n)),n<r&&xe(e))}c.push(t)}return we(c)}return me.prototype=b.filters=b.pseudos,b.setFilters=new me,h=se.tokenize=function(e,t){var n,r,i,o,a,s,u,l=x[e+" "];if(l)return t?0:l.slice(0);a=e,s=[],u=b.preFilter;while(a){for(o in n&&!(r=_.exec(a))||(r&&(a=a.slice(r[0].length)||a),s.push(i=[])),n=!1,(r=z.exec(a))&&(n=r.shift(),i.push({value:n,type:r[0].replace(B," ")}),a=a.slice(n.length)),b.filter)!(r=G[o].exec(a))||u[o]&&!(r=u[o](r))||(n=r.shift(),i.push({value:n,type:o,matches:r}),a=a.slice(n.length));if(!n)break}return t?a.length:a?se.error(e):x(e,s).slice(0)},f=se.compile=function(e,t){var n,y,v,m,x,r,i=[],o=[],a=A[e+" "];if(!a){t||(t=h(e)),n=t.length;while(n--)(a=Se(t[n]))[E]?i.push(a):o.push(a);(a=A(e,(y=o,m=0<(v=i).length,x=0<y.length,r=function(e,t,n,r,i){var o,a,s,u=0,l="0",c=e&&[],f=[],p=w,d=e||x&&b.find.TAG("*",i),h=k+=null==p?1:Math.random()||.1,g=d.length;for(i&&(w=t==C||t||i);l!==g&&null!=(o=d[l]);l++){if(x&&o){a=0,t||o.ownerDocument==C||(T(o),n=!S);while(s=y[a++])if(s(o,t||C,n)){r.push(o);break}i&&(k=h)}m&&((o=!s&&o)&&u--,e&&c.push(o))}if(u+=l,m&&l!==u){a=0;while(s=v[a++])s(c,f,t,n);if(e){if(0<u)while(l--)c[l]||f[l]||(f[l]=q.call(r));f=Te(f)}H.apply(r,f),i&&!e&&0<f.length&&1<u+v.length&&se.uniqueSort(r)}return i&&(k=h,w=p),c},m?le(r):r))).selector=e}return a},g=se.select=function(e,t,n,r){var i,o,a,s,u,l="function"==typeof e&&e,c=!r&&h(e=l.selector||e);if(n=n||[],1===c.length){if(2<(o=c[0]=c[0].slice(0)).length&&"ID"===(a=o[0]).type&&9===t.nodeType&&S&&b.relative[o[1].type]){if(!(t=(b.find.ID(a.matches[0].replace(te,ne),t)||[])[0]))return n;l&&(t=t.parentNode),e=e.slice(o.shift().value.length)}i=G.needsContext.test(e)?0:o.length;while(i--){if(a=o[i],b.relative[s=a.type])break;if((u=b.find[s])&&(r=u(a.matches[0].replace(te,ne),ee.test(o[0].type)&&ve(t.parentNode)||t))){if(o.splice(i,1),!(e=r.length&&xe(o)))return H.apply(n,r),n;break}}}return(l||f(e,c))(r,t,!S,n,!t||ee.test(e)&&ve(t.parentNode)||t),n},d.sortStable=E.split("").sort(j).join("")===E,d.detectDuplicates=!!l,T(),d.sortDetached=ce(function(e){return 1&e.compareDocumentPosition(C.createElement("fieldset"))}),ce(function(e){return e.innerHTML="<a href='#'></a>","#"===e.firstChild.getAttribute("href")})||fe("type|href|height|width",function(e,t,n){if(!n)return e.getAttribute(t,"type"===t.toLowerCase()?1:2)}),d.attributes&&ce(function(e){return e.innerHTML="<input/>",e.firstChild.setAttribute("value",""),""===e.firstChild.getAttribute("value")})||fe("value",function(e,t,n){if(!n&&"input"===e.nodeName.toLowerCase())return e.defaultValue}),ce(function(e){return null==e.getAttribute("disabled")})||fe(R,function(e,t,n){var r;if(!n)return!0===e[t]?t.toLowerCase():(r=e.getAttributeNode(t))&&r.specified?r.value:null}),se}(C);E.find=d,E.expr=d.selectors,E.expr[":"]=E.expr.pseudos,E.uniqueSort=E.unique=d.uniqueSort,E.text=d.getText,E.isXMLDoc=d.isXML,E.contains=d.contains,E.escapeSelector=d.escape;var h=function(e,t,n){var r=[],i=void 0!==n;while((e=e[t])&&9!==e.nodeType)if(1===e.nodeType){if(i&&E(e).is(n))break;r.push(e)}return r},T=function(e,t){for(var n=[];e;e=e.nextSibling)1===e.nodeType&&e!==t&&n.push(e);return n},k=E.expr.match.needsContext;function A(e,t){return e.nodeName&&e.nodeName.toLowerCase()===t.toLowerCase()}var N=/^<([a-z][^\/\0>:\x20\t\r\n\f]*)[\x20\t\r\n\f]*\/?>(?:<\/\1>|)$/i;function j(e,n,r){return m(n)?E.grep(e,function(e,t){return!!n.call(e,t,e)!==r}):n.nodeType?E.grep(e,function(e){return e===n!==r}):"string"!=typeof n?E.grep(e,function(e){return-1<i.call(n,e)!==r}):E.filter(n,e,r)}E.filter=function(e,t,n){var r=t[0];return n&&(e=":not("+e+")"),1===t.length&&1===r.nodeType?E.find.matchesSelector(r,e)?[r]:[]:E.find.matches(e,E.grep(t,function(e){return 1===e.nodeType}))},E.fn.extend({find:function(e){var t,n,r=this.length,i=this;if("string"!=typeof e)return this.pushStack(E(e).filter(function(){for(t=0;t<r;t++)if(E.contains(i[t],this))return!0}));for(n=this.pushStack([]),t=0;t<r;t++)E.find(e,i[t],n);return 1<r?E.uniqueSort(n):n},filter:function(e){return this.pushStack(j(this,e||[],!1))},not:function(e){return this.pushStack(j(this,e||[],!0))},is:function(e){return!!j(this,"string"==typeof e&&k.test(e)?E(e):e||[],!1).length}});var D,q=/^(?:\s*(<[\w\W]+>)[^>]*|#([\w-]+))$/;(E.fn.init=function(e,t,n){var r,i;if(!e)return this;if(n=n||D,"string"==typeof e){if(!(r="<"===e[0]&&">"===e[e.length-1]&&3<=e.length?[null,e,null]:q.exec(e))||!r[1]&&t)return!t||t.jquery?(t||n).find(e):this.constructor(t).find(e);if(r[1]){if(t=t instanceof E?t[0]:t,E.merge(this,E.parseHTML(r[1],t&&t.nodeType?t.ownerDocument||t:S,!0)),N.test(r[1])&&E.isPlainObject(t))for(r in t)m(this[r])?this[r](t[r]):this.attr(r,t[r]);return this}return(i=S.getElementById(r[2]))&&(this[0]=i,this.length=1),this}return e.nodeType?(this[0]=e,this.length=1,this):m(e)?void 0!==n.ready?n.ready(e):e(E):E.makeArray(e,this)}).prototype=E.fn,D=E(S);var L=/^(?:parents|prev(?:Until|All))/,H={children:!0,contents:!0,next:!0,prev:!0};function O(e,t){while((e=e[t])&&1!==e.nodeType);return e}E.fn.extend({has:function(e){var t=E(e,this),n=t.length;return this.filter(function(){for(var e=0;e<n;e++)if(E.contains(this,t[e]))return!0})},closest:function(e,t){var n,r=0,i=this.length,o=[],a="string"!=typeof e&&E(e);if(!k.test(e))for(;r<i;r++)for(n=this[r];n&&n!==t;n=n.parentNode)if(n.nodeType<11&&(a?-1<a.index(n):1===n.nodeType&&E.find.matchesSelector(n,e))){o.push(n);break}return this.pushStack(1<o.length?E.uniqueSort(o):o)},index:function(e){return e?"string"==typeof e?i.call(E(e),this[0]):i.call(this,e.jquery?e[0]:e):this[0]&&this[0].parentNode?this.first().prevAll().length:-1},add:function(e,t){return this.pushStack(E.uniqueSort(E.merge(this.get(),E(e,t))))},addBack:function(e){return this.add(null==e?this.prevObject:this.prevObject.filter(e))}}),E.each({parent:function(e){var t=e.parentNode;return t&&11!==t.nodeType?t:null},parents:function(e){return h(e,"parentNode")},parentsUntil:function(e,t,n){return h(e,"parentNode",n)},next:function(e){return O(e,"nextSibling")},prev:function(e){return O(e,"previousSibling")},nextAll:function(e){return h(e,"nextSibling")},prevAll:function(e){return h(e,"previousSibling")},nextUntil:function(e,t,n){return h(e,"nextSibling",n)},prevUntil:function(e,t,n){return h(e,"previousSibling",n)},siblings:function(e){return T((e.parentNode||{}).firstChild,e)},children:function(e){return T(e.firstChild)},contents:function(e){return null!=e.contentDocument&&r(e.contentDocument)?e.contentDocument:(A(e,"template")&&(e=e.content||e),E.merge([],e.childNodes))}},function(r,i){E.fn[r]=function(e,t){var n=E.map(this,i,e);return"Until"!==r.slice(-5)&&(t=e),t&&"string"==typeof t&&(n=E.filter(t,n)),1<this.length&&(H[r]||E.uniqueSort(n),L.test(r)&&n.reverse()),this.pushStack(n)}});var P=/[^\x20\t\r\n\f]+/g;function R(e){return e}function M(e){throw e}function I(e,t,n,r){var i;try{e&&m(i=e.promise)?i.call(e).done(t).fail(n):e&&m(i=e.then)?i.call(e,t,n):t.apply(void 0,[e].slice(r))}catch(e){n.apply(void 0,[e])}}E.Callbacks=function(r){var e,n;r="string"==typeof r?(e=r,n={},E.each(e.match(P)||[],function(e,t){n[t]=!0}),n):E.extend({},r);var i,t,o,a,s=[],u=[],l=-1,c=function(){for(a=a||r.once,o=i=!0;u.length;l=-1){t=u.shift();while(++l<s.length)!1===s[l].apply(t[0],t[1])&&r.stopOnFalse&&(l=s.length,t=!1)}r.memory||(t=!1),i=!1,a&&(s=t?[]:"")},f={add:function(){return s&&(t&&!i&&(l=s.length-1,u.push(t)),function n(e){E.each(e,function(e,t){m(t)?r.unique&&f.has(t)||s.push(t):t&&t.length&&"string"!==w(t)&&n(t)})}(arguments),t&&!i&&c()),this},remove:function(){return E.each(arguments,function(e,t){var n;while(-1<(n=E.inArray(t,s,n)))s.splice(n,1),n<=l&&l--}),this},has:function(e){return e?-1<E.inArray(e,s):0<s.length},empty:function(){return s&&(s=[]),this},disable:function(){return a=u=[],s=t="",this},disabled:function(){return!s},lock:function(){return a=u=[],t||i||(s=t=""),this},locked:function(){return!!a},fireWith:function(e,t){return a||(t=[e,(t=t||[]).slice?t.slice():t],u.push(t),i||c()),this},fire:function(){return f.fireWith(this,arguments),this},fired:function(){return!!o}};return f},E.extend({Deferred:function(e){var o=[["notify","progress",E.Callbacks("memory"),E.Callbacks("memory"),2],["resolve","done",E.Callbacks("once memory"),E.Callbacks("once memory"),0,"resolved"],["reject","fail",E.Callbacks("once memory"),E.Callbacks("once memory"),1,"rejected"]],i="pending",a={state:function(){return i},always:function(){return s.done(arguments).fail(arguments),this},"catch":function(e){return a.then(null,e)},pipe:function(){var i=arguments;return E.Deferred(function(r){E.each(o,function(e,t){var n=m(i[t[4]])&&i[t[4]];s[t[1]](function(){var e=n&&n.apply(this,arguments);e&&m(e.promise)?e.promise().progress(r.notify).done(r.resolve).fail(r.reject):r[t[0]+"With"](this,n?[e]:arguments)})}),i=null}).promise()},then:function(t,n,r){var u=0;function l(i,o,a,s){return function(){var n=this,r=arguments,e=function(){var e,t;if(!(i<u)){if((e=a.apply(n,r))===o.promise())throw new TypeError("Thenable self-resolution");t=e&&("object"==typeof e||"function"==typeof e)&&e.then,m(t)?s?t.call(e,l(u,o,R,s),l(u,o,M,s)):(u++,t.call(e,l(u,o,R,s),l(u,o,M,s),l(u,o,R,o.notifyWith))):(a!==R&&(n=void 0,r=[e]),(s||o.resolveWith)(n,r))}},t=s?e:function(){try{e()}catch(e){E.Deferred.exceptionHook&&E.Deferred.exceptionHook(e,t.stackTrace),u<=i+1&&(a!==M&&(n=void 0,r=[e]),o.rejectWith(n,r))}};i?t():(E.Deferred.getStackHook&&(t.stackTrace=E.Deferred.getStackHook()),C.setTimeout(t))}}return E.Deferred(function(e){o[0][3].add(l(0,e,m(r)?r:R,e.notifyWith)),o[1][3].add(l(0,e,m(t)?t:R)),o[2][3].add(l(0,e,m(n)?n:M))}).promise()},promise:function(e){return null!=e?E.extend(e,a):a}},s={};return E.each(o,function(e,t){var n=t[2],r=t[5];a[t[1]]=n.add,r&&n.add(function(){i=r},o[3-e][2].disable,o[3-e][3].disable,o[0][2].lock,o[0][3].lock),n.add(t[3].fire),s[t[0]]=function(){return s[t[0]+"With"](this===s?void 0:this,arguments),this},s[t[0]+"With"]=n.fireWith}),a.promise(s),e&&e.call(s,s),s},when:function(e){var n=arguments.length,t=n,r=Array(t),i=s.call(arguments),o=E.Deferred(),a=function(t){return function(e){r[t]=this,i[t]=1<arguments.length?s.call(arguments):e,--n||o.resolveWith(r,i)}};if(n<=1&&(I(e,o.done(a(t)).resolve,o.reject,!n),"pending"===o.state()||m(i[t]&&i[t].then)))return o.then();while(t--)I(i[t],a(t),o.reject);return o.promise()}});var W=/^(Eval|Internal|Range|Reference|Syntax|Type|URI)Error$/;E.Deferred.exceptionHook=function(e,t){C.console&&C.console.warn&&e&&W.test(e.name)&&C.console.warn("jQuery.Deferred exception: "+e.message,e.stack,t)},E.readyException=function(e){C.setTimeout(function(){throw e})};var F=E.Deferred();function $(){S.removeEventListener("DOMContentLoaded",$),C.removeEventListener("load",$),E.ready()}E.fn.ready=function(e){return F.then(e)["catch"](function(e){E.readyException(e)}),this},E.extend({isReady:!1,readyWait:1,ready:function(e){(!0===e?--E.readyWait:E.isReady)||(E.isReady=!0)!==e&&0<--E.readyWait||F.resolveWith(S,[E])}}),E.ready.then=F.then,"complete"===S.readyState||"loading"!==S.readyState&&!S.documentElement.doScroll?C.setTimeout(E.ready):(S.addEventListener("DOMContentLoaded",$),C.addEventListener("load",$));var B=function(e,t,n,r,i,o,a){var s=0,u=e.length,l=null==n;if("object"===w(n))for(s in i=!0,n)B(e,t,s,n[s],!0,o,a);else if(void 0!==r&&(i=!0,m(r)||(a=!0),l&&(a?(t.call(e,r),t=null):(l=t,t=function(e,t,n){return l.call(E(e),n)})),t))for(;s<u;s++)t(e[s],n,a?r:r.call(e[s],s,t(e[s],n)));return i?e:l?t.call(e):u?t(e[0],n):o},_=/^-ms-/,z=/-([a-z])/g;function U(e,t){return t.toUpperCase()}function X(e){return e.replace(_,"ms-").replace(z,U)}var V=function(e){return 1===e.nodeType||9===e.nodeType||!+e.nodeType};function G(){this.expando=E.expando+G.uid++}G.uid=1,G.prototype={cache:function(e){var t=e[this.expando];return t||(t={},V(e)&&(e.nodeType?e[this.expando]=t:Object.defineProperty(e,this.expando,{value:t,configurable:!0}))),t},set:function(e,t,n){var r,i=this.cache(e);if("string"==typeof t)i[X(t)]=n;else for(r in t)i[X(r)]=t[r];return i},get:function(e,t){return void 0===t?this.cache(e):e[this.expando]&&e[this.expando][X(t)]},access:function(e,t,n){return void 0===t||t&&"string"==typeof t&&void 0===n?this.get(e,t):(this.set(e,t,n),void 0!==n?n:t)},remove:function(e,t){var n,r=e[this.expando];if(void 0!==r){if(void 0!==t){n=(t=Array.isArray(t)?t.map(X):(t=X(t))in r?[t]:t.match(P)||[]).length;while(n--)delete r[t[n]]}(void 0===t||E.isEmptyObject(r))&&(e.nodeType?e[this.expando]=void 0:delete e[this.expando])}},hasData:function(e){var t=e[this.expando];return void 0!==t&&!E.isEmptyObject(t)}};var Y=new G,Q=new G,J=/^(?:\{[\w\W]*\}|\[[\w\W]*\])$/,K=/[A-Z]/g;function Z(e,t,n){var r,i;if(void 0===n&&1===e.nodeType)if(r="data-"+t.replace(K,"-$&").toLowerCase(),"string"==typeof(n=e.getAttribute(r))){try{n="true"===(i=n)||"false"!==i&&("null"===i?null:i===+i+""?+i:J.test(i)?JSON.parse(i):i)}catch(e){}Q.set(e,t,n)}else n=void 0;return n}E.extend({hasData:function(e){return Q.hasData(e)||Y.hasData(e)},data:function(e,t,n){return Q.access(e,t,n)},removeData:function(e,t){Q.remove(e,t)},_data:function(e,t,n){return Y.access(e,t,n)},_removeData:function(e,t){Y.remove(e,t)}}),E.fn.extend({data:function(n,e){var t,r,i,o=this[0],a=o&&o.attributes;if(void 0===n){if(this.length&&(i=Q.get(o),1===o.nodeType&&!Y.get(o,"hasDataAttrs"))){t=a.length;while(t--)a[t]&&0===(r=a[t].name).indexOf("data-")&&(r=X(r.slice(5)),Z(o,r,i[r]));Y.set(o,"hasDataAttrs",!0)}return i}return"object"==typeof n?this.each(function(){Q.set(this,n)}):B(this,function(e){var t;if(o&&void 0===e)return void 0!==(t=Q.get(o,n))?t:void 0!==(t=Z(o,n))?t:void 0;this.each(function(){Q.set(this,n,e)})},null,e,1<arguments.length,null,!0)},removeData:function(e){return this.each(function(){Q.remove(this,e)})}}),E.extend({queue:function(e,t,n){var r;if(e)return t=(t||"fx")+"queue",r=Y.get(e,t),n&&(!r||Array.isArray(n)?r=Y.access(e,t,E.makeArray(n)):r.push(n)),r||[]},dequeue:function(e,t){t=t||"fx";var n=E.queue(e,t),r=n.length,i=n.shift(),o=E._queueHooks(e,t);"inprogress"===i&&(i=n.shift(),r--),i&&("fx"===t&&n.unshift("inprogress"),delete o.stop,i.call(e,function(){E.dequeue(e,t)},o)),!r&&o&&o.empty.fire()},_queueHooks:function(e,t){var n=t+"queueHooks";return Y.get(e,n)||Y.access(e,n,{empty:E.Callbacks("once memory").add(function(){Y.remove(e,[t+"queue",n])})})}}),E.fn.extend({queue:function(t,n){var e=2;return"string"!=typeof t&&(n=t,t="fx",e--),arguments.length<e?E.queue(this[0],t):void 0===n?this:this.each(function(){var e=E.queue(this,t,n);E._queueHooks(this,t),"fx"===t&&"inprogress"!==e[0]&&E.dequeue(this,t)})},dequeue:function(e){return this.each(function(){E.dequeue(this,e)})},clearQueue:function(e){return this.queue(e||"fx",[])},promise:function(e,t){var n,r=1,i=E.Deferred(),o=this,a=this.length,s=function(){--r||i.resolveWith(o,[o])};"string"!=typeof e&&(t=e,e=void 0),e=e||"fx";while(a--)(n=Y.get(o[a],e+"queueHooks"))&&n.empty&&(r++,n.empty.add(s));return s(),i.promise(t)}});var ee=/[+-]?(?:\d*\.|)\d+(?:[eE][+-]?\d+|)/.source,te=new RegExp("^(?:([+-])=|)("+ee+")([a-z%]*)$","i"),ne=["Top","Right","Bottom","Left"],re=S.documentElement,ie=function(e){return E.contains(e.ownerDocument,e)},oe={composed:!0};re.getRootNode&&(ie=function(e){return E.contains(e.ownerDocument,e)||e.getRootNode(oe)===e.ownerDocument});var ae=function(e,t){return"none"===(e=t||e).style.display||""===e.style.display&&ie(e)&&"none"===E.css(e,"display")};function se(e,t,n,r){var i,o,a=20,s=r?function(){return r.cur()}:function(){return E.css(e,t,"")},u=s(),l=n&&n[3]||(E.cssNumber[t]?"":"px"),c=e.nodeType&&(E.cssNumber[t]||"px"!==l&&+u)&&te.exec(E.css(e,t));if(c&&c[3]!==l){u/=2,l=l||c[3],c=+u||1;while(a--)E.style(e,t,c+l),(1-o)*(1-(o=s()/u||.5))<=0&&(a=0),c/=o;c*=2,E.style(e,t,c+l),n=n||[]}return n&&(c=+c||+u||0,i=n[1]?c+(n[1]+1)*n[2]:+n[2],r&&(r.unit=l,r.start=c,r.end=i)),i}var ue={};function le(e,t){for(var n,r,i,o,a,s,u,l=[],c=0,f=e.length;c<f;c++)(r=e[c]).style&&(n=r.style.display,t?("none"===n&&(l[c]=Y.get(r,"display")||null,l[c]||(r.style.display="")),""===r.style.display&&ae(r)&&(l[c]=(u=a=o=void 0,a=(i=r).ownerDocument,s=i.nodeName,(u=ue[s])||(o=a.body.appendChild(a.createElement(s)),u=E.css(o,"display"),o.parentNode.removeChild(o),"none"===u&&(u="block"),ue[s]=u)))):"none"!==n&&(l[c]="none",Y.set(r,"display",n)));for(c=0;c<f;c++)null!=l[c]&&(e[c].style.display=l[c]);return e}E.fn.extend({show:function(){return le(this,!0)},hide:function(){return le(this)},toggle:function(e){return"boolean"==typeof e?e?this.show():this.hide():this.each(function(){ae(this)?E(this).show():E(this).hide()})}});var ce,fe,pe=/^(?:checkbox|radio)$/i,de=/<([a-z][^\/\0>\x20\t\r\n\f]*)/i,he=/^$|^module$|\/(?:java|ecma)script/i;ce=S.createDocumentFragment().appendChild(S.createElement("div")),(fe=S.createElement("input")).setAttribute("type","radio"),fe.setAttribute("checked","checked"),fe.setAttribute("name","t"),ce.appendChild(fe),v.checkClone=ce.cloneNode(!0).cloneNode(!0).lastChild.checked,ce.innerHTML="<textarea>x</textarea>",v.noCloneChecked=!!ce.cloneNode(!0).lastChild.defaultValue,ce.innerHTML="<option></option>",v.option=!!ce.lastChild;var ge={thead:[1,"<table>","</table>"],col:[2,"<table><colgroup>","</colgroup></table>"],tr:[2,"<table><tbody>","</tbody></table>"],td:[3,"<table><tbody><tr>","</tr></tbody></table>"],_default:[0,"",""]};function ye(e,t){var n;return n="undefined"!=typeof e.getElementsByTagName?e.getElementsByTagName(t||"*"):"undefined"!=typeof e.querySelectorAll?e.querySelectorAll(t||"*"):[],void 0===t||t&&A(e,t)?E.merge([e],n):n}function ve(e,t){for(var n=0,r=e.length;n<r;n++)Y.set(e[n],"globalEval",!t||Y.get(t[n],"globalEval"))}ge.tbody=ge.tfoot=ge.colgroup=ge.caption=ge.thead,ge.th=ge.td,v.option||(ge.optgroup=ge.option=[1,"<select multiple='multiple'>","</select>"]);var me=/<|&#?\w+;/;function xe(e,t,n,r,i){for(var o,a,s,u,l,c,f=t.createDocumentFragment(),p=[],d=0,h=e.length;d<h;d++)if((o=e[d])||0===o)if("object"===w(o))E.merge(p,o.nodeType?[o]:o);else if(me.test(o)){a=a||f.appendChild(t.createElement("div")),s=(de.exec(o)||["",""])[1].toLowerCase(),u=ge[s]||ge._default,a.innerHTML=u[1]+E.htmlPrefilter(o)+u[2],c=u[0];while(c--)a=a.lastChild;E.merge(p,a.childNodes),(a=f.firstChild).textContent=""}else p.push(t.createTextNode(o));f.textContent="",d=0;while(o=p[d++])if(r&&-1<E.inArray(o,r))i&&i.push(o);else if(l=ie(o),a=ye(f.appendChild(o),"script"),l&&ve(a),n){c=0;while(o=a[c++])he.test(o.type||"")&&n.push(o)}return f}var be=/^([^.]*)(?:\.(.+)|)/;function we(){return!0}function Te(){return!1}function Ce(e,t){return e===function(){try{return S.activeElement}catch(e){}}()==("focus"===t)}function Se(e,t,n,r,i,o){var a,s;if("object"==typeof t){for(s in"string"!=typeof n&&(r=r||n,n=void 0),t)Se(e,s,n,r,t[s],o);return e}if(null==r&&null==i?(i=n,r=n=void 0):null==i&&("string"==typeof n?(i=r,r=void 0):(i=r,r=n,n=void 0)),!1===i)i=Te;else if(!i)return e;return 1===o&&(a=i,(i=function(e){return E().off(e),a.apply(this,arguments)}).guid=a.guid||(a.guid=E.guid++)),e.each(function(){E.event.add(this,t,i,r,n)})}function Ee(e,i,o){o?(Y.set(e,i,!1),E.event.add(e,i,{namespace:!1,handler:function(e){var t,n,r=Y.get(this,i);if(1&e.isTrigger&&this[i]){if(r.length)(E.event.special[i]||{}).delegateType&&e.stopPropagation();else if(r=s.call(arguments),Y.set(this,i,r),t=o(this,i),this[i](),r!==(n=Y.get(this,i))||t?Y.set(this,i,!1):n={},r!==n)return e.stopImmediatePropagation(),e.preventDefault(),n&&n.value}else r.length&&(Y.set(this,i,{value:E.event.trigger(E.extend(r[0],E.Event.prototype),r.slice(1),this)}),e.stopImmediatePropagation())}})):void 0===Y.get(e,i)&&E.event.add(e,i,we)}E.event={global:{},add:function(t,e,n,r,i){var o,a,s,u,l,c,f,p,d,h,g,y=Y.get(t);if(V(t)){n.handler&&(n=(o=n).handler,i=o.selector),i&&E.find.matchesSelector(re,i),n.guid||(n.guid=E.guid++),(u=y.events)||(u=y.events=Object.create(null)),(a=y.handle)||(a=y.handle=function(e){return"undefined"!=typeof E&&E.event.triggered!==e.type?E.event.dispatch.apply(t,arguments):void 0}),l=(e=(e||"").match(P)||[""]).length;while(l--)d=g=(s=be.exec(e[l])||[])[1],h=(s[2]||"").split(".").sort(),d&&(f=E.event.special[d]||{},d=(i?f.delegateType:f.bindType)||d,f=E.event.special[d]||{},c=E.extend({type:d,origType:g,data:r,handler:n,guid:n.guid,selector:i,needsContext:i&&E.expr.match.needsContext.test(i),namespace:h.join(".")},o),(p=u[d])||((p=u[d]=[]).delegateCount=0,f.setup&&!1!==f.setup.call(t,r,h,a)||t.addEventListener&&t.addEventListener(d,a)),f.add&&(f.add.call(t,c),c.handler.guid||(c.handler.guid=n.guid)),i?p.splice(p.delegateCount++,0,c):p.push(c),E.event.global[d]=!0)}},remove:function(e,t,n,r,i){var o,a,s,u,l,c,f,p,d,h,g,y=Y.hasData(e)&&Y.get(e);if(y&&(u=y.events)){l=(t=(t||"").match(P)||[""]).length;while(l--)if(d=g=(s=be.exec(t[l])||[])[1],h=(s[2]||"").split(".").sort(),d){f=E.event.special[d]||{},p=u[d=(r?f.delegateType:f.bindType)||d]||[],s=s[2]&&new RegExp("(^|\\.)"+h.join("\\.(?:.*\\.|)")+"(\\.|$)"),a=o=p.length;while(o--)c=p[o],!i&&g!==c.origType||n&&n.guid!==c.guid||s&&!s.test(c.namespace)||r&&r!==c.selector&&("**"!==r||!c.selector)||(p.splice(o,1),c.selector&&p.delegateCount--,f.remove&&f.remove.call(e,c));a&&!p.length&&(f.teardown&&!1!==f.teardown.call(e,h,y.handle)||E.removeEvent(e,d,y.handle),delete u[d])}else for(d in u)E.event.remove(e,d+t[l],n,r,!0);E.isEmptyObject(u)&&Y.remove(e,"handle events")}},dispatch:function(e){var t,n,r,i,o,a,s=new Array(arguments.length),u=E.event.fix(e),l=(Y.get(this,"events")||Object.create(null))[u.type]||[],c=E.event.special[u.type]||{};for(s[0]=u,t=1;t<arguments.length;t++)s[t]=arguments[t];if(u.delegateTarget=this,!c.preDispatch||!1!==c.preDispatch.call(this,u)){a=E.event.handlers.call(this,u,l),t=0;while((i=a[t++])&&!u.isPropagationStopped()){u.currentTarget=i.elem,n=0;while((o=i.handlers[n++])&&!u.isImmediatePropagationStopped())u.rnamespace&&!1!==o.namespace&&!u.rnamespace.test(o.namespace)||(u.handleObj=o,u.data=o.data,void 0!==(r=((E.event.special[o.origType]||{}).handle||o.handler).apply(i.elem,s))&&!1===(u.result=r)&&(u.preventDefault(),u.stopPropagation()))}return c.postDispatch&&c.postDispatch.call(this,u),u.result}},handlers:function(e,t){var n,r,i,o,a,s=[],u=t.delegateCount,l=e.target;if(u&&l.nodeType&&!("click"===e.type&&1<=e.button))for(;l!==this;l=l.parentNode||this)if(1===l.nodeType&&("click"!==e.type||!0!==l.disabled)){for(o=[],a={},n=0;n<u;n++)void 0===a[i=(r=t[n]).selector+" "]&&(a[i]=r.needsContext?-1<E(i,this).index(l):E.find(i,this,null,[l]).length),a[i]&&o.push(r);o.length&&s.push({elem:l,handlers:o})}return l=this,u<t.length&&s.push({elem:l,handlers:t.slice(u)}),s},addProp:function(t,e){Object.defineProperty(E.Event.prototype,t,{enumerable:!0,configurable:!0,get:m(e)?function(){if(this.originalEvent)return e(this.originalEvent)}:function(){if(this.originalEvent)return this.originalEvent[t]},set:function(e){Object.defineProperty(this,t,{enumerable:!0,configurable:!0,writable:!0,value:e})}})},fix:function(e){return e[E.expando]?e:new E.Event(e)},special:{load:{noBubble:!0},click:{setup:function(e){var t=this||e;return pe.test(t.type)&&t.click&&A(t,"input")&&Ee(t,"click",we),!1},trigger:function(e){var t=this||e;return pe.test(t.type)&&t.click&&A(t,"input")&&Ee(t,"click"),!0},_default:function(e){var t=e.target;return pe.test(t.type)&&t.click&&A(t,"input")&&Y.get(t,"click")||A(t,"a")}},beforeunload:{postDispatch:function(e){void 0!==e.result&&e.originalEvent&&(e.originalEvent.returnValue=e.result)}}}},E.removeEvent=function(e,t,n){e.removeEventListener&&e.removeEventListener(t,n)},E.Event=function(e,t){if(!(this instanceof E.Event))return new E.Event(e,t);e&&e.type?(this.originalEvent=e,this.type=e.type,this.isDefaultPrevented=e.defaultPrevented||void 0===e.defaultPrevented&&!1===e.returnValue?we:Te,this.target=e.target&&3===e.target.nodeType?e.target.parentNode:e.target,this.currentTarget=e.currentTarget,this.relatedTarget=e.relatedTarget):this.type=e,t&&E.extend(this,t),this.timeStamp=e&&e.timeStamp||Date.now(),this[E.expando]=!0},E.Event.prototype={constructor:E.Event,isDefaultPrevented:Te,isPropagationStopped:Te,isImmediatePropagationStopped:Te,isSimulated:!1,preventDefault:function(){var e=this.originalEvent;this.isDefaultPrevented=we,e&&!this.isSimulated&&e.preventDefault()},stopPropagation:function(){var e=this.originalEvent;this.isPropagationStopped=we,e&&!this.isSimulated&&e.stopPropagation()},stopImmediatePropagation:function(){var e=this.originalEvent;this.isImmediatePropagationStopped=we,e&&!this.isSimulated&&e.stopImmediatePropagation(),this.stopPropagation()}},E.each({altKey:!0,bubbles:!0,cancelable:!0,changedTouches:!0,ctrlKey:!0,detail:!0,eventPhase:!0,metaKey:!0,pageX:!0,pageY:!0,shiftKey:!0,view:!0,"char":!0,code:!0,charCode:!0,key:!0,keyCode:!0,button:!0,buttons:!0,clientX:!0,clientY:!0,offsetX:!0,offsetY:!0,pointerId:!0,pointerType:!0,screenX:!0,screenY:!0,targetTouches:!0,toElement:!0,touches:!0,which:!0},E.event.addProp),E.each({focus:"focusin",blur:"focusout"},function(t,e){E.event.special[t]={setup:function(){return Ee(this,t,Ce),!1},trigger:function(){return Ee(this,t),!0},_default:function(e){return Y.get(e.target,t)},delegateType:e}}),E.each({mouseenter:"mouseover",mouseleave:"mouseout",pointerenter:"pointerover",pointerleave:"pointerout"},function(e,i){E.event.special[e]={delegateType:i,bindType:i,handle:function(e){var t,n=e.relatedTarget,r=e.handleObj;return n&&(n===this||E.contains(this,n))||(e.type=r.origType,t=r.handler.apply(this,arguments),e.type=i),t}}}),E.fn.extend({on:function(e,t,n,r){return Se(this,e,t,n,r)},one:function(e,t,n,r){return Se(this,e,t,n,r,1)},off:function(e,t,n){var r,i;if(e&&e.preventDefault&&e.handleObj)return r=e.handleObj,E(e.delegateTarget).off(r.namespace?r.origType+"."+r.namespace:r.origType,r.selector,r.handler),this;if("object"==typeof e){for(i in e)this.off(i,t,e[i]);return this}return!1!==t&&"function"!=typeof t||(n=t,t=void 0),!1===n&&(n=Te),this.each(function(){E.event.remove(this,e,n,t)})}});var ke=/<script|<style|<link/i,Ae=/checked\s*(?:[^=]|=\s*.checked.)/i,Ne=/^\s*<!\[CDATA\[|\]\]>\s*$/g;function je(e,t){return A(e,"table")&&A(11!==t.nodeType?t:t.firstChild,"tr")&&E(e).children("tbody")[0]||e}function De(e){return e.type=(null!==e.getAttribute("type"))+"/"+e.type,e}function qe(e){return"true/"===(e.type||"").slice(0,5)?e.type=e.type.slice(5):e.removeAttribute("type"),e}function Le(e,t){var n,r,i,o,a,s;if(1===t.nodeType){if(Y.hasData(e)&&(s=Y.get(e).events))for(i in Y.remove(t,"handle events"),s)for(n=0,r=s[i].length;n<r;n++)E.event.add(t,i,s[i][n]);Q.hasData(e)&&(o=Q.access(e),a=E.extend({},o),Q.set(t,a))}}function He(n,r,i,o){r=g(r);var e,t,a,s,u,l,c=0,f=n.length,p=f-1,d=r[0],h=m(d);if(h||1<f&&"string"==typeof d&&!v.checkClone&&Ae.test(d))return n.each(function(e){var t=n.eq(e);h&&(r[0]=d.call(this,e,t.html())),He(t,r,i,o)});if(f&&(t=(e=xe(r,n[0].ownerDocument,!1,n,o)).firstChild,1===e.childNodes.length&&(e=t),t||o)){for(s=(a=E.map(ye(e,"script"),De)).length;c<f;c++)u=e,c!==p&&(u=E.clone(u,!0,!0),s&&E.merge(a,ye(u,"script"))),i.call(n[c],u,c);if(s)for(l=a[a.length-1].ownerDocument,E.map(a,qe),c=0;c<s;c++)u=a[c],he.test(u.type||"")&&!Y.access(u,"globalEval")&&E.contains(l,u)&&(u.src&&"module"!==(u.type||"").toLowerCase()?E._evalUrl&&!u.noModule&&E._evalUrl(u.src,{nonce:u.nonce||u.getAttribute("nonce")},l):b(u.textContent.replace(Ne,""),u,l))}return n}function Oe(e,t,n){for(var r,i=t?E.filter(t,e):e,o=0;null!=(r=i[o]);o++)n||1!==r.nodeType||E.cleanData(ye(r)),r.parentNode&&(n&&ie(r)&&ve(ye(r,"script")),r.parentNode.removeChild(r));return e}E.extend({htmlPrefilter:function(e){return e},clone:function(e,t,n){var r,i,o,a,s,u,l,c=e.cloneNode(!0),f=ie(e);if(!(v.noCloneChecked||1!==e.nodeType&&11!==e.nodeType||E.isXMLDoc(e)))for(a=ye(c),r=0,i=(o=ye(e)).length;r<i;r++)s=o[r],u=a[r],void 0,"input"===(l=u.nodeName.toLowerCase())&&pe.test(s.type)?u.checked=s.checked:"input"!==l&&"textarea"!==l||(u.defaultValue=s.defaultValue);if(t)if(n)for(o=o||ye(e),a=a||ye(c),r=0,i=o.length;r<i;r++)Le(o[r],a[r]);else Le(e,c);return 0<(a=ye(c,"script")).length&&ve(a,!f&&ye(e,"script")),c},cleanData:function(e){for(var t,n,r,i=E.event.special,o=0;void 0!==(n=e[o]);o++)if(V(n)){if(t=n[Y.expando]){if(t.events)for(r in t.events)i[r]?E.event.remove(n,r):E.removeEvent(n,r,t.handle);n[Y.expando]=void 0}n[Q.expando]&&(n[Q.expando]=void 0)}}}),E.fn.extend({detach:function(e){return Oe(this,e,!0)},remove:function(e){return Oe(this,e)},text:function(e){return B(this,function(e){return void 0===e?E.text(this):this.empty().each(function(){1!==this.nodeType&&11!==this.nodeType&&9!==this.nodeType||(this.textContent=e)})},null,e,arguments.length)},append:function(){return He(this,arguments,function(e){1!==this.nodeType&&11!==this.nodeType&&9!==this.nodeType||je(this,e).appendChild(e)})},prepend:function(){return He(this,arguments,function(e){if(1===this.nodeType||11===this.nodeType||9===this.nodeType){var t=je(this,e);t.insertBefore(e,t.firstChild)}})},before:function(){return He(this,arguments,function(e){this.parentNode&&this.parentNode.insertBefore(e,this)})},after:function(){return He(this,arguments,function(e){this.parentNode&&this.parentNode.insertBefore(e,this.nextSibling)})},empty:function(){for(var e,t=0;null!=(e=this[t]);t++)1===e.nodeType&&(E.cleanData(ye(e,!1)),e.textContent="");return this},clone:function(e,t){return e=null!=e&&e,t=null==t?e:t,this.map(function(){return E.clone(this,e,t)})},html:function(e){return B(this,function(e){var t=this[0]||{},n=0,r=this.length;if(void 0===e&&1===t.nodeType)return t.innerHTML;if("string"==typeof e&&!ke.test(e)&&!ge[(de.exec(e)||["",""])[1].toLowerCase()]){e=E.htmlPrefilter(e);try{for(;n<r;n++)1===(t=this[n]||{}).nodeType&&(E.cleanData(ye(t,!1)),t.innerHTML=e);t=0}catch(e){}}t&&this.empty().append(e)},null,e,arguments.length)},replaceWith:function(){var n=[];return He(this,arguments,function(e){var t=this.parentNode;E.inArray(this,n)<0&&(E.cleanData(ye(this)),t&&t.replaceChild(e,this))},n)}}),E.each({appendTo:"append",prependTo:"prepend",insertBefore:"before",insertAfter:"after",replaceAll:"replaceWith"},function(e,a){E.fn[e]=function(e){for(var t,n=[],r=E(e),i=r.length-1,o=0;o<=i;o++)t=o===i?this:this.clone(!0),E(r[o])[a](t),u.apply(n,t.get());return this.pushStack(n)}});var Pe=new RegExp("^("+ee+")(?!px)[a-z%]+$","i"),Re=/^--/,Me=function(e){var t=e.ownerDocument.defaultView;return t&&t.opener||(t=C),t.getComputedStyle(e)},Ie=function(e,t,n){var r,i,o={};for(i in t)o[i]=e.style[i],e.style[i]=t[i];for(i in r=n.call(e),t)e.style[i]=o[i];return r},We=new RegExp(ne.join("|"),"i"),Fe="[\\x20\\t\\r\\n\\f]",$e=new RegExp("^"+Fe+"+|((?:^|[^\\\\])(?:\\\\.)*)"+Fe+"+$","g");function Be(e,t,n){var r,i,o,a,s=Re.test(t),u=e.style;return(n=n||Me(e))&&(a=n.getPropertyValue(t)||n[t],s&&a&&(a=a.replace($e,"$1")||void 0),""!==a||ie(e)||(a=E.style(e,t)),!v.HyperelBoxStyles()&&Pe.test(a)&&We.test(t)&&(r=u.width,i=u.minWidth,o=u.maxWidth,u.minWidth=u.maxWidth=u.width=a,a=n.width,u.width=r,u.minWidth=i,u.maxWidth=o)),void 0!==a?a+"":a}function _e(e,t){return{get:function(){if(!e())return(this.get=t).apply(this,arguments);delete this.get}}}!function(){function e(){if(l){u.style.cssText="position:absolute;left:-11111px;width:60px;margin-top:1px;padding:0;border:0",l.style.cssText="position:relative;display:block;box-sizing:border-box;overflow:scroll;margin:auto;border:1px;padding:1px;width:60%;top:1%",re.appendChild(u).appendChild(l);var e=C.getComputedStyle(l);n="1%"!==e.top,s=12===t(e.marginLeft),l.style.right="60%",o=36===t(e.right),r=36===t(e.width),l.style.position="absolute",i=12===t(l.offsetWidth/3),re.removeChild(u),l=null}}function t(e){return Math.round(parseFloat(e))}var n,r,i,o,a,s,u=S.createElement("div"),l=S.createElement("div");l.style&&(l.style.backgroundClip="content-box",l.cloneNode(!0).style.backgroundClip="",v.clearCloneStyle="content-box"===l.style.backgroundClip,E.extend(v,{boxSizingReliable:function(){return e(),r},HyperelBoxStyles:function(){return e(),o},HyperelPosition:function(){return e(),n},reliableMarginLeft:function(){return e(),s},scrollboxSize:function(){return e(),i},reliableTrDimensions:function(){var e,t,n,r;return null==a&&(e=S.createElement("table"),t=S.createElement("tr"),n=S.createElement("div"),e.style.cssText="position:absolute;left:-11111px;border-collapse:separate",t.style.cssText="border:1px solid",t.style.height="1px",n.style.height="9px",n.style.display="block",re.appendChild(e).appendChild(t).appendChild(n),r=C.getComputedStyle(t),a=parseInt(r.height,10)+parseInt(r.borderTopWidth,10)+parseInt(r.borderBottomWidth,10)===t.offsetHeight,re.removeChild(e)),a}}))}();var ze=["Webkit","Moz","ms"],Ue=S.createElement("div").style,Xe={};function Ve(e){var t=E.cssProps[e]||Xe[e];return t||(e in Ue?e:Xe[e]=function(e){var t=e[0].toUpperCase()+e.slice(1),n=ze.length;while(n--)if((e=ze[n]+t)in Ue)return e}(e)||e)}var Ge=/^(none|table(?!-c[ea]).+)/,Ye={position:"absolute",visibility:"hidden",display:"block"},Qe={letterSpacing:"0",fontWeight:"400"};function Je(e,t,n){var r=te.exec(t);return r?Math.max(0,r[2]-(n||0))+(r[3]||"px"):t}function Ke(e,t,n,r,i,o){var a="width"===t?1:0,s=0,u=0;if(n===(r?"border":"content"))return 0;for(;a<4;a+=2)"margin"===n&&(u+=E.css(e,n+ne[a],!0,i)),r?("content"===n&&(u-=E.css(e,"padding"+ne[a],!0,i)),"margin"!==n&&(u-=E.css(e,"border"+ne[a]+"Width",!0,i))):(u+=E.css(e,"padding"+ne[a],!0,i),"padding"!==n?u+=E.css(e,"border"+ne[a]+"Width",!0,i):s+=E.css(e,"border"+ne[a]+"Width",!0,i));return!r&&0<=o&&(u+=Math.max(0,Math.ceil(e["offset"+t[0].toUpperCase()+t.slice(1)]-o-u-s-.5))||0),u}function Ze(e,t,n){var r=Me(e),i=(!v.boxSizingReliable()||n)&&"border-box"===E.css(e,"boxSizing",!1,r),o=i,a=Be(e,t,r),s="offset"+t[0].toUpperCase()+t.slice(1);if(Pe.test(a)){if(!n)return a;a="auto"}return(!v.boxSizingReliable()&&i||!v.reliableTrDimensions()&&A(e,"tr")||"auto"===a||!parseFloat(a)&&"inline"===E.css(e,"display",!1,r))&&e.getClientRects().length&&(i="border-box"===E.css(e,"boxSizing",!1,r),(o=s in e)&&(a=e[s])),(a=parseFloat(a)||0)+Ke(e,t,n||(i?"border":"content"),o,r,a)+"px"}function et(e,t,n,r,i){return new et.prototype.init(e,t,n,r,i)}E.extend({cssHooks:{opacity:{get:function(e,t){if(t){var n=Be(e,"opacity");return""===n?"1":n}}}},cssNumber:{animationIterationCount:!0,columnCount:!0,fillOpacity:!0,flexGrow:!0,flexShrink:!0,fontWeight:!0,gridArea:!0,gridColumn:!0,gridColumnEnd:!0,gridColumnStart:!0,gridRow:!0,gridRowEnd:!0,gridRowStart:!0,lineHeight:!0,opacity:!0,order:!0,orphans:!0,widows:!0,zIndex:!0,zoom:!0},cssProps:{},style:function(e,t,n,r){if(e&&3!==e.nodeType&&8!==e.nodeType&&e.style){var i,o,a,s=X(t),u=Re.test(t),l=e.style;if(u||(t=Ve(s)),a=E.cssHooks[t]||E.cssHooks[s],void 0===n)return a&&"get"in a&&void 0!==(i=a.get(e,!1,r))?i:l[t];"string"===(o=typeof n)&&(i=te.exec(n))&&i[1]&&(n=se(e,t,i),o="number"),null!=n&&n==n&&("number"!==o||u||(n+=i&&i[3]||(E.cssNumber[s]?"":"px")),v.clearCloneStyle||""!==n||0!==t.indexOf("background")||(l[t]="inherit"),a&&"set"in a&&void 0===(n=a.set(e,n,r))||(u?l.setProperty(t,n):l[t]=n))}},css:function(e,t,n,r){var i,o,a,s=X(t);return Re.test(t)||(t=Ve(s)),(a=E.cssHooks[t]||E.cssHooks[s])&&"get"in a&&(i=a.get(e,!0,n)),void 0===i&&(i=Be(e,t,r)),"normal"===i&&t in Qe&&(i=Qe[t]),""===n||n?(o=parseFloat(i),!0===n||isFinite(o)?o||0:i):i}}),E.each(["height","width"],function(e,u){E.cssHooks[u]={get:function(e,t,n){if(t)return!Ge.test(E.css(e,"display"))||e.getClientRects().length&&e.getBoundingClientRect().width?Ze(e,u,n):Ie(e,Ye,function(){return Ze(e,u,n)})},set:function(e,t,n){var r,i=Me(e),o=!v.scrollboxSize()&&"absolute"===i.position,a=(o||n)&&"border-box"===E.css(e,"boxSizing",!1,i),s=n?Ke(e,u,n,a,i):0;return a&&o&&(s-=Math.ceil(e["offset"+u[0].toUpperCase()+u.slice(1)]-parseFloat(i[u])-Ke(e,u,"border",!1,i)-.5)),s&&(r=te.exec(t))&&"px"!==(r[3]||"px")&&(e.style[u]=t,t=E.css(e,u)),Je(0,t,s)}}}),E.cssHooks.marginLeft=_e(v.reliableMarginLeft,function(e,t){if(t)return(parseFloat(Be(e,"marginLeft"))||e.getBoundingClientRect().left-Ie(e,{marginLeft:0},function(){return e.getBoundingClientRect().left}))+"px"}),E.each({margin:"",padding:"",border:"Width"},function(i,o){E.cssHooks[i+o]={expand:function(e){for(var t=0,n={},r="string"==typeof e?e.split(" "):[e];t<4;t++)n[i+ne[t]+o]=r[t]||r[t-2]||r[0];return n}},"margin"!==i&&(E.cssHooks[i+o].set=Je)}),E.fn.extend({css:function(e,t){return B(this,function(e,t,n){var r,i,o={},a=0;if(Array.isArray(t)){for(r=Me(e),i=t.length;a<i;a++)o[t[a]]=E.css(e,t[a],!1,r);return o}return void 0!==n?E.style(e,t,n):E.css(e,t)},e,t,1<arguments.length)}}),((E.Tween=et).prototype={constructor:et,init:function(e,t,n,r,i,o){this.elem=e,this.prop=n,this.easing=i||E.easing._default,this.options=t,this.start=this.now=this.cur(),this.end=r,this.unit=o||(E.cssNumber[n]?"":"px")},cur:function(){var e=et.propHooks[this.prop];return e&&e.get?e.get(this):et.propHooks._default.get(this)},run:function(e){var t,n=et.propHooks[this.prop];return this.options.duration?this.pos=t=E.easing[this.easing](e,this.options.duration*e,0,1,this.options.duration):this.pos=t=e,this.now=(this.end-this.start)*t+this.start,this.options.step&&this.options.step.call(this.elem,this.now,this),n&&n.set?n.set(this):et.propHooks._default.set(this),this}}).init.prototype=et.prototype,(et.propHooks={_default:{get:function(e){var t;return 1!==e.elem.nodeType||null!=e.elem[e.prop]&&null==e.elem.style[e.prop]?e.elem[e.prop]:(t=E.css(e.elem,e.prop,""))&&"auto"!==t?t:0},set:function(e){E.fx.step[e.prop]?E.fx.step[e.prop](e):1!==e.elem.nodeType||!E.cssHooks[e.prop]&&null==e.elem.style[Ve(e.prop)]?e.elem[e.prop]=e.now:E.style(e.elem,e.prop,e.now+e.unit)}}}).scrollTop=et.propHooks.scrollLeft={set:function(e){e.elem.nodeType&&e.elem.parentNode&&(e.elem[e.prop]=e.now)}},E.easing={linear:function(e){return e},swing:function(e){return.5-Math.cos(e*Math.PI)/2},_default:"swing"},E.fx=et.prototype.init,E.fx.step={};var tt,nt,rt,it,ot=/^(?:toggle|show|hide)$/,at=/queueHooks$/;function st(){nt&&(!1===S.hidden&&C.requestAnimationFrame?C.requestAnimationFrame(st):C.setTimeout(st,E.fx.interval),E.fx.tick())}function ut(){return C.setTimeout(function(){tt=void 0}),tt=Date.now()}function lt(e,t){var n,r=0,i={height:e};for(t=t?1:0;r<4;r+=2-t)i["margin"+(n=ne[r])]=i["padding"+n]=e;return t&&(i.opacity=i.width=e),i}function ct(e,t,n){for(var r,i=(ft.tweeners[t]||[]).concat(ft.tweeners["*"]),o=0,a=i.length;o<a;o++)if(r=i[o].call(n,t,e))return r}function ft(o,e,t){var n,a,r=0,i=ft.prefilters.length,s=E.Deferred().always(function(){delete u.elem}),u=function(){if(a)return!1;for(var e=tt||ut(),t=Math.max(0,l.startTime+l.duration-e),n=1-(t/l.duration||0),r=0,i=l.tweens.length;r<i;r++)l.tweens[r].run(n);return s.notifyWith(o,[l,n,t]),n<1&&i?t:(i||s.notifyWith(o,[l,1,0]),s.resolveWith(o,[l]),!1)},l=s.promise({elem:o,props:E.extend({},e),opts:E.extend(!0,{specialEasing:{},easing:E.easing._default},t),originalProperties:e,originalOptions:t,startTime:tt||ut(),duration:t.duration,tweens:[],createTween:function(e,t){var n=E.Tween(o,l.opts,e,t,l.opts.specialEasing[e]||l.opts.easing);return l.tweens.push(n),n},stop:function(e){var t=0,n=e?l.tweens.length:0;if(a)return this;for(a=!0;t<n;t++)l.tweens[t].run(1);return e?(s.notifyWith(o,[l,1,0]),s.resolveWith(o,[l,e])):s.rejectWith(o,[l,e]),this}}),c=l.props;for(!function(e,t){var n,r,i,o,a;for(n in e)if(i=t[r=X(n)],o=e[n],Array.isArray(o)&&(i=o[1],o=e[n]=o[0]),n!==r&&(e[r]=o,delete e[n]),(a=E.cssHooks[r])&&"expand"in a)for(n in o=a.expand(o),delete e[r],o)n in e||(e[n]=o[n],t[n]=i);else t[r]=i}(c,l.opts.specialEasing);r<i;r++)if(n=ft.prefilters[r].call(l,o,c,l.opts))return m(n.stop)&&(E._queueHooks(l.elem,l.opts.queue).stop=n.stop.bind(n)),n;return E.map(c,ct,l),m(l.opts.start)&&l.opts.start.call(o,l),l.progress(l.opts.progress).done(l.opts.done,l.opts.complete).fail(l.opts.fail).always(l.opts.always),E.fx.timer(E.extend(u,{elem:o,anim:l,queue:l.opts.queue})),l}E.Animation=E.extend(ft,{tweeners:{"*":[function(e,t){var n=this.createTween(e,t);return se(n.elem,e,te.exec(t),n),n}]},tweener:function(e,t){m(e)?(t=e,e=["*"]):e=e.match(P);for(var n,r=0,i=e.length;r<i;r++)n=e[r],ft.tweeners[n]=ft.tweeners[n]||[],ft.tweeners[n].unshift(t)},prefilters:[function(e,t,n){var r,i,o,a,s,u,l,c,f="width"in t||"height"in t,p=this,d={},h=e.style,g=e.nodeType&&ae(e),y=Y.get(e,"fxshow");for(r in n.queue||(null==(a=E._queueHooks(e,"fx")).unqueued&&(a.unqueued=0,s=a.empty.fire,a.empty.fire=function(){a.unqueued||s()}),a.unqueued++,p.always(function(){p.always(function(){a.unqueued--,E.queue(e,"fx").length||a.empty.fire()})})),t)if(i=t[r],ot.test(i)){if(delete t[r],o=o||"toggle"===i,i===(g?"hide":"show")){if("show"!==i||!y||void 0===y[r])continue;g=!0}d[r]=y&&y[r]||E.style(e,r)}if((u=!E.isEmptyObject(t))||!E.isEmptyObject(d))for(r in f&&1===e.nodeType&&(n.overflow=[h.overflow,h.overflowX,h.overflowY],null==(l=y&&y.display)&&(l=Y.get(e,"display")),"none"===(c=E.css(e,"display"))&&(l?c=l:(le([e],!0),l=e.style.display||l,c=E.css(e,"display"),le([e]))),("inline"===c||"inline-block"===c&&null!=l)&&"none"===E.css(e,"float")&&(u||(p.done(function(){h.display=l}),null==l&&(c=h.display,l="none"===c?"":c)),h.display="inline-block")),n.overflow&&(h.overflow="hidden",p.always(function(){h.overflow=n.overflow[0],h.overflowX=n.overflow[1],h.overflowY=n.overflow[2]})),u=!1,d)u||(y?"hidden"in y&&(g=y.hidden):y=Y.access(e,"fxshow",{display:l}),o&&(y.hidden=!g),g&&le([e],!0),p.done(function(){for(r in g||le([e]),Y.remove(e,"fxshow"),d)E.style(e,r,d[r])})),u=ct(g?y[r]:0,r,p),r in y||(y[r]=u.start,g&&(u.end=u.start,u.start=0))}],prefilter:function(e,t){t?ft.prefilters.unshift(e):ft.prefilters.push(e)}}),E.speed=function(e,t,n){var r=e&&"object"==typeof e?E.extend({},e):{complete:n||!n&&t||m(e)&&e,duration:e,easing:n&&t||t&&!m(t)&&t};return E.fx.off?r.duration=0:"number"!=typeof r.duration&&(r.duration in E.fx.speeds?r.duration=E.fx.speeds[r.duration]:r.duration=E.fx.speeds._default),null!=r.queue&&!0!==r.queue||(r.queue="fx"),r.old=r.complete,r.complete=function(){m(r.old)&&r.old.call(this),r.queue&&E.dequeue(this,r.queue)},r},E.fn.extend({fadeTo:function(e,t,n,r){return this.filter(ae).css("opacity",0).show().end().animate({opacity:t},e,n,r)},animate:function(t,e,n,r){var i=E.isEmptyObject(t),o=E.speed(e,n,r),a=function(){var e=ft(this,E.extend({},t),o);(i||Y.get(this,"finish"))&&e.stop(!0)};return a.finish=a,i||!1===o.queue?this.each(a):this.queue(o.queue,a)},stop:function(i,e,o){var a=function(e){var t=e.stop;delete e.stop,t(o)};return"string"!=typeof i&&(o=e,e=i,i=void 0),e&&this.queue(i||"fx",[]),this.each(function(){var e=!0,t=null!=i&&i+"queueHooks",n=E.timers,r=Y.get(this);if(t)r[t]&&r[t].stop&&a(r[t]);else for(t in r)r[t]&&r[t].stop&&at.test(t)&&a(r[t]);for(t=n.length;t--;)n[t].elem!==this||null!=i&&n[t].queue!==i||(n[t].anim.stop(o),e=!1,n.splice(t,1));!e&&o||E.dequeue(this,i)})},finish:function(a){return!1!==a&&(a=a||"fx"),this.each(function(){var e,t=Y.get(this),n=t[a+"queue"],r=t[a+"queueHooks"],i=E.timers,o=n?n.length:0;for(t.finish=!0,E.queue(this,a,[]),r&&r.stop&&r.stop.call(this,!0),e=i.length;e--;)i[e].elem===this&&i[e].queue===a&&(i[e].anim.stop(!0),i.splice(e,1));for(e=0;e<o;e++)n[e]&&n[e].finish&&n[e].finish.call(this);delete t.finish})}}),E.each(["toggle","show","hide"],function(e,r){var i=E.fn[r];E.fn[r]=function(e,t,n){return null==e||"boolean"==typeof e?i.apply(this,arguments):this.animate(lt(r,!0),e,t,n)}}),E.each({slideDown:lt("show"),slideUp:lt("hide"),slideToggle:lt("toggle"),fadeIn:{opacity:"show"},fadeOut:{opacity:"hide"},fadeToggle:{opacity:"toggle"}},function(e,r){E.fn[e]=function(e,t,n){return this.animate(r,e,t,n)}}),E.timers=[],E.fx.tick=function(){var e,t=0,n=E.timers;for(tt=Date.now();t<n.length;t++)(e=n[t])()||n[t]!==e||n.splice(t--,1);n.length||E.fx.stop(),tt=void 0},E.fx.timer=function(e){E.timers.push(e),E.fx.start()},E.fx.interval=13,E.fx.start=function(){nt||(nt=!0,st())},E.fx.stop=function(){nt=null},E.fx.speeds={slow:600,fast:200,_default:400},E.fn.delay=function(r,e){return r=E.fx&&E.fx.speeds[r]||r,e=e||"fx",this.queue(e,function(e,t){var n=C.setTimeout(e,r);t.stop=function(){C.clearTimeout(n)}})},rt=S.createElement("input"),it=S.createElement("select").appendChild(S.createElement("option")),rt.type="checkbox",v.checkOn=""!==rt.value,v.optSelected=it.selected,(rt=S.createElement("input")).value="t",rt.type="radio",v.radioValue="t"===rt.value;var pt,dt=E.expr.attrHandle;E.fn.extend({attr:function(e,t){return B(this,E.attr,e,t,1<arguments.length)},removeAttr:function(e){return this.each(function(){E.removeAttr(this,e)})}}),E.extend({attr:function(e,t,n){var r,i,o=e.nodeType;if(3!==o&&8!==o&&2!==o)return"undefined"==typeof e.getAttribute?E.prop(e,t,n):(1===o&&E.isXMLDoc(e)||(i=E.attrHooks[t.toLowerCase()]||(E.expr.match.bool.test(t)?pt:void 0)),void 0!==n?null===n?void E.removeAttr(e,t):i&&"set"in i&&void 0!==(r=i.set(e,n,t))?r:(e.setAttribute(t,n+""),n):i&&"get"in i&&null!==(r=i.get(e,t))?r:null==(r=E.find.attr(e,t))?void 0:r)},attrHooks:{type:{set:function(e,t){if(!v.radioValue&&"radio"===t&&A(e,"input")){var n=e.value;return e.setAttribute("type",t),n&&(e.value=n),t}}}},removeAttr:function(e,t){var n,r=0,i=t&&t.match(P);if(i&&1===e.nodeType)while(n=i[r++])e.removeAttribute(n)}}),pt={set:function(e,t,n){return!1===t?E.removeAttr(e,n):e.setAttribute(n,n),n}},E.each(E.expr.match.bool.source.match(/\w+/g),function(e,t){var a=dt[t]||E.find.attr;dt[t]=function(e,t,n){var r,i,o=t.toLowerCase();return n||(i=dt[o],dt[o]=r,r=null!=a(e,t,n)?o:null,dt[o]=i),r}});var ht=/^(?:input|select|textarea|button)$/i,gt=/^(?:a|area)$/i;function yt(e){return(e.match(P)||[]).join(" ")}function vt(e){return e.getAttribute&&e.getAttribute("class")||""}function mt(e){return Array.isArray(e)?e:"string"==typeof e&&e.match(P)||[]}E.fn.extend({prop:function(e,t){return B(this,E.prop,e,t,1<arguments.length)},removeProp:function(e){return this.each(function(){delete this[E.propFix[e]||e]})}}),E.extend({prop:function(e,t,n){var r,i,o=e.nodeType;if(3!==o&&8!==o&&2!==o)return 1===o&&E.isXMLDoc(e)||(t=E.propFix[t]||t,i=E.propHooks[t]),void 0!==n?i&&"set"in i&&void 0!==(r=i.set(e,n,t))?r:e[t]=n:i&&"get"in i&&null!==(r=i.get(e,t))?r:e[t]},propHooks:{tabIndex:{get:function(e){var t=E.find.attr(e,"tabindex");return t?parseInt(t,10):ht.test(e.nodeName)||gt.test(e.nodeName)&&e.href?0:-1}}},propFix:{"for":"htmlFor","class":"className"}}),v.optSelected||(E.propHooks.selected={get:function(e){var t=e.parentNode;return t&&t.parentNode&&t.parentNode.selectedIndex,null},set:function(e){var t=e.parentNode;t&&(t.selectedIndex,t.parentNode&&t.parentNode.selectedIndex)}}),E.each(["tabIndex","readOnly","maxLength","cellSpacing","cellPadding","rowSpan","colSpan","useMap","frameBorder","contentEditable"],function(){E.propFix[this.toLowerCase()]=this}),E.fn.extend({addClass:function(t){var e,n,r,i,o,a;return m(t)?this.each(function(e){E(this).addClass(t.call(this,e,vt(this)))}):(e=mt(t)).length?this.each(function(){if(r=vt(this),n=1===this.nodeType&&" "+yt(r)+" "){for(o=0;o<e.length;o++)i=e[o],n.indexOf(" "+i+" ")<0&&(n+=i+" ");a=yt(n),r!==a&&this.setAttribute("class",a)}}):this},removeClass:function(t){var e,n,r,i,o,a;return m(t)?this.each(function(e){E(this).removeClass(t.call(this,e,vt(this)))}):arguments.length?(e=mt(t)).length?this.each(function(){if(r=vt(this),n=1===this.nodeType&&" "+yt(r)+" "){for(o=0;o<e.length;o++){i=e[o];while(-1<n.indexOf(" "+i+" "))n=n.replace(" "+i+" "," ")}a=yt(n),r!==a&&this.setAttribute("class",a)}}):this:this.attr("class","")},toggleClass:function(t,n){var e,r,i,o,a=typeof t,s="string"===a||Array.isArray(t);return m(t)?this.each(function(e){E(this).toggleClass(t.call(this,e,vt(this),n),n)}):"boolean"==typeof n&&s?n?this.addClass(t):this.removeClass(t):(e=mt(t),this.each(function(){if(s)for(o=E(this),i=0;i<e.length;i++)r=e[i],o.hasClass(r)?o.removeClass(r):o.addClass(r);else void 0!==t&&"boolean"!==a||((r=vt(this))&&Y.set(this,"__className__",r),this.setAttribute&&this.setAttribute("class",r||!1===t?"":Y.get(this,"__className__")||""))}))},hasClass:function(e){var t,n,r=0;t=" "+e+" ";while(n=this[r++])if(1===n.nodeType&&-1<(" "+yt(vt(n))+" ").indexOf(t))return!0;return!1}});var xt=/\r/g;E.fn.extend({val:function(n){var r,e,i,t=this[0];return arguments.length?(i=m(n),this.each(function(e){var t;1===this.nodeType&&(null==(t=i?n.call(this,e,E(this).val()):n)?t="":"number"==typeof t?t+="":Array.isArray(t)&&(t=E.map(t,function(e){return null==e?"":e+""})),(r=E.valHooks[this.type]||E.valHooks[this.nodeName.toLowerCase()])&&"set"in r&&void 0!==r.set(this,t,"value")||(this.value=t))})):t?(r=E.valHooks[t.type]||E.valHooks[t.nodeName.toLowerCase()])&&"get"in r&&void 0!==(e=r.get(t,"value"))?e:"string"==typeof(e=t.value)?e.replace(xt,""):null==e?"":e:void 0}}),E.extend({valHooks:{option:{get:function(e){var t=E.find.attr(e,"value");return null!=t?t:yt(E.text(e))}},select:{get:function(e){var t,n,r,i=e.options,o=e.selectedIndex,a="select-one"===e.type,s=a?null:[],u=a?o+1:i.length;for(r=o<0?u:a?o:0;r<u;r++)if(((n=i[r]).selected||r===o)&&!n.disabled&&(!n.parentNode.disabled||!A(n.parentNode,"optgroup"))){if(t=E(n).val(),a)return t;s.push(t)}return s},set:function(e,t){var n,r,i=e.options,o=E.makeArray(t),a=i.length;while(a--)((r=i[a]).selected=-1<E.inArray(E.valHooks.option.get(r),o))&&(n=!0);return n||(e.selectedIndex=-1),o}}}}),E.each(["radio","checkbox"],function(){E.valHooks[this]={set:function(e,t){if(Array.isArray(t))return e.checked=-1<E.inArray(E(e).val(),t)}},v.checkOn||(E.valHooks[this].get=function(e){return null===e.getAttribute("value")?"on":e.value})}),v.focusin="onfocusin"in C;var bt=/^(?:focusinfocus|focusoutblur)$/,wt=function(e){e.stopPropagation()};E.extend(E.event,{trigger:function(e,t,n,r){var i,o,a,s,u,l,c,f,p=[n||S],d=y.call(e,"type")?e.type:e,h=y.call(e,"namespace")?e.namespace.split("."):[];if(o=f=a=n=n||S,3!==n.nodeType&&8!==n.nodeType&&!bt.test(d+E.event.triggered)&&(-1<d.indexOf(".")&&(d=(h=d.split(".")).shift(),h.sort()),u=d.indexOf(":")<0&&"on"+d,(e=e[E.expando]?e:new E.Event(d,"object"==typeof e&&e)).isTrigger=r?2:3,e.namespace=h.join("."),e.rnamespace=e.namespace?new RegExp("(^|\\.)"+h.join("\\.(?:.*\\.|)")+"(\\.|$)"):null,e.result=void 0,e.target||(e.target=n),t=null==t?[e]:E.makeArray(t,[e]),c=E.event.special[d]||{},r||!c.trigger||!1!==c.trigger.apply(n,t))){if(!r&&!c.noBubble&&!x(n)){for(s=c.delegateType||d,bt.test(s+d)||(o=o.parentNode);o;o=o.parentNode)p.push(o),a=o;a===(n.ownerDocument||S)&&p.push(a.defaultView||a.parentWindow||C)}i=0;while((o=p[i++])&&!e.isPropagationStopped())f=o,e.type=1<i?s:c.bindType||d,(l=(Y.get(o,"events")||Object.create(null))[e.type]&&Y.get(o,"handle"))&&l.apply(o,t),(l=u&&o[u])&&l.apply&&V(o)&&(e.result=l.apply(o,t),!1===e.result&&e.preventDefault());return e.type=d,r||e.isDefaultPrevented()||c._default&&!1!==c._default.apply(p.pop(),t)||!V(n)||u&&m(n[d])&&!x(n)&&((a=n[u])&&(n[u]=null),E.event.triggered=d,e.isPropagationStopped()&&f.addEventListener(d,wt),n[d](),e.isPropagationStopped()&&f.removeEventListener(d,wt),E.event.triggered=void 0,a&&(n[u]=a)),e.result}},simulate:function(e,t,n){var r=E.extend(new E.Event,n,{type:e,isSimulated:!0});E.event.trigger(r,null,t)}}),E.fn.extend({trigger:function(e,t){return this.each(function(){E.event.trigger(e,t,this)})},triggerHandler:function(e,t){var n=this[0];if(n)return E.event.trigger(e,t,n,!0)}}),v.focusin||E.each({focus:"focusin",blur:"focusout"},function(n,r){var i=function(e){E.event.simulate(r,e.target,E.event.fix(e))};E.event.special[r]={setup:function(){var e=this.ownerDocument||this.document||this,t=Y.access(e,r);t||e.addEventListener(n,i,!0),Y.access(e,r,(t||0)+1)},teardown:function(){var e=this.ownerDocument||this.document||this,t=Y.access(e,r)-1;t?Y.access(e,r,t):(e.removeEventListener(n,i,!0),Y.remove(e,r))}}});var Tt=C.location,Ct={guid:Date.now()},St=/\?/;E.parseXML=function(e){var t,n;if(!e||"string"!=typeof e)return null;try{t=(new C.DOMParser).parseFromString(e,"text/xml")}catch(e){}return n=t&&t.getElementsByTagName("parsererror")[0],t&&!n||E.error("Invalid XML: "+(n?E.map(n.childNodes,function(e){return e.textContent}).join("\n"):e)),t};var Et=/\[\]$/,kt=/\r?\n/g,At=/^(?:submit|button|image|reset|file)$/i,Nt=/^(?:input|select|textarea|keygen)/i;function jt(n,e,r,i){var t;if(Array.isArray(e))E.each(e,function(e,t){r||Et.test(n)?i(n,t):jt(n+"["+("object"==typeof t&&null!=t?e:"")+"]",t,r,i)});else if(r||"object"!==w(e))i(n,e);else for(t in e)jt(n+"["+t+"]",e[t],r,i)}E.param=function(e,t){var n,r=[],i=function(e,t){var n=m(t)?t():t;r[r.length]=encodeURIComponent(e)+"="+encodeURIComponent(null==n?"":n)};if(null==e)return"";if(Array.isArray(e)||e.jquery&&!E.isPlainObject(e))E.each(e,function(){i(this.name,this.value)});else for(n in e)jt(n,e[n],t,i);return r.join("&")},E.fn.extend({serialize:function(){return E.param(this.serializeArray())},serializeArray:function(){return this.map(function(){var e=E.prop(this,"elements");return e?E.makeArray(e):this}).filter(function(){var e=this.type;return this.name&&!E(this).is(":disabled")&&Nt.test(this.nodeName)&&!At.test(e)&&(this.checked||!pe.test(e))}).map(function(e,t){var n=E(this).val();return null==n?null:Array.isArray(n)?E.map(n,function(e){return{name:t.name,value:e.replace(kt,"\r\n")}}):{name:t.name,value:n.replace(kt,"\r\n")}}).get()}});var Dt=/%20/g,qt=/#.*$/,Lt=/([?&])_=[^&]*/,Ht=/^(.*?):[ \t]*([^\r\n]*)$/gm,Ot=/^(?:GET|HEAD)$/,Pt=/^\/\//,Rt={},Mt={},It="*/".concat("*"),Wt=S.createElement("a");function Ft(o){return function(e,t){"string"!=typeof e&&(t=e,e="*");var n,r=0,i=e.toLowerCase().match(P)||[];if(m(t))while(n=i[r++])"+"===n[0]?(n=n.slice(1)||"*",(o[n]=o[n]||[]).unshift(t)):(o[n]=o[n]||[]).push(t)}}function $t(t,i,o,a){var s={},u=t===Mt;function l(e){var r;return s[e]=!0,E.each(t[e]||[],function(e,t){var n=t(i,o,a);return"string"!=typeof n||u||s[n]?u?!(r=n):void 0:(i.dataTypes.unshift(n),l(n),!1)}),r}return l(i.dataTypes[0])||!s["*"]&&l("*")}function Bt(e,t){var n,r,i=E.ajaxSettings.flatOptions||{};for(n in t)void 0!==t[n]&&((i[n]?e:r||(r={}))[n]=t[n]);return r&&E.extend(!0,e,r),e}Wt.href=Tt.href,E.extend({active:0,lastModified:{},etag:{},ajaxSettings:{url:Tt.href,type:"GET",isLocal:/^(?:about|app|app-storage|.+-extension|file|res|widget):$/.test(Tt.protocol),global:!0,processData:!0,async:!0,contentType:"application/x-www-form-urlencoded; charset=UTF-8",accepts:{"*":It,text:"text/plain",html:"text/html",xml:"application/xml, text/xml",json:"application/json, text/javascript"},contents:{xml:/\bxml\b/,html:/\bhtml/,json:/\bjson\b/},responseFields:{xml:"responseXML",text:"responseText",json:"responseJSON"},converters:{"* text":String,"text html":!0,"text json":JSON.parse,"text xml":E.parseXML},flatOptions:{url:!0,context:!0}},ajaxSetup:function(e,t){return t?Bt(Bt(e,E.ajaxSettings),t):Bt(E.ajaxSettings,e)},ajaxPrefilter:Ft(Rt),ajaxTransport:Ft(Mt),ajax:function(e,t){"object"==typeof e&&(t=e,e=void 0),t=t||{};var c,f,p,n,d,r,h,g,i,o,y=E.ajaxSetup({},t),v=y.context||y,m=y.context&&(v.nodeType||v.jquery)?E(v):E.event,x=E.Deferred(),b=E.Callbacks("once memory"),w=y.statusCode||{},a={},s={},u="canceled",T={readyState:0,getResponseHeader:function(e){var t;if(h){if(!n){n={};while(t=Ht.exec(p))n[t[1].toLowerCase()+" "]=(n[t[1].toLowerCase()+" "]||[]).concat(t[2])}t=n[e.toLowerCase()+" "]}return null==t?null:t.join(", ")},getAllResponseHeaders:function(){return h?p:null},setRequestHeader:function(e,t){return null==h&&(e=s[e.toLowerCase()]=s[e.toLowerCase()]||e,a[e]=t),this},overrideMimeType:function(e){return null==h&&(y.mimeType=e),this},statusCode:function(e){var t;if(e)if(h)T.always(e[T.status]);else for(t in e)w[t]=[w[t],e[t]];return this},abort:function(e){var t=e||u;return c&&c.abort(t),l(0,t),this}};if(x.promise(T),y.url=((e||y.url||Tt.href)+"").replace(Pt,Tt.protocol+"//"),y.type=t.method||t.type||y.method||y.type,y.dataTypes=(y.dataType||"*").toLowerCase().match(P)||[""],null==y.crossDomain){r=S.createElement("a");try{r.href=y.url,r.href=r.href,y.crossDomain=Wt.protocol+"//"+Wt.host!=r.protocol+"//"+r.host}catch(e){y.crossDomain=!0}}if(y.data&&y.processData&&"string"!=typeof y.data&&(y.data=E.param(y.data,y.traditional)),$t(Rt,y,t,T),h)return T;for(i in(g=E.event&&y.global)&&0==E.active++&&E.event.trigger("ajaxStart"),y.type=y.type.toUpperCase(),y.hasContent=!Ot.test(y.type),f=y.url.replace(qt,""),y.hasContent?y.data&&y.processData&&0===(y.contentType||"").indexOf("application/x-www-form-urlencoded")&&(y.data=y.data.replace(Dt,"+")):(o=y.url.slice(f.length),y.data&&(y.processData||"string"==typeof y.data)&&(f+=(St.test(f)?"&":"?")+y.data,delete y.data),!1===y.cache&&(f=f.replace(Lt,"$1"),o=(St.test(f)?"&":"?")+"_="+Ct.guid+++o),y.url=f+o),y.ifModified&&(E.lastModified[f]&&T.setRequestHeader("If-Modified-Since",E.lastModified[f]),E.etag[f]&&T.setRequestHeader("If-None-Match",E.etag[f])),(y.data&&y.hasContent&&!1!==y.contentType||t.contentType)&&T.setRequestHeader("Content-Type",y.contentType),T.setRequestHeader("Accept",y.dataTypes[0]&&y.accepts[y.dataTypes[0]]?y.accepts[y.dataTypes[0]]+("*"!==y.dataTypes[0]?", "+It+"; q=0.01":""):y.accepts["*"]),y.headers)T.setRequestHeader(i,y.headers[i]);if(y.beforeSend&&(!1===y.beforeSend.call(v,T,y)||h))return T.abort();if(u="abort",b.add(y.complete),T.done(y.success),T.fail(y.error),c=$t(Mt,y,t,T)){if(T.readyState=1,g&&m.trigger("ajaxSend",[T,y]),h)return T;y.async&&0<y.timeout&&(d=C.setTimeout(function(){T.abort("timeout")},y.timeout));try{h=!1,c.send(a,l)}catch(e){if(h)throw e;l(-1,e)}}else l(-1,"No Transport");function l(e,t,n,r){var i,o,a,s,u,l=t;h||(h=!0,d&&C.clearTimeout(d),c=void 0,p=r||"",T.readyState=0<e?4:0,i=200<=e&&e<300||304===e,n&&(s=function(e,t,n){var r,i,o,a,s=e.contents,u=e.dataTypes;while("*"===u[0])u.shift(),void 0===r&&(r=e.mimeType||t.getResponseHeader("Content-Type"));if(r)for(i in s)if(s[i]&&s[i].test(r)){u.unshift(i);break}if(u[0]in n)o=u[0];else{for(i in n){if(!u[0]||e.converters[i+" "+u[0]]){o=i;break}a||(a=i)}o=o||a}if(o)return o!==u[0]&&u.unshift(o),n[o]}(y,T,n)),!i&&-1<E.inArray("script",y.dataTypes)&&E.inArray("json",y.dataTypes)<0&&(y.converters["text script"]=function(){}),s=function(e,t,n,r){var i,o,a,s,u,l={},c=e.dataTypes.slice();if(c[1])for(a in e.converters)l[a.toLowerCase()]=e.converters[a];o=c.shift();while(o)if(e.responseFields[o]&&(n[e.responseFields[o]]=t),!u&&r&&e.dataFilter&&(t=e.dataFilter(t,e.dataType)),u=o,o=c.shift())if("*"===o)o=u;else if("*"!==u&&u!==o){if(!(a=l[u+" "+o]||l["* "+o]))for(i in l)if((s=i.split(" "))[1]===o&&(a=l[u+" "+s[0]]||l["* "+s[0]])){!0===a?a=l[i]:!0!==l[i]&&(o=s[0],c.unshift(s[1]));break}if(!0!==a)if(a&&e["throws"])t=a(t);else try{t=a(t)}catch(e){return{state:"parsererror",error:a?e:"No conversion from "+u+" to "+o}}}return{state:"success",data:t}}(y,s,T,i),i?(y.ifModified&&((u=T.getResponseHeader("Last-Modified"))&&(E.lastModified[f]=u),(u=T.getResponseHeader("etag"))&&(E.etag[f]=u)),204===e||"HEAD"===y.type?l="nocontent":304===e?l="notmodified":(l=s.state,o=s.data,i=!(a=s.error))):(a=l,!e&&l||(l="error",e<0&&(e=0))),T.status=e,T.statusText=(t||l)+"",i?x.resolveWith(v,[o,l,T]):x.rejectWith(v,[T,l,a]),T.statusCode(w),w=void 0,g&&m.trigger(i?"ajaxSuccess":"ajaxError",[T,y,i?o:a]),b.fireWith(v,[T,l]),g&&(m.trigger("ajaxComplete",[T,y]),--E.active||E.event.trigger("ajaxStop")))}return T},getJSON:function(e,t,n){return E.get(e,t,n,"json")},getScript:function(e,t){return E.get(e,void 0,t,"script")}}),E.each(["get","post"],function(e,i){E[i]=function(e,t,n,r){return m(t)&&(r=r||n,n=t,t=void 0),E.ajax(E.extend({url:e,type:i,dataType:r,data:t,success:n},E.isPlainObject(e)&&e))}}),E.ajaxPrefilter(function(e){var t;for(t in e.headers)"content-type"===t.toLowerCase()&&(e.contentType=e.headers[t]||"")}),E._evalUrl=function(e,t,n){return E.ajax({url:e,type:"GET",dataType:"script",cache:!0,async:!1,global:!1,converters:{"text script":function(){}},dataFilter:function(e){E.globalEval(e,t,n)}})},E.fn.extend({wrapAll:function(e){var t;return this[0]&&(m(e)&&(e=e.call(this[0])),t=E(e,this[0].ownerDocument).eq(0).clone(!0),this[0].parentNode&&t.insertBefore(this[0]),t.map(function(){var e=this;while(e.firstElementChild)e=e.firstElementChild;return e}).append(this)),this},wrapInner:function(n){return m(n)?this.each(function(e){E(this).wrapInner(n.call(this,e))}):this.each(function(){var e=E(this),t=e.contents();t.length?t.wrapAll(n):e.append(n)})},wrap:function(t){var n=m(t);return this.each(function(e){E(this).wrapAll(n?t.call(this,e):t)})},unwrap:function(e){return this.parent(e).not("body").each(function(){E(this).replaceWith(this.childNodes)}),this}}),E.expr.pseudos.hidden=function(e){return!E.expr.pseudos.visible(e)},E.expr.pseudos.visible=function(e){return!!(e.offsetWidth||e.offsetHeight||e.getClientRects().length)},E.ajaxSettings.xhr=function(){try{return new C.XMLHttpRequest}catch(e){}};var _t={0:200,1223:204},zt=E.ajaxSettings.xhr();v.cors=!!zt&&"withCredentials"in zt,v.ajax=zt=!!zt,E.ajaxTransport(function(i){var o,a;if(v.cors||zt&&!i.crossDomain)return{send:function(e,t){var n,r=i.xhr();if(r.open(i.type,i.url,i.async,i.username,i.password),i.xhrFields)for(n in i.xhrFields)r[n]=i.xhrFields[n];for(n in i.mimeType&&r.overrideMimeType&&r.overrideMimeType(i.mimeType),i.crossDomain||e["X-Requested-With"]||(e["X-Requested-With"]="XMLHttpRequest"),e)r.setRequestHeader(n,e[n]);o=function(e){return function(){o&&(o=a=r.onload=r.onerror=r.onabort=r.ontimeout=r.onreadystatechange=null,"abort"===e?r.abort():"error"===e?"number"!=typeof r.status?t(0,"error"):t(r.status,r.statusText):t(_t[r.status]||r.status,r.statusText,"text"!==(r.responseType||"text")||"string"!=typeof r.responseText?{binary:r.response}:{text:r.responseText},r.getAllResponseHeaders()))}},r.onload=o(),a=r.onerror=r.ontimeout=o("error"),void 0!==r.onabort?r.onabort=a:r.onreadystatechange=function(){4===r.readyState&&C.setTimeout(function(){o&&a()})},o=o("abort");try{r.send(i.hasContent&&i.data||null)}catch(e){if(o)throw e}},abort:function(){o&&o()}}}),E.ajaxPrefilter(function(e){e.crossDomain&&(e.contents.script=!1)}),E.ajaxSetup({accepts:{script:"text/javascript, application/javascript, application/ecmascript, application/x-ecmascript"},contents:{script:/\b(?:java|ecma)script\b/},converters:{"text script":function(e){return E.globalEval(e),e}}}),E.ajaxPrefilter("script",function(e){void 0===e.cache&&(e.cache=!1),e.crossDomain&&(e.type="GET")}),E.ajaxTransport("script",function(n){var r,i;if(n.crossDomain||n.scriptAttrs)return{send:function(e,t){r=E("<script>").attr(n.scriptAttrs||{}).prop({charset:n.scriptCharset,src:n.url}).on("load error",i=function(e){r.remove(),i=null,e&&t("error"===e.type?404:200,e.type)}),S.head.appendChild(r[0])},abort:function(){i&&i()}}});var Ut,Xt=[],Vt=/(=)\?(?=&|$)|\?\?/;E.ajaxSetup({jsonp:"callback",jsonpCallback:function(){var e=Xt.pop()||E.expando+"_"+Ct.guid++;return this[e]=!0,e}}),E.ajaxPrefilter("json jsonp",function(e,t,n){var r,i,o,a=!1!==e.jsonp&&(Vt.test(e.url)?"url":"string"==typeof e.data&&0===(e.contentType||"").indexOf("application/x-www-form-urlencoded")&&Vt.test(e.data)&&"data");if(a||"jsonp"===e.dataTypes[0])return r=e.jsonpCallback=m(e.jsonpCallback)?e.jsonpCallback():e.jsonpCallback,a?e[a]=e[a].replace(Vt,"$1"+r):!1!==e.jsonp&&(e.url+=(St.test(e.url)?"&":"?")+e.jsonp+"="+r),e.converters["script json"]=function(){return o||E.error(r+" was not called"),o[0]},e.dataTypes[0]="json",i=C[r],C[r]=function(){o=arguments},n.always(function(){void 0===i?E(C).removeProp(r):C[r]=i,e[r]&&(e.jsonpCallback=t.jsonpCallback,Xt.push(r)),o&&m(i)&&i(o[0]),o=i=void 0}),"script"}),v.createHTMLDocument=((Ut=S.implementation.createHTMLDocument("").body).innerHTML="<form></form><form></form>",2===Ut.childNodes.length),E.parseHTML=function(e,t,n){return"string"!=typeof e?[]:("boolean"==typeof t&&(n=t,t=!1),t||(v.createHTMLDocument?((r=(t=S.implementation.createHTMLDocument("")).createElement("base")).href=S.location.href,t.head.appendChild(r)):t=S),o=!n&&[],(i=N.exec(e))?[t.createElement(i[1])]:(i=xe([e],t,o),o&&o.length&&E(o).remove(),E.merge([],i.childNodes)));var r,i,o},E.fn.load=function(e,t,n){var r,i,o,a=this,s=e.indexOf(" ");return-1<s&&(r=yt(e.slice(s)),e=e.slice(0,s)),m(t)?(n=t,t=void 0):t&&"object"==typeof t&&(i="POST"),0<a.length&&E.ajax({url:e,type:i||"GET",dataType:"html",data:t}).done(function(e){o=arguments,a.html(r?E("<div>").append(E.parseHTML(e)).find(r):e)}).always(n&&function(e,t){a.each(function(){n.apply(this,o||[e.responseText,t,e])})}),this},E.expr.pseudos.animated=function(t){return E.grep(E.timers,function(e){return t===e.elem}).length},E.offset={setOffset:function(e,t,n){var r,i,o,a,s,u,l=E.css(e,"position"),c=E(e),f={};"static"===l&&(e.style.position="relative"),s=c.offset(),o=E.css(e,"top"),u=E.css(e,"left"),("absolute"===l||"fixed"===l)&&-1<(o+u).indexOf("auto")?(a=(r=c.position()).top,i=r.left):(a=parseFloat(o)||0,i=parseFloat(u)||0),m(t)&&(t=t.call(e,n,E.extend({},s))),null!=t.top&&(f.top=t.top-s.top+a),null!=t.left&&(f.left=t.left-s.left+i),"using"in t?t.using.call(e,f):c.css(f)}},E.fn.extend({offset:function(t){if(arguments.length)return void 0===t?this:this.each(function(e){E.offset.setOffset(this,t,e)});var e,n,r=this[0];return r?r.getClientRects().length?(e=r.getBoundingClientRect(),n=r.ownerDocument.defaultView,{top:e.top+n.pageYOffset,left:e.left+n.pageXOffset}):{top:0,left:0}:void 0},position:function(){if(this[0]){var e,t,n,r=this[0],i={top:0,left:0};if("fixed"===E.css(r,"position"))t=r.getBoundingClientRect();else{t=this.offset(),n=r.ownerDocument,e=r.offsetParent||n.documentElement;while(e&&(e===n.body||e===n.documentElement)&&"static"===E.css(e,"position"))e=e.parentNode;e&&e!==r&&1===e.nodeType&&((i=E(e).offset()).top+=E.css(e,"borderTopWidth",!0),i.left+=E.css(e,"borderLeftWidth",!0))}return{top:t.top-i.top-E.css(r,"marginTop",!0),left:t.left-i.left-E.css(r,"marginLeft",!0)}}},offsetParent:function(){return this.map(function(){var e=this.offsetParent;while(e&&"static"===E.css(e,"position"))e=e.offsetParent;return e||re})}}),E.each({scrollLeft:"pageXOffset",scrollTop:"pageYOffset"},function(t,i){var o="pageYOffset"===i;E.fn[t]=function(e){return B(this,function(e,t,n){var r;if(x(e)?r=e:9===e.nodeType&&(r=e.defaultView),void 0===n)return r?r[i]:e[t];r?r.scrollTo(o?r.pageXOffset:n,o?n:r.pageYOffset):e[t]=n},t,e,arguments.length)}}),E.each(["top","left"],function(e,n){E.cssHooks[n]=_e(v.HyperelPosition,function(e,t){if(t)return t=Be(e,n),Pe.test(t)?E(e).position()[n]+"px":t})}),E.each({Height:"height",Width:"width"},function(a,s){E.each({padding:"inner"+a,content:s,"":"outer"+a},function(r,o){E.fn[o]=function(e,t){var n=arguments.length&&(r||"boolean"!=typeof e),i=r||(!0===e||!0===t?"margin":"border");return B(this,function(e,t,n){var r;return x(e)?0===o.indexOf("outer")?e["inner"+a]:e.document.documentElement["client"+a]:9===e.nodeType?(r=e.documentElement,Math.max(e.body["scroll"+a],r["scroll"+a],e.body["offset"+a],r["offset"+a],r["client"+a])):void 0===n?E.css(e,t,i):E.style(e,t,n,i)},s,n?e:void 0,n)}})}),E.each(["ajaxStart","ajaxStop","ajaxComplete","ajaxError","ajaxSuccess","ajaxSend"],function(e,t){E.fn[t]=function(e){return this.on(t,e)}}),E.fn.extend({bind:function(e,t,n){return this.on(e,null,t,n)},unbind:function(e,t){return this.off(e,null,t)},delegate:function(e,t,n,r){return this.on(t,e,n,r)},undelegate:function(e,t,n){return 1===arguments.length?this.off(e,"**"):this.off(t,e||"**",n)},hover:function(e,t){return this.mouseenter(e).mouseleave(t||e)}}),E.each("blur focus focusin focusout resize scroll click dblclick mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave change select submit keydown keypress keyup contextmenu".split(" "),function(e,n){E.fn[n]=function(e,t){return 0<arguments.length?this.on(n,null,e,t):this.trigger(n)}});var Gt=/^[\s\uFEFF\xA0]+|([^\s\uFEFF\xA0])[\s\uFEFF\xA0]+$/g;E.proxy=function(e,t){var n,r,i;if("string"==typeof t&&(n=e[t],t=e,e=n),m(e))return r=s.call(arguments,2),(i=function(){return e.apply(t||this,r.concat(s.call(arguments)))}).guid=e.guid=e.guid||E.guid++,i},E.holdReady=function(e){e?E.readyWait++:E.ready(!0)},E.isArray=Array.isArray,E.parseJSON=JSON.parse,E.nodeName=A,E.isFunction=m,E.isWindow=x,E.camelCase=X,E.type=w,E.now=Date.now,E.isNumeric=function(e){var t=E.type(e);return("number"===t||"string"===t)&&!isNaN(e-parseFloat(e))},E.trim=function(e){return null==e?"":(e+"").replace(Gt,"$1")},"function"==typeof define&&define.amd&&define("jquery",[],function(){return E});var Yt=C.jQuery,Qt=C.$;return E.noConflict=function(e){return C.$===E&&(C.$=Qt),e&&C.jQuery===E&&(C.jQuery=Yt),E},"undefined"==typeof e&&(C.jQuery=C.$=E),E});

/*! jQuery Migrate v3.4.0 | (c) OpenJS Foundation and other contributors | jquery.org/license */
"undefined"==typeof jQuery.migrateMute&&(jQuery.migrateMute=!0),function(t){"use strict";"function"==typeof define&&define.amd?define(["jquery"],function(e){return t(e,window)}):"object"==typeof module&&module.exports?module.exports=t(require("jquery"),window):t(jQuery,window)}(function(s,n){"use strict";function e(e){return 0<=function(e,t){for(var r=/^(\d+)\.(\d+)\.(\d+)/,n=r.exec(e)||[],o=r.exec(t)||[],a=1;a<=3;a++){if(+n[a]>+o[a])return 1;if(+n[a]<+o[a])return-1}return 0}(s.fn.jquery,e)}s.migrateVersion="3.4.0";var t=Object.create(null),o=(s.migrateDisablePatches=function(){for(var e=0;e<arguments.length;e++)t[arguments[e]]=!0},s.migrateEnablePatches=function(){for(var e=0;e<arguments.length;e++)delete t[arguments[e]]},s.migrateIsPatchEnabled=function(e){return!t[e]},n.console&&n.console.log&&(s&&e("3.0.0")||n.console.log("JQMIGRATE: jQuery 3.0.0+ REQUIRED"),s.migrateWarnings&&n.console.log("JQMIGRATE: Migrate plugin loaded multiple times"),n.console.log("JQMIGRATE: Migrate is installed"+(s.migrateMute?"":" with logging active")+", version "+s.migrateVersion)),{});function i(e,t){var r=n.console;!s.migrateIsPatchEnabled(e)||s.migrateDeduplicateWarnings&&o[t]||(o[t]=!0,s.migrateWarnings.push(t+" ["+e+"]"),r&&r.warn&&!s.migrateMute&&(r.warn("JQMIGRATE: "+t),s.migrateTrace&&r.trace&&r.trace()))}function r(e,t,r,n,o){Object.defineProperty(e,t,{configurable:!0,enumerable:!0,get:function(){return i(n,o),r},set:function(e){i(n,o),r=e}})}function a(e,t,r,n,o){var a=e[t];e[t]=function(){return o&&i(n,o),(s.migrateIsPatchEnabled(n)?r:a||s.noop).apply(this,arguments)}}function u(e,t,r,n,o){if(!o)throw new Error("No warning message provided");a(e,t,r,n,o)}function d(e,t,r,n){a(e,t,r,n)}s.migrateDeduplicateWarnings=!0,s.migrateWarnings=[],void 0===s.migrateTrace&&(s.migrateTrace=!0),s.migrateReset=function(){o={},s.migrateWarnings.length=0},"BackCompat"===n.document.compatMode&&i("quirks","jQuery is not compatible with Quirks Mode");var c,l,p,f={},m=s.fn.init,y=s.find,h=/\[(\s*[-\w]+\s*)([~|^$*]?=)\s*([-\w#]*?#[-\w#]*)\s*\]/,g=/\[(\s*[-\w]+\s*)([~|^$*]?=)\s*([-\w#]*?#[-\w#]*)\s*\]/g,v=/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g;for(c in d(s.fn,"init",function(e){var t=Array.prototype.slice.call(arguments);return s.migrateIsPatchEnabled("selector-empty-id")&&"string"==typeof e&&"#"===e&&(i("selector-empty-id","jQuery( '#' ) is not a valid selector"),t[0]=[]),m.apply(this,t)},"selector-empty-id"),s.fn.init.prototype=s.fn,d(s,"find",function(t){var r=Array.prototype.slice.call(arguments);if("string"==typeof t&&h.test(t))try{n.document.querySelector(t)}catch(e){t=t.replace(g,function(e,t,r,n){return"["+t+r+'"'+n+'"]'});try{n.document.querySelector(t),i("selector-hash","Attribute selector with '#' must be quoted: "+r[0]),r[0]=t}catch(e){i("selector-hash","Attribute selector with '#' was not fixed: "+r[0])}}return y.apply(this,r)},"selector-hash"),y)Object.prototype.hasOwnProperty.call(y,c)&&(s.find[c]=y[c]);u(s.fn,"size",function(){return this.length},"size","jQuery.fn.size() is deprecated and removed; use the .length property"),u(s,"parseJSON",function(){return JSON.parse.apply(null,arguments)},"parseJSON","jQuery.parseJSON is deprecated; use JSON.parse"),u(s,"holdReady",s.holdReady,"holdReady","jQuery.holdReady is deprecated"),u(s,"unique",s.uniqueSort,"unique","jQuery.unique is deprecated; use jQuery.uniqueSort"),r(s.expr,"filters",s.expr.pseudos,"expr-pre-pseudos","jQuery.expr.filters is deprecated; use jQuery.expr.pseudos"),r(s.expr,":",s.expr.pseudos,"expr-pre-pseudos","jQuery.expr[':'] is deprecated; use jQuery.expr.pseudos"),e("3.1.1")&&u(s,"trim",function(e){return null==e?"":(e+"").replace(v,"")},"trim","jQuery.trim is deprecated; use String.prototype.trim"),e("3.2.0")&&(u(s,"nodeName",function(e,t){return e.nodeName&&e.nodeName.toLowerCase()===t.toLowerCase()},"nodeName","jQuery.nodeName is deprecated"),u(s,"isArray",Array.isArray,"isArray","jQuery.isArray is deprecated; use Array.isArray")),e("3.3.0")&&(u(s,"isNumeric",function(e){var t=typeof e;return("number"==t||"string"==t)&&!isNaN(e-parseFloat(e))},"isNumeric","jQuery.isNumeric() is deprecated"),s.each("Boolean Number String Function Array Date RegExp Object Error Symbol".split(" "),function(e,t){f["[object "+t+"]"]=t.toLowerCase()}),u(s,"type",function(e){return null==e?e+"":"object"==typeof e||"function"==typeof e?f[Object.prototype.toString.call(e)]||"object":typeof e},"type","jQuery.type is deprecated"),u(s,"isFunction",function(e){return"function"==typeof e},"isFunction","jQuery.isFunction() is deprecated"),u(s,"isWindow",function(e){return null!=e&&e===e.window},"isWindow","jQuery.isWindow() is deprecated")),s.ajax&&(l=s.ajax,p=/(=)\?(?=&|$)|\?\?/,d(s,"ajax",function(){var e=l.apply(this,arguments);return e.promise&&(u(e,"success",e.done,"jqXHR-methods","jQXHR.success is deprecated and removed"),u(e,"error",e.fail,"jqXHR-methods","jQXHR.error is deprecated and removed"),u(e,"complete",e.always,"jqXHR-methods","jQXHR.complete is deprecated and removed")),e},"jqXHR-methods"),e("4.0.0")||s.ajaxPrefilter("+json",function(e){!1!==e.jsonp&&(p.test(e.url)||"string"==typeof e.data&&0===(e.contentType||"").indexOf("application/x-www-form-urlencoded")&&p.test(e.data))&&i("jsonp-promotion","JSON-to-JSONP auto-promotion is deprecated")}));var j=s.fn.removeAttr,b=s.fn.toggleClass,w=/\S+/g;function Q(e){return e.replace(/-([a-z])/g,function(e,t){return t.toUpperCase()})}d(s.fn,"removeAttr",function(e){var r=this;return s.each(e.match(w),function(e,t){s.expr.match.bool.test(t)&&(i("removeAttr-bool","jQuery.fn.removeAttr no longer sets boolean properties: "+t),r.prop(t,!1))}),j.apply(this,arguments)},"removeAttr-bool"),d(s.fn,"toggleClass",function(t){return void 0!==t&&"boolean"!=typeof t?b.apply(this,arguments):(i("toggleClass-bool","jQuery.fn.toggleClass( boolean ) is deprecated"),this.each(function(){var e=this.getAttribute&&this.getAttribute("class")||"";e&&s.data(this,"__className__",e),this.setAttribute&&this.setAttribute("class",!e&&!1!==t&&s.data(this,"__className__")||"")}))},"toggleClass-bool");var x,A=!1,R=/^[a-z]/,T=/^(?:Border(?:Top|Right|Bottom|Left)?(?:Width|)|(?:Margin|Padding)?(?:Top|Right|Bottom|Left)?|(?:Min|Max)?(?:Width|Height))$/;s.swap&&s.each(["height","width","reliableMarginRight"],function(e,t){var r=s.cssHooks[t]&&s.cssHooks[t].get;r&&(s.cssHooks[t].get=function(){var e;return A=!0,e=r.apply(this,arguments),A=!1,e})}),d(s,"swap",function(e,t,r,n){var o,a={};for(o in A||i("swap","jQuery.swap() is undocumented and deprecated"),t)a[o]=e.style[o],e.style[o]=t[o];for(o in r=r.apply(e,n||[]),t)e.style[o]=a[o];return r},"swap"),e("3.4.0")&&"undefined"!=typeof Proxy&&(s.cssProps=new Proxy(s.cssProps||{},{set:function(){return i("cssProps","jQuery.cssProps is deprecated"),Reflect.set.apply(this,arguments)}})),e("4.0.0")&&"undefined"!=typeof Proxy&&(s.cssNumber=new Proxy({animationIterationCount:!0,columnCount:!0,fillOpacity:!0,flexGrow:!0,flexShrink:!0,fontWeight:!0,gridArea:!0,gridColumn:!0,gridColumnEnd:!0,gridColumnStart:!0,gridRow:!0,gridRowEnd:!0,gridRowStart:!0,lineHeight:!0,opacity:!0,order:!0,orphans:!0,widows:!0,zIndex:!0,zoom:!0},{get:function(){return i("css-number","jQuery.cssNumber is deprecated"),Reflect.get.apply(this,arguments)},set:function(){return i("css-number","jQuery.cssNumber is deprecated"),Reflect.set.apply(this,arguments)}})),x=s.fn.css,d(s.fn,"css",function(e,t){var r,n=this;return e&&"object"==typeof e&&!Array.isArray(e)?(s.each(e,function(e,t){s.fn.css.call(n,e,t)}),this):("number"==typeof t&&(t=Q(e),r=t,R.test(r)&&T.test(r[0].toUpperCase()+r.slice(1))||s.cssNumber[t]||i("css-number",'Number-typed values are deprecated for jQuery.fn.css( "'+e+'", value )')),x.apply(this,arguments))},"css-number");function C(e){var t=n.document.implementation.createHTMLDocument("");return t.body.innerHTML=e,t.body&&t.body.innerHTML}var S,N,P,k,H,E,M,q=s.data,D=(d(s,"data",function(e,t,r){var n,o,a;if(t&&"object"==typeof t&&2===arguments.length){for(a in n=s.hasData(e)&&q.call(this,e),o={},t)a!==Q(a)?(i("data-camelCase","jQuery.data() always sets/gets camelCased names: "+a),n[a]=t[a]):o[a]=t[a];return q.call(this,e,o),t}return t&&"string"==typeof t&&t!==Q(t)&&(n=s.hasData(e)&&q.call(this,e))&&t in n?(i("data-camelCase","jQuery.data() always sets/gets camelCased names: "+t),2<arguments.length&&(n[t]=r),n[t]):q.apply(this,arguments)},"data-camelCase"),s.fx&&(P=s.Tween.prototype.run,k=function(e){return e},d(s.Tween.prototype,"run",function(){1<s.easing[this.easing].length&&(i("easing-one-arg","'jQuery.easing."+this.easing.toString()+"' should use only one argument"),s.easing[this.easing]=k),P.apply(this,arguments)},"easing-one-arg"),S=s.fx.interval,N="jQuery.fx.interval is deprecated",n.requestAnimationFrame&&Object.defineProperty(s.fx,"interval",{configurable:!0,enumerable:!0,get:function(){return n.document.hidden||i("fx-interval",N),s.migrateIsPatchEnabled("fx-interval")&&void 0===S?13:S},set:function(e){i("fx-interval",N),S=e}})),s.fn.load),F=s.event.add,W=s.event.fix,O=(s.event.props=[],s.event.fixHooks={},r(s.event.props,"concat",s.event.props.concat,"event-old-patch","jQuery.event.props.concat() is deprecated and removed"),d(s.event,"fix",function(e){var t=e.type,r=this.fixHooks[t],n=s.event.props;if(n.length){i("event-old-patch","jQuery.event.props are deprecated and removed: "+n.join());while(n.length)s.event.addProp(n.pop())}if(r&&!r._migrated_&&(r._migrated_=!0,i("event-old-patch","jQuery.event.fixHooks are deprecated and removed: "+t),(n=r.props)&&n.length))while(n.length)s.event.addProp(n.pop());return t=W.call(this,e),r&&r.filter?r.filter(t,e):t},"event-old-patch"),d(s.event,"add",function(e,t){return e===n&&"load"===t&&"complete"===n.document.readyState&&i("load-after-event","jQuery(window).on('load'...) called after load event occurred"),F.apply(this,arguments)},"load-after-event"),s.each(["load","unload","error"],function(e,t){d(s.fn,t,function(){var e=Array.prototype.slice.call(arguments,0);return"load"===t&&"string"==typeof e[0]?D.apply(this,e):(i("shorthand-removed-v3","jQuery.fn."+t+"() is deprecated"),e.splice(0,0,t),arguments.length?this.on.apply(this,e):(this.triggerHandler.apply(this,e),this))},"shorthand-removed-v3")}),s.each("blur focus focusin focusout resize scroll click dblclick mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave change select submit keydown keypress keyup contextmenu".split(" "),function(e,r){u(s.fn,r,function(e,t){return 0<arguments.length?this.on(r,null,e,t):this.trigger(r)},"shorthand-deprecated-v3","jQuery.fn."+r+"() event shorthand is deprecated")}),s(function(){s(n.document).triggerHandler("ready")}),s.event.special.ready={setup:function(){this===n.document&&i("ready-event","'ready' event is deprecated")}},u(s.fn,"bind",function(e,t,r){return this.on(e,null,t,r)},"pre-on-methods","jQuery.fn.bind() is deprecated"),u(s.fn,"unbind",function(e,t){return this.off(e,null,t)},"pre-on-methods","jQuery.fn.unbind() is deprecated"),u(s.fn,"delegate",function(e,t,r,n){return this.on(t,e,r,n)},"pre-on-methods","jQuery.fn.delegate() is deprecated"),u(s.fn,"undelegate",function(e,t,r){return 1===arguments.length?this.off(e,"**"):this.off(t,e||"**",r)},"pre-on-methods","jQuery.fn.undelegate() is deprecated"),u(s.fn,"hover",function(e,t){return this.on("mouseenter",e).on("mouseleave",t||e)},"pre-on-methods","jQuery.fn.hover() is deprecated"),/<(?!area|br|col|embed|hr|img|input|link|meta|param)(([a-z][^\/\0>\x20\t\r\n\f]*)[^>]*)\/>/gi),_=(s.UNSAFE_restoreLegacyHtmlPrefilter=function(){s.migrateEnablePatches("self-closed-tags")},d(s,"htmlPrefilter",function(e){var t,r;return(r=(t=e).replace(O,"<$1></$2>"))!==t&&C(t)!==C(r)&&i("self-closed-tags","HTML tags must be properly nested and closed: "+t),e.replace(O,"<$1></$2>")},"self-closed-tags"),s.migrateDisablePatches("self-closed-tags"),s.fn.offset);return d(s.fn,"offset",function(){var e=this[0];return!e||e.nodeType&&e.getBoundingClientRect?_.apply(this,arguments):(i("offset-valid-elem","jQuery.fn.offset() requires a valid DOM element"),arguments.length?this:void 0)},"offset-valid-elem"),s.ajax&&(H=s.param,d(s,"param",function(e,t){var r=s.ajaxSettings&&s.ajaxSettings.traditional;return void 0===t&&r&&(i("param-ajax-traditional","jQuery.param() no longer uses jQuery.ajaxSettings.traditional"),t=r),H.call(this,e,t)},"param-ajax-traditional")),u(s.fn,"andSelf",s.fn.addBack,"andSelf","jQuery.fn.andSelf() is deprecated and removed, use jQuery.fn.addBack()"),s.Deferred&&(E=s.Deferred,M=[["resolve","done",s.Callbacks("once memory"),s.Callbacks("once memory"),"resolved"],["reject","fail",s.Callbacks("once memory"),s.Callbacks("once memory"),"rejected"],["notify","progress",s.Callbacks("memory"),s.Callbacks("memory")]],d(s,"Deferred",function(e){var a=E(),i=a.promise();function t(){var o=arguments;return s.Deferred(function(n){s.each(M,function(e,t){var r="function"==typeof o[e]&&o[e];a[t[1]](function(){var e=r&&r.apply(this,arguments);e&&"function"==typeof e.promise?e.promise().done(n.resolve).fail(n.reject).progress(n.notify):n[t[0]+"With"](this===i?n.promise():this,r?[e]:arguments)})}),o=null}).promise()}return u(a,"pipe",t,"deferred-pipe","deferred.pipe() is deprecated"),u(i,"pipe",t,"deferred-pipe","deferred.pipe() is deprecated"),e&&e.call(a,a),a},"deferred-pipe"),s.Deferred.exceptionHook=E.exceptionHook),s});
//# sourceMappingURL=jquery-migrate.min.map
/*!
  * Bootstrap v4.5.3 (https://getbootstrap.com/)
  * Copyright 2011-2020 The Bootstrap Authors (https://github.com/twbs/bootstrap/graphs/contributors)
  * Licensed under MIT (https://github.com/twbs/bootstrap/blob/main/LICENSE)
  */
!function(t,e){"object"==typeof exports&&"undefined"!=typeof module?e(exports,require("jquery")):"function"==typeof define&&define.amd?define(["exports","jquery"],e):e((t="undefined"!=typeof globalThis?globalThis:t||self).bootstrap={},t.jQuery)}(this,(function(t,e){"use strict";function n(t){return t&&"object"==typeof t&&"default"in t?t:{default:t}}var i=n(e);function o(t,e){for(var n=0;n<e.length;n++){var i=e[n];i.enumerable=i.enumerable||!1,i.configurable=!0,"value"in i&&(i.writable=!0),Object.defineProperty(t,i.key,i)}}function r(t,e,n){return e&&o(t.prototype,e),n&&o(t,n),t}function a(){return(a=Object.assign||function(t){for(var e=1;e<arguments.length;e++){var n=arguments[e];for(var i in n)Object.prototype.hasOwnProperty.call(n,i)&&(t[i]=n[i])}return t}).apply(this,arguments)}var s="transitionend";function l(t){var e=this,n=!1;return i.default(this).one(u.TRANSITION_END,(function(){n=!0})),setTimeout((function(){n||u.triggerTransitionEnd(e)}),t),this}var u={TRANSITION_END:"bsTransitionEnd",getUID:function(t){do{t+=~~(1e6*Math.random())}while(document.getElementById(t));return t},getSelectorFromElement:function(t){var e=t.getAttribute("data-target");if(!e||"#"===e){var n=t.getAttribute("href");e=n&&"#"!==n?n.trim():""}try{return document.querySelector(e)?e:null}catch(t){return null}},getTransitionDurationFromElement:function(t){if(!t)return 0;var e=i.default(t).css("transition-duration"),n=i.default(t).css("transition-delay"),o=parseFloat(e),r=parseFloat(n);return o||r?(e=e.split(",")[0],n=n.split(",")[0],1e3*(parseFloat(e)+parseFloat(n))):0},reflow:function(t){return t.offsetHeight},triggerTransitionEnd:function(t){i.default(t).trigger(s)},supportsTransitionEnd:function(){return Boolean(s)},isElement:function(t){return(t[0]||t).nodeType},typeCheckConfig:function(t,e,n){for(var i in n)if(Object.prototype.hasOwnProperty.call(n,i)){var o=n[i],r=e[i],a=r&&u.isElement(r)?"element":null===(s=r)||"undefined"==typeof s?""+s:{}.toString.call(s).match(/\s([a-z]+)/i)[1].toLowerCase();if(!new RegExp(o).test(a))throw new Error(t.toUpperCase()+': Option "'+i+'" provided type "'+a+'" but expected type "'+o+'".')}var s},findShadowRoot:function(t){if(!document.documentElement.attachShadow)return null;if("function"==typeof t.getRootNode){var e=t.getRootNode();return e instanceof ShadowRoot?e:null}return t instanceof ShadowRoot?t:t.parentNode?u.findShadowRoot(t.parentNode):null},jQueryDetection:function(){if("undefined"==typeof i.default)throw new TypeError("Bootstrap's JavaScript requires jQuery. jQuery must be included before Bootstrap's JavaScript.");var t=i.default.fn.jquery.split(" ")[0].split(".");if(t[0]<2&&t[1]<9||1===t[0]&&9===t[1]&&t[2]<1||t[0]>=4)throw new Error("Bootstrap's JavaScript requires at least jQuery v1.9.1 but less than v4.0.0")}};u.jQueryDetection(),i.default.fn.emulateTransitionEnd=l,i.default.event.special[u.TRANSITION_END]={bindType:s,delegateType:s,handle:function(t){if(i.default(t.target).is(this))return t.handleObj.handler.apply(this,arguments)}};var f="bs.alert",d=i.default.fn.alert,c=function(){function t(t){this._element=t}var e=t.prototype;return e.close=function(t){var e=this._element;t&&(e=this._getRootElement(t)),this._triggerCloseEvent(e).isDefaultPrevented()||this._removeElement(e)},e.dispose=function(){i.default.removeData(this._element,f),this._element=null},e._getRootElement=function(t){var e=u.getSelectorFromElement(t),n=!1;return e&&(n=document.querySelector(e)),n||(n=i.default(t).closest(".alert")[0]),n},e._triggerCloseEvent=function(t){var e=i.default.Event("close.bs.alert");return i.default(t).trigger(e),e},e._removeElement=function(t){var e=this;if(i.default(t).removeClass("show"),i.default(t).hasClass("fade")){var n=u.getTransitionDurationFromElement(t);i.default(t).one(u.TRANSITION_END,(function(n){return e._destroyElement(t,n)})).emulateTransitionEnd(n)}else this._destroyElement(t)},e._destroyElement=function(t){i.default(t).detach().trigger("closed.bs.alert").remove()},t._jQueryInterface=function(e){return this.each((function(){var n=i.default(this),o=n.data(f);o||(o=new t(this),n.data(f,o)),"close"===e&&o[e](this)}))},t._handleDismiss=function(t){return function(e){e&&e.preventDefault(),t.close(this)}},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}}]),t}();i.default(document).on("click.bs.alert.data-api",'[data-dismiss="alert"]',c._handleDismiss(new c)),i.default.fn.alert=c._jQueryInterface,i.default.fn.alert.Constructor=c,i.default.fn.alert.noConflict=function(){return i.default.fn.alert=d,c._jQueryInterface};var h="bs.button",p=i.default.fn.button,m="active",g='[data-toggle^="button"]',_='input:not([type="hidden"])',v=".btn",b=function(){function t(t){this._element=t,this.shouldAvoidTriggerChange=!1}var e=t.prototype;return e.toggle=function(){var t=!0,e=!0,n=i.default(this._element).closest('[data-toggle="buttons"]')[0];if(n){var o=this._element.querySelector(_);if(o){if("radio"===o.type)if(o.checked&&this._element.classList.contains(m))t=!1;else{var r=n.querySelector(".active");r&&i.default(r).removeClass(m)}t&&("checkbox"!==o.type&&"radio"!==o.type||(o.checked=!this._element.classList.contains(m)),this.shouldAvoidTriggerChange||i.default(o).trigger("change")),o.focus(),e=!1}}this._element.hasAttribute("disabled")||this._element.classList.contains("disabled")||(e&&this._element.setAttribute("aria-pressed",!this._element.classList.contains(m)),t&&i.default(this._element).toggleClass(m))},e.dispose=function(){i.default.removeData(this._element,h),this._element=null},t._jQueryInterface=function(e,n){return this.each((function(){var o=i.default(this),r=o.data(h);r||(r=new t(this),o.data(h,r)),r.shouldAvoidTriggerChange=n,"toggle"===e&&r[e]()}))},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}}]),t}();i.default(document).on("click.bs.button.data-api",g,(function(t){var e=t.target,n=e;if(i.default(e).hasClass("btn")||(e=i.default(e).closest(v)[0]),!e||e.hasAttribute("disabled")||e.classList.contains("disabled"))t.preventDefault();else{var o=e.querySelector(_);if(o&&(o.hasAttribute("disabled")||o.classList.contains("disabled")))return void t.preventDefault();"INPUT"!==n.tagName&&"LABEL"===e.tagName||b._jQueryInterface.call(i.default(e),"toggle","INPUT"===n.tagName)}})).on("focus.bs.button.data-api blur.bs.button.data-api",g,(function(t){var e=i.default(t.target).closest(v)[0];i.default(e).toggleClass("focus",/^focus(in)?$/.test(t.type))})),i.default(window).on("load.bs.button.data-api",(function(){for(var t=[].slice.call(document.querySelectorAll('[data-toggle="buttons"] .btn')),e=0,n=t.length;e<n;e++){var i=t[e],o=i.querySelector(_);o.checked||o.hasAttribute("checked")?i.classList.add(m):i.classList.remove(m)}for(var r=0,a=(t=[].slice.call(document.querySelectorAll('[data-toggle="button"]'))).length;r<a;r++){var s=t[r];"true"===s.getAttribute("aria-pressed")?s.classList.add(m):s.classList.remove(m)}})),i.default.fn.button=b._jQueryInterface,i.default.fn.button.Constructor=b,i.default.fn.button.noConflict=function(){return i.default.fn.button=p,b._jQueryInterface};var y=!1,E=document.getElementsByTagName("html")[0],w=window.getComputedStyle(E).direction||"",T=E.getAttribute("dir")||"",C=w||T;C&&"rtl"===C.trim().toLowerCase()&&(y=!0);var S={documentDirectionIsRtl:y},D="carousel",N="bs.carousel",A=".bs.carousel",k=i.default.fn[D],I={interval:5e3,keyboard:!0,slide:!1,pause:"hover",wrap:!0,touch:!0},O={interval:"(number|boolean)",keyboard:"boolean",slide:"(boolean|string)",pause:"(string|boolean)",wrap:"boolean",touch:"boolean"},x="next",j="prev",L="slid.bs.carousel",P="active",F=".active.carousel-item",R={TOUCH:"touch",PEN:"pen"},B=function(){function t(t,e){this._items=null,this._interval=null,this._activeElement=null,this._isPaused=!1,this._isSliding=!1,this.touchTimeout=null,this.touchStartX=0,this.touchDeltaX=0,this._config=this._getConfig(e),this._element=t,this._indicatorsElement=this._element.querySelector(".carousel-indicators"),this._touchSupported="ontouchstart"in document.documentElement||navigator.maxTouchPoints>0,this._pointerEvent=Boolean(window.PointerEvent||window.MSPointerEvent),this._addEventListeners()}var e=t.prototype;return e.next=function(){this._isSliding||this._slide(x)},e.nextWhenVisible=function(){var t=i.default(this._element);!document.hidden&&t.is(":visible")&&"hidden"!==t.css("visibility")&&this.next()},e.prev=function(){this._isSliding||this._slide(j)},e.pause=function(t){t||(this._isPaused=!0),this._element.querySelector(".carousel-item-next, .carousel-item-prev")&&(u.triggerTransitionEnd(this._element),this.cycle(!0)),clearInterval(this._interval),this._interval=null},e.cycle=function(t){t||(this._isPaused=!1),this._interval&&(clearInterval(this._interval),this._interval=null),this._config.interval&&!this._isPaused&&(this._interval=setInterval((document.visibilityState?this.nextWhenVisible:this.next).bind(this),this._config.interval))},e.to=function(t){var e=this;this._activeElement=this._element.querySelector(F);var n=this._getItemIndex(this._activeElement);if(!(t>this._items.length-1||t<0))if(this._isSliding)i.default(this._element).one(L,(function(){return e.to(t)}));else{if(n===t)return this.pause(),void this.cycle();var o=t>n?x:j;this._slide(o,this._items[t])}},e.dispose=function(){i.default(this._element).off(A),i.default.removeData(this._element,N),this._items=null,this._config=null,this._element=null,this._interval=null,this._isPaused=null,this._isSliding=null,this._activeElement=null,this._indicatorsElement=null},e._getConfig=function(t){return t=a({},I,t),u.typeCheckConfig(D,t,O),t},e._handleSwipe=function(){var t=Math.abs(this.touchDeltaX);if(!(t<=40)){var e=t/this.touchDeltaX;this.touchDeltaX=0,e>0&&(S.documentDirectionIsRtl?this.next():this.prev()),e<0&&(S.documentDirectionIsRtl?this.prev():this.next())}},e._addEventListeners=function(){var t=this;this._config.keyboard&&i.default(this._element).on("keydown.bs.carousel",(function(e){return t._keydown(e)})),"hover"===this._config.pause&&i.default(this._element).on("mouseenter.bs.carousel",(function(e){return t.pause(e)})).on("mouseleave.bs.carousel",(function(e){return t.cycle(e)})),this._config.touch&&this._addTouchEventListeners()},e._addTouchEventListeners=function(){var t=this;if(this._touchSupported){var e=function(e){t._pointerEvent&&R[e.originalEvent.pointerType.toUpperCase()]?t.touchStartX=e.originalEvent.clientX:t._pointerEvent||(t.touchStartX=e.originalEvent.touches[0].clientX)},n=function(e){t._pointerEvent&&R[e.originalEvent.pointerType.toUpperCase()]&&(t.touchDeltaX=e.originalEvent.clientX-t.touchStartX),t._handleSwipe(),"hover"===t._config.pause&&(t.pause(),t.touchTimeout&&clearTimeout(t.touchTimeout),t.touchTimeout=setTimeout((function(e){return t.cycle(e)}),500+t._config.interval))};i.default(this._element.querySelectorAll(".carousel-item img")).on("dragstart.bs.carousel",(function(t){return t.preventDefault()})),this._pointerEvent?(i.default(this._element).on("pointerdown.bs.carousel",(function(t){return e(t)})),i.default(this._element).on("pointerup.bs.carousel",(function(t){return n(t)})),this._element.classList.add("pointer-event")):(i.default(this._element).on("touchstart.bs.carousel",(function(t){return e(t)})),i.default(this._element).on("touchmove.bs.carousel",(function(e){return function(e){e.originalEvent.touches&&e.originalEvent.touches.length>1?t.touchDeltaX=0:t.touchDeltaX=e.originalEvent.touches[0].clientX-t.touchStartX}(e)})),i.default(this._element).on("touchend.bs.carousel",(function(t){return n(t)})))}},e._keydown=function(t){if(!/input|textarea/i.test(t.target.tagName))switch(t.which){case 37:t.preventDefault(),this.prev();break;case 39:t.preventDefault(),this.next()}},e._getItemIndex=function(t){return this._items=t&&t.parentNode?[].slice.call(t.parentNode.querySelectorAll(".carousel-item")):[],this._items.indexOf(t)},e._getItemByDirection=function(t,e){var n=t===x,i=t===j,o=this._getItemIndex(e),r=this._items.length-1;if((i&&0===o||n&&o===r)&&!this._config.wrap)return e;var a=(o+(t===j?-1:1))%this._items.length;return-1===a?this._items[this._items.length-1]:this._items[a]},e._triggerSlideEvent=function(t,e){var n=this._getItemIndex(t),o=this._getItemIndex(this._element.querySelector(F)),r=i.default.Event("slide.bs.carousel",{relatedTarget:t,direction:e,from:o,to:n});return i.default(this._element).trigger(r),r},e._setActiveIndicatorElement=function(t){if(this._indicatorsElement){var e=[].slice.call(this._indicatorsElement.querySelectorAll(".active"));i.default(e).removeClass(P);var n=this._indicatorsElement.children[this._getItemIndex(t)];n&&i.default(n).addClass(P)}},e._slide=function(t,e){var n,o,r,a=this,s=this._element.querySelector(F),l=this._getItemIndex(s),f=e||s&&this._getItemByDirection(t,s),d=this._getItemIndex(f),c=Boolean(this._interval);if(t===x?(n="carousel-item-left",o="carousel-item-next",r="left"):(n="carousel-item-right",o="carousel-item-prev",r="right"),f&&i.default(f).hasClass(P))this._isSliding=!1;else if(!this._triggerSlideEvent(f,r).isDefaultPrevented()&&s&&f){this._isSliding=!0,c&&this.pause(),this._setActiveIndicatorElement(f);var h=i.default.Event(L,{relatedTarget:f,direction:r,from:l,to:d});if(i.default(this._element).hasClass("slide")){i.default(f).addClass(o),u.reflow(f),i.default(s).addClass(n),i.default(f).addClass(n);var p=parseInt(f.getAttribute("data-interval"),10);p?(this._config.defaultInterval=this._config.defaultInterval||this._config.interval,this._config.interval=p):this._config.interval=this._config.defaultInterval||this._config.interval;var m=u.getTransitionDurationFromElement(s);i.default(s).one(u.TRANSITION_END,(function(){i.default(f).removeClass(n+" "+o).addClass(P),i.default(s).removeClass("active "+o+" "+n),a._isSliding=!1,setTimeout((function(){return i.default(a._element).trigger(h)}),0)})).emulateTransitionEnd(m)}else i.default(s).removeClass(P),i.default(f).addClass(P),this._isSliding=!1,i.default(this._element).trigger(h);c&&this.cycle()}},t._jQueryInterface=function(e){return this.each((function(){var n=i.default(this).data(N),o=a({},I,i.default(this).data());"object"==typeof e&&(o=a({},o,e));var r="string"==typeof e?e:o.slide;if(n||(n=new t(this,o),i.default(this).data(N,n)),"number"==typeof e)n.to(e);else if("string"==typeof r){if("undefined"==typeof n[r])throw new TypeError('No method named "'+r+'"');n[r]()}else o.interval&&o.ride&&(n.pause(),n.cycle())}))},t._dataApiClickHandler=function(e){var n=u.getSelectorFromElement(this);if(n){var o=i.default(n)[0];if(o&&i.default(o).hasClass("carousel")){var r=a({},i.default(o).data(),i.default(this).data()),s=this.getAttribute("data-slide-to");s&&(r.interval=!1),t._jQueryInterface.call(i.default(o),r),s&&i.default(o).data(N).to(s),e.preventDefault()}}},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}},{key:"Default",get:function(){return I}}]),t}();i.default(document).on("click.bs.carousel.data-api","[data-slide], [data-slide-to]",B._dataApiClickHandler),i.default(window).on("load.bs.carousel.data-api",(function(){for(var t=[].slice.call(document.querySelectorAll('[data-ride="carousel"]')),e=0,n=t.length;e<n;e++){var o=i.default(t[e]);B._jQueryInterface.call(o,o.data())}})),i.default.fn[D]=B._jQueryInterface,i.default.fn[D].Constructor=B,i.default.fn[D].noConflict=function(){return i.default.fn[D]=k,B._jQueryInterface};var H="collapse",M="bs.collapse",q=i.default.fn[H],Q={toggle:!0,parent:""},W={toggle:"boolean",parent:"(string|element)"},U="show",V="collapse",Y="collapsing",z="collapsed",X="width",K='[data-toggle="collapse"]',G=function(){function t(t,e){this._isTransitioning=!1,this._element=t,this._config=this._getConfig(e),this._triggerArray=[].slice.call(document.querySelectorAll('[data-toggle="collapse"][href="#'+t.id+'"],[data-toggle="collapse"][data-target="#'+t.id+'"]'));for(var n=[].slice.call(document.querySelectorAll(K)),i=0,o=n.length;i<o;i++){var r=n[i],a=u.getSelectorFromElement(r),s=[].slice.call(document.querySelectorAll(a)).filter((function(e){return e===t}));null!==a&&s.length>0&&(this._selector=a,this._triggerArray.push(r))}this._parent=this._config.parent?this._getParent():null,this._config.parent||this._addAriaAndCollapsedClass(this._element,this._triggerArray),this._config.toggle&&this.toggle()}var e=t.prototype;return e.toggle=function(){i.default(this._element).hasClass(U)?this.hide():this.show()},e.show=function(){var e,n,o=this;if(!this._isTransitioning&&!i.default(this._element).hasClass(U)&&(this._parent&&0===(e=[].slice.call(this._parent.querySelectorAll(".show, .collapsing")).filter((function(t){return"string"==typeof o._config.parent?t.getAttribute("data-parent")===o._config.parent:t.classList.contains(V)}))).length&&(e=null),!(e&&(n=i.default(e).not(this._selector).data(M))&&n._isTransitioning))){var r=i.default.Event("show.bs.collapse");if(i.default(this._element).trigger(r),!r.isDefaultPrevented()){e&&(t._jQueryInterface.call(i.default(e).not(this._selector),"hide"),n||i.default(e).data(M,null));var a=this._getDimension();i.default(this._element).removeClass(V).addClass(Y),this._element.style[a]=0,this._triggerArray.length&&i.default(this._triggerArray).removeClass(z).attr("aria-expanded",!0),this.setTransitioning(!0);var s="scroll"+(a[0].toUpperCase()+a.slice(1)),l=u.getTransitionDurationFromElement(this._element);i.default(this._element).one(u.TRANSITION_END,(function(){i.default(o._element).removeClass(Y).addClass("collapse show"),o._element.style[a]="",o.setTransitioning(!1),i.default(o._element).trigger("shown.bs.collapse")})).emulateTransitionEnd(l),this._element.style[a]=this._element[s]+"px"}}},e.hide=function(){var t=this;if(!this._isTransitioning&&i.default(this._element).hasClass(U)){var e=i.default.Event("hide.bs.collapse");if(i.default(this._element).trigger(e),!e.isDefaultPrevented()){var n=this._getDimension();this._element.style[n]=this._element.getBoundingClientRect()[n]+"px",u.reflow(this._element),i.default(this._element).addClass(Y).removeClass("collapse show");var o=this._triggerArray.length;if(o>0)for(var r=0;r<o;r++){var a=this._triggerArray[r],s=u.getSelectorFromElement(a);if(null!==s)i.default([].slice.call(document.querySelectorAll(s))).hasClass(U)||i.default(a).addClass(z).attr("aria-expanded",!1)}this.setTransitioning(!0);this._element.style[n]="";var l=u.getTransitionDurationFromElement(this._element);i.default(this._element).one(u.TRANSITION_END,(function(){t.setTransitioning(!1),i.default(t._element).removeClass(Y).addClass(V).trigger("hidden.bs.collapse")})).emulateTransitionEnd(l)}}},e.setTransitioning=function(t){this._isTransitioning=t},e.dispose=function(){i.default.removeData(this._element,M),this._config=null,this._parent=null,this._element=null,this._triggerArray=null,this._isTransitioning=null},e._getConfig=function(t){return(t=a({},Q,t)).toggle=Boolean(t.toggle),u.typeCheckConfig(H,t,W),t},e._getDimension=function(){return i.default(this._element).hasClass(X)?X:"height"},e._getParent=function(){var e,n=this;u.isElement(this._config.parent)?(e=this._config.parent,"undefined"!=typeof this._config.parent.jquery&&(e=this._config.parent[0])):e=document.querySelector(this._config.parent);var o='[data-toggle="collapse"][data-parent="'+this._config.parent+'"]',r=[].slice.call(e.querySelectorAll(o));return i.default(r).each((function(e,i){n._addAriaAndCollapsedClass(t._getTargetFromElement(i),[i])})),e},e._addAriaAndCollapsedClass=function(t,e){var n=i.default(t).hasClass(U);e.length&&i.default(e).toggleClass(z,!n).attr("aria-expanded",n)},t._getTargetFromElement=function(t){var e=u.getSelectorFromElement(t);return e?document.querySelector(e):null},t._jQueryInterface=function(e){return this.each((function(){var n=i.default(this),o=n.data(M),r=a({},Q,n.data(),"object"==typeof e&&e?e:{});if(!o&&r.toggle&&"string"==typeof e&&/show|hide/.test(e)&&(r.toggle=!1),o||(o=new t(this,r),n.data(M,o)),"string"==typeof e){if("undefined"==typeof o[e])throw new TypeError('No method named "'+e+'"');o[e]()}}))},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}},{key:"Default",get:function(){return Q}}]),t}();i.default(document).on("click.bs.collapse.data-api",K,(function(t){"A"===t.currentTarget.tagName&&t.preventDefault();var e=i.default(this),n=u.getSelectorFromElement(this),o=[].slice.call(document.querySelectorAll(n));i.default(o).each((function(){var t=i.default(this),n=t.data(M)?"toggle":e.data();G._jQueryInterface.call(t,n)}))})),i.default.fn[H]=G._jQueryInterface,i.default.fn[H].Constructor=G,i.default.fn[H].noConflict=function(){return i.default.fn[H]=q,G._jQueryInterface};var $="undefined"!=typeof window&&"undefined"!=typeof document&&"undefined"!=typeof navigator,J=function(){for(var t=["Edge","Trident","Firefox"],e=0;e<t.length;e+=1)if($&&navigator.userAgent.indexOf(t[e])>=0)return 1;return 0}();var Z=$&&window.Promise?function(t){var e=!1;return function(){e||(e=!0,window.Promise.resolve().then((function(){e=!1,t()})))}}:function(t){var e=!1;return function(){e||(e=!0,setTimeout((function(){e=!1,t()}),J))}};function tt(t){return t&&"[object Function]"==={}.toString.call(t)}function et(t,e){if(1!==t.nodeType)return[];var n=t.ownerDocument.defaultView.getComputedStyle(t,null);return e?n[e]:n}function nt(t){return"HTML"===t.nodeName?t:t.parentNode||t.host}function it(t){if(!t)return document.body;switch(t.nodeName){case"HTML":case"BODY":return t.ownerDocument.body;case"#document":return t.body}var e=et(t),n=e.overflow,i=e.overflowX,o=e.overflowY;return/(auto|scroll|overlay)/.test(n+o+i)?t:it(nt(t))}function ot(t){return t&&t.referenceNode?t.referenceNode:t}var rt=$&&!(!window.MSInputMethodContext||!document.documentMode),at=$&&/MSIE 10/.test(navigator.userAgent);function st(t){return 11===t?rt:10===t?at:rt||at}function lt(t){if(!t)return document.documentElement;for(var e=st(10)?document.body:null,n=t.offsetParent||null;n===e&&t.nextElementSibling;)n=(t=t.nextElementSibling).offsetParent;var i=n&&n.nodeName;return i&&"BODY"!==i&&"HTML"!==i?-1!==["TH","TD","TABLE"].indexOf(n.nodeName)&&"static"===et(n,"position")?lt(n):n:t?t.ownerDocument.documentElement:document.documentElement}function ut(t){return null!==t.parentNode?ut(t.parentNode):t}function ft(t,e){if(!(t&&t.nodeType&&e&&e.nodeType))return document.documentElement;var n=t.compareDocumentPosition(e)&Node.DOCUMENT_POSITION_FOLLOWING,i=n?t:e,o=n?e:t,r=document.createRange();r.setStart(i,0),r.setEnd(o,0);var a,s,l=r.commonAncestorContainer;if(t!==l&&e!==l||i.contains(o))return"BODY"===(s=(a=l).nodeName)||"HTML"!==s&&lt(a.firstElementChild)!==a?lt(l):l;var u=ut(t);return u.host?ft(u.host,e):ft(t,ut(e).host)}function dt(t){var e=arguments.length>1&&void 0!==arguments[1]?arguments[1]:"top",n="top"===e?"scrollTop":"scrollLeft",i=t.nodeName;if("BODY"===i||"HTML"===i){var o=t.ownerDocument.documentElement,r=t.ownerDocument.scrollingElement||o;return r[n]}return t[n]}function ct(t,e){var n=arguments.length>2&&void 0!==arguments[2]&&arguments[2],i=dt(e,"top"),o=dt(e,"left"),r=n?-1:1;return t.top+=i*r,t.bottom+=i*r,t.left+=o*r,t.right+=o*r,t}function ht(t,e){var n="x"===e?"Left":"Top",i="Left"===n?"Right":"Bottom";return parseFloat(t["border"+n+"Width"])+parseFloat(t["border"+i+"Width"])}function pt(t,e,n,i){return Math.max(e["offset"+t],e["scroll"+t],n["client"+t],n["offset"+t],n["scroll"+t],st(10)?parseInt(n["offset"+t])+parseInt(i["margin"+("Height"===t?"Top":"Left")])+parseInt(i["margin"+("Height"===t?"Bottom":"Right")]):0)}function mt(t){var e=t.body,n=t.documentElement,i=st(10)&&getComputedStyle(n);return{height:pt("Height",e,n,i),width:pt("Width",e,n,i)}}var gt=function(t,e){if(!(t instanceof e))throw new TypeError("Cannot call a class as a function")},_t=function(){function t(t,e){for(var n=0;n<e.length;n++){var i=e[n];i.enumerable=i.enumerable||!1,i.configurable=!0,"value"in i&&(i.writable=!0),Object.defineProperty(t,i.key,i)}}return function(e,n,i){return n&&t(e.prototype,n),i&&t(e,i),e}}(),vt=function(t,e,n){return e in t?Object.defineProperty(t,e,{value:n,enumerable:!0,configurable:!0,writable:!0}):t[e]=n,t},bt=Object.assign||function(t){for(var e=1;e<arguments.length;e++){var n=arguments[e];for(var i in n)Object.prototype.hasOwnProperty.call(n,i)&&(t[i]=n[i])}return t};function yt(t){return bt({},t,{right:t.left+t.width,bottom:t.top+t.height})}function Et(t){var e={};try{if(st(10)){e=t.getBoundingClientRect();var n=dt(t,"top"),i=dt(t,"left");e.top+=n,e.left+=i,e.bottom+=n,e.right+=i}else e=t.getBoundingClientRect()}catch(t){}var o={left:e.left,top:e.top,width:e.right-e.left,height:e.bottom-e.top},r="HTML"===t.nodeName?mt(t.ownerDocument):{},a=r.width||t.clientWidth||o.width,s=r.height||t.clientHeight||o.height,l=t.offsetWidth-a,u=t.offsetHeight-s;if(l||u){var f=et(t);l-=ht(f,"x"),u-=ht(f,"y"),o.width-=l,o.height-=u}return yt(o)}function wt(t,e){var n=arguments.length>2&&void 0!==arguments[2]&&arguments[2],i=st(10),o="HTML"===e.nodeName,r=Et(t),a=Et(e),s=it(t),l=et(e),u=parseFloat(l.borderTopWidth),f=parseFloat(l.borderLeftWidth);n&&o&&(a.top=Math.max(a.top,0),a.left=Math.max(a.left,0));var d=yt({top:r.top-a.top-u,left:r.left-a.left-f,width:r.width,height:r.height});if(d.marginTop=0,d.marginLeft=0,!i&&o){var c=parseFloat(l.marginTop),h=parseFloat(l.marginLeft);d.top-=u-c,d.bottom-=u-c,d.left-=f-h,d.right-=f-h,d.marginTop=c,d.marginLeft=h}return(i&&!n?e.contains(s):e===s&&"BODY"!==s.nodeName)&&(d=ct(d,e)),d}function Tt(t){var e=arguments.length>1&&void 0!==arguments[1]&&arguments[1],n=t.ownerDocument.documentElement,i=wt(t,n),o=Math.max(n.clientWidth,window.innerWidth||0),r=Math.max(n.clientHeight,window.innerHeight||0),a=e?0:dt(n),s=e?0:dt(n,"left"),l={top:a-i.top+i.marginTop,left:s-i.left+i.marginLeft,width:o,height:r};return yt(l)}function Ct(t){var e=t.nodeName;if("BODY"===e||"HTML"===e)return!1;if("fixed"===et(t,"position"))return!0;var n=nt(t);return!!n&&Ct(n)}function St(t){if(!t||!t.parentElement||st())return document.documentElement;for(var e=t.parentElement;e&&"none"===et(e,"transform");)e=e.parentElement;return e||document.documentElement}function Dt(t,e,n,i){var o=arguments.length>4&&void 0!==arguments[4]&&arguments[4],r={top:0,left:0},a=o?St(t):ft(t,ot(e));if("viewport"===i)r=Tt(a,o);else{var s=void 0;"scrollParent"===i?"BODY"===(s=it(nt(e))).nodeName&&(s=t.ownerDocument.documentElement):s="window"===i?t.ownerDocument.documentElement:i;var l=wt(s,a,o);if("HTML"!==s.nodeName||Ct(a))r=l;else{var u=mt(t.ownerDocument),f=u.height,d=u.width;r.top+=l.top-l.marginTop,r.bottom=f+l.top,r.left+=l.left-l.marginLeft,r.right=d+l.left}}var c="number"==typeof(n=n||0);return r.left+=c?n:n.left||0,r.top+=c?n:n.top||0,r.right-=c?n:n.right||0,r.bottom-=c?n:n.bottom||0,r}function Nt(t){return t.width*t.height}function At(t,e,n,i,o){var r=arguments.length>5&&void 0!==arguments[5]?arguments[5]:0;if(-1===t.indexOf("auto"))return t;var a=Dt(n,i,r,o),s={top:{width:a.width,height:e.top-a.top},right:{width:a.right-e.right,height:a.height},bottom:{width:a.width,height:a.bottom-e.bottom},left:{width:e.left-a.left,height:a.height}},l=Object.keys(s).map((function(t){return bt({key:t},s[t],{area:Nt(s[t])})})).sort((function(t,e){return e.area-t.area})),u=l.filter((function(t){var e=t.width,i=t.height;return e>=n.clientWidth&&i>=n.clientHeight})),f=u.length>0?u[0].key:l[0].key,d=t.split("-")[1];return f+(d?"-"+d:"")}function kt(t,e,n){var i=arguments.length>3&&void 0!==arguments[3]?arguments[3]:null,o=i?St(e):ft(e,ot(n));return wt(n,o,i)}function It(t){var e=t.ownerDocument.defaultView.getComputedStyle(t),n=parseFloat(e.marginTop||0)+parseFloat(e.marginBottom||0),i=parseFloat(e.marginLeft||0)+parseFloat(e.marginRight||0);return{width:t.offsetWidth+i,height:t.offsetHeight+n}}function Ot(t){var e={left:"right",right:"left",bottom:"top",top:"bottom"};return t.replace(/left|right|bottom|top/g,(function(t){return e[t]}))}function xt(t,e,n){n=n.split("-")[0];var i=It(t),o={width:i.width,height:i.height},r=-1!==["right","left"].indexOf(n),a=r?"top":"left",s=r?"left":"top",l=r?"height":"width",u=r?"width":"height";return o[a]=e[a]+e[l]/2-i[l]/2,o[s]=n===s?e[s]-i[u]:e[Ot(s)],o}function jt(t,e){return Array.prototype.find?t.find(e):t.filter(e)[0]}function Lt(t,e,n){return(void 0===n?t:t.slice(0,function(t,e,n){if(Array.prototype.findIndex)return t.findIndex((function(t){return t[e]===n}));var i=jt(t,(function(t){return t[e]===n}));return t.indexOf(i)}(t,"name",n))).forEach((function(t){t.function&&console.warn("`modifier.function` is deprecated, use `modifier.fn`!");var n=t.function||t.fn;t.enabled&&tt(n)&&(e.offsets.popper=yt(e.offsets.popper),e.offsets.reference=yt(e.offsets.reference),e=n(e,t))})),e}function Pt(){if(!this.state.isDestroyed){var t={instance:this,styles:{},arrowStyles:{},attributes:{},flipped:!1,offsets:{}};t.offsets.reference=kt(this.state,this.popper,this.reference,this.options.positionFixed),t.placement=At(this.options.placement,t.offsets.reference,this.popper,this.reference,this.options.modifiers.flip.boundariesElement,this.options.modifiers.flip.padding),t.originalPlacement=t.placement,t.positionFixed=this.options.positionFixed,t.offsets.popper=xt(this.popper,t.offsets.reference,t.placement),t.offsets.popper.position=this.options.positionFixed?"fixed":"absolute",t=Lt(this.modifiers,t),this.state.isCreated?this.options.onUpdate(t):(this.state.isCreated=!0,this.options.onCreate(t))}}function Ft(t,e){return t.some((function(t){var n=t.name;return t.enabled&&n===e}))}function Rt(t){for(var e=[!1,"ms","Webkit","Moz","O"],n=t.charAt(0).toUpperCase()+t.slice(1),i=0;i<e.length;i++){var o=e[i],r=o?""+o+n:t;if("undefined"!=typeof document.body.style[r])return r}return null}function Bt(){return this.state.isDestroyed=!0,Ft(this.modifiers,"applyStyle")&&(this.popper.removeAttribute("x-placement"),this.popper.style.position="",this.popper.style.top="",this.popper.style.left="",this.popper.style.right="",this.popper.style.bottom="",this.popper.style.willChange="",this.popper.style[Rt("transform")]=""),this.disableEventListeners(),this.options.removeOnDestroy&&this.popper.parentNode.removeChild(this.popper),this}function Ht(t){var e=t.ownerDocument;return e?e.defaultView:window}function Mt(t,e,n,i){var o="BODY"===t.nodeName,r=o?t.ownerDocument.defaultView:t;r.addEventListener(e,n,{passive:!0}),o||Mt(it(r.parentNode),e,n,i),i.push(r)}function qt(t,e,n,i){n.updateBound=i,Ht(t).addEventListener("resize",n.updateBound,{passive:!0});var o=it(t);return Mt(o,"scroll",n.updateBound,n.scrollParents),n.scrollElement=o,n.eventsEnabled=!0,n}function Qt(){this.state.eventsEnabled||(this.state=qt(this.reference,this.options,this.state,this.scheduleUpdate))}function Wt(){var t,e;this.state.eventsEnabled&&(cancelAnimationFrame(this.scheduleUpdate),this.state=(t=this.reference,e=this.state,Ht(t).removeEventListener("resize",e.updateBound),e.scrollParents.forEach((function(t){t.removeEventListener("scroll",e.updateBound)})),e.updateBound=null,e.scrollParents=[],e.scrollElement=null,e.eventsEnabled=!1,e))}function Ut(t){return""!==t&&!isNaN(parseFloat(t))&&isFinite(t)}function Vt(t,e){Object.keys(e).forEach((function(n){var i="";-1!==["width","height","top","right","bottom","left"].indexOf(n)&&Ut(e[n])&&(i="px"),t.style[n]=e[n]+i}))}var Yt=$&&/Firefox/i.test(navigator.userAgent);function zt(t,e,n){var i=jt(t,(function(t){return t.name===e})),o=!!i&&t.some((function(t){return t.name===n&&t.enabled&&t.order<i.order}));if(!o){var r="`"+e+"`",a="`"+n+"`";console.warn(a+" modifier is required by "+r+" modifier in order to work, be sure to include it before "+r+"!")}return o}var Xt=["auto-start","auto","auto-end","top-start","top","top-end","right-start","right","right-end","bottom-end","bottom","bottom-start","left-end","left","left-start"],Kt=Xt.slice(3);function Gt(t){var e=arguments.length>1&&void 0!==arguments[1]&&arguments[1],n=Kt.indexOf(t),i=Kt.slice(n+1).concat(Kt.slice(0,n));return e?i.reverse():i}var $t="flip",Jt="clockwise",Zt="counterclockwise";function te(t,e,n,i){var o=[0,0],r=-1!==["right","left"].indexOf(i),a=t.split(/(\+|\-)/).map((function(t){return t.trim()})),s=a.indexOf(jt(a,(function(t){return-1!==t.search(/,|\s/)})));a[s]&&-1===a[s].indexOf(",")&&console.warn("Offsets separated by white space(s) are deprecated, use a comma (,) instead.");var l=/\s*,\s*|\s+/,u=-1!==s?[a.slice(0,s).concat([a[s].split(l)[0]]),[a[s].split(l)[1]].concat(a.slice(s+1))]:[a];return(u=u.map((function(t,i){var o=(1===i?!r:r)?"height":"width",a=!1;return t.reduce((function(t,e){return""===t[t.length-1]&&-1!==["+","-"].indexOf(e)?(t[t.length-1]=e,a=!0,t):a?(t[t.length-1]+=e,a=!1,t):t.concat(e)}),[]).map((function(t){return function(t,e,n,i){var o=t.match(/((?:\-|\+)?\d*\.?\d*)(.*)/),r=+o[1],a=o[2];if(!r)return t;if(0===a.indexOf("%")){var s=void 0;switch(a){case"%p":s=n;break;case"%":case"%r":default:s=i}return yt(s)[e]/100*r}if("vh"===a||"vw"===a)return("vh"===a?Math.max(document.documentElement.clientHeight,window.innerHeight||0):Math.max(document.documentElement.clientWidth,window.innerWidth||0))/100*r;return r}(t,o,e,n)}))}))).forEach((function(t,e){t.forEach((function(n,i){Ut(n)&&(o[e]+=n*("-"===t[i-1]?-1:1))}))})),o}var ee={placement:"bottom",positionFixed:!1,eventsEnabled:!0,removeOnDestroy:!1,onCreate:function(){},onUpdate:function(){},modifiers:{shift:{order:100,enabled:!0,fn:function(t){var e=t.placement,n=e.split("-")[0],i=e.split("-")[1];if(i){var o=t.offsets,r=o.reference,a=o.popper,s=-1!==["bottom","top"].indexOf(n),l=s?"left":"top",u=s?"width":"height",f={start:vt({},l,r[l]),end:vt({},l,r[l]+r[u]-a[u])};t.offsets.popper=bt({},a,f[i])}return t}},offset:{order:200,enabled:!0,fn:function(t,e){var n=e.offset,i=t.placement,o=t.offsets,r=o.popper,a=o.reference,s=i.split("-")[0],l=void 0;return l=Ut(+n)?[+n,0]:te(n,r,a,s),"left"===s?(r.top+=l[0],r.left-=l[1]):"right"===s?(r.top+=l[0],r.left+=l[1]):"top"===s?(r.left+=l[0],r.top-=l[1]):"bottom"===s&&(r.left+=l[0],r.top+=l[1]),t.popper=r,t},offset:0},preventOverflow:{order:300,enabled:!0,fn:function(t,e){var n=e.boundariesElement||lt(t.instance.popper);t.instance.reference===n&&(n=lt(n));var i=Rt("transform"),o=t.instance.popper.style,r=o.top,a=o.left,s=o[i];o.top="",o.left="",o[i]="";var l=Dt(t.instance.popper,t.instance.reference,e.padding,n,t.positionFixed);o.top=r,o.left=a,o[i]=s,e.boundaries=l;var u=e.priority,f=t.offsets.popper,d={primary:function(t){var n=f[t];return f[t]<l[t]&&!e.escapeWithReference&&(n=Math.max(f[t],l[t])),vt({},t,n)},secondary:function(t){var n="right"===t?"left":"top",i=f[n];return f[t]>l[t]&&!e.escapeWithReference&&(i=Math.min(f[n],l[t]-("right"===t?f.width:f.height))),vt({},n,i)}};return u.forEach((function(t){var e=-1!==["left","top"].indexOf(t)?"primary":"secondary";f=bt({},f,d[e](t))})),t.offsets.popper=f,t},priority:["left","right","top","bottom"],padding:5,boundariesElement:"scrollParent"},keepTogether:{order:400,enabled:!0,fn:function(t){var e=t.offsets,n=e.popper,i=e.reference,o=t.placement.split("-")[0],r=Math.floor,a=-1!==["top","bottom"].indexOf(o),s=a?"right":"bottom",l=a?"left":"top",u=a?"width":"height";return n[s]<r(i[l])&&(t.offsets.popper[l]=r(i[l])-n[u]),n[l]>r(i[s])&&(t.offsets.popper[l]=r(i[s])),t}},arrow:{order:500,enabled:!0,fn:function(t,e){var n;if(!zt(t.instance.modifiers,"arrow","keepTogether"))return t;var i=e.element;if("string"==typeof i){if(!(i=t.instance.popper.querySelector(i)))return t}else if(!t.instance.popper.contains(i))return console.warn("WARNING: `arrow.element` must be child of its popper element!"),t;var o=t.placement.split("-")[0],r=t.offsets,a=r.popper,s=r.reference,l=-1!==["left","right"].indexOf(o),u=l?"height":"width",f=l?"Top":"Left",d=f.toLowerCase(),c=l?"left":"top",h=l?"bottom":"right",p=It(i)[u];s[h]-p<a[d]&&(t.offsets.popper[d]-=a[d]-(s[h]-p)),s[d]+p>a[h]&&(t.offsets.popper[d]+=s[d]+p-a[h]),t.offsets.popper=yt(t.offsets.popper);var m=s[d]+s[u]/2-p/2,g=et(t.instance.popper),_=parseFloat(g["margin"+f]),v=parseFloat(g["border"+f+"Width"]),b=m-t.offsets.popper[d]-_-v;return b=Math.max(Math.min(a[u]-p,b),0),t.arrowElement=i,t.offsets.arrow=(vt(n={},d,Math.round(b)),vt(n,c,""),n),t},element:"[x-arrow]"},flip:{order:600,enabled:!0,fn:function(t,e){if(Ft(t.instance.modifiers,"inner"))return t;if(t.flipped&&t.placement===t.originalPlacement)return t;var n=Dt(t.instance.popper,t.instance.reference,e.padding,e.boundariesElement,t.positionFixed),i=t.placement.split("-")[0],o=Ot(i),r=t.placement.split("-")[1]||"",a=[];switch(e.behavior){case $t:a=[i,o];break;case Jt:a=Gt(i);break;case Zt:a=Gt(i,!0);break;default:a=e.behavior}return a.forEach((function(s,l){if(i!==s||a.length===l+1)return t;i=t.placement.split("-")[0],o=Ot(i);var u=t.offsets.popper,f=t.offsets.reference,d=Math.floor,c="left"===i&&d(u.right)>d(f.left)||"right"===i&&d(u.left)<d(f.right)||"top"===i&&d(u.bottom)>d(f.top)||"bottom"===i&&d(u.top)<d(f.bottom),h=d(u.left)<d(n.left),p=d(u.right)>d(n.right),m=d(u.top)<d(n.top),g=d(u.bottom)>d(n.bottom),_="left"===i&&h||"right"===i&&p||"top"===i&&m||"bottom"===i&&g,v=-1!==["top","bottom"].indexOf(i),b=!!e.flipVariations&&(v&&"start"===r&&h||v&&"end"===r&&p||!v&&"start"===r&&m||!v&&"end"===r&&g),y=!!e.flipVariationsByContent&&(v&&"start"===r&&p||v&&"end"===r&&h||!v&&"start"===r&&g||!v&&"end"===r&&m),E=b||y;(c||_||E)&&(t.flipped=!0,(c||_)&&(i=a[l+1]),E&&(r=function(t){return"end"===t?"start":"start"===t?"end":t}(r)),t.placement=i+(r?"-"+r:""),t.offsets.popper=bt({},t.offsets.popper,xt(t.instance.popper,t.offsets.reference,t.placement)),t=Lt(t.instance.modifiers,t,"flip"))})),t},behavior:"flip",padding:5,boundariesElement:"viewport",flipVariations:!1,flipVariationsByContent:!1},inner:{order:700,enabled:!1,fn:function(t){var e=t.placement,n=e.split("-")[0],i=t.offsets,o=i.popper,r=i.reference,a=-1!==["left","right"].indexOf(n),s=-1===["top","left"].indexOf(n);return o[a?"left":"top"]=r[n]-(s?o[a?"width":"height"]:0),t.placement=Ot(e),t.offsets.popper=yt(o),t}},hide:{order:800,enabled:!0,fn:function(t){if(!zt(t.instance.modifiers,"hide","preventOverflow"))return t;var e=t.offsets.reference,n=jt(t.instance.modifiers,(function(t){return"preventOverflow"===t.name})).boundaries;if(e.bottom<n.top||e.left>n.right||e.top>n.bottom||e.right<n.left){if(!0===t.hide)return t;t.hide=!0,t.attributes["x-out-of-boundaries"]=""}else{if(!1===t.hide)return t;t.hide=!1,t.attributes["x-out-of-boundaries"]=!1}return t}},computeStyle:{order:850,enabled:!0,fn:function(t,e){var n=e.x,i=e.y,o=t.offsets.popper,r=jt(t.instance.modifiers,(function(t){return"applyStyle"===t.name})).gpuAcceleration;void 0!==r&&console.warn("WARNING: `gpuAcceleration` option moved to `computeStyle` modifier and will not be supported in future versions of Popper.js!");var a=void 0!==r?r:e.gpuAcceleration,s=lt(t.instance.popper),l=Et(s),u={position:o.position},f=function(t,e){var n=t.offsets,i=n.popper,o=n.reference,r=Math.round,a=Math.floor,s=function(t){return t},l=r(o.width),u=r(i.width),f=-1!==["left","right"].indexOf(t.placement),d=-1!==t.placement.indexOf("-"),c=e?f||d||l%2==u%2?r:a:s,h=e?r:s;return{left:c(l%2==1&&u%2==1&&!d&&e?i.left-1:i.left),top:h(i.top),bottom:h(i.bottom),right:c(i.right)}}(t,window.deviceHyperelRatio<2||!Yt),d="bottom"===n?"top":"bottom",c="right"===i?"left":"right",h=Rt("transform"),p=void 0,m=void 0;if(m="bottom"===d?"HTML"===s.nodeName?-s.clientHeight+f.bottom:-l.height+f.bottom:f.top,p="right"===c?"HTML"===s.nodeName?-s.clientWidth+f.right:-l.width+f.right:f.left,a&&h)u[h]="translate3d("+p+"px, "+m+"px, 0)",u[d]=0,u[c]=0,u.willChange="transform";else{var g="bottom"===d?-1:1,_="right"===c?-1:1;u[d]=m*g,u[c]=p*_,u.willChange=d+", "+c}var v={"x-placement":t.placement};return t.attributes=bt({},v,t.attributes),t.styles=bt({},u,t.styles),t.arrowStyles=bt({},t.offsets.arrow,t.arrowStyles),t},gpuAcceleration:!0,x:"bottom",y:"right"},applyStyle:{order:900,enabled:!0,fn:function(t){var e,n;return Vt(t.instance.popper,t.styles),e=t.instance.popper,n=t.attributes,Object.keys(n).forEach((function(t){!1!==n[t]?e.setAttribute(t,n[t]):e.removeAttribute(t)})),t.arrowElement&&Object.keys(t.arrowStyles).length&&Vt(t.arrowElement,t.arrowStyles),t},onLoad:function(t,e,n,i,o){var r=kt(o,e,t,n.positionFixed),a=At(n.placement,r,e,t,n.modifiers.flip.boundariesElement,n.modifiers.flip.padding);return e.setAttribute("x-placement",a),Vt(e,{position:n.positionFixed?"fixed":"absolute"}),n},gpuAcceleration:void 0}}},ne=function(){function t(e,n){var i=this,o=arguments.length>2&&void 0!==arguments[2]?arguments[2]:{};gt(this,t),this.scheduleUpdate=function(){return requestAnimationFrame(i.update)},this.update=Z(this.update.bind(this)),this.options=bt({},t.Defaults,o),this.state={isDestroyed:!1,isCreated:!1,scrollParents:[]},this.reference=e&&e.jquery?e[0]:e,this.popper=n&&n.jquery?n[0]:n,this.options.modifiers={},Object.keys(bt({},t.Defaults.modifiers,o.modifiers)).forEach((function(e){i.options.modifiers[e]=bt({},t.Defaults.modifiers[e]||{},o.modifiers?o.modifiers[e]:{})})),this.modifiers=Object.keys(this.options.modifiers).map((function(t){return bt({name:t},i.options.modifiers[t])})).sort((function(t,e){return t.order-e.order})),this.modifiers.forEach((function(t){t.enabled&&tt(t.onLoad)&&t.onLoad(i.reference,i.popper,i.options,t,i.state)})),this.update();var r=this.options.eventsEnabled;r&&this.enableEventListeners(),this.state.eventsEnabled=r}return _t(t,[{key:"update",value:function(){return Pt.call(this)}},{key:"destroy",value:function(){return Bt.call(this)}},{key:"enableEventListeners",value:function(){return Qt.call(this)}},{key:"disableEventListeners",value:function(){return Wt.call(this)}}]),t}();ne.Utils=("undefined"!=typeof window?window:global).PopperUtils,ne.placements=Xt,ne.Defaults=ee;var ie="dropdown",oe="bs.dropdown",re=".bs.dropdown",ae=i.default.fn[ie],se=new RegExp("38|40|27"),le="hide.bs.dropdown",ue="hidden.bs.dropdown",fe="click.bs.dropdown.data-api",de="keydown.bs.dropdown.data-api",ce="disabled",he="show",pe="dropdown-menu-right",me='[data-toggle="dropdown"]',ge=".dropdown-menu",_e={offset:0,flip:!0,boundary:"scrollParent",reference:"toggle",display:"dynamic",popperConfig:null},ve={offset:"(number|string|function)",flip:"boolean",boundary:"(string|element)",reference:"(string|element)",display:"string",popperConfig:"(null|object)"},be=function(){function t(t,e){this._element=t,this._popper=null,this._config=this._getConfig(e),this._menu=this._getMenuElement(),this._inNavbar=this._detectNavbar(),this._addEventListeners()}var e=t.prototype;return e.toggle=function(){if(!this._element.disabled&&!i.default(this._element).hasClass(ce)){var e=i.default(this._menu).hasClass(he);t._clearMenus(),e||this.show(!0)}},e.show=function(e){if(void 0===e&&(e=!1),!(this._element.disabled||i.default(this._element).hasClass(ce)||i.default(this._menu).hasClass(he))){var n={relatedTarget:this._element},o=i.default.Event("show.bs.dropdown",n),r=t._getParentFromElement(this._element);if(i.default(r).trigger(o),!o.isDefaultPrevented()){if(!this._inNavbar&&e){if("undefined"==typeof ne)throw new TypeError("Bootstrap's dropdowns require Popper.js (https://popper.js.org/)");var a=this._element;"parent"===this._config.reference?a=r:u.isElement(this._config.reference)&&(a=this._config.reference,"undefined"!=typeof this._config.reference.jquery&&(a=this._config.reference[0])),"scrollParent"!==this._config.boundary&&i.default(r).addClass("position-static"),this._popper=new ne(a,this._menu,this._getPopperConfig())}"ontouchstart"in document.documentElement&&0===i.default(r).closest(".navbar-nav").length&&i.default(document.body).children().on("mouseover",null,i.default.noop),this._element.focus(),this._element.setAttribute("aria-expanded",!0),i.default(this._menu).toggleClass(he),i.default(r).toggleClass(he).trigger(i.default.Event("shown.bs.dropdown",n))}}},e.hide=function(){if(!this._element.disabled&&!i.default(this._element).hasClass(ce)&&i.default(this._menu).hasClass(he)){var e={relatedTarget:this._element},n=i.default.Event(le,e),o=t._getParentFromElement(this._element);i.default(o).trigger(n),n.isDefaultPrevented()||(this._popper&&this._popper.destroy(),i.default(this._menu).toggleClass(he),i.default(o).toggleClass(he).trigger(i.default.Event(ue,e)))}},e.dispose=function(){i.default.removeData(this._element,oe),i.default(this._element).off(re),this._element=null,this._menu=null,null!==this._popper&&(this._popper.destroy(),this._popper=null)},e.update=function(){this._inNavbar=this._detectNavbar(),null!==this._popper&&this._popper.scheduleUpdate()},e._addEventListeners=function(){var t=this;i.default(this._element).on("click.bs.dropdown",(function(e){e.preventDefault(),e.stopPropagation(),t.toggle()}))},e._getConfig=function(t){return t=a({},this.constructor.Default,i.default(this._element).data(),t),u.typeCheckConfig(ie,t,this.constructor.DefaultType),t},e._getMenuElement=function(){if(!this._menu){var e=t._getParentFromElement(this._element);e&&(this._menu=e.querySelector(ge))}return this._menu},e._getPlacement=function(){var t=i.default(this._element.parentNode),e="bottom-start";return t.hasClass("dropup")?e=i.default(this._menu).hasClass(pe)?"top-end":"top-start":t.hasClass("dropright")?e="right-start":t.hasClass("dropleft")?e="left-start":i.default(this._menu).hasClass(pe)&&(e="bottom-end"),e},e._detectNavbar=function(){return i.default(this._element).closest(".navbar").length>0},e._getOffset=function(){var t=this,e={};return"function"==typeof this._config.offset?e.fn=function(e){return e.offsets=a({},e.offsets,t._config.offset(e.offsets,t._element)||{}),e}:e.offset=this._config.offset,e},e._getPopperConfig=function(){var t={placement:this._getPlacement(),modifiers:{offset:this._getOffset(),flip:{enabled:this._config.flip},preventOverflow:{boundariesElement:this._config.boundary}}};return"static"===this._config.display&&(t.modifiers.applyStyle={enabled:!1}),a({},t,this._config.popperConfig)},t._jQueryInterface=function(e){return this.each((function(){var n=i.default(this).data(oe);if(n||(n=new t(this,"object"==typeof e?e:null),i.default(this).data(oe,n)),"string"==typeof e){if("undefined"==typeof n[e])throw new TypeError('No method named "'+e+'"');n[e]()}}))},t._clearMenus=function(e){if(!e||3!==e.which&&("keyup"!==e.type||9===e.which))for(var n=[].slice.call(document.querySelectorAll(me)),o=0,r=n.length;o<r;o++){var a=t._getParentFromElement(n[o]),s=i.default(n[o]).data(oe),l={relatedTarget:n[o]};if(e&&"click"===e.type&&(l.clickEvent=e),s){var u=s._menu;if(i.default(a).hasClass(he)&&!(e&&("click"===e.type&&/input|textarea/i.test(e.target.tagName)||"keyup"===e.type&&9===e.which)&&i.default.contains(a,e.target))){var f=i.default.Event(le,l);i.default(a).trigger(f),f.isDefaultPrevented()||("ontouchstart"in document.documentElement&&i.default(document.body).children().off("mouseover",null,i.default.noop),n[o].setAttribute("aria-expanded","false"),s._popper&&s._popper.destroy(),i.default(u).removeClass(he),i.default(a).removeClass(he).trigger(i.default.Event(ue,l)))}}}},t._getParentFromElement=function(t){var e,n=u.getSelectorFromElement(t);return n&&(e=document.querySelector(n)),e||t.parentNode},t._dataApiKeydownHandler=function(e){if(!(/input|textarea/i.test(e.target.tagName)?32===e.which||27!==e.which&&(40!==e.which&&38!==e.which||i.default(e.target).closest(ge).length):!se.test(e.which))&&!this.disabled&&!i.default(this).hasClass(ce)){var n=t._getParentFromElement(this),o=i.default(n).hasClass(he);if(o||27!==e.which){if(e.preventDefault(),e.stopPropagation(),!o||27===e.which||32===e.which)return 27===e.which&&i.default(n.querySelector(me)).trigger("focus"),void i.default(this).trigger("click");var r=[].slice.call(n.querySelectorAll(".dropdown-menu .dropdown-item:not(.disabled):not(:disabled)")).filter((function(t){return i.default(t).is(":visible")}));if(0!==r.length){var a=r.indexOf(e.target);38===e.which&&a>0&&a--,40===e.which&&a<r.length-1&&a++,a<0&&(a=0),r[a].focus()}}}},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}},{key:"Default",get:function(){return _e}},{key:"DefaultType",get:function(){return ve}}]),t}();i.default(document).on(de,me,be._dataApiKeydownHandler).on(de,ge,be._dataApiKeydownHandler).on(fe+" keyup.bs.dropdown.data-api",be._clearMenus).on(fe,me,(function(t){t.preventDefault(),t.stopPropagation(),be._jQueryInterface.call(i.default(this),"toggle")})).on(fe,".dropdown form",(function(t){t.stopPropagation()})),i.default.fn[ie]=be._jQueryInterface,i.default.fn[ie].Constructor=be,i.default.fn[ie].noConflict=function(){return i.default.fn[ie]=ae,be._jQueryInterface};var ye="modal",Ee="bs.modal",we=".bs.modal",Te=i.default.fn.modal,Ce={backdrop:!0,keyboard:!0,focus:!0,show:!0},Se={backdrop:"(boolean|string)",keyboard:"boolean",focus:"boolean",show:"boolean"},De="hidden.bs.modal",Ne="show.bs.modal",Ae="focusin.bs.modal",ke="resize.bs.modal",Ie="click.dismiss.bs.modal",Oe="keydown.dismiss.bs.modal",xe="mousedown.dismiss.bs.modal",je="modal-open",Le="fade",Pe="show",Fe="modal-static",Re=".fixed-top, .fixed-bottom, .is-fixed, .sticky-top",Be=".sticky-top",He=function(){function t(t,e){this._config=this._getConfig(e),this._element=t,this._dialog=t.querySelector(".modal-dialog"),this._backdrop=null,this._isShown=!1,this._isBodyOverflowing=!1,this._ignoreBackdropClick=!1,this._isTransitioning=!1,this._scrollbarWidth=0}var e=t.prototype;return e.toggle=function(t){return this._isShown?this.hide():this.show(t)},e.show=function(t){var e=this;if(!this._isShown&&!this._isTransitioning){i.default(this._element).hasClass(Le)&&(this._isTransitioning=!0);var n=i.default.Event(Ne,{relatedTarget:t});i.default(this._element).trigger(n),this._isShown||n.isDefaultPrevented()||(this._isShown=!0,this._checkScrollbar(),this._setScrollbar(),this._adjustDialog(),this._setEscapeEvent(),this._setResizeEvent(),i.default(this._element).on(Ie,'[data-dismiss="modal"]',(function(t){return e.hide(t)})),i.default(this._dialog).on(xe,(function(){i.default(e._element).one("mouseup.dismiss.bs.modal",(function(t){i.default(t.target).is(e._element)&&(e._ignoreBackdropClick=!0)}))})),this._showBackdrop((function(){return e._showElement(t)})))}},e.hide=function(t){var e=this;if(t&&t.preventDefault(),this._isShown&&!this._isTransitioning){var n=i.default.Event("hide.bs.modal");if(i.default(this._element).trigger(n),this._isShown&&!n.isDefaultPrevented()){this._isShown=!1;var o=i.default(this._element).hasClass(Le);if(o&&(this._isTransitioning=!0),this._setEscapeEvent(),this._setResizeEvent(),i.default(document).off(Ae),i.default(this._element).removeClass(Pe),i.default(this._element).off(Ie),i.default(this._dialog).off(xe),o){var r=u.getTransitionDurationFromElement(this._element);i.default(this._element).one(u.TRANSITION_END,(function(t){return e._hideModal(t)})).emulateTransitionEnd(r)}else this._hideModal()}}},e.dispose=function(){[window,this._element,this._dialog].forEach((function(t){return i.default(t).off(we)})),i.default(document).off(Ae),i.default.removeData(this._element,Ee),this._config=null,this._element=null,this._dialog=null,this._backdrop=null,this._isShown=null,this._isBodyOverflowing=null,this._ignoreBackdropClick=null,this._isTransitioning=null,this._scrollbarWidth=null},e.handleUpdate=function(){this._adjustDialog()},e._getConfig=function(t){return t=a({},Ce,t),u.typeCheckConfig(ye,t,Se),t},e._triggerBackdropTransition=function(){var t=this;if("static"===this._config.backdrop){var e=i.default.Event("hidePrevented.bs.modal");if(i.default(this._element).trigger(e),e.isDefaultPrevented())return;var n=this._element.scrollHeight>document.documentElement.clientHeight;n||(this._element.style.overflowY="hidden"),this._element.classList.add(Fe);var o=u.getTransitionDurationFromElement(this._dialog);i.default(this._element).off(u.TRANSITION_END),i.default(this._element).one(u.TRANSITION_END,(function(){t._element.classList.remove(Fe),n||i.default(t._element).one(u.TRANSITION_END,(function(){t._element.style.overflowY=""})).emulateTransitionEnd(t._element,o)})).emulateTransitionEnd(o),this._element.focus()}else this.hide()},e._showElement=function(t){var e=this,n=i.default(this._element).hasClass(Le),o=this._dialog?this._dialog.querySelector(".modal-body"):null;this._element.parentNode&&this._element.parentNode.nodeType===Node.ELEMENT_NODE||document.body.appendChild(this._element),this._element.style.display="block",this._element.removeAttribute("aria-hidden"),this._element.setAttribute("aria-modal",!0),this._element.setAttribute("role","dialog"),i.default(this._dialog).hasClass("modal-dialog-scrollable")&&o?o.scrollTop=0:this._element.scrollTop=0,n&&u.reflow(this._element),i.default(this._element).addClass(Pe),this._config.focus&&this._enforceFocus();var r=i.default.Event("shown.bs.modal",{relatedTarget:t}),a=function(){e._config.focus&&e._element.focus(),e._isTransitioning=!1,i.default(e._element).trigger(r)};if(n){var s=u.getTransitionDurationFromElement(this._dialog);i.default(this._dialog).one(u.TRANSITION_END,a).emulateTransitionEnd(s)}else a()},e._enforceFocus=function(){var t=this;i.default(document).off(Ae).on(Ae,(function(e){document!==e.target&&t._element!==e.target&&0===i.default(t._element).has(e.target).length&&t._element.focus()}))},e._setEscapeEvent=function(){var t=this;this._isShown?i.default(this._element).on(Oe,(function(e){t._config.keyboard&&27===e.which?(e.preventDefault(),t.hide()):t._config.keyboard||27!==e.which||t._triggerBackdropTransition()})):this._isShown||i.default(this._element).off(Oe)},e._setResizeEvent=function(){var t=this;this._isShown?i.default(window).on(ke,(function(e){return t.handleUpdate(e)})):i.default(window).off(ke)},e._hideModal=function(){var t=this;this._element.style.display="none",this._element.setAttribute("aria-hidden",!0),this._element.removeAttribute("aria-modal"),this._element.removeAttribute("role"),this._isTransitioning=!1,this._showBackdrop((function(){i.default(document.body).removeClass(je),t._resetAdjustments(),t._resetScrollbar(),i.default(t._element).trigger(De)}))},e._removeBackdrop=function(){this._backdrop&&(i.default(this._backdrop).remove(),this._backdrop=null)},e._showBackdrop=function(t){var e=this,n=i.default(this._element).hasClass(Le)?Le:"";if(this._isShown&&this._config.backdrop){if(this._backdrop=document.createElement("div"),this._backdrop.className="modal-backdrop",n&&this._backdrop.classList.add(n),i.default(this._backdrop).appendTo(document.body),i.default(this._element).on(Ie,(function(t){e._ignoreBackdropClick?e._ignoreBackdropClick=!1:t.target===t.currentTarget&&e._triggerBackdropTransition()})),n&&u.reflow(this._backdrop),i.default(this._backdrop).addClass(Pe),!t)return;if(!n)return void t();var o=u.getTransitionDurationFromElement(this._backdrop);i.default(this._backdrop).one(u.TRANSITION_END,t).emulateTransitionEnd(o)}else if(!this._isShown&&this._backdrop){i.default(this._backdrop).removeClass(Pe);var r=function(){e._removeBackdrop(),t&&t()};if(i.default(this._element).hasClass(Le)){var a=u.getTransitionDurationFromElement(this._backdrop);i.default(this._backdrop).one(u.TRANSITION_END,r).emulateTransitionEnd(a)}else r()}else t&&t()},e._adjustDialog=function(){var t=this._element.scrollHeight>document.documentElement.clientHeight;!this._isBodyOverflowing&&t&&(this._element.style.paddingLeft=this._scrollbarWidth+"px"),this._isBodyOverflowing&&!t&&(this._element.style.paddingRight=this._scrollbarWidth+"px")},e._resetAdjustments=function(){this._element.style.paddingLeft="",this._element.style.paddingRight=""},e._checkScrollbar=function(){var t=document.body.getBoundingClientRect();this._isBodyOverflowing=Math.round(t.left+t.right)<window.innerWidth,this._scrollbarWidth=this._getScrollbarWidth()},e._setScrollbar=function(){var t=this;if(this._isBodyOverflowing){var e=[].slice.call(document.querySelectorAll(Re)),n=[].slice.call(document.querySelectorAll(Be));i.default(e).each((function(e,n){var o=n.style.paddingRight,r=i.default(n).css("padding-right");i.default(n).data("padding-right",o).css("padding-right",parseFloat(r)+t._scrollbarWidth+"px")})),i.default(n).each((function(e,n){var o=n.style.marginRight,r=i.default(n).css("margin-right");i.default(n).data("margin-right",o).css("margin-right",parseFloat(r)-t._scrollbarWidth+"px")}));var o=document.body.style.paddingRight,r=i.default(document.body).css("padding-right");i.default(document.body).data("padding-right",o).css("padding-right",parseFloat(r)+this._scrollbarWidth+"px")}i.default(document.body).addClass(je)},e._resetScrollbar=function(){var t=[].slice.call(document.querySelectorAll(Re));i.default(t).each((function(t,e){var n=i.default(e).data("padding-right");i.default(e).removeData("padding-right"),e.style.paddingRight=n||""}));var e=[].slice.call(document.querySelectorAll(".sticky-top"));i.default(e).each((function(t,e){var n=i.default(e).data("margin-right");"undefined"!=typeof n&&i.default(e).css("margin-right",n).removeData("margin-right")}));var n=i.default(document.body).data("padding-right");i.default(document.body).removeData("padding-right"),document.body.style.paddingRight=n||""},e._getScrollbarWidth=function(){var t=document.createElement("div");t.className="modal-scrollbar-measure",document.body.appendChild(t);var e=t.getBoundingClientRect().width-t.clientWidth;return document.body.removeChild(t),e},t._jQueryInterface=function(e,n){return this.each((function(){var o=i.default(this).data(Ee),r=a({},Ce,i.default(this).data(),"object"==typeof e&&e?e:{});if(o||(o=new t(this,r),i.default(this).data(Ee,o)),"string"==typeof e){if("undefined"==typeof o[e])throw new TypeError('No method named "'+e+'"');o[e](n)}else r.show&&o.show(n)}))},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}},{key:"Default",get:function(){return Ce}}]),t}();i.default(document).on("click.bs.modal.data-api",'[data-toggle="modal"]',(function(t){var e,n=this,o=u.getSelectorFromElement(this);o&&(e=document.querySelector(o));var r=i.default(e).data(Ee)?"toggle":a({},i.default(e).data(),i.default(this).data());"A"!==this.tagName&&"AREA"!==this.tagName||t.preventDefault();var s=i.default(e).one(Ne,(function(t){t.isDefaultPrevented()||s.one(De,(function(){i.default(n).is(":visible")&&n.focus()}))}));He._jQueryInterface.call(i.default(e),r,this)})),i.default.fn.modal=He._jQueryInterface,i.default.fn.modal.Constructor=He,i.default.fn.modal.noConflict=function(){return i.default.fn.modal=Te,He._jQueryInterface};var Me=["background","cite","href","itemtype","longdesc","poster","src","xlink:href"],qe={"*":["class","dir","id","lang","role",/^aria-[\w-]*$/i],a:["target","href","title","rel"],area:[],b:[],br:[],col:[],code:[],div:[],em:[],hr:[],h1:[],h2:[],h3:[],h4:[],h5:[],h6:[],i:[],img:["src","srcset","alt","title","width","height"],li:[],ol:[],p:[],pre:[],s:[],small:[],span:[],sub:[],sup:[],strong:[],u:[],ul:[]},Qe=/^(?:(?:https?|mailto|ftp|tel|file):|[^#&/:?]*(?:[#/?]|$))/gi,We=/^data:(?:image\/(?:bmp|gif|jpeg|jpg|png|tiff|webp)|video\/(?:mpeg|mp4|ogg|webm)|audio\/(?:mp3|oga|ogg|opus));base64,[\d+/a-z]+=*$/i;function Ue(t,e,n){if(0===t.length)return t;if(n&&"function"==typeof n)return n(t);for(var i=(new window.DOMParser).parseFromString(t,"text/html"),o=Object.keys(e),r=[].slice.call(i.body.querySelectorAll("*")),a=function(t,n){var i=r[t],a=i.nodeName.toLowerCase();if(-1===o.indexOf(i.nodeName.toLowerCase()))return i.parentNode.removeChild(i),"continue";var s=[].slice.call(i.attributes),l=[].concat(e["*"]||[],e[a]||[]);s.forEach((function(t){(function(t,e){var n=t.nodeName.toLowerCase();if(-1!==e.indexOf(n))return-1===Me.indexOf(n)||Boolean(t.nodeValue.match(Qe)||t.nodeValue.match(We));for(var i=e.filter((function(t){return t instanceof RegExp})),o=0,r=i.length;o<r;o++)if(n.match(i[o]))return!0;return!1})(t,l)||i.removeAttribute(t.nodeName)}))},s=0,l=r.length;s<l;s++)a(s);return i.body.innerHTML}var Ve="tooltip",Ye="bs.tooltip",ze=".bs.tooltip",Xe=i.default.fn.tooltip,Ke=new RegExp("(^|\\s)bs-tooltip\\S+","g"),Ge=["sanitize","whiteList","sanitizeFn"],$e={animation:"boolean",template:"string",title:"(string|element|function)",trigger:"string",delay:"(number|object)",html:"boolean",selector:"(string|boolean)",placement:"(string|function)",offset:"(number|string|function)",container:"(string|element|boolean)",fallbackPlacement:"(string|array)",boundary:"(string|element)",sanitize:"boolean",sanitizeFn:"(null|function)",whiteList:"object",popperConfig:"(null|object)"},Je={AUTO:"auto",TOP:"top",RIGHT:"right",BOTTOM:"bottom",LEFT:"left"},Ze={animation:!0,template:'<div class="tooltip" role="tooltip"><div class="arrow"></div><div class="tooltip-inner"></div></div>',trigger:"hover focus",title:"",delay:0,html:!1,selector:!1,placement:"top",offset:0,container:!1,fallbackPlacement:"flip",boundary:"scrollParent",sanitize:!0,sanitizeFn:null,whiteList:qe,popperConfig:null},tn="show",en="out",nn={HIDE:"hide.bs.tooltip",HIDDEN:"hidden.bs.tooltip",SHOW:"show.bs.tooltip",SHOWN:"shown.bs.tooltip",INSERTED:"inserted.bs.tooltip",CLICK:"click.bs.tooltip",FOCUSIN:"focusin.bs.tooltip",FOCUSOUT:"focusout.bs.tooltip",MOUSEENTER:"mouseenter.bs.tooltip",MOUSELEAVE:"mouseleave.bs.tooltip"},on="fade",rn="show",an="hover",sn="focus",ln=function(){function t(t,e){if("undefined"==typeof ne)throw new TypeError("Bootstrap's tooltips require Popper.js (https://popper.js.org/)");this._isEnabled=!0,this._timeout=0,this._hoverState="",this._activeTrigger={},this._popper=null,this.element=t,this.config=this._getConfig(e),this.tip=null,this._setListeners()}var e=t.prototype;return e.enable=function(){this._isEnabled=!0},e.disable=function(){this._isEnabled=!1},e.toggleEnabled=function(){this._isEnabled=!this._isEnabled},e.toggle=function(t){if(this._isEnabled)if(t){var e=this.constructor.DATA_KEY,n=i.default(t.currentTarget).data(e);n||(n=new this.constructor(t.currentTarget,this._getDelegateConfig()),i.default(t.currentTarget).data(e,n)),n._activeTrigger.click=!n._activeTrigger.click,n._isWithActiveTrigger()?n._enter(null,n):n._leave(null,n)}else{if(i.default(this.getTipElement()).hasClass(rn))return void this._leave(null,this);this._enter(null,this)}},e.dispose=function(){clearTimeout(this._timeout),i.default.removeData(this.element,this.constructor.DATA_KEY),i.default(this.element).off(this.constructor.EVENT_KEY),i.default(this.element).closest(".modal").off("hide.bs.modal",this._hideModalHandler),this.tip&&i.default(this.tip).remove(),this._isEnabled=null,this._timeout=null,this._hoverState=null,this._activeTrigger=null,this._popper&&this._popper.destroy(),this._popper=null,this.element=null,this.config=null,this.tip=null},e.show=function(){var t=this;if("none"===i.default(this.element).css("display"))throw new Error("Please use show on visible elements");var e=i.default.Event(this.constructor.Event.SHOW);if(this.isWithContent()&&this._isEnabled){i.default(this.element).trigger(e);var n=u.findShadowRoot(this.element),o=i.default.contains(null!==n?n:this.element.ownerDocument.documentElement,this.element);if(e.isDefaultPrevented()||!o)return;var r=this.getTipElement(),a=u.getUID(this.constructor.NAME);r.setAttribute("id",a),this.element.setAttribute("aria-describedby",a),this.setContent(),this.config.animation&&i.default(r).addClass(on);var s="function"==typeof this.config.placement?this.config.placement.call(this,r,this.element):this.config.placement,l=this._getAttachment(s);this.addAttachmentClass(l);var f=this._getContainer();i.default(r).data(this.constructor.DATA_KEY,this),i.default.contains(this.element.ownerDocument.documentElement,this.tip)||i.default(r).appendTo(f),i.default(this.element).trigger(this.constructor.Event.INSERTED),this._popper=new ne(this.element,r,this._getPopperConfig(l)),i.default(r).addClass(rn),"ontouchstart"in document.documentElement&&i.default(document.body).children().on("mouseover",null,i.default.noop);var d=function(){t.config.animation&&t._fixTransition();var e=t._hoverState;t._hoverState=null,i.default(t.element).trigger(t.constructor.Event.SHOWN),e===en&&t._leave(null,t)};if(i.default(this.tip).hasClass(on)){var c=u.getTransitionDurationFromElement(this.tip);i.default(this.tip).one(u.TRANSITION_END,d).emulateTransitionEnd(c)}else d()}},e.hide=function(t){var e=this,n=this.getTipElement(),o=i.default.Event(this.constructor.Event.HIDE),r=function(){e._hoverState!==tn&&n.parentNode&&n.parentNode.removeChild(n),e._cleanTipClass(),e.element.removeAttribute("aria-describedby"),i.default(e.element).trigger(e.constructor.Event.HIDDEN),null!==e._popper&&e._popper.destroy(),t&&t()};if(i.default(this.element).trigger(o),!o.isDefaultPrevented()){if(i.default(n).removeClass(rn),"ontouchstart"in document.documentElement&&i.default(document.body).children().off("mouseover",null,i.default.noop),this._activeTrigger.click=!1,this._activeTrigger.focus=!1,this._activeTrigger.hover=!1,i.default(this.tip).hasClass(on)){var a=u.getTransitionDurationFromElement(n);i.default(n).one(u.TRANSITION_END,r).emulateTransitionEnd(a)}else r();this._hoverState=""}},e.update=function(){null!==this._popper&&this._popper.scheduleUpdate()},e.isWithContent=function(){return Boolean(this.getTitle())},e.addAttachmentClass=function(t){i.default(this.getTipElement()).addClass("bs-tooltip-"+t)},e.getTipElement=function(){return this.tip=this.tip||i.default(this.config.template)[0],this.tip},e.setContent=function(){var t=this.getTipElement();this.setElementContent(i.default(t.querySelectorAll(".tooltip-inner")),this.getTitle()),i.default(t).removeClass("fade show")},e.setElementContent=function(t,e){"object"!=typeof e||!e.nodeType&&!e.jquery?this.config.html?(this.config.sanitize&&(e=Ue(e,this.config.whiteList,this.config.sanitizeFn)),t.html(e)):t.text(e):this.config.html?i.default(e).parent().is(t)||t.empty().append(e):t.text(i.default(e).text())},e.getTitle=function(){var t=this.element.getAttribute("data-original-title");return t||(t="function"==typeof this.config.title?this.config.title.call(this.element):this.config.title),t},e._getPopperConfig=function(t){var e=this;return a({},{placement:t,modifiers:{offset:this._getOffset(),flip:{behavior:this.config.fallbackPlacement},arrow:{element:".arrow"},preventOverflow:{boundariesElement:this.config.boundary}},onCreate:function(t){t.originalPlacement!==t.placement&&e._handlePopperPlacementChange(t)},onUpdate:function(t){return e._handlePopperPlacementChange(t)}},this.config.popperConfig)},e._getOffset=function(){var t=this,e={};return"function"==typeof this.config.offset?e.fn=function(e){return e.offsets=a({},e.offsets,t.config.offset(e.offsets,t.element)||{}),e}:e.offset=this.config.offset,e},e._getContainer=function(){return!1===this.config.container?document.body:u.isElement(this.config.container)?i.default(this.config.container):i.default(document).find(this.config.container)},e._getAttachment=function(t){return Je[t.toUpperCase()]},e._setListeners=function(){var t=this;this.config.trigger.split(" ").forEach((function(e){if("click"===e)i.default(t.element).on(t.constructor.Event.CLICK,t.config.selector,(function(e){return t.toggle(e)}));else if("manual"!==e){var n=e===an?t.constructor.Event.MOUSEENTER:t.constructor.Event.FOCUSIN,o=e===an?t.constructor.Event.MOUSELEAVE:t.constructor.Event.FOCUSOUT;i.default(t.element).on(n,t.config.selector,(function(e){return t._enter(e)})).on(o,t.config.selector,(function(e){return t._leave(e)}))}})),this._hideModalHandler=function(){t.element&&t.hide()},i.default(this.element).closest(".modal").on("hide.bs.modal",this._hideModalHandler),this.config.selector?this.config=a({},this.config,{trigger:"manual",selector:""}):this._fixTitle()},e._fixTitle=function(){var t=typeof this.element.getAttribute("data-original-title");(this.element.getAttribute("title")||"string"!==t)&&(this.element.setAttribute("data-original-title",this.element.getAttribute("title")||""),this.element.setAttribute("title",""))},e._enter=function(t,e){var n=this.constructor.DATA_KEY;(e=e||i.default(t.currentTarget).data(n))||(e=new this.constructor(t.currentTarget,this._getDelegateConfig()),i.default(t.currentTarget).data(n,e)),t&&(e._activeTrigger["focusin"===t.type?sn:an]=!0),i.default(e.getTipElement()).hasClass(rn)||e._hoverState===tn?e._hoverState=tn:(clearTimeout(e._timeout),e._hoverState=tn,e.config.delay&&e.config.delay.show?e._timeout=setTimeout((function(){e._hoverState===tn&&e.show()}),e.config.delay.show):e.show())},e._leave=function(t,e){var n=this.constructor.DATA_KEY;(e=e||i.default(t.currentTarget).data(n))||(e=new this.constructor(t.currentTarget,this._getDelegateConfig()),i.default(t.currentTarget).data(n,e)),t&&(e._activeTrigger["focusout"===t.type?sn:an]=!1),e._isWithActiveTrigger()||(clearTimeout(e._timeout),e._hoverState=en,e.config.delay&&e.config.delay.hide?e._timeout=setTimeout((function(){e._hoverState===en&&e.hide()}),e.config.delay.hide):e.hide())},e._isWithActiveTrigger=function(){for(var t in this._activeTrigger)if(this._activeTrigger[t])return!0;return!1},e._getConfig=function(t){var e=i.default(this.element).data();return Object.keys(e).forEach((function(t){-1!==Ge.indexOf(t)&&delete e[t]})),"number"==typeof(t=a({},this.constructor.Default,e,"object"==typeof t&&t?t:{})).delay&&(t.delay={show:t.delay,hide:t.delay}),"number"==typeof t.title&&(t.title=t.title.toString()),"number"==typeof t.content&&(t.content=t.content.toString()),u.typeCheckConfig(Ve,t,this.constructor.DefaultType),t.sanitize&&(t.template=Ue(t.template,t.whiteList,t.sanitizeFn)),t},e._getDelegateConfig=function(){var t={};if(this.config)for(var e in this.config)this.constructor.Default[e]!==this.config[e]&&(t[e]=this.config[e]);return t},e._cleanTipClass=function(){var t=i.default(this.getTipElement()),e=t.attr("class").match(Ke);null!==e&&e.length&&t.removeClass(e.join(""))},e._handlePopperPlacementChange=function(t){this.tip=t.instance.popper,this._cleanTipClass(),this.addAttachmentClass(this._getAttachment(t.placement))},e._fixTransition=function(){var t=this.getTipElement(),e=this.config.animation;null===t.getAttribute("x-placement")&&(i.default(t).removeClass(on),this.config.animation=!1,this.hide(),this.show(),this.config.animation=e)},t._jQueryInterface=function(e){return this.each((function(){var n=i.default(this),o=n.data(Ye),r="object"==typeof e&&e;if((o||!/dispose|hide/.test(e))&&(o||(o=new t(this,r),n.data(Ye,o)),"string"==typeof e)){if("undefined"==typeof o[e])throw new TypeError('No method named "'+e+'"');o[e]()}}))},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}},{key:"Default",get:function(){return Ze}},{key:"NAME",get:function(){return Ve}},{key:"DATA_KEY",get:function(){return Ye}},{key:"Event",get:function(){return nn}},{key:"EVENT_KEY",get:function(){return ze}},{key:"DefaultType",get:function(){return $e}}]),t}();i.default.fn.tooltip=ln._jQueryInterface,i.default.fn.tooltip.Constructor=ln,i.default.fn.tooltip.noConflict=function(){return i.default.fn.tooltip=Xe,ln._jQueryInterface};var un="popover",fn="bs.popover",dn=".bs.popover",cn=i.default.fn.popover,hn=new RegExp("(^|\\s)bs-popover\\S+","g"),pn=a({},ln.Default,{placement:"right",trigger:"click",content:"",template:'<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-header"></h3><div class="popover-body"></div></div>'}),mn=a({},ln.DefaultType,{content:"(string|element|function)"}),gn={HIDE:"hide.bs.popover",HIDDEN:"hidden.bs.popover",SHOW:"show.bs.popover",SHOWN:"shown.bs.popover",INSERTED:"inserted.bs.popover",CLICK:"click.bs.popover",FOCUSIN:"focusin.bs.popover",FOCUSOUT:"focusout.bs.popover",MOUSEENTER:"mouseenter.bs.popover",MOUSELEAVE:"mouseleave.bs.popover"},_n=function(t){var e,n;function o(){return t.apply(this,arguments)||this}n=t,(e=o).prototype=Object.create(n.prototype),e.prototype.constructor=e,e.__proto__=n;var a=o.prototype;return a.isWithContent=function(){return this.getTitle()||this._getContent()},a.addAttachmentClass=function(t){i.default(this.getTipElement()).addClass("bs-popover-"+t)},a.getTipElement=function(){return this.tip=this.tip||i.default(this.config.template)[0],this.tip},a.setContent=function(){var t=i.default(this.getTipElement());this.setElementContent(t.find(".popover-header"),this.getTitle());var e=this._getContent();"function"==typeof e&&(e=e.call(this.element)),this.setElementContent(t.find(".popover-body"),e),t.removeClass("fade show")},a._getContent=function(){return this.element.getAttribute("data-content")||this.config.content},a._cleanTipClass=function(){var t=i.default(this.getTipElement()),e=t.attr("class").match(hn);null!==e&&e.length>0&&t.removeClass(e.join(""))},o._jQueryInterface=function(t){return this.each((function(){var e=i.default(this).data(fn),n="object"==typeof t?t:null;if((e||!/dispose|hide/.test(t))&&(e||(e=new o(this,n),i.default(this).data(fn,e)),"string"==typeof t)){if("undefined"==typeof e[t])throw new TypeError('No method named "'+t+'"');e[t]()}}))},r(o,null,[{key:"VERSION",get:function(){return"4.5.3"}},{key:"Default",get:function(){return pn}},{key:"NAME",get:function(){return un}},{key:"DATA_KEY",get:function(){return fn}},{key:"Event",get:function(){return gn}},{key:"EVENT_KEY",get:function(){return dn}},{key:"DefaultType",get:function(){return mn}}]),o}(ln);i.default.fn.popover=_n._jQueryInterface,i.default.fn.popover.Constructor=_n,i.default.fn.popover.noConflict=function(){return i.default.fn.popover=cn,_n._jQueryInterface};var vn="scrollspy",bn="bs.scrollspy",yn="."+bn,En=i.default.fn[vn],wn={offset:10,method:"auto",target:""},Tn={offset:"number",method:"string",target:"(string|element)"},Cn="active",Sn=".nav, .list-group",Dn=".nav-link",Nn="position",An=function(){function t(t,e){var n=this;this._element=t,this._scrollElement="BODY"===t.tagName?window:t,this._config=this._getConfig(e),this._selector=this._config.target+" "+".nav-link,"+this._config.target+" .list-group-item,"+this._config.target+" .dropdown-item",this._offsets=[],this._targets=[],this._activeTarget=null,this._scrollHeight=0,i.default(this._scrollElement).on("scroll.bs.scrollspy",(function(t){return n._process(t)})),this.refresh(),this._process()}var e=t.prototype;return e.refresh=function(){var t=this,e=this._scrollElement===this._scrollElement.window?"offset":Nn,n="auto"===this._config.method?e:this._config.method,o=n===Nn?this._getScrollTop():0;this._offsets=[],this._targets=[],this._scrollHeight=this._getScrollHeight(),[].slice.call(document.querySelectorAll(this._selector)).map((function(t){var e,r=u.getSelectorFromElement(t);if(r&&(e=document.querySelector(r)),e){var a=e.getBoundingClientRect();if(a.width||a.height)return[i.default(e)[n]().top+o,r]}return null})).filter((function(t){return t})).sort((function(t,e){return t[0]-e[0]})).forEach((function(e){t._offsets.push(e[0]),t._targets.push(e[1])}))},e.dispose=function(){i.default.removeData(this._element,bn),i.default(this._scrollElement).off(yn),this._element=null,this._scrollElement=null,this._config=null,this._selector=null,this._offsets=null,this._targets=null,this._activeTarget=null,this._scrollHeight=null},e._getConfig=function(t){if("string"!=typeof(t=a({},wn,"object"==typeof t&&t?t:{})).target&&u.isElement(t.target)){var e=i.default(t.target).attr("id");e||(e=u.getUID(vn),i.default(t.target).attr("id",e)),t.target="#"+e}return u.typeCheckConfig(vn,t,Tn),t},e._getScrollTop=function(){return this._scrollElement===window?this._scrollElement.pageYOffset:this._scrollElement.scrollTop},e._getScrollHeight=function(){return this._scrollElement.scrollHeight||Math.max(document.body.scrollHeight,document.documentElement.scrollHeight)},e._getOffsetHeight=function(){return this._scrollElement===window?window.innerHeight:this._scrollElement.getBoundingClientRect().height},e._process=function(){var t=this._getScrollTop()+this._config.offset,e=this._getScrollHeight(),n=this._config.offset+e-this._getOffsetHeight();if(this._scrollHeight!==e&&this.refresh(),t>=n){var i=this._targets[this._targets.length-1];this._activeTarget!==i&&this._activate(i)}else{if(this._activeTarget&&t<this._offsets[0]&&this._offsets[0]>0)return this._activeTarget=null,void this._clear();for(var o=this._offsets.length;o--;){this._activeTarget!==this._targets[o]&&t>=this._offsets[o]&&("undefined"==typeof this._offsets[o+1]||t<this._offsets[o+1])&&this._activate(this._targets[o])}}},e._activate=function(t){this._activeTarget=t,this._clear();var e=this._selector.split(",").map((function(e){return e+'[data-target="'+t+'"],'+e+'[href="'+t+'"]'})),n=i.default([].slice.call(document.querySelectorAll(e.join(","))));n.hasClass("dropdown-item")?(n.closest(".dropdown").find(".dropdown-toggle").addClass(Cn),n.addClass(Cn)):(n.addClass(Cn),n.parents(Sn).prev(".nav-link, .list-group-item").addClass(Cn),n.parents(Sn).prev(".nav-item").children(Dn).addClass(Cn)),i.default(this._scrollElement).trigger("activate.bs.scrollspy",{relatedTarget:t})},e._clear=function(){[].slice.call(document.querySelectorAll(this._selector)).filter((function(t){return t.classList.contains(Cn)})).forEach((function(t){return t.classList.remove(Cn)}))},t._jQueryInterface=function(e){return this.each((function(){var n=i.default(this).data(bn);if(n||(n=new t(this,"object"==typeof e&&e),i.default(this).data(bn,n)),"string"==typeof e){if("undefined"==typeof n[e])throw new TypeError('No method named "'+e+'"');n[e]()}}))},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}},{key:"Default",get:function(){return wn}}]),t}();i.default(window).on("load.bs.scrollspy.data-api",(function(){for(var t=[].slice.call(document.querySelectorAll('[data-spy="scroll"]')),e=t.length;e--;){var n=i.default(t[e]);An._jQueryInterface.call(n,n.data())}})),i.default.fn[vn]=An._jQueryInterface,i.default.fn[vn].Constructor=An,i.default.fn[vn].noConflict=function(){return i.default.fn[vn]=En,An._jQueryInterface};var kn="bs.tab",In=i.default.fn.tab,On="active",xn="fade",jn="show",Ln=".active",Pn="> li > .active",Fn=function(){function t(t){this._element=t}var e=t.prototype;return e.show=function(){var t=this;if(!(this._element.parentNode&&this._element.parentNode.nodeType===Node.ELEMENT_NODE&&i.default(this._element).hasClass(On)||i.default(this._element).hasClass("disabled"))){var e,n,o=i.default(this._element).closest(".nav, .list-group")[0],r=u.getSelectorFromElement(this._element);if(o){var a="UL"===o.nodeName||"OL"===o.nodeName?Pn:Ln;n=(n=i.default.makeArray(i.default(o).find(a)))[n.length-1]}var s=i.default.Event("hide.bs.tab",{relatedTarget:this._element}),l=i.default.Event("show.bs.tab",{relatedTarget:n});if(n&&i.default(n).trigger(s),i.default(this._element).trigger(l),!l.isDefaultPrevented()&&!s.isDefaultPrevented()){r&&(e=document.querySelector(r)),this._activate(this._element,o);var f=function(){var e=i.default.Event("hidden.bs.tab",{relatedTarget:t._element}),o=i.default.Event("shown.bs.tab",{relatedTarget:n});i.default(n).trigger(e),i.default(t._element).trigger(o)};e?this._activate(e,e.parentNode,f):f()}}},e.dispose=function(){i.default.removeData(this._element,kn),this._element=null},e._activate=function(t,e,n){var o=this,r=(!e||"UL"!==e.nodeName&&"OL"!==e.nodeName?i.default(e).children(Ln):i.default(e).find(Pn))[0],a=n&&r&&i.default(r).hasClass(xn),s=function(){return o._transitionComplete(t,r,n)};if(r&&a){var l=u.getTransitionDurationFromElement(r);i.default(r).removeClass(jn).one(u.TRANSITION_END,s).emulateTransitionEnd(l)}else s()},e._transitionComplete=function(t,e,n){if(e){i.default(e).removeClass(On);var o=i.default(e.parentNode).find("> .dropdown-menu .active")[0];o&&i.default(o).removeClass(On),"tab"===e.getAttribute("role")&&e.setAttribute("aria-selected",!1)}if(i.default(t).addClass(On),"tab"===t.getAttribute("role")&&t.setAttribute("aria-selected",!0),u.reflow(t),t.classList.contains(xn)&&t.classList.add(jn),t.parentNode&&i.default(t.parentNode).hasClass("dropdown-menu")){var r=i.default(t).closest(".dropdown")[0];if(r){var a=[].slice.call(r.querySelectorAll(".dropdown-toggle"));i.default(a).addClass(On)}t.setAttribute("aria-expanded",!0)}n&&n()},t._jQueryInterface=function(e){return this.each((function(){var n=i.default(this),o=n.data(kn);if(o||(o=new t(this),n.data(kn,o)),"string"==typeof e){if("undefined"==typeof o[e])throw new TypeError('No method named "'+e+'"');o[e]()}}))},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}}]),t}();i.default(document).on("click.bs.tab.data-api",'[data-toggle="tab"], [data-toggle="pill"], [data-toggle="list"]',(function(t){t.preventDefault(),Fn._jQueryInterface.call(i.default(this),"show")})),i.default.fn.tab=Fn._jQueryInterface,i.default.fn.tab.Constructor=Fn,i.default.fn.tab.noConflict=function(){return i.default.fn.tab=In,Fn._jQueryInterface};var Rn="toast",Bn="bs.toast",Hn=i.default.fn.toast,Mn="click.dismiss.bs.toast",qn="hide",Qn="show",Wn="showing",Un={animation:"boolean",autohide:"boolean",delay:"number"},Vn={animation:!0,autohide:!0,delay:500},Yn=function(){function t(t,e){this._element=t,this._config=this._getConfig(e),this._timeout=null,this._setListeners()}var e=t.prototype;return e.show=function(){var t=this,e=i.default.Event("show.bs.toast");if(i.default(this._element).trigger(e),!e.isDefaultPrevented()){this._clearTimeout(),this._config.animation&&this._element.classList.add("fade");var n=function(){t._element.classList.remove(Wn),t._element.classList.add(Qn),i.default(t._element).trigger("shown.bs.toast"),t._config.autohide&&(t._timeout=setTimeout((function(){t.hide()}),t._config.delay))};if(this._element.classList.remove(qn),u.reflow(this._element),this._element.classList.add(Wn),this._config.animation){var o=u.getTransitionDurationFromElement(this._element);i.default(this._element).one(u.TRANSITION_END,n).emulateTransitionEnd(o)}else n()}},e.hide=function(){if(this._element.classList.contains(Qn)){var t=i.default.Event("hide.bs.toast");i.default(this._element).trigger(t),t.isDefaultPrevented()||this._close()}},e.dispose=function(){this._clearTimeout(),this._element.classList.contains(Qn)&&this._element.classList.remove(Qn),i.default(this._element).off(Mn),i.default.removeData(this._element,Bn),this._element=null,this._config=null},e._getConfig=function(t){return t=a({},Vn,i.default(this._element).data(),"object"==typeof t&&t?t:{}),u.typeCheckConfig(Rn,t,this.constructor.DefaultType),t},e._setListeners=function(){var t=this;i.default(this._element).on(Mn,'[data-dismiss="toast"]',(function(){return t.hide()}))},e._close=function(){var t=this,e=function(){t._element.classList.add(qn),i.default(t._element).trigger("hidden.bs.toast")};if(this._element.classList.remove(Qn),this._config.animation){var n=u.getTransitionDurationFromElement(this._element);i.default(this._element).one(u.TRANSITION_END,e).emulateTransitionEnd(n)}else e()},e._clearTimeout=function(){clearTimeout(this._timeout),this._timeout=null},t._jQueryInterface=function(e){return this.each((function(){var n=i.default(this),o=n.data(Bn);if(o||(o=new t(this,"object"==typeof e&&e),n.data(Bn,o)),"string"==typeof e){if("undefined"==typeof o[e])throw new TypeError('No method named "'+e+'"');o[e](this)}}))},r(t,null,[{key:"VERSION",get:function(){return"4.5.3"}},{key:"DefaultType",get:function(){return Un}},{key:"Default",get:function(){return Vn}}]),t}();i.default.fn.toast=Yn._jQueryInterface,i.default.fn.toast.Constructor=Yn,i.default.fn.toast.noConflict=function(){return i.default.fn.toast=Hn,Yn._jQueryInterface},t.Alert=c,t.Button=b,t.Carousel=B,t.Collapse=G,t.Dropdown=be,t.Modal=He,t.Popover=_n,t.Rtl=S,t.Scrollspy=An,t.Tab=Fn,t.Toast=Yn,t.Tooltip=ln,t.Util=u,Object.defineProperty(t,"__esModule",{value:!0})}));
//# sourceMappingURL=bootstrap.bundle.min.js.map
window.ModalManager = function ($) {
    var defaults = {
        $body: '',
        $bodyClass: '',
        $footer: '',
        $headerClass: '',
        title: '',
        $title: null,
        id: 'myModal',
        modalPlaceSelector: '#modal-place',
        size: 'lg',
        dontShowHeader: false,
        backdrop: true,
        keyboard: true,
        shownCallback: null
    };

    var getModalHeader = function (options) {
        var modalHeader = $('<div/>',
            {
                'class': 'modal-header ' + options.$headerClass
            });
        var closeButton = $('<button/>',
            {
                'class': 'close',
                'type': 'button',
                'data-dismiss': 'modal',
                'aria-label': 'Close'
            });
        var timesIcon = $('<span/>',
            {
                'aria-hidden': true,
                'text': '×'
            });
        closeButton.append(timesIcon);
        var modalTitle = $('<h4/>',
            {
                'id': options.id + 'Label',
                'class': 'modal-title',
            });
        if (options.$title) {
            modalTitle.append(options.$title);
        } else {
            modalTitle.text(options.title);
        }
        modalHeader.append(modalTitle);
        modalHeader.append(closeButton);
        return modalHeader;
    };
    var getModal = function (options) {
        var modalWrapper = $('<div/>',
            {
                'class': 'modal fade',
                'id': options.id,
                'data-backdrop': options.backdrop,
                'data-keyboard': options.keyboard,
                'tabindex': '-1',
                'role': 'dialog',
                'aria-labelledby': options.id + 'Label'
            });
        var modalDialog = $('<div/>',
            {
                'class': 'modal-dialog modal-' + options.size,
                'role': 'document'
            });
        var modalContent = $('<div/>',
            {
                'class': 'modal-content'
            });
        if (!options.dontShowHeader) {
            var modalHeader = getModalHeader(options);
            modalContent.append(modalHeader);
        }
        if (options.$body) {
            var modalBody = $('<div/>',
                {
                    'class': 'modal-body ' + options.$bodyClass
                }).append(options.$body);
            modalContent.append(modalBody);
        }

        if (options.$footer) {
            var modalFooter = $('<div/>',
                {
                    'class': 'modal-footer'
                }).append(options.$footer);
            modalContent.append(modalFooter);
        }
        modalDialog.append(modalContent);
        modalWrapper.append(modalDialog);
        modalWrapper.on('hidden.bs.modal', function () {
            clearPreviousModal(options.modalPlaceSelector);
        });
        if (typeof options.shownCallback === 'function') {
            modalWrapper.on('shown.bs.modal', function () {
                options.shownCallback();
            });
        }
        return modalWrapper;
    };

    var clearPreviousModal = function (modalPlaceSelector) {
        $(modalPlaceSelector).html('');
        $('.modal-backdrop').remove();
    }

    var openModal = function (options) {
        var modalOptions = $.extend({}, defaults, options);

        clearPreviousModal(modalOptions.modalPlaceSelector);
        $(modalOptions.modalPlaceSelector).html(getModal(modalOptions));
        $('#' + modalOptions.id).modal('show');
    }

    var closeModal = function (id) {
        id = id || 'myModal';
        $('#' + id).modal('hide');
    }

    var confirm = function (confirmOptions) {
        if (typeof confirmOptions.callback !== 'function')
            throw new Error('Callback function must be provided.');
        if (!confirmOptions.message)
            throw new Error('message must be provided.');
        confirmOptions.yesLabel = confirmOptions.yesLabel || window.tetaI18n.t('Yes');
        confirmOptions.noLabel = confirmOptions.noLabel || window.tetaI18n.t('No');
        var modalOptions = {
            backdrop: 'static',
            keyboard: false,
            dontShowHeader: true,
            size: 'md',
            $body: $('<div/>', {text: confirmOptions.message}),
            $footer: $('<div/>')
                .append($('<button/>',
                    {
                        'class': 'btn btn-primary',
                        'id': 'yes_button',
                        'type': 'button',
                        'text': confirmOptions.yesLabel,
                        on: {
                            click: function () {
                                confirmOptions.callback(true);
                                closeModal();
                            }

                        }
                    })
                    .append($('<i/>',
                        {
                            'class': 'fa fa-check mx-1'
                        }))
                )
                .append($('<button/>',
                    {
                        'class': 'btn btn-secondary mx-1',
                        'id': 'no_button',
                        'type': 'button',
                        'text': confirmOptions.noLabel,
                        one: {
                            click: function () {
                                confirmOptions.callback(false);
                                closeModal();

                            }
                        }
                    })
                    .append($('<i/>',
                        {
                            'class': 'fa fa-times mx-1'
                        }))
                )
        };
        openModal(modalOptions);
    }
    return {
        openModal: openModal,
        closeModal: closeModal,
        confirm: confirm
    };
}(window.jQuery);
/*! js-cookie v2.2.1 | MIT */

!function(a){var b;if("function"==typeof define&&define.amd&&(define(a),b=!0),"object"==typeof exports&&(module.exports=a(),b=!0),!b){var c=window.Cookies,d=window.Cookies=a();d.noConflict=function(){return window.Cookies=c,d}}}(function(){function a(){for(var a=0,b={};a<arguments.length;a++){var c=arguments[a];for(var d in c)b[d]=c[d]}return b}function b(a){return a.replace(/(%[0-9A-Z]{2})+/g,decodeURIComponent)}function c(d){function e(){}function f(b,c,f){if("undefined"!=typeof document){f=a({path:"/"},e.defaults,f),"number"==typeof f.expires&&(f.expires=new Date(1*new Date+864e5*f.expires)),f.expires=f.expires?f.expires.toUTCString():"";try{var g=JSON.stringify(c);/^[\{\[]/.test(g)&&(c=g)}catch(j){}c=d.write?d.write(c,b):encodeURIComponent(c+"").replace(/%(23|24|26|2B|3A|3C|3E|3D|2F|3F|40|5B|5D|5E|60|7B|7D|7C)/g,decodeURIComponent),b=encodeURIComponent(b+"").replace(/%(23|24|26|2B|5E|60|7C)/g,decodeURIComponent).replace(/[\(\)]/g,escape);var h="";for(var i in f)f[i]&&(h+="; "+i,!0!==f[i]&&(h+="="+f[i].split(";")[0]));return document.cookie=b+"="+c+h}}function g(a,c){if("undefined"!=typeof document){for(var e={},f=document.cookie?document.cookie.split("; "):[],g=0;g<f.length;g++){var h=f[g].split("="),i=h.slice(1).join("=");c||'"'!==i.charAt(0)||(i=i.slice(1,-1));try{var j=b(h[0]);if(i=(d.read||d)(i,j)||b(i),c)try{i=JSON.parse(i)}catch(k){}if(e[j]=i,a===j)break}catch(k){}}return a?e[a]:e}}return e.set=f,e.get=function(a){return g(a,!1)},e.getJSON=function(a){return g(a,!0)},e.remove=function(b,c){f(b,"",a(c,{expires:-1}))},e.defaults={},e.withConverter=c,e}return c(function(){})});
/*!
 * IE10 viewport hack for Surface/desktop Windows 8 bug
 * Copyright 2014 Twitter, Inc.
 * Licensed under the Creative Commons Attribution 3.0 Unported License. For
 * details, see http://creativecommons.org/licenses/by/3.0/.
 */

// See the Getting Started docs for more information:
// http://getbootstrap.com/getting-started/#support-ie10-width

(function () {
  'use strict';
  if (navigator.userAgent.match(/IEMobile\/10\.0/)) {
    var msViewportStyle = document.createElement('style')
    msViewportStyle.appendChild(
      document.createTextNode(
        '@-ms-viewport{width:auto!important}'
      )
    )
    document.querySelector('head').appendChild(msViewportStyle)
  }
})();

window.AddAntiForgeryToken = function () {
    return { "__RequestVerificationToken": $('input[name="__RequestVerificationToken').val() };
};

;(function(root, factory) {
  if (typeof define === 'function' && define.amd) {
    define(['jquery'], factory);
  } else if (typeof exports === 'object') {
    module.exports = factory(require('jquery'));
  } else {
    root.jquery_mmenu_all_js = factory(root.jQuery);
  }
}(this, function(jQuery) {
/*!
 * jQuery mmenu v7.2.2
 * @requires jQuery 1.7.0 or later
 *
 * mmenu.frebsite.nl
 *
 * Copyright (c) Fred Heusschen
 * www.frebsite.nl
 *
 * License: CC-BY-NC-4.0
 * http://creativecommons.org/licenses/by-nc/4.0/
 */
!function(h){var n,p,o,a,t,f="mmenu",e="7.2.2";h[f]&&h[f].version>e||(h[f]=function(t,e,n){return this.$menu=t,this._api=["bind","getInstance","initPanels","openPanel","closePanel","closeAllPanels","setSelected"],this.opts=e,this.conf=n,this.vars={},this.cbck={},this.mtch={},"function"==typeof this.___deprecated&&this.___deprecated(),this._initWrappers(),this._initAddons(),this._initExtensions(),this._initHooks(),this._initMenu(),this._initPanels(),this._initOpened(),this._initAnchors(),this._initMatchMedia(),"function"==typeof this.___debug&&this.___debug(),this},h[f].version=e,h[f].uniqueId=0,h[f].wrappers={},h[f].addons={},h[f].defaults={hooks:{},extensions:[],wrappers:[],navbar:{add:!0,title:"Menu",titleLink:"parent"},onClick:{setSelected:!0},slidingSubmenus:!0},h[f].configuration={classNames:{divider:"Divider",inset:"Inset",nolistview:"NoListview",nopanel:"NoPanel",panel:"Panel",selected:"Selected",spacer:"Spacer",vertical:"Vertical"},clone:!1,language:null,openingInterval:25,panelNodetype:"ul, ol, div",transitionDuration:400},h[f].prototype={getInstance:function(){return this},initPanels:function(t){this._initPanels(t)},openPanel:function(t,e){if(this.trigger("openPanel:before",t),t&&t.length&&(t.is("."+p.panel)||(t=t.closest("."+p.panel)),t.is("."+p.panel))){var n=this;if("boolean"!=typeof e&&(e=!0),t.parent("."+p.listitem+"_vertical").length)t.parents("."+p.listitem+"_vertical").addClass(p.listitem+"_opened").children("."+p.panel).removeClass(p.hidden),this.openPanel(t.parents("."+p.panel).not(function(){return h(this).parent("."+p.listitem+"_vertical").length}).first()),this.trigger("openPanel:start",t),this.trigger("openPanel:finish",t);else{if(t.hasClass(p.panel+"_opened"))return;var i=this.$pnls.children("."+p.panel),s=this.$pnls.children("."+p.panel+"_opened");if(!h[f].support.csstransitions)return s.addClass(p.hidden).removeClass(p.panel+"_opened"),t.removeClass(p.hidden).addClass(p.panel+"_opened"),this.trigger("openPanel:start",t),void this.trigger("openPanel:finish",t);i.not(t).removeClass(p.panel+"_opened-parent");for(var a=t.data(o.parent);a;)(a=a.closest("."+p.panel)).parent("."+p.listitem+"_vertical").length||a.addClass(p.panel+"_opened-parent"),a=a.data(o.parent);i.removeClass(p.panel+"_highest").not(s).not(t).addClass(p.hidden),t.removeClass(p.hidden);var r=function(){s.removeClass(p.panel+"_opened"),t.addClass(p.panel+"_opened"),t.hasClass(p.panel+"_opened-parent")?(s.addClass(p.panel+"_highest"),t.removeClass(p.panel+"_opened-parent")):(s.addClass(p.panel+"_opened-parent"),t.addClass(p.panel+"_highest")),n.trigger("openPanel:start",t)},l=function(){s.removeClass(p.panel+"_highest").addClass(p.hidden),t.removeClass(p.panel+"_highest"),n.trigger("openPanel:finish",t)};e&&!t.hasClass(p.panel+"_noanimation")?setTimeout(function(){n.__transitionend(t,function(){l()},n.conf.transitionDuration),r()},n.conf.openingInterval):(r(),l())}this.trigger("openPanel:after",t)}},closePanel:function(t){this.trigger("closePanel:before",t);var e=t.parent();e.hasClass(p.listitem+"_vertical")&&(e.removeClass(p.listitem+"_opened"),t.addClass(p.hidden),this.trigger("closePanel",t)),this.trigger("closePanel:after",t)},closeAllPanels:function(t){this.trigger("closeAllPanels:before"),this.$pnls.find("."+p.listview).children().removeClass(p.listitem+"_selected").filter("."+p.listitem+"_vertical").removeClass(p.listitem+"_opened");var e=this.$pnls.children("."+p.panel),n=t&&t.length?t:e.first();this.$pnls.children("."+p.panel).not(n).removeClass(p.panel+"_opened").removeClass(p.panel+"_opened-parent").removeClass(p.panel+"_highest").addClass(p.hidden),this.openPanel(n,!1),this.trigger("closeAllPanels:after")},togglePanel:function(t){var e=t.parent();e.hasClass(p.listitem+"_vertical")&&this[e.hasClass(p.listitem+"_opened")?"closePanel":"openPanel"](t)},setSelected:function(t){this.trigger("setSelected:before",t),this.$menu.find("."+p.listitem+"_selected").removeClass(p.listitem+"_selected"),t.addClass(p.listitem+"_selected"),this.trigger("setSelected:after",t)},bind:function(t,e){this.cbck[t]=this.cbck[t]||[],this.cbck[t].push(e)},trigger:function(){var t=Array.prototype.slice.call(arguments),e=t.shift();if(this.cbck[e])for(var n=0,i=this.cbck[e].length;n<i;n++)this.cbck[e][n].apply(this,t)},matchMedia:function(t,e,n){var i={yes:e,no:n};this.mtch[t]=this.mtch[t]||[],this.mtch[t].push(i)},i18n:function(t){return h[f].i18n(t,this.conf.language)},_initHooks:function(){for(var t in this.opts.hooks)this.bind(t,this.opts.hooks[t])},_initWrappers:function(){this.trigger("initWrappers:before");for(var t=0;t<this.opts.wrappers.length;t++){var e=h[f].wrappers[this.opts.wrappers[t]];"function"==typeof e&&e.call(this)}this.trigger("initWrappers:after")},_initAddons:function(){var t;for(t in this.trigger("initAddons:before"),h[f].addons)h[f].addons[t].add.call(this),h[f].addons[t].add=function(){};for(t in h[f].addons)h[f].addons[t].setup.call(this);this.trigger("initAddons:after")},_initExtensions:function(){this.trigger("initExtensions:before");var e=this;for(var t in this.opts.extensions.constructor===Array&&(this.opts.extensions={all:this.opts.extensions}),this.opts.extensions)this.opts.extensions[t]=this.opts.extensions[t].length?p.menu+"_"+this.opts.extensions[t].join(" "+p.menu+"_"):"",this.opts.extensions[t]&&function(t){e.matchMedia(t,function(){this.$menu.addClass(this.opts.extensions[t])},function(){this.$menu.removeClass(this.opts.extensions[t])})}(t);this.trigger("initExtensions:after")},_initMenu:function(){this.trigger("initMenu:before");this.conf.clone&&(this.$orig=this.$menu,this.$menu=this.$orig.clone(),this.$menu.add(this.$menu.find("[id]")).filter("[id]").each(function(){h(this).attr("id",p.mm(h(this).attr("id")))})),this.$menu.attr("id",this.$menu.attr("id")||this.__getUniqueId()),this.$pnls=h('<div class="'+p.panels+'" />').append(this.$menu.children(this.conf.panelNodetype)).prependTo(this.$menu),this.$menu.addClass(p.menu).parent().addClass(p.wrapper),this.trigger("initMenu:after")},_initPanels:function(t){this.trigger("initPanels:before",t),t=t||this.$pnls.children(this.conf.panelNodetype);var i=h(),s=this,a=function(t){t.filter(s.conf.panelNodetype).each(function(t){var e=s._initPanel(h(this));if(e){s._initNavbar(e),s._initListview(e),i=i.add(e);var n=e.children("."+p.listview).children("li").children(s.conf.panelNodetype).add(e.children("."+s.conf.classNames.panel));n.length&&a(n)}})};a(t),this.trigger("initPanels:after",i)},_initPanel:function(t){this.trigger("initPanel:before",t);if(t.hasClass(p.panel))return t;if(this.__refactorClass(t,this.conf.classNames.panel,p.panel),this.__refactorClass(t,this.conf.classNames.nopanel,p.nopanel),this.__refactorClass(t,this.conf.classNames.inset,p.listview+"_inset"),t.filter("."+p.listview+"_inset").addClass(p.nopanel),t.hasClass(p.nopanel))return!1;var e=t.hasClass(this.conf.classNames.vertical)||!this.opts.slidingSubmenus;t.removeClass(this.conf.classNames.vertical);var n=t.attr("id")||this.__getUniqueId();t.is("ul, ol")&&(t.removeAttr("id"),t.wrap("<div />"),t=t.parent()),t.attr("id",n),t.addClass(p.panel+" "+p.hidden);var i=t.parent("li");return e?i.addClass(p.listitem+"_vertical"):t.appendTo(this.$pnls),i.length&&(i.data(o.child,t),t.data(o.parent,i)),this.trigger("initPanel:after",t),t},_initNavbar:function(t){if(this.trigger("initNavbar:before",t),!t.children("."+p.navbar).length){var e=t.data(o.parent),n=h('<div class="'+p.navbar+'" />'),i=this.__getPanelTitle(t,this.opts.navbar.title),s="";if(e&&e.length){if(e.hasClass(p.listitem+"_vertical"))return;if(e.parent().is("."+p.listview))var a=e.children("a, span").not("."+p.btn+"_next");else a=e.closest("."+p.panel).find('a[href="#'+t.attr("id")+'"]');var r=(e=(a=a.first()).closest("."+p.panel)).attr("id");switch(i=this.__getPanelTitle(t,h("<span>"+a.text()+"</span>").text()),this.opts.navbar.titleLink){case"anchor":s=a.attr("href");break;case"parent":s="#"+r}n.append('<a class="'+p.btn+" "+p.btn+"_prev "+p.navbar+'__btn" href="#'+r+'" />')}else if(!this.opts.navbar.title)return;this.opts.navbar.add&&t.addClass(p.panel+"_has-navbar"),n.append('<a class="'+p.navbar+'__title"'+(s.length?' href="'+s+'"':"")+">"+i+"</a>").prependTo(t),this.trigger("initNavbar:after",t)}},_initListview:function(t){this.trigger("initListview:before",t);var e=this.__childAddBack(t,"ul, ol");this.__refactorClass(e,this.conf.classNames.nolistview,p.nolistview);var n=e.not("."+p.nolistview).addClass(p.listview).children().addClass(p.listitem);this.__refactorClass(n,this.conf.classNames.selected,p.listitem+"_selected"),this.__refactorClass(n,this.conf.classNames.divider,p.listitem+"_divider"),this.__refactorClass(n,this.conf.classNames.spacer,p.listitem+"_spacer"),n.children("a, span").not("."+p.btn).addClass(p.listitem+"__text");var i=t.data(o.parent);if(i&&i.is("."+p.listitem)&&!i.children("."+p.btn).length){var s=i.children("a, span").first(),a=h('<a class="'+p.btn+" "+p.btn+"_next "+p.listitem+'__btn" href="#'+t.attr("id")+'" />');a.insertAfter(s),s.is("span")&&(a.addClass(p.listitem+"__text").html(s.html()),s.remove())}this.trigger("initListview:after",t)},_initOpened:function(){this.trigger("initOpened:before");var t=this.$pnls.find("."+p.listitem+"_selected").removeClass(p.listitem+"_selected").last().addClass(p.listitem+"_selected"),e=t.length?t.closest("."+p.panel):this.$pnls.children("."+p.panel).first();this.openPanel(e,!1),this.trigger("initOpened:after")},_initAnchors:function(){this.trigger("initAnchors:before");var c=this;t.$body.on(a.click+"-oncanvas","a[href]",function(t){var e=h(this),n=e.attr("href"),i=c.$menu.find(e).length,s=e.is("."+p.listitem+" > a"),a=e.is('[rel="external"]')||e.is('[target="_blank"]');if(i&&1<n.length&&"#"==n.slice(0,1))try{var r=c.$menu.find(n);if(r.is("."+p.panel))return c[e.parent().hasClass(p.listitem+"_vertical")?"togglePanel":"openPanel"](r),void t.preventDefault()}catch(t){}var l={close:null,setSelected:null,preventDefault:"#"==n.slice(0,1)};for(var o in h[f].addons){var d=h[f].addons[o].clickAnchor.call(c,e,i,s,a);if(d){if("boolean"==typeof d)return void t.preventDefault();"object"==typeof d&&(l=h.extend({},l,d))}}i&&s&&!a&&(c.__valueOrFn(e,c.opts.onClick.setSelected,l.setSelected)&&c.setSelected(h(t.target).parent()),c.__valueOrFn(e,c.opts.onClick.preventDefault,l.preventDefault)&&t.preventDefault(),c.__valueOrFn(e,c.opts.onClick.close,l.close)&&c.opts.offCanvas&&"function"==typeof c.close&&c.close())}),this.trigger("initAnchors:after")},_initMatchMedia:function(){var n=this;for(var i in this.mtch)!function(){var e=i,t=window.matchMedia(e);n._fireMatchMedia(e,t),t.addListener(function(t){n._fireMatchMedia(e,t)})}()},_fireMatchMedia:function(t,e){for(var n=e.matches?"yes":"no",i=0;i<this.mtch[t].length;i++)this.mtch[t][i][n].call(this)},_getOriginalMenuId:function(){var t=this.$menu.attr("id");return this.conf.clone&&t&&t.length&&(t=p.umm(t)),t},__api:function(){var n=this,i={};return h.each(this._api,function(t){var e=this;i[e]=function(){var t=n[e].apply(n,arguments);return void 0===t?i:t}}),i},__valueOrFn:function(t,e,n){if("function"==typeof e){var i=e.call(t[0]);if(void 0!==i)return i}return"function"!=typeof e&&void 0!==e||void 0===n?e:n},__getPanelTitle:function(t,e){var n;return"function"==typeof this.opts.navbar.title&&(n=this.opts.navbar.title.call(t[0])),void 0===n&&(n=t.data(o.title)),void 0!==n?n:"string"==typeof e?this.i18n(e):this.i18n(h[f].defaults.navbar.title)},__refactorClass:function(t,e,n){return t.filter("."+e).removeClass(e).addClass(n)},__findAddBack:function(t,e){return t.find(e).add(t.filter(e))},__childAddBack:function(t,e){return t.children(e).add(t.filter(e))},__filterListItems:function(t){return t.not("."+p.listitem+"_divider").not("."+p.hidden)},__filterListItemAnchors:function(t){return this.__filterListItems(t).children("a").not("."+p.btn+"_next")},__openPanelWoAnimation:function(t){t.hasClass(p.panel+"_noanimation")||(t.addClass(p.panel+"_noanimation"),this.__transitionend(t,function(){t.removeClass(p.panel+"_noanimation")},this.conf.openingInterval),this.openPanel(t))},__transitionend:function(e,n,t){var i=!1,s=function(t){void 0!==t&&t.target!=e[0]||(i||(e.off(a.transitionend),e.off(a.webkitTransitionEnd),n.call(e[0])),i=!0)};e.on(a.transitionend,s),e.on(a.webkitTransitionEnd,s),setTimeout(s,1.1*t)},__getUniqueId:function(){return p.mm(h[f].uniqueId++)}},h.fn[f]=function(n,i){!function(){if(h[f].glbl)return;t={$wndw:h(window),$docu:h(document),$html:h("html"),$body:h("body")},p={},o={},a={},h.each([p,o,a],function(t,i){i.add=function(t){t=t.split(" ");for(var e=0,n=t.length;e<n;e++)i[t[e]]=i.mm(t[e])}}),p.mm=function(t){return"mm-"+t},p.add("wrapper menu panels panel nopanel navbar listview nolistview listitem btn hidden"),p.umm=function(t){return"mm-"==t.slice(0,3)&&(t=t.slice(3)),t},o.mm=function(t){return"mm-"+t},o.add("parent child title"),a.mm=function(t){return t+".mm"},a.add("transitionend webkitTransitionEnd click scroll resize keydown mousedown mouseup touchstart touchmove touchend orientationchange"),h[f]._c=p,h[f]._d=o,h[f]._e=a,h[f].glbl=t}(),n=h.extend(!0,{},h[f].defaults,n),i=h.extend(!0,{},h[f].configuration,i);var s=h();return this.each(function(){var t=h(this);if(!t.data(f)){var e=new h[f](t,n,i);e.$menu.data(f,e.__api()),s=s.add(e.$menu)}}),s},h[f].i18n=(n={},function(t,e){switch(typeof t){case"object":return"string"==typeof e&&(void 0===n[e]&&(n[e]={}),h.extend(n[e],t)),n;case"string":return"string"==typeof e&&void 0!==n[e]&&n[e][t]||t;case"undefined":default:return n}}),h[f].support={touch:"ontouchstart"in window||navigator.msMaxTouchPoints||!1,csstransitions:"undefined"==typeof Modernizr||void 0===Modernizr.csstransitions||Modernizr.csstransitions})}(jQuery);
!function(r){var s,i,o,a,t="mmenu",p="offCanvas";r[t].addons[p]={setup:function(){if(this.opts[p]){var e=this.opts[p],i=this.conf[p];a=r[t].glbl,this._api=r.merge(this._api,["open","close","setPage"]),"object"!=typeof e&&(e={}),e=this.opts[p]=r.extend(!0,{},r[t].defaults[p],e),"string"!=typeof i.page.selector&&(i.page.selector="> "+i.page.nodetype),this.vars.opened=!1;var o=[s.menu+"_offcanvas"];this.bind("initMenu:after",function(){var e=this;this._initBlocker(),this.setPage(a.$page),this["_initWindow_"+p](),this.$menu.addClass(o.join(" ")).parent("."+s.wrapper).removeClass(s.wrapper),this.$menu[i.menu.insertMethod](i.menu.insertSelector);var t=window.location.hash;if(t){var n=this._getOriginalMenuId();n&&n==t.slice(1)&&setTimeout(function(){e.open()},1e3)}}),this.bind("setPage:after",function(e){a.$blck&&a.$blck.children("a").attr("href","#"+e.attr("id"))}),this.bind("open:start:sr-aria",function(){this.__sr_aria(this.$menu,"hidden",!1)}),this.bind("close:finish:sr-aria",function(){this.__sr_aria(this.$menu,"hidden",!0)}),this.bind("initMenu:after:sr-aria",function(){this.__sr_aria(this.$menu,"hidden",!0)}),this.bind("initBlocker:after:sr-text",function(){a.$blck.children("a").html(this.__sr_text(this.i18n(this.conf.screenReader.text.closeMenu)))})}},add:function(){s=r[t]._c,i=r[t]._d,o=r[t]._e,s.add("slideout page no-csstransforms3d"),i.add("style")},clickAnchor:function(e,t){var n=this;if(this.opts[p]){var i=this._getOriginalMenuId();if(i&&e.is('[href="#'+i+'"]')){if(t)return this.open(),!0;var o=e.closest("."+s.menu);if(o.length){var r=o.data("mmenu");if(r&&r.close)return r.close(),n.__transitionend(o,function(){n.open()},n.conf.transitionDuration),!0}return this.open(),!0}if(a.$page)return(i=a.$page.first().attr("id"))&&e.is('[href="#'+i+'"]')?(this.close(),!0):void 0}}},r[t].defaults[p]={blockUI:!0,moveBackground:!0},r[t].configuration[p]={menu:{insertMethod:"prependTo",insertSelector:"body"},page:{nodetype:"div",selector:null,noSelector:[],wrapIfNeeded:!0}},r[t].prototype.open=function(){if(this.trigger("open:before"),!this.vars.opened){var e=this;this._openSetup(),setTimeout(function(){e._openFinish()},this.conf.openingInterval),this.trigger("open:after")}},r[t].prototype._openSetup=function(){var e=this,t=this.opts[p];this.closeAllOthers(),a.$page.each(function(){r(this).data(i.style,r(this).attr("style")||"")}),a.$wndw.trigger(o.resize+"-"+p,[!0]);var n=[s.wrapper+"_opened"];t.blockUI&&n.push(s.wrapper+"_blocking"),"modal"==t.blockUI&&n.push(s.wrapper+"_modal"),t.moveBackground&&n.push(s.wrapper+"_background"),a.$html.addClass(n.join(" ")),setTimeout(function(){e.vars.opened=!0},this.conf.openingInterval),this.$menu.addClass(s.menu+"_opened")},r[t].prototype._openFinish=function(){var e=this;this.__transitionend(a.$page.first(),function(){e.trigger("open:finish")},this.conf.transitionDuration),this.trigger("open:start"),a.$html.addClass(s.wrapper+"_opening")},r[t].prototype.close=function(){if(this.trigger("close:before"),this.vars.opened){var t=this;this.__transitionend(a.$page.first(),function(){t.$menu.removeClass(s.menu+"_opened");var e=[s.wrapper+"_opened",s.wrapper+"_blocking",s.wrapper+"_modal",s.wrapper+"_background"];a.$html.removeClass(e.join(" ")),a.$page.each(function(){var e=r(this).data(i.style);r(this).attr("style",e)}),t.vars.opened=!1,t.trigger("close:finish")},this.conf.transitionDuration),this.trigger("close:start"),a.$html.removeClass(s.wrapper+"_opening"),this.trigger("close:after")}},r[t].prototype.closeAllOthers=function(){a.$body.find("."+s.menu+"_offcanvas").not(this.$menu).each(function(){var e=r(this).data(t);e&&e.close&&e.close()})},r[t].prototype.setPage=function(e){this.trigger("setPage:before",e);var t=this,n=this.conf[p];e&&e.length||(e=a.$body.find(n.page.selector).not("."+s.menu).not("."+s.wrapper+"__blocker"),n.page.noSelector.length&&(e=e.not(n.page.noSelector.join(", "))),1<e.length&&n.page.wrapIfNeeded&&(e=e.wrapAll("<"+this.conf[p].page.nodetype+" />").parent())),e.addClass(s.page+" "+s.slideout).each(function(){r(this).attr("id",r(this).attr("id")||t.__getUniqueId())}),a.$page=e,this.trigger("setPage:after",e)},r[t].prototype["_initWindow_"+p]=function(){a.$wndw.off(o.keydown+"-"+p).on(o.keydown+"-"+p,function(e){if(a.$html.hasClass(s.wrapper+"_opened")&&9==e.keyCode)return e.preventDefault(),!1});var i=0;a.$wndw.off(o.resize+"-"+p).on(o.resize+"-"+p,function(e,t){if(1==a.$page.length&&(t||a.$html.hasClass(s.wrapper+"_opened"))){var n=a.$wndw.height();(t||n!=i)&&(i=n,a.$page.css("minHeight",n))}})},r[t].prototype._initBlocker=function(){var t=this,e=this.opts[p],n=this.conf[p];this.trigger("initBlocker:before"),e.blockUI&&(a.$blck||(a.$blck=r('<div class="'+s.wrapper+"__blocker "+s.slideout+'" />').append("<a />")),a.$blck.appendTo(n.menu.insertSelector).off(o.touchstart+"-"+p+" "+o.touchmove+"-"+p).on(o.touchstart+"-"+p+" "+o.touchmove+"-"+p,function(e){e.preventDefault(),e.stopPropagation(),a.$blck.trigger(o.mousedown+"-"+p)}).off(o.mousedown+"-"+p).on(o.mousedown+"-"+p,function(e){e.preventDefault(),a.$html.hasClass(s.wrapper+"_modal")||(t.closeAllOthers(),t.close())}),this.trigger("initBlocker:after"))}}(jQuery);
!function(n){var s,o,i="mmenu",e="screenReader";n[i].addons[e]={setup:function(){var r=this,t=this.opts[e],a=this.conf[e];n[i].glbl,"boolean"==typeof t&&(t={aria:t,text:t}),"object"!=typeof t&&(t={}),(t=this.opts[e]=n.extend(!0,{},n[i].defaults[e],t)).aria&&(this.bind("initAddons:after",function(){this.bind("initMenu:after",function(){this.trigger("initMenu:after:sr-aria")}),this.bind("initNavbar:after",function(){this.trigger("initNavbar:after:sr-aria",arguments[0])}),this.bind("openPanel:start",function(){this.trigger("openPanel:start:sr-aria",arguments[0])}),this.bind("close:start",function(){this.trigger("close:start:sr-aria")}),this.bind("close:finish",function(){this.trigger("close:finish:sr-aria")}),this.bind("open:start",function(){this.trigger("open:start:sr-aria")}),this.bind("initOpened:after",function(){this.trigger("initOpened:after:sr-aria")})}),this.bind("updateListview",function(){this.$pnls.find("."+s.listview).children().each(function(){r.__sr_aria(n(this),"hidden",n(this).is("."+s.hidden))})}),this.bind("openPanel:start",function(t){var i=this.$menu.find("."+s.panel).not(t).not(t.parents("."+s.panel)),n=t.add(t.find("."+s.listitem+"_vertical ."+s.listitem+"_opened").children("."+s.panel));this.__sr_aria(i,"hidden",!0),this.__sr_aria(n,"hidden",!1)}),this.bind("closePanel",function(t){this.__sr_aria(t,"hidden",!0)}),this.bind("initPanels:after",function(t){var i=t.find("."+s.btn).each(function(){r.__sr_aria(n(this),"owns",n(this).attr("href").replace("#",""))});this.__sr_aria(i,"haspopup",!0)}),this.bind("initNavbar:after",function(t){var i=t.children("."+s.navbar);this.__sr_aria(i,"hidden",!t.hasClass(s.panel+"_has-navbar"))}),t.text&&"parent"==this.opts.navbar.titleLink&&this.bind("initNavbar:after",function(t){var i=t.children("."+s.navbar),n=!!i.children("."+s.btn+"_prev").length;this.__sr_aria(i.children("."+s.title),"hidden",n)})),t.text&&(this.bind("initAddons:after",function(){this.bind("setPage:after",function(){this.trigger("setPage:after:sr-text",arguments[0])}),this.bind("initBlocker:after",function(){this.trigger("initBlocker:after:sr-text")})}),this.bind("initNavbar:after",function(t){var i=t.children("."+s.navbar),n=this.i18n(a.text.closeSubmenu);i.children("."+s.btn+"_prev").html(this.__sr_text(n))}),this.bind("initListview:after",function(t){var i=t.data(o.parent);if(i&&i.length){var n=i.children("."+s.btn+"_next"),e=this.i18n(a.text[n.parent().is("."+s.listitem+"_vertical")?"toggleSubmenu":"openSubmenu"]);n.append(r.__sr_text(e))}}))},add:function(){s=n[i]._c,o=n[i]._d,n[i]._e,s.add("sronly")},clickAnchor:function(t,i){}},n[i].defaults[e]={aria:!0,text:!0},n[i].configuration[e]={text:{closeMenu:"Close menu",closeSubmenu:"Close submenu",openSubmenu:"Open submenu",toggleSubmenu:"Toggle submenu"}},n[i].prototype.__sr_aria=function(t,i,n){t.prop("aria-"+i,n)[n?"attr":"removeAttr"]("aria-"+i,n)},n[i].prototype.__sr_role=function(t,i){t.prop("role",i)[i?"attr":"removeAttr"]("role",i)},n[i].prototype.__sr_text=function(t){return'<span class="'+s.sronly+'">'+t+"</span>"}}(jQuery);
!function(n){var e,r,s,t="mmenu",i="scrollBugFix";n[t].addons[i]={setup:function(){var o=this.opts[i];this.conf[i];s=n[t].glbl,n[t].support.touch&&this.opts.offCanvas&&this.opts.offCanvas.blockUI&&("boolean"==typeof o&&(o={fix:o}),"object"!=typeof o&&(o={}),(o=this.opts[i]=n.extend(!0,{},n[t].defaults[i],o)).fix&&(this.bind("open:start",function(){this.$pnls.children("."+e.panel+"_opened").scrollTop(0)}),this.bind("initMenu:after",function(){this["_initWindow_"+i]()})))},add:function(){e=n[t]._c,n[t]._d,r=n[t]._e},clickAnchor:function(o,t){}},n[t].defaults[i]={fix:!0},n[t].prototype["_initWindow_"+i]=function(){var o=this;n(document).off(r.touchmove+"-"+i).on(r.touchmove+"-"+i,function(o){s.$html.hasClass(e.wrapper+"_opened")&&o.preventDefault()});var t=!1;s.$body.off(r.touchstart+"-"+i).on(r.touchstart+"-"+i,"."+e.panels+"> ."+e.panel,function(o){s.$html.hasClass(e.wrapper+"_opened")&&(t||(t=!0,0===o.currentTarget.scrollTop?o.currentTarget.scrollTop=1:o.currentTarget.scrollHeight===o.currentTarget.scrollTop+o.currentTarget.offsetHeight&&(o.currentTarget.scrollTop-=1),t=!1))}).off(r.touchmove+"-"+i).on(r.touchmove+"-"+i,"."+e.panels+"> ."+e.panel,function(o){s.$html.hasClass(e.wrapper+"_opened")&&n(this)[0].scrollHeight>n(this).innerHeight()&&o.stopPropagation()}),s.$wndw.off(r.orientationchange+"-"+i).on(r.orientationchange+"-"+i,function(){o.$pnls.children("."+e.panel+"_opened").scrollTop(0).css({"-webkit-overflow-scrolling":"auto"}).css({"-webkit-overflow-scrolling":"touch"})})}}(jQuery);
!function(s){var a,e="mmenu",i="autoHeight";s[e].addons[i]={setup:function(){var h=this.opts[i];this.conf[i];if(s[e].glbl,"boolean"==typeof h&&h&&(h={height:"auto"}),"string"==typeof h&&(h={height:h}),"object"!=typeof h&&(h={}),"auto"==(h=this.opts[i]=s.extend(!0,{},s[e].defaults[i],h)).height||"highest"==h.height){this.bind("initMenu:after",function(){this.$menu.addClass(a.menu+"_autoheight")});var t=function(t){if(!this.opts.offCanvas||this.vars.opened){var e=Math.max(parseInt(this.$pnls.css("top"),10),0)||0,i=Math.max(parseInt(this.$pnls.css("bottom"),10),0)||0,n=0;this.$menu.addClass(a.menu+"_autoheight-measuring"),"auto"==h.height?((t=t||this.$pnls.children("."+a.panel+"_opened")).parent("."+a.listitem+"_vertical").length&&(t=t.parents("."+a.panel).not(function(){return s(this).parent("."+a.listitem+"_vertical").length})),t.length||(t=this.$pnls.children("."+a.panel)),n=t.first().outerHeight()):"highest"==h.height&&this.$pnls.children("."+a.panel).each(function(){var t=s(this);t.parent("."+a.listitem+"_vertical").length&&(t=t.parents("."+a.panel).not(function(){return s(this).parent("."+a.listitem+"_vertical").length})),n=Math.max(n,t.first().outerHeight())}),this.$menu.height(n+e+i).removeClass(a.menu+"_autoheight-measuring")}};this.opts.offCanvas&&this.bind("open:start",t),"highest"==h.height&&this.bind("initPanels:after",t),"auto"==h.height&&(this.bind("updateListview",t),this.bind("openPanel:start",t),this.bind("closePanel",t))}},add:function(){a=s[e]._c,s[e]._d,s[e]._e.add("resize")},clickAnchor:function(t,e){}},s[e].defaults[i]={height:"default"}}(jQuery);
!function(s){var a,h="mmenu",c="backButton";s[h].addons[c]={setup:function(){if(this.opts.offCanvas){var o=this,n=this.opts[c];this.conf[c];s[h].glbl,"boolean"==typeof n&&(n={close:n}),"object"!=typeof n&&(n={}),n=s.extend(!0,{},s[h].defaults[c],n);var e="#"+this.$menu.attr("id");if(n.close){var i=[];function t(){i=[e],this.$pnls.children("."+a.panel+"_opened-parent").add(o.$pnls.children("."+a.panel+"_opened")).each(function(){i.push("#"+s(this).attr("id"))})}this.bind("open:finish",function(){history.pushState(null,document.title,e)}),this.bind("open:finish",t),this.bind("openPanel:finish",t),this.bind("close:finish",function(){i=[],history.back(),history.pushState(null,document.title,location.pathname+location.search)}),s(window).on("popstate",function(n){if(o.vars.opened&&i.length){var t=(i=i.slice(0,-1))[i.length-1];t==e?o.close():(o.openPanel(s(t)),history.pushState(null,document.title,e))}})}n.open&&s(window).on("popstate",function(n){o.vars.opened||location.hash!=e||o.open()})}},add:function(){window.history&&window.history.pushState?(a=s[h]._c,s[h]._d,s[h]._e):s[h].addons[c].setup=function(){}},clickAnchor:function(n,t){}},s[h].defaults[c]={close:!1,open:!1}}(jQuery);
!function(t){var o,d,n="mmenu",p="columns";t[n].addons[p]={setup:function(){var i=this.opts[p];this.conf[p];if(t[n].glbl,"boolean"==typeof i&&(i={add:i}),"number"==typeof i&&(i={add:!0,visible:i}),"object"!=typeof i&&(i={}),"number"==typeof i.visible&&(i.visible={min:i.visible,max:i.visible}),(i=this.opts[p]=t.extend(!0,{},t[n].defaults[p],i)).add){i.visible.min=Math.max(1,Math.min(6,i.visible.min)),i.visible.max=Math.max(i.visible.min,Math.min(6,i.visible.max));for(var a="",s="",e=0;e<=i.visible.max;e++)a+=" "+o.menu+"_columns-"+e,s+=" "+o.panel+"_columns-"+e;a.length&&(a=a.slice(1),s=s.slice(1));var l=s+" "+o.panel+"_opened "+o.panel+"_opened-parent "+o.panel+"_highest";this.bind("openPanel:before",function(e){var n=e.data(d.parent);if(n&&(n=n.closest("."+o.panel)).length){var i=n.attr("class");if(i&&(i=i.split(o.panel+"_columns-")[1]))for(i=parseInt(i.split(" ")[0],10)+1;0<i;){var a=this.$pnls.children("."+o.panel+"_columns-"+i);if(!a.length){i=-1;break}i++,a.removeClass(l).addClass(o.hidden)}}}),this.bind("openPanel:start",function(e){var n=this.$pnls.children("."+o.panel+"_opened-parent").length;e.hasClass(o.panel+"_opened-parent")||n++,n=Math.min(i.visible.max,Math.max(i.visible.min,n)),this.$menu.removeClass(a).addClass(o.menu+"_columns-"+n),this.$pnls.children("."+o.panel).removeClass(s).filter("."+o.panel+"_opened-parent").add(e).slice(-i.visible.max).each(function(e){t(this).addClass(o.panel+"_columns-"+e)})})}},add:function(){o=t[n]._c,d=t[n]._d,t[n]._e},clickAnchor:function(e,n){}},t[n].defaults[p]={add:!1,visible:{min:1,max:3}}}(jQuery);
!function(a){var s,d,n="mmenu",c="counters";a[n].addons[c]={setup:function(){var i=this,e=this.opts[c];this.conf[c];if(a[n].glbl,"boolean"==typeof e&&(e={add:e,update:e}),"object"!=typeof e&&(e={}),e=this.opts[c]=a.extend(!0,{},a[n].defaults[c],e),this.bind("initListview:after",function(t){var n=this.conf.classNames[c].counter;this.__refactorClass(t.find("."+n),n,s.counter)}),e.add&&this.bind("initListview:after",function(t){var n;switch(e.addTo){case"panels":n=t;break;default:n=t.filter(e.addTo)}n.each(function(){var t=a(this).data(d.parent);t&&(t.find("."+s.counter).length||t.children("."+s.btn).prepend(a('<span class="'+s.counter+'" />')))})}),e.update){var t=function(t){(t=t||this.$pnls.children("."+s.panel)).each(function(){var t=a(this),n=t.data(d.parent);if(n){var e=n.find("."+s.counter);e.length&&(t=t.children("."+s.listview)).length&&e.html(i.__filterListItems(t.children()).length)}})};this.bind("initListview:after",t),this.bind("updateListview",t)}},add:function(){s=a[n]._c,d=a[n]._d,a[n]._e,s.add("counter")},clickAnchor:function(t,n){}},a[n].defaults[c]={add:!1,addTo:"panels",count:!1},a[n].configuration.classNames[c]={counter:"Counter"}}(jQuery);
!function(d){var l,i,t="mmenu",a="dividers";d[t].addons[a]={setup:function(){var e=this,n=this.opts[a];this.conf[a];if(d[t].glbl,"boolean"==typeof n&&(n={add:n,fixed:n}),"object"!=typeof n&&(n={}),(n=this.opts[a]=d.extend(!0,{},d[t].defaults[a],n)).type&&this.bind("initMenu:after",function(){this.$menu.addClass(l.menu+"_"+a+"-"+n.type)}),n.add&&this.bind("initListview:after",function(i){var t;switch(n.addTo){case"panels":t=i;break;default:t=i.filter(n.addTo)}t.length&&(t.children("."+l.listitem+"_divider").remove(),t.find("."+l.listview).each(function(){var t="";e.__filterListItems(d(this).children()).each(function(){var i=d.trim(d(this).children("a, span").text()).slice(0,1).toLowerCase();i!=t&&i.length&&(t=i,d('<li class="'+l.listitem+" "+l.listitem+'_divider">'+i+"</li>").insertBefore(this))})}))}),n.fixed){this.bind("initPanels:after",function(){void 0===this.$fixeddivider&&(this.$fixeddivider=d('<ul class="'+l.listview+" "+l.listview+'_fixeddivider"><li class="'+l.listitem+" "+l.listitem+'_divider"></li></ul>').appendTo(this.$pnls).children())});var s=function(i){if(!(i=i||this.$pnls.children("."+l.panel+"_opened")).is(":hidden")){var t=i.find("."+l.listitem+"_divider").not("."+l.hidden),e=i.scrollTop()||0,n="";t.each(function(){d(this).position().top+e<e+1&&(n=d(this).text())}),this.$fixeddivider.text(n),this.$pnls[n.length?"addClass":"removeClass"](l.panel+"_dividers")}};this.bind("open:start",s),this.bind("openPanel:start",s),this.bind("updateListview",s),this.bind("initPanel:after",function(t){t.off(i.scroll+"-"+a+" "+i.touchmove+"-"+a).on(i.scroll+"-"+a+" "+i.touchmove+"-"+a,function(i){t.hasClass(l.panel+"_opened")&&s.call(e,t)})})}},add:function(){l=d[t]._c,d[t]._d,(i=d[t]._e).add("scroll")},clickAnchor:function(i,t){}},d[t].defaults[a]={add:!1,addTo:"panels",fixed:!1,type:null}}(jQuery);
!function(y){var x,p,o,n="mmenu",O="drag";function $(n,e,t){return n<e&&(n=e),t<n&&(n=t),n}y[n].addons[O]={setup:function(){if(this.opts.offCanvas){var e=this.opts[O],t=this.conf[O];o=y[n].glbl,"boolean"==typeof e&&(e={menu:e,panels:e}),"object"!=typeof e&&(e={}),"boolean"==typeof e.menu&&(e.menu={open:e.menu}),"object"!=typeof e.menu&&(e.menu={}),"boolean"==typeof e.panels&&(e.panels={close:e.panels}),"object"!=typeof e.panels&&(e.panels={}),(e=this.opts[O]=y.extend(!0,{},y[n].defaults[O],e)).menu.open&&this.bind("setPage:after",function(){(function(e,t,o){var i,a,s,r,p=this,c={events:"panleft panright",typeLower:"x",typeUpper:"X",open_dir:"right",close_dir:"left",negative:!1},d="width",u=c.open_dir,f=function(n){n<=e.maxStartPos&&(m=1)},l=function(){return y("."+x.slideout)},m=0,h=0,g=0,n=this.opts.extensions.all,_=void 0===n?"left":-1<n.indexOf(x.menu+"_position-right")?"right":-1<n.indexOf(x.menu+"_position-top")?"top":-1<n.indexOf(x.menu+"_position-bottom")?"bottom":"left",v=void 0===n?"back":-1<n.indexOf(x.menu+"_position-top")||-1<n.indexOf(x.menu+"_position-bottom")||-1<n.indexOf(x.menu+"_position-front")?"front":"back";switch(_){case"top":case"bottom":c.events="panup pandown",c.typeLower="y",c.typeUpper="Y",d="height"}switch(_){case"right":case"bottom":c.negative=!0,f=function(n){n>=o.$wndw[d]()-e.maxStartPos&&(m=1)}}switch(_){case"right":c.open_dir="left",c.close_dir="right";break;case"top":c.open_dir="down",c.close_dir="up";break;case"bottom":c.open_dir="up",c.close_dir="down"}switch(v){case"front":l=function(){return p.$menu}}var b=this.__valueOrFn(this.$menu,e.node,o.$page);"string"==typeof b&&(b=y(b));var w=new Hammer(b[0],this.opts[O].vendors.hammer);w.on("panstart",function(n){f(n.center[c.typeLower]),r=l(),u=c.open_dir}),w.on(c.events+" panend",function(n){0<m&&n.preventDefault()}),w.on(c.events,function(n){if(i=n["delta"+c.typeUpper],c.negative&&(i=-i),i!=h&&(u=h<=i?c.open_dir:c.close_dir),(h=i)>e.threshold&&1==m){if(o.$html.hasClass(x.wrapper+"_opened"))return;m=2,p._openSetup(),p.trigger("open:start"),o.$html.addClass(x.dragging),g=$(o.$wndw[d]()*t[d].perc,t[d].min,t[d].max)}2==m&&(a=$(h,10,g)-("front"==v?g:0),c.negative&&(a=-a),s="translate"+c.typeUpper+"("+a+"px )",r.css({"-webkit-transform":"-webkit-"+s,transform:s}))}),w.on("panend",function(n){2==m&&(o.$html.removeClass(x.dragging),r.css("transform",""),p[u==c.open_dir?"_openFinish":"close"]()),m=0})}).call(this,e.menu,t.menu,o)}),e.panels.close&&this.bind("initPanel:after",function(n){(function(n,e,t,o){var i=this,a=n.data(p.parent);if(a){a=a.closest("."+x.panel);var s=new Hammer(n[0],i.opts[O].vendors.hammer),r=null;s.on("panright",function(n){r||(i.openPanel(a),r=setTimeout(function(){clearTimeout(r),r=null},i.conf.openingInterval+i.conf.transitionDuration))})}}).call(this,n,e.panels,t.panels,o)})}},add:function(){if("function"!=typeof Hammer||Hammer.VERSION<2)return y[n].addons[O].add=function(){},void(y[n].addons[O].setup=function(){});x=y[n]._c,p=y[n]._d,y[n]._e,x.add("dragging")},clickAnchor:function(n,e){}},y[n].defaults[O]={menu:{open:!1,maxStartPos:100,threshold:50},panels:{close:!1},vendors:{hammer:{}}},y[n].configuration[O]={menu:{width:{perc:.8,min:140,max:440},height:{perc:.8,min:140,max:880}},panels:{}}}(jQuery);
!function(n){var $,s,a,y,t="mmenu",f="dropdown";n[t].addons[f]={setup:function(){if(this.opts.offCanvas){var o=this,x=this.opts[f],b=this.conf[f];if(y=n[t].glbl,"boolean"==typeof x&&x&&(x={drop:x}),"object"!=typeof x&&(x={}),"string"==typeof x.position&&(x.position={of:x.position}),(x=this.opts[f]=n.extend(!0,{},n[t].defaults[f],x)).drop){var g;this.bind("initMenu:after",function(){if(this.$menu.addClass($.menu+"_"+f),"string"!=typeof x.position.of){var t=this._getOriginalMenuId();t&&t.length&&(x.position.of='[href="#'+t+'"]')}"string"==typeof x.position.of&&(g=n(x.position.of),x.event=x.event.split(" "),1==x.event.length&&(x.event[1]=x.event[0]),"hover"==x.event[0]&&g.on(a.mouseenter+"-"+f,function(){o.open()}),"hover"==x.event[1]&&this.$menu.on(a.mouseleave+"-"+f,function(){o.close()}))}),this.bind("open:start",function(){this.$menu.data(s.style,this.$menu.attr("style")||""),y.$html.addClass($.wrapper+"_dropdown")}),this.bind("close:finish",function(){this.$menu.attr("style",this.$menu.data(s.style)),y.$html.removeClass($.wrapper+"_dropdown")});var e=function(t,o){var e,i,n=o[0],s=o[1],a="x"==t?"scrollLeft":"scrollTop",f="x"==t?"outerWidth":"outerHeight",r="x"==t?"left":"top",p="x"==t?"right":"bottom",l="x"==t?"width":"height",h="x"==t?"maxWidth":"maxHeight",u=null,d=y.$wndw[a](),c=g.offset()[r]-=d,m=c+g[f](),v=y.$wndw[l](),w=b.offset.button[t]+b.offset.viewport[t];if(x.position[t])switch(x.position[t]){case"left":case"bottom":u="after";break;case"right":case"top":u="before"}return null===u&&(u=c+(m-c)/2<v/2?"after":"before"),"after"==u?(i=v-((e="x"==t?c:m)+w),n[r]=e+b.offset.button[t],n[p]="auto",x.tip&&s.push($.menu+"_tip-"+("x"==t?"left":"top"))):(i=(e="x"==t?m:c)-w,n[p]="calc( 100% - "+(e-b.offset.button[t])+"px )",n[r]="auto",x.tip&&s.push($.menu+"_tip-"+("x"==t?"right":"bottom"))),x.fitViewport&&(n[h]=Math.min(b[l].max,i)),[n,s]},i=function(t){if(this.vars.opened){this.$menu.attr("style",this.$menu.data(s.style));var o=[{},[]];o=e.call(this,"y",o),o=e.call(this,"x",o),this.$menu.css(o[0]),x.tip&&this.$menu.removeClass($.tipleft+" "+$.tipright+" "+$.tiptop+" "+$.tipbottom).addClass(o[1].join(" "))}};this.bind("open:start",i),y.$wndw.on(a.resize+"-"+f,function(t){i.call(o)}),this.opts.offCanvas.blockUI||y.$wndw.on(a.scroll+"-"+f,function(t){i.call(o)})}}},add:function(){$=n[t]._c,s=n[t]._d,a=n[t]._e,$.add("dropdown"),a.add("mouseenter mouseleave resize scroll")},clickAnchor:function(t,o){}},n[t].defaults[f]={drop:!1,fitViewport:!0,event:"click",position:{},tip:!0},n[t].configuration[f]={offset:{button:{x:-5,y:5},viewport:{x:20,y:20}},height:{max:880},width:{max:440}}}(jQuery);
!function(c){var f,a,s="mmenu",d="fixedElements";c[s].addons[d]={setup:function(){if(this.opts.offCanvas){this.opts[d];var o=this.conf[d];a=c[s].glbl;this.bind("setPage:after",function(s){var t=this.conf.classNames[d].fixed,i=s.find("."+t);this.__refactorClass(i,t,f.slideout),i[o.elemInsertMethod](o.elemInsertSelector);var e=this.conf.classNames[d].sticky,n=s.find("."+e);this.__refactorClass(n,e,f.sticky),(n=s.find("."+f.sticky)).length&&(this.bind("open:start",function(){if("hidden"==a.$html.css("overflow")){var s=a.$wndw.scrollTop()+o.sticky.offset;n.each(function(){c(this).css("top",parseInt(c(this).css("top"),10)+s)})}}),this.bind("close:finish",function(){n.css("top","")}))})}},add:function(){f=c[s]._c,c[s]._d,c[s]._e,f.add("sticky")},clickAnchor:function(s,t){}},c[s].configuration[d]={sticky:{offset:0},elemInsertMethod:"appendTo",elemInsertSelector:"body"},c[s].configuration.classNames[d]={fixed:"Fixed",sticky:"Sticky"}}(jQuery);
!function(c){var d,s,a,n="mmenu",t="iconbar";c[n].addons[t]={setup:function(){var e=this,o=this.opts[t];this.conf[t];if(c[n].glbl,o instanceof Array&&(o={add:!0,top:o}),o.add){var r=null;if(c.each(["top","bottom"],function(a,n){var t=o[n];t instanceof Array||(t=[t]);for(var e=c('<div class="'+d.iconbar+"__"+n+'" />'),i=0,s=t.length;i<s;i++)e.append(t[i]);e.children().length&&(r||(r=c('<div class="'+d.iconbar+'" />')),r.append(e))}),r&&(this.bind("initMenu:after",function(){var a=d.menu+"_iconbar";o.size&&(a+=" "+d.menu+"_iconbar-"+o.size),this.$menu.addClass(a).prepend(r)}),"tabs"==o.type)){r.addClass(d.iconbar+"_tabs");var i=r.find("a");i.on(a.click+"-"+t,function(a){var n=c(this);if(n.hasClass(d.iconbar+"__tab_selected"))a.stopImmediatePropagation();else try{var t=c(n.attr("href"));t.hasClass(d.panel)&&(a.preventDefault(),a.stopImmediatePropagation(),e.openPanel(t,!1))}catch(a){}}),this.bind("openPanel:start",function a(n){i.removeClass(d.iconbar+"__tab_selected");var t=i.filter('[href="#'+n.attr("id")+'"]');if(t.length)t.addClass(d.iconbar+"__tab_selected");else{var e=n.data(s.parent);e&&e.length&&a(e.closest("."+d.panel))}})}}},add:function(){d=c[n]._c,s=c[n]._d,a=c[n]._e,d.add(t)},clickAnchor:function(a,n){}},c[n].defaults[t]={add:!1,top:[],bottom:[]},c[n].configuration[t]={}}(jQuery);
!function(s){var d,r="mmenu",o="iconPanels";s[r].addons[o]={setup:function(){var n=this,a=this.opts[o],t=(this.conf[o],!1);s[r].glbl,"boolean"==typeof a&&(a={add:a}),"number"!=typeof a&&"string"!=typeof a||(a={add:!0,visible:a}),"object"!=typeof a&&(a={}),"first"==a.visible&&(t=!0,a.visible=1),(a=this.opts[o]=s.extend(!0,{},s[r].defaults[o],a)).visible=Math.min(3,Math.max(1,a.visible)),a.visible++;var l="";if(!t){for(var e=0;e<=a.visible;e++)l+=" "+d.panel+"_iconpanel-"+e;l.length&&(l=l.slice(1))}if(a.add){var i=function(e){if(!e.parent("."+d.listitem+"_vertical").length){var i=n.$pnls.children("."+d.panel);t?i.removeClass(d.panel+"_iconpanel-first").first().addClass(d.panel+"_iconpanel-first"):i.removeClass(l).filter("."+d.panel+"_opened-parent").removeClass(d.hidden).not(function(){return s(this).parent("."+d.listitem+"_vertical").length}).add(e).slice(-a.visible).each(function(e){s(this).addClass(d.panel+"_iconpanel-"+e)})}};this.bind("initMenu:after",function(){var e=[d.menu+"_iconpanel"];a.size&&e.push(d.menu+"_iconpanel-"+a.size),a.hideNavbar&&e.push(d.menu+"_hidenavbar"),a.hideDivider&&e.push(d.menu+"_hidedivider"),this.$menu.addClass(e.join(" "))}),this.bind("openPanel:start",i),this.bind("initPanels:after",function(e){i.call(n,n.$pnls.children("."+d.panel+"_opened"))}),this.bind("initListview:after",function(e){!a.blockPanel||e.parent("."+d.listitem+"_vertical").length||e.children("."+d.panel+"__blocker").length||e.prepend('<a href="#'+e.closest("."+d.panel).attr("id")+'" class="'+d.panel+'__blocker" />')})}},add:function(){d=s[r]._c,s[r]._d,s[r]._e},clickAnchor:function(e,i){}},s[r].defaults[o]={add:!1,blockPanel:!0,hideDivider:!1,hideNavbar:!0,visible:3}}(jQuery);
!function(s){var o,d,a,i,r="mmenu",c="keyboardNavigation";s[r].addons[c]={setup:function(){if(!s[r].support.touch){var e=this.opts[c];this.conf[c];if(i=s[r].glbl,"boolean"!=typeof e&&"string"!=typeof e||(e={enable:e}),"object"!=typeof e&&(e={}),(e=this.opts[c]=s.extend(!0,{},s[r].defaults[c],e)).enable){var n=s('<button class="'+o.tabstart+'" />'),t=s('<button class="'+o.tabend+'" />'),a=s('<button class="'+o.tabend+'" />');this.bind("initMenu:after",function(){e.enhance&&this.$menu.addClass(o.menu+"_keyboardfocus"),this["_initWindow_"+c](e.enhance)}),this.bind("initOpened:before",function(){this.$menu.prepend(n).append(t).children("."+o.mm("navbars-top")+", ."+o.mm("navbars-bottom")).children("."+o.navbar).children("a."+o.title).attr("tabindex",-1)}),this.bind("initBlocker:after",function(){i.$blck.append(a).children("a").addClass(o.tabstart)}),this.bind("open:finish",function(){f.call(this,null,e.enable)}),this.bind("openPanel:finish",function(n){f.call(this,n,e.enable)}),this.bind("initOpened:after:sr-aria",function(){var n=this.$menu.add(i.$blck).children("."+o.tabstart+", ."+o.tabend);this.__sr_aria(n,"hidden",!0),this.__sr_role(n,"presentation")})}}},add:function(){o=s[r]._c,d=s[r]._d,a=s[r]._e,o.add("tabstart tabend"),a.add("focusin keydown")},clickAnchor:function(n,e){}},s[r].defaults[c]={enable:!1,enhance:!1},s[r].configuration[c]={},s[r].prototype["_initWindow_"+c]=function(n){i.$wndw.off(a.keydown+"-offCanvas"),i.$wndw.off(a.focusin+"-"+c).on(a.focusin+"-"+c,function(n){if(i.$html.hasClass(o.wrapper+"_opened")){var e=s(n.target);if(e.is("."+o.tabend)){var t=s();e.parent().is("."+o.menu)&&i.$blck&&(t=i.$blck),e.parent().is("."+o.wrapper+"__blocker")&&(t=i.$body.find("."+o.menu+"_offcanvas").filter("."+o.menu+"_opened")),t.length||(t=e.parent()),t.children("."+o.tabstart).focus()}}}),i.$wndw.off(a.keydown+"-"+c).on(a.keydown+"-"+c,function(n){var e=s(n.target),t=e.closest("."+o.menu);if(t.length){t.data("mmenu");if(e.is("input, textarea"));else switch(n.keyCode){case 13:(e.is(".mm-toggle")||e.is(".mm-check"))&&e.trigger(a.click);break;case 32:case 37:case 38:case 39:case 40:n.preventDefault()}}}),n&&i.$wndw.off(a.keydown+"-"+c).on(a.keydown+"-"+c,function(n){var e=s(n.target),t=e.closest("."+o.menu);if(t.length){var a=t.data("mmenu");if(e.is("input"))switch(n.keyCode){case 27:e.val("")}else switch(n.keyCode){case 8:var i=t.find("."+o.panel+"_opened").data(d.parent);i&&i.length&&a.openPanel(i.closest("."+o.panel));break;case 27:t.hasClass(o.menu+"_offcanvas")&&a.close()}}})};var l="input, select, textarea, button, label, a[href]";function f(n,e){n=n||this.$pnls.children("."+o.panel+"_opened");var t=s(),a=this.$menu.children("."+o.mm("navbars_top")+", ."+o.mm("navbars_bottom")).children("."+o.navbar);a.find(l).filter(":focus").length||("default"==e&&((t=n.children("."+o.listview).find("a[href]").not("."+o.hidden)).length||(t=n.find(l).not("."+o.hidden)),t.length||(t=a.find(l).not("."+o.hidden))),t.length||(t=this.$menu.children("."+o.tabstart)),t.first().focus())}}(jQuery);
!function(e){var i,l="mmenu",t="lazySubmenus";e[l].addons[t]={setup:function(){var n=this.opts[t];this.conf[t];e[l].glbl,"boolean"==typeof n&&(n={load:n}),"object"!=typeof n&&(n={}),(n=this.opts[t]=e.extend(!0,{},e[l].defaults[t],n)).load&&(this.bind("initMenu:after",function(){this.$pnls.find("li").children(this.conf.panelNodetype).not("."+i.inset).not("."+i.nolistview).not("."+i.nopanel).addClass(i.panel+"_lazysubmenu "+i.nolistview+" "+i.nopanel)}),this.bind("initPanels:before",function(n){n=n||this.$pnls.children(this.conf.panelNodetype),this.__findAddBack(n,"."+i.panel+"_lazysubmenu").not("."+i.panel+"_lazysubmenu ."+i.panel+"_lazysubmenu").removeClass(i.panel+"_lazysubmenu "+i.nolistview+" "+i.nopanel)}),this.bind("initOpened:before",function(){var n=this.$pnls.find("."+this.conf.classNames.selected).parents("."+i.panel+"_lazysubmenu");n.length&&(n.removeClass(i.panel+"_lazysubmenu "+i.nolistview+" "+i.nopanel),this.initPanels(n.last()))}),this.bind("openPanel:before",function(n){var e=this.__findAddBack(n,"."+i.panel+"_lazysubmenu").not("."+i.panel+"_lazysubmenu ."+i.panel+"_lazysubmenu");e.length&&this.initPanels(e)}))},add:function(){i=e[l]._c,e[l]._d,e[l]._e},clickAnchor:function(n,e){}},e[l].defaults[t]={load:!1},e[l].configuration[t]={}}(jQuery);
!function(b){var p,h="mmenu",m="navbars";b[h].addons[m]={setup:function(){var d=this,l=this.opts[m],v=this.conf[m];if(b[h].glbl,void 0!==l){l instanceof Array||(l=[l]);var f={},u={};l.length&&(b.each(l,function(n){var t=l[n];"boolean"==typeof t&&t&&(t={}),"object"!=typeof t&&(t={}),void 0===t.content&&(t.content=["prev","title"]),t.content instanceof Array||(t.content=[t.content]),t=b.extend(!0,{},d.opts.navbar,t);var a=b('<div class="'+p.navbar+'" />'),e=t.height;"number"!=typeof e?e=1:1<(e=Math.min(4,Math.max(1,e)))&&a.addClass(p.navbar+"_size-"+e);var o=t.position;switch(o){case"bottom":break;default:o="top"}f[o]||(f[o]=0),f[o]+=e,u[o]||(u[o]=b('<div class="'+p.navbars+"_"+o+'" />')),u[o].append(a);for(var r=0,s=t.content.length;r<s;r++){var i=b[h].addons[m][t.content[r]]||null;i?i.call(d,a,t,v):((i=t.content[r])instanceof b||(i=b(t.content[r])),a.append(i))}var c=b[h].addons[m][t.type]||null;c&&c.call(d,a,t,v),a.children("."+p.btn).length&&a.addClass(p.navbar+"_has-btns")}),this.bind("initMenu:after",function(){for(var n in f)this.$menu.addClass(p.menu+"_navbar_"+n+"-"+f[n]),this.$menu["bottom"==n?"append":"prepend"](u[n])}))}},add:function(){p=b[h]._c,b[h]._d,b[h]._e,p.add(m)},clickAnchor:function(n,t){}},b[h].configuration[m]={breadcrumbs:{separator:"/",removeFirst:!1}},b[h].configuration.classNames[m]={}}(jQuery);
!function(f){var o,i,a,c="mmenu",d="pageScroll";f[c].addons[d]={setup:function(){var n=this,t=this.opts[d],s=this.conf[d];if(a=f[c].glbl,"boolean"==typeof t&&(t={scroll:t}),(t=this.opts[d]=f.extend(!0,{},f[c].defaults[d],t)).scroll&&this.bind("close:finish",function(){h(s.scrollOffset)}),t.update){var e=[],r=[];(n=this).bind("initListview:after",function(t){n.__filterListItemAnchors(t.find("."+o.listview).children("li")).each(function(){var t=f(this).attr("href");p(t)&&e.push(t)}),r=e.reverse()});var l=-1;a.$wndw.on(i.scroll+"-"+d,function(t){for(var e=a.$wndw.scrollTop(),i=0;i<r.length;i++)if(f(r[i]).offset().top<e+s.updateOffset){l!==i&&(l=i,n.setSelected(n.__filterListItemAnchors(n.$pnls.children("."+o.panel+"_opened").find("."+o.listview).children("li")).filter('[href="'+r[i]+'"]').parent()));break}})}},add:function(){o=f[c]._c,f[c]._d,i=f[c]._e},clickAnchor:function(t,e,i){if(s=!1,e&&i&&this.opts.offCanvas&&this.opts[d].scroll&&a.$page&&a.$page.length){var n=t.attr("href");if(p(n)){if(s=f(n),!this.$menu.is("."+o.menu+"_sidebar-expanded")||!a.$html.is("."+o.wrapper+"_sidebar-expanded"))return{close:!0};h(this.conf[d].scrollOffset)}}}},f[c].defaults[d]={scroll:!1,update:!1};var s=!(f[c].configuration[d]={scrollOffset:0,updateOffset:50});function h(t){s&&s.length&&s.is(":visible")&&a.$html.add(a.$body).animate({scrollTop:s.offset().top+t}),s=!1}function p(t){try{return!("#"==t||"#"!=t.slice(0,1)||!a.$page.find(t).length)}catch(t){return!1}}}(jQuery);
!function(o){var f,u,h,n="mmenu",_="searchfield";function l(e,n){if(n)for(var s in n)e.attr(s,n[s])}o[n].addons[_]={setup:function(){var a=this,t=this.opts[_],e=this.conf[_];o[n].glbl,"boolean"==typeof t&&(t={add:t}),"object"!=typeof t&&(t={}),"boolean"==typeof t.panel&&(t.panel={add:t.panel}),"object"!=typeof t.panel&&(t.panel={}),t.add&&("panel"==t.addTo&&(t.panel.add=!0),t.panel.add&&(t.showSubPanels=!1,t.panel.splash&&(t.cancel=!0)),t=this.opts[_]=o.extend(!0,{},o[n].defaults[_],t),e=this.conf[_]=o.extend(!0,{},o[n].configuration[_],e),this.bind("close:start",function(){this.$menu.find("."+f.searchfield).children("input").blur()}),this.bind("initPanels:after",function(e){var n,s=o();switch(t.panel.add&&(s=this._initSearchPanel(e)),t.addTo){case"panels":n=e;break;case"panel":n=s;break;default:n=this.$menu.find(t.addTo)}(n.each(function(){var e=a._initSearchfield(o(this));t.search&&e.length&&a._initSearching(e)}),t.noResults)&&(t.panel.add?s:e).each(function(){a._initNoResultsMsg(o(this))})}))},add:function(){f=o[n]._c,u=o[n]._d,h=o[n]._e,f.add("searchfield"),u.add("searchfield"),h.add("input focus blur")},clickAnchor:function(e,n){if(e.hasClass(f.searchfield+"__btn")){if(e.hasClass(f.btn+"_close")){var s=e.closest("."+f.searchfield).find("input");return s.val(""),this.search(s),!0}if(e.hasClass(f.btn+"_next"))return e.closest("."+f.searchfield).submit(),!0}}},o[n].defaults[_]={add:!1,addTo:"panels",noResults:"No results found.",placeholder:"Search",panel:{add:!1,dividers:!0,fx:"none",id:null,splash:null,title:"Search"},search:!0,showTextItems:!1,showSubPanels:!0},o[n].configuration[_]={clear:!1,form:!1,input:!1,submit:!1},o[n].prototype._initSearchPanel=function(e){var n=this.opts[_];this.conf[_];if(this.$pnls.children("."+f.panel+"_search").length)return o();var s=o('<div class="'+f.panel+'_search " />').append("<ul />").appendTo(this.$pnls);switch(n.panel.id&&s.attr("id",n.panel.id),n.panel.title&&s.attr("data-mm-title",n.panel.title),n.panel.fx){case!1:break;case"none":s.addClass(f.panel+"_noanimation");break;default:s.addClass(f.panel+"_fx-"+n.panel.fx)}return n.panel.splash&&s.append('<div class="'+f.panel+'__searchsplash">'+n.panel.splash+"</div>"),this._initPanels(s),s},o[n].prototype._initSearchfield=function(e){var n=this.opts[_],s=this.conf[_];if(e.parent("."+f.listitem+"_vertical").length)return o();if(e.find("."+f.searchfield).length)return e.find("."+f.searchfield);var a=o("<"+(s.form?"form":"div")+' class="'+f.searchfield+'" />'),t=o('<div class="'+f.searchfield+'__input" />'),i=o('<input placeholder="'+this.i18n(n.placeholder)+'" type="text" autocomplete="off" />');return t.append(i).appendTo(a),e.prepend(a),e.hasClass(f.panel)&&e.addClass(f.panel+"_has-searchfield"),l(i,s.input),s.clear&&o('<a class="'+f.btn+" "+f.btn+"_close "+f.searchfield+'__btn" href="#" />').appendTo(t),l(a,s.form),s.form&&s.submit&&!s.clear&&o('<a class="'+f.btn+" "+f.btn+"_next "+f.searchfield+'__btn" href="#" />').appendTo(t),n.cancel&&o('<a href="#" class="'+f.searchfield+'__cancel">'+this.i18n("cancel")+"</a>").appendTo(a),a},o[n].prototype._initSearching=function(e){var n=this,s=this.opts[_],a=(this.conf[_],{});e.closest("."+f.panel+"_search").length?(a.$pnls=this.$pnls.find("."+f.panel),a.$nrsp=e.closest("."+f.panel)):e.closest("."+f.panel).length?(a.$pnls=e.closest("."+f.panel),a.$nrsp=a.$pnls):(a.$pnls=this.$pnls.find("."+f.panel),a.$nrsp=this.$menu),a.$pnls=a.$pnls.not(function(){return o(this).parent("."+f.listitem+"_vertical").length}),s.panel.add&&(a.$pnls=a.$pnls.not("."+f.panel+"_search"));var t=e.find("input"),i=e.find("."+f.searchfield+"__cancel"),l=this.$pnls.children("."+f.panel+"_search"),d=a.$pnls.find("."+f.listitem);a.$itms=d.not("."+f.listitem+"_divider"),a.$dvdr=d.filter("."+f.listitem+"_divider"),s.panel.add&&s.panel.splash&&t.off(h.focus+"-"+_+"-splash").on(h.focus+"-"+_+"-splash",function(e){n.openPanel(l)}),s.cancel&&(t.off(h.focus+"-"+_+"-cancel").on(h.focus+"-"+_+"-cancel",function(e){i.addClass(f.searchfield+"__cancel-active")}),i.off(h.click+"-"+_+"-splash").on(h.click+"-"+_+"-splash",function(e){e.preventDefault(),o(this).removeClass(f.searchfield+"__cancel-active"),l.hasClass(f.panel+"_opened")&&n.openPanel(n.$pnls.children("."+f.panel+"_opened-parent").last())})),s.panel.add&&"panel"==s.addTo&&this.bind("openPanel:finish",function(e){e[0]===l[0]&&t.focus()}),t.data(u.searchfield,a).off(h.input+"-"+_).on(h.input+"-"+_,function(e){(function(e){switch(e){case 9:case 16:case 17:case 18:case 37:case 38:case 39:case 40:return!0}return!1})(e.keyCode)||n.search(t)}),this.search(t)},o[n].prototype._initNoResultsMsg=function(e){var n=this.opts[_];this.conf[_];if(e.closest("."+f.panel).length||(e=this.$pnls.children("."+f.panel).first()),!e.children("."+f.panel+"__noresultsmsg").length){var s=e.children("."+f.listview).first(),a=o('<div class="'+f.panel+"__noresultsmsg "+f.hidden+'" />').append(this.i18n(n.noResults));s.length?a.insertAfter(s):a.prependTo(e)}},o[n].prototype.search=function(a,s){var t=this,i=this.opts[_];this.conf[_];a=a||this.$menu.find("."+f.searchfield).chidren("input").first(),s=(s=s||a.val()).toLowerCase().trim();var e=a.data(u.searchfield),n=a.closest("."+f.searchfield).find("."+f.btn),l=this.$pnls.children("."+f.panel+"_search"),d=e.$pnls,h=e.$itms,r=e.$dvdr,c=e.$nrsp;if(h.removeClass(f.listitem+"_nosubitems").find("."+f.btn+"_fullwidth-search").removeClass(f.btn+"_fullwidth-search "+f.btn+"_fullwidth"),l.children("."+f.listview).empty(),d.scrollTop(0),s.length){if(h.add(r).addClass(f.hidden),h.each(function(){var e=o(this),n="a";(i.showTextItems||i.showSubPanels&&e.find("."+f.btn+"_next"))&&(n="a, span"),-1<e.children(n).not("."+f.btn+"_next").text().toLowerCase().indexOf(s)&&e.removeClass(f.hidden)}),i.panel.add){var p=o();d.each(function(){var e=t.__filterListItems(o(this).find("."+f.listitem)).clone(!0);e.length&&(i.panel.dividers&&(p=p.add('<li class="'+f.listitem+" "+f.listitem+'_divider">'+o(this).find("."+f.navbar+"__title").text()+"</li>")),p=p.add(e))}),p.find("."+f.mm("toggle")).remove().end().find("."+f.mm("check")).remove().end().find("."+f.btn).remove(),l.children("."+f.listview).append(p),this.openPanel(l)}else i.showSubPanels&&d.each(function(e){var n=o(this);t.__filterListItems(n.find("."+f.listitem)).each(function(){var e=o(this).data(u.child);e&&e.find("."+f.listview).children().removeClass(f.hidden)})}),o(d.get().reverse()).each(function(e){var n=o(this),s=n.data(u.parent);s&&(t.__filterListItems(n.find("."+f.listitem)).length?s.hasClass(f.hidden)&&s.removeClass(f.hidden).children("."+f.btn+"_next").not("."+f.btn+"_fullwidth").addClass(f.btn+"_fullwidth").addClass(f.btn+"_fullwidth-search"):a.closest("."+f.panel).length||((n.hasClass(f.panel+"_opened")||n.hasClass(f.panel+"_opened-parent"))&&setTimeout(function(){t.openPanel(s.closest("."+f.panel))},(e+1)*(1.5*t.conf.openingInterval)),s.addClass(f.listitem+"_nosubitems")))}),this.__filterListItems(d.find("."+f.listitem)).each(function(){o(this).prevAll("."+f.listitem+"_divider").first().removeClass(f.hidden)});n.removeClass(f.hidden),c.find("."+f.panel+"__noresultsmsg")[h.not("."+f.hidden).length?"addClass":"removeClass"](f.hidden),i.panel.add&&(i.panel.splash&&l.find("."+f.panel+"__searchsplash").addClass(f.hidden),h.add(r).removeClass(f.hidden))}else h.add(r).removeClass(f.hidden),n.addClass(f.hidden),c.find("."+f.panel+"__noresultsmsg").addClass(f.hidden),i.panel.add&&(i.panel.splash?l.find("."+f.panel+"__searchsplash").removeClass(f.hidden):a.closest("."+f.panel+"_search").length||this.openPanel(this.$pnls.children("."+f.panel+"_opened-parent").last()));this.trigger("updateListview")}}(jQuery);
!function(d){var h,t,e="mmenu",o="sectionIndexer";d[e].addons[o]={setup:function(){var s=this,i=this.opts[o];this.conf[o];d[e].glbl,"boolean"==typeof i&&(i={add:i}),"object"!=typeof i&&(i={}),i=this.opts[o]=d.extend(!0,{},d[e].defaults[o],i);var r=null;this.bind("initPanels:after",function(e){if(i.add){var a;switch(i.addTo){case"panels":a=e;break;default:a=(a=d(i.addTo,this.$menu)).filter("."+h.panel)}a.find("."+h.listitem+"_divider").closest("."+h.panel).addClass(h.panel+"_has-sectionindexer"),r||(r=d('<div class="'+h.sectionindexer+'" />').prependTo(this.$menu).append('<a href="#a">a</a><a href="#b">b</a><a href="#c">c</a><a href="#d">d</a><a href="#e">e</a><a href="#f">f</a><a href="#g">g</a><a href="#h">h</a><a href="#i">i</a><a href="#j">j</a><a href="#k">k</a><a href="#l">l</a><a href="#m">m</a><a href="#n">n</a><a href="#o">o</a><a href="#p">p</a><a href="#q">q</a><a href="#r">r</a><a href="#s">s</a><a href="#t">t</a><a href="#u">u</a><a href="#v">v</a><a href="#w">w</a><a href="#x">x</a><a href="#y">y</a><a href="#z">z</a>')).on(t.mouseover+"-"+o+" "+t.touchstart+"-"+o,"a",function(e){var a=d(e.target).attr("href").slice(1),n=s.$pnls.children("."+h.panel+"_opened"),i=n.find("."+h.listview),r=-1,t=n.scrollTop();n.scrollTop(0),i.children("."+h.listitem+"_divider").not("."+h.hidden).each(function(){r<0&&a==d(this).text().slice(0,1).toLowerCase()&&(r=d(this).position().top)}),n.scrollTop(-1<r?r:t)});var n=function(e){e=e||this.$pnls.children("."+h.panel+"_opened"),this.$menu[(e.hasClass(h.panel+"_has-sectionindexer")?"add":"remove")+"Class"](h.menu+"_has-sectionindexer")};this.bind("openPanel:start",n),this.bind("initPanels:after",n)}})},add:function(){h=d[e]._c,d[e]._d,t=d[e]._e,h.add("sectionindexer"),t.add("mouseover")},clickAnchor:function(e,a){if(e.parent().is("."+h.indexer))return!0}},d[e].defaults[o]={add:!1,addTo:"panels"}}(jQuery);
!function(t){var s,a,l="mmenu",r="setSelected";t[l].addons[r]={setup:function(){var n=this,e=this.opts[r];this.conf[r];if(t[l].glbl,"boolean"==typeof e&&(e={hover:e,parent:e}),"object"!=typeof e&&(e={}),"detect"==(e=this.opts[r]=t.extend(!0,{},t[l].defaults[r],e)).current){var i=function(e){e=e.split("?")[0].split("#")[0];var t=n.$menu.find('a[href="'+e+'"], a[href="'+e+'/"]');t.length?n.setSelected(t.parent(),!0):(e=e.split("/").slice(0,-1)).length&&i(e.join("/"))};this.bind("initMenu:after",function(){i(window.location.href)})}else e.current||this.bind("initListview:after",function(e){e.find("."+s.listview).children("."+s.listitem+"_selected").removeClass(s.listitem+"_selected")});e.hover&&this.bind("initMenu:after",function(){this.$menu.addClass(s.menu+"_selected-hover")}),e.parent&&(this.bind("openPanel:finish",function(e){this.$pnls.find("."+s.listview).find("."+s.listitem+"_selected-parent").removeClass(s.listitem+"_selected-parent");for(var t=e.data(a.parent);t;)t.not("."+s.listitem+"_vertical").addClass(s.listitem+"_selected-parent"),t=t.closest("."+s.panel).data(a.parent)}),this.bind("initMenu:after",function(){this.$menu.addClass(s.menu+"_selected-parent")}))},add:function(){s=t[l]._c,a=t[l]._d,t[l]._e},clickAnchor:function(e,t){}},t[l].defaults[r]={current:!0,hover:!1,parent:!1}}(jQuery);
!function(a){var n,t,l="mmenu",i="sidebar";a[l].addons[i]={setup:function(){if(this.opts.offCanvas){var e=this.opts[i];this.conf[i];t=a[l].glbl,("string"==typeof e||"boolean"==typeof e&&e||"number"==typeof e)&&(e={expanded:e}),"object"!=typeof e&&(e={}),"boolean"==typeof e.collapsed&&e.collapsed&&(e.collapsed="all"),"string"!=typeof e.collapsed&&"number"!=typeof e.collapsed||(e.collapsed={use:e.collapsed}),"object"!=typeof e.collapsed&&(e.collapsed={}),"number"==typeof e.collapsed.use&&(e.collapsed.use="(min-width: "+e.collapsed.use+"px)"),"boolean"==typeof e.expanded&&e.expanded&&(e.expanded="all"),"string"!=typeof e.expanded&&"number"!=typeof e.expanded||(e.expanded={use:e.expanded}),"object"!=typeof e.expanded&&(e.expanded={}),"number"==typeof e.expanded.use&&(e.expanded.use="(min-width: "+e.expanded.use+"px)"),e=this.opts[i]=a.extend(!0,{},a[l].defaults[i],e);var d=n.wrapper+"_sidebar-collapsed";e.collapsed.size&&(d+=" "+n.wrapper+"_sidebar-collapsed-"+e.collapsed.size);var s=n.wrapper+"_sidebar-expanded";e.expanded.size&&(s+=" "+n.wrapper+"_sidebar-expanded-"+e.expanded.size),e.collapsed.use&&(this.bind("initMenu:after",function(){this.$menu.addClass(n.menu+"_sidebar-collapsed"),e.collapsed.blockMenu&&this.opts.offCanvas&&!this.$menu.children("."+n.menu+"__blocker").length&&this.$menu.prepend('<a class="'+n.menu+'__blocker" href="#'+this.$menu.attr("id")+'" />'),e.collapsed.hideNavbar&&this.$menu.addClass(n.menu+"_hidenavbar"),e.collapsed.hideDivider&&this.$menu.addClass(n.menu+"_hidedivider")}),"boolean"==typeof e.collapsed.use?this.bind("initMenu:after",function(){t.$html.addClass(d)}):this.matchMedia(e.collapsed.use,function(){t.$html.addClass(d)},function(){t.$html.removeClass(d)})),e.expanded.use&&(this.bind("initMenu:after",function(){this.$menu.addClass(n.menu+"_sidebar-expanded")}),"boolean"==typeof e.expanded.use?this.bind("initMenu:after",function(){t.$html.addClass(s),this.open()}):this.matchMedia(e.expanded.use,function(){t.$html.addClass(s),t.$html.hasClass(n.wrapper+"_sidebar-closed")||this.open()},function(){t.$html.removeClass(s),this.close()}),this.bind("close:start",function(){t.$html.hasClass(s)&&t.$html.addClass(n.wrapper+"_sidebar-closed")}),this.bind("open:start",function(){t.$html.removeClass(n.wrapper+"_sidebar-closed")}))}},add:function(){n=a[l]._c,a[l]._d,a[l]._e},clickAnchor:function(e,d,s){if(this.opts[i].expanded.use&&t.$html.is("."+n.wrapper+"_sidebar-expanded")&&d&&s)return{close:!1}}},a[l].defaults[i]={collapsed:{use:!1,blockMenu:!0,hideDivider:!1,hideNavbar:!0},expanded:{use:!1}},a[l].configuration[i]={}}(jQuery);
!function(n){var l,t="mmenu",e="toggles";n[t].addons[e]={setup:function(){var s=this;this.opts[e],this.conf[e];n[t].glbl,this.bind("initPanels:after",function(t){this.__refactorClass(t.find("input"),this.conf.classNames[e].toggle,l.toggle),this.__refactorClass(t.find("input"),this.conf.classNames[e].check,l.check),t.find("input."+l.toggle+", input."+l.check).each(function(){var t=n(this),e=t.closest("li"),c=t.hasClass(l.toggle)?"toggle":"check",i=t.attr("id")||s.__getUniqueId();e.children('label[for="'+i+'"]').length||(t.attr("id",i),e.prepend(t),n('<label for="'+i+'" class="'+l[c]+'"></label>').insertAfter(e.children("."+l.listitem+"__text").last()))})})},add:function(){l=n[t]._c,n[t]._d,n[t]._e,l.add("toggle check")},clickAnchor:function(t,e){}},n[t].configuration.classNames[e]={toggle:"Toggle",check:"Check"}}(jQuery);
!function(d){var s="mmenu";d[s].addons.navbars.breadcrumbs=function(a,r,i){var n=this,b=d[s]._c,c=d[s]._d;b.add("separator");var e=d('<span class="'+b.navbar+'__breadcrumbs" />').appendTo(a);this.bind("initNavbar:after",function(a){if(!a.children("."+b.navbar).children("."+b.navbar+"__breadcrumbs").length){a.removeClass(b.panel+"_has-navbar");for(var r=[],n=d('<span class="'+b.navbar+'__breadcrumbs"></span>'),e=a,s=!0;e&&e.length;){if(e.is("."+b.panel)||(e=e.closest("."+b.panel)),!e.parent("."+b.listitem+"_vertical").length){var t=e.children("."+b.navbar).children("."+b.navbar+"__title").text();t.length&&r.unshift(s?"<span>"+t+"</span>":'<a href="#'+e.attr("id")+'">'+t+"</a>"),s=!1}e=e.data(c.parent)}i.breadcrumbs.removeFirst&&r.shift(),n.append(r.join('<span class="'+b.separator+'">'+i.breadcrumbs.separator+"</span>")).appendTo(a.children("."+b.navbar))}}),this.bind("openPanel:start",function(a){var r=a.find("."+b.navbar+"__breadcrumbs");r.length&&e.html(r.html()||"")}),this.bind("initNavbar:after:sr-aria",function(a){a.children("."+b.navbar).children("."+b.breadcrumbs).children("a").each(function(){n.__sr_aria(d(this),"owns",d(this).attr("href").slice(1))})})}}(jQuery);
!function(s){var r="mmenu";s[r].addons.navbars.close=function(t,e){var n=s[r]._c;s[r].glbl;n.add("close");var a=s('<a class="'+n.btn+" "+n.btn+"_close "+n.navbar+'__btn" href="#" />').appendTo(t);this.bind("setPage:after",function(t){a.attr("href","#"+t.attr("id"))}),this.bind("setPage:after:sr-text",function(t){a.html(this.__sr_text(this.i18n(this.conf.screenReader.text.closeMenu))),this.__sr_aria(a,"owns",a.attr("href").slice(1))})}}(jQuery);
!function(h){var d="mmenu",o="navbars";h[d].addons[o].next=function(a,n){var t,e,s,r=h[d]._c,i=h('<a class="'+r.btn+" "+r.btn+"_next "+r.navbar+'__btn" href="#" />').appendTo(a);this.bind("openPanel:start",function(a){t=a.find("."+this.conf.classNames[o].panelNext),e=t.attr("href"),s=t.html(),e?i.attr("href",e):i.removeAttr("href"),i[e||s?"removeClass":"addClass"](r.hidden),i.html(s)}),this.bind("openPanel:start:sr-aria",function(a){this.__sr_aria(i,"hidden",i.hasClass(r.hidden)),this.__sr_aria(i,"owns",(i.attr("href")||"").slice(1))})},h[d].configuration.classNames[o].panelNext="Next"}(jQuery);
!function(h){var l="mmenu",d="navbars";h[l].addons[d].prev=function(a,n){var r,e,t,i=h[l]._c,s=h('<a class="'+i.btn+" "+i.btn+"_prev "+i.navbar+'__btn" href="#" />').appendTo(a);this.bind("initNavbar:after",function(a){a.removeClass(i.panel+"_has-navbar")}),this.bind("openPanel:start",function(a){a.parent("."+i.listitem+"_vertical").length||((r=a.find("."+this.conf.classNames[d].panelPrev)).length||(r=a.children("."+i.navbar).children("."+i.btn+"_prev")),e=r.attr("href"),t=r.html(),e?s.attr("href",e):s.removeAttr("href"),s[e||t?"removeClass":"addClass"](i.hidden),s.html(t))}),this.bind("initNavbar:after:sr-aria",function(a){var n=a.children("."+i.navbar);this.__sr_aria(n,"hidden",!0)}),this.bind("openPanel:start:sr-aria",function(a){this.__sr_aria(s,"hidden",s.hasClass(i.hidden)),this.__sr_aria(s,"owns",(s.attr("href")||"").slice(1))})},h[l].configuration.classNames[d].panelPrev="Prev"}(jQuery);
!function(t){var a="mmenu";t[a].addons.navbars.searchfield=function(s,e){t[a]._c;"object"!=typeof this.opts.searchfield&&(this.opts.searchfield={}),this.opts.searchfield.add=!0,this.opts.searchfield.addTo=s}}(jQuery);
!function(i){var o="mmenu",c="navbars";i[o].addons[c].tabs=function(a,e,t){var s=i[o]._c,r=i[o]._d,n=i[o]._e,l=this,d=a.children("a");a.addClass(s.navbar+"_tabs").parent().addClass(s.navbars+"_has-tabs"),d.on(n.click+"-"+c,function(a){a.preventDefault();var e=i(this);if(e.hasClass(s.navbar+"__tab_selected"))a.stopImmediatePropagation();else try{l.openPanel(i(e.attr("href")),!1),a.stopImmediatePropagation()}catch(a){}}),this.bind("openPanel:start",function a(e){d.removeClass(s.navbar+"__tab_selected");var t=d.filter('[href="#'+e.attr("id")+'"]');if(t.length)t.addClass(s.navbar+"__tab_selected");else{var n=e.data(r.parent);n&&n.length&&a(n.closest("."+s.panel))}})}}(jQuery);
!function(h){var d="mmenu",o="navbars";h[d].addons[o].title=function(t,a){var e,n,i,r,s=h[d]._c,l=h('<a class="'+s.navbar+'__title" />').appendTo(t);this.bind("openPanel:start",function(t){t.parent("."+s.listitem+"_vertical").length||((i=t.find("."+this.conf.classNames[o].panelTitle)).length||(i=t.children("."+s.navbar).children("."+s.navbar+"__title")),e=i.attr("href"),n=i.html()||a.title,e?l.attr("href",e):l.removeAttr("href"),l[e||n?"removeClass":"addClass"](s.hidden),l.html(n))}),this.bind("openPanel:start:sr-aria",function(t){if(this.opts.screenReader.text&&(r||(r=this.$menu.children("."+s.navbars+"_top, ."+s.navbars+"_bottom").children("."+s.navbar).children("."+s.btn+"_prev")),r.length)){var a=!0;"parent"==this.opts.navbar.titleLink&&(a=!r.hasClass(s.hidden)),this.__sr_aria(l,"hidden",a)}})},h[d].configuration.classNames[o].panelTitle="Title"}(jQuery);
jQuery.mmenu.wrappers.angular=function(){this.opts.onClick={close:!0,preventDefault:!1,setSelected:!0}};
!function(a){a.mmenu.wrappers.bootstrap3=function(){if(this.$menu.hasClass("navbar-collapse")){this.conf.classNames.selected="active",this.conf.classNames.divider="divider",this.conf.clone=!0,this.opts.hooks=this.opts.hooks||{};for(var n="",a=["nav-tabs","nav-pills","navbar-nav"],e=0;e<a.length;e++)if(this.$menu.find("."+a[e]).length){n=a[e];break}n.length&&(this.opts.hooks["initMenu:before"]=function(){"navbar-nav"==n&&this.$menu.wrapInner("<div />")},this.opts.hooks["initMenu:after"]=function(){t.menu.call(this),t.dropdown.call(this),t[n.split("nav-").join("").split("-nav").join("")].call(this)})}};var t={menu:function(){this.$menu.find(".nav").removeClass("nav").end().find(".sr-only").remove().end().find(".divider:empty").remove();for(var n=["role","aria-haspopup","aria-expanded"],a=0;a<n.length;a++)this.$menu.find("["+n[a]+"]").removeAttr(n[a])},dropdown:function(){var n=this.$menu.find(".dropdown");n.removeClass("dropdown"),n.children(".dropdown-toggle").find(".caret").remove().end().each(function(){a(this).replaceWith("<span>"+a(this).html()+"</span>")}),n.children(".dropdown-menu").removeClass("dropdown-menu")},tabs:function(){this.$menu.find(".nav-tabs").removeClass("nav-tabs")},pills:function(){this.$menu.find(".nav-pills").removeClass("nav-pills")},navbar:function(){var a=this;this.$menu.removeClass("collapse navbar-collapse").find('[class*="navbar-"]').removeClass("navbar-left navbar-right navbar-nav navbar-text navbar-btn");var n=this.$menu.find(".navbar-form");this.conf.searchform={form:{action:n.attr("action"),method:n.attr("method")},input:{name:n.find("input").attr("name")},submit:!0},n.remove(),(this.$orig||this.$menu).closest(".navbar").find(".navbar-header").find(".navbar-toggle").off("click").on("click",function(n){a.open(),n.stopImmediatePropagation(),n.preventDefault()})}}}(jQuery);
!function(i){function s(n){for(var a=i("<a />"),e=["href","title","target"],t=0;t<e.length;t++)void 0!==n.attr(e[t])&&a.attr(e[t],n.attr(e[t]));return a.html(n.html()),a.find(".sr-only").remove(),a}function o(n){var e=i("<ul />");return n.children().each(function(){var n=i(this),a=i("<li />");n.hasClass("dropdown-divider")?a.addClass("Divider"):n.hasClass("dropdown-item")&&a.append(s(n)),e.append(a)}),e}i.mmenu.wrappers.bootstrap4=function(){var e=this;if(this.$menu.hasClass("navbar-collapse")){this.conf.clone=!1;var n=i("<nav />"),r=i("<div />");n.append(r),this.$menu.children().each(function(){var n,t,a=i(this);switch(!0){case a.hasClass("navbar-nav"):r.append((n=a,t=i("<ul />"),n.find(".nav-item").each(function(){var n=i(this),a=i("<li />");if(n.hasClass("active")&&a.addClass("Selected"),!n.hasClass("nav-link")){var e=n.children(".dropdown-menu");e.length&&a.append(o(e)),n=n.children(".nav-link")}a.prepend(s(n)),t.append(a)}),t));break;case a.hasClass("dropdown-menu"):r.append(o(a));break;case a.hasClass("form-inline"):e.conf.searchfield.form={action:a.attr("action")||null,method:a.attr("method")||null},e.conf.searchfield.input={name:a.find("input").attr("name")||null},e.conf.searchfield.clear=!1,e.conf.searchfield.submit=!0;break;default:r.append(a.clone(!0))}}),this.bind("initMenu:before",function(){n.prependTo("body"),this.$menu=n}),this.$menu.parent().find(".navbar-toggler").removeAttr("data-target").removeAttr("aria-controls").off("click").on("click",function(n){n.preventDefault(),n.stopImmediatePropagation(),e[e.vars.opened?"close":"open"]()})}}}(jQuery);
!function(n){n.mmenu.wrappers.jqueryMobile=function(){var t=this;this.opts.onClick.close=!1,this.conf.offCanvas.page.selector="div.ui-page-active",n("body").on("pagecontainerchange",function(e,n){"function"==typeof t.close&&(t.close(),t.setPage(n.toPage))}),this.bind("initAnchors:after",function(){n("body").on("click",".mm-listview a",function(e){e.isDefaultPrevented()||(e.preventDefault(),n("body").pagecontainer("change",n(this).attr("href")))})})}}(jQuery);
jQuery.mmenu.wrappers.magento=function(){this.conf.classNames.selected="active"};
jQuery.mmenu.wrappers.olark=function(){this.conf.offCanvas.page.noSelector.push("#olark")};
!function(o){var r="mmenu";o[r].wrappers.turbolinks=function(){var n,t;o(document).on("turbolinks:before-visit",function(){t=o("html"),n=t.attr("class"),n=o.grep(n.split(/\s+/),function(n){return!/mm-/.test(n)}).join(" ")}).on("turbolinks:load",function(){void 0!==t&&(t.attr("class",n),o[r].glbl=!1)})}}(jQuery);
!function(s){s.mmenu.wrappers.wordpress=function(){this.conf.classNames.selected="current-menu-item",s("#wpadminbar").css("position","fixed").addClass("mm-slideout")}}(jQuery);
jQuery.mmenu.i18n({Menu:"Menü"},"de");
jQuery.mmenu.i18n({"Close menu":"Menü schließen","Close submenu":"Untermenü schließen","Open submenu":"Untermenü öffnen","Toggle submenu":"Untermenü wechseln"},"de");
jQuery.mmenu.i18n({Search:"Suche","No results found.":"Keine Ergebnisse gefunden.",cancel:"beenden"},"de");
jQuery.mmenu.i18n({Menu:"Menu"},"nl");
jQuery.mmenu.i18n({"Close menu":"Menu sluiten","Close submenu":"Submenu sluiten","Open submenu":"Submenu openen","Toggle submenu":"Submenu wisselen"},"nl");
jQuery.mmenu.i18n({Search:"Zoeken","No results found.":"Geen resultaten gevonden.",cancel:"annuleren"},"nl");
jQuery.mmenu.i18n({Menu:"Меню"},"ru");
jQuery.mmenu.i18n({"Close menu":"Закрыть меню","Close submenu":"Закрыть подменю","Open submenu":"Открыть подменю","Toggle submenu":"Переключить подменю"},"ru");
jQuery.mmenu.i18n({Search:"Найти","No results found.":"Ничего не найдено.","Search results":"Результаты поиска"},"ru");
return true;
}));

!function(o){var f,u,h,n="mmenu",_="searchfield";function l(e,n){if(n)for(var s in n)e.attr(s,n[s])}o[n].addons[_]={setup:function(){var a=this,t=this.opts[_],e=this.conf[_];o[n].glbl,"boolean"==typeof t&&(t={add:t}),"object"!=typeof t&&(t={}),"boolean"==typeof t.panel&&(t.panel={add:t.panel}),"object"!=typeof t.panel&&(t.panel={}),t.add&&("panel"==t.addTo&&(t.panel.add=!0),t.panel.add&&(t.showSubPanels=!1,t.panel.splash&&(t.cancel=!0)),t=this.opts[_]=o.extend(!0,{},o[n].defaults[_],t),e=this.conf[_]=o.extend(!0,{},o[n].configuration[_],e),this.bind("close:start",function(){this.$menu.find("."+f.searchfield).children("input").blur()}),this.bind("initPanels:after",function(e){var n,s=o();switch(t.panel.add&&(s=this._initSearchPanel(e)),t.addTo){case"panels":n=e;break;case"panel":n=s;break;default:n=this.$menu.find(t.addTo)}(n.each(function(){var e=a._initSearchfield(o(this));t.search&&e.length&&a._initSearching(e)}),t.noResults)&&(t.panel.add?s:e).each(function(){a._initNoResultsMsg(o(this))})}))},add:function(){f=o[n]._c,u=o[n]._d,h=o[n]._e,f.add("searchfield"),u.add("searchfield"),h.add("input focus blur")},clickAnchor:function(e,n){if(e.hasClass(f.searchfield+"__btn")){if(e.hasClass(f.btn+"_close")){var s=e.closest("."+f.searchfield).find("input");return s.val(""),this.search(s),!0}if(e.hasClass(f.btn+"_next"))return e.closest("."+f.searchfield).submit(),!0}}},o[n].defaults[_]={add:!1,addTo:"panels",noResults:"No results found.",placeholder:"Search",panel:{add:!1,dividers:!0,fx:"none",id:null,splash:null,title:"Search"},search:!0,showTextItems:!1,showSubPanels:!0},o[n].configuration[_]={clear:!1,form:!1,input:!1,submit:!1},o[n].prototype._initSearchPanel=function(e){var n=this.opts[_];this.conf[_];if(this.$pnls.children("."+f.panel+"_search").length)return o();var s=o('<div class="'+f.panel+'_search " />').append("<ul />").appendTo(this.$pnls);switch(n.panel.id&&s.attr("id",n.panel.id),n.panel.title&&s.attr("data-mm-title",n.panel.title),n.panel.fx){case!1:break;case"none":s.addClass(f.panel+"_noanimation");break;default:s.addClass(f.panel+"_fx-"+n.panel.fx)}return n.panel.splash&&s.append('<div class="'+f.panel+'__searchsplash">'+n.panel.splash+"</div>"),this._initPanels(s),s},o[n].prototype._initSearchfield=function(e){var n=this.opts[_],s=this.conf[_];if(e.parent("."+f.listitem+"_vertical").length)return o();if(e.find("."+f.searchfield).length)return e.find("."+f.searchfield);var a=o("<"+(s.form?"form":"div")+' class="'+f.searchfield+'" />'),t=o('<div class="'+f.searchfield+'__input" />'),i=o('<input placeholder="'+this.i18n(n.placeholder)+'" type="text" autocomplete="off" />');return t.append(i).appendTo(a),e.prepend(a),e.hasClass(f.panel)&&e.addClass(f.panel+"_has-searchfield"),l(i,s.input),s.clear&&o('<a class="'+f.btn+" "+f.btn+"_close "+f.searchfield+'__btn" href="#" />').appendTo(t),l(a,s.form),s.form&&s.submit&&!s.clear&&o('<a class="'+f.btn+" "+f.btn+"_next "+f.searchfield+'__btn" href="#" />').appendTo(t),n.cancel&&o('<a href="#" class="'+f.searchfield+'__cancel">'+this.i18n("cancel")+"</a>").appendTo(a),a},o[n].prototype._initSearching=function(e){var n=this,s=this.opts[_],a=(this.conf[_],{});e.closest("."+f.panel+"_search").length?(a.$pnls=this.$pnls.find("."+f.panel),a.$nrsp=e.closest("."+f.panel)):e.closest("."+f.panel).length?(a.$pnls=e.closest("."+f.panel),a.$nrsp=a.$pnls):(a.$pnls=this.$pnls.find("."+f.panel),a.$nrsp=this.$menu),a.$pnls=a.$pnls.not(function(){return o(this).parent("."+f.listitem+"_vertical").length}),s.panel.add&&(a.$pnls=a.$pnls.not("."+f.panel+"_search"));var t=e.find("input"),i=e.find("."+f.searchfield+"__cancel"),l=this.$pnls.children("."+f.panel+"_search"),d=a.$pnls.find("."+f.listitem);a.$itms=d.not("."+f.listitem+"_divider"),a.$dvdr=d.filter("."+f.listitem+"_divider"),s.panel.add&&s.panel.splash&&t.off(h.focus+"-"+_+"-splash").on(h.focus+"-"+_+"-splash",function(e){n.openPanel(l)}),s.cancel&&(t.off(h.focus+"-"+_+"-cancel").on(h.focus+"-"+_+"-cancel",function(e){i.addClass(f.searchfield+"__cancel-active")}),i.off(h.click+"-"+_+"-splash").on(h.click+"-"+_+"-splash",function(e){e.preventDefault(),o(this).removeClass(f.searchfield+"__cancel-active"),l.hasClass(f.panel+"_opened")&&n.openPanel(n.$pnls.children("."+f.panel+"_opened-parent").last())})),s.panel.add&&"panel"==s.addTo&&this.bind("openPanel:finish",function(e){e[0]===l[0]&&t.focus()}),t.data(u.searchfield,a).off(h.input+"-"+_).on(h.input+"-"+_,function(e){(function(e){switch(e){case 9:case 16:case 17:case 18:case 37:case 38:case 39:case 40:return!0}return!1})(e.keyCode)||n.search(t)}),this.search(t)},o[n].prototype._initNoResultsMsg=function(e){var n=this.opts[_];this.conf[_];if(e.closest("."+f.panel).length||(e=this.$pnls.children("."+f.panel).first()),!e.children("."+f.panel+"__noresultsmsg").length){var s=e.children("."+f.listview).first(),a=o('<div class="'+f.panel+"__noresultsmsg "+f.hidden+'" />').append(this.i18n(n.noResults));s.length?a.insertAfter(s):a.prependTo(e)}},o[n].prototype.search=function(a,s){var t=this,i=this.opts[_];this.conf[_];a=a||this.$menu.find("."+f.searchfield).chidren("input").first(),s=(s=s||a.val()).toLowerCase().trim();var e=a.data(u.searchfield),n=a.closest("."+f.searchfield).find("."+f.btn),l=this.$pnls.children("."+f.panel+"_search"),d=e.$pnls,h=e.$itms,r=e.$dvdr,c=e.$nrsp;if(h.removeClass(f.listitem+"_nosubitems").find("."+f.btn+"_fullwidth-search").removeClass(f.btn+"_fullwidth-search "+f.btn+"_fullwidth"),l.children("."+f.listview).empty(),d.scrollTop(0),s.length){if(h.add(r).addClass(f.hidden),h.each(function(){var e=o(this),n="a";(i.showTextItems||i.showSubPanels&&e.find("."+f.btn+"_next"))&&(n="a, span"),-1<e.children(n).not("."+f.btn+"_next").text().toLowerCase().indexOf(s)&&e.removeClass(f.hidden)}),i.panel.add){var p=o();d.each(function(){var e=t.__filterListItems(o(this).find("."+f.listitem)).clone(!0);e.length&&(i.panel.dividers&&(p=p.add('<li class="'+f.listitem+" "+f.listitem+'_divider">'+o(this).find("."+f.navbar+"__title").text()+"</li>")),p=p.add(e))}),p.find("."+f.mm("toggle")).remove().end().find("."+f.mm("check")).remove().end().find("."+f.btn).remove(),l.children("."+f.listview).append(p),this.openPanel(l)}else i.showSubPanels&&d.each(function(e){var n=o(this);t.__filterListItems(n.find("."+f.listitem)).each(function(){var e=o(this).data(u.child);e&&e.find("."+f.listview).children().removeClass(f.hidden)})}),o(d.get().reverse()).each(function(e){var n=o(this),s=n.data(u.parent);s&&(t.__filterListItems(n.find("."+f.listitem)).length?s.hasClass(f.hidden)&&s.removeClass(f.hidden).children("."+f.btn+"_next").not("."+f.btn+"_fullwidth").addClass(f.btn+"_fullwidth").addClass(f.btn+"_fullwidth-search"):a.closest("."+f.panel).length||((n.hasClass(f.panel+"_opened")||n.hasClass(f.panel+"_opened-parent"))&&setTimeout(function(){t.openPanel(s.closest("."+f.panel))},(e+1)*(1.5*t.conf.openingInterval)),s.addClass(f.listitem+"_nosubitems")))}),this.__filterListItems(d.find("."+f.listitem)).each(function(){o(this).prevAll("."+f.listitem+"_divider").first().removeClass(f.hidden)});n.removeClass(f.hidden),c.find("."+f.panel+"__noresultsmsg")[h.not("."+f.hidden).length?"addClass":"removeClass"](f.hidden),i.panel.add&&(i.panel.splash&&l.find("."+f.panel+"__searchsplash").addClass(f.hidden),h.add(r).removeClass(f.hidden))}else h.add(r).removeClass(f.hidden),n.addClass(f.hidden),c.find("."+f.panel+"__noresultsmsg").addClass(f.hidden),i.panel.add&&(i.panel.splash?l.find("."+f.panel+"__searchsplash").removeClass(f.hidden):a.closest("."+f.panel+"_search").length||this.openPanel(this.$pnls.children("."+f.panel+"_opened-parent").last()));this.trigger("updateListview")}}(jQuery);
window.instantiateMenu = function () {    
    var getBottomContent = function () {
        var bottomContent = '<div class="mx-auto p-0">';
        var hasSomething = false;
        if (window.tetaConfigs.brandContent) {
            bottomContent += window.tetaConfigs.brandContent;
            hasSomething = true;
        }
        if (window.tetaConfigs.version) {
            bottomContent += "<div class='version-display'>" +
                window.tetaI18n.t('Version') +
                " : " +
                window.tetaConfigs.version +
                " </div>";
            hasSomething = true;
        }
        bottomContent += '</div>';
        return hasSomething ? bottomContent : null;
    };

    if (!window.tetaConfigs)
        throw new Error('window.tetaConfigs is needed');
    var navbars = [
        {
            "position": "top",
            "content": [
                "searchfield"
            ]
        },
        {
            "position": "top",
            "content": [
                "breadcrumbs"
            ]
        }
    ];
    var bottomContent = getBottomContent();
    if (bottomContent) {
        navbars.push(
            {
                "position": "bottom",
                "content": bottomContent
            });
    }
    $('nav#menu').mmenu({
        offCanvas: {
            position: window.tetaConfigs.isRtl ? "right" : "left",
            "zposition": "front"
        },
        "navbars": navbars,
        "extensions": [
            "position-front",
            window.tetaConfigs.isRtl ? "position-right" : "position-left"
        ]
    },
        {
            "searchfield": {
                "clear": true,
                "panel": true
            },
            language: window.tetaConfigs.neutralCulture
        });
};

(function () {

    var disableStickyOnEdge = function () {
        if (/Edge\/\d./i.test(navigator.userAgent)) {
            $('.t-table-wrapper .sticky-row th').css({ position: 'relative' });
        }
    };
    $(disableStickyOnEdge);

    var rootUrl = $('meta[name="root"]').prop("content");
    if (!rootUrl)
        return;
    if (rootUrl.slice(-1) !== "/") {
        rootUrl += "/";
    }
    window.rootUrl = rootUrl;
})();

window.handleAvatarError = function(img) {
	 var $img = $(img);
	 if ($img.attr('src').indexOf('login.png') === -1)
		 $img.attr('src', window.top.rootUrl + 'Content/common-assets-includes/images/global/login.png');
}

window.MainPartLoadingManager = function() {
    var shouldWait = false;

    var showMain = function() {
        $("#loadingPart").addClass("hidden");
        $("#mainPartOfTheForm").removeClass("hidden");
    };

    var showLoader = function() {
        $("#loadingPart").removeClass("hidden");
    };

    var setShouldWait = function(b) {
        shouldWait = b;
    };

    var showMainIfReady = function() {
        if (!shouldWait)
            showMain();
    }

    return {
        showMain: showMain,
        showLoader: showLoader,
        setShouldWait: setShouldWait,
        showMainIfReady: showMainIfReady
    };
}();

$(document).ready(function() {
    $(function() {
        window.MainPartLoadingManager.showMainIfReady();
        if (typeof window.instantiateMenu === 'function')
		    window.instantiateMenu();
    });
});

window.PageAddressManager = function () {
    _address = {};

    var checkAddressType = function (address) {
        var mandatoryFields = ['namespaceId', 'entityId', 'pageType', 'pageId'];
        for (var i = 0; i < mandatoryFields.length; i++) {
            if (!address[mandatoryFields[i]])
                throw new Error('No ' + mandatoryFields[i] + ' in address');
        }
    }

    var setPageAddress = function(address) {
        checkAddressType(address);
        _address = address;
    };

    var getNamespaceId = function() {
        return _address.namespaceId ||
            $('input[name="NamespaceId"]').val();
    };
    var getEntityId = function() {
        return _address.entityId ||
            $('input[name="EntityId"]').val();
    };
    var getFormSubjectId = function () {
        return _address.formSubjectId || $('input[name="FormSubjectId"]').val();
    };
    var getPageId = function () {
        return _address.pageId ||
            $('input[name="FormId"]').val() ||
            $('input[name="ReportId"]').val() ||
            $('input[name="DashboardId"]').val();
    };
    var getPageType = function() {
        return _address.pageType;
    };

    var getPageParameters = function() {
        switch (getPageType()) {
            case 'Form':
                return 'FormId=' + getPageId() + '&FormSubjectId=' + getFormSubjectId();
            case 'Report':
                return 'ReportId=' + getPageId();
            case 'Dashboard':
                return 'DashboardId=' + getPageId();
        }
        throw new Error('Page type not identified: ' + getPageType());
    }

    var getQueryParameters = function () {
        return 'NamespaceId=' +
            getNamespaceId() +
            '&EntityId=' +
            getEntityId() +
            '&' +
            getPageParameters();
	 };

    var getPageCode = function () {
        return getNamespaceId() +
            '-' +
            getEntityId() +
            '-' +
            getPageId();
	 };

    var navigateTo = function(url, isAlreadyAbsolute, newTab) {
		 var absoluteUrl = isAlreadyAbsolute ? url : window.top.rootUrl + url;
        if (newTab || (window.event && window.event.ctrlKey))
		    window.open(absoluteUrl, '_blank', 'noopener');
	    else
		    location.href = absoluteUrl;
    };

    var getFormUrl = function (address) {
        return 'Form' + (address.type ? ('/' + address.type) : '') + '?' +
            'NamespaceId=' +
            address.namespace  +
            '&EntityId=' +
            address.entity +
            (address.formId ? ('&FormId=' + address.formId) : '') +
            (address.ids ? ('&Ids=' + address.ids) : '') +
            (address.subject ? ('&FormSubjectId=' + address.subject) : '');
    };
    
    var navigateToForm = function (address, newTab) {
        var url = getFormUrl(address);
        navigateTo(url, false, newTab);
    };

    return {
        setPageAddress: setPageAddress,
        getNamespaceId: getNamespaceId,
        getEntityId: getEntityId,
        getFormSubjectId: getFormSubjectId,
        getPageId: getPageId,
        getPageType: getPageType,
        getQueryParameters: getQueryParameters,
        getPageCode: getPageCode,
        
        getFormUrl: getFormUrl,
        navigateTo: navigateTo,
        navigateToForm: navigateToForm
    };
}();

function checkboxChanged(element) {
	var $element = $(element);
	$element.val(element.checked);
	$element.attr("value", element.checked ? "true" : "false");
	var $hiddenField = $("input[name='" + $element.attr('associated-hidden-name') + "']");
	$hiddenField.val(element.checked ? true : false);
	$hiddenField.attr("value", element.checked ? true : false);
	$hiddenField.trigger('change');
}

(function() {
    window.tetaI18n = function() {
        var tetaJsTexts = {};

		  var fetchText = function (txtKey) {
				var value = tetaJsTexts[txtKey];
				if (!value) {
					 console.warn(txtKey + ' was not found in the tetaI18n dictionary.');
					 value = txtKey;
				}
				if (arguments.length > 1) {
					for (var i = 1; i < arguments.length; i++) {
						value = value.replace('{' + (i - 1) + '}', arguments[i]);
					}
				}
            return value;
        };

        var addDictionary = function(dicObj) {
            for (var key in dicObj)
                if (dicObj.hasOwnProperty(key))
                    tetaJsTexts[key] = dicObj[key];
        };

        var setCurrentCulture = function (cultureName) {
            //            $.ajax({
            //                type: "POST",
            //                url: '/Account/ChangeCulture?culture=' + cultureName,                
            //                success: function (result) {
            //                    location.reload();
            //                }
            //            });
            window.Cookies.set('_culture', cultureName, { expires: 365 });
            location.reload();
        };

		  return {
			  /**
			   * Fetches the text in the current language
			   * @param {string} txtKey
				* @param {...string} arguments for values which are can be in a format similar to C# string.format like: 'Hello {0}!'
			   */
            t: fetchText,
              addDictionary: addDictionary,
              setCurrentCulture: setCurrentCulture
        };
    }();
})();
// CEF stands for Client-side Expression Functions
window.CEF = function () {
    
    var isTableEmpty = function (name) {
        return $('table[name="' + name + '"] tbody tr:not([doppelganger])').length === 0;
    };

    return {
        IsTableEmpty: isTableEmpty
    };
}();
window.FormUtils = function() {
	var getDefaultValue = function($input) {
		return $input.data('default-value');
	};

	var trySetOptionIfExists = function($select, optionValue) {
		if ($select.find('option[value="' + optionValue + '"]').length > 0) {
			$select.val(optionValue);
			return true;
		}
		return false;
	};

	var clearSelect = function($select) {
		if ($select.is('[multiple]')) {
			$select.find('option:selected').removeAttr("selected");
		} else {
			var hadOptionalValue = trySetOptionIfExists($select, '0') || trySetOptionIfExists($select, '');
			if (!hadOptionalValue /*&& $select.is('[nodatamandatory]')*/) {
				$select.prop('selectedIndex', -1);
			}
		}
		var defaultValue = getDefaultValue($select);
		if (defaultValue)
			$select.val(defaultValue);
		$select.trigger('change');
	};

	var clearInput = function($input) {
		$input.val('');
		$input.trigger('change');
	};

	var setButtonActiveIfExisted = function($input, value) {
		var $buttonLabel = $input.parent('label.btn');
		$buttonLabel.removeClass('active');
		if (value) {
			$buttonLabel.addClass('active');
		}
	};

	var clearCheckBoxOrRadio = function($input) {
		var defaultValue = getDefaultValue($input);
		if (defaultValue === undefined) {
			$input.removeAttr('checked').removeAttr('selected');
			setButtonActiveIfExisted($input, false);
		} else if (defaultValue === 'checked') {
			$input.prop('checked', 'checked');
			setButtonActiveIfExisted($input, true);
		} else if (defaultValue === 'selected') {
			$input.prop('selected', 'selected');
		} else {
			console.error('Invalid default value');
			return;
		}
		$input.trigger('change');
	};

	var clearField = function($field) {
		if ($field.is('input:text, input:password, input:file, textarea, input[type="number"]'))
			clearInput($field);
		else if ($field.is('input:radio, input:checkbox'))
			clearCheckBoxOrRadio($field);
		else if ($field.is('select'))
			clearSelect($field);
	};

	var clearFields = function($fields) {
		$fields.each(function(idx, input) {
			clearField($(input));
		});
	};

	var clearFieldsOf = function(selector) {
		var $form = $(selector);
		var $fields = $form.find(
			'input:text, input:password, input:file, textarea, input[type="number"], input:radio, input:checkbox, select');
		clearFields($fields);
	};

	var getSelectValue = function($select) {
		if ($select.attr('data-isremote') && $select.attr('initvalue') && !$select.attr('init-fetched')) {
			return $select.attr('initvalue');
		}
		return $select.val();
		
	};

	return {
		clearField: clearField,
		clearFields: clearFields,
		clearFieldsOf: clearFieldsOf,
		getSelectValue: getSelectValue
	};
}();

var OnloadUiRulesManager = function() {
	var ajaxIsRunning = false;
	var alreadyLoaded = false;

	var setAjaxRunning = function(b) {
		ajaxIsRunning = b;
		window.MainPartLoadingManager.setShouldWait(b);
	};

	var thingsToDoAfterUiRulesAreDone = function() {
		$('form').trigger('reinitialize.areYouSure');
		window.MainPartLoadingManager.showMain();
	};

	var rulesAreDone = function() {
		if (ajaxIsRunning) {
			return;
		}
		if (alreadyLoaded) {
			console.warn('Already loaded!');
			return;
		}
		alreadyLoaded = true;
		thingsToDoAfterUiRulesAreDone();
	};

	var ajaxRulesAreDone = function() {
		setAjaxRunning(false);
		rulesAreDone();
	};

	return {
		rulesAreDone: rulesAreDone,
		setAjaxRunning: setAjaxRunning,
		ajaxRulesAreDone: ajaxRulesAreDone
	}
}();

lc = {};

function runAjax(NamespaceId, EntityId, FormId, source, eventName, cb, scope, paramList) {
	OnloadUiRulesManager.setAjaxRunning(true)
	var AjaxParams = {
		source: source ? source : "",
		eventName: eventName ? eventName : "",
		NamespaceId: NamespaceId ? NamespaceId : "",
		EntityId: EntityId ? EntityId : "",
		FormId: FormId ? FormId : "",
		Qs: getQs(paramList),
		LCs: getLCs()
	}
	var obj = JSON.stringify(AjaxParams);
	$.ajax({
		type: 'POST',
		url: window.top.rootUrl + 'Form/GetOperations',
		data: obj,
		headers: AddAntiForgeryToken(),
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		async: true,
		processData: true,
		cache: false,
		success: function(data) {
			try {
				if (data)
					cb(data, scope);
			} catch (e) {
				console.log(e);
			}
			OnloadUiRulesManager.ajaxRulesAreDone();
		},
		error: function(xhr) {
			OnloadUiRulesManager.ajaxRulesAreDone();
			Error(xhr);
			//alert('عدم ارتباط با سرور.');
		}
	});
}

function getLCs() {
	var LCs = [];
	for (i in lc) {
		if (i == "proptotype") continue;
		LCs.push({ Field: i, Value: lc[i] });
	}
	return LCs;
}

function getQs(paramList) {
	var qList = [];
	qList.push({ Field: "__parentIds", Value: window.top.logicParentIds });
	if (paramList) {
		var params = paramList.split(',');
		if (params && params.length > 0) {
			for (var i = 0, len = params.length; i < len; i++) {
				var element = $("[name='" + params[i] + "']");
				AddElementToQList(qList, element, params[i]);
			}
		}
	} else {
		var allElem =
			$(
				'select[name], input[name][type="text"], input[name][type="hidden"], input[name][type="number"], textarea');
		if (allElem && allElem.length > 0) {
			for (var i = 0, len = allElem.length; i < len; i++) {
				AddElementToQList(qList, $(allElem[i]), null);
			}
		}
	}
	//console.log("getQs:" );
	//console.log(qList);
	return qList;
}

function AddElementToQList(qList, eleman, name) {
	//if (name) {
	//	console.log("AddElementToQList:" + name + ":" );
	//	console.log(eleman);
	//}
	var obj = {};
	obj.Field = name ? name : eleman.attr('name');
	if (!obj.Field) return;
	var v = null;
	if (eleman.is('select')) {
		var elemanfind = eleman.find("option:selected");
		if (elemanfind) {
			if (eleman.attr('multiple') === 'multiple') {
				
				if (FormUtils.getSelectValue(eleman).length)
					v = "{" + eleman.val() + "}"; // majboor shodam majboor 
				else
					v = String(eleman.val());
			} else
				v = FormUtils.getSelectValue(eleman);
		}
	} else {
		v = eleman.val();
	}
	obj.Value = v;
	//if (name) {
	//	console.log(obj);
	//}
	if (obj.Field && obj.Value)
		qList.push(obj);
}

function getFieldValue(pId, elem, scope, bReport) {
	var table;
	var rowIndex;
	var cellIndex;
	if (scope || bReport) {
		rowIndex = $(elem).closest("tr").index();
		if (bReport) {
			table = $(elem).closest("table");
			var targetElem = $("*[name='" + pId + "']");
			var targetVal = targetElem ? $(targetElem).val() : "";
			if (targetVal) return targetVal;
			if (pId == 'Ids') {
				return $(elem).closest("tr").attr("ids");
			}
			$(table).find("th").each(function(idx, el) {
				var colname = $(el).data("colname");
				if (colname == pId) {
					cellIndex = $(el).index();
					return;
				}
			});
			targetElem = $($(elem).closest("tr ").find("td")[cellIndex]).find("input,select");
			if (targetElem) {
				targetVal = $(targetElem).val();
			} else {
				targetVal = $($(elem).closest("tr ").find("td")[cellIndex]).find("span").html();
			}
			return targetVal;
			//return $(elem).closest("tr").attr("ids");
		}
		if (scope) {
			table = $('table[name="' + scope + '"]');
			var p = $($(table).find("tr")[rowIndex]).find('input[name="' +
				scope +
				'[' +
				(rowIndex - 1) +
				'].' +
				pId +
				'"],select[name="' +
				scope +
				'[' +
				(rowIndex - 1) +
				'].' +
				pId +
				'"]').val();
			return p;
		}
	} else {
		return $("*[name='" + pId + "']").val();
	}
}

function fetchLC(lcId) {
	return lc["lc_" + lcId];
}

function getLCValue(lcId) {
	return lc[lcId];
}

function setLCValue(lcId, val) {
	return lc[lcId] = val;
}

function IsTrue(elem) {
	if (elem)
		return true;
	return false;
}

function IsFalse(elem) {
	if (elem)
		return false;
	return true;
}

function preProcessFormula(formula, elem, bReport) {
	if (!formula) return "";
	if (formula.lastIndexOf("'", 0) === 0)
		return eval(formula);
	var patt = /q\[\w*\]|qm\[\w*\]|qpkv\[\w*\]|qmpkv\[\w*\]/ig;
	var result = formula.match(patt);
	if (result instanceof Array) {
		for (var i in result) {
			if (result[i].replace) {
				var pId = result[i].replace("q[", "").replace("qm[", "").replace("]", "");
				var s = getFieldValue(pId, elem, null, bReport);
				if (!s) {
					s = "0";
				}
				if (isNaN(s)) {
					s = "'" + s + "'";
				}
				formula = formula.replace(result[i], s ? s : "0");
			}
		}
	} else if (result) {
		var pId = result.replace("q[", "").replace("qm[", "").replace("]", "");
		var s = getFieldValue(pId);
		if (!s) {
			s = "0";
		}
		formula = formula.replace(result, s);
	}
	patt = /lc\[[0-9]*\]/ig;
	result = formula.match(patt);
	if (result instanceof Array) {
		for (var i in result) {
			var lcId = result[i].replace("lc[", "").replace("]", "");
			var s = getLCValue(lcId);
			if (!s)
				s = "0";
			formula = formula.replace(result[i], s)
		}
	} else if (result) {
		var lcId = result.replace("lc[", "").replace("]", "");
		var s = this.getLCValue(lcId);
		if (!s)
			s = "0";
		formula = formula.replace(result, s);
	}
	{
		var re = /`/ig;
		formula = String(formula).replace(re, "'");
		if (1 == 1) //is better find the condition for check this block
		{
			re = /;/ig;
			formula = String(formula).replace(re, ",");
		}
		formula = String(formula).replace(/=/g, "==");
		formula = String(formula).replace(/>==/g, ">=");
		formula = String(formula).replace(/<==/g, "<=");
		formula = String(formula).replace(/!==/g, "!=");

		formula = String(formula).replace(/and/ig, " && ");
		formula = String(formula).replace(/or/ig, " || ");
		formula = String(formula).replace(/Number/ig, "Number");
		formula = String(formula).replace(/equal/ig, "FormFunctions.equal");
		//formula = String(formula).replace(/SUM/ig, "SUM");
		//formula = String(formula).replace(/SUBTOTAL/ig, "SUBTOTAL");
		//formula = String(formula).replace(/COUNT/ig, "COUNT");
		//formula = String(formula).replace(/EQUAL/ig, "EQUAL");
		//formula = String(formula).replace(/\[\]/ig, "\"\"");
		//if (formula.indexOf("p[") == -1) {
		//    formula = String(formula).replace(/\[/ig, "\"");
		//    formula = String(formula).replace(/\]/ig, "\"");
		//}
	}
	//while (true) {
	//    var idx = formula.indexOf("Clientfuncs_");
	//    if (idx < 0) break;
	//    if (formula.substr(idx, 7) == "GetIndex") {
	//        formula = formula.replace("Clientfuncs_GetIndex()", "cf_GetIndex(scopeId,tIndex,form)");
	//    }
	//}
	return eval(formula);
}

function getControlContainer(fieldName) {
	console.log('[getControlContainer v2.0] fieldName:', fieldName);
	// اول از data-id استفاده کن که container کامل فیلد است
	var $dataIdContainer = $('[data-id="' + fieldName + '"]');
	console.log('[getControlContainer v2.0] $dataIdContainer.length:', $dataIdContainer.length);
	if ($dataIdContainer.length) {
		console.log('[getControlContainer v2.0] Using data-id container');
		return $dataIdContainer;
	}
	console.log('[getControlContainer v2.0] Falling back to getSpecifierContainer');
	return getSpecifierContainer("name=\"" + fieldName + "\"");
}

function getSpecifierContainer(specifier) {
	console.log('[getSpecifierContainer v2.1] specifier:', specifier);
	
	// اگر specifier به فرمت name="fieldName" است، fieldName را استخراج کن و از data-id استفاده کن
	var nameMatch = specifier.match(/name="([^"]+)"/);
	if (nameMatch) {
		var fieldName = nameMatch[1];
		var $dataIdContainer = $('[data-id="' + fieldName + '"]');
		console.log('[getSpecifierContainer v2.1] fieldName:', fieldName, '$dataIdContainer.length:', $dataIdContainer.length);
		if ($dataIdContainer.length) {
			console.log('[getSpecifierContainer v2.1] Using data-id container for:', fieldName);
			return $dataIdContainer;
		}
	}
	
	// fallback به روش قبلی
	var $container = $("[" + specifier + "]").closest(".neo-control");
	console.log('[getSpecifierContainer v2.1] neo-control.length:', $container.length);
	if ($container.length)
		return $container;
	return $("[" + specifier + "]").closest("div").parent();
}

function runSingleOperation(op, specifier) {
	var getHtmlProperty = function(attr) {
		switch (attr) {
		case 'ReadOnly':
			return 'disabled';
		default:
			throw new Error('Attribute not supported');
		}
	};
	switch (op.Type) {
	case 11: //"SetValue":
		if (op.Value === null || op.Value === '')
			window.FormUtils.clearFields($("[" + specifier + "]"));
		else {
			var $control = $("*[" + specifier + "]");
			if ($control.is('[data-isremote]')) {
				window.Select2Beneficiary.setRemoteValues(op.Value, $control);
			} else {
				$control.val(op.Value).trigger('change');
			}
		}
		break;
	case 12: //"ToggleClass":
		switch (op.TargetArea) {
		case 1: //Container
			getSpecifierContainer(specifier).toggleClass(op.Attr);
			break;
		case 2: //Input
			$("*[" + specifier + "]").toggleClass(op.Attr);
			break;
		case 3: //Label
			$("*[" + specifier + "]").parent().find("label").toggleClass(op.Attr);
			break;
		case 4: //column
			$("button[refer='" + op.TargetId + "']").parent().toggleClass(op.Attr);
			//to apply style to all rows of that column use the below Code :
			var colIdx = $("button[refer='" + op.TargetId + "']").closest('th').index();
			$("button[refer='" + op.TargetId + "']")
				.closest('table')
				.find('tbody')
				.find("tr")
				.each(function(idx, elem) {
					if (idx == 0) {
					} else {
						$($(elem).find("td")[colIdx]).toggleClass(op.Attr);
					}
				});
			//...
			break;
		case 5: //cell
			break;
		}
		break;
	case 13: //"SetClass":
		switch (op.TargetArea) {
		case 1: //Container
			getSpecifierContainer(specifier).addClass(op.Attr);
			break;
		case 2: //Input
			$("*[" + specifier + "]").addClass(op.Attr);
			break;
		case 3: //Label
			$($("*[" + specifier + "]").parent()).find("span").addClass(op.Attr);
			break;
		case 4: //column
			//just column Header
			$("button[refer='" + op.TargetId + "']").parent().addClass(op.Attr);
			//to apply style to all rows of that column use the below Code :
			var colIdx = $("button[refer='" + op.TargetId + "']").closest('th').index();
			$("button[refer='" + op.TargetId + "']").closest('table').find('tbody').find("tr").each(
				function(idx, elem) {
					if (idx == 0) {
					} else {
						$($(elem).find("td")[colIdx]).addClass(op.Attr);
					}
				})
			//...
			break;
		case 5: //cell

			break;
		}
		break;
	case 14: //"RemoveClass":
		switch (op.TargetArea) {
		case 1: //Container
			getSpecifierContainer(specifier).removeClass(op.Attr);
			break;
		case 2: //Input
			$("*[" + specifier + "]").removeClass(op.Attr);
			break;
		case 3: //Label
			$($("*[" + specifier + "]").parent()).find("span").removeClass(op.Attr);
			break;
		case 4: //column
			$("button[refer='" + op.TargetId + "']").parent().removeClass(op.Attr);
			//to apply style to all rows of that column use the below Code :
			var colIdx = $("button[refer='" + op.TargetId + "']").closest('th').index();
			$("button[refer='" + op.TargetId + "']").closest('table').find('tbody').find("tr").each(
				function(idx, elem) {
					if (idx == 0) {
					} else {
						$($(elem).find("td")[colIdx]).removeClass(op.Attr);
					}
				})
			//...
			break;
		case 5: //cell
			break;
		}
		break;
	case 15: //"ChangeCSSAttribute":
		switch (op.TargetArea) {
		case 1: //Container
			getSpecifierContainer(specifier).css(op.Attr, op.Value);
			break;
		case 2: //Input
			$("*[" + specifier + "]").css(op.Attr, op.Value);
			break;
		case 3: //Label
			$($("*[" + specifier + "]").parent()).find("span").css(op.Attr, op.Value);
			break;
		case 4: //column
			$("button[refer='" + op.TargetId + "']").parent().css(op.Attr, op.Value);
			//to apply style to all rows of that column use the below Code :
			var colIdx = $("button[refer='" + op.TargetId + "']").closest('th').index();
			$("button[refer='" + op.TargetId + "']").closest('table').find('tbody').find("tr").each(
				function(idx, elem) {
					if (idx == 0) {
					} else {
						$($(elem).find("td")[colIdx]).css(op.Attr, op.Value);
					}
				})
			//...
			break;
		case 5: //cell
			break;
		}

		break;
	case 17: //"SetProperty":
		var attrVal = (op.Value.toLowerCase() === 'false') && (op.Attr === 'ReadOnly') ? false : op.Value;
		$("[" + specifier + "]").prop(getHtmlProperty(op.Attr), attrVal);
		$("[associated-hidden-name='" + op.TargetId + "']").prop(getHtmlProperty(op.Attr), attrVal);
		break;
	case 21: //"ShowWarning":
		alert(op.Value);
		break;
	case 22: //"ShowError":
		alert(op.Value);
		return;
	case 23: //"ShowPrompt":
		if (!prompt(op.Value)) {
			return;
		}
		break;
	case 24: //"ShowModal":
		break;
	case 25: //"ShowWindow":
		break;
	case 26: //"ShowAutoHideWindow":
		break;
	case 31: //"AddRow":
		break;
	case 32: //"ChangeList":
		var $select = $("select[" + specifier + "]");
		$select.attr("filter-formula", op.Formula);
		var value = $select.val() || $select.attr("initvalue");
		if ($select.is('[data-isremote]')) {
			window.Select2Beneficiary.setRemoteValues(value, $select);
		} else {
			$select.html("");
			for (var i = 0, len = op.Rows.length; i < len; i++) {
				var ids = op.Rows[i].Ids;
				$select.append('<option value=' + ids + '>' + op.Rows[i].DisplayValue + '</option>');
			}
			if (!value || !value.length) return;
			if (typeof value === 'string')
				value = value.split(',');
			$select.val(value);
			$select.trigger('change');
		}
		break;
	}
}

function runOperation(op, scope) {
	var specifier = "name='" + op.TargetId + "'";
	if (!scope)
		return runSingleOperation(op, specifier);

	$.each($('[full-column-name="' + scope + '_' + op.TargetId + '"]'),
		function(idx, item) {
			specifier = "name='" + $(item).prop('name') + "'";
			runSingleOperation(op, specifier);
		}
	);
}

function runOperations(operations, scope) {
	for (var i in operations.Items) {
		var op = operations.Items[i];
		runOperation(op, scope);
	}
}

var eTargetArea =
{
	Container: 1,
	Input: 2,
	Label: 3,
	Column: 4,
	Cell: 5,
	HeaderCell: 6,
};

function Client_AddClass(This, scope, fieldName, className, targetArea, formula) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		var hasRequire = $(targetField).attr("required") || $(targetField).attr("hasrequired");
		if (formula) {
			if (className == "ShowHide") {
				if (hasRequire) {
					$(targetField).attr("hasrequired", true);
					$(targetField).attr("disabled", "disabled");
				}
			}
			$(targetField).addClass(className);
		} else {
			if (className == "ShowHide") {
				if (hasRequire) {
					$(targetField).attr("required", "required");
					$(targetField).removeAttr("disabled");
				}
			}
			$(targetField).removeClass(className);
		}
	} else if (className == "ShowHide" && (targetArea == "Container" || targetArea == "Input")) {
		var $inputField = $("*[name=" + fieldName + "]");
		if ($inputField.prop('type') === 'hidden') {
			$inputField = $("[associated-hidden-name=" + fieldName + "]");
		}
		var hasRequire = $inputField.attr("required") || $inputField.attr("hasrequired");
		if (formula) {
			if (hasRequire) {
				$inputField.attr("disabled", "disabled");
				$inputField.attr("hasrequired", true);
			}
		} else {
			if (hasRequire) {
				$inputField.attr("required", "required");
				$inputField.removeAttr("disabled");
			}
		}
	}
	switch (targetArea) {
	case "Container":
		var $container;
		if (formula) {
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$container = $("*[name=" + fieldName + "]").closest("div").parent();
				$container.addClass(className);
			} else {
				if ($("[name=" + fieldName + "]").length)
					$container = getControlContainer(fieldName);
				else $container = $('[id="' + fieldName + '"]');
				$container.addClass(className);
			}
			// Also hide parent table cells/rows to free up space
			if (className == "ShowHide" && $container.length) {
				$container.closest("td, th").addClass(className);
				// If all cells in a row are hidden, hide the row too
				var $row = $container.closest("tr");
				if ($row.length) {
					var visibleCells = $row.find("td, th").not(".ShowHide").length;
					if (visibleCells === 0) {
						$row.addClass(className);
					}
				}
			}
		} else {
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$container = $("*[name=" + fieldName + "]").closest("div").parent();
				$container.removeClass(className);
			} else {
				if ($("[name=" + fieldName + "]").length)
					$container = getControlContainer(fieldName);
				else $container = $('[id="' + fieldName + '"]');
				$container.removeClass(className);
			}
			// Also show parent table cells/rows
			if (className == "ShowHide" && $container.length) {
				var $cell = $container.closest("td, th");
				$cell.removeClass(className);
				// Show the row if at least one cell is now visible
				var $row = $container.closest("tr");
				if ($row.length) {
					var visibleCells = $row.find("td, th").not(".ShowHide").length;
					if (visibleCells > 0) {
						$row.removeClass(className);
					}
				}
			}
		}
		break;
	case "Input":
		if (formula) {
			$("*[name=" + fieldName + "]").addClass(className);
		} else {
			$("*[name=" + fieldName + "]").removeClass(className);
		}
		break;
	case "Label":
		if (formula) {
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().find('span').addClass(className);
			} else {
				$("*[name=" + fieldName + "]").closest("div").find('span').addClass(className);
			}
		} else {
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().find('span').removeClass(className);
			} else {
				$("*[name=" + fieldName + "]").closest("div").find('span').removeClass(className);
			}

		}
		break
	case eTargetArea.Column:
		break
	case eTargetArea.Cell:
		break
	case eTargetArea.HeaderCell:
		break
	}
}

function Client_RemoveClass(This, scope, fieldName, className, targetArea, formula) {
	Client_AddClass(This, scope, fieldName, className, targetArea, !formula);
}

function Client_ChangeCSSAttr(This, scope, fieldName, attribute, targetArea, formula) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		$(targetField).css(attribute, formula);
	} else
		switch (targetArea) {
		case "Container":
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().css(attribute, formula);
			} else {
				$("*[name=" + fieldName + "]").closest("div").parent().css(attribute, formula);
			}

		case "Input":
			$("*[name=" + fieldName + "]").css(attribute, formula);
			break;
		case "Label":
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().find('span').css(attribute, formula);
			} else {
				$("*[name=" + fieldName + "]").closest("div").find('span').css(attribute, formula);
			}

			break;
		case eTargetArea.Column:
			break;
		case eTargetArea.Cell:
			break;
		case eTargetArea.HeaderCell:
			break;
		}
}

function Client_ChangeAttr(This, scope, fieldName, attribute, targetArea, formula) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		$(targetField).attr(attribute, formula);
	} else
		switch (targetArea) {
		case "Container":
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().attr(attribute, formula);
			} else {
				$("*[name=" + fieldName + "]").closest("div").attr(attribute, formula);
			}

		case "Input":
			$("*[name=" + fieldName + "]").attr(attribute, formula);
			break;
		case "Label":
			if ($("*[name=" + fieldName + "]").closest("div.upload")[0]) {
				$("*[name=" + fieldName + "]").closest("div").parent().find('span').attr(attribute, formula);
			} else {
				$("*[name=" + fieldName + "]").closest("div").find('span').attr(attribute, formula);
			}

			break;
		case eTargetArea.Column:
			break;
		case eTargetArea.Cell:
			break;
		case eTargetArea.HeaderCell:
			break;
		}
}

function Client_GetVal(This, scope, fieldName) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		return $(targetField).val();
	} else
		return $("*[name=" + fieldName + "]").val();
}

function Client_SetVal(This, scope, fieldName, formula) {
	if (scope) {
		var rowIndex = $(This).closest("tr").index();
		var table = $('table[name="' + scope + '"]');
		var targetField = $($(table).find("tr")[rowIndex]).find('input[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"],select[name="' +
			scope +
			'[' +
			(rowIndex - 1) +
			'].' +
			fieldName +
			'"]');
		return $(targetField).val(formula).trigger('change');
	} else
		$("*[name=" + fieldName + "]").val(formula).trigger('change');
}

function Client_SetLC(This, scope, lcName, formula) {
	lc[lcName] = formula;
}

function IsNullOrEmpty(obj) {
	if (obj)
		return false;
	return true;
}

function sysLink(sysLinkCt, bReport) {
	var urlAddress = "";
	urlAddress += $(sysLinkCt).attr("systemLinkAddress");
	var paramsObj = $(sysLinkCt).attr("linkParameter").split(',');
	var params = [];
	for (var i in paramsObj) {
		var param = paramsObj[i].toString();
		if (!param) continue;
		var paramPair = param.split('=');
		if (paramPair.length == 2) {
			var nF = preProcessFormula(paramPair[1].toString(), sysLinkCt, bReport);
			var nP = paramPair[0] + "=" + nF;
			params.push(nP);
		}
	}
	urlAddress += "&" + params.join("&");
	if (window.event && window.event.ctrlKey)
		window.open(urlAddress, '_blank');
	else
		location.href = urlAddress;
}

function InvalidMsg(elem) {
	if (elem) {
		elem.setCustomValidity('');
		if (elem.validity.patternMismatch || elem.validity.rangeOverflow || elem.validity.rangeUnderflow) {
			elem.setCustomValidity(window.tetaI18n.t('InvalidInputValue'));
		} else if (elem.value === '' || elem.value === null){
			elem.setCustomValidity(window.tetaI18n.t('RequiredError'));
		}
	} else {
		var e = window.event;
		if (e) {
			e.target.setCustomValidity('');
			if (!e.target.validity.valid) {
				if (e.target.validity.patternMismatch || elem.validity.rangeOverflow || elem.validity.rangeUnderflow) {
					e.target.setCustomValidity(window.tetaI18n.t('InvalidInputValue'));
				} else {
					e.target.setCustomValidity(window.tetaI18n.t('RequiredError'));
				}
			}
		}
	}
	return true;
};

function checkboxlistChanged(chk) {
	var chkName = $(chk).attr("name").split('_')[0];
	var mChkVal = $("input[name=" + chkName + "]").val();
	if (chk.checked) {
		mChkVal = mChkVal | $(chk).val();
	} else
		mChkVal = mChkVal & ~$(chk).val();

	$("input[name=" + chkName + "]").val(mChkVal);
}

function applySeperator(input, event) {
	try {
		var val = $(input).val(), out;
		val = val.replace(/,/g, "");
		var t = new RegExp(/^\d+$/);
		var s = t.test(val.toString());
		var len = val.length;
		val = val.split("");
		var reload = false;
		for (var j = 0, l = val.length; j < l; j++) {
			var v = val[j];
			var s = t.test(v);
			if (!s) {
				val.splice(j, 1);
				reload = true;
			}
		}
		if (reload) {
			$(input).val(val.join(""));
			applySeperator(input, event);
		}
		if (len <= 3) {
			return;
		}
		var NOS = parseInt(len / 3);
		if (NOS) {
			for (var i = 0; i < NOS; i++) {
				var lastIndex = val.indexOf(',');
				if (lastIndex === -1)
					val.splice((val.length - 3), 0, ',');
				else {
					val.splice((lastIndex - 3), 0, ',');
				}
				//val = out();
			}
			if (val[0] === ",") {
				val = val.slice(1, val.length);
			}
			out = val.join("");
			$(input).val(out);
		}
	} catch (e) {
		alert(e);
	}
}

function addComboIdsAndRedirect(initialUrl, comboId) {
	if (!initialUrl || !comboId) {
		console.warn("Wrong arguments: initialUrl=" + initialUrl + " comboId=" + comboId);
		return;
	}
	var $selectBox = $('select[name="' + comboId + '"]');
	var val = $selectBox.val();
	if (!val) {
		//alert('لطفا ابتدا یکی از مقادیر لیست را انتخاب نمایید.');
		window.toast.info(window.tetaI18n.t('FirstSelectAValue'));
		return;
	}
	var ids = Array.isArray(val) ? val[0] : val;

	//    console.log(initialUrl);
	//    console.log(comboId);
	//    console.log($('select[name="' + comboId +'"').val());
	window.PageAddressManager.navigateTo(initialUrl + '&ids=' + ids, true);
}

function initializePluginsDefaults() {
	if ($.fn.select2) {
		$.fn.select2.defaults.set("theme", "bootstrap4");
		$.fn.select2.defaults.set("containerCssClass", ":all:");
		$.fn.select2.defaults.set("language", window.tetaConfigs.neutralCulture);
	}
}

var Select2Beneficiary = function() {
	var templateResult = function(data, container) {
		if (data.cssClass) {
			var contextualClass = data.cssClass.toLowerCase();
			$(container).addClass('bg-' + contextualClass);
			if (['danger', 'success'].indexOf(contextualClass) !== -1) {
				$(container).addClass('text-white');
			}
		}
		return data.text;
	};

	var templateSelection = function(data, container) {
		if (data.cssClass) {
			$(container).addClass('bg-' + data.cssClass.toLowerCase());
		}
		return data.text;
	};

	var getAjaxObject = function(namespaceId, entityId, fieldId, formId, isMandatory, remoteUrl, filterFormula) {
		var ret = {
			url: remoteUrl || (window.top.rootUrl + 'form/GetComboData'),
			//			dataType: 'json',
			type: 'POST',
			//			contentType: "application/json; charset=utf-8",
			//			params: { // extra parameters that will be passed to ajax
			//				contentType: "application/json; charset=utf-8"
			//			},
			headers: window.AddAntiForgeryToken(),
			delay: 400,
			data: function(params) {
				var filterObj =
				{
					Expression: params.term, // search term
					Filter: $('[name="' + fieldId + '"]').attr('filter-formula') || filterFormula,
					NamespaceId: namespaceId,
					EntityId: entityId,
					FieldId: fieldId,
					PageType: window.PageAddressManager.getPageType(),
					FormId: formId,
					Count: 30,
					IsMandatory: isMandatory,
					Page: params.page || 1,
					Qs: [],
					LCs: []
				};
				if (filterObj.Filter) {
					filterObj.Qs = getQs();
					filterObj.LCs = getLCs();
				}
				return filterObj;
				//				return JSON.stringify(filterObj);
			},
			processResults: function(data, params) {
				params.page = params.page || 1;
				return {
					results: $.map(data.Rows,
						function(item) {
							return {
								text: item.DisplayValue,
								id: item.Ids,
								cssClass: item.Style
							}
						}),
					pagination: {
						more: data.HasMore //(params.page * 30) < data.total_count
					}
				};
			},
			minimumInputLength: 3,
			cache: true
		};
		return ret;
	};

	var selectRows = function(rows, $remoteSelect) {
		if (!rows) return;
		$.each(rows,
			function(idx, row) {
				var $rowOption = $remoteSelect.find("option[value='" + row.id + "']");
				if (!$rowOption.length) {
					//					var newOption = new Option(row.text, row.id, true, true);
					$remoteSelect.append($('<option>',
						{
							val: row.id,
							text: row.text,
							selected: true
						})) /*.trigger('change')*/;
				} else if (!$rowOption.is('[selected]')) {
					$remoteSelect.val(row.id).trigger('change');
				}
			});
	};

	var fetchInitValues = function(initialValues, namespaceId, entityId, fieldId, formId, filter) {
		var filterObj = {
			InitValues: initialValues,
			NamespaceId: namespaceId,
			EntityId: entityId,
			FieldId: fieldId,
			Filter: filter,
			PageType: window.PageAddressManager.getPageType(),
			FormId: formId,
			IsMandatory: false //?
		};
		if (filter) {
			filterObj.Qs = getQs();
		}
		return $.ajax({
			type: 'POST',
			headers: window.AddAntiForgeryToken(),
			url: window.top.rootUrl + "form/GetComboInitValues",
			data: filterObj
		}).then(function(response) {
			return response.Rows.map(function(row) {
				return {
					id: row.Ids,
					text: row.DisplayValue,
					cssClass: row.Style
				};
			});
		});
	};

	var instantiateRemoteDataSelect = function() {
		var thisRemoteSelect = this;
		var $thisRemoteSelect = $(thisRemoteSelect);
		var namespaceId = $thisRemoteSelect.data('namespace') || window.PageAddressManager.getNamespaceId();
		var entityId = $thisRemoteSelect.data('entity') || window.PageAddressManager.getEntityId();
		var fieldId = $thisRemoteSelect.data('column') ||
			$thisRemoteSelect.parents('.neo-control').data('id');

		var formId = $thisRemoteSelect.data('form') || window.PageAddressManager.getPageId();
		$thisRemoteSelect
			.select2({
				ajax: getAjaxObject(
					namespaceId,
					entityId,
					fieldId,
					formId,
					$thisRemoteSelect.attr("required") ? true : false,
					$thisRemoteSelect.data("remote-url"),
					$thisRemoteSelect.attr('filter-formula')),
				templateResult: templateResult,
				templateSelection: templateSelection
			});

		var initialValuesOfSelect = $thisRemoteSelect.attr('initvalue');
		if (Boolean(initialValuesOfSelect)) {
			fetchInitValues(initialValuesOfSelect,
					namespaceId,
					entityId,
					fieldId,
					formId,
					$thisRemoteSelect.attr("filter-formula"))
				.then(function(rows) {
						selectRows(rows, $thisRemoteSelect);
						$thisRemoteSelect.attr('init-fetched', true);
					},
					function(err) {
						console.error(err);
					});
		}
	};

	var instantiateControls = function() {
		if (!$('select').select2) return;
		$('select[data-isremote]')
			.each(instantiateRemoteDataSelect);

		$('select:not([data-isremote])')
			.select2();
	};

	//	var getMissingOptions = function(idsArray, $remoteSelect) {
	//		 return idsArray.filter(function(row) {
	//			 return $remoteSelect.find('option[value="' + row.id + '"][selected="selected"]').length === 0;
	//		});
	//	};

	var setRemoteValues = function(values, $remoteSelect) {
		if (!values)
			return;
		if (Array.isArray(values))
			values = values.toString();
		var namespaceId = $remoteSelect.data('namespace') || window.PageAddressManager.getNamespaceId();
		var entityId = $remoteSelect.data('entity') || window.PageAddressManager.getEntityId();
		var formId = $remoteSelect.data('form') || window.PageAddressManager.getPageId();
		fetchInitValues(values,
				namespaceId,
				entityId,
				$remoteSelect.parents('.neo-control').data('id'),
				formId,
				$remoteSelect.attr("filter-formula"))
			.then(function(rows) {
					//					window.FormUtils.clearField($remoteSelect);
					$remoteSelect.empty();
					selectRows(rows, $remoteSelect);
				},
				function(err) {
					console.error(err);
				});
	};

	return {
		instantiateControls: instantiateControls,
		getAjaxObject: getAjaxObject,
		fetchInitValues: fetchInitValues,
		templateResult: templateResult,
		templateSelection: templateSelection,

		setRemoteValues: setRemoteValues
	};
}();

var PwtDatepickerBeneficiary = function() {
	var instantiatePwtDatepicker = function($item) {
		var dontConvert = $item.attr('dont-convert');
		var calendarType = $item.hasClass('miladi') ? 'gregorian' : 'persian';
		var pOptions = {
			calendarType: calendarType,
			calendar: {
				persian: {
					showHint: true,
					locale: dontConvert ? 'en' : 'fa'
				},
				gregorian: {
					showHint: true
				}
			},
			format: dontConvert ? 'YYYY/MM/DD' : 'dddd DD MMMM YYYY',
			toolbox: {
				enabled: true,
				calendarSwitch: { enabled: !dontConvert }
			},
			observer: true,
			autoClose: true,
			initialValue: !!$item.attr('value'),
			initialValueType: dontConvert ? calendarType : 'gregorian'
		};
		if (!dontConvert) {
			pOptions.altField = '[name="' + $item.attr('associated-hidden-name') + '"]';
			pOptions.altFieldFormatter =
				function(unixDate) {
					var date = new Date(unixDate);
					return date.getFullYear() +
						'/' +
						("0" + (date.getMonth() + 1)).slice(-2) +
						'/' +
						("0" + date.getDate()).slice(-2);
				}
		}
		$item.pDatepicker(pOptions);
		if (dontConvert) return;
		$item.on('change',
			function() {
				var $this = $(this);
				if (!$this.val())
					$('[id="' + $this.attr('associated-hidden-name') + '"]').val('');
			});
	};

	var instantiateControls = function() {
		if ($.fn.pDatepicker) {
			$('input.dateField').each(function(idx, item) {
				instantiatePwtDatepicker($(item));
			});
		}
	};

	return {
		instantiateControls: instantiateControls,
		instantiatePwtDatepicker: instantiatePwtDatepicker
	};
}();

var AddClearBeneficiary = function() {

	var onClear = function($input) {
		setTimeout(function() {
				$input.trigger('change');
			},
			0);
	};

	var basicOptions = {
		closeSymbol: "✖",
		symbolClass: '',
		onClear: onClear
	};

	var instantiate = function() {
		if ($.fn.addClear === undefined) return;
		$('input[name][type="text"], input[name][type="number"], textarea').addClear(basicOptions);
		// با کلیدِ اسکیپ مقدارِ دیتپیکر خالی نمیشد.
		$('.dateField').addClear(Object.assign({ clearOnEscape: false }, basicOptions));

		//        $('input[name][type="text"]:not(.dateField), input[name][type="number"], textarea').addClear(basicOptions);
		//
		//        // با کلیدِ اسکیپ مقدارِ دیتپیکر خالی نمیشد.
		//        // not([name]) برایِ خراب نشدنِ دیتپیکرِ اسکجولِ گزارش هست!!
		//        $('.dateField:not([name])').addClear(Object.assign({ clearOnEscape: false }, basicOptions));
	};

	return {
		instantiate: instantiate
	};
}();

function instantiatePlugins() {
	Select2Beneficiary.instantiateControls();
	PwtDatepickerBeneficiary.instantiateControls();
	AddClearBeneficiary.instantiate();
}

// Select2 Change Handler - برای اجرای UI Rules از طریق inputChanged
// این handler لازم است چون Select2 از jQuery events استفاده می‌کند و onchange attribute DOM با آن کار نمی‌کند
var Select2ChangeHandler = function() {
	
	var initializeChangeHandlers = function() {
		
		// برای همه select ها - trigger کردن inputChanged
		$(document).on('change', 'select', function(e) {
			var $select = $(this);
			var selectName = $select.attr('name');
			var onchangeAttr = $select.attr('onchange');
			var selectValue = $select.val();
			
			
			// اگر onchange attribute دارد، از آن استفاده کن
			if (onchangeAttr && onchangeAttr.indexOf('inputChanged') !== -1) {
				try {
					var match = onchangeAttr.match(/inputChanged\(this,\s*'([^']*)',\s*'([^']*)'\)/);
					if (match) {
						var source = match[1];
						var scope = match[2];
						
						if (typeof inputChanged === 'function') {
							inputChanged(this, source, scope);
						}
					}
				} catch (ex) {
					console.error('[Select2ChangeHandler] Error executing onchange:', ex);
				}
			}
			// اگر onchange attribute ندارد، سعی کن مستقیماً inputChanged را با نام فیلد فراخوانی کن
			else if (selectName && typeof inputChanged === 'function') {
				try {
					inputChanged(this, selectName, '');
				} catch (ex) {
					console.error('[Select2ChangeHandler] Error in fallback inputChanged:', ex);
				}
			}
		});
	};
	
	return {
		initialize: initializeChangeHandlers
	};
}();

$(function() {
	initializePluginsDefaults();
	instantiatePlugins();
	Select2ChangeHandler.initialize();
	$('[nodatamandatory]').prop('selectedIndex', -1);
	$('[nodatamandatory]').trigger('change');
	$('form').trigger('reinitialize.areYouSure');
});
var SessionStorageStack = function () {

	var push = function (stackName, item) {
		var existingArr = sessionStorage.getItem(stackName);
		var array = !existingArr ? [] : JSON.parse(existingArr);
		array.push(item);
		sessionStorage.setItem(stackName, JSON.stringify(array));
	};

	var pop = function (stackName) {
		var sessionStorageItem = sessionStorage.getItem(stackName);
		if (!sessionStorageItem) return undefined;
		var array = JSON.parse(sessionStorageItem);
		if (!Array.isArray(array) || array.length === 0)
			return null;
		var poped = array.pop();
		sessionStorage.setItem(stackName, JSON.stringify(array));
		return poped;
	};

	var top = function (stackName) {
		var existingArr = JSON.parse(sessionStorage.getItem(stackName));
		if (!Array.isArray(existingArr) || existingArr.length === 0)
			return null;
		return existingArr[existingArr.length - 1];
	};

	return {
		push: push,
		pop: pop,
		top: top
	};
}();

var HistoryManager = function () {
	var previousPageObj = null;

	var isForm = function (url) {
		return url && url.toLowerCase().indexOf('/form/') !== -1; 
			// && (url.toLowerCase().indexOf('iframe') === -1)
			// && (url.toLowerCase().indexOf('/Index') === -1)
    };

	var formsAreEqual = function (a, b) {
		return a.Main === b.Main &&
			a.queryString.NamespaceId === b.queryString.NamespaceId &&
			a.queryString.EntityId === b.queryString.EntityId &&
			a.queryString.FormId === b.queryString.FormId &&
			a.queryString.ids === b.queryString.ids &&
			a.queryString.__parentNamespaceId === b.queryString.__parentNamespaceId &&
			a.queryString.__parentEntityId === b.queryString.__parentEntityId &&
			a.queryString.__parentFormSubjectId === b.queryString.__parentFormSubjectId &&
			a.queryString.__subTableAssociationFieldId === b.queryString.__subTableAssociationFieldId &&
			a.queryString.__parentIds === b.queryString.__parentIds &&
			a.queryString.pid === b.queryString.pid &&
			a.queryString.wid === b.queryString.wid &&
			a.queryString.TaskId === b.queryString.TaskId &&
			a.queryString.ProcessId === b.queryString.ProcessId &&
			a.queryString.Caller === b.queryString.Caller;
	};

	var pagesAreEqual = function (a, b) {
		return a.Main === b.Main;
	};

	var acquireCurrentFormObject = function () {
		return {
			NamespaceId: window.PageAddressManager.getNamespaceId(),
			EntityId: window.PageAddressManager.getEntityId(),
			FormId: window.PageAddressManager.getPageId(),
			ids: $('input[name="ids"]').val(),
			Id: $('input[name="ids"]').val(),
			__parentNamespaceId: $('input[name="__parentNamespaceId"]').val(),
			__parentEntityId: $('input[name="__parentEntityId"]').val(),
			__parentFormSubjectId: $('input[name="__parentFormSubjectId"]').val(),
			__subTableAssociationFieldId: $('input[name="__subTableAssociationFieldId"]').val(),
			__parentIds: $('input[name="__parentIds"]').val(),
			pid: $('input[name="pid"]').val(),
			wid: $('input[name="wid"]').val(),
			TaskId: $('input[name="TaskId"]').val(),
			ProcessId: $('input[name="ProcessId"]').val(),
			Caller: $('input[name="Caller"]').val()
		};
	};

	var acquireCurrentPageObject = function () {
		var search = location.search.substring(1);
		var obj = {};
		if (!search)
			return obj;
		try {
			obj = JSON.parse('{"' + decodeURI(search).replace(/"/g, '\\"').replace(/&/g, '","').replace(/=/g, '":"') + '"}');
		} finally {
			return obj;
        }
    };

	var perceivePageUrl = function () {
		var currentUrl = window.location.href;
		var mainUrl = currentUrl.split('?');
		var pageObject = {
			Main: mainUrl[0],
			queryString: isForm(currentUrl) ? acquireCurrentFormObject() : acquireCurrentPageObject()
		};
		var previousPage = SessionStorageStack.top(window.name + 'TetaHistory');
		if (previousPage !== null &&
			(
			    (isForm(currentUrl) && formsAreEqual(pageObject, previousPage)) || 
				(!isForm(currentUrl) && pagesAreEqual(pageObject, previousPage))
			)) {
			SessionStorageStack.pop(window.name + 'TetaHistory');
        }
		SessionStorageStack.push(window.name + 'TetaHistory', pageObject);
    };

	var getBackUrl = function () {
		SessionStorageStack.pop(window.name + 'TetaHistory');
		previousPageObj = SessionStorageStack.pop(window.name + 'TetaHistory');
		if (!previousPageObj)
			return null;
		var previousUrl = previousPageObj.Main + '?' + $.param(previousPageObj.queryString);
		return previousUrl;
	};

	var goBack = function (btn) {
		window.goBackCalled = true;
		if (window.LaddaManager)
			window.LaddaManager.startLadda(btn);
		var backUrl = getBackUrl();
		if (!backUrl)
			window.close();
		else
			window.location.href = backUrl;
	};

	var revertBack = function () {
		if (previousPageObj)
			SessionStorageStack.push(window.name + 'TetaHistory', previousPageObj);
		perceivePageUrl();
		if (window.LaddaManager)
			window.LaddaManager.stopLadda();
	};

	return {
		perceivePageUrl: perceivePageUrl,
		getBackUrl: getBackUrl,
		goBack: goBack,
		revertBack: revertBack
	};
}();

$(HistoryManager.perceivePageUrl);
(function(a){function f(c){if(!a("#toast-container").length){var d="top-right top-left top-center bottom-right bottom-left bottom-center".split(" ").includes(a.toastDefaults.position)?a.toastDefaults.position:"top-right";a("body").prepend('<div id="toast-container" class="toast-container" aria-live="polite" aria-atomic="true"></div>');a("#toast-container").addClass(d)}d=a("#toast-container");var b="",e=b="",g="toast-"+l,f=c.type,t=c.title,m=c.subtitle,n=c.content,h=c.img,p=c.delay?'data-delay="'+
    c.delay+'"':'data-autohide="false"',q="",r=a.toastDefaults.dismissible,u=a.toastDefaults.style.toast,k=!1;"undefined"!==typeof c.dismissible&&(r=c.dismissible);switch(f){case "info":e=a.toastDefaults.style.info||"bg-info";b=a.toastDefaults.style.info||"text-white";break;case "success":e=a.toastDefaults.style.success||"bg-success";b=a.toastDefaults.style.info||"text-white";break;case "warning":e=a.toastDefaults.style.warning||"bg-warning";b=a.toastDefaults.style.warning||"text-white";break;case "error":e=
    a.toastDefaults.style.error||"bg-danger",b=a.toastDefaults.style.error||"text-white"}a.toastDefaults.pauseDelayOnHover&&c.delay&&(p='data-autohide="false"',q='data-hide-after="'+(Math.floor(Date.now()/1E3)+c.delay/1E3)+'"');b='<div id="'+g+'" class="toast '+u+'" role="alert" aria-live="assertive" aria-atomic="true" '+p+" "+q+'><div class="toast-header '+(e+" "+b+'">');h&&(b+='<img src="'+h.src+'" class="mr-2 '+(h["class"]||"")+'" alt="'+(h.alt||"Image")+'">');b+='<strong class="mr-auto">'+t+"</strong>";
    m&&(b+='<small class="text-white">'+m+"</small>");r&&(b+='<button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">\n                        <span aria-hidden="true" class="text-white">&times;</span>\n                    </button>');b+="</div>";n&&(b+='<div class="toast-body">\n                        '+n+"\n                    </div>");b+="</div>";a.toastDefaults.stackable||d.find(".toast").each(function(){a(this).remove()});d.append(b);d.find(".toast:last").toast("show");
    a.toastDefaults.pauseDelayOnHover&&(setTimeout(function(){k||a("#"+g).toast("hide")},c.delay),a("body").on("mouseover","#"+g,function(){k=!0}),a(document).on("mouseleave","#"+g,function(){var b=Math.floor(Date.now()/1E3),c=parseInt(a(this).data("hideAfter"));k=!1;b>=c&&a(this).toast("hide")}));l++}a.toastDefaults={position:"top-right",dismissible:!0,stackable:!0,pauseDelayOnHover:!0,style:{toast:"",info:"",success:"",warning:"",error:""}};a("body").on("hidden.bs.toast",".toast",function(){a(this).remove()});
    var l=1;a.snack=function(a,d,b){return f({type:a,title:d,delay:b})};a.toast=function(a){return f(a)}})(jQuery);

/*
 * Provides api consistency and compatibility with previously used toastr
 */
window.toast = function($) {

    var getDelay = function (options) {
        var delay = 0;
        if(options) {
            delay = options.timeOut || options.delay;
        }
        return delay || 5000;
    };

    var showToast = function (type, message, title, options) {
        if(!title) {
            $.snack(type, message, getDelay(options));
        }
        else {
        $.toast({
            type: type,
            title: title,
//          subtitle: '',
            content: message,
            delay: getDelay(options)
//            ,
//            img: {
//                src: '',
//                class: 'rounded-0',
//                alt: ''
//            }
        });
        }
    }

    var info = function(message, title, options) {
        showToast('info', message, title, options);
    };
    var warning = function(message, title, options) {
        showToast('warning', message, title, options);
    };
    var success = function(message, title, options) {
        showToast('success', message, title, options);
    };
    var error = function(message, title, options) {
        showToast('error', message, title, options);
    };

    return {
        info: info,
        warning: warning,
        success: success,
        error: error
    }

 
}(window.jQuery);
