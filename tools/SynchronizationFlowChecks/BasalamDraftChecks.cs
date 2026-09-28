using System.Net;
using Basalam.SDK;
using Basalam.SDK.Config;
using Hyper.Infrastructure.Features.Integrations;

static class BasalamDraftChecks
{
    public static async Task Run(ExternalIntegrationConnection connection, BasalamOAuthStore store, Action<bool,string> check)
    {
        var transport=new DraftTransport(); using var http=new HttpClient(transport);
        using var sdk=new BasalamClient(new BasalamConfig {MaxRetries=5,RetryDelayMilliseconds=0},httpClient:http);
        var adapter=new BasalamSdkAdapter(sdk,store);
        await adapter.PrepareCreationAsync(connection,default);
        var id=await adapter.CreateDraftAsync(connection,new("Vacuum bag",125,100,2,200,null,null),default);
        var snapshot=await adapter.ReadCreatedAsync(connection,id,default);
        check(id=="700" && snapshot.Title=="Vacuum bag" && snapshot.Price==125 && snapshot.Inventory==0
            && transport.Posts==1 && transport.Gets==1 && transport.Grant=="Bearer fixture-grant",
            "production draft adapter uses scoped vault token, one POST and exact-identity GET");
        transport.Vendor=999;
        try{await adapter.ReadCreatedAsync(connection,id,default); throw new Exception("Expected vendor mismatch");}
        catch(IntegrationProviderException e) when(e.Code=="VendorMismatch")
        {check(transport.Posts==1,"draft readback cannot adopt a product from another booth");}
        transport.Vendor=71; transport.WrongId=true;
        try{await adapter.ReadCreatedAsync(connection,id,default); throw new Exception("Expected identity mismatch");}
        catch(System.Text.Json.JsonException){check(transport.Posts==1,"draft readback rejects a different returned product ID");}
    }
    sealed class DraftTransport:HttpMessageHandler
    {
        public int Posts,Gets,Vendor=71; public bool WrongId; public string? Grant;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)
        {
            Grant=request.Headers.Authorization?.ToString();
            if(request.Method==HttpMethod.Post)
            {
                if(request.RequestUri!.AbsolutePath!="/v1/vendors/71/products") throw new Exception("Wrong booth route");
                Posts++; return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created){Content=new StringContent("{\"id\":700}")});
            }
            if(request.Method!=HttpMethod.Get || request.RequestUri!.AbsolutePath!="/v1/products/700") throw new Exception("Unexpected request");
            Gets++;
            var body=System.Text.Json.JsonSerializer.Serialize(new{id=WrongId?999:700,title="Vacuum bag",price=125,inventory=0,vendor=new{id=Vendor}});
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK){Content=new StringContent(body)});
        }
    }
}
