using Microsoft.Extensions.Primitives;
using MTGApplication.Extensions;

namespace MTGApplication.Models.Structs;

public readonly struct Commanders
{
  public Commanders(string commander, string partner)
  {
    Commander = commander;
    Partner = partner;
  }

  public string Commander { get; }
  public string Partner { get; }

  public string AsKebabString()
  {
    var stringBuilder = new System.Text.StringBuilder();
    stringBuilder.Append(Commander.ToKebabCase().ToLower());

    if (!string.IsNullOrEmpty(Partner))
    {
      if (!string.IsNullOrEmpty(Commander))
      {
        stringBuilder.Append('-');
      }
      stringBuilder.Append(Partner.ToKebabCase().ToLower());
    }

    return stringBuilder.ToString();
  }
}
