using MTGApplication.General.ViewModels;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI.UseCases;

public class FetchCardWithId(IScryfallImporter importer) : UseCaseFunc<Guid, Task<CardImportResult>>
{
  public async override Task<CardImportResult> Execute(Guid id)
    => await importer.ImportWithId(id);
}