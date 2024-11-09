import {HWProvider} from "@multiversx/sdk-hw-provider/out";
import {Address, Message, Transaction, TransactionPayload} from "@multiversx/sdk-core/out";
import {
    loginApproved,
    loginNotApproved,
    signingMessageModal,
    signingMessageModalClose,
    signingModal,
    signingModalClose
} from "./common";

class HardwareWallet {
    async init() {
        this.provider = new HWProvider();
        return await this.provider.init();
    }

    async prepLedger() {
        $("#LedgerConnectModal").modal("show");
        return await this.init();
    }

    async getLedgerAddresses(pageNumber, pageSize) {
        try {
            return await this.provider.getAccounts(pageNumber, pageSize);
        } catch {
            return null;
        }
    }

    async login(authToken) {
        const payloadToSign = Buffer.from(`${authToken}{}`);
        const addressIndex = document.getElementById("LedgerAddressIndex").value;

        try {
            const {address, signature} = await this.provider.tokenLogin({
                addressIndex: addressIndex,
                token: payloadToSign
            });

            loginApproved();
            localStorage.setItem("walletType", "3");
            $("#LedgerConnectModal").modal("hide");
            $("#WalletConnectionsModal").modal("hide");
            return JSON.stringify({
                address: address || "",
                signature: signature.toString('hex')
            });
        } catch (err) {
            loginNotApproved();
            $("#LedgerConnectModal").modal("hide");
            return JSON.stringify({
                address: "",
                signature: null
            });
        }
    }

    async logout() {
        await this.provider.logout();
    }

    async signMessage(message) {
        signingMessageModal("Ledger");
        const address = await this.provider.getAddress();

        try {
            let signedMessage = await this.provider.signMessage(
                new Message({
                    address: address,
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
        signingModal("Ledger");

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

export const Obj = new HardwareWallet();
