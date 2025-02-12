using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.Features.EdhrecSearch.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using System.Diagnostics.CodeAnalysis;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;

namespace MTGApplication.Features.EdhrecSearch.ViewModels;
public partial class EdhrecSearchPageViewModel(IMTGCardImporter importer) : CardSearchViewModel(importer)
{
  public CommanderTheme[] CommanderThemes { get; set; } = [];

  [NotNull] public IAsyncRelayCommand<CommanderTheme>? ChangeCommanderThemeCommand => field ??= new ChangeCommanderTheme(this).Command;
}