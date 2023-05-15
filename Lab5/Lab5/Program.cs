using BL;

Beehive beehive = new Beehive();
beehive.StartBeehive(100);
Console.WriteLine("Best value: {0}\nAdded entities: {1}", beehive.GetBestSolution().Value, beehive.GetBestSolution().GetAddedEntities());


// int bestScoutBeesNum = 4, bestWorkerBees = 30, bestFlowerNum = 10;
// int  bestValue = 0;
//
// for (int i = 0; i < 4; i++)
// {
//     Console.WriteLine("----------------------------");
//
//     int tempBestNum = bestFlowerNum;
//     
//     bestValue = 0;
//     for (int flowerNum = bestScoutBeesNum; flowerNum <= 300; flowerNum++)
//     {
//         int sum = 0;
//         for (int j = 0; j < 100; j++)
//         {
//             Beehive beehive = new Beehive(bestScoutBeesNum, bestWorkerBees, flowerNum);
//             beehive.StartBeehive(10);
//             Backpack backpack = beehive.GetBestSolution();
//             // Console.WriteLine(backpack.Value);
//             sum += backpack.Value;
//
//         }
//         if (sum > bestValue)
//         {
//             bestValue = sum;
//             tempBestNum = flowerNum;
//         }
//     }
//
//     bestFlowerNum = tempBestNum;
//     Console.WriteLine("Найкраще значення к-сті квіток: {0}", bestFlowerNum);
//
//     
//     // Console.WriteLine("{0} {1}", bestFlowerNum, bestValue);
//
//     tempBestNum = bestScoutBeesNum;
//
//     bestValue = 0;
//     for (int scoutBeesNum = 1; scoutBeesNum < bestFlowerNum + 1; scoutBeesNum++)
//     {
//         int sum = 0;
//         for (int j = 0; j < 100; j++)
//         {
//             Beehive beehive = new Beehive(scoutBeesNum, bestWorkerBees, bestFlowerNum);
//             beehive.StartBeehive(10);
//             Backpack backpack = beehive.GetBestSolution();
//             // Console.WriteLine(backpack.Value);
//             sum += backpack.Value;
//         }
//
//         if (sum > bestValue)
//         {
//             bestValue = sum;
//             tempBestNum = scoutBeesNum;
//         }
//     }
//
//     bestScoutBeesNum = tempBestNum;
//     Console.WriteLine("Найкраще значення к-сті розвідників: {0}", bestScoutBeesNum);
//
//     // Console.WriteLine("{0} {1}", bestScoutBeesNum, bestValue);
//
//     tempBestNum = bestWorkerBees;
//
//     bestValue = 0;
//     for (int workerBees = 1; workerBees <= 15*bestScoutBeesNum; workerBees++)
//     {
//         int sum = 0;
//         for (int j = 0; j < 100; j++)
//         {
//             
//             beehive.StartBeehive(10);
//             Backpack backpack = beehive.GetBestSolution();
//             // Console.WriteLine(backpack.Value);
//             sum += backpack.Value;
//         }
//
//         if (sum > bestValue)
//         {
//             bestValue = sum;
//             tempBestNum = workerBees;
//         }
//     }
//
//     bestWorkerBees = tempBestNum;
//
//     Console.WriteLine("Найкраще значення к-сті робітників: {0}", bestWorkerBees);
//     // Console.WriteLine("{0} {1}", bestWorkerBees, bestValue);
// }