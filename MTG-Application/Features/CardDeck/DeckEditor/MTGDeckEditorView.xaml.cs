using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MTGApplication.API.CardAPI;
using MTGApplication.Database;
using MTGApplication.Database.Extensions;
using MTGApplication.Database.Repositories;
using MTGApplication.General;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public sealed partial class MTGDeckEditorView : Page
{
  public MTGDeckEditorView()
  {
    InitializeComponent();
  }

  public MTGDeckEditorViewModel ViewModel { get; set; } = new();

  protected override void OnNavigatedTo(NavigationEventArgs e)
  {
    base.OnNavigatedTo(e);

    if (e.Parameter is string deckName) ViewModel.OpenDeckCommand.Execute(deckName);
  }

  private void CardView_DragOver(object sender, Microsoft.UI.Xaml.DragEventArgs e)
  {
    //TODO: event
  }

  private void CardView_Drop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
  {
    //TODO: event
  }

  private void CardView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
  {
    //TODO: event
  }

  private void CardView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
  {
    //TODO: event
  }

  private void CardView_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
  {
    //TODO: event
  }

  private void CardView_LosingFocus(Microsoft.UI.Xaml.UIElement sender, Microsoft.UI.Xaml.Input.LosingFocusEventArgs args)
  {
    //TODO: event
  }
}

public class GetDeckOrDefaultUseCase : UseCase<Task<MTGCardDeck>>
{
  public GetDeckOrDefaultUseCase(string name, IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Name = name;
    Repository = repository;
    CardAPI = cardAPI;
  }

  public string Name { get; }
  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public async override Task<MTGCardDeck> Execute()
  {
    if (string.IsNullOrEmpty(Name)) return default;

    return await (await Repository.Get(Name)).AsMTGCardDeck(CardAPI);
  }
}

public class DeckDTORepository : IRepository<MTGCardDeckDTO>
{
  public DeckDTORepository(CardDbContextFactory dbContextFactory) => DbContextFactory = dbContextFactory;

  public CardDbContextFactory DbContextFactory { get; }

  public Task<bool> Add(MTGCardDeckDTO item) => throw new NotImplementedException();
  public Task<bool> AddOrUpdate(MTGCardDeckDTO item) => throw new NotImplementedException();
  public Task<bool> Exists(string name) => throw new NotImplementedException();
  public Task<IEnumerable<MTGCardDeckDTO>> Get() => throw new NotImplementedException();

  public async Task<MTGCardDeckDTO> Get(string name, Expression<Func<MTGCardDeckDTO, object>>[] Includes = null)
  {
    using var db = DbContextFactory.CreateDbContext();
    db.ChangeTracker.LazyLoadingEnabled = false;
    db.ChangeTracker.AutoDetectChangesEnabled = false;
    var deck = db.MTGDecks.Where(x => x.Name == name).SetIncludesOrDefault(Includes).FirstOrDefault();
    db.ChangeTracker.AutoDetectChangesEnabled = true;
    return await Task.FromResult(deck);
  }

  public Task<bool> Remove(MTGCardDeckDTO item) => throw new NotImplementedException();
  public Task<bool> Update(MTGCardDeckDTO item) => throw new NotImplementedException();
}
