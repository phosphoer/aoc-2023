using System;

namespace Puzzles
{
  public static class Puzzle2
  {
    public struct ColorCount
    {
      public int Power => R * G * B;
      public int R;
      public int G;
      public int B;
    }

    public struct GameInfo
    {
      public int GameId;
      public List<ColorCount> ColorCounts;
      public ColorCount MinColorCount;
    }

    public static ColorCount MaxColorCount = new ColorCount()
    {
      R = 12,
      G = 13,
      B = 14
    };

    public static void Run()
    {
      Console.WriteLine("Puzzle 2...");

      int validIdTotal = 0;
      int minPowerSum = 0;

      string[] allGameStrings = Puzzle2Input.Input.Split(Environment.NewLine);
      foreach (string gameString in allGameStrings)
      {
        GameInfo gameInfo = GetGameInfoFromLine(gameString);
        Console.WriteLine($"Game {gameInfo.GameId}");
        bool isGameValid = true;
        for (int i = 0; i < gameInfo.ColorCounts.Count; ++i)
        {
          var colorCount = gameInfo.ColorCounts[i];
          bool isSetValid = colorCount.R <= MaxColorCount.R
                            && colorCount.G <= MaxColorCount.G
                            && colorCount.B <= MaxColorCount.B;

          gameInfo.MinColorCount.R = Math.Max(colorCount.R, gameInfo.MinColorCount.R);
          gameInfo.MinColorCount.G = Math.Max(colorCount.G, gameInfo.MinColorCount.G);
          gameInfo.MinColorCount.B = Math.Max(colorCount.B, gameInfo.MinColorCount.B);

          isGameValid &= isSetValid;
          string validIndicator = isSetValid ? string.Empty : "!!! ";
          Console.WriteLine($"  {validIndicator}{colorCount.R} R, {colorCount.G} G, {colorCount.B} B");
        }

        Console.WriteLine($"  Min Counts: {gameInfo.MinColorCount.R} R, {gameInfo.MinColorCount.G} G, {gameInfo.MinColorCount.B} B");

        minPowerSum += gameInfo.MinColorCount.Power;
        if (isGameValid)
          validIdTotal += gameInfo.GameId;
      }

      Console.WriteLine($"Valid game ID total: {validIdTotal}");
      Console.WriteLine($"Min Power Sum: {minPowerSum}");
    }

    private static GameInfo GetGameInfoFromLine(string line)
    {
      GameInfo gameInfo = new();
      gameInfo.ColorCounts = new();
      gameInfo.MinColorCount = new()
      {
        R = 0,
        G = 0,
        B = 0
      };

      string[] gameParts = line.Split(": ");
      string gameIdString = gameParts[0].Split(" ")[1];
      gameInfo.GameId = int.Parse(gameIdString);

      string gameInfoString = gameParts[1];
      string[] gameInfoSets = gameInfoString.Split("; ");
      foreach (string set in gameInfoSets)
      {
        string[] colorCounts = set.Split(", ");
        ColorCount countInfo = new ColorCount();
        foreach (string colorCount in colorCounts)
        {
          string[] countPair = colorCount.Split(" ");
          int count = int.Parse(countPair[0]);
          string colorName = countPair[1];
          if (colorName == "red")
            countInfo.R += count;
          else if (colorName == "green")
            countInfo.G += count;
          else if (colorName == "blue")
            countInfo.B += count;
        }

        gameInfo.ColorCounts.Add(countInfo);
      }

      return gameInfo;
    }
  }
}