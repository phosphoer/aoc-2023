namespace Puzzles
{
  public static class Puzzle4
  {
    public static void Run()
    {
      Console.WriteLine("Puzzle 4...");

      string[] cardStrings = Puzzle4Input.Input.Split(Environment.NewLine);
      int cardValueTotal = 0;
      List<Card> cards = new();
      for (int i = 0; i < cardStrings.Length; ++i)
      {
        Card card = Card.FromString(cardStrings[i]);
        card.CardId = i + 1;
        cardValueTotal += card.Value;
        cards.Add(card);
        // Console.WriteLine($"Card {i} has {card.ValidWins.Count} wins and a score of {card.Value}");
      }

      int totalCardCount = cards.Count;
      int iterations = 0;
      int maxStackSize = 0;
      Stack<Card> copyStack = new(cards);
      while (copyStack.Count > 0)
      {
        Card card = copyStack.Pop();
        for (int i = 0; i < card.ValidWins.Count; ++i)
        {
          Card copy = cards[card.Index + i + 1];
          copyStack.Push(copy);
          totalCardCount += 1;
          maxStackSize = Math.Max(maxStackSize, copyStack.Count);
        }

        iterations++;
      }

      Console.WriteLine($"Card value total: {cardValueTotal}");
      Console.WriteLine($"Card copies total: {totalCardCount}");
      Console.WriteLine($"Max stack size: {maxStackSize}");
      Console.WriteLine($"Iteration count: {iterations}");
    }

    public static void CreateCardCopies(Card card, List<Card> cards, List<Card> copies, int depth)
    {
      for (int i = 0; i < depth; ++i)
        Console.Write('.');

      Console.WriteLine($"Card {card.CardId} has {card.ValidWins.Count} matching numbers");
      for (int j = card.Index + 1; j < card.Index + 1 + card.ValidWins.Count; ++j)
      {
        Card copy = cards[j];
        copies.Add(card);

        if (copy.ValidWins.Count > 0)
          CreateCardCopies(copy, cards, copies, depth + 1);
      }
    }

    public struct Card
    {
      public static Card FromString(string cardStr)
      {
        Card card;
        card.WinningNumbers = new();
        card.Numbers = new();
        card.ValidWins = new();
        card.CardId = 0;

        int startIndex = cardStr.IndexOf(':') + 1;
        string parseString = string.Empty;
        bool isWinning = true;
        for (int i = startIndex; i < cardStr.Length; ++i)
        {
          char c = cardStr[i];
          bool isDigit = char.IsDigit(c);
          if (isDigit)
            parseString += c;

          if (!isDigit || i == cardStr.Length - 1)
          {
            if (int.TryParse(parseString, out int number))
            {
              if (isWinning)
                card.WinningNumbers.Add(number);
              else
                card.Numbers.Add(number);

              if (!isWinning && card.WinningNumbers.Contains(number))
                card.ValidWins.Add(number);
            }

            if (c == '|')
              isWinning = false;

            parseString = string.Empty;
          }
        }

        return card;
      }

      public int Value => ValidWins.Count > 0 ? 1 << ValidWins.Count - 1 : 0;
      public int Index => CardId - 1;

      // public Card Copy()
      // {
      //   Card copy;
      //   copy.CardId = CardId;
      //   copy.Numbers = Numbers;
      //   copy.ValidWins = ValidWins;
      //   copy.WinningNumbers = WinningNumbers;
      //   return copy;
      // }

      public int CardId;
      public List<int> WinningNumbers;
      public List<int> Numbers;
      public List<int> ValidWins;
    }
  }
}