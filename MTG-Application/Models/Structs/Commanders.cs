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
}
