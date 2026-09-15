import json,re
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
MODEL=ROOT/'src/Core/Hyper.Infrastructure/Data/Repository/Hyper/HyperSqlServerModel.cs'
ENTITIES=ROOT/'src/Core/Hyper.Domain/Entities/Database/SqlServerEntities.cs'
def norm(s):return re.sub('[^a-z0-9]','',s.lower())
metadata=json.loads((ROOT/'docs/schema/hyper-display-metadata.json').read_text('utf-8-sig'))
lookup={}
for row in metadata:
    lookup.setdefault(norm('TBL_'+row['table']),{'title':row['title'],'category':row['category'],'columns':{}})['columns'][norm(row.get('column') or '')]=row.get('columnTitle')
vocab_text='''ACT=فعالیت|ACTION=عملیات|ACTIVE=فعال|ACTIVITY=فعالیت|AGGREGATION=تجمیع|ANNOTATION=یادداشت|ASSIGNEE=مسئول|ASSIGNER=واگذارکننده|ASSOCIATION=ارتباط|ASSOCIATIONS=ارتباط‌ها|ATTEMPTS=تلاش‌ها|BATCH=دسته|BINDING=اتصال|BLOCK=بلوک|BUSINESS=کسب‌وکار|BYTEARRAY=داده باینری|BYTES=بایت‌ها|CACHED=کش‌شده|CALL=فراخوانی|CASE=پرونده|CATEGORY=دسته‌بندی|CAUSE=علت|CFG=تنظیمات|CHAIN=زنجیره|CHANGE=تغییر|CLASS=کلاس|CLAUSE=عبارت|CLOSE=بستن|COLLECT=جمع‌آوری|COLUMN=ستون|CONCURRENT=همزمان|CONFIGURATION=پیکربندی|CONTENT=محتوا|COUNTER=شمارنده|CREATE=ایجاد|CREATED=ایجادشده|CURRENT=فعلی|CUSTOM=سفارشی|DATA=داده|DATE=تاریخ|DEC=تصمیم|DEF=تعریف|DEFAULT=پیش‌فرض|DEFINITION=تعریف|DELEGATION=واگذاری|DELETE=حذف|DELETED=حذف‌شده|DEPLOY=استقرار|DEPLOYMENT=استقرار|DESCRIPTION=توضیحات|DETAIL=تفصیلی|DETAILS=جزئیات|DGRM=نمودار|DOUBLE=اعشاری|DUE=سررسید|DUEDATE=تاریخ سررسید|DURATION=مدت|EDIT=ویرایش|EMAIL=رایانامه|ENABLE=فعال‌سازی|END=پایان|ENT=موجودیت|ENTITY=موجودیت|ERROR=خطا|ESCAPE=گریز|EVAL=ارزیابی|EVENT=رویداد|EXCEPTION=استثنا|EXCLUSIVE=انحصاری|EXEC=اجرا|EXECUTION=اجرا|EXP=انقضا|EXPORT=خروجی|EXPORTABLE=قابل صدور|EXT=خارجی|EXTERNAL=خارجی|FAILED=ناموفق|FAILURE=شکست|FILTER=فیلتر|FILTERABLE=قابل فیلتر|FIRST=اول|FOLDER=پوشه|FOLLOW=پیگیری|FORM=فرم|FORMAT=قالب|FULL=کامل|FULLNAME=نام کامل|GENERATED=تولیدشده|GROUP=گروه|HANDLER=پردازشگر|HAS=دارای|HASH=هش|HINTS=راهنما|HISTORY=تاریخچه|HOSTNAME=نام میزبان|HTML=اچ‌تی‌ام‌ال|ICON=آیکون|ID=شناسه|INCIDENT=رخداد خطا|INCOMING=ورودی|INDEX=ترتیب|INITIAL=اولیه|INLINE=درون‌خطی|INST=نمونه|INSTANCE=نمونه|INVOCATIONS=فراخوانی‌ها|IS=آیا|JOB=کار پس‌زمینه|JOBS=کارهای پس‌زمینه|KEY=کلید|KIND=نوع|LAST=آخرین|LIMIT=محدودیت|LOCAL=محلی|LOCK=قفل|LOG=سوابق|LOGO=نشان|LONG=عدد صحیح بلند|MESSAGE=پیام|MILLISECONDS=میلی‌ثانیه|MOBILE=تلفن همراه|MONITOR=پایش|MSG=پیام|NAME=نام|NEED=نیاز|NEW=جدید|NORMALIZE=نرمال‌سازی|NOTIFICATION=اعلان|NULLABLE=تهی‌پذیر|OFFSET=آفست|OLD=قبلی|OPERATION=عملیات|ORDER=سفارش|ORG=سازمان|OUT=خروجی|OUTCOMING=خروجی|OWNER=مالک|PACKAGES=بسته‌ها|PARENT=والد|PASSWORD=رمز عبور|PER=به‌ازای|PERMS=مجوزها|PICTURE=تصویر|PREV=قبلی|PRIORITY=اولویت|PROC=فرایند|PROCESS=فرایند|PROPERTIES=ویژگی‌ها|PROPERTY=ویژگی|PWD=رمز عبور|QUERY=پرس‌وجو|READ=خواندن|READONLY=فقط‌خواندنی|REASON=دلیل|REF=مرجع|RELATIONS=روابط|REMOVAL=حذف|REPEAT=تکرار|REPORT=گزارش|REPORTER=گزارش‌دهنده|REQ=درخواست|REQUIRED=الزامی|RESOURCE=منبع|RESTARTED=راه‌اندازی مجدد|RESULT=نتیجه|RETRIES=تلاش‌های مجدد|REV=بازنگری|ROOT=ریشه|ROW=سطر|RULE=قاعده|SALT=نمک رمزنگاری|SATISFIED=برآورده‌شده|SCOPE=دامنه مجوز|SCRIPT=اسکریپت|SEED=بذر|SELECTOR=انتخابگر|SENDER=فرستنده|SENTRY=شرط محافظ|SEQUENCE=توالی|SHOW=نمایش|SORTABLE=قابل مرتب‌سازی|SOURCE=منبع|SQL=اس‌کیوال|STACK=پشته|STANDARD=استاندارد|START=شروع|STARTABLE=قابل شروع|STATE=وضعیت|SUGGEST=پیشنهاد|SUGGESTABLE=قابل پیشنهاد|SUMMABLE=قابل جمع|SUPER=بالادست|SUSPENSION=تعلیق|SYNC=یکسان‌سازی|TABLE=جدول|TAG=برچسب|TAGS=برچسب‌ها|TARGET=مقصد|TASK=وظیفه|TENANT=مستاجر|TEXT=متن|TEXT2=متن دوم|TIME=زمان|TIMESTAMP=مهر زمانی|TOPIC=موضوع|TOTAL=مجموع|TTL=مدت نگهداری|TYPE=نوع|UP=بعدی|UPDATE=به‌روزرسانی|UPDATED=به‌روزشده|URL=نشانی وب|USED=مصرف‌شده|USER=کاربر|USERNAME=نام کاربری|VALIDATOR=اعتبارسنج|VALUE=مقدار|VAR=متغیر|VARIABLE=متغیر|VERSION=نسخه|VISIBLE=نمایان|WORKER=پردازشگر|ACCOUNT=حساب|IDENTIFIER=شناسه|ADMIN=ادمین|AT=در زمان|UTC=یوتی‌سی|COMPLETED=تکمیل‌شده|CONNECTED=متصل‌شده|CONNECTION=اتصال|CORRELATION=همبستگی|BY=توسط|CREDENTIAL=اعتبارنامه|CREDENTIALS=اعتبارنامه‌ها|JSON=جی‌سان|DIRECTION=جهت|DISPLAY=نمایشی|ENDED=پایان‌یافته|EXPIRES=انقضا|PARCEL=مرسوله|PRODUCT=کالا|SKU=کد کالا|VARIANT=گونه کالا|RUNS=اجراها|FINISHED=پایان‌یافته|HYPER=هایپریک|SALE=فروش|ENABLED=فعال|ISSUER=صادرکننده|ITEMS=اقلام|WRITTEN=نوشته‌شده|INVENTORY=موجودی|PRICE=قیمت|RUN=اجرا|LEASE=اجاره پردازش|MAPPING=نگاشت|COUNT=تعداد|MERCHANT=مغازه‌دار|NEXT=بعدی|ATTEMPT=تلاش|PAYLOAD=محتوای رویداد|PROCESSED=پردازش‌شده|PROVIDER=پلتفرم|QUANTITY=تعداد|RECEIVED=دریافت‌شده|RELEASED=آزادشده|REQUESTED=درخواست‌شده|RESERVATION=رزرو|REVOKED=لغوشده|SHOP=مغازه|SIGNATURE=امضا|VALID=معتبر|SIMULATION=شبیه‌سازی|STARTED=شروع‌شده|STATUS=وضعیت|SUBJECT=موضوع|DOCUMENT=سند|CNT=تعداد|CREDIT=بستانکار|DEBIT=بدهکار|TOKEN=توکن|GLOBAL=عمومی|SYSTEM=سامانه|UPLOAD=بارگذاری|FILE=فایل|TITLE=عنوان|AUTHORIZATION=مجوز|MEMBERSHIP=عضویت|REMEMBER=به‌خاطرسپاری|ME=من|MEMBER=عضو|CONSTRAINT=محدودیت|DASHBOARD=داشبورد|DECISION=تصمیم|LINK=پیوند|MENU=منو|MODEL=مدل|SORT=مرتب‌سازی|HEADER=سرصفحه|IN=ورودی|SCHEMA=طرح پایگاه داده|ATTACHMENT=پیوست|COMMENT=نظر|OP=عملیات|CHANGEQUEUE=صف تغییر|QUEUE=صف|SUBSCR=اشتراک|METER=سنجش|PART=بخش|CONNECTIONS=اتصال‌ها|MAPPINGS=نگاشت‌ها|SIMULATIONS=شبیه‌سازی‌ها|AUDITS=ممیزی‌ها|ACCESS=دسترسی|OUTBOX=صف خروجی|REQUESTS=درخواست‌ها|WEBHOOK=وب‌هوک|INBOX=صندوق ورودی|LOGS=سوابق|BALANCE=مانده|INTEGRATION=یکسان‌سازی'''
VOCAB=dict(pair.split('=',1) for pair in vocab_text.split('|'))
SPECIAL={'ACTINST':'نمونه فعالیت','CASEACTINST':'نمونه فعالیت پرونده','CASEINST':'نمونه پرونده','DECINST':'نمونه تصمیم','IDENTITYLINK':'پیوند هویت','PROCINST':'نمونه فرایند','TASKINST':'نمونه وظیفه','VARINST':'نمونه متغیر','CAMFORMDEF':'تعریف فرم موتور','PROCDEF':'تعریف فرایند','JOBDEF':'تعریف کار پس‌زمینه','BYTES':'داده باینری'}
VOCAB.update(SPECIAL)
VOCAB.update({'INFO':'اطلاعات','CREATETIME':'زمان ایجاد','EXTENSION':'پسوند','FILETYPE':'نوع فایل','ORIGINALNAME':'نام اصلی','RELATIVEURL':'نشانی نسبی','SIZE':'اندازه','UID':'شناسه یکتا','CONFIG':'تنظیمات','FILES':'فایل‌ها'})
missing=set()
def translate(raw):
    tokens=re.sub(r'([a-z0-9])([A-Z])',r'\1_\2',raw).strip('_').replace(' ','_').split('_')
    result=[]
    for token in tokens:
        key=token.upper()
        if key not in VOCAB:missing.add(token)
        result.append(VOCAB.get(key,token))
    return ' '.join(result)
