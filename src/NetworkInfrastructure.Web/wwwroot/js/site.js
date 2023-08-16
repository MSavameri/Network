let x = getCookie('toggle')

if (x) {
    document.documentElement.setAttribute('data-bs-theme', x)
}

document.getElementById('btnSwitch').addEventListener('click', () => {
    if (document.documentElement.getAttribute('data-bs-theme') == 'light') {
        document.documentElement.setAttribute('data-bs-theme', 'dark')
        eraseCookie('toggle')
        setCookie('toggle', 'dark', 7);
        document.getElementById('spnSwitch').className = 'bi bi-sun'
    }
    else {
        document.documentElement.setAttribute('data-bs-theme', 'light')
        eraseCookie('toggle')
        setCookie('toggle', 'light', 7);
        document.getElementById('spnSwitch').className = 'bi bi-moon-fill'
    }
})

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}