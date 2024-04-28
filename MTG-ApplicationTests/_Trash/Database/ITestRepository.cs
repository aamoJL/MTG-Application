using MTGApplication.General.Databases.Repositories;

namespace MTGApplicationTests.Database;

public interface ITestRepository<T> : IDisposable, IRepository<T>
{
}
