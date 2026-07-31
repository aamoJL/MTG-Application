using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace MTGApplication.General.Services.Cache;

public interface IMemoryCache<TKey>
{
  public IEnumerable<TKey> Keys { get; }

  public object? Get(TKey key);
  public TItem? Get<TItem>(TKey key);
  public TItem? GetOrCreate<TItem>(TKey key, Func<ICacheEntry, TItem> factory);
  public object? Set(TKey key, object value);
  public void Remove(TKey key);
  public void Clear();
}
