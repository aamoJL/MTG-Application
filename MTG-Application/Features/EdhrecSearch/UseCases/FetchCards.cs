using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;
using static MTGApplication.General.Services.Importers.CardImporter.EdhrecImporter;

namespace MTGApplication.Features.EdhrecSearch.UseCases;

public class FetchCards(IMTGCardImporter Importer) : UseCaseFunc<CommanderTheme, Task<CardImportResult>>
{
  public IMTGCardImporter Importer { get; } = Importer;

  public override async Task<CardImportResult> Execute(CommanderTheme theme)
  {
    var query = string.Join(Environment.NewLine, await FetchNewCardNames(theme.Uri));

    return await Importer.ImportWithString(query);
  }
}
