using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;

namespace MTGApplicationTests.TestUtility.Mocker;

public static class MTGCardDTOMocker
{
  public static MTGCardDTO Mock(
    string name,
    int count = 1,
    Guid? scryfallId = null,
    Guid? oracleId = null,
    string? setCode = null,
    string? collectorNumber = null,
    string group = "")
  {
    return new(
        scryfallId: scryfallId ?? new("4f8dc511-e307-4412-bb79-375a6077312d"),
        oracleId: oracleId ?? new("8095ca78-db19-4724-a6ff-eacc85fa2274"),
        setCode: setCode ?? "otj",
        collectorNumber: collectorNumber ?? "1",
        name: name,
        count: count,
        group: group);
  }
}
