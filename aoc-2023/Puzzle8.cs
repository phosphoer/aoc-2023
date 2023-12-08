namespace Puzzles
{
  public class Puzzle8
  {
    public static void Run()
    {
      Graph graph = new Graph(Puzzle8Input.Test3);
      graph.Print();

      int count = graph.RunInstructions2(outputDebug: true);
      Console.WriteLine($"Instructions to reach ZZZ: {count}");
    }

    public class Graph
    {
      public List<Node> Nodes = new();
      public Dictionary<string, Node> NodeMap = new();
      public string Instructions = string.Empty;

      public Graph(string graphString)
      {
        List<string> lines = new(graphString.Split(Environment.NewLine));
        Instructions = lines[0];
        lines.RemoveAt(0);

        Node currentNode = null;
        foreach (string line in lines)
        {
          string parseBuffer = string.Empty;
          for (int i = 0; i < line.Length; ++i)
          {
            char c = line[i];
            if (char.IsLetterOrDigit(c))
              parseBuffer += c;

            if (parseBuffer.Length == 3)
            {
              if (currentNode == null)
              {
                currentNode = new Node();
                currentNode.Name = parseBuffer;
              }
              else if (string.IsNullOrEmpty(currentNode.Left))
                currentNode.Left = parseBuffer;
              else if (string.IsNullOrEmpty(currentNode.Right))
                currentNode.Right = parseBuffer;

              parseBuffer = string.Empty;
            }
          }

          if (currentNode != null)
          {
            Nodes.Add(currentNode);
            currentNode = null;
          }
        }

        foreach (var node in Nodes)
        {
          NodeMap[node.Name] = node;
        }

        foreach (var node in Nodes)
        {
          node.LeftNode = NodeMap[node.Left];
          node.RightNode = NodeMap[node.Right];
        }
      }

      public int RunInstructions(bool outputDebug)
      {
        int count = 0;
        bool isDone = false;
        Node currentNode = NodeMap["AAA"];
        Node endNode = NodeMap["ZZZ"];
        while (!isDone)
        {
          for (int i = 0; i < Instructions.Length; ++i)
          {
            if (outputDebug)
            {
              if (i == 0)
                Console.Write($"{currentNode.Name}");
              else
                Console.Write($"->{currentNode.Name}");
            }

            char dir = Instructions[i];
            if (dir == 'L')
              currentNode = currentNode.LeftNode;
            else if (dir == 'R')
              currentNode = currentNode.RightNode;

            count += 1;
            if (currentNode == endNode)
            {
              isDone = true;
              break;
            }
          }
        }

        if (outputDebug)
        {
          Console.WriteLine();
        }

        return count;
      }

      public int RunInstructions2(bool outputDebug)
      {
        int count = 0;
        bool isDone = false;
        List<Node> currentNodes = new();

        foreach (var node in Nodes)
          if (node.Name.EndsWith('A'))
            currentNodes.Add(node);

        if (outputDebug)
        {
          Console.Write($"Starting nodes: ");
          foreach (var node in currentNodes)
            Console.Write($"{node.Name} ");

          Console.WriteLine();
        }

        while (!isDone)
        {
          foreach (char dir in Instructions)
          {
            if (outputDebug)
            {
              if (count % 10000 == 0)
                Console.Write('.');

              if (count % 1000000 == 0)
                Console.WriteLine();
            }

            if (dir == 'L')
            {
              for (int i = 0; i < currentNodes.Count; ++i)
                currentNodes[i] = currentNodes[i].LeftNode;
            }
            else if (dir == 'R')
            {
              for (int i = 0; i < currentNodes.Count; ++i)
                currentNodes[i] = currentNodes[i].RightNode;
            }

            count += 1;

            bool allDone = true;
            foreach (var node in currentNodes)
              allDone &= node.IsEndNode;

            if (allDone)
            {
              isDone = true;
              break;
            }
          }
        }

        if (outputDebug)
        {
          Console.WriteLine();
        }

        return count;
      }

      public void Print()
      {
        Console.WriteLine(Instructions);
        foreach (var node in Nodes)
        {
          Console.WriteLine(node);
        }
      }
    }

    public class Node
    {
      public bool IsEndNode => Name.EndsWith('Z');

      public string Name = string.Empty;
      public string Left = string.Empty;
      public string Right = string.Empty;
      public Node LeftNode = null;
      public Node RightNode = null;

      public override string ToString()
      {
        return $"{Name} = ({Left}, {Right})";
      }
    }
  }
}