using System.Globalization;

using Lodasharp;

using static Lodasharp.LsArray;

LsNode applicationState = Arr("Rogue");

LsNode app = [("run", (LsFunc)( (@this, arg) =>
{
    var target = arg.Get(0);

    var result = target
        .Bind((String s) => $"Hello, {s}!", n => DateTimeOffset.Now)
        .Bind((DateTimeOffset now) => now.ToString(CultureInfo.InvariantCulture));

    return result;
} ))];

applicationState = app.Call("run", applicationState);

Console.WriteLine(applicationState);
