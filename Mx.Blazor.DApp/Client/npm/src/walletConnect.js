import QRCode from "qrcode";
import { WalletConnectProvider } from "@multiversx/sdk-wallet-connect-provider";
import { Address, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import {
    showConnectionError,
    hideConnectionError,
    signingModal,
    signingModalClose,
    cancelTxToast
} from "./common";

const bridgeUrl = "https://bridge.walletconnect.org";

class WalletConnect {
    async init() {
        this.provider = new WalletConnectProvider(bridgeUrl, this.prepareCallbacks());
        var initialized = await this.provider.init();
        if (!initialized)
            showConnectionError();
        else
            hideConnectionError();

        return initialized;
    }

    prepareCallbacks() {
        const self = this;

        return {
            onClientLogin: async () => {
                closeModal();
                sessionStorage.setItem("wallettype", "5");
                DotNet.invokeMethodAsync('Mx.Blazor.DApp.Client', 'MaiarClientConnect', JSON.stringify({ address: self.provider.address, signature: self.provider.signature }));
            },
            onClientLogout: async () => {
                DotNet.invokeMethodAsync('Mx.Blazor.DApp.Client', 'MaiarClientDisconnect');
            }
        };
    }

    async login(authToken) {
        const connectorUri = await this.provider.login();
        await openModal(connectorUri + "&token=" + authToken);
    }

    async getAddress() {
        return await this.provider.getAddress();
    }

    async isConnected() {
        return await this.provider.isConnected();
    }

    async logout() {
        await this.provider.logout();
    }

    transactionCanceled() {
        cancelTxToast();
    }

    async signTransaction(transactionRequest) {
        signingModal("Maiar");

        const transaction = new Transaction({
            nonce: transactionRequest.nonce,
            value: transactionRequest.value,
            receiver: new Address(transactionRequest.receiver),
            gasPrice: transactionRequest.gasPrice,
            gasLimit: transactionRequest.gasLimit,
            data: new TransactionPayload(transactionRequest.data),
            chainID: transactionRequest.chainID,
            version: transactionRequest.transactionVersion
        });

        try {
            await this.provider.signTransaction(transaction);
            return JSON.stringify(transaction.toSendable(), null, 4);
        }
        catch (err) {
            return "canceled";
        }
        finally {
            signingModalClose();
        }
    }

    async signTransactions(transactionsRequest) {
        signingModal("Maiar");

        const transactions = transactionsRequest.map(transactionRequest =>
            new Transaction({
                nonce: transactionRequest.nonce,
                value: transactionRequest.value,
                receiver: new Address(transactionRequest.receiver),
                gasPrice: transactionRequest.gasPrice,
                gasLimit: transactionRequest.gasLimit,
                data: new TransactionPayload(transactionRequest.data),
                chainID: transactionRequest.chainID,
                version: transactionRequest.transactionVersion
            })
        );

        try {
            await this.provider.signTransactions(transactions);
            return JSON.stringify(transactions.map(transaction => transaction.toSendable()), null, 4);
        }
        catch (err) {
            return "canceled";
        }
        finally {
            signingModalClose();
        }
    }
}

export const Obj = new WalletConnect();

async function openModal(connectorUri) {
    const svg = await QRCode.toString(connectorUri, { type: "svg" });

    $("#WalletConnectQRContainer").html(svg);
    $("#WalletConnectQRModal").modal("show");
    document.getElementById("WalletConnectUri").value = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com/?wallet-connect=" + encodeURIComponent(connectorUri);
    document.getElementById("WalletConnectUri").dispatchEvent(new Event('change'));
}

function closeModal() {
    $("#WalletConnectionsModal").modal("hide");
    $("#WalletConnectQRModal").modal("hide");
}
