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
    Groups.CollectionChanged += Groups_CollectionChanged;

    // empty key will be the last group
    var groups = new List<string>([.. Model.Select(c => c.Group)
      .Where(g => g != string.Empty).Order(), string.Empty]).Distinct();

    foreach (var item in groups)
      Groups.Add(new(item, Model));
  }

  public ObservableCollection<DeckCardGroupViewModel> GroupViewModels { get; } = [];

  public GroupedCardListConfirmers GroupedListConfirmers { private get; init; } = new();

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

      new ShowNotification(Notifier).Execute(new(NotificationType.Success, "Group added successfully."));
    }
    catch (Exception e)
    {
      new ShowNotification(Notifier).Execute(new(NotificationType.Error, e.Message));
    }
  }

  private ObservableCollection<DeckEditorCardGroup> Groups { get; } = [];
  private DeckCardGroupViewModel.Factory GroupViewModelFactory => field ??= new()
  {
    Worker = Worker,
    Importer = Importer,
    EdhrecImporter = EdhrecImporter,
    Notifier = Notifier,
    UndoStack = UndoStack,
    ListConfirmers = Confirmers,
    GroupConfirmers = GroupedListConfirmers.GroupConfirmers,
    CardFactory = CardViewModelFactory,
    NameValidator = key => !Groups.Any(x => x.GroupKey == key),
    OnGroupDelete = OnDeleteGroup,
  };

  private void OnDeleteGroup(DeckEditorCardGroup group)
  {
    UndoStack.PushAndExecute(
      new ReversibleCommand<DeckEditorCardGroup>(group)
      {
        ReversibleAction = new ReversibleRemoveGroupAction(Groups)
      });
  }

  private void Groups_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    foreach (var item in e.AddedItems<DeckEditorCardGroup>())
    {
      item.PropertyChanged += GroupItem_PropertyChanged;

      GroupViewModels.Add(GroupViewModelFactory.Build(item));
    }
    foreach (var item in e.RemovedItems<DeckEditorCardGroup>())
    {
      item.PropertyChanged -= GroupItem_PropertyChanged;

      if (GroupViewModels.TryFindIndex(vm => vm.GroupKey == item.GroupKey, out var i))
        GroupViewModels.RemoveAt(i);
    }
  }

  private void GroupItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is not DeckEditorCardGroup group)
      return;

    switch (e.PropertyName)
    {
      case nameof(DeckEditorCardGroup.GroupKey):
        if (GroupViewModels.TryFindIndices(vm => vm.GroupKey == group.GroupKey, out var indices) && indices.Length > 1)
          GroupViewModels.RemoveAt(indices.First());
        break;
    }
  }
}
