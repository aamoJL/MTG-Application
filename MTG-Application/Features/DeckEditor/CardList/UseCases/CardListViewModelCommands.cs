using MTGApplication.Features.DeckEditor.ViewModels;

namespace MTGApplication.Features.DeckEditor.CardList.UseCases;

public partial class CardListViewModelCommands(CardListViewModel viewmodel) { }

public partial class GroupedCardListViewModelCommands(GroupedCardListViewModel viewmodel) { }

public partial class CardGroupViewModelCommands(CardGroupViewModel groupViewmodel, GroupedCardListViewModel listViewmodel) { }
