using AntAlgo;

// int[,] matrix =
// {
//     { 0, 31, 15, 19, 8, 55 }, //0
//     { 19, 0, 22, 31, 7, 35 }, //1
//     { 25, 43, 0, 53, 57, 16 }, //2
//     { 5, 50, 49, 0, 39, 9 }, //3
//     { 24, 24, 33, 5, 0, 14 }, //4
//     { 34, 26, 6, 3, 36, 0 }
// };
int size = 100;

int [,] matrix = new int[size, size];

Random rnd = new Random();

for (int i = 0; i < size; i++)
{
    for (int j = 0; j < size; j++)
    {
        matrix[i, j] = rnd.Next(5,51);
        if (i == j)
        {
            matrix[i, j] = 0;
        }
    }

    
}

// for (int i = 0; i < size; i++)
// {
//     for (int j = 0; j < size; j++)
//     {
//         Console.Write("{0} ", matrix[i,j]);
//     }
//     Console.WriteLine();
// }

AntAlgoPerformer performer = new AntAlgoPerformer();
performer.GetGraph(matrix);
int length = performer.LaunchAlgo(35);
Console.WriteLine("{0}", length);
performer.PrintBestWay();