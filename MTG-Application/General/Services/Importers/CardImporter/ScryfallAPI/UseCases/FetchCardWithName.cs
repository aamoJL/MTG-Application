using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI.UseCases;

public class FetchCardWithName(IScryfallImporter importer) : UseCaseFunc<string, Task<CardImportResult>>
{
  public bool Fuzzy { get; init; } = true;

  public async override Task<CardImportResult> Execute(string name)
    => await importer.ImportWithName(name, Fuzzy);
}
