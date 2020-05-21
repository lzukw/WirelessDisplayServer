# How does this all work?

For people who do not know ASP.NET this document describes how this program 
(WirelessDisplayServer) works.

Personally, I found it difficult to learn ASP.NET using 
[Microsoft's documentation](https://docs.microsoft.com/en-us/aspnet/core).
In my opinion, a very good place to learn ASP.NET is
[Learn razor pages](https://www.learnrazorpages.com/).


## Project setup

This project was created by the command `dotnet new webapi --no-https` (run in
the folder WirelessDisplayServer), and then edited with Visual Studio Code. This
command creates a program, that can be run and provides a web-api (REST-api) for
weather-forecast-Items. All things, that have to do with these weather-forcasts
are deleted from the project. The remaining relevant files are:

- Program.cs
- Startup.cs
- appsettings.json

Newly created relevant files are:

- IScreenResolutionService.cs and ScreenResolutionService.cs in the subfolder Services.
- IStreamPlayerService.cs and StreamPlayerService.cs in the subfolder Services.
- ScreenResController.cs and StreamPlayerCpontroller.cs in the subfolder Controllers.
- static .html-, .css-, and .js-Files in the subfolder wwwroot.

## The webserver run by ASP.NET

All the magic is done whithin "code behind the scenes" by ASP.NET. All this code is
configured in the two files `Program.cs` and `Startup.cs`. The File `Program.cs`
contains the entry-Point for the Program `public static void Main(string[] args)`.
Here an Object, that implements the `IHost`-interface is created and started. This
host is an object that encapsulates an app's resources ( [see Generic host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1) ).
One of the services, that are started by this `IHost` is a web-service, that 
listens for HTTP-Requests on the network. 

The file `Program.cs` has not been modified.

When The `IHost` is created, it uses the class `Startup` defined in 
`Startup.cs`. Here all configuration is done. The following features, which are
offered by the ASP.NET framework, are used, and some of these features are configured
in `Startup.cs`:

- Configuration
- Logging
- Dependency-Injection
- Serving static files

### Configuration

Configuration-parameters that can be used in an ASP.NET-application can be provided
by the user in several ways. The most important way to provide configration-parameters
are:

- Command-line-arguments (which become the `string[] args` paremter of `Main()`)
- The file `appsettings.json`

The following configuration-parameters are necessary. Their default-values are set in
`appsettings.json`, but they could be overwritten by providing command-line-arguments:

- `PathToTightVncViewer`: Path to the VNC-viewer-executable (vncviewer.exe), 
- `VncViewerArgs`: Command-line-argumentes passed to the the VNC-viewer-executable (vncviewer.exe)
- `PathToFFplay`:  Path to FFplay.exe
- `FFplayArgs`: Command-line-arguments passed to FFplay.exe
- `urls`: Has a value of "http://*:80", which means that the webserver listens on all 
          interfaces on port 80.

Remark: In `VncViewerArgs` and `FFplayArgs` the placeholder `ppppp` is replaced by the 
port-number provided by the user, when calling vncviewer.exe / FFplay.exe.

The `Startup`-object defined in `Startup.cs` receives all the configuration-parameters
provided by the different sources (command-line-arguments, appsettings.json) when it is
instantiated. The ASP.NET-framework passes the configuration-parameters in form of an 
object implementing the `IConfiguration`-interface to the constructor of the
`Startup`-class. The constructor stores this opject in a proprty named `Configuration`.

Within the methods of the `Startup`-class the configuration-parameters can be read
in a simple way. For example, to get the path to the VNC-viewer-executable (provided in
appsettings.json or by command-line-parameters) the code in `Startup.cs` just reads the
value of `Configuration["PathToTightVncViewer"]`.

This used methodology is a form of "dependecy injection", which is called
"constructor-injection". This design-pattern seems to be widely used in the 
ASP.NET-framework. It consists of the following steps:

- A class needs some data: For example the `Startup`-class needs configuration-data.
- Configuration-data can be provided by different classes, but all these
  classes must implement the same interface. In our example the class of the object 
  providing the configuration-data is not known, but it implements the 
  `IConfiguration`-interface. This interface provides properties or methods to read
  the configuration-data.
- The `Startup`-class has a parameterized constructor. This constructor receives 
  a parameter of type `Iconfiguration`. When a `Startup`-object is instatiated
  it stores the received object implementing the `Iconfiguration`-interface in
  a local property. This property can later be used by methods inside the 
  `Startup`-class.
- The "code behind the scenes" in the ASP.NET-framework is responsible for
  instiating an object of the `Startup`-class. At this point, this "code behind the
  scenes" must also pass an object, which is storing the configuration-data and 
  implements the `Iconfiguration`-Interface, to the constructor of the `Startup`-class.

### Logging

TODO: Explanation of injected Ilogger<T>

See also [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1)

### Dependency-Injection

TODO: Further example with ScreenResController.cs, IScreenResolutionService.cs and ScreenResolutionService.cs, Configuration of dependency-injection in Startup.cs with
services.AddSingleton

### Controllers and HTTP-Requests 

TODO: bring example with a complete HTTPP-request ( and describe the role of the Controller).

### Serving static files

TODO: Explain static Website (Configuration in Startup.cs with app.UseDefaultFiles(); and app.UseStaticFiles();, folder wwwroot)


