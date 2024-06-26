using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Features.CardSearch.ViewModels;
using MTGApplication.Features.EdhrecSearch.Models;
using MTGApplication.General.Services.Importers.CardImporter;

namespace MTGApplication.Features.EdhrecSearch.ViewModels;
public partial class EdhrecSearchPageViewModel(MTGCardImporter importer) : CardSearchViewModel(importer)
{
  [ObservableProperty] private CommanderTheme[] commanderThemes;
  [ObservableProperty] private CommanderTheme selectedTheme;
}