namespace AntAlgo;

internal class Road
{
    public int Length { get; set; }
    public double CurrentPheromoneAmount { get; set; } = 0.1;
    public double Visibility { get; set; }
    public double PheromoneToAdd { get; set; }
}