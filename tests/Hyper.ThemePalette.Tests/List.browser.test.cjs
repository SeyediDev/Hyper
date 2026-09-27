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
let browser;
before(async () => { browser = await chromium.launch({ headless: true, ...(process.env.NEO_TEST_BROWSER ? { channel: process.env.NEO_TEST_BROWSER } : {}) }); });
after(async () => { await browser?.close(); });
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
const filterScript=read(path.join(mvc,'CommonAssets/Scripts/Features/Filter/ColumnFilter.js'));
const jquery=read(path.join(mvc,'CommonAssets/LibmanLibs/jquery/jquery.min.js'));
const filterSvg='<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 5h16l-6 7v5l-4 2v-7L4 5z"/></svg>';
function listFixture(palette){return `<!doctype html><html><head><meta charset="utf-8"><style>${css}\n${deployed}</style><style id="host">${hostCss(palette)}</style><style>body{padding:20px}.modern-settings-btn{display:inline-flex;align-items:center;justify-content:center;width:40px;height:40px;margin:4px;border-radius:8px}.modern-settings-btn svg{width:20px;height:20px}.table-scroll{overflow:auto}table{width:100%}.modern-sort-button{background:transparent;border:0;color:inherit}.sort-button-content{display:flex;align-items:center;gap:8px}.sort-icon{width:16px;height:16px}th{min-width:130px}</style></head><body dir="rtl"><form id="query"><section class="modern-title-box top-page"><h3>فهرست مغازه</h3><button type="button" id="add" class="modern-settings-btn modern-add-btn" title="افزودن"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M12 5v14M5 12h14"/></svg></button><button type="button" id="refresh" class="modern-settings-btn refresh-icon" title="Refresh"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M4 12a8 8 0 1 0 2-6M4 3v6h6"/></svg></button><button type="button" id="showFilter" class="modern-settings-btn" title="فیلتر"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><polygon points="22,3 2,3 10,12 10,19 14,21 14,12"/></svg></button><button type="button" id="grid" class="modern-settings-btn" title="جدول"><svg viewBox="0 0 24 24" fill="none" stroke="white"><rect x="3" y="3" width="18" height="18"/><path d="M3 9h18M9 3v18"/></svg></button></section><ul class="pagination"><li class="page-item active"><a class="page-link mouse-pointer" href="#page1">1</a></li><li class="page-item"><a class="page-link mouse-pointer" href="#page2">2</a></li><li class="page-item disabled"><span class="page-link">قبلی</span></li></ul><div class="table-scroll"><table class="table"><thead><tr>${['Name','Email'].map((name,i)=>`<th class="cColumn"><div class="neo-column-header-actions"><button type="button" class="modern-sort-button"><span class="sort-button-content"><span>${name==='Name'?'نام':'ایمیل'}</span><svg class="sort-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M3 6h18M7 12h10M10 18h4"/></svg></span></button><button type="button" class="neo-column-filter" data-filter-column="${name}" aria-label="فیلتر ${name}">${i?filterSvg.replace('<svg ','<svg class="neo-column-filter-icon" '):filterSvg}</button></div></th>`).join('')}</tr></thead><tbody><tr><td>نمونه</td><td>sample@example.test</td></tr></tbody></table></div><div id="filterDiv" style="display:none"><div class="modern-filter-content"><div class="neo-control" data-id="Name"><label for="field-Name">نام</label><input id="field-Name" name="Name" class="form-control"></div><div class="neo-control" data-id="Email"><input id="field-Email" name="Email" class="form-control"></div></div></div></form></body></html>`;}
for(const palette of palettes)for(const width of [390,1366])test(`${palette.Name}: list controls at ${width}px`,async t=>{
 const page=await browser.newPage({viewport:{width,height:850},colorScheme:'dark'});const errors=[];
 page.on('pageerror',e=>errors.push(e.message));t.after(async()=>{await page.close();assert.deepEqual(errors,[])});
 await page.setContent(listFixture(palette));await page.addScriptTag({content:bridge});await page.addScriptTag({content:jquery});
 await page.evaluate(()=>{window.sorts=0;window.submits=0;document.querySelector('form').addEventListener('submit',e=>{e.preventDefault();window.submits++});document.querySelectorAll('th').forEach(el=>el.addEventListener('click',()=>window.sorts++));document.querySelector('#showFilter').addEventListener('click',()=>{document.querySelector('#filterDiv').style.display='block'});});
 await page.addScriptTag({content:filterScript});
 for(const selector of ['#add','#refresh','#grid','#showFilter','.page-item.active .page-link','.page-item:not(.active) .page-link']){
   const c=await colors(page,selector);assert.ok(contrast(c)>=4.5,`${selector} contrast ${contrast(c).toFixed(2)}`);
 }
 for(const selector of ['#refresh','#grid','#showFilter'])assert.ok(luminance((await colors(page,selector)).bg)>.75,'light toolbar surface');
 assert.equal(await page.locator('#grid rect').evaluate(el=>getComputedStyle(el).fill),'none','outlined grid icon is not a solid square');
 for(const selector of ['#add path','#grid path','#showFilter polygon'])assert.equal(await page.locator(selector).evaluate(el=>getComputedStyle(el).stroke),await page.locator(selector).evaluate(el=>getComputedStyle(el).color),'icons inherit readable foreground');
 await page.locator('#refresh').hover();await page.waitForTimeout(350);assert.ok(contrast(await colors(page,'#refresh'))>=4.5,JSON.stringify(await colors(page,'#refresh')));
 for(const button of await page.locator('.neo-column-filter').all()){
   const b=await button.boundingBox();assert.ok(b.width>=28&&b.height>=28);
   const icon=await button.locator('svg').boundingBox();assert.equal(icon.width,16);assert.equal(icon.height,16);
   const sort=await button.locator('..').locator('.modern-sort-button').boundingBox();assert.ok(Math.abs(sort.y+sort.height/2-b.y-b.height/2)<2,'single aligned header row');
 }
 await page.locator('[data-filter-column="Name"]').click();await page.waitForFunction(()=>document.activeElement?.id==='field-Name');
 await page.locator('#field-Name').fill('نمونه');await page.locator('#field-Name').dispatchEvent('change');
 await page.waitForFunction(()=>document.querySelector('[data-filter-column="Name"]').getAttribute('aria-pressed')==='true');
 assert.equal(await page.evaluate(()=>window.sorts),0);assert.equal(await page.evaluate(()=>window.submits),0);
});
