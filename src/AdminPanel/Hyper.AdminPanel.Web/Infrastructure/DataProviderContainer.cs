using Neo.Bpms.Infrastructure.Features.Orm.Provider;
using System.Collections;

namespace Hyper.AdminPanel.Web.Infrastructure;

public class DataProviderContainer(IConfiguration configuration, ILogger<DataProviderContainer> logger) : IDataProviderContainer
{
    private Dictionary<string, IDataProvider>? _providers;
    private const string DefaultProviderName = "default";

    public IEnumerator<IDataProvider> GetEnumerator()
    {
        return _providers?.Values.GetEnumerator()!;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Init()
    {
        _providers = new Dictionary<string, IDataProvider>
        {
            { DefaultProviderName, new SqlServerProvider(configuration, logger, DefaultProviderName) },
            { "Domain", new SqlServerProvider(configuration, logger, "Domain", false) },
        };
    }

    public IDataProvider GetProvider(string providerName)
    {
        if (_providers != null)
        {
            _providers.TryGetValue(providerName, out IDataProvider? provider);
            if (provider == null)
                _providers.TryGetValue("Domain", out provider);
            return provider!;
        }
        return null!;
    }
}
