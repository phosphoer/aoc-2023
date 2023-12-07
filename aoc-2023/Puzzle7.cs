namespace Puzzles
{
  public static class Puzzle7
  {
    public const bool kJokersWild = true;

    public static void Run()
    {
      string input = Puzzle7Input.Input;
      string[] inputLines = input.Split(Environment.NewLine);

      // Parse file
      List<HandBid> hands = new();
      foreach (var line in inputLines)
      {
        string[] handAndBidStr = line.Split(' ');
        HandBid handBid;
        handBid.Hand = handAndBidStr[0];
        handBid.Bid = long.Parse(handAndBidStr[1]);
        hands.Add(handBid);
      }

      hands.Sort((a, b) =>
      {
        return CompareHands(a.Hand, b.Hand);
      });

      int rank = 1;
      long totalWinnings = 0;
      foreach (var hand in hands)
      {
        long scaledBid = hand.Bid * rank;
        long strength = GetHandStrength(hand.Hand);
        totalWinnings += scaledBid;
        Console.WriteLine($"{hand.Hand} (Strength {strength}): {rank} x {hand.Bid} = {scaledBid}");
        rank += 1;
      }

      Console.WriteLine($"Total winnings: {totalWinnings}");
    }

    public struct HandBid
    {
      public string Hand;
      public long Bid;
    }

    public static int GetCardValue(char card)
    {
      if (char.IsDigit(card))
        return int.Parse(card.ToString());
      else if (card == 'A')
        return 14;
      else if (card == 'K')
        return 13;
      else if (card == 'Q')
        return 12;
      else if (!kJokersWild && card == 'J')
        return 11;
      else if (kJokersWild && card == 'J')
        return 1;
      else if (card == 'T')
        return 10;

      return 0;
    }

    public static int CompareHands(string handA, string handB)
    {
      int handAStrength = GetHandStrength(handA);
      int handBStrength = GetHandStrength(handB);

      if (handAStrength == handBStrength)
      {
        for (int i = 0; i < handA.Length && i < handB.Length; ++i)
        {
          int valueA = GetCardValue(handA[i]);
          int valueB = GetCardValue(handB[i]);
          if (valueA != valueB)
            return valueA - valueB;
        }
      }

      return handAStrength - handBStrength;
    }

    public static int GetHandStrength(string hand)
    {
      string uniqueCards = string.Empty;
      int[] cardCounts = new int[15];
      int biggestCardCount = 0;

      for (int i = 0; i < hand.Length; ++i)
      {
        char card = hand[i];
        int cardValue = GetCardValue(card);

        if (kJokersWild && card == 'J')
        {
          for (int j = 0; j < cardCounts.Length; ++j)
          {
            cardCounts[j] += 1;
            biggestCardCount = Math.Max(cardCounts[j], biggestCardCount);
          }
        }
        else
        {
          if (!uniqueCards.Contains(card))
            uniqueCards += card;

          cardCounts[cardValue] += 1;

          biggestCardCount = Math.Max(cardCounts[cardValue], biggestCardCount);
        }
      }

      // Five of a kind
      if (uniqueCards.Length == 1 || hand == "JJJJJ")
        return 7;

      // Four of a kind 
      if (biggestCardCount == 4)
        return 6;

      // Full House
      if (biggestCardCount == 3 && uniqueCards.Length == 2)
        return 5;

      // Three of a kind 
      if (biggestCardCount == 3 && uniqueCards.Length == 3)
        return 4;

      // Two pair
      if (biggestCardCount == 2 && uniqueCards.Length == 3)
        return 3;

      // One pair 
      if (biggestCardCount == 2 && uniqueCards.Length == 4)
        return 2;

      // High card
      return 1;
    }
  }
}