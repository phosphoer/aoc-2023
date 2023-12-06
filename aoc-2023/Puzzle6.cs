namespace Puzzles
{
  public static class Puzzle6
  {
    const bool kEnablePart2 = true;

    public static void Run()
    {
      string[] inputStrings = Puzzle6Input.Input.Split(Environment.NewLine);
      string raceTimesStr = inputStrings[0];
      string raceDistancesStr = inputStrings[1];

      List<long> timeValues = kEnablePart2 ? ParseValuesJoined(raceTimesStr) : ParseValues(raceTimesStr);
      List<long> distanceValues = kEnablePart2 ? ParseValuesJoined(raceDistancesStr) : ParseValues(raceDistancesStr);

      if (timeValues.Count != distanceValues.Count)
        Console.WriteLine("Error: Mismatching time and distance value counts");

      List<RaceInfo> races = new();
      for (int i = 0; i < timeValues.Count; ++i)
      {
        races.Add(new RaceInfo()
        {
          Time = timeValues[i],
          Distance = distanceValues[i]
        });
      }

      long productTotal = 1;
      foreach (var race in races)
      {
        Console.WriteLine($"Race: {race}");

        long minChargeTime = FindValidTimeExtent(race, -1);
        long maxChargeTime = FindValidTimeExtent(race, 1);
        long winningCount = (maxChargeTime - minChargeTime) + 1;
        productTotal *= winningCount;
        Console.WriteLine($"{winningCount} winning times from {minChargeTime} to {maxChargeTime}");
      }

      Console.WriteLine($"Winning option product total: {productTotal}");
    }

    private static long FindValidTimeExtent(RaceInfo race, long direction)
    {
      long bestChargeTime = race.Time / 2;
      long minChargeTime = bestChargeTime;
      for (long i = 0; i < bestChargeTime; ++i)
      {
        long testChargeTime = bestChargeTime + i * direction;
        long testDistance = CalculateDistance(testChargeTime, race.Time);
        if (testDistance > race.Distance)
          minChargeTime = testChargeTime;
      }

      return minChargeTime;
    }

    private static long CalculateDistance(long chargeTime, long totalTime)
    {
      long remainingTime = Math.Max(0, totalTime - chargeTime);
      return remainingTime * chargeTime;
    }

    private static List<long> ParseValues(string line)
    {
      List<long> values = new();
      string parseString = string.Empty;
      for (int i = 0; i < line.Length; ++i)
      {
        char c = line[i];
        if (char.IsDigit(c))
          parseString += line[i];

        if (!char.IsDigit(c) || i >= line.Length - 1)
        {
          if (parseString.Length > 0)
            values.Add(long.Parse(parseString));

          parseString = string.Empty;
        }
      }

      return values;
    }

    private static List<long> ParseValuesJoined(string line)
    {
      List<long> values = new();
      string parseString = string.Empty;
      for (int i = 0; i < line.Length; ++i)
      {
        char c = line[i];
        if (char.IsDigit(c))
          parseString += line[i];
      }

      values.Add(long.Parse(parseString));
      return values;
    }

    private struct RaceInfo
    {
      public long Time;
      public long Distance;

      public override string ToString()
      {
        return $"Time: {Time}, Distance: {Distance}";
      }
    }
  }
}