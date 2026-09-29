using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Basalam.SDK;
using Basalam.SDK.Config;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Hyperyek.Accounting.Contracts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

static class ParcelLifecycleChecks
{
    public static async Task Run(DbContextOptions<HyperIntegrationContext> options, Action<bool,string> check)
    {
        await using var db=new HyperIntegrationContext(options);
        await db.Database.ExecuteSqlRawAsync("DROP TABLE dbo.IntegrationParcelCommands;");
        var schema=await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory,"ensure-integration-parcel-commands.sql"));
        await db.Database.ExecuteSqlRawAsync(schema);await db.Database.ExecuteSqlRawAsync(schema);
        check(true,"actual additive parcel schema applied twice on disposable SQL only");
        var connection=new ExternalIntegrationConnection {ShopId=888,TenantId="parcel-fixture",Provider=Provider.Basalam,
            AccountIdentifier="71",DisplayName="parcel fixture",CredentialType=IntegrationCredentialType.OAuth2,
            CredentialsJson="{\"webhookSecret\":\"parcel-secret\"}"};
        db.Add(connection);await db.SaveChangesAsync();
        var remote=new Remote();using var http=new HttpClient(remote);
        using var sdk=new BasalamClient(new BasalamConfig {MaxRetries=3,RetryDelayMilliseconds=0},httpClient:http);
        var oauth=new BasalamOAuthService(Options.Create(new BasalamOAuthSettings()),http,new EphemeralDataProtectionProvider(), FixtureProtection.Provider());
        db.Add(oauth.CreateTokenEntity(connection.Id,888,"parcel-fixture",Provider.Basalam,
            new(){AccessToken="parcel-grant",ExpiresIn=3600,Scope="vendor.parcel.read vendor.parcel.write"}));
        db.Add(new ExternalOrderMapping {ConnectionId=connection.Id,ShopId=888,ExternalOrderId="700",ExternalParcelId="70",HyperSaleOrderId=99});
        await db.SaveChangesAsync();
        var adapter=new BasalamSdkAdapter(sdk,new BasalamOAuthStore(db, oauth, FixtureProtection.LegacyVault()));
        var resolver=new IntegrationStrategyResolver([adapter]);
        var owner=new Owner();using var ownerHttp=new HttpClient(owner){BaseAddress=new Uri("https://accounting.fixture.invalid/")};
        var commands=new HyperyekAccountingApiClient(ownerHttp);
        var dispatcher=new IntegrationBusinessEventDispatcher(commands,new UnregisteredIntegrationEngagementPort(),null!,db,resolver);
        var queue=new IntegrationScenarioQueue(db,new(db,resolver,null!,Options.Create(new IntegrationInventoryCaptureOptions()),
            Options.Create(new BasalamOAuthSettings()),null!,dispatcher,commands,parcelCommands:new IntegrationParcelCommandProcessor(db,resolver)));
        var ingress=new IntegrationWebhookIngress(db,new IntegrationWebhookVerifier(FixtureProtection.LegacyVault()));
        var index=0;
        async Task<IntegrationScenarioJob> Deliver(string body="{\"id\":70}")
        {
            var accepted=await ingress.ReceiveAsync(new(Hyper.Integration.Contracts.IntegrationProvider.Basalam,"71","parcel-event-"+(++index),
                "VENDOR_PARCEL_CHANGES",null,null,System.Text.Encoding.UTF8.GetBytes(body),Authorization:"Bearer parcel-secret"),default);
            check(accepted.Status==WebhookIngressStatus.Accepted,"parcel webhook queued with durable notification identity");
            check(await queue.ProcessConnectionAsync(connection.Id,default),"parcel scenario worker executes");
            return await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x=>x.Id==accepted.ScenarioJobId);
        }
        var job=await Deliver("{\"id\":70,\"order_item_id\":999,\"order_id\":999,\"status\":\"delivered\",\"tracking_code\":\"fake\"}");
        check(job.Status==IntegrationScenarioStatus.Completed && owner.Last is {ExternalOrderId:"700",ExternalParcelId:"70",Status:"preparing",TrackingCode:null}
            && owner.Last.EventId==job.EventId && owner.Last.Scope==new AccountingScope(888,"parcel-fixture"),
            "thin/stale webhook hydrates scoped provider parcel; forged status/order/tracking ignored");
        check(remote.Grant=="Bearer parcel-grant" && remote.Reads==1,"parcel read uses scoped OAuth and official gateway route");
        remote.Status=3238;remote.Tracking="TRACK-1";await Deliver();
        check(owner.Last is {Status:"shipped",TrackingCode:"TRACK-1"},"posted code and post_receipt tracking mapped to accounting command");
        remote.Delivered=true;await Deliver();
        check(owner.Last!.Status=="delivered","delivery requires explicit provider delivery flag");
        remote.Status=3195;remote.Delivered=false;var calls=owner.Calls;job=await Deliver();
        check(job.Status==IntegrationScenarioStatus.NeedsAttention && owner.Calls==calls,"satisfaction alone does not invent delivery");
        remote.Delivered=true;remote.Status=3067;job=await Deliver();
        check(job.Status==IntegrationScenarioStatus.NeedsAttention && owner.Calls==calls,"cancelled parcel never becomes delivered from stale flag");
        remote.Status=3238;remote.Vendor=999;job=await Deliver();
        check(job.ErrorCode=="VendorMismatch" && owner.Calls==calls,"foreign vendor rejected before accounting call");
        remote.Vendor=71;remote.Id=71;job=await Deliver();
        check(job.ErrorCode=="ParcelInvalidResponse" && owner.Calls==calls,"wrong returned parcel identity rejected");
        remote.Id=70;remote.Order=701;job=await Deliver();
        check(job.Status==IntegrationScenarioStatus.NeedsAttention && job.ResultJson!.Contains("ParcelInvoiceMappingRequired") && owner.Calls==calls,
            "unbound order cannot update an invoice");
        remote.Order=700;
        await db.ExternalOrderMappings.Where(x=>x.ConnectionId==connection.Id).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.ExternalParcelId,"another"));
        job=await Deliver();check(job.Status==IntegrationScenarioStatus.NeedsAttention && owner.Calls==calls,"different parcel on same order requires explicit mapping");
        await db.ExternalOrderMappings.Where(x=>x.ConnectionId==connection.Id).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.ExternalParcelId,"70"));
        owner.Pending=true;job=await Deliver();
        check(job.Status==IntegrationScenarioStatus.NeedsAttention && job.ResultJson!.Contains("AccountingTrackingCodeNeedsReview"),
            "precise accounting tracking limitation remains actionable in scenario result");
        owner.Pending=false;remote.Fail=true;calls=owner.Calls;job=await Deliver();
        check(job.Status==IntegrationScenarioStatus.Pending && job.ErrorCode=="Http503" && owner.Calls==calls,
            "temporary provider detail failure retries read without accounting mutation");
        remote.Fail=false;
        await db.IntegrationScenarioJobs.Where(x=>x.Id==job.Id).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.NextAttemptAtUtc,DateTime.UtcNow.AddSeconds(-1)));
        check(await queue.ProcessConnectionAsync(connection.Id,default),"provider read retry recovers in durable queue");
        check((await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x=>x.Id==job.Id)).Status==IntegrationScenarioStatus.Completed,
            "retry completes after authoritative read and accounting acknowledgment");

        var api=new IntegrationParcelApi(db,queue);var scope=new IntegrationConnectionCommandRequest(888,"parcel-fixture",connection.Id);
        var prepare=new ParcelCommandRequest(Guid.NewGuid(),"70","700",ParcelCommandTarget.Preparing);
        check(await api.StartAsync(scope with{TenantId="foreign"},prepare,"tester",default) is null,
            "foreign tenant cannot enqueue a parcel command");
        remote.Status=3739;remote.Delivered=false;remote.Tracking=null;
        var queued=(await api.StartAsync(scope,prepare,"tester",default))!;
        check(queued.Status=="Pending" && remote.Posts==0,"explicit parcel preparation queues without synchronous mutation");
        check((await api.StartAsync(scope,prepare,"tester",default))!.JobId==queued.JobId,"identical parcel intake returns same durable receipt");
        try{await api.StartAsync(scope,prepare with{OrderId="999"},"tester",default);throw new Exception("Expected request conflict");}
        catch(InvalidOperationException e) when(e.Message=="ParcelRequestConflict"){check(true,"request identity cannot be reused with a different order");}
        check(await queue.ProcessConnectionAsync(connection.Id,default),"preparation command processed by real scenario queue");
        check((await api.ReadAsync(scope,prepare.RequestId,default))!.Status=="Completed" && remote.Posts==1 && remote.Status==3237,
            "preparation confirmed by provider readback after one POST");
        var posted=new ParcelCommandRequest(Guid.NewGuid(),"70","700",ParcelCommandTarget.Posted,3198,"TRACK-2");
        remote.LoseResponse=true;
        var postedReceipt=(await api.StartAsync(scope,posted,"tester",default))!;
        check(await queue.ProcessConnectionAsync(connection.Id,default),"posted transport ambiguity retained by queue");
        check((await api.ReadAsync(scope,posted.RequestId,default))!.Status=="Verifying" && remote.Posts==2,
            "lost POST response not reported completed and not retried inside SDK");
        remote.LoseResponse=false;
        await db.IntegrationScenarioJobs.Where(x=>x.Id==postedReceipt.JobId).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.NextAttemptAtUtc,DateTime.UtcNow.AddSeconds(-1)));
        check(await queue.ProcessConnectionAsync(connection.Id,default),"recovered worker retries verification");
        check((await api.ReadAsync(scope,posted.RequestId,default))!.Status=="Completed" && remote.Posts==2 && remote.Tracking=="TRACK-2",
            "crash-safe recovery performs GET only and checks final tracking");
        try{await api.StartAsync(scope,posted with{RequestId=Guid.NewGuid()},"tester",default);throw new Exception("Expected duplicate target conflict");}
        catch(InvalidOperationException e) when(e.Message=="ParcelCommandAlreadyExists"){check(remote.Posts==2,"new request ID cannot resend an existing parcel transition");}
        check(await api.ReadAsync(scope with{ShopId=999},posted.RequestId,default) is null,"foreign shop cannot read command receipt");

        // New scoped parcel receipt with an uncertain POST that never took effect.
        await db.Set<IntegrationParcelCommand>().Where(x=>x.ConnectionId==connection.Id).ExecuteDeleteAsync();
        remote.Status=3237;remote.Delivered=false;remote.IgnorePost=true;remote.PostStatus=HttpStatusCode.ServiceUnavailable;
        posted=posted with{RequestId=Guid.NewGuid(),TrackingCode="TRACK-3"};
        postedReceipt=(await api.StartAsync(scope,posted,"tester",default))!;
        check(await queue.ProcessConnectionAsync(connection.Id,default),"failed remote POST recorded after send marker");
        var sends=remote.Posts;remote.PostStatus=HttpStatusCode.OK;
        await db.IntegrationScenarioJobs.Where(x=>x.Id==postedReceipt.JobId).ExecuteUpdateAsync(s=>s.SetProperty(x=>x.NextAttemptAtUtc,DateTime.UtcNow.AddSeconds(-1)));
        check(await queue.ProcessConnectionAsync(connection.Id,default),"unconfirmed command rechecked without resending");
        check(remote.Posts==sends && (await api.ReadAsync(scope,posted.RequestId,default))!.Status=="Verifying",
            "503 and a new worker attempt never cause a second shipment POST or false success");
        await db.IntegrationScenarioJobs.Where(x=>x.Id==postedReceipt.JobId).ExecuteUpdateAsync(s=>s
            .SetProperty(x=>x.Attempts,IntegrationRetryPolicy.MaxAttempts-1)
            .SetProperty(x=>x.NextAttemptAtUtc,DateTime.UtcNow.AddSeconds(-1)));
        check(await queue.ProcessConnectionAsync(connection.Id,default),"final verification attempt executes");
        var exhausted=await api.ReadAsync(scope,posted.RequestId,default);
        check(exhausted is {Status:"NeedsAttention",ErrorCode:"ParcelCommandNotConfirmed"} && remote.Posts==sends,
            "exhausted ambiguity becomes actionable without a second POST");

        // Fixture-only reset: production reconciliation must never delete receipts.
        await db.Set<IntegrationParcelCommand>().Where(x=>x.ConnectionId==connection.Id).ExecuteDeleteAsync();
        remote.Status=3238;remote.IgnorePost=false;
        prepare=prepare with{RequestId=Guid.NewGuid()};
        await api.StartAsync(scope,prepare,"tester",default);
        check(await queue.ProcessConnectionAsync(connection.Id,default),"already advanced parcel command executes readback");
        check((await api.ReadAsync(scope,prepare.RequestId,default))!.Status=="Completed" && remote.Posts==sends,
            "preparation never regresses a shipped parcel or repeats a write");
        remote.Status=3237;
        posted=posted with{RequestId=Guid.NewGuid(),ShippingMethod=999};
        await api.StartAsync(scope,posted,"tester",default);
        check(await queue.ProcessConnectionAsync(connection.Id,default),"shipping method mismatch evaluated");
        check((await api.ReadAsync(scope,posted.RequestId,default)) is {Status:"NeedsAttention",ErrorCode:"ParcelShippingMethodChanged"}
            && remote.Posts==sends,"changed shipping method requires review before POST");
        foreach(var invalid in new[]{posted with{RequestId=Guid.Empty},posted with{ParcelId="070"},
            posted with{ShippingMethod=null},posted with{TrackingCode="bad\ncode"},prepare with{TrackingCode="unexpected"}})
        {
            try{await api.StartAsync(scope,invalid,"tester",default);throw new Exception("Expected invalid parcel input");}
            catch(ArgumentException e) when(e.Message=="InvalidParcelCommand")
            {check(remote.Posts==sends,"invalid parcel input rejected without provider side effect");}
        }
    }
    sealed class Remote:HttpMessageHandler
    {
        public int Id=70,Order=700,Vendor=71,Status=3237,Reads,Posts;public bool Delivered,Fail,LoseResponse,IgnorePost;
        public string? Tracking,Grant;public HttpStatusCode PostStatus=HttpStatusCode.OK;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)
        {
            if(request.Method==HttpMethod.Post)
            {
                if(request.RequestUri!.Host!="openapi.basalam.com")throw new Exception("Wrong parcel command host");
                Posts++;
                if(request.RequestUri.AbsolutePath=="/v1/vendor-parcels/70/set-preparation")
                {if(!IgnorePost)Status=3237;}
                else if(request.RequestUri.AbsolutePath=="/v1/vendor-parcels/70/set-posted")
                {
                    var body=JsonDocument.Parse(await request.Content!.ReadAsStringAsync(ct)).RootElement;
                    if(body.GetProperty("shipping_method").GetInt32()!=3198)throw new Exception("Wrong shipping method");
                    if(!IgnorePost){Tracking=body.GetProperty("tracking_code").GetString();Status=3238;}
                }
                else throw new Exception("Wrong parcel command path");
                if(LoseResponse)throw new HttpRequestException("fixture dropped POST response");
                return new(PostStatus){Content=new StringContent("{}")};
            }
            if(request.Method!=HttpMethod.Get || request.RequestUri!.Host!="openapi.basalam.com" || request.RequestUri.AbsolutePath!="/v1/vendor-parcels/70")
                throw new Exception("Unexpected parcel transport route");
            Reads++;Grant=request.Headers.Authorization?.ToString();
            return new HttpResponseMessage(Fail?HttpStatusCode.ServiceUnavailable:HttpStatusCode.OK){Content=JsonContent.Create(new {
                id=Id,vendor=new{id=Vendor},order=new{id=Order},status=new{id=Status,title="untrusted translated label"},is_delivered=Delivered,
                shipping_method=new{current=new{id=3198}},post_receipt=new{tracking_code=Tracking}})};
        }
    }
    sealed class Owner:HttpMessageHandler
    {
        public int Calls;public bool Pending;public ParcelStatusCommand? Last;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)
        {
            if(request.Method!=HttpMethod.Post || request.RequestUri!.AbsolutePath!="/api/hyperyek/v1/accounting/parcels/status")throw new Exception("Unexpected accounting parcel route");
            Calls++;Last=await request.Content!.ReadFromJsonAsync<ParcelStatusCommand>(cancellationToken:ct);
            return new(Pending?HttpStatusCode.Accepted:HttpStatusCode.OK){Content=JsonContent.Create(new AccountingCommandResult(Pending?AccountingCommandStatus.PendingDependency:AccountingCommandStatus.Applied,
                "99",ErrorCode:Pending?"AccountingTrackingCodeNeedsReview":null))};
        }
    }
}
