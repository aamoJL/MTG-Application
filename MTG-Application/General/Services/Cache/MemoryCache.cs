using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.General.Services.Cache;

public class MemoryCache<T> : IMemoryCache<T> where T : notnull
{
  private readonly MemoryCache _cache = new(new MemoryCacheOptions());

  public IEnumerable<T> Keys => _cache.Keys.OfType<T>();

  public object? Get(T key) => _cache.Get(key);

  public TItem? GetOrCreate<TItem>(T key, Func<ICacheEntry, TItem> factory) => _cache.GetOrCreate(key, factory);

  public object Set(T key, object value) => _cache.Set(key, value);

  public void Remove(T key) => _cache.Remove(key);

  public TItem? Get<TItem>(T key) => _cache.Get<TItem>(key);

  public void Clear() => _cache.Clear();
}