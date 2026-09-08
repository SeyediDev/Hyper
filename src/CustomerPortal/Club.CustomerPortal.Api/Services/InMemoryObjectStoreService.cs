using System.Collections.Concurrent;
using Neo.Domain.Features.ObjectStore;
using Neo.Domain.Features.ObjectStore.Dto;

namespace Hyper.CustomerPortal.Api.Services;

internal sealed class InMemoryObjectStoreService : IObjectStoreService
{
    private readonly ConcurrentDictionary<string, ObjectStoreDto> _store = new();

    public Task<bool> HasAsync(string objectId)
    {
        return Task.FromResult(_store.ContainsKey(objectId));
    }

    public Task<string?> CheckSumAsync(string objectId) => Task.FromResult<string?>(null);

    public Task<string?> UploadFileAsync(string objectId, ObjectStoreDto fileData)
    {
        _store[objectId] = fileData;
        return Task.FromResult<string?>(objectId);
    }

    public Task<ObjectStoreDto?> DownloadFileAsync(string objectId)
    {
        if (_store.TryGetValue(objectId, out ObjectStoreDto? item))
        {
            return Task.FromResult<ObjectStoreDto?>(item);
        }
        return Task.FromResult<ObjectStoreDto?>(null);
    }
}

