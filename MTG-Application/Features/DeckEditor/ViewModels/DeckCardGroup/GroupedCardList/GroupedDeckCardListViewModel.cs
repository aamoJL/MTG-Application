using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.UseCases.ReversibleActions;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;
using MTGApplication.Features.DeckEditor.ViewModels.DeckCardList;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.NotificationService.UseCases;
using MTGApplication.General.Services.ReversibleCommandService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCardGroup.GroupedCardList;

public partial class GroupedDeckCardListViewModel : DeckCardListViewModel
{
  public GroupedDeckCardListViewModel(ObservableCollection<DeckEditorMTGCard> list) : base(list)
  {
    // empty key will be the last group
    var groups = new List<string>([.. Model.Select(c => c.Group)
      .Where(g => g != string.Empty).Order(), string.Empty]).Distinct();

    foreach (var item in groups)
      Groups.Add(new(item, Model));

    Groups.CollectionChanged += Groups_CollectionChanged;
  }

  public ObservableCollection<DeckCardGroupViewModel> GroupViewModels
  {
    get => field ??= GroupViewModels = [.. Groups.Select(GroupViewModelFactory.Build)]; private set;
  }

  public GroupedCardListConfirmers GroupedListConfirmers { private get; init; } = new();

  private ObservableCollection<DeckEditorCardGroup> Groups { get; } = [];
  private DeckCardGroupViewModel.Factory GroupViewModelFactory => field ??= new()
  {
    Worker = Worker,
    Importer = Importer,
    EdhrecImporter = EdhrecImporter,
    Notifier = Notifier,
    UndoStack = UndoStack,
    CardFilter = CardFilter,
    CardSorter = CardSorter,
    ListConfirmers = Confirmers,
    GroupConfirmers = GroupedListConfirmers.GroupConfirmers,
    CardFactory = CardViewModelFactory,
    OnGroupDelete = OnDeleteGroup,
    OnGroupRename = OnGroupRename,
  };

  [RelayCommand]
  private async Task AddGroup()
  {
    try
    {
      var groupName = await GroupedListConfirmers.ConfirmAddGroup(GroupConfirmations.GetAddCardGroupConfirmation());

      if (string.IsNullOrEmpty(groupName))
        return;

      if (GroupViewModels.Any(x => x.GroupKey == groupName))
      {
        // TODO: add validation to confirmation?
        new ShowNotification(Notifier).Execute(new(NotificationType.Error, "Group already exists."));
        return;
      }

      UndoStack.PushAndExecute(
        new ReversibleCommand<DeckEditorCardGroup>(new(groupName, Model))
        {
          ReversibleAction = new ReversibleAddGroupAction(Groups)
        });
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  private void OnDeleteGroup(DeckEditorCardGroup group)
  {
    UndoStack.PushAndExecute(
      new ReversibleCommand<DeckEditorCardGroup>(group)
      {
        ReversibleAction = new ReversibleRemoveGroupAction(Groups)
      });
  }

  private void OnGroupRename(DeckEditorCardGroup group, string newKey)
  {
    if (Groups.Any(x => x.GroupKey == newKey))
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, "Group already exists"));
      return;
    }

    var addNewGroup = new ReversibleCommand<DeckEditorCardGroup>(new(newKey, Model))
    {
      ReversibleAction = new ReversibleAddGroupAction(Groups)
    };
    var regroupCards = new ReversibleCommand<(IEnumerable<DeckEditorMTGCard> Cards, string NewKey)>(new([.. group.Cards], newKey))
    {
      ReversibleAction = new ReversibleCardCollectionGroupsChangeAction()
    };
    var removeOldGroup = new ReversibleCommand<DeckEditorCardGroup>(group)
    {
      ReversibleAction = new ReversibleRemoveGroupAction(Groups)
    };

    UndoStack.PushAndExecute(new CombinedReversibleCommand()
    {
      Commands = [addNewGroup, regroupCards, removeOldGroup]
    });
  }

  protected override DeckEditorMTGCard TransformCardModel(DeckEditorMTGCard card) => card.Copy();

  private void Groups_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    foreach (var item in e.AddedItems<DeckEditorCardGroup>())
    {
      if (!GroupViewModels.Any(x => x.GroupKey == item.GroupKey))
      {
        if (Groups.TryFindIndex(x => x.GroupKey == item.GroupKey, out var i))
          GroupViewModels.Insert(i, GroupViewModelFactory.Build(item));
      }
    }
    foreach (var item in e.RemovedItems<DeckEditorCardGroup>())
    {
      if (GroupViewModels.TryFindIndex(vm => vm.GroupKey == item.GroupKey, out var i))
        GroupViewModels.RemoveAt(i);
    }
  }
}
