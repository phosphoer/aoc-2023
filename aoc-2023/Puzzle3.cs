namespace Puzzles
{
  public static class Puzzle3
  {
    public static void Run()
    {
      Console.WriteLine("Puzzle 3...");

      Schematic schematic = new Schematic(Puzzle3Input.Test2);

      Console.WriteLine($"Schematic dimensions: {schematic.Width}x{schematic.Height}");

      Console.Write("Detected symbols: ");
      foreach (var symbol in schematic.DetectedSymbols)
        Console.Write(symbol);
      Console.Write(Environment.NewLine);

      Console.Write("Rejected symbols: ");
      foreach (var symbol in schematic.RejectedSymbols)
        Console.Write(symbol);
      Console.Write(Environment.NewLine);

      Console.WriteLine($"Index Map");
      for (int y = 0; y < schematic.Height; ++y)
      {
        for (int x = 0; x < schematic.Width; ++x)
        {
          int index = schematic.PartIndexMap[x + y * schematic.Width];
          if (index >= 0)
            Console.Write(index);
          else
            Console.Write(".");
        }

        Console.Write(Environment.NewLine);
      }

      int partNumberSum = 0;
      foreach (var partNumber in schematic.PartNumbers)
      {
        // Check around border of number for symbols
        bool isValid = false;
        for (int y = -1; !isValid && y < 2; ++y)
        {
          for (int x = -1; !isValid && x < partNumber.Width + 1; ++x)
          {
            char val = schematic.GetCharacter(partNumber.Position.X + x, partNumber.Position.Y + y);
            // Console.WriteLine($"{val}: {Schematic.IsSymbol(val)}");
            // Console.Write(val);
            if (Schematic.IsSymbol(val))
            {
              partNumberSum += partNumber.Value;
              isValid = true;
            }
          }
        }

        string validStr = isValid ? "*" : " ";
        Console.WriteLine($" {validStr}Part number {partNumber.Value} with width {partNumber.Width} at coord {partNumber.Position}");
      }

      int totalRatio = 0;
      foreach (var gear in schematic.Gears)
      {
        PartNumber gearPartA = schematic.PartNumbers[gear.PartIndexA];
        PartNumber gearPartB = schematic.PartNumbers[gear.PartIndexB];
        int ratio = schematic.GetGearRatio(gear);
        totalRatio += ratio;
        Console.WriteLine($"Gear between {gearPartA.Value} and {gearPartB.Value} with ratio {ratio} at coord {gear.Position}");
      }

      Console.WriteLine($"Valid part number total: {partNumberSum}");
      Console.WriteLine($"Gear ratio total: {totalRatio}");
    }

    public static char GetCharacter(Schematic schematic, int x, int y)
    {
      return schematic.Data[x + y * schematic.Width];
    }
  }

  public struct Coord
  {
    public int X;
    public int Y;

    public override string ToString()
    {
      return $"({X}, {Y})";
    }
  }

  public struct PartNumber
  {
    public Coord Position;
    public int Value;
    public int Width;
  }

  public struct Gear
  {
    public Coord Position;
    public int PartIndexA;
    public int PartIndexB;
  }

  public class Schematic
  {
    public List<PartNumber> PartNumbers = new();
    public List<Gear> Gears = new();
    public List<char> DetectedSymbols = new();
    public List<char> RejectedSymbols = new();
    public char[] Data = { };
    public int[] PartIndexMap = { };
    public int Width = 0;
    public int Height = 0;

    public Schematic(string schematicString)
    {
      Data = schematicString.Replace(Environment.NewLine, string.Empty).ToCharArray();
      Width = schematicString.IndexOf(Environment.NewLine);
      Height = Data.Length / Width;

      PartIndexMap = new int[Width * Height];
      for (int i = 0; i < PartIndexMap.Length; ++i)
        PartIndexMap[i] = -1;

      // Detect parts and symbols
      List<Gear> potentialGears = new();
      for (int y = 0; y < Height; ++y)
      {
        string parsedString = string.Empty;
        for (int x = 0; x < Width; ++x)
        {
          char character = GetCharacter(x, y);
          if (char.IsDigit(character))
          {
            parsedString += character;
          }

          // End of number or line
          if (!char.IsDigit(character) || x == Width - 1)
          {
            bool isNumber = int.TryParse(parsedString, out int partNumberVal);
            if (parsedString.Length > 0 && isNumber)
            {
              PartNumber partNumber = new();
              partNumber.Width = (int)Math.Log10(partNumberVal) + 1;
              partNumber.Position = new Coord() { X = x - partNumber.Width, Y = y };
              partNumber.Value = partNumberVal;
              PartNumbers.Add(partNumber);

              int startIndex = x + y * Width;
              for (int i = startIndex - 1; i >= startIndex - partNumber.Width; --i)
                PartIndexMap[i] = PartNumbers.Count - 1;
            }
            else if (parsedString.Length > 0 && !isNumber)
            {
              Console.WriteLine($"Failed to parse number: {parsedString}");
            }

            if (IsSymbol(character) && !DetectedSymbols.Contains(character))
              DetectedSymbols.Add(character);
            else if (!IsSymbol(character) && !RejectedSymbols.Contains(character))
              RejectedSymbols.Add(character);

            if (character == '*')
              potentialGears.Add(new Gear() { Position = new Coord() { X = x, Y = y } });

            parsedString = string.Empty;
          }
        }
      }

      // Find gears
      List<int> adjacentParts = new();
      for (int i = 0; i < potentialGears.Count; ++i)
      {
        Gear gear = potentialGears[i];
        // Console.WriteLine($"Potential Gear at {gear.Position}");

        adjacentParts.Clear();
        for (int y = -1; y <= 1; ++y)
        {
          for (int x = -1; x <= 1; ++x)
          {
            int partIndex = GetPartNumberIndex(gear.Position.X + x, gear.Position.Y + y);
            if (partIndex >= 0 && !adjacentParts.Contains(partIndex))
            {
              adjacentParts.Add(partIndex);
              // Console.WriteLine($"Adjacent part at {gear.Position.X + x}, {gear.Position.Y + y}");
            }
          }
        }

        if (adjacentParts.Count == 2)
        {
          gear.PartIndexA = adjacentParts[0];
          gear.PartIndexB = adjacentParts[1];
          Gears.Add(gear);
        }
      }
    }

    public static bool IsSymbol(char character)
    {
      return character != '.' && !char.IsDigit(character);
    }

    public char GetCharacter(int x, int y)
    {
      if (x < 0 || y < 0 || x >= Width || y >= Height)
        return '.';

      return Data[x + y * Width];
    }

    public int GetGearRatio(Gear gear)
    {
      PartNumber partA = PartNumbers[gear.PartIndexA];
      PartNumber partB = PartNumbers[gear.PartIndexB];
      return partA.Value * partB.Value;
    }

    public int GetPartNumberIndex(int x, int y)
    {
      if (x < 0 || y < 0 || x >= Width || y >= Height)
        return -1;

      return PartIndexMap[x + y * Width];
    }

    public bool TryGetPartNumber(int x, int y, out PartNumber partNumber)
    {
      int index = GetPartNumberIndex(x, y);
      if (index >= 0)
      {
        partNumber = PartNumbers[index];
        return true;
      }

      partNumber = default;
      return false;
    }
  }
}