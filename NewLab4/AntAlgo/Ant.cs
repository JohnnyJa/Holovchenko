namespace AntAlgo;

internal class Ant
{
    public int StartPosition { get; set; }
    public int CurrPosition { get; set; }
    public int LengthOfWay { get; set; }
    public List<int> Way { get; set; } = new List<int>();
}