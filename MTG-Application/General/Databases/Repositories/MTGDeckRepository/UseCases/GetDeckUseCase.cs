﻿using MTGApplication.API.CardAPI;
using MTGApplication.General.UseCases;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.General.Databases.Repositories.MTGDeckRepository;

public class GetDeckUseCase : UseCase<string, Task<MTGCardDeck>>
{
  public GetDeckUseCase(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  public async override Task<MTGCardDeck> Execute(string name)
  {
    if (string.IsNullOrEmpty(name)) return null;

    return await (await Repository.Get(name)).AsMTGCardDeck(CardAPI);
  }
}