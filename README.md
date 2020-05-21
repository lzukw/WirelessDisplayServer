# Wireless-Display Server

## Overview 
With this program a 'presentation-computer' is able to stream its desktop (screen-content) to
a 'projecting-computer'. The projecting-computer shows the streamed dekstop in
fullscreen-mode. Since the projecting-computer is connected to a projector,
the desktop of the presentation-computer is shown by the projector.

The projecting-computer is using Windows as operating system, the 
presentation-computer can use Linux, Mac-OS and windows.

This program (WirelessDisplayServer) has to be run on the projecting-computer. 
It is an ASP.NET webabi including a "static" website.

With either the webabi or the website, the presentation-computer can control the
projecting-computer by communication with this program (WirelessDisplayServer):

- The screen-resolution of the projecting-computer and the projector can be changed.
- Either a VNC-viewer in reverse-mode (listen-mode), or
- FFplay (ffmpeg) for showing the remote desktop can be started.

For changing the screen-resolution of the projecting-computer the external 
console-program screenres.exe 
( [see here on github](https://github.com/lzukw/ScreenRes) )
is used. 

As VNC-viewer the portable program vncviewer.exe from TightVNC, version 1.3.10 is used
( [see here](https://www.tightvnc.com/download/1.3.10/tightvnc-1.3.10_x86_viewer.zip) ).

For FFplay.exe (a receiver for a stream sent by FFmpeg) also a portable version is used
( [see here](https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-4.2.2-win64-static.zip) ).

## Running and Configuration

The source-code of this program (WirelessDisplayServer) is in the folder 
`WirelessDisplayServer`. From within this directory the program can be started
using `dotnet run`. But first the necessary third-party executables must be put
in the correct folders. Three third-party executables are used:

- vncviewer.exe from tightvnc-1.3.10_x86.zip
- ffplay.exe from ffmpeg-4.2.2-win64-static.zip
- screenres.exe from the folder ScreenRes

In `appsettings.jcon` the paths of these executables are preset to:

- ..\Third_Party\tightvnc-1.3.10_x86\vncviewer.exe
- ..\Third_Party\ffmpeg-4.2.2-win64-static\bin\ffplay.exe
- ..\Third_Party\ScreenRes\screenres.exe

Just extraxt the zip-Files in the folder `Third-party`, and double-check
the correct paths. If this program can't find the executables in the
specified paths, `dotnet run` terminates with an exception and a 
corresponding message.

Feel free to put the executables in other folders and change their
relative paths in `appsettings.json`. You can also change the 
command-line-arguments that are used, when these executables are
started (The substring `ppppp` in the command-line-arguments is replaced with
a port-number).

## Installing

An executable version can be created with the following command:

```
dotnet publish -c Release -o WirelessDisplayServer_executable -r win-x64 --self-contained
```
The paremter `--self-contained` creates a 'stand-alone' executable version. This 
paremeter can be omitted, if .NET-Core version 3.1 is installed on the target system.

All necessary files are put in the directory `WirelessDisplayServer_executable`.
The executable to start is called `WirelessDisplayServer.exe`. The configuration
can still be changed, by changing the contents of `appsettings.json`, which has
also been copied to `WirelessDisplayServer_executable` by `dotnet publish`.

## API-calls

The allowed API-calls to this program (WirelessDisplayServer) are given below. In the examples 
below, the IP-address of the projecting-computer (WirelessDisplayserver) is `192.168.1.119`.
For each API-call a curl-command is given.

### API-calls for manipulating the screen-resolution

GET: api/ScreenRes/AvailableScreenResolutions
```
curl  -v http://192.168.1.119/api/ScreenRes/AvailableScreenResolutions
```
Get the available screen-resolutions of the projecting computer. The returned text is a 
json-object. For example: `["640x480","800x600","1366x654","1024x768"]`.

GET: api/ScreenRes/InitialScreenResolution
```
curl  -v http://192.168.1.119/api/ScreenRes/InitialScreenResolution
```
Returns the initial screen-resolution of the projecting-computer (The 
screen-resolution, when WirelessDisplayServer was started). Return-value is for 
example: `"1366x654"`

GET: api/ScreenRes/CurrentScreenResolution
```
curl  -v http://192.168.1.119/api/ScreenRes/CurrentScreenResolution
```
Returns the current screen-resolution of the projecting-computer. Return-value is for
example: `"1024x768"`

POST: api/ScreenRes/SetScreenResolution
```
curl -v -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '"1024x768"' http://192.168.1.119/api/ScreenRes/SetScreenResolution
```
Change the screen-resolution of the projecting-computer running this
program (WirelessDisplayServer). The value to post is a string containing the
screen-resolution to set (for example "1024x768").

### API-calls for starting and stopping streaming-programs on the projecting-computer

POST: api/StreamPlayer/StartFfplay
```
curl -v -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '12345' http://192.168.1.119/api/StreamPlayer/StartFfplay
```
Start FFplay.exe and tell it to listen on the specified port. The data to post 
is this port-number, for example `12345` in the example above.

POST: api/StreamPlayer/StartVncViewerReverse
```
curl -v -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '5500' http://192.168.1.119/api/StreamPlayer/StartVncViewerReverse
```
Start vncviwer.exe in reverse-mode (listen-mode). The data to post is the port-number
which vncviewer.exe listens on. In the example above this is the typically used
port-number `5500`.

POST: api/StreamPlayer/StopAllStreamPlayers
```
curl -v -X POST -H "Accept: application/json" -H "Content-Type: application/json" --data '' http://192.168.1.119/api/StreamPlayer/StopAllStreamPlayers
```
Stop FFplay.exe and vncviewer.exe, if they are running. This POST-request doesn't
need any data.

GET: api/StreamPlayer/FfplayStarted
```
curl  -v http://192.168.1.119/api/StreamPlayer/FfplayStarted
```
Returns `true` true, if FFplay.exe has been started and is still running, and `false` otherwise.

GET: api/StreamPlayer/VncViewerStarted
```
curl  -v http://192.168.1.119/api/StreamPlayer/VncViewerStarted
```
Returns `true` true, if FFplay has been started, and `false` otherwise.

## Using the website

This program (WirelessDisplayServer) also serves a "static" website. The
user on the presentation-computer can use this website by calling
http://192.168.1.119 in a webbrowser (assuming an ip-address of 192.168.1.119
of the projecting-computer).

The website consits of a static html-page (index.html), some css and a javascript-
file (site.js). When the user uses the form-elements on the website, javascript-
function from site.js are called. These javascript-functions use the webapi described
above to control the projecting-computer. Also, these javascript-functions change the 
displayed html-page in the browser by hiding some content and displaying other content
of the page. So the page doesn't look static, allthough it is a static site (meaning,
that the file index.html is never changed by the webserver).

With the website, the user on the presentation-computer can also set the 
display-resolution of the projecting-computer (and the projector), and start or stop
sreaming. In case of an error, an error-site is displayed.


