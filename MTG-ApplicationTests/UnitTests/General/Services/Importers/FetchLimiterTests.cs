using MTGApplication.General.Services.Importers;

namespace MTGApplicationTests.UnitTests.General.Services.Importers;

[TestClass]
public class FetchLimiterTests
{
  [TestMethod]
  public async Task Limit()
  {
    var limit = 500;
    var limiter = new FetchLimiter();
    var start = DateTime.Now;

    var tasks = new Task[]
    {
      limiter.Wait(limit), // first does not have to wait, if last fetch stamp is over the limit
      limiter.Wait(limit * 2), // = 2
      limiter.Wait(limit), // = 3
      limiter.Wait(limit * 2), // = 5
    };

    await Task.WhenAll(tasks);

    var stop = DateTime.Now;
    var deltaMillis = (stop - start).TotalMilliseconds;

    Assert.IsGreaterThanOrEqualTo(5 * limit, deltaMillis);
  }
}
