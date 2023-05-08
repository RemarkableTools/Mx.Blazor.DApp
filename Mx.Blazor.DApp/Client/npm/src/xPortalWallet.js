import QRCode from "qrcode";
import { WalletConnectV2Provider } from "@multiversx/sdk-wallet-connect-provider";
import { Address, SignableMessage, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import {
    showConnectionError,
    hideConnectionError,
    signingMessageModal,
    signingMessageModalClose,
    signingModal,
    signingModalClose,
    cancelTxToast
} from "./common";

const relayUrl = "wss://relay.walletconnect.com";
const projectId = "17e2b91ffd21870bd04d7295e26453ad";

class XPortal {
    async init(chainId) {
        this.provider = new WalletConnectV2Provider(this.prepareCallbacks(), chainId, relayUrl, projectId);
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
                localStorage.setItem("wallettype", "2");
                DotNet.invokeMethodAsync('Mx.Blazor.DApp.Client', 'XPortalClientConnect', JSON.stringify({ address: self.provider.address, signature: self.provider.signature }));
            },
            onClientLogout: async () => {
                DotNet.invokeMethodAsync('Mx.Blazor.DApp.Client', 'XPortalClientDisconnect');
            },
            onClientEvent: (event) => {
                console.log('wc2 session event: ', event);
            }
        };
    }

    async login(authToken) {
        const { uri, approval } = await this.provider.connect();

        await openModal(uri + "&token=" + authToken);
        await this.provider.login({ approval: approval, token: authToken });
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

    async signMessage(message) {
        signingMessageModal("xPortal");

        const signableMessage = new SignableMessage({
            message: Buffer.from(message)
        });

        try {
            await this.provider.signMessage(signableMessage);
            return JSON.stringify(signableMessage.toJSON(), null, 4);
        }
        catch (err) {
            return "canceled";
        }
        finally {
            signingMessageModalClose();
        }
    }

    async signTransaction(transactionRequest) {
        signingModal("xPortal");

        const transaction = new Transaction({
            nonce: transactionRequest.nonce,
            value: transactionRequest.value,
            receiver: new Address(transactionRequest.receiver),
            sender: new Address(transactionRequest.sender),
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
        signingModal("xPortal");

        const transactions = transactionsRequest.map(transactionRequest =>
            new Transaction({
                nonce: transactionRequest.nonce,
                value: transactionRequest.value,
                receiver: new Address(transactionRequest.receiver),
                sender: new Address(transactionRequest.sender),
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

export const Obj = new XPortal();

async function openModal(uri) {
    const svg = await QRCode.toString(uri, { type: "svg" });

    $("#WalletConnectQRContainer").html(svg);
    $("#WalletConnectQRModal").modal("show");
    document.getElementById("WalletConnectUri").value = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://maiar.com/?wallet-connect=" + encodeURIComponent(uri);
    document.getElementById("WalletConnectUri").dispatchEvent(new Event('change'));
}

function closeModal() {
    $("#WalletConnectionsModal").modal("hide");
    $("#WalletConnectQRModal").modal("hide");
}
