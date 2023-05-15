namespace BL;

public class Beehive
{
    private int ScoutBeesNum = 4;
    private int WorkerBeesNum = 30;
    private int FlowersNum = 10;
    private PossibleEntities _possibleEntities = new PossibleEntities();
    private PossibleEntities _sortedEntities = new PossibleEntities();
    
    private List<Backpack> _allFlowers = new List<Backpack>();

    public Beehive()
    {
        _sortedEntities.SortByСost();
    }
    public Beehive(int scoutBeesNum, int workerBeesNum, int flowersNum)
    {
        ScoutBeesNum = scoutBeesNum;
        WorkerBeesNum = workerBeesNum;
        FlowersNum = flowersNum;
        _sortedEntities.SortByСost();
    }

    public void StartBeehive(int iterations)
    {
        GenerateFlowers();
        for (int i = 0; i < iterations; i++)
        {
            Backpack[] scoutedFlowers = SendScouts();
            SendWorkers(scoutedFlowers);
        }
    }

    private void GenerateFlowers()
    {
        for (int i = 0; i < FlowersNum; i++)
        {
            Backpack backpack = new Backpack();
            backpack.GenerateRandomBackpack();
            _allFlowers.Add(backpack);
        }
    }

    private Backpack[] SendScouts()
    {
        Backpack[] scoutedFlowers = GetFlowersToScout();
        EvaluateFlowers(ref scoutedFlowers);
        return scoutedFlowers;
    }

    private Backpack[] GetFlowersToScout()
    {
        var rnd = new Random();
        var randomNumbers = Enumerable.Range(0, FlowersNum).OrderBy(x => rnd.Next()).Take(ScoutBeesNum).ToList();

        Backpack[] scoutedFlowers = new Backpack[ScoutBeesNum];
        for (int i = 0; i < ScoutBeesNum; i++)
        {
            scoutedFlowers[i] = _allFlowers[randomNumbers[i]];
        }
        randomNumbers.Sort((x, y) => y.CompareTo(x));
        for (int i = 0; i < ScoutBeesNum; i++)
        {
            _allFlowers.RemoveAt(randomNumbers[i]);
        }
        
        return scoutedFlowers;
    }

    private void EvaluateFlowers(ref Backpack[] scoutedFlowers)
    {
        Array.Sort(scoutedFlowers);
    }

    private void SendWorkers(Backpack[] scoutedFlowers)
    {
        int sentWorkers = 0, flowerIndex = 0;
        while (WorkerBeesNum > sentWorkers && ScoutBeesNum > flowerIndex)
        {
            int workersNeeds = CountWorkersSentOnFlower(sentWorkers, scoutedFlowers[flowerIndex]);
            Backpack newFlower = SendWorkerOnFlower(workersNeeds, scoutedFlowers[flowerIndex]);
            _allFlowers.Add(newFlower);
            sentWorkers += workersNeeds;

            flowerIndex++;
        }

        for (int i = flowerIndex; i < ScoutBeesNum; i++)
        {
            _allFlowers.Add(scoutedFlowers[flowerIndex]);
        }
    }

    private int CountWorkersSentOnFlower(int alreadySentWorkers, Backpack flower)
    {
        int sentWorkers;
        if (alreadySentWorkers + flower.NumOfEntities <= WorkerBeesNum)
        {
            sentWorkers = flower.NumOfEntities;
        }
        else
        {
            sentWorkers = WorkerBeesNum - alreadySentWorkers;
        }

        return sentWorkers;
    }

    private Backpack SendWorkerOnFlower(int numOfWorkers, Backpack flower)
    {
        Backpack bestFlower = flower;
        int checkedEntity = 0;
        for (int i = 0; i < numOfWorkers; i++)
        {
            Backpack processedFlower = (Backpack) flower.Clone();
            while (!processedFlower.IsEntityAdded(checkedEntity))
            {
                checkedEntity++;
            }

            processedFlower.RemoveEntity(checkedEntity);
            processedFlower = TryToUpgrade(processedFlower);

            if (processedFlower.Size > bestFlower.Size)
            {
                bestFlower = processedFlower;
            }
        }

        return bestFlower;
    }

    private Backpack TryToUpgrade(Backpack flower)
    {
        for (int i = 15 - 1; i >= 0; i--)
        {
            if (flower.IsPossibleToAdd(i))
            {
                flower.AddEntity(i);
            }
        }

        return flower;
    }

    public Backpack GetBestSolution()
    {
        return _allFlowers.Max();
    }
}