using CommunityToolkit.Mvvm.Input;
using MTGApplication.Models;
using System.Collections.Generic;

namespace MTGApplication.Services;

/// <summary>
/// Service that handles command Undo and Redo
/// </summary>
public partial class CommandService
{
  public interface ICommand
  {
    public void Execute();
    public void Undo();
  }

  public class RemoveCardFromCardlistCommand : ICommand
  {
    MTGCardDeck CardDeck { get; }
    public Enums.CardlistType ListType { get; }
    public MTGCard Card { get; }

    public RemoveCardFromCardlistCommand(MTGCardDeck deck, Enums.CardlistType listType, MTGCard card)
    {
      CardDeck = deck;
      ListType = listType;
      Card = card;
    }

    public void Execute()
    {
      if(CardDeck == null) return;
      CardDeck.RemoveFromCardlist(ListType, Card);
    }

    public void Undo()
    {
      if (CardDeck == null) return;
      CardDeck.AddToCardlist(ListType, Card);
    }
  }

  public Stack<ICommand> UndoCommandStack { get; } = new();
  public Stack<ICommand> RedoCommandStack { get; } = new();

  [RelayCommand]
  public void Undo()
  {
    if(UndoCommandStack.Count > 0)
    {
      var command = UndoCommandStack.Pop();
      command.Undo();
      RedoCommandStack.Push(command);
    }
  }

  [RelayCommand]
  public void Redo()
  {
    if (RedoCommandStack.Count > 0)
    {
      var command = RedoCommandStack.Pop();
      command.Execute();
      UndoCommandStack.Push(command);
    }
  }

  public void Execute(ICommand command)
  {
    UndoCommandStack.Push(command);
    RedoCommandStack.Clear();
    command.Execute();
  }
}
