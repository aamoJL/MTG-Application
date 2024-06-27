using MTGApplication.General.Models;

namespace MTGApplicationTests.TestUtility.Mocker;

public class MTGCardMocker
{
  public static MTGCard Mock()
    => new(MTGCardInfoMocker.MockInfo());

  public static IEnumerable<MTGCard> Mock(int count)
    => Enumerable.Range(1, count).Select(x => new MTGCard(MTGCardInfoMocker.MockInfo()));
}