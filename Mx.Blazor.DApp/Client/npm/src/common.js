export function showConnectionError() {
    document.getElementById("WalletConnectionsError").className = "d-block";
    document.getElementById("WalletConnectionsLoginApproval").className = "d-none";
}

export function hideConnectionError() {
    document.getElementById("WalletConnectionsError").className = "d-none";
    document.getElementById("WalletConnectionsLoginApproval").className = "d-none";
}

export function loginApproved() {
    document.getElementById("WalletConnectionsLoginApproval").className = "d-none";
}

export function loginNotApproved() {
    document.getElementById("WalletConnectionsLoginApproval").className = "d-block";
}

export function signingMessageModal(device) {
    $("#MessageSignatureConfirmDevice").html(device);
    $("#WalletMessageSignatureModal").modal("show");
}

export function signingMessageModalClose() {
    $("#MessageSignatureConfirmDevice").html("");
    $("#WalletMessageSignatureModal").modal("hide");
}

export function signingModal(device) {
    $("#SignatureConfirmDevice").html(device);
    $("#WalletSignatureModal").modal("show");
}

export function signingModalClose() {
    $("#SignatureConfirmDevice").html("");
    $("#WalletSignatureModal").modal("hide");
}

export function cancelTxToast() {
    new bootstrap.Toast(document.getElementById("canceledTransactionToast")).show();
}

export const isWindowAvailable = () =>
    typeof window != 'undefined' && typeof window?.location != 'undefined';

export function detectCurrentPlatform() {
    if (window?.ReactNativeWebView) {
        return PlatformsEnum.reactNative;
    }
    if (window.webkit) {
        return PlatformsEnum.ios;
    }
    return PlatformsEnum.web;
};

export const PlatformsEnum = {
    ios: 'ios',
    reactNative: 'reactNative',
    web: 'web'
}
