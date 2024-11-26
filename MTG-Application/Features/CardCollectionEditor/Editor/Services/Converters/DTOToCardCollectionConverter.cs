﻿using MTGApplication.Features.CardCollectionEditor.CardCollection.Models;
using MTGApplication.Features.CardCollectionEditor.CardCollectionList.Models;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.Editor.Services.Converters;

public class DTOToCardCollectionConverter(MTGCardImporter importer)
{
  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  public async Task<MTGCardCollection> Convert(MTGCardCollectionDTO dto)
  {
    if (dto == null)
      return null;

    try
    {
      return new MTGCardCollection()
      {
        Name = dto.Name,
        CollectionLists = new(await Task.WhenAll(dto.CollectionLists.Select(async x =>
        {
          return new MTGCardCollectionList()
          {
            Name = x.Name,
            SearchQuery = x.SearchQuery,
            Cards = new((await importer.ImportFromDTOs([.. x.Cards])).Found.Select(x => new CardCollectionMTGCard(x.Info)))
          };
        })))
      };
    }
    catch { throw; }
  }
}