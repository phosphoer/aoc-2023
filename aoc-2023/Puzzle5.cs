namespace Puzzles
{
  public static class Puzzle5
  {
    const bool kOutputSeedInfo = false;
    const bool kOutputMapsInfo = false;
    const bool kRunSingleSeedInputs = false;
    const bool kRunSeedRanges = true;

    public static void Run()
    {
      System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
      stopwatch.Start();

      // Parse seed numbers
      string[] inputLines = Puzzle5Input.Input.Split(Environment.NewLine);
      string[] seedStrings = inputLines[0].Split(' ');
      List<double> seedInputs = new();
      for (int i = 1; i < seedStrings.Length; ++i)
        seedInputs.Add(double.Parse(seedStrings[i]));

      List<SeedRange> seedRanges = new();
      for (int i = 1; i + 1 < seedStrings.Length; i += 2)
      {
        seedRanges.Add(new SeedRange()
        {
          Min = double.Parse(seedStrings[i]),
          Length = double.Parse(seedStrings[i + 1])
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
            DestinationStart = double.Parse(valueStrings[0]),
            SourceStart = double.Parse(valueStrings[1]),
            Length = double.Parse(valueStrings[2]),
          };

          currentMap.MapRanges.Add(mapRange);
        }
      }

      // Debug output
      if (kOutputSeedInfo)
      {
        Console.Write($"Seeds: ");
        foreach (double seed in seedInputs)
          Console.Write($"{seed} ");
        Console.Write(Environment.NewLine);

        Console.WriteLine($"Seed Ranges: ");
        double totalSeedCount = 0;
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
        double minLocationNumber = double.MaxValue;
        List<double> mappedSeedValues = new();
        foreach (double seedValue in seedInputs)
        {
          double mappedValue = seedValue;
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
        double currentRange = 0;
        double minLocationNumber = double.MaxValue;
        double totalSeedsSkipped = 0;
        foreach (SeedRange seedRange in seedRanges)
        {
          Console.WriteLine($"{currentRange + 1}/{seedRanges.Count}: {seedRange}");
          minLocationNumber = Math.Min(minLocationNumber, CalculateSeedRangeFast(seedRange, seedMaps, out double skipCount));
          totalSeedsSkipped += skipCount;
          currentRange += 1;
        }

        stopwatch.Stop();

        Console.WriteLine($"Minimum location number: {minLocationNumber}");
        Console.WriteLine($"Total seeds skipped: {totalSeedsSkipped}");
        Console.WriteLine($"Total duration: {stopwatch.ElapsedMilliseconds}ms");
      }
    }

    private static double CalculateSeedRangeFast(SeedRange seedRange, IReadOnlyList<SeedMap> seedMaps, out double totalSkipCount)
    {
      totalSkipCount = 0;
      double minValue = double.MaxValue;
      for (double seedValue = seedRange.Min; seedValue < seedRange.Max; ++seedValue)
      {
        double mappedValue = seedValue;

        // Console.Write($"Seed ");

        double seedSkipAmount = double.MaxValue;
        foreach (var seedMap in seedMaps)
        {
          // Console.Write($"{mappedValue} -> ");
          mappedValue = seedMap.GetMappedValue(mappedValue, out double skipAmount);

          if (skipAmount < seedSkipAmount)
            seedSkipAmount = skipAmount;

          // if (skipAmount > 0)
          //   Console.Write($"(+{skipAmount}) ");
        }

        // Console.Write($"{mappedValue}");
        // Console.Write(Environment.NewLine);

        if (seedSkipAmount > 0)
        {
          // Console.WriteLine($"Skipping {seedSkipAmount} seeds");
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
      public double Max => Min + Length - 1;
      public double Min;
      public double Length;

      public override string ToString()
      {
        return $"({Min} to {Max})";
      }
    }

    private struct MapRange
    {
      public double SourceEnd => SourceStart + Length - 1;
      public double DestinationEnd => DestinationStart + Length - 1;

      public double SourceStart;
      public double DestinationStart;
      public double Length;

      public override string ToString()
      {
        return $"Source: {SourceStart}, Destination: {DestinationStart}, Length: {Length}";
      }

      public bool IsSourceInRange(double sourceValue)
      {
        return sourceValue >= SourceStart && sourceValue < SourceStart + Length;
      }

      public double GetMappedValue(double sourceValue)
      {
        return (sourceValue - SourceStart) + DestinationStart;
      }
    }

    private class SeedMap
    {
      public string Name = string.Empty;
      public List<MapRange> MapRanges = new();

      public double GetMappedValue(double sourceValue)
      {
        for (int i = 0; i < MapRanges.Count; ++i)
        {
          MapRange range = MapRanges[i];
          if (range.IsSourceInRange(sourceValue))
            return range.GetMappedValue(sourceValue);
        }

        return sourceValue;
      }

      public double GetMappedValue(double sourceValue, out double skipAmount)
      {
        skipAmount = double.MaxValue;
        double smallestSourceStart = double.MaxValue;
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
            double diff = range.SourceStart - sourceValue;
            skipAmount = diff;
          }
        }

        return sourceValue;
      }
    }
  }
}