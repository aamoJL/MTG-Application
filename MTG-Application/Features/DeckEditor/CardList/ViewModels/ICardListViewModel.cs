using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public interface ICardListViewModel
{
  ObservableCollection<DeckEditorMTGCard> Cards { get; }

  IAsyncRelayCommand<DeckEditorMTGCard>? AddCardCommand { get; }
  IRelayCommand<DeckEditorMTGCard>? RemoveCardCommand { get; }
  IRelayCommand<DeckEditorMTGCard>? BeginMoveFromCommand { get; }
  IAsyncRelayCommand<DeckEditorMTGCard>? BeginMoveToCommand { get; }
  IRelayCommand<DeckEditorMTGCard>? ExecuteMoveCommand { get; }
  IRelayCommand? ClearCommand { get; }
  IAsyncRelayCommand<string>? ImportCardsCommand { get; }
  IAsyncRelayCommand<string>? ExportCardsCommand { get; }
}
