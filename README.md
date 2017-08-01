
# IPTree.Net
IPTree.Net is a simple and high performant open source library for blocking IP addresses or IP networks. (.NETStandard 1.3)

## Usage
The following example instantiates a new `IPTree` and adds some IP addresses. 

``` csharp

var ipTree = new Tireless.Net.Blocking.IPTree();
ipTree.AddNetwork(IPAddress.Parse("127.0.0.0"), 8, "localhost");
ipTree.AddNetwork(IPAddress.Parse("192.168.0.0"), 24, "hacker");

var testIps = new System.Collections.Generic.List<IPAddress>();
testIps.Add(IPAddress.Parse("127.0.0.1"));
testIps.Add(IPAddress.Parse("192.168.2.1"));
testIps.Add(IPAddress.Parse("192.168.0.1"));
testIps.Add(IPAddress.Parse("8.8.8.8"));

foreach (var ip in testIps)
{
    var rule = ipTree.IsBlocked(ip);
    Console.Write(ip + ": ");
    if (rule != null)
        Console.WriteLine("is blocked by \"" + rule + "\" rule ");
    else
        Console.WriteLine("not blocked.");
}
Console.ReadLine();
``` 

Output:

```
127.0.0.1: is blocked by "localhost" rule
192.168.2.1: not blocked.
192.168.0.1: is blocked by "hacker" rule
8.8.8.8: not blocked.
```

## Builds

Get it via NuGet https://www.nuget.org/packages/Tireless.IPTree/

### Build from Code
Just clone the repository and open the solution in Visual Studio 2017.
Or use the dotnet client via command line.
