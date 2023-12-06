namespace Puzzles
{
  public static class Puzzle5
  {
    const bool kOutputSeedInfo = false;
    const bool kOutputMapsInfo = false;
    const bool kOutputFilteringInfo = true;
    const bool kRunSingleSeedInputs = false;
    const bool kRunSeedRanges = true;

    public static void Run()
    {
      System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
      stopwatch.Start();

      // Parse seed numbers
      string[] inputLines = Puzzle5Input.Test1.Split(Environment.NewLine);
      string[] seedStrings = inputLines[0].Split(' ');
      List<long> seedInputs = new();
      for (int i = 1; i < seedStrings.Length; ++i)
        seedInputs.Add(long.Parse(seedStrings[i]));

      List<SeedRange> seedRanges = new();
      for (int i = 1; i + 1 < seedStrings.Length; i += 2)
      {
        seedRanges.Add(new SeedRange()
        {
          Min = long.Parse(seedStrings[i]),
          Length = long.Parse(seedStrings[i + 1])
        });
      }

      // Parse seed maps
      List<SeedMap> seedMaps = new();
      SeedMap currentMap = null;
      for (int i = 1; i < inputLines.Length; ++i)
      {
        string line = inputLines[i];
        if (line.Contains("map:"))
        {
          currentMap = new SeedMap();
          currentMap.Name = line;
          seedMaps.Add(currentMap);
        }
        else if (currentMap != null && line.Length > 0 && char.IsDigit(line[0]))
        {
          string[] valueStrings = line.Split(' ');
          MapRange mapRange = new()
          {
            DestinationStart = long.Parse(valueStrings[0]),
            SourceStart = long.Parse(valueStrings[1]),
            Length = long.Parse(valueStrings[2]),
          };

          currentMap.MapRanges.Add(mapRange);
        }
      }

      // Debug output
      if (kOutputSeedInfo)
      {
        Console.Write($"Seeds: ");
        foreach (long seed in seedInputs)
          Console.Write($"{seed} ");
        Console.Write(Environment.NewLine);

        Console.WriteLine($"Seed Ranges: ");
        long totalSeedCount = 0;
        foreach (var seedRange in seedRanges)
        {
          totalSeedCount += seedRange.Length;
          Console.WriteLine($"  {seedRange}");
        }

        Console.WriteLine($"Seed total: {totalSeedCount}");
      }

      if (kOutputMapsInfo)
      {
        foreach (var seedMap in seedMaps)
        {
          Console.Write(Environment.NewLine);
          Console.WriteLine(seedMap.Name);

          foreach (var mapRange in seedMap.MapRanges)
            Console.WriteLine(mapRange);
        }
      }

      // Day 1 solution
      if (kRunSingleSeedInputs)
      {
        long minLocationNumber = long.MaxValue;
        List<long> mappedSeedValues = new();
        foreach (long seedValue in seedInputs)
        {
          long mappedValue = seedValue;
          Console.WriteLine($"Seed {seedValue} maps to:");

          foreach (var seedMap in seedMaps)
          {
            mappedValue = seedMap.GetMappedValue(mappedValue);
            Console.WriteLine($"  {seedMap.Name} {mappedValue}");
          }

          if (mappedValue < minLocationNumber)
            minLocationNumber = mappedValue;

          mappedSeedValues.Add(mappedValue);
        }

        Console.WriteLine($"Minimum location number: {minLocationNumber}");
      }

      // Day 2 solution
      if (kRunSeedRanges)
      {
        long currentRange = 0;
        long minLocationNumber = long.MaxValue;
        long totalSeedsSkipped = 0;
        foreach (SeedRange seedRange in seedRanges)
        {
          Console.WriteLine($"{currentRange + 1}/{seedRanges.Count}: {seedRange}");
          minLocationNumber = Math.Min(minLocationNumber, CalculateSeedRangeFast(seedRange, seedMaps, out long skipCount));
          totalSeedsSkipped += skipCount;
          currentRange += 1;
        }

        stopwatch.Stop();

        Console.WriteLine($"Minimum location number: {minLocationNumber}");
        Console.WriteLine($"Total seeds skipped: {totalSeedsSkipped}");
        Console.WriteLine($"Total duration: {stopwatch.ElapsedMilliseconds}ms");
      }
    }

    private static long CalculateSeedRangeFast(SeedRange seedRange, IReadOnlyList<SeedMap> seedMaps, out long totalSkipCount)
    {
      totalSkipCount = 0;
      long minValue = long.MaxValue;
      for (long seedValue = seedRange.Min; seedValue < seedRange.Max; ++seedValue)
      {
        long mappedValue = seedValue;

        if (kOutputFilteringInfo)
          Console.Write($"Seed ");

        long seedSkipAmount = long.MaxValue;
        foreach (var seedMap in seedMaps)
        {
          if (kOutputFilteringInfo)
            Console.Write($"{mappedValue} -> ");

          mappedValue = seedMap.GetMappedValue(mappedValue, out long skipAmount);

          if (skipAmount < seedSkipAmount)
            seedSkipAmount = skipAmount;

          if (kOutputFilteringInfo && skipAmount > 0)
            Console.Write($"(+{skipAmount}) ");
        }

        if (kOutputFilteringInfo)
        {
          Console.Write($"{mappedValue}");
          Console.Write(Environment.NewLine);
        }

        if (seedSkipAmount > 0)
        {
          if (kOutputFilteringInfo)
            Console.WriteLine($"Skipping {seedSkipAmount} seeds");

          seedValue += seedSkipAmount - 1;
          totalSkipCount += seedSkipAmount;
        }

        if (mappedValue < minValue)
          minValue = mappedValue;
      }

      return minValue;
    }

    private struct SeedRange
    {
      public long Max => Min + Length - 1;
      public long Min;
      public long Length;

      public override string ToString()
      {
        return $"({Min} to {Max})";
      }
    }

    private struct MapRange
    {
      public long SourceEnd => SourceStart + Length - 1;
      public long DestinationEnd => DestinationStart + Length - 1;

      public long SourceStart;
      public long DestinationStart;
      public long Length;

      public override string ToString()
      {
        return $"Source: {SourceStart}, Destination: {DestinationStart}, Length: {Length}";
      }

      public bool IsSourceInRange(long sourceValue)
      {
        return sourceValue >= SourceStart && sourceValue < SourceStart + Length;
      }

      public long GetMappedValue(long sourceValue)
      {
        return (sourceValue - SourceStart) + DestinationStart;
      }
    }

    private class SeedMap
    {
      public string Name = string.Empty;
      public List<MapRange> MapRanges = new();

      public long GetMappedValue(long sourceValue)
      {
        for (int i = 0; i < MapRanges.Count; ++i)
        {
          MapRange range = MapRanges[i];
          if (range.IsSourceInRange(sourceValue))
            return range.GetMappedValue(sourceValue);
        }

        return sourceValue;
      }

      public long GetMappedValue(long sourceValue, out long skipAmount)
      {
        skipAmount = long.MaxValue;
        long smallestSourceStart = long.MaxValue;
        for (int i = 0; i < MapRanges.Count; ++i)
        {
          MapRange range = MapRanges[i];
          if (range.IsSourceInRange(sourceValue))
          {
            skipAmount = range.SourceEnd - sourceValue;
            return range.GetMappedValue(sourceValue);
          }
          else if (range.SourceStart > sourceValue && range.SourceStart < smallestSourceStart)
          {
            smallestSourceStart = range.SourceStart;
            long diff = range.SourceStart - sourceValue;
            skipAmount = diff;
          }
        }

        return sourceValue;
      }
    }
  }
}