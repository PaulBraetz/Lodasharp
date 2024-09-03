using Lodasharp;

JsNode applicationState = [];

JsNode app = [("run", (JsFunc)( (@this, arg) => $"Hello, {arg.Get(0).AsString}!" ))];

applicationState = app.Call("run", applicationState);

Console.WriteLine(applicationState);
