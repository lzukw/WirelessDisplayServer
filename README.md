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

On windows the following programs are needed:

For changing the screen-resolution the external console-program screenres.exe 
( [see here on github](https://github.com/lzukw/ScreenRes) )
is used. The github-repository also contains screenres.exe.

As VNC-viewer the portable program vncviewer.exe from TightVNC, version 1.3.10 is used
( [see here](https://www.tightvnc.com/download/1.3.10/tightvnc-1.3.10_x86_viewer.zip) ).

For FFplay.exe (a receiver for a stream sent by FFmpeg) also a portable version is used
( [see here](https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-4.2.2-win64-static.zip) ).

As long as github doesn't complain about large files, I provide a copy of 
TightVNC and ffmpeg my repository 
[Third-party-tools](https://github.com/lzukw/Third-party-tools).


## Running and Configuration

The source-code of this program (WirelessDisplayServer) is in the folder 
`WirelessDisplayServer`. From within this directory the program can be started
using `dotnet run`. But first the necessary third-party executables must be 
made available. 

The executables are not started directly by WirelessDisplayServer, but they 
are started using the scripts in the directory [Scripts/<operating-system>]. 
Have a look at the [README.md] in the directory [Scripts] for furhter 
information.

On Windows three third-party executables are used:

- vncviewer.exe from tightvnc-1.3.10_x86.zip
- ffplay.exe from ffmpeg-4.2.2-win64-static.zip
- screenres.exe from the folder ScreenRes

The scripts in [Scripts/Windows] look for the executables in

- ..\..\Third_Party\tightvnc-1.3.10_x86\vncviewer.exe
- ..\..\Third_Party\ffmpeg-4.2.2-win64-static\bin\ffplay.exe
- ..\..\Third_Party\ScreenRes\screenres.exe

Downlaod the zip-Files [tightvnc-1.3.10_x86_viewer.zip] and 
[ffmpeg-4.2.2-win64-static.zip] and extract them into the 
[ThirdParty]-folder. Clone the 
[ScreenRes-Repository](https://github.com/lzukw/ScreenRes) or download it as
zip and also put it into the [ThirdParty]-folder.

Double-check the correct paths. If this program can't find the executables in 
the specified paths, the scripts starting these executable fail, and so will
the whole program.

Feel free to put the executables in other folders and change their
relative paths in the scripts. You can also change the 
command-line-arguments that are used, when these executables are
started. When the scripts are started from the WirelessDisplayServer 
several command-line-arguments are passed to them. See the [README.md] in the
directory [Scripts] for details.

## Installing

An executable version can be created with the following commands (from within the folder,
where `Program.cs` and `Startup.cs` are):

```
mkdir ..\WirelessDisplayServer_executable 
dotnet publish -c Release -o ..\WirelessDisplayServer_executable\WirelessDisplayServer -r win-x64 --self-contained true
```

On Linux, replace `win-x64` with `linux-x64` and on macOS with `osx-x64`.

The parameter `--self-contained true` creates a self-contained (stand-alone) 
executable version for the given operating system. Such
a version also contains the used dotnet core framework, so on the 
target-computer no dotnet-core-framework needs to be installed. Self-contained
builds only work for the given platform, and consume most disk-space.

With `--self-contained false` a "runtime-dependent"-Version of the program, 
which only runs on the specified platform. The "runtime-dependet" version also
requires, that dotnet core (version 3.1) is installed on the computer. This
variant conumes the least disk-space.

With `--self-contained false` and omitting `-r ...-x64`, A "runtime-dependent" 
version can be built, that runs on every platform (operating-system). 
The only requirement is, that dotnet-core runtime must be installed on the 
target-computer.

If a "runtime-dependet" version is executed on a computer without an installed
dotnet-core-framework, a popup appears and offers the user the possibility to
install the missing dotnet-core-framework.

The three folders

- [WirelessDisplayServer_executable],
- [Scripts], and
- [ThirdParty]

contain the whole executable program. The file to be executed is
[WirelessDisplayServer_executable\WirelessDisplayServer.exe] on Windows and
[WirelessDisplayServer_executable/WirelessDisplayServer] on Linux.

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