# Original declarations are preserved; only display/mapping attributes are regenerated.
s=ENTITIES.read_text('utf-8-sig')
s=re.sub(r'^\s*\[(?:DisplayName|DbMap|OldDbMap)\([^\n]*\)\]\r?\n','',s,flags=re.M)
records=[]
for b in re.finditer(r'modelBuilder\.Entity<(\w+)>\(entity =>\s*\{(.*?)\n\s*\}\);',MODEL.read_text('utf-8'),re.S):
    cls,body=b[1],b[2]; table=re.search(r'To(Table|View)\("([^"]+)"',body)[2]
    meta=lookup.get(norm(table),{})
    prefix='سوابق موتور: ' if table.startswith('ACT_HI') else 'موتور: ' if table.startswith('ACT_') else ''
    stem=re.sub(r'^(ACT_\w+_|View_|vw_)','',table)
    title=meta.get('title') or prefix+translate(stem)
    if title and not re.search('[\u0600-\u06ff]',title):title=translate(title)
    if cls=='SqlTblShop':title='مغازه'
    category=meta.get('category') or ('جداول موتور' if table.startswith('ACT_') else 'گزارش‌های تجمیعی' if table.startswith(('View_','vw_')) else 'یکسان‌سازی')
    props=[]
    for p in re.finditer(r'Property\(e => e\.(\w+)\)\.HasColumnName\("([^"]+)"\)([^\n]*)',body):
        prop,col,config=p[1],p[2],p[3]
        name=meta.get('columns',{}).get(norm(col))
        if not name or not re.search('[\u0600-\u06ff]',name):name=translate(col)
        # Common terminology throughout the panel.
        name=name.replace('فروشگاه','مغازه')
        props.append({'property':prop,'column':col,'title':name,'identity':'UseIdentityColumn' in config})
    key=re.search(r'entity.HasKey\(([^\n]+?)\)\.',body)
    keys=re.findall('"([^"]+)"',key[1]) if key else []
    records.append({'class':cls,'table':table,'title':title,'category':category,'keys':keys,'fields':props})
    pattern=r'(public sealed class '+cls+r' : [^\n]+\n\{)(.*?)(\n\})'
    match=re.search(pattern,s,re.S)
    if not match:raise ValueError(cls)
    fields=match[2]
    for p in props:
        fields=re.sub(r'(^    public [^\n]+ '+p['property']+r' \{)',lambda mm:'    [DisplayName('+json.dumps(p['title'],ensure_ascii=False)+')]\n    [DbMap('+json.dumps(p['column'])+')]\n'+mm[1],fields,flags=re.M)
    replacement='[DisplayName('+json.dumps(title,ensure_ascii=False)+')]\n[DbMap('+json.dumps(table)+')]\n'+match[1]+fields+match[3]
    s=s[:match.start()]+replacement+s[match.end():]
if missing: print('UNRESOLVED TOKENS:', ','.join(sorted(missing)))
ENTITIES.write_text(s.rstrip()+'\n',encoding='utf-8')
(ROOT/'docs/schema/sql-ui-catalog.json').write_text(json.dumps(records,ensure_ascii=False,indent=2),encoding='utf-8')
print('Entities:',len(records),'Fields:',sum(len(r['fields']) for r in records))
