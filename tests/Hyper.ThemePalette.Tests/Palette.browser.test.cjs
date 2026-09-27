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
function fixture(palette) {
    return `<!doctype html><html dir="rtl"><head><meta charset="utf-8"><style>${css}</style><style id="host">${hostCss(palette)}</style>
    <style>body{padding:24px}.samples{display:grid;grid-template-columns:repeat(auto-fit,minmax(260px,1fr));gap:24px}.value{color:var(--neo-dashboard-primary);font-size:32px;padding:30px;text-align:center}.controls{padding:24px}.sample-chart{height:100px}</style>
    </head><body><main class="body-content"><h1>پیش‌نمایش آزمایشی تم ${palette.Name}</h1>
    <ul class="dashboard-tabs-list"><li class="dashboard-tab-item active"><a class="dashboard-tab-link" href="#sample">نمای کلی کسب‌وکار</a></li><li class="dashboard-tab-item"><a class="dashboard-tab-link" href="#sample">بازاریابی و فروش</a></li></ul>
    <div class="samples">${['مغازه‌ها','کالاها','طرف حساب‌ها','انبارها'].map((name,i)=>`<article class="neo-dashboard-widget-container"><header class="neo-dashboard-widget-title-bar"><span class="neo-dashboard-widget-title-text">${name}</span></header><div class="value">${[2943,199998,9419,2696][i]}</div></article>`).join('')}</div>
    <section class="card controls modern-form"><label for="draft">عنوان</label><input id="draft" class="form-control" value="مقدار آزمایشی"><button id="apply" class="btn btn-primary">اعمال فیلتر</button><span class="text-muted">توضیح فیلد</span></section>
    <table class="table table-striped"><thead><tr><th>گزارش</th></tr></thead><tbody><tr><td>نمونه</td></tr></tbody></table>
    <svg class="sample-chart"><rect class="highcharts-background" width="200" height="100"/><text class="highcharts-title" x="100" y="25">نمودار نمونه</text><rect id="series" fill="#22c55e" x="50" y="40" width="20" height="30"/></svg>
    </main></body></html>`;
}
async function colors(page, selector) {
    return page.locator(selector).first().evaluate(el => {
        // Composite transparent fills against their real ancestors, as the browser does.
        const parse = color => { const n=color.match(/[\d.]+/g).map(Number); return [...n.slice(0,3),n[3] ?? 1]; };
        const chain=[]; for(let node=el;node;node=node.parentElement) chain.push(node);
        let bg=[255,255,255];
        for(const node of chain.reverse()) { const c=parse(getComputedStyle(node).backgroundColor); bg=bg.map((v,i)=>v*(1-c[3])+c[i]*c[3]); }
        const style=getComputedStyle(el);
        return { fg:parse(style.color).slice(0,3), bg, image:style.backgroundImage };
    });
}
function luminance(rgb) { const c=rgb.map(v=>v/255).map(v=>v<=.04045?v/12.92:((v+.055)/1.055)**2.4); return c[0]*.2126+c[1]*.7152+c[2]*.0722; }
function contrast(c) { const a=luminance(c.fg),b=luminance(c.bg); return (Math.max(a,b)+.05)/(Math.min(a,b)+.05); }
for (const palette of palettes) for (const os of ['light','dark']) {
    test(`${palette.Name}: legacy dashboard with ${os} OS`, async t => {
        const page=await browser.newPage({colorScheme:os,viewport:{width:1366,height:768}});
        const errors=[]; page.on('pageerror', e=>errors.push(e.message));
        t.after(async()=>{ await page.close(); assert.deepEqual(errors,[]); });
        await page.setContent(fixture(palette));
        assert.ok(luminance((await colors(page,'body')).bg)>.75,'first paint stays light before bridge/API');
        assert.equal(await page.locator('html').evaluate(el=>getComputedStyle(el).getPropertyValue('--neo-dashboard-bg-secondary').trim()),palette.BackgroundSecondary,'host canvas wins before bridge, even with dark OS');
        await page.addScriptTag({content:bridge});
        await page.waitForFunction(()=>window.NeoTheme?.snapshot());
        assert.equal(await page.locator('html').getAttribute('data-neo-mode'),'light');
        for(const selector of ['body','.dashboard-tab-link','.value','#draft','#apply','.text-muted','th','td']) {
            const c=await colors(page,selector);
            assert.ok(contrast(c)>=4.5,`${selector}: ${contrast(c).toFixed(2)}:1`);
        }
        const title=await colors(page,'.neo-dashboard-widget-title-text');
        const endpoints=[palette.BackgroundTertiary,'#ffffff'].map(hex=>hex.match(/[a-f\d]{2}/gi).map(v=>parseInt(v,16)));
        for(const bg of endpoints) assert.ok(contrast({...title,bg})>=4.5,'title contrast at both gradient ends');
        assert.ok(luminance((await colors(page,'.neo-dashboard-widget-container')).bg)>.9,'opaque bright card');
        assert.equal(await page.locator('#series').evaluate(el=>getComputedStyle(el).fill),'rgb(34, 197, 94)');
        await page.locator('#draft').fill('پیش‌نویس حفظ شود');
        await page.locator('#host').evaluate((el,css)=>{el.textContent=css},hostCss(palettes[(palettes.indexOf(palette)+1)%palettes.length]));
        await page.waitForFunction(expected=>getComputedStyle(document.documentElement).getPropertyValue('--neo-accent').trim()===expected,palettes[(palettes.indexOf(palette)+1)%palettes.length].Primary);
        assert.equal(await page.locator('#draft').inputValue(),'پیش‌نویس حفظ شود');
        assert.equal(await page.locator('html').getAttribute('data-neo-mode'),'light');
        if(palette.Preference===7 && os==='dark') {
            await page.locator('#host').evaluate((el,css)=>{el.textContent=css},hostCss(palette));
            await page.waitForTimeout(250);
            await page.screenshot({path:path.join(require('node:os').tmpdir(),'hyper-teal-light-fixture.png'),fullPage:true});
            await page.setViewportSize({width:390,height:844});
            assert.ok(await page.locator('#draft').isVisible());
        }
    });
}
