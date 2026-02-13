using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;

namespace MTGApplicationTests.TestUtility.Database;

[Obsolete("Use just simple test repository with a type")]
public class SimpleTestCardCollectionRepository : SimpleTestRepository<MTGCardCollectionDTO> { }