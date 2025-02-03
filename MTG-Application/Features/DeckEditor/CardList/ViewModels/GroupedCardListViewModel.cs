using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.CardList.Services.Factories;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.GroupedCardListViewModelCommands;

namespace MTGApplication.Features.DeckEditor.ViewModels;

public partial class GroupedCardListViewModel : CardListViewModel
{
  public GroupedCardListViewModel(IMTGCardImporter importer, GroupedCardListConfirmers? confirmers = null) : base(importer, confirmers)
  {
    Confirmers ??= confirmers ?? new();

    PropertyChanging += GroupedCardListViewModel_PropertyChanging;
    PropertyChanged += GroupedCardListViewModel_PropertyChanged;

    Cards.CollectionChanged += Source_CollectionChanged;
  }

  [NotNull] public ObservableCollection<CardGroupViewModel>? Groups => field ??= [new GroupedCardListCardGroupFactory(this).CreateCardGroup(string.Empty)];

  public override GroupedCardListConfirmers Confirmers { get; }

  [NotNull] public IAsyncRelayCommand? AddGroupCommand => field ??= new AddCardGroup(this).Command;
  [NotNull] public IRelayCommand<CardGroupViewModel>? RemoveGroupCommand => field ??= new RemoveCardGroup(this).Command;
  [NotNull] public IAsyncRelayCommand<CardGroupViewModel>? RenameGroupCommand => field ??= new RenameCardGroup(this).Command;

  private void GroupedCardListViewModel_PropertyChanging(object? _, System.ComponentModel.PropertyChangingEventArgs e)
  {
    if (e.PropertyName == nameof(Cards))
      Cards.CollectionChanged -= Source_CollectionChanged;
  }

  private void GroupedCardListViewModel_PropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Cards))
    {
      Groups.Clear();

      foreach (var group in Cards
        .Select(c => c.Group)
        .Where(g => g != string.Empty)
        .Distinct()
        .Order())
      {
        Groups.Add(new GroupedCardListCardGroupFactory(this).CreateCardGroup(group));
      }

      Groups.Add(new GroupedCardListCardGroupFactory(this).CreateCardGroup(string.Empty));

      if (Cards is INotifyCollectionChanged observableSource)
        observableSource.CollectionChanged += Source_CollectionChanged;
    }
  }

  private void Source_CollectionChanged(object? _, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is IList newItems)
    {
      // Add missing groups
      foreach (var item in newItems.OfType<DeckEditorMTGCard>()
        .DistinctBy(g => g.Group)
        .Where(c => Groups.FirstOrDefault(g => g.Key == c.Group) is null))
      {
        Groups.Add(new GroupedCardListCardGroupFactory(this).CreateCardGroup(item.Group));
      }
    }
  }
}