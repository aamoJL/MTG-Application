﻿using MTGApplication.General.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter;

public abstract class MTGCardImporter : ICardImporter<MTGCardInfo>
{
  public abstract string Name { get; }
  public abstract int PageSize { get; }

  public abstract Task<CardImportResult<MTGCardInfo>> ImportCardsWithSearchQuery(string searchParams);
  public abstract Task<CardImportResult<MTGCardInfo>> ImportFromString(string importText);
  public abstract Task<CardImportResult<MTGCardInfo>> ImportFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false);
  /// <summary>
  /// Fetch cards from the API using <see cref="MTGCardDTO"/> array
  /// </summary>
  public abstract Task<CardImportResult<MTGCardInfo>> ImportFromDTOs(IEnumerable<MTGCardDTO> dtos);
}