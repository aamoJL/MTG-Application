using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.Features.EdhrecSearch.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;

namespace MTGApplication.Features.EdhrecSearch.ViewModels;
public partial class EdhrecSearchPageViewModel(MTGCardImporter importer) : CardSearchViewModel(importer)
{
  [ObservableProperty] private CommanderTheme[] commanderThemes;
  [ObservableProperty] private CommanderTheme selectedTheme;

  public IAsyncRelayCommand<CommanderTheme> ChangeCommanderThemeCommand => (changeCommanderTheme ??= new ChangeCommanderTheme(this)).Command;

  private ChangeCommanderTheme changeCommanderTheme;
}