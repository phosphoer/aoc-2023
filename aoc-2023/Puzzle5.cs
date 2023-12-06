namespace Puzzles
{
  public static class Puzzle5
  {
    public static void Run()
    {
      // Parse seed numbers
      string[] inputLines = Puzzle5Input.Input.Split(Environment.NewLine);
      string[] seedStrings = inputLines[0].Split(' ');
      List<uint> seedInputs = new();
      for (int i = 1; i < seedStrings.Length; ++i)
        seedInputs.Add(uint.Parse(seedStrings[i]));

      List<SeedRange> seedRanges = new();
      for (int i = 1; i + 1 < seedStrings.Length; i += 2)
      {
        seedRanges.Add(new SeedRange()
        {
          Min = uint.Parse(seedStrings[i]),
          Length = uint.Parse(seedStrings[i + 1])
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
            DestinationStart = uint.Parse(valueStrings[0]),
            SourceStart = uint.Parse(valueStrings[1]),
            Length = uint.Parse(valueStrings[2]),
          };

          currentMap.MapRanges.Add(mapRange);
        }
      }

      // Debug output
      Console.Write($"Seeds: ");
      foreach (uint seed in seedInputs)
        Console.Write($"{seed} ");
      Console.Write(Environment.NewLine);

      Console.WriteLine($"Seed Ranges: ");
      uint totalSeedCount = 0;
      foreach (var seedRange in seedRanges)
      {
        totalSeedCount += seedRange.Length;
        Console.WriteLine($"  {seedRange}");
      }

      Console.WriteLine($"Seed total: {totalSeedCount}");

      foreach (var seedMap in seedMaps)
      {
        Console.Write(Environment.NewLine);
        Console.WriteLine(seedMap.Name);
        foreach (var mapRange in seedMap.MapRanges)
          Console.WriteLine(mapRange);
      }

      uint minLocationNumber = uint.MaxValue;
      List<uint> mappedSeedValues = new();
      foreach (uint seedValue in seedInputs)
      {
        uint mappedValue = seedValue;
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

    private struct SeedRange
    {
      public uint Max => Min + Length - 1;
      public uint Min;
      public uint Length;

      public override string ToString()
      {
        return $"({Min} to {Max})";
      }
    }

    private struct MapRange
    {
      public uint SourceStart;
      public uint DestinationStart;
      public uint Length;

      public override string ToString()
      {
        return $"Source: {SourceStart}, Destination: {DestinationStart}, Length: {Length}";
      }

      public bool IsSourceInRange(uint sourceValue)
      {
        return sourceValue > SourceStart && sourceValue < SourceStart + Length;
      }

      public uint GetMappedValue(uint sourceValue)
      {
        return (sourceValue - SourceStart) + DestinationStart;
      }
    }

    private class SeedMap
    {
      public string Name = string.Empty;
      public List<MapRange> MapRanges = new();

      public uint GetMappedValue(uint sourceValue)
      {
        for (int i = 0; i < MapRanges.Count; ++i)
        {
          MapRange range = MapRanges[i];
          if (range.IsSourceInRange(sourceValue))
            return range.GetMappedValue(sourceValue);
        }

        return sourceValue;
      }
    }
  }
}