
InteractiveTestGraphSet();

void InteractiveTestGraphSet()
{
    Vector2GraphSet <char> graphSet = new Vector2GraphSet<char>(10, 10);
    string[]? tokens;
    string command;
    Console.Clear();
    do
    {
        Console.WriteLine(graphSet);
        command = "";
        string? input = Console.ReadLine();
        Console.Clear();
        tokens = input?.Split(new char[] {' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (tokens == null) continue;
        if (tokens.Length == 0) continue;

        command = tokens[0];
        switch(command) {
            case "add":
                graphSet.AddValue(tokens[1][0], new UnityEngine.Vector2Int(Int32.Parse(tokens[2]), Int32.Parse(tokens[3])));                
                break;
            case "rm":
                graphSet.RemoveValue(new UnityEngine.Vector2Int(Int32.Parse(tokens[1]), Int32.Parse(tokens[2])));
                break;
            case "fill":
                int x1 = Int32.Parse(tokens[1]);
                int y1 = Int32.Parse(tokens[2]);
                int x2 = Int32.Parse(tokens[3]);
                int y2 = Int32.Parse(tokens[4]);
                for (int x = x1; x <= x2; x++)
                {
                    for(int y = y1; y <= y2; y++)
                    {
                        graphSet.AddPosition(new UnityEngine.Vector2Int(x, y));
                    }
                }
                break;
            default:
                break;
        }
    } 
    while (command != "quit") ;
}



void TestGraphSetAdd()
{
    /*
    Vector2GraphSet graphSet = new Vector2GraphSet(3, 3);
    Console.WriteLine(graphSet);
    graphSet = new Vector2GraphSet(0, 0);
    Console.WriteLine(graphSet);
    graphSet = new Vector2GraphSet(5, 1);
    Console.WriteLine(graphSet);
    */
    Vector2GraphSet<char> graphSet = new Vector2GraphSet<char>(7, 10);    
    graphSet.AddPosition(new UnityEngine.Vector2Int(3, 3));        
    graphSet.AddPosition(new UnityEngine.Vector2Int(5, 3));
    graphSet.AddPosition(new UnityEngine.Vector2Int(6, 3));        
    graphSet.AddPosition(new UnityEngine.Vector2Int(2, 3));
    graphSet.AddPosition(new UnityEngine.Vector2Int(1, 3));
    graphSet.AddPosition(new UnityEngine.Vector2Int(0, 0));
    graphSet.AddPosition(new UnityEngine.Vector2Int(6, 6));    
    graphSet.AddPosition(new UnityEngine.Vector2Int(0, 1));
    graphSet.AddPosition(new UnityEngine.Vector2Int(0, 2));    
    graphSet.AddPosition(new UnityEngine.Vector2Int(0, 3));
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 2));
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 4));
    Console.WriteLine(graphSet);
    graphSet.AddPosition(new UnityEngine.Vector2Int(6, 5));
    graphSet.AddPosition(new UnityEngine.Vector2Int(6, 4));
    Console.WriteLine(graphSet);
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 6));
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 5));
    Console.WriteLine(graphSet);
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 0));
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 1));
    Console.WriteLine(graphSet);
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 3));
    Console.WriteLine(graphSet);
    graphSet.AddPosition(new UnityEngine.Vector2Int(0, 6));
    Console.WriteLine(graphSet);
    graphSet.RemovePosition(new UnityEngine.Vector2Int(0, 6));
    Console.WriteLine(graphSet);
    graphSet.RemovePosition(new UnityEngine.Vector2Int(4, 6));
    Console.WriteLine(graphSet);
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 7));
    Console.WriteLine(graphSet);
    graphSet.AddPosition(new UnityEngine.Vector2Int(4, 6));
    Console.WriteLine(graphSet);
}

//write as an automated test???
void TestUniqueIDProvider()
{
    UniqueIDProvider uniqueIDProvider = new UniqueIDProvider();

    List<int> IDs = new List<int>();


    for (int i = 0; i < 3; i++)
    {
        GetNewID(uniqueIDProvider, IDs);
    }

    ReturnID(uniqueIDProvider, IDs, 2);
    ReturnID(uniqueIDProvider, IDs, 0);
    ReturnID(uniqueIDProvider, IDs, 1);
    for (int i = 0; i < 5; i++)
    {
        GetNewID(uniqueIDProvider, IDs);
    }

    ReturnID(uniqueIDProvider, IDs, 3);
    GetNewID(uniqueIDProvider, IDs);
    GetNewID(uniqueIDProvider, IDs);
    ReturnID(uniqueIDProvider, IDs, 2);
    ReturnID(uniqueIDProvider, IDs, 1);
    GetNewID(uniqueIDProvider, IDs);
    GetNewID(uniqueIDProvider, IDs);
    GetNewID(uniqueIDProvider, IDs);
}

void GetNewID(UniqueIDProvider uniqueIDProvider, List<int> IDs)
{
    int newID = uniqueIDProvider.GetNewID();
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"getting new ID: {newID}");
    IDs.Add(newID);
    Console.ResetColor();
    printAllIDs(uniqueIDProvider, IDs);
}

void ReturnID(UniqueIDProvider uniqueIDProvider, List<int> IDs, int id)
{
    Console.ForegroundColor= ConsoleColor.Red;
    Console.WriteLine($"returning ID {id}");
    Console.ResetColor();
    uniqueIDProvider.ReturnID(id);
    IDs.Remove(id);
    printAllIDs(uniqueIDProvider, IDs);
}

void printAllIDs(UniqueIDProvider uniqueIDProvider, List<int> IDs)
{
    Console.WriteLine("printing all IDs");
    foreach (int id in IDs)
    {
        Console.WriteLine(id);
    }
    if (IDs.Count == 0) Console.WriteLine("<none>");
    Console.WriteLine("           ");
    Console.WriteLine("Data Structure:");
    Console.WriteLine(uniqueIDProvider);
    Console.WriteLine("-----------");
}