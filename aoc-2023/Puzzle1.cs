namespace Puzzles
{
  public static class Puzzle1
  {
    public static void Run()
    {
      Console.WriteLine("Puzzle 1...");

      int total = 0;
      string[] inputLines = Puzzle1Input.Input.Split(Environment.NewLine);
      foreach (var line in inputLines)
      {
        int calibrationValue = GetCalibrationValue(line, includeLetters: true);
        total += calibrationValue;
        Console.WriteLine($"{line}: {calibrationValue}");
      }

      Console.WriteLine($"Total: {total}");
    }

    public static bool TryGetDigit(string line, int startIndex, out int digitValue, out int endIndex)
    {
      string currentToken = string.Empty;
      for (int i = startIndex; i < line.Length; ++i)
      {
        currentToken += line[i];

        bool couldBeValid = false;
        for (int j = 0; j < Puzzle1Input.DigitNames.Length; ++j)
        {
          string digitName = Puzzle1Input.DigitNames[j];
          if (digitName == currentToken)
          {
            digitValue = j + 1;
            endIndex = i;
            return true;
          }
          else if (digitName.StartsWith(currentToken))
          {
            couldBeValid = true;
          }
        }

        if (!couldBeValid)
          break;
      }

      digitValue = -1;
      endIndex = -1;
      return false;
    }

    private static int GetCalibrationValue(string forLine, bool includeLetters = false)
    {
      int firstValue = -1;
      int lastValue = -1;
      for (int i = 0; i < forLine.Length; ++i)
      {
        if (includeLetters)
        {
          int stringDigit, endIndex;
          bool gotDigit = TryGetDigit(forLine, i, out stringDigit, out endIndex);
          if (gotDigit)
          {
            lastValue = stringDigit;
            if (firstValue == -1)
              firstValue = stringDigit;

            i = endIndex;
          }
        }

        char strChar = forLine[i];
        if (char.IsAsciiDigit(strChar))
        {
          int charValue = (int)char.GetNumericValue(strChar);
          lastValue = charValue;
          if (firstValue == -1)
            firstValue = charValue;
        }
      }

      int totalValue = firstValue * 10 + lastValue;
      return totalValue;
    }
  }
}