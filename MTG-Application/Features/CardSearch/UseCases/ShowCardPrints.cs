using Microsoft.UI.Xaml;
using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.DragAndDrop;
using MTGApplication.General.Views.Styles.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public class ShowCardPrints(XamlRoot xamlRoot) : UseCaseFunc<Confirmation<IEnumerable<MTGCard>>, Task>
{
  public XamlRoot XamlRoot { get; } = xamlRoot;
  public ListViewDragAndDrop<MTGCard> CardDragAndDrop { get; } = new(itemToArgsConverter: (item) => new(item))
  {
    AcceptMove = false
  };

  public override async Task Execute(Confirmation<IEnumerable<MTGCard>> msg)
  {
    ArgumentNullException.ThrowIfNull(XamlRoot);

    Application.Current.Resources.TryGetValue(nameof(MTGPrintGridViewItemTemplate), out var template);

    await DialogService.ShowAsync(XamlRoot, new GridViewDialog(
      title: msg.Title,
      items: [.. msg.Data],
      itemTemplate: (DataTemplate)template)
    {
      PrimaryButtonText = string.Empty,
      CloseButtonText = "Close",
      CanDragItems = true,
      CanSelectItems = false,
      OnItemDragStarting = (args) =>
      {
        if (args.Items.FirstOrDefault() is MTGCard card)
        {
          CardDragAndDrop.OnInternalDragStarting(new CardMoveArgs(card), out var operation);
          args.Data.RequestedOperation = operation;
        }
      }
    });
  }
}