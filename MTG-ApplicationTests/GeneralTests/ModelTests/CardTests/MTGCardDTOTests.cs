using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ModelTests.CardTests;

[TestClass]
public class MTGCardDTOTests
{
  [TestMethod]
  public void Compare_AreSame()
  {
    var dto = new MTGCardDTO(MTGCardInfoMocker.MockInfo(scryfallId: Guid.Empty, oracleId: Guid.Empty));
    var dto2 = new MTGCardDTO(MTGCardInfoMocker.MockInfo(scryfallId: Guid.Empty, oracleId: Guid.Empty));

    Assert.IsTrue(dto.Compare(dto2));
  }

  [TestMethod]
  public void Compare_DifferentId_AreSame()
  {
    var dto = new MTGCardDTO(MTGCardInfoMocker.MockInfo(scryfallId: Guid.Empty, oracleId: Guid.Empty))
    {
      Id = 1
    };
    var dto2 = new MTGCardDTO(MTGCardInfoMocker.MockInfo(scryfallId: Guid.Empty, oracleId: Guid.Empty))
    {
      Id = 2
    };

    Assert.IsTrue(dto.Compare(dto2));
  }

  [TestMethod]
  public void Compare_DifferentCount_AreSame()
  {
    var dto = new MTGCardDTO(MTGCardInfoMocker.MockInfo(scryfallId: Guid.Empty, oracleId: Guid.Empty))
    {
      Count = 1
    };
    var dto2 = new MTGCardDTO(MTGCardInfoMocker.MockInfo(scryfallId: Guid.Empty, oracleId: Guid.Empty))
    {
      Count = 2
    };

    Assert.IsTrue(dto.Compare(dto2));
  }

  [TestMethod]
  public void Compare_AreDifferent()
  {
    var dto = new MTGCardDTO(MTGCardInfoMocker.MockInfo(scryfallId: Guid.Empty, oracleId: Guid.Empty))
    {
      Name = "First"
    };
    var dto2 = new MTGCardDTO(MTGCardInfoMocker.MockInfo(scryfallId: Guid.Empty, oracleId: Guid.Empty))
    {
      Name = "Second"
    };

    Assert.IsFalse(dto.Compare(dto2));
  }
}