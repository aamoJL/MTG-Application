using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Features.EdhrecSearch.UseCases;

public class FetchCardsByTheme(IMTGCardImporter Importer, IEdhrecImporter edhrecImporter) : UseCaseFunc<CommanderTheme, Task<CardImportResult>>
{
  public IMTGCardImporter Importer { get; } = Importer;
  public IEdhrecImporter EdhrecImporter { get; } = edhrecImporter;

  public override async Task<CardImportResult> Execute(CommanderTheme theme)
  {
    var query = string.Join(Environment.NewLine, await EdhrecImporter.FetchNewCardNames(theme.Uri));

    return await Importer.ImportWithString(query);
  }
}
