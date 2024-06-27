using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckTesting.UseCases;

public class DrawCard : ViewModelCommand<DeckTestingPageViewModel>
{
  public DrawCard(DeckTestingPageViewModel viewmodel) : base(viewmodel)
    => viewmodel.Library.CollectionChanged += Library_CollectionChanged;

  protected override bool CanExecute() => Viewmodel.Library.Count > 0;

  protected override void Execute()
  {
    if (!CanExecute()) return;

    var card = Viewmodel.Library[0];
    Viewmodel.Library.RemoveAt(0);
    Viewmodel.Hand.Add(card);
  }

  private void Library_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    => Command.NotifyCanExecuteChanged();
}
