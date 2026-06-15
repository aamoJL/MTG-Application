using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers;

public class FetchLimiter
{
  private static readonly SemaphoreSlim semaphore = new(1, 1);
  private static DateTime lastStamp = DateTime.Now;

  public async Task Wait(int limitMillis)
  {
    await semaphore.WaitAsync();

    var stamp = DateTime.Now;
    var deltaMillis = (stamp - lastStamp).TotalMilliseconds;
    var remainingMillis = Math.Max(0, limitMillis - deltaMillis);

    lastStamp = stamp.AddMilliseconds(remainingMillis);

    await Task.Delay((int)remainingMillis);

    semaphore.Release();
  }
}