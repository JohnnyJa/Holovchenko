// See https://aka.ms/new-console-template for more information

using Entities;
using MainLogic;

Console.WriteLine("Hello, World!");

Repository rep = new Repository();
rep.CreateDb();
// rep.OpenDb();
Entity ent = new Entity()
{
    Id = 15,
    Data = "qqqqqqqqqqqqqqqq"
};
rep.AddNewEntity(ent);
ent = new Entity()
{
    Id = 11,
    Data = "rrrrrrrrrrrrrrrr"
};
rep.AddNewEntity(ent);

ent = new Entity()
{
    Id = 1027,
    Data = "ffffffffffffffff"
};
rep.AddNewEntity(ent);
ent = rep.GetEntityByKey(11);

Console.WriteLine("{0} {1}", ent.Data, rep.GetComparisons());
rep.UpdateDataByKey(11, "aaaaaaaaaaaaaaaa");

ent = rep.GetEntityByKey(1027);
Console.WriteLine("{0} {1}", ent.Data, rep.GetComparisons());


// rep.GetIndexFile();
