﻿using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.ConfirmationService.Extensions;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowOverrideDialog : ShowDialogUseCase<ConfirmationResult>
{
  public ShowOverrideDialog(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<ConfirmationResult> ShowDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    PrimaryButtonText = "Override",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();
}
