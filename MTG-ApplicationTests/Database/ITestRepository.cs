using MTGApplication.Interfaces;

namespace MTGApplicationTests.Database;

public interface ITestRepository<T> : IDisposable, IRepository<T>
{
}
