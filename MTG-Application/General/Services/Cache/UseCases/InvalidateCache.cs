using MTGApplication.General.ViewModels;

namespace MTGApplication.General.Services.Cache.UseCases;

public class InvalidateCache : UseCaseAction<IMemoryCache<Caching.CacheKey>>
{
  public override void Execute(IMemoryCache<Caching.CacheKey> cache)
    => cache.Invalidate();
}
