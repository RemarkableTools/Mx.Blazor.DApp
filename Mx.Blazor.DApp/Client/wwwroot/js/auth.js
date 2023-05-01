let timer;
let minutes;

function initInactivityTimer(mins) {
    minutes = mins;

    document.addEventListener("mousedown", resetTimer);
    resetTimer();
}

function resetTimer() {
    clearTimeout(timer);
    timer = setTimeout(disconnect, minutes * 60 * 1000);
}

function disconnect() {
    document.removeEventListener("mousedown", resetTimer);
    DotNet.invokeMethodAsync('Mx.Blazor.DApp.Client', 'DisconnectByInactivity');
}

function removeInactivityTimer() {
    document.removeEventListener("mousedown", resetTimer);
    clearTimeout(timer);
}
