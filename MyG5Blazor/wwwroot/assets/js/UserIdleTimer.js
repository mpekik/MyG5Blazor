//
// Shout out to equiman for his answer on stackoverflow.com that informed this solution
//

// Currently can't figure out how to make the callback to a static method
// and be able to force the update via the NavigationManager - so I am 
// using the object reference for the main layout to test and learn more
//
var theMainLayoutObjectRef_G;

var theUserIdleTimer_G = null;
var iTimeoutLength_G = 1000
var timerEnabled_G = false;

function setupUserIdleTimer(theMainLayout, iTimeoutLength, timerEnabled) {
    theMainLayoutObjectRef_G = theMainLayout;
    iTimeoutLength_G = iTimeoutLength;
    timerEnabled_G = timerEnabled;
    console.log("setupUserIdleTimer(" + iTimeoutLength_G.toString() + ")");

    // Hook into DOM Events that would mean someone is actually 
    // interacting with the UI, skipping onmousemove as this will 
    // be a touch screen anyway 
    //
    document.onkeypress = resetUserIdleTimer;
    document.onmousedown = resetUserIdleTimer;
    document.ontouchstart = resetUserIdleTimer;
    document.onclick = resetUserIdleTimer;
    document.onkeypress = resetUserIdleTimer;

    // Start the timer
    //
    resetUserIdleTimer();
}

function userTimerExpired() {
    console.log("userTimerExpired()");

    // Non-static call - that I can actually get to compile and work.
    //
    if (timerEnabled_G == true) {
        theMainLayoutObjectRef_G.invokeMethodAsync('theIdleTimeoutFired');
    }
    timerEnabled_G = false;
    // TODO: How do I access the NavigationManager from a staic method?
    //
    // If theIdleTimeoutFired() were static then the following
    //
    // DotNet.invokeMethodAsync('20191008 - BlazorTestApp', 'theIdleTimeoutFired');

    // Keep doing what we are doing, processing logic for what to do with this info
    // will be on the rendering side.  e.g. It we are already on the welcome page 
    // nothing to do, etc.
    //
    //resetUserIdleTimer();
}

function resetUserIdleTimer() {
    console.log("resetUserIdleTimer()");

    if (theUserIdleTimer_G) clearTimeout(theUserIdleTimer_G);

    theUserIdleTimer_G = setTimeout(userTimerExpired, iTimeoutLength_G)
}

//
// TODO: Add start and stop timer calls for when user videos are actively playing.
//       Logic for keeping track of the video player state will be in the VideoPlayerController
//