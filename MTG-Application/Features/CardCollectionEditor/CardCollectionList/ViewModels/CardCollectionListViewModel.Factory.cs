using MTGApplication.Features.CardCollection.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionListViewModel
{
  public class Factory
  {
    public CardCollectionListConfirmers Confirmers { get; init; } = new();
    public Notifier Notifier { get; init; } = new();
    public ClipboardService ClipboardService { get; init; } = new();

    public async Task<CardCollectionListViewModel> Build(MTGCardCollectionList model, MTGCardImporter importer, Func<string, bool> nameValidation)
    {
      var viewmodel = new CardCollectionListViewModel(model, importer, nameValidation)
      {
        Confirmers = Confirmers,
        Notifier = Notifier,
        ClipboardService = ClipboardService
      };

      await viewmodel.UpdateQueryCards();

      return viewmodel;
    }
  }
}