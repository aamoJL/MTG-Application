using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;

public interface IScryfallImporter
{
  public Task<CardImportResult> ImportWithName(string name, bool fuzzy);
  public Task<CardImportResult> ImportWithId(Guid id);
  public bool TryParseCardIdFromUri(string data, out Guid id);
  public bool TryParseCardNameFromUri(string data, out string name);
}
