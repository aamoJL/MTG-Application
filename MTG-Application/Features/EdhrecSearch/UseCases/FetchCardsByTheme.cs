using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.EdhrecSearch.UseCases;

public class FetchCardsByTheme(IMTGCardImporter importer, IEdhrecImporter edhrecImporter) : UseCaseFunc<CommanderTheme, Task<CardImportResult>>
{
  public override async Task<CardImportResult> Execute(CommanderTheme theme)
  {
    var names = await edhrecImporter.FetchNewCardNames(theme.Uri);
    var query = string.Join(Environment.NewLine, names);

    return await new FetchCardsWithImportString(importer).Execute(query);
  }
}
