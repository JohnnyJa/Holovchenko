using System.Collections;

namespace BL;

public class PossibleEntities
{

    private readonly Entity[] _possibleEntitiesArray = new[]
    {
        new Entity("1", 6, 2),
        new Entity("2",6, 4),
        new Entity("3",7, 5),
        new Entity("4",6, 4),
        new Entity("5",4, 2),
        new Entity("6",2, 1),
        new Entity("7",3, 1),
        new Entity("8",4, 2),
        new Entity("9",5, 4),
        new Entity("10",2, 1),
        new Entity("11",1, 1),
        new Entity("12",3, 2),
        new Entity("13",4, 2),
        new Entity("14",8, 3),
        new Entity("15",7, 2)
    };

    public Entity this[int index] => _possibleEntitiesArray[index];

    public void SortByСost()
    {
        Array.Sort(_possibleEntitiesArray);
    }
}