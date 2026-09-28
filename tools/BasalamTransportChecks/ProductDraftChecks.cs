using System.Net;
using System.Text.Json;
using Basalam.SDK;
using Basalam.SDK.Config;
using Basalam.SDK.Services;
using Basalam.SDK.Errors;

static class ProductDraftChecks
{
    public static async Task Run(Action<bool,string> check)
    {
        var transport=new Handler(); using var http=new HttpClient(transport);
        using var client=new BasalamClient(new BasalamConfig {MaxRetries=5,RetryDelayMilliseconds=0},httpClient:http);
        var drafts=(IProductDraftService)client.Products;
        var draft=new ProductDraftRequest("Vacuum bag",1000000,50,2,200);
        var id=await drafts.CreateDraftAsync(71,draft);
        using var payload=JsonDocument.Parse(transport.Body!);
        var p=payload.RootElement;
        check(id==700 && transport.Count==1 && transport.Path=="/v1/vendors/71/products","draft POST returns exact created identity");
        check(p.GetProperty("status").GetInt32()==3790 && p.GetProperty("stock").GetInt32()==0
            && p.GetProperty("category_id").GetInt32()==50 && p.GetProperty("preparation_days").GetInt32()==2
            && p.GetProperty("package_weight").GetInt32()==200 && p.GetProperty("primary_price").GetInt64()==1000000,
            "draft uses official typed fields, unpublished status and zero stock");
        check(!p.TryGetProperty("photo",out _) && !p.TryGetProperty("sku",out _) && !p.TryGetProperty("vendor_id",out _),
            "draft omits unspecified metadata and invented identities");
        foreach(var status in new[]{503,429,408,422})
        {
            transport.Status=status; var before=transport.Count;
            try {await drafts.CreateDraftAsync(71,draft); throw new Exception("Expected creation rejection");}
            catch(BasalamAPIError e) {check(e.StatusCode==status && transport.Count==before+1,"draft never retries HTTP "+status+" even with MaxRetries5");}
        }
        var calls=transport.Count;
        try {await drafts.CreateDraftAsync(71,draft with{PackageWeight=0}); throw new Exception("Expected validation");}
        catch(ArgumentException){check(transport.Count==calls,"missing publication metadata fails before HTTP");}
        transport.Status=201; transport.Response="{}";
        try {await drafts.CreateDraftAsync(71,draft); throw new Exception("Expected identity validation");}
        catch(JsonException){check(transport.Count==calls+1,"malformed creation success cannot fabricate product identity");}
    }
    sealed class Handler:HttpMessageHandler
    {
        public int Count,Status=201; public string Response="{\"id\":700}"; public string? Body,Path;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)
        {
            Count++; Path=request.RequestUri!.AbsolutePath; Body=await request.Content!.ReadAsStringAsync(ct);
            return new((HttpStatusCode)Status){Content=new StringContent(Response)};
        }
    }
}
