sessionStorage.setItem("mobileDevice", isDevice());
function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}

sessionStorage.setItem("extensionAvailable", isExtension());
function isExtension() {
    return !!window.elrondWallet;
}

sessionStorage.setItem("metaMaskAvailable", isMetaMask());
function isMetaMask() {
    return !!window.ethereum;
}

function getAccessToken() {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    return urlParams.get('accessToken');
}

function cancelTxToast() {
    new bootstrap.Toast(document.getElementById("canceledTransactionToast")).show();
}