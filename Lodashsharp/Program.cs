using Lodasharp;

LsNode applicationState = [];

LsNode app = [("run", (LsFunc)( (@this, arg) => $"Hello, {arg.Get(0).AsString}!" ))];

applicationState = app.Call("run", applicationState);

Console.WriteLine(applicationState);
