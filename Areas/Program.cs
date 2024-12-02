
using AreasNS;

Areas areas = new Areas();

areas.AddArea( "A", [
    (0,0),
    (4,0),
    (4,4),
    (0,4),
]);

areas.AddArea( "B", [
    (0,0),
    (1,0),
    (1,1),
    (0,1),
]);


Console.WriteLine("1, 1: " + areas.BestArea(1,1));
Console.WriteLine("3, 7: " + areas.BestArea(3, 7));
Console.WriteLine("7, 3: " + areas.BestArea(7, 3));
Console.WriteLine("9, 9: " + areas.BestArea(9, 9));

Console.ReadKey();