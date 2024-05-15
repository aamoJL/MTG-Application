﻿using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Extensions;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public class GetDeckNames : UseCase<Task<string[]>>
{
  public GetDeckNames(IRepository<MTGCardDeckDTO> repository) => Repository = repository;

  public IRepository<MTGCardDeckDTO> Repository { get; }

  public override async Task<string[]> Execute()
    => (await Repository.Get(ExpressionExtensions.EmptyArray<MTGCardDeckDTO>())).Select(x => x.Name).OrderBy(x => x).ToArray();
}