import QRCode from "qrcode";
import {WalletConnectV2Provider} from "@multiversx/sdk-wallet-connect-provider/out";
import {Address, Message, Transaction, TransactionPayload} from "@multiversx/sdk-core/out";
import {
    showConnectionError,
    hideConnectionError,
    signingMessageModal,
    signingMessageModalClose,
    signingModal,
    signingModalClose
} from "./common";

const relayUrl = "wss://relay.walletconnect.com";
const projectId = "17e2b91ffd21870bd04d7295e26453ad";

class XPortal {
    async init(chainId) {
        this.provider = new WalletConnectV2Provider(this.prepareCallbacks(), chainId, relayUrl, projectId);
        const initialized = await this.provider.init();
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
            },
            onClientLogout: async () => {
            },
            onClientEvent: (event) => {
            }
        };
    }

    async login(authToken) {
        const {uri, approval} = await this.provider.connect({
            methods: ["mvx_signNativeAuthToken", "mvx_cancelAction"],
        });
        await openModal(uri + "&token=" + authToken);

        let account = await this.provider.login({approval: approval, token: authToken});

        if (this.isConnected()) {
            closeModal();
            localStorage.setItem("walletType", "2");
        }

        return account;
    }

    isConnected() {
        return this.provider.isConnected() && this.provider.account.signature;
    }

    async logout() {
        await this.provider.logout();
    }

    async signMessage(message) {
        signingMessageModal("xPortal");

        try {
            let signedMessage = await this.provider.signMessage(
                new Message({
                    data: Buffer.from(message)
                })
            );

            return signedMessage.signature.toString("hex")
        } catch (err) {
            return null;
        } finally {
            signingMessageModalClose();
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
                version: transactionRequest.transactionVersion,
                options: transactionRequest.options,
                guardian: new Address(transactionRequest.guardian)
            })
        );

        try {
            const signedTransactions = await this.provider.signTransactions(transactions);
            return JSON.stringify(signedTransactions.map(transaction => transaction.toSendable()), null, 4);
        } catch (err) {
            return null;
        } finally {
            signingModalClose();
        }
    }
}

export const
    Obj = new XPortal();

async function

openModal(uri) {
    const svg = await QRCode.toString(uri, {type: "svg"});

    $("#WalletConnectQRContainer").html(svg);
    $("#WalletConnectQRModal").modal("show");
    document.getElementById("WalletConnectUri").value = "https://maiar.page.link/?apn=com.elrond.maiar.wallet&isi=1519405832&ibi=com.elrond.maiar.wallet&link=https://xportal.com/?wallet-connect=" + encodeURIComponent(uri);
    document.getElementById("WalletConnectUri").dispatchEvent(new Event('change'));
}

function

closeModal() {
    $("#WalletConnectionsModal").modal("hide");
    $("#WalletConnectQRModal").modal("hide");
}
