// Real host Mmenu assets and plugin; sample navigation only, no live host access.
const {test,before,after}=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs'), path=require('node:path'), os=require('node:os');
const {chromium}=require('playwright');
const root=path.resolve(__dirname,'../..');
const web=path.join(root,'src/AdminPanel/Hyper.AdminPanel.Web');
const mvc=path.join(process.env.NEO_BPMS_ROOT||path.resolve(root,'../../Neo-Bpms'),'src/Neo.Bpms.UI.MVC');
const read=p=>fs.readFileSync(p,'utf8');
const palettes=JSON.parse(read(process.env.HYPER_PALETTES||path.join(os.tmpdir(),'hyper-theme-palettes.json')));
const razor=read(path.join(web,'Views/Shared/Layout/_ThemeVariables.cshtml'));
const host=p=>razor.match(/<style>([\s\S]*?)<\/style>/)[1].replace(/@\(string.IsNullOrEmpty\(themeObj.DashboardWidgetTitleText\)[^;]+/g,p.DashboardWidgetTitleText).replace(/@themeObj\.(\w+)/g,(_,n)=>p[n]);
const base=[path.join(mvc,'CommonAssets/LibmanLibs/bootstrap/css/bootstrap.min.css'),...['bundles/rtl.css','bundles/ux-theme.css','Content/common-assets-includes/features/global/neo-dashboard.css'].map(p=>path.join(web,'wwwroot',p)),...['neo-theme.css','neo-theme-mvc.css'].map(p=>path.join(mvc,'wwwroot/css',p)),path.join(web,'wwwroot/Content/custom-theme.css')].map(read).join('\n');
const menu=['mmenu.css','mmenu-rtl-fix.css','custom-menu-icons.css'].map(p=>read(path.join(web,'wwwroot/bundles/mmenu',p))).join('\n');
const bridge=read(path.join(mvc,'wwwroot/js/neo-theme.js'));
const plugin=read(path.join(web,'wwwroot/bundles/mmenu/mmenu.js'));
let browser;
before(async()=>{browser=await chromium.launch({headless:true,...(process.env.NEO_TEST_BROWSER?{channel:process.env.NEO_TEST_BROWSER}:{})});});
after(async()=>{await browser?.close()});
async function inspect(page,selector){return page.locator(selector).first().evaluate(el=>{
    const parse=s=>{const n=s.match(/[\d.]+/g).map(Number);return [...n.slice(0,3),n[3]??1]};
    const nodes=[];for(let n=el;n;n=n.parentElement)nodes.push(n);
    let bg=[255,255,255];for(const n of nodes.reverse()){const c=parse(getComputedStyle(n).backgroundColor);bg=bg.map((v,i)=>v*(1-c[3])+c[i]*c[3])}
    const c=parse(getComputedStyle(el).color);const fg=bg.map((v,i)=>v*(1-c[3])+c[i]*c[3]);
    const lum=rgb=>{const c=rgb.map(v=>v/255).map(v=>v<=.04045?v/12.92:((v+.055)/1.055)**2.4);return c[0]*.2126+c[1]*.7152+c[2]*.0722};
    const a=lum(fg),b=lum(bg);return {contrast:(Math.max(a,b)+.05)/(Math.min(a,b)+.05),brightness:b,color:getComputedStyle(el).color};
});}
for(const palette of palettes)for(const mode of ['light','dark'])test(`${palette.Name}: real Mmenu, ${mode} OS`,async t=>{
    const page=await browser.newPage({colorScheme:mode,viewport:{width:mode==='light'?390:1366,height:820}});
    const errors=[];page.on('pageerror',e=>errors.push(e.message));t.after(async()=>{await page.close();assert.deepEqual(errors,[])});
    await page.setContent(`<!doctype html><html><head><meta charset="utf-8"><style>${base}</style><style id="host">${host(palette)}</style></head><body dir="rtl" style="direction:rtl;text-align:right"><div id="page"><a href="#menu2" id="open">منو</a><main>صفحهٔ آزمایشی</main></div><style>${menu}</style><nav id="menu2" dir="rtl"><ul><li class="Selected"><a href="#home">صفحه اصلی</a></li><li><a href="#report">گزارش‌ها</a></li><li><span>مدیریت فروش</span><ul><li><a href="#orders">سفارش‌ها</a></li><li><a href="#customers">مشتریان</a></li></ul></li></ul></nav></body></html>`);
    await page.addScriptTag({content:bridge});await page.addScriptTag({content:plugin});
    await page.evaluate(()=>{window.sampleMenu=new Mmenu('#menu2',{navbar:{title:'پنل مدیریت هایپریک'},offCanvas:{position:'right'},theme:document.documentElement.getAttribute('data-neo-mode')||'light',counters:{add:false},navbars:[{position:'top',content:['searchfield']},{position:'top',content:['prev','title']},{position:'bottom',content:["<div id='menu-version-info'>نسخه: آزمایشی</div>"]}]});});
    await page.locator('#open').click();await page.waitForTimeout(450);
    assert.ok(await page.locator('#menu2').isVisible());
    const bounds=await page.locator('#menu2').boundingBox();
    assert.ok(bounds.x>=-1 && bounds.x+bounds.width<=page.viewportSize().width+1,'open menu remains inside viewport');
    for(const selector of ['#menu2 .mm-navbar','#menu-version-info','#menu2 a[href="#home"]','#menu2 a[href="#report"]','#menu2 .mm-navbar__title','#menu2 input']){
        const c=await inspect(page,selector);assert.ok(c.contrast>=4.5,`${selector}: ${c.contrast.toFixed(2)}:1`);assert.ok(c.brightness>.75,`${selector}: light surface`);
    }
    await page.locator('#menu2 .mm-panel--opened a[href="#report"]:visible').first().hover();await page.waitForTimeout(200);assert.ok((await inspect(page,'#menu2 a[href="#report"]')).contrast>=4.5);
    await page.locator('#menu2 .mm-panel--opened a.mm-btn--next').first().click();await page.waitForTimeout(450);
    assert.ok(await page.locator('#menu2 a[href="#orders"]').isVisible());assert.ok((await inspect(page,'#menu2 a[href="#orders"]')).contrast>=4.5);
    await page.locator('#menu2 .mm-navbars--top .mm-btn--prev').click();await page.waitForTimeout(450);
    await page.locator('#menu2 input').fill('گزارش');await page.waitForTimeout(350);
    assert.ok(await page.locator('#menu2 .mm-panel--opened a[href="#report"]:visible').count()>0);
    assert.equal(await page.locator('#menu2 .mm-panel--opened a[href="#home"]:visible').count(),0);
    await page.locator('#menu2 input').fill('');await page.waitForTimeout(350);
    const next=palettes[(palettes.indexOf(palette)+1)%palettes.length];await page.locator('#host').evaluate((el,css)=>el.textContent=css,host(next));
    await page.waitForFunction(expected=>getComputedStyle(document.documentElement).getPropertyValue('--neo-accent').trim()===expected,next.Primary);
    assert.ok((await inspect(page,'#menu2 a[href="#home"]')).contrast>=4.5);
    await page.locator('#menu2 .mm-panel--opened a[href="#report"]:visible').first().focus();assert.equal(await page.locator('#menu2 .mm-panel--opened a[href="#report"]:visible').first().evaluate(el=>el===document.activeElement),true);
});
