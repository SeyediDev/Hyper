using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

static class ProductReadinessChecks
{
    public static async Task Run(DbContextOptions<HyperIntegrationContext> options, Action<bool,string> check)
    {
        var source = new AccountingFixture(); var remote = new Creator(); var resolver = new IntegrationStrategyResolver([remote]); var vendor = 3000;
        IntegrationScenarioQueue Queue(HyperIntegrationContext db)
        {
            var collector=new IntegrationProductPreparationCollector(db);
            return new(db, new(db, resolver, null!,
                Options.Create(new IntegrationInventoryCaptureOptions()), Options.Create(new BasalamOAuthSettings()), null!,
                new IntegrationBusinessEventDispatcher(source,new UnregisteredIntegrationEngagementPort(),null!,db,resolver,collector),source,
                new IntegrationProductCreationProcessor(db,resolver,source),collector));
        }
        IntegrationProductReadinessApi Api(HyperIntegrationContext db) => new(db, new IntegrationProductDraftApi(db, Queue(db), source), source);
        async Task<IntegrationConnectionCommandRequest> Scope()
        {
            await using var db = new HyperIntegrationContext(options);
            var c = new ExternalIntegrationConnection { ShopId=7,TenantId="tenant-a",Provider=Provider.Basalam,
                AccountIdentifier=(++vendor).ToString(),DisplayName="readiness fixture",CredentialsJson="{}",CredentialType=IntegrationCredentialType.BearerToken };
            db.Add(c); await db.SaveChangesAsync(); return new(7,"tenant-a",c.Id);
        }
        async Task<ProductPreparationItem?> Prepare(IntegrationConnectionCommandRequest s, ProductPreparationInput input, int revision=0)
        { await using var db = new HyperIntegrationContext(options); return await Api(db).PrepareAsync(s,input,revision,"fixture-user",default); }
        async Task<ProductPreparationBoard?> Board(IntegrationConnectionCommandRequest s)
        { await using var db = new HyperIntegrationContext(options); return await Api(db).BoardAsync(s,0,25,default); }
        async Task<bool> Policy(IntegrationConnectionCommandRequest s, ProductPreparationPolicy policy, ProductTransferDirection direction=ProductTransferDirection.ToPlatform)
        { await using var db = new HyperIntegrationContext(options); return await Api(db).SetPolicyAsync(s,direction,policy,"fixture-user",default); }
        async Task Process(IntegrationConnectionCommandRequest s)
        { await using var db = new HyperIntegrationContext(options); check(await Queue(db).ProcessConnectionAsync(s.ConnectionId,default),"readiness job processed"); }
        async Task Conflict(Func<Task> action)
        { try { await action(); } catch(InvalidOperationException e) when(e.Message=="PreparationRevisionConflict") {check(true,"stale edit or submitted rewrite refused");return;} throw new Exception("Expected revision conflict"); }
        var input = new ProductPreparationInput(ProductTransferDirection.ToPlatform,"44",100,2,200,new string('x',10001));
        var s = await Scope();
        await using(var db = new HyperIntegrationContext(options))
            check((await Api(db).PolicyAsync(s,ProductTransferDirection.ToPlatform,default))!.Mode==ProductPreparationMode.Hold,"default policy holds instead of silently adapting");
        check(await Prepare(s with {TenantId="foreign"},input) is null && !await Policy(s with {ShopId=99},new(ProductPreparationMode.Adapt)),"foreign scope cannot prepare or change policy");
        var blocked = (await Prepare(s,input))!;
        check(blocked.JobId is null && blocked.Issues.Any(x=>x.Code=="DescriptionTooLong") && blocked.Prepared.Description==input.Description,"strict mode retains invalid original and does not enqueue");
        check((await Prepare(s,input))!.Id==blocked.Id && (await Board(s))!.Items.Single().History.Count==1,"replayed intake is deduplicated without audit spam");
        await Conflict(async()=>await Prepare(s,input with {Description="different"}));
        check(await Policy(s,new(ProductPreparationMode.Adapt,TrimDescription:true,TruncateDescription:true)),"explicit adaptation policy stored");
        var adapted = (await Prepare(s,input with {Description="  "+input.Description+"  "},blocked.Revision))!;
        check(adapted.JobId>0 && adapted.Changes.Count==2 && adapted.Prepared.Description!.Length==10000 && adapted.History.Count==2,"allowlisted fixes recorded before atomic queue submission");
        check(adapted.History[0].Policy.Mode==ProductPreparationMode.Hold && adapted.History[1].Policy.Mode==ProductPreparationMode.Adapt,"each revision retains its policy and actor");
        var before = (await Board(s))!;
        check(before.Queued==1 && before.Completed==0 && before.AdaptedCompleted==0,"adapted queued data is not counted as registered");
        await Process(s);
        var done = (await Board(s))!;
        check(done.Completed==1 && done.AdaptedCompleted==1 && done.Items.Single().ExternalProductId=="900","only verified remote mapping counts as adapted completion");
        check(remote.Draft!.PrimaryPrice==125 && source.Product.Stock==10 && remote.Draft.PackageWeight==200,"adaptation never changes source price stock or explicit weight");
        check((await Prepare(s,adapted.Input,adapted.Revision))!.JobId==adapted.JobId && remote.Creates==1,"repeat after completion cannot resend");
        await Conflict(async()=>await Prepare(s,adapted.Input with {CategoryId=999},adapted.Revision));

        s=await Scope(); await Policy(s,new(ProductPreparationMode.Adapt,true,true));
        var missing=(await Prepare(s,new(ProductTransferDirection.ToPlatform,"44")))!;
        check(missing.JobId is null && missing.Issues.Count==3,"adapt mode never invents category preparation or package weight");
        check((await Board(s))!.NeedsAttention==1,"unresolved item visible in board totals");
        var valid=input with {Description="valid"};
        var fixedItem=(await Prepare(s,valid,missing.Revision))!;
        check(fixedItem.JobId>0 && fixedItem.Revision==2,"user can repair missing metadata and enqueue same item");
        await using(var db=new HyperIntegrationContext(options))
        {
            var collector=new IntegrationProductPreparationCollector(db);
            await collector.CollectAsync(new(7,"tenant-a"),s.ConnectionId,44,null,default);
            check((await Api(db).BoardAsync(s,0,25,default))!.Items.Single().Revision==2,"later discovery never overwrites user edits or submitted job");
        }
        await Process(s);

        s=await Scope();
        var pair=await Task.WhenAll(Prepare(s,valid),Prepare(s,valid));
        check(pair[0]!.Id==pair[1]!.Id && pair[0]!.JobId==pair[1]!.JobId,"concurrent identical requests commit one preparation receipt and job");
        check(await Board(s with {TenantId="foreign"}) is null,"foreign tenant cannot read board or audit");
        await Process(s);

        s=await Scope();
        await Policy(s,new(ProductPreparationMode.Adapt,true,true),ProductTransferDirection.ToAccounting);
        var inbound=(await Prepare(s,new(ProductTransferDirection.ToAccounting,"60438766")))!;
        check(inbound.JobId is null && inbound.Issues.Single().Code=="AccountingCreationNotAvailable","unsupported accounting destination remains actionable, never reports success");
        await using(var db = new HyperIntegrationContext(options))
            check((await Api(db).PolicyAsync(s,ProductTransferDirection.ToPlatform,default))!.Mode==ProductPreparationMode.Hold,"policies are isolated by transfer direction");
        await using(var db = new HyperIntegrationContext(options))
            await db.ExternalIntegrationConnections.Where(x=>x.Id==s.ConnectionId).ExecuteUpdateAsync(x=>x.SetProperty(c=>c.Provider,Provider.Digikala));
        check((await Prepare(s,valid))!.Issues.Any(x=>x.Code=="PlatformCreationNotAvailable"),"unsupported platform captured without using Basalam rules or transport");

        s=await Scope(); await Policy(s,new(ProductPreparationMode.Adapt,true,true)); remote.Fail=true;
        await Prepare(s,input); await Process(s); remote.Fail=false;
        var uncertain=(await Board(s))!;
        check(uncertain.NeedsAttention==1 && uncertain.AdaptedCompleted==0 && uncertain.Items.Single().Issues.Any(x=>x.Code=="CreationOutcomeUnknown"),"uncertain POST visible but never counted as adapted success");
        await Conflict(async()=>await Prepare(s,valid,1));

        s=await Scope();
        await using(var db = new HyperIntegrationContext(options))
        {
            var collector=new IntegrationProductPreparationCollector(db);
            await collector.CollectAsync(new(7,"tenant-a"),s.ConnectionId,null,"60438766",default);
            await collector.CollectAsync(new(7,"tenant-a"),s.ConnectionId,null,"60438766",default);
            check((await Api(db).BoardAsync(s,0,25,default))!.Items.Single().History.Count==1,"repeated inbound discovery collects one issue and preserves history");
            await Queue(db).EnqueueAsync(new(7,"tenant-a"),s.ConnectionId,new("discover",IntegrationSyncItem.Product,IntegrationSyncTrigger.Initial),default);
            check(await Queue(db).ProcessConnectionAsync(s.ConnectionId,default),"catalog scenario reaches preparation collector");
        }
        var discovered=(await Board(s))!;
        check(discovered.Total==2 && discovered.NeedsAttention==2 && discovered.Items.All(x=>x.JobId is null),"unmapped products from both directions collected without implicit create");

        s=await Scope();
        await using(var db = new HyperIntegrationContext(options))
        {
            db.IntegrationWebhookInbox.Add(new() { ConnectionId=s.ConnectionId,ExternalEventId="unmapped-notification",
                EventType="product.updated",PayloadJson="{\"product_id\":60438766}",ReceivedAtUtc=DateTime.UtcNow });
            await db.SaveChangesAsync();
            await Queue(db).EnqueueAsync(new(7,"tenant-a"),s.ConnectionId,new("unmapped-notification",IntegrationSyncItem.Product,IntegrationSyncTrigger.BoothChanged),default);
        }
        await Process(s);
        check((await Board(s))!.Items.Single().Input.SourceProductId=="60438766" && source.Commands==0,
            "real inbound dispatcher retains unmapped product for remediation without accounting mutation");

        s=await Scope();
        await using(var db = new HyperIntegrationContext(options))
        {
            var api=new IntegrationProductReadinessApi(db,new IntegrationProductDraftApi(db,new FailingQueue(),source),source);
            try { await api.PrepareAsync(s,valid,0,"fixture",default); throw new Exception("Expected queue failure"); }
            catch(InvalidOperationException e) when(e.Message=="FixtureQueueFailure") { }
        }
        await using(var db = new HyperIntegrationContext(options))
            check(!await db.Set<IntegrationProductPreparation>().AnyAsync(x=>x.ConnectionId==s.ConnectionId)
                && !await db.Set<IntegrationProductCreation>().AnyAsync(x=>x.ConnectionId==s.ConnectionId),"queue failure rolls back preparation and receipt together");
    }
    sealed class Creator : IExternalIntegrationAdapter,IExternalProductCreator
    {
        public int Creates; public bool Fail; public ExternalProductDraft? Draft;
        public Provider Provider=>Provider.Basalam; public bool IsImplemented=>true;
        public bool SupportsCredentialType(IntegrationCredentialType type)=>true;
        public Task PrepareCreationAsync(ExternalIntegrationConnection c,CancellationToken ct)=>Task.CompletedTask;
        public Task<string> CreateDraftAsync(ExternalIntegrationConnection c,ExternalProductDraft draft,CancellationToken ct)
        { Creates++; Draft=draft; if(Fail)throw new HttpRequestException("fixture uncertain POST");return Task.FromResult("900"); }
        public Task<ExternalCatalogItem> ReadCreatedAsync(ExternalIntegrationConnection c,string id,CancellationToken ct)=>Task.FromResult(new ExternalCatalogItem(id,null,Draft!.Name,Draft.PrimaryPrice,0,null));
        public Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(ExternalIntegrationConnection c,CancellationToken ct)=>Task.FromResult<IReadOnlyCollection<ExternalCatalogItem>>([]);
        public Task PublishInventoryAsync(ExternalIntegrationConnection c,IReadOnlyCollection<ExternalInventoryUpdate> u,CancellationToken ct)=>throw new NotSupportedException();
    }
    sealed class FailingQueue : IIntegrationScenarioQueue
    {
        public Task<long> EnqueueAsync(OwnedIntegrationShop s,long c,IntegrationScenarioRequest r,CancellationToken ct)=>throw new InvalidOperationException("FixtureQueueFailure");
        public Task<bool> ProcessNextAsync(CancellationToken ct)=>throw new NotSupportedException();
        public Task<IReadOnlyList<IntegrationScenarioConnection>> ConnectionsAsync(OwnedIntegrationShop s,CancellationToken ct)=>throw new NotSupportedException();
        public Task<IReadOnlyList<IntegrationScenarioJob>> RecentAsync(OwnedIntegrationShop s,CancellationToken ct)=>throw new NotSupportedException();
    }
}
