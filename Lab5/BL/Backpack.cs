namespace BL;

public class Backpack : IComparable, ICloneable
{
    private readonly PossibleEntities _possibleEntities = new PossibleEntities();
    private readonly bool[] _addedEntities = new bool[15];

    public int NumOfEntities { get; private set; } = 0;
    public int Size { get; private set; } = 0;
    public int Value { get; private set; } = 0;
    public int MaxSize { get; } = 24;

    public Backpack()
    {
        _possibleEntities.SortByСost();
    }

    private Backpack( bool[] addedEntities, int numOfEntities, int size, int value)
    {
        _possibleEntities.SortByСost();
        _addedEntities = (bool[])addedEntities.Clone();
        NumOfEntities = numOfEntities;
        Size = size;
        Value = value;
    }


    public void AddEntity(int index)
    {
        _addedEntities[index] = true;
        Size += _possibleEntities[index].Size;
        Value += _possibleEntities[index].Price;
        NumOfEntities++;
    }

    public void RemoveEntity(int index)
    {
        _addedEntities[index] = false;
        Size -= _possibleEntities[index].Size;
        Value -= _possibleEntities[index].Price;
        NumOfEntities--;
    }

    public int CountPrice()
    {
        int sum = 0;
        for (int i = 0; i < 15; i++)
        {
            sum += _addedEntities[i] ? _possibleEntities[i].Price : 0;
        }

        return sum;
    }

    public int CountSize()
    {
        int sum = 0;
        for (int i = 0; i < 15; i++)
        {
            sum += _addedEntities[i] ? _possibleEntities[i].Size : 0;
        }

        return sum;
    }

    public bool IsOversize()
    {
        return MaxSize > Size;
    }

    public bool IsPossibleToAdd(int index)
    {
        return Size + _possibleEntities[index].Size <= MaxSize;
    }

    public bool IsEntityAdded(int index)
    {
        return _addedEntities[index];
    }

    public void GenerateRandomBackpack()
    {
        Random random = new Random();
        int newEntity = random.Next(0, 15);
        while (IsPossibleToAdd(newEntity))
        {
            if (!IsEntityAdded(newEntity))
            {
                AddEntity(newEntity);
            }

            newEntity = random.Next(0, 15);
        }
    }

    public int CompareTo(object? obj)
    {
        if (obj is Backpack backpack) return Value.CompareTo(backpack.Value);
             else throw new ArgumentException("Wrong parameter value");
         }

    public object Clone()
    {
        return new Backpack(_addedEntities, NumOfEntities, Size, Value);
    }

    public string GetAddedEntities()
    {
        string ans = "";
        for (int i = 0; i < 15; i++)
        {
            if (_addedEntities[i])
            {
                ans += _possibleEntities[i].Name + ' ';
            }
        }

        return ans;
    }
}