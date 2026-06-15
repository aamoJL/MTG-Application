using MTGApplication.General.Services.Importers;

namespace MTGApplicationTests.UnitTests.General.Services.Importers;

[TestClass]
public class FetchLimiterTests
{
  public TestContext TestContext { get; set; }

  [TestMethod]
  public async Task Limit()
  {
    var limit = 500;
    var limiter = new FetchLimiter();
    var start = DateTime.Now;

    var tasks = new Task[]
    {
      limiter.Wait(limit), // = 1
      limiter.Wait(limit * 2), // = 3
      limiter.Wait(limit), // = 4
      limiter.Wait(limit * 2), // = 6
    };

    await Task.WhenAll(tasks);

    var stop = DateTime.Now;
    var deltaMillis = (stop - start).TotalMilliseconds;

    Assert.IsGreaterThanOrEqualTo(6 * limit, deltaMillis);
  }
}
