namespace Puzzles
{
  public static class Puzzle3
  {
    public struct Coord
    {
      public int X;
      public int Y;
    }

    public struct PartNumber
    {
      public Coord Position;
      public int Value;
      public int Width;
    }

    public class Schematic
    {
      public List<PartNumber> PartNumbers = new();
      public List<char> DetectedSymbols = new();
      public List<char> RejectedSymbols = new();
      public char[] Data = { };
      public int Width = 0;
      public int Height = 0;

      public Schematic(string schematicString)
      {
        Data = schematicString.Replace(Environment.NewLine, string.Empty).ToCharArray();
        Width = schematicString.IndexOf(Environment.NewLine);
        Height = Data.Length / Width;

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
              }
              else if (parsedString.Length > 0 && !isNumber)
              {
                Console.WriteLine($"Failed to parse number: {parsedString}");
              }

              if (IsSymbol(character) && !DetectedSymbols.Contains(character))
                DetectedSymbols.Add(character);
              else if (!IsSymbol(character) && !RejectedSymbols.Contains(character))
                RejectedSymbols.Add(character);

              parsedString = string.Empty;
            }
          }
        }
      }

      public static bool IsSymbol(char character)
      {
        return character != '.' && !char.IsDigit(character);
      }

      public char GetCharacter(int x, int y)
      {
        if (x < 0 || y < 0 || x >= Width || y >= Width)
          return '.';

        return Data[x + y * Width];
      }
    }

    public static void Run()
    {
      Console.WriteLine("Puzzle 3...");

      Schematic schematic = new Schematic(Puzzle3Input.Input);

      Console.WriteLine($"Schematic dimensions: {schematic.Width}x{schematic.Height}");

      Console.Write("Detected symbols: ");
      foreach (var symbol in schematic.DetectedSymbols)
        Console.Write(symbol);
      Console.Write(Environment.NewLine);

      Console.Write("Rejected symbols: ");
      foreach (var symbol in schematic.RejectedSymbols)
        Console.Write(symbol);
      Console.Write(Environment.NewLine);

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
        Console.WriteLine($" {validStr}Part number {partNumber.Value} with width {partNumber.Width} at coord ({partNumber.Position.X}, {partNumber.Position.Y})");
      }

      Console.WriteLine($"Valid part number total: {partNumberSum}");
    }

    public static char GetCharacter(Schematic schematic, int x, int y)
    {
      return schematic.Data[x + y * schematic.Width];
    }
  }
}