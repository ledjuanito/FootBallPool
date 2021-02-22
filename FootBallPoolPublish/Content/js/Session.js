var sessionTimeout = parseInt(sessionTimeout) * 60 * 1000 + 1000;
var sessionTimeoutWarning = sessionTimeout - 60000; //Warn 1 minute before session timeout

var sessionTimeoutHandler = setTimeout('sessionEnd()', sessionTimeout);
var sessionTimeoutWarningHandler = setTimeout('sessionWarning()', sessionTimeoutWarning);

function sessionEnd() {
    $('#logoff-form').submit();
}

function sessionWarning() {
    $("#session-modal").modal("show");
}

function sessionContinue() {
    $.get(window.location, function (data) {
        window.clearTimeout(sessionTimeoutHandler);
        window.clearTimeout(sessionTimeoutWarningHandler);

        sessionTimeoutHandler = setTimeout('sessionEnd()', sessionTimeout);
        sessionTimeoutWarningHandler = setTimeout('sessionWarning()', sessionTimeoutWarning);
    });
}