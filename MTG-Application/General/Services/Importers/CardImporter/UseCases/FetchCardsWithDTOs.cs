using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class FetchCardsWithDTOs(IMTGCardImporter importer) : UseCaseFunc<IEnumerable<MTGCardDTO>, Task<CardImportResult>>
{
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  /// <exception cref="System.Text.Json.JsonException"></exception>
  public async override Task<CardImportResult> Execute(IEnumerable<MTGCardDTO> dtos)
    => await importer.ImportWithDTOs(dtos);
}