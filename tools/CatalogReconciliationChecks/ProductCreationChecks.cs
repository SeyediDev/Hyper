using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

static class ProductCreationChecks
{
    public static async Task Run(DbContextOptions<HyperIntegrationContext> options, Action<bool,string> check)
    {
        var source = new AccountingFixture();
        var remote = new Creator();
        var resolver = new IntegrationStrategyResolver([remote]);
        var vendor = 800;
        IntegrationScenarioQueue Queue(HyperIntegrationContext db)
        {
            var settings = Options.Create(new IntegrationInventoryCaptureOptions());
            return new(db, new(db, resolver, null!, settings, Options.Create(new BasalamOAuthSettings()), null!, null!, source,
                new IntegrationProductCreationProcessor(db, resolver, source)));
        }
        async Task<IntegrationProductDraftRequest> Request()
        {
            await using var db = new HyperIntegrationContext(options);
            var c = new ExternalIntegrationConnection { ShopId=7, TenantId="tenant-a", Provider=Provider.Basalam,
                AccountIdentifier=(++vendor).ToString(), DisplayName="draft fixture", CredentialsJson="{}", CredentialType=IntegrationCredentialType.BearerToken };
            db.Add(c); await db.SaveChangesAsync();
            return new(7,"tenant-a",c.Id,Guid.NewGuid(),44,100,2,200,"Approved description",123);
        }
        async Task<IntegrationProductDraftStatus?> Start(IntegrationProductDraftRequest request)
        {
            await using var db = new HyperIntegrationContext(options);
            return await new IntegrationProductDraftApi(db,Queue(db),source).StartAsync(request,default);
        }
        async Task<IntegrationProductDraftStatus> Read(IntegrationProductDraftRequest request)
        {
            await using var db = new HyperIntegrationContext(options);
            return (await new IntegrationProductDraftApi(db,Queue(db),source)
                .ReadAsync(new(request.ShopId,request.TenantId,request.ConnectionId),request.RequestId,default))!;
        }
        async Task Process(IntegrationProductDraftRequest request)
        {
            var status=await Read(request);
            await using var db = new HyperIntegrationContext(options);
            await db.IntegrationScenarioJobs.Where(x=>x.Id==status.JobId)
                .ExecuteUpdateAsync(s=>s.SetProperty(x=>x.NextAttemptAtUtc,DateTime.UtcNow.AddSeconds(-1)));
            check(await Queue(db).ProcessConnectionAsync(request.ConnectionId,default),"creation worker processes durable scoped job");
        }
        async Task Reject(Func<Task> action, string error)
        {
            try { await action(); }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException && e.Message==error)
            { check(true,error); return; }
            throw new InvalidOperationException("Expected "+error);
        }
        async Task<int> Mappings(long connection)
        {
            await using var db = new HyperIntegrationContext(options);
            return await db.ExternalProductMappings.CountAsync(x=>x.ConnectionId==connection);
        }
        var r=await Request();
        await Reject(async()=>await Start(r with {CategoryId=0}),"InvalidProductDraft");
        await Reject(async()=>await Start(r with {PreparationDays=null}),"InvalidProductDraft");
        check(await Start(r with {TenantId="other"}) is null,"foreign tenant cannot create a draft or reveal connection");
        var queued=(await Start(r))!;
        var same=(await Start(r))!;
        check(queued.JobId==same.JobId && same.Status=="Pending" && remote.Creates==0,"repeated ingress queues once and never sends inline");
        await Reject(async()=>await Start(r with {CategoryId=200}),"DraftRequestConflict");
        await Reject(async()=>await Start(r with {RequestId=Guid.NewGuid()}),"ProductCreationAlreadyExists");
        await using(var db = new HyperIntegrationContext(options))
            check(await new IntegrationMappingApi(db).CreateProductMappingAsync(new(7,"tenant-a",r.ConnectionId,44,"999"),default) is null,
                "manual mapping cannot race a pending creation");
        await Process(r);
        var done=await Read(r);
        check(done.Status=="Completed" && done.ExternalProductId=="700" && done.MappingId>0
            && await Mappings(r.ConnectionId)==1 && remote.Creates==1 && remote.Reads==1,"confirmed creation maps exactly its returned identity");
        check(remote.LastDraft!.Name==source.Product.Name && remote.LastDraft.PrimaryPrice==125
            && remote.LastDraft.CategoryId==100,"draft uses accounting title/price and explicit publication metadata");
        check((await Start(r))!.Status=="Completed" && remote.Creates==1,"completed request cannot create again");

        r=await Request(); await Start(r); var before=remote.Creates;
        remote.FailSend=true; await Process(r); remote.FailSend=false;
        check((await Read(r)).ErrorCode=="CreationOutcomeUnknown" && await Mappings(r.ConnectionId)==0
            && remote.Creates==before+1,"lost POST response is actionable with no guessed mapping");
        await Reject(async()=>await Start(r with {RequestId=Guid.NewGuid()}),"ProductCreationAlreadyExists");
        check((await Start(r))!.Status=="NeedsAttention" && remote.Creates==before+1,"same or fresh request cannot duplicate uncertain POST");

        r=await Request(); await Start(r); before=remote.Creates;
        await using(var db=new HyperIntegrationContext(options))
            await db.Set<IntegrationProductCreation>().Where(x=>x.ConnectionId==r.ConnectionId)
                .ExecuteUpdateAsync(s=>s.SetProperty(x=>x.State,(byte)1));
        await Process(r);
        check((await Read(r)).ErrorCode=="CreationOutcomeUnknown" && remote.Creates==before,"crash after send marker never repeats POST");

        r=await Request(); await Start(r); before=remote.Creates;
        remote.FailRead=true; await Process(r); remote.FailRead=false;
        check((await Read(r)).Status=="Verifying" && await Mappings(r.ConnectionId)==0,"transient GET keeps received identity before mapping");
        await Process(r);
        check((await Read(r)).Status=="Completed" && remote.Creates==before+1,"read-back retry GET does not repeat creation POST");

        r=await Request(); await Start(r); before=remote.Creates;
        source.Product=source.Product with {Name="Source changed after approval"}; await Process(r);
        check((await Read(r)).ErrorCode=="CreationSourceChanged" && remote.Creates==before,"changed source needs fresh review before send");
        source.Product=source.Product with {Name="Rice"};
        r=await Request(); await Start(r); remote.WrongRead=true; await Process(r); remote.WrongRead=false;
        check((await Read(r)).ErrorCode=="CreationReadbackMismatch" && await Mappings(r.ConnectionId)==0,"mismatched read-back cannot create mapping");

        r=await Request(); await Start(r); before=remote.Creates;
        await using(var db=new HyperIntegrationContext(options))
            await db.ExternalIntegrationConnections.Where(x=>x.Id==r.ConnectionId)
                .ExecuteUpdateAsync(s=>s.SetProperty(x=>x.AccountIdentifier,"9999"));
        await Process(r);
        check((await Read(r)).ErrorCode=="CreationConnectionChanged" && remote.Creates==before,"changed booth identity cannot redirect an approved draft");

        r=await Request(); await Start(r); before=remote.Creates;
        remote.BeforeReadReturn=async()=>
        {
            await using var db=new HyperIntegrationContext(options);
            await db.ExternalIntegrationConnections.Where(x=>x.Id==r.ConnectionId)
                .ExecuteUpdateAsync(s=>s.SetProperty(x=>x.AccountIdentifier,"9998"));
        };
        await Process(r); remote.BeforeReadReturn=null;
        var changedDuringRead=await Read(r);
        check(changedDuringRead.ErrorCode=="CreationConnectionChanged" && changedDuringRead.ExternalProductId=="700"
            && remote.Creates==before+1 && await Mappings(r.ConnectionId)==0,
            "booth change during provider read-back preserves remote identity without mapping to changed connection");

        r=await Request(); await Start(r); before=remote.Creates;
        remote.FailPrepare=true; await Process(r); remote.FailPrepare=false;
        check((await Read(r)).Status=="Pending" && remote.Creates==before,"credential preflight failure cannot mark a nonexistent POST as sent");
        await Process(r);
        check((await Read(r)).Status=="Completed" && remote.Creates==before+1,"preflight recovery performs only the first creation POST");

        r=await Request();
        var responses=await Task.WhenAll(Start(r),Start(r));
        check(responses[0]!.JobId==responses[1]!.JobId,"concurrent duplicate starts elect one SQL receipt/job");
        await Process(r);
        await using(var db=new HyperIntegrationContext(options))
            check(await new IntegrationProductDraftApi(db,Queue(db),source).ReadAsync(new(7,"other",r.ConnectionId),r.RequestId,default) is null,
                "foreign tenant cannot read draft results");
    }

    sealed class Creator : IExternalIntegrationAdapter, IExternalProductCreator
    {
        public Provider Provider=>Provider.Basalam;
        public bool IsImplemented=>true;
        public bool SupportsCredentialType(IntegrationCredentialType type)=>true;
        public int Creates,Reads; public bool FailSend,FailRead,WrongRead,FailPrepare; public ExternalProductDraft? LastDraft;
        public Func<Task>? BeforeReadReturn;
        public Task PrepareCreationAsync(ExternalIntegrationConnection connection,CancellationToken ct)
        {
            if(FailPrepare) throw new HttpRequestException("fixture preflight outage");
            return Task.CompletedTask;
        }
        public Task<string> CreateDraftAsync(ExternalIntegrationConnection connection,ExternalProductDraft draft,CancellationToken ct)
        {
            Creates++; LastDraft=draft;
            if(FailSend) throw new HttpRequestException("fixture lost response");
            return Task.FromResult("700");
        }
        public async Task<ExternalCatalogItem> ReadCreatedAsync(ExternalIntegrationConnection connection,string id,CancellationToken ct)
        {
            Reads++;
            if(FailRead) throw new HttpRequestException("fixture GET outage");
            if(BeforeReadReturn is not null) await BeforeReadReturn();
            return new ExternalCatalogItem(id,null,WrongRead ? "Other" : LastDraft!.Name,LastDraft!.PrimaryPrice,0,null);
        }
        public Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(ExternalIntegrationConnection c,CancellationToken ct)=>throw new NotSupportedException();
        public Task PublishInventoryAsync(ExternalIntegrationConnection c,IReadOnlyCollection<ExternalInventoryUpdate> u,CancellationToken ct)=>throw new NotSupportedException();
    }
}
