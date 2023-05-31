﻿sessionStorage.setItem("mobiledevice", isDevice());
function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}

sessionStorage.setItem("extensionAvailable", isExtension());
function isExtension() {
    return !!window.elrondWallet;
}

function getAccessToken() {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    return urlParams.get('accessToken');
}