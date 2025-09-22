using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckTesting.UseCases;

public class DrawCard : SyncCommand
{
  public DrawCard(DeckTestingPageViewModel viewmodel) => Viewmodel = viewmodel;

  public DeckTestingPageViewModel Viewmodel
  {
    get;
    private init
    {
      field = value;
      field.Library.CollectionChanged += Library_CollectionChanged;
    }
  }

  protected override bool CanExecute() => Viewmodel.Library.Count > 0;

  protected override void Execute()
  {
    if (!CanExecute()) return;

    var card = Viewmodel.Library[0];
    Viewmodel.Library.RemoveAt(0);
    Viewmodel.Hand.Add(card);
  }

  private void Library_CollectionChanged(object? _, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    => Command.NotifyCanExecuteChanged();
}
