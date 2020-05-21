// Called, when index.html is loaded from the server (projecting-computer)
function showStartPlayerPage()
{
    // Fill in the IP-Address of the server (projecting-computer). This is
    // the host-part of the URL, so this is is not the IP-Address, if the website
    // is called using the hostname instead of the IP-Address
    var ip = location.host;
    document.getElementById("ProjectingComputerIP").innerHTML = ip;

    // Get the available screenResolutions and add them as options to the <select>-Element
    // Aslo, the initial screen-Resolution is fetched and inserted it in appropriate Elements.
    fetch("api/ScreenRes/AvailableScreenResolutions")
        .then(response => response.json())
        .then(resolutions => _processScreenResolutions(resolutions) )
        .catch(error => console.error('Unable to get available screen-resolutions.', error));
        
    // Show only the first of three possible "Forms"
    document.getElementById("FormToStartPlayer").removeAttribute("hidden");
    document.getElementById("FormToStopPlayer").setAttribute("hidden", "hidden");
    document.getElementById("ErrorPage").setAttribute("hidden", "hidden");
}


function _processScreenResolutions(resolutions)
{
    // Fill in passed resolutions in select-Elements "ScreenResolutionStart"  and "ScreenResolutionStop" 
    // In "ScreenResolutionStart" also preselect the resolution, whose width is nearest to 1024 pixel,
    // and in "ScreenResolutionStop" preselect the initial screen resolution
    var selectElementStart = document.getElementById("ScreenResolutionStart");
    var selectElementStop = document.getElementById("ScreenResolutionStop");
    selectElementStart.innerHTML = "";
    selectElementStop.innerHTML = "";

    var bestRes = null;
    const bestWidth = 1024;
    var smallestWidthDiff = bestWidth;

    for (var i = 0; i < resolutions.length; i++)
    {
        var optionElementStart = document.createElement("option");
        optionElementStart.value = resolutions[i];
        optionElementStart.innerHTML = resolutions[i];
        selectElementStart.appendChild(optionElementStart);

        var optionElementStop = document.createElement("option");
        optionElementStop.value = resolutions[i];
        optionElementStop.innerHTML = resolutions[i];
        selectElementStop.appendChild(optionElementStop);

        var width = Number(resolutions[i].split("x")[0]);
        var diff = Math.abs(width - bestWidth);
        if (diff < smallestWidthDiff)
        {
            smallestWidthDiff = diff;
            bestRes = optionElementStart;
        }
    }

    // preselect the resolutions, whose whidth is nearest to 1024 Pixel in Element "ScreenResolutionStart"
    if (bestRes !== null)
    {
        bestRes.selected = true;
    }

    // Fetch the initial Screen-Resolution, then preselect it in "ScreenResolutionStop" and fill it in
    // the element "InitialResolution"
    fetch("api/ScreenRes/InitialScreenResolution")
        .then(response => response.json())
        .then(initalResolution => _updateFormWithInitialScreenResolution(initalResolution))
        .catch(error => console.error('Unable to get initial screen-resolution.', error));
}


function _updateFormWithInitialScreenResolution(initialResolution)
{ 
    document.getElementById("InitialResolutionStart").innerHTML = initialResolution;
    document.getElementById("InitialResolutionStop").innerHTML = initialResolution;

    el = document.getElementById("ScreenResolutionStop")
    for (var i = 0; i < el.options.length; i++)
    {
        el.options[i].selected = (el.options[i].value === initialResolution)
    }
}


// Called, when user pushes the Button "Start streaming"
function startPlayer() {
    // First set Screen-Resoltution of presentation-Computer via call to web-api
    // then call _startVncOrFFmpeg()
    var e = document.getElementById("ScreenResolutionStart");
    var resolution = e.options[e.selectedIndex].value;

    fetch("api/ScreenRes/SetScreenResolution", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: "\"" + resolution + "\""
    })
        .then(response => _startVncOrFFmpeg())
        .catch(error => console.error('Unable to set screen-resolutions before starting player.', error));
}


function _startVncOrFFmpeg()
{
    // Start either VNC-Viewer in reverse connection mode or FFplay via call to web-api
    // then call either _checkIfVNCViewerReverseStarted() or _checkIfFFplayStarted()

    var portNo = document.getElementById("PortNo").value;

    if (document.getElementById("RadioButtonVNC").checked) {
        fetch("api/StreamPlayer/StartVncViewerReverse", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: "" + portNo
        })
            .then(response => _checkIfVNCViewerReverseStarted())
            .catch (error => console.error('Unable to start VNC-viewer.', error));

    }
    else if (document.getElementById("RadioButtonFFmpeg").checked) {
        fetch("api/StreamPlayer/StartFfplay", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: "" + portNo
        })
            .then(response => _checkIfFFplayStarted())
            .catch(error => console.error('Unable to start FFplay.', error));
    }   
}

function _checkIfVNCViewerReverseStarted()
{
    fetch("api/StreamPlayer/VncViewerStarted")
        .then(response => response.json())
        .then(successfullStart => _proceedToStopOrErrorPage(successfullStart) )
        .catch(error => console.error('Unable to verify if VNC-viewer has been started.', error));
}


function _checkIfFFplayStarted()
{
    fetch("api/StreamPlayer/FfplayStarted")
        .then(response => response.json())
        .then(successfullStart => _proceedToStopOrErrorPage(successfullStart))
        .catch(error => console.error('Unable to verify if FFplay has been started.', error));
}


function _proceedToStopOrErrorPage(successfullStart)
{
    if (successfullStart)
    {
        // Proceed to Stop-Page
        document.getElementById("FormToStartPlayer").setAttribute("hidden", "hidden");
        document.getElementById("FormToStopPlayer").removeAttribute("hidden");
        document.getElementById("ErrorPage").setAttribute("hidden", "hidden");

        // No more actions necessary (options of "ScreenResolutionStop" already present and
        // initial screen-resoltuion is preselected).
    }
    else
    {
        // Proceed to Error-Page
        document.getElementById("FormToStartPlayer").setAttribute("hidden", "hidden");
        document.getElementById("FormToStopPlayer").setAttribute("hidden", "hidden");
        document.getElementById("ErrorPage").removeAttribute("hidden");

        document.getElementById("ErrorMessage").innerHTML = "Could not start VNC-viewer/FFplay. Reason unknown, sorry!";

    }
}


// Called, when user pushes the Button "Stop Streaming" on the Stop-Page
function stopPlayer()
{
    // Change Screen-Resoltion of projecting-computer ( if the user doesn't change the
    // preselected value, this is the initial screen-resolution)
    var e = document.getElementById("ScreenResolutionStop");
    var resolution = e.options[e.selectedIndex].value;

    fetch("api/ScreenRes/SetScreenResolution", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: "\"" + resolution + "\""
    })
        .then(response => _stopAllPlayers())
        .catch(error => console.error('Unable to set screen-resolutions before stopping players.', error));

}


function _stopAllPlayers()
{
    // Stop Players and start again with start-page by calling ShowStartPlayerPage()
    fetch("api/StreamPlayer/StopAllStreamPlayers", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: ""
    })
        .then(response => showStartPlayerPage() )
        .catch(error => console.error('Unable to stop players.', error));
}

// Called, when user pushes the Button "Try again" on the Error-Page
function confirmError()
{
    // Just start 
    showStartPlayerPage();
}