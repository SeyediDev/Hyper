// Isolated sample markup + actual host palette and Neo assets. No live account or API calls.
const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { chromium } = require('playwright');
const root = path.resolve(__dirname, '../..');
const mvc = path.join(process.env.NEO_BPMS_ROOT || path.resolve(root, '../../Neo-Bpms'), 'src/Neo.Bpms.UI.MVC');
const read = p => fs.readFileSync(p, 'utf8');
const palettes = JSON.parse(read(process.env.HYPER_PALETTES || path.join(require('node:os').tmpdir(), 'hyper-theme-palettes.json')));
const razor = read(path.join(root, 'src/AdminPanel/Hyper.AdminPanel.Web/Views/Shared/Layout/_ThemeVariables.cshtml'));
function hostCss(palette) {
    return razor.match(/<style>([\s\S]*?)<\/style>/)[1]
        .replace(/@\(string.IsNullOrEmpty\(themeObj.DashboardWidgetTitleText\)[^;]+/g, palette.DashboardWidgetTitleText)
        .replace(/@themeObj\.(\w+)/g, (_, name) => { assert.ok(palette[name], name); return palette[name]; });
}
const css = [
    'CommonAssets/LibmanLibs/bootstrap/css/bootstrap.min.css',
    'CommonAssets/Styles/Features/Global/neo-dashboard.css',
    'wwwroot/css/neo-theme.css', 'wwwroot/css/neo-theme-mvc.css'
].map(p => read(path.join(mvc, p))).join('\n');
const bridge = read(path.join(mvc, 'wwwroot/js/neo-theme.js'));
let browser; let browserServer;
before(async () => { browserServer = await chromium.launchServer({ headless: true, ...(process.env.NEO_TEST_BROWSER ? { channel: process.env.NEO_TEST_BROWSER } : {}) }); browser = await chromium.connect(browserServer.wsEndpoint()); });
after(async () => { await browser?.close(); await browserServer?.kill(); });
async function colors(page, selector) {
    return page.locator(selector).first().evaluate(el => {
        // Composite transparent fills against their real ancestors, as the browser does.
        const parse = color => { const n=color.match(/[\d.]+/g).map(Number); return [...n.slice(0,3).map(v=>color.startsWith('color(srgb')?v*255:v),n[3] ?? 1]; };
        const chain=[]; for(let node=el;node;node=node.parentElement) chain.push(node);
        let bg=[255,255,255];
        for(const node of chain.reverse()) { const c=parse(getComputedStyle(node).backgroundColor); bg=bg.map((v,i)=>v*(1-c[3])+c[i]*c[3]); }
        const style=getComputedStyle(el);
        return { fg:parse(style.color).slice(0,3), bg, image:style.backgroundImage };
    });
}
function luminance(rgb) { const c=rgb.map(v=>v/255).map(v=>v<=.04045?v/12.92:((v+.055)/1.055)**2.4); return c[0]*.2126+c[1]*.7152+c[2]*.0722; }
function contrast(c) { const a=luminance(c.fg),b=luminance(c.bg); return (Math.max(a,b)+.05)/(Math.min(a,b)+.05); }

const web=path.join(root,'src/AdminPanel/Hyper.AdminPanel.Web');
const deployed=read(path.join(web,'wwwroot/bundles/rtl.css'))+'\n'+read(path.join(web,'wwwroot/Content/custom-theme.css'));
const filterScript=read(path.join(web,'wwwroot/Content/common-assets-includes/features/filter/ColumnFilter.js'));
assert.equal(filterScript, read(path.join(mvc,'CommonAssets/Scripts/Features/Filter/ColumnFilter.js')), 'Host filter script must match the canonical source; run CopyFilterAssets');
const jquery=read(path.join(mvc,'CommonAssets/LibmanLibs/jquery/jquery.min.js'));
const filterSvg='<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 5h16l-6 7v5l-4 2v-7L4 5z"/></svg>';

const partial = name => read(path.join(mvc, 'Views/Report/Partials', name));
const styles = text => [...text.matchAll(/<style[^>]*>([\s\S]*?)<\/style>/g)].map(m=>m[1].replaceAll('@@','@')).join('\n');
const reportCss = styles(partial('_Styles.cshtml')) + styles(partial('_Modal.filter-tooltip.cshtml'));
const headerSource = partial('_Header.cshtml');
function action(name) {
 const markup=[...headerSource.matchAll(/<a\b[^>]*>[\s\S]*?<\/a>/g)].find(m=>m[0].includes('data-action="'+name+'"'))?.[0];
 assert.ok(markup, name); return markup.replace(/@ViewTexts\.\w+/g,'Action');
}
const toggleSource=partial('_Scripts.toggles.cshtml');
const toggleStart=toggleSource.indexOf('window.toggleFilterTooltip = function');
const toggle=toggleSource.slice(toggleStart,toggleSource.indexOf('\n};',toggleStart)+3);
const events=partial('_Scripts.event-manager.cshtml');
const eventStart=events.indexOf("document.addEventListener('click', function(e)");
const outside=events.slice(eventStart,events.indexOf('\n    });',eventStart)+8);
for(const palette of palettes)for(const width of [390,1366])test(`${palette.Name}: Report filters and toolbar at ${width}px`,async t=>{
 const page=await browser.newPage({viewport:{width,height:650},colorScheme:'dark'});const errors=[];
 page.on('pageerror',e=>errors.push(e.message));t.after(async()=>{await page.close();assert.deepEqual(errors,[])});
 await page.setContent(`<!doctype html><html><head><style>${css}\n${deployed}\n${reportCss}\n${hostCss(palette)}</style></head><body dir="rtl" class="page-ready report-page"><form id="query"><section class="reportHeader top-page"><div class="icons">${action('toggle-report-info')}${action('toggle-report-designs-modal')}${action('toggle-filter-tooltip')}</div></section><div id="ReportContainer"><table class="table"><thead><tr><th><div class="neo-column-header-actions"><button type="button" class="modern-sort-button">شهر</button><button type="button" class="neo-column-filter" data-filter-column="City" aria-label="فیلتر شهر">${filterSvg}</button></div></th></tr></thead></table></div><ul class="pagination"><li class="page-item active"><a class="page-link" href="#">1</a></li></ul><div id="filterTooltipPanel" class="filter-tooltip-panel popular-filters-hidden"><div class="filter-panel-header"><h5>فیلتر</h5></div><div class="filter-panel-content-wrapper"><div class="filter-main-content"><div class="modern-filter-content"><div class="neo-control" data-id="City"><label for="field-City">شهر</label><input class="form-control" name="City" id="field-City" value="Tehran"></div></div><div class="modern-filter-buttons"><button id="submit-filter" type="button" class="btn btn-success">اعمال</button></div></div></div></div></form></body></html>`);
 await page.addScriptTag({content:bridge});await page.addScriptTag({content:jquery});await page.addScriptTag({content:toggle});await page.addScriptTag({content:outside});
 await page.evaluate(()=>{window.sorts=0;window.submits=0;document.querySelector('form').addEventListener('submit',e=>{e.preventDefault();window.submits++});document.querySelector('th').addEventListener('click',()=>window.sorts++);});
 await page.addScriptTag({content:filterScript});
 for(const selector of ['[data-action="toggle-report-info"]','[data-action="toggle-report-designs-modal"]','.page-link']) {
  const c=await colors(page,selector);assert.ok(contrast(c)>=4.5,`${selector} contrast ${contrast(c).toFixed(2)}`);
 }
 assert.equal(await page.locator('[data-action="toggle-report-info"] circle[fill="none"]').evaluate(e=>getComputedStyle(e).fill),'none');
 assert.notEqual(await page.locator('[data-action="toggle-report-info"] circle[fill="currentColor"]').evaluate(e=>getComputedStyle(e).fill),'none');
 await page.locator('.neo-column-filter').click();
 await page.waitForFunction(()=>document.activeElement.id==='field-City',null,{timeout:3000});
 for(const selector of ['#filterTooltipPanel','#field-City','#submit-filter'])assert.ok(contrast(await colors(page,selector))>=4.5,selector+' contrast');
 const box=await page.locator('#filterTooltipPanel').boundingBox();
 assert.ok(box.x>=0 && box.y>=0 && box.x+box.width<=width+1 && box.y+box.height<=651,JSON.stringify(box));
 await page.locator('.neo-column-filter').focus();await page.keyboard.press('Space');
 await page.waitForFunction(()=>document.activeElement.id==='field-City');
 assert.equal(await page.locator('#field-City').inputValue(),'Tehran');
 assert.deepEqual(await page.evaluate(()=>[window.sorts,window.submits]),[0,0]);
});
