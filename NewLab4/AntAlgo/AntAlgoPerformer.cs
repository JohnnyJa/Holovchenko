namespace AntAlgo;

public class AntAlgoPerformer
{
    private Road[,] _roads = null!;
    private int _a, _b, _Lmin, _numOfAnts, _size, _Tmax;
    private double _p;
    private readonly List<Ant> _colony = new List<Ant>();
    private List<int> _bestWay = new List<int>();
    private int _bestWayLength = -1, _prevBestLength = 0;


    public void GetGraph(int[,] matrix)
    {
        _size = matrix.GetUpperBound(0) + 1;
        _roads = new Road[_size, _size];
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _roads[i, j] = new Road
                {
                    Length = matrix[i, j]
                };
            }
        }
    }

    public int LaunchAlgo(int numOfAnts)
    {
        _numOfAnts = numOfAnts;
        SetParameters();
        SetWays();
        GenerateColony();
        MainAlgo();
        return _bestWayLength;
    }

    public void PrintBestWay()
    {
        Console.WriteLine("{0} {1}", _bestWayLength, _Lmin);
        foreach (var node in _bestWay)
        {
            Console.Write("{0} ", node);
        }

        Console.WriteLine("\n\n");
    }

    private void SetParameters()
    {
        _a = 2;
        _b = 3;
        _p = 0.4;
        FindLmin();
        _Tmax = 1000;
    }

    private void FindLmin()
    {
        List<bool> isVisited = new List<bool>(Enumerable.Repeat(false, _size))
        {
            [0] = true
        };
        int currPosition = 0;
        while (!IsAllVisited(isVisited))
        {
            int nextPosition = FindMinLength(currPosition, isVisited);
            _Lmin += _roads[currPosition, nextPosition].Length;
            isVisited[nextPosition] = true;
            currPosition = nextPosition;
        }

        _Lmin += _roads[currPosition, 0].Length;
    }

    private int FindMinLength(int startNode, List<bool> isVisited)
    {
        int minLength = 100;
        int minNode = startNode;
        for (int endNode = 0; endNode < _size; endNode++)
        {
            if (startNode == endNode)
            {
                continue;
            }

            if (minLength > _roads[startNode, endNode].Length && !isVisited[endNode])
            {
                minLength = _roads[startNode, endNode].Length;
                minNode = endNode;
            }
        }

        return minNode;
    }

    private void SetWays()
    {
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _roads[i, j].Visibility = 1 / (double)_roads[i, j].Length;
                _roads[i, j].CurrentPheromoneAmount = 0.1;
            }
        }
    }

    private void GenerateColony()
    {
        var rnd = Enumerable.Range(0, _size);
        var random = new Random();
        var shuffle = rnd.ToList();
        for (var i = 2; i < shuffle.Count; ++i)
        {
            var temp = shuffle[i];
            var nextRandom = random.Next(i - 1);
            shuffle[i] = shuffle[nextRandom];
            shuffle[nextRandom] = temp;
        }

        for (int i = 0; i < _numOfAnts; i++)
        {
            _colony.Add(new Ant()
            {
                StartPosition = shuffle[i],
                CurrPosition = shuffle[i],
                LengthOfWay = 0
            });
        }
    }

    private void MainAlgo()
    {
        for (int i = 0; i < _Tmax; i++)
        {
            ResetWays();
            foreach (var ant in _colony)
            {
                ResetAnt(ant);
                FindWholeWay(ant);
                PutPheromone(ant);
            }

            Ant bestWayAnt = _colony.MinBy(ant => ant.LengthOfWay)!;
            _bestWay = bestWayAnt.Way;
            _prevBestLength = _bestWayLength;
            _bestWayLength = bestWayAnt.LengthOfWay;
            
            if (i % 20 == 0)
            {
                Console.WriteLine("{0}", _bestWayLength);
            }

            UpdatePheromone();
        }
    }

    private void PutPheromone(Ant ant)
    {
        double pheromoneAmount = _Lmin / (double)ant.LengthOfWay;
        for (int i = 1; i < ant.Way.Count; i++)
        {
            _roads[ant.Way[i - 1], ant.Way[i]].PheromoneToAdd += pheromoneAmount;
        }
    }

    private void UpdatePheromone()
    {
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _roads[i, j].CurrentPheromoneAmount =
                    (1 - _p) * _roads[i, j].CurrentPheromoneAmount + _roads[i, j].PheromoneToAdd;
            }
        }

        for (int i = 1; i < _bestWay.Count; i++)
        {
            _roads[_bestWay[i - 1], _bestWay[i]].PheromoneToAdd += _Lmin / (double)_bestWayLength;
        }
    }

    private void ResetAnt(Ant ant)
    {
        ant.LengthOfWay = 0;
        ant.Way = new List<int>();
        ant.CurrPosition = ant.StartPosition;
    }

    private void ResetWays()
    {
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _roads[i, j].PheromoneToAdd = 0;
            }
        }
    }

    private void FindWholeWay(Ant ant)
    {
        List<bool> isVisited = new List<bool>(Enumerable.Repeat(false, _size))
        {
            [ant.StartPosition] = true
        };
        ant.CurrPosition = ant.StartPosition;

        while (!IsAllVisited(isVisited))
        {
            int nextPosition = FindBestWay(ant.CurrPosition, isVisited);
            ant.Way.Add(ant.CurrPosition);
            ant.LengthOfWay += _roads[ant.CurrPosition, nextPosition].Length;
            isVisited[nextPosition] = true;
            ant.CurrPosition = nextPosition;
        }

        ant.Way.Add(ant.CurrPosition);
        ant.Way.Add(ant.StartPosition);
        ant.LengthOfWay += _roads[ant.CurrPosition, ant.StartPosition].Length;
    }

    private bool IsAllVisited(List<bool> isVisited)
    {
        bool isAllVisited = true;
        foreach (var isWayVisited in isVisited)
        {
            if (isWayVisited == false)
                isAllVisited = false;
        }

        return isAllVisited;
    }

    private int FindBestWay(int currentPosition, List<bool> isVisited)
    {
        double sum = CountSum(currentPosition);


        double[] probabilityToChoose = new double[_size];
        for (int i = 0; i < _size; i++)
        {
            if (i == currentPosition || isVisited[i])
            {
                probabilityToChoose[i] = -1;
                continue;
            }

            Road road = _roads[currentPosition, i];
            probabilityToChoose[i] = Math.Pow(road.Visibility, _a) * Math.Pow(road.CurrentPheromoneAmount, _b) / sum;
        }

        return Array.IndexOf(probabilityToChoose, probabilityToChoose.Max());
    }


    private double CountSum(int currentPosition)
    {
        double sum = 0;
        for (int i = 0; i < _size; i++)
        {
            if (currentPosition == i)
            {
                continue;
            }

            Road road = _roads[currentPosition, i];
            sum += Math.Pow(road.Visibility, _a) * Math.Pow(road.CurrentPheromoneAmount, _b);
        }

        return sum;
    }
}