# LogExCore
![Build and Run Tests](https://github.com/mezm/LogExCore/workflows/Build%20and%20Run%20Tests/badge.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/v/LogExCore.svg)](https://www.nuget.org/packages/LogExCore/) 

**LogExCore** is a library that extends `Microsoft.Extensions.Logging`.
Now it contains only `SingleLineConsoleLogger`. But there are plans to add few more, for example `FileLogger`

# Install
To install **LogExCore** run:
```
Install-Package LogExCore
```

# Loggers
## SingleLineConsoleLogger
`SingleLineConsoleLogger` is similar to `Microsoft.Extensions.Logging.Console.ConsoleLogger` but write each log message by a single line.

To use it in ASP.NET CORE you should modify **Program.cs**:
```csharp
using LogExCore;

...

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSingleLineConsole(); // set SingleLineConsoleLoggerProvider
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```
To use it in console application you can write something like this:
```csharp
using LogExCore;

...

var loggerFactory = LoggerFactory.Create(builder => builder.AddSingleLineConsole());
var logger = loggerFactory.CreateLogger<Program>();
