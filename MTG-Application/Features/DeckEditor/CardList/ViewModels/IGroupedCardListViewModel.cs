using CommunityToolkit.Mvvm.Input;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public interface IGroupedCardListViewModel : ICardListViewModel
{
  IAsyncRelayCommand? AddGroupCommand { get; }
  IRelayCommand<CardGroupViewModel>? RemoveGroupCommand { get; }
  IAsyncRelayCommand<CardGroupViewModel>? RenameGroupCommand { get; }
}
