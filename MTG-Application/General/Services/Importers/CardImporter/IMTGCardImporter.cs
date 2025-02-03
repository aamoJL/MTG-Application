using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter;

public interface IMTGCardImporter : ICardImporter
{
  /// <summary>
  /// Fetch cards from the API using <see cref="MTGCardDTO"/> array
  /// </summary>
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public abstract Task<CardImportResult> ImportWithDTOs(IEnumerable<MTGCardDTO> dtos);
}