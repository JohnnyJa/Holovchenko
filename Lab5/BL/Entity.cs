namespace BL;

public class Entity : IComparable
{
    public string Name { get; }
    public int Price { get; }
    public int Size { get; }

    public Entity(string name, int price, int size)
    {
        Name = name;
        Price = price;
        Size = size;
    }

    public int CompareTo(object? obj)
    {
        if (obj is Entity entity) return ((double)Price / Size).CompareTo((double)entity.Price / entity.Size);
        else throw new ArgumentException("Wrong parameter value");
    }
}