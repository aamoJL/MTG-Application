using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Services;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.CardCollectionList.ViewModels;

public partial class CardCollectionListViewModel
{
  public class Factory
  {
    public CardCollectionListConfirmers Confirmers { get; init; } = new();
    public Notifier Notifier { get; init; } = new();
    public ClipboardService ClipboardService { get; init; } = new();

    public async Task<CardCollectionListViewModel> Build(MTGCardCollectionList model, IMTGCardImporter importer, Func<string, bool> existsValidation)
    {
      var viewmodel = new CardCollectionListViewModel(model, importer, existsValidation)
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