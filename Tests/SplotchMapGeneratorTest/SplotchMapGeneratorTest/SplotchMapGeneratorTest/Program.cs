
//TestSplotchGenerator();
using UnityEngine;

TestMapGenerator();
//TestRandomInt();
//TestRandomVector();

void TestRandomVector()
{
    PerformInteractiveTest((uint seed) =>
    {
        UnityEngine.Vector2Int min;
        UnityEngine.Vector2Int max;
        string[] input = { "" };
        bool minXParseWorked, minYParseWorked;
        bool maxXParseWorked, maxYParseWorked;
        int minX, minY, maxX, maxY;
        do
        {            
            Console.WriteLine("Enter min: ");
            input = Console.ReadLine().Split(',');
            minXParseWorked = int.TryParse(input[0], out minX);
            minYParseWorked = int.TryParse(input[1], out minY);
        } while (!minXParseWorked || !minYParseWorked);

        do
        {            
            Console.WriteLine("Enter max: ");
            input = Console.ReadLine().Split(',');
            maxXParseWorked = int.TryParse(input[0], out maxX);
            maxYParseWorked = int.TryParse(input[1], out maxY);
        } while (!maxXParseWorked || !maxYParseWorked);

        Vector2Int minPoint = new Vector2Int(minX, minY);
        Vector2Int maxPoint = new Vector2Int(maxX, maxY);
        Console.WriteLine($"random point: {SplotchGenerator.RandomRange(seed, minPoint, maxPoint)}");
    });
}

void TestRandomInt()
{
    PerformInteractiveTest((uint seed) =>
    {
        uint min;
        uint max;        
        Console.WriteLine("Enter min: ");
        while(!uint.TryParse(Console.ReadLine(), out min));
        Console.WriteLine("Enter max: ");
        while (!uint.TryParse(Console.ReadLine(), out max));
        Console.WriteLine(SplotchGenerator.RandomRange(seed, min, max));
    });
}

void TestSplotchGenerator()
{
    bool[,] splotch = SplotchGenerator.GenerateSplotchFooprint(4);
    for(int i = 0; i < 9; i++)
    {
        for(int j = 0; j < 9; j++)
        {
            Console.Write(splotch[i, j] ? '#' : '.');
        }
        Console.Write("\n");
    }
}
void TestMapGenerator() {
    var parameters = new SplotchParameters
    {
        width = 40,
        height = 30,
        numCellsHorizontal = 4,
        numCellsVertical = 3,
        minSplotchRadius = 3,
        maxSplotchRadius = 8,
        borderWidth = 1,
        splotchProbability = 0.5f,
        seed = 1000005
    };

    SplotchMap splotchMap;
        

    PerformInteractiveTest((uint seed) =>
    {
        parameters.seed = seed;
        splotchMap = SplotchGenerator.GenerateSplotchMap(parameters);
        for (int y = 0; y < parameters.height; y++)
        {
            for (int x = 0; x < parameters.width; x++)
            {
                char printValue = ' ';
                switch(splotchMap.splotchValues[x,y])
                {
                    case SplotchMap.SplotchValue.Empty:
                        printValue = ' ';
                        break;
                    case SplotchMap.SplotchValue.Occupied:
                        printValue = '#';
                        break;
                    case SplotchMap.SplotchValue.SplotchCenter:
                        printValue = '@';
                        break;
                }
                Console.Write(printValue);
            }
            Console.WriteLine();
        }
    });    
}

void PerformInteractiveTest(Action<uint> testFunction)
{
    string input = "";
    do
    {
        Console.WriteLine("Enter seed:");
        input = Console.ReadLine();
        Console.Clear();
        uint seedInput;
        if (!uint.TryParse(input, out seedInput)) continue;
        testFunction(seedInput);        
    } while (input != "q");
}
