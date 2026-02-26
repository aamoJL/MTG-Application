using MTGApplication.Features.DeckEditor.Models;
using System.ComponentModel;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;

public partial class PartnerViewModel : CommanderViewModelBase
{
  public PartnerViewModel(DeckEditorMTGDeck deck) : base(deck)
    => Model?.PropertyChanged += CommanderPartner_PropertyChanged;

  protected override DeckEditorMTGCard? Model { get => Source.CommanderPartner; set => Source.CommanderPartner = value; }

  protected override void OnPropertyChanging(PropertyChangingEventArgs e)
  {
    base.OnPropertyChanging(e);

    switch (e.PropertyName)
    {
      case nameof(Source.CommanderPartner):
        Source.CommanderPartner?.PropertyChanged -= CommanderPartner_PropertyChanged;
        break;
    }
  }

  protected override void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    base.Source_PropertyChanged(sender, e);

    switch (e.PropertyName)
    {
      case nameof(Source.CommanderPartner):
        Source.CommanderPartner?.PropertyChanged += CommanderPartner_PropertyChanged;
        break;
    }
  }

  private void CommanderPartner_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Source.CommanderPartner.Info): OnPropertyChanged(nameof(Info)); break;
    }
  }
}
