using MTGApplication.Features.DeckEditor.Models;
using System.ComponentModel;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCommanders;

public partial class CommanderViewModel : CommanderViewModelBase
{
  public CommanderViewModel(DeckEditorMTGDeck deck) : base(deck)
    => Model?.PropertyChanged += Commander_PropertyChanged;

  protected override DeckEditorMTGCard? Model { get => Source.Commander; set => Source.Commander = value; }

  protected override void OnPropertyChanging(PropertyChangingEventArgs e)
  {
    base.OnPropertyChanging(e);

    switch (e.PropertyName)
    {
      case nameof(Source.Commander):
        Model?.PropertyChanged -= Commander_PropertyChanged;
        break;
    }
  }

  protected override void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    base.Source_PropertyChanged(sender, e);

    switch (e.PropertyName)
    {
      case nameof(Source.Commander):
        Model?.PropertyChanged += Commander_PropertyChanged;
        break;
    }
  }

  private void Commander_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Source.Commander.Info): OnPropertyChanged(nameof(Info)); break;
    }
  }
}
