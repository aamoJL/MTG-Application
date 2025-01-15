using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter;

public abstract class MTGCardImporter : ICardImporter
{
  public abstract string Name { get; }
  public abstract int PageSize { get; }

  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  /// <exception cref="System.Text.Json.JsonException"></exception>
  public abstract Task<CardImportResult> ImportCardsWithSearchQuery(string searchParams, bool pagination = true);
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public abstract Task<CardImportResult> ImportWithString(string importText);
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  /// <exception cref="System.Text.Json.JsonException"></exception>
  public abstract Task<CardImportResult> ImportWithUri(string pageUri, bool paperOnly = false, bool fetchAll = false);
  /// <summary>
  /// Fetch cards from the API using <see cref="MTGCardDTO"/> array
  /// </summary>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public abstract Task<CardImportResult> ImportWithDTOs(IEnumerable<MTGCardDTO> dtos);
}