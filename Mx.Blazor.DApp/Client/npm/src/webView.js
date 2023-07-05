import { Address, SignableMessage, Transaction, TransactionPayload } from "@multiversx/sdk-core";
import {
    cancelTxToast,
    isWindowAvailable,
    detectCurrentPlatform,
    PlatformsEnum
} from "./common";

export const currentPlatform = detectCurrentPlatform();
export const targetOrigin = isWindowAvailable()
    ? window?.parent?.origin ?? '*'
    : '*';

export const handleWaitForMessage = (eventHandler) => {
    const handleMessageReceived = (event) => {
        let eventData = event.data;
        if (
            event.target.origin !== targetOrigin &&
            currentPlatform !== PlatformsEnum.reactNative
        ) {
            return;
        }
        try {
            eventData = JSON.parse(eventData);
            eventHandler(eventData);
        } catch (err) {
            console.error('error parsing response');
            throw err;
        }
    };
    if (document) {
        document.addEventListener('message', handleMessageReceived);
    }
    if (window) {
        window.addEventListener('message', handleMessageReceived);
    }
};

class WebView {
    async login() {
        try {
            requestMethods.login[currentPlatform]();
            const accessToken = new Promise((resolve) => {
                function handleTokenReceived(eventData) {
                    const { message, type } = eventData;
                    if (type === WebViewProviderResponseEnums.loginResponse) {
                        try {
                            const { accessToken, error } = message;
                            if (!error) {
                                resolve(accessToken);
                            } else {
                                throw error;
                            }
                        } catch (err) {
                            throw err;
                        }
                    }
                    if (document) {
                        document.removeEventListener('message', handleTokenReceived);
                    }
                }
                handleWaitForMessage(handleTokenReceived);
            });
            return await accessToken;
        } catch (err) {
            return "";
        }
    }

    getAddress() {
        return JSON.parse(localStorage.getItem("accountToken")).Address || "";
    }

    isConnected() {
        return !!getAddress();
    }

    async logout() {
        try {
            requestMethods.logout[currentPlatform]();
        } catch (err) {
        }
    }

    transactionCanceled() {
        cancelTxToast();
    }

    async signMessage(message) {
        const signableMessage = new SignableMessage({
            message: Buffer.from(message)
        });

        try {
            requestMethods.signMessage[currentPlatform](signableMessage);
            const signedMessage = new Promise((resolve) => {
                function handleSignMessageResponse(eventData) {
                    const { message, type } = eventData;
                    if (type === WebViewProviderResponseEnums.signMessageResponse) {
                        try {
                            const { signature, error } = message;
                            if (!error) {
                                resolve(signature);
                            } else {
                                throw error;
                            }
                        } catch (err) {
                            throw err;
                        }
                    }
                    if (document) {
                        document.removeEventListener('message', handleSignMessageResponse);
                    }
                }
                handleWaitForMessage(handleSignMessageResponse);
            });
            return await signedMessage;
        }
        catch (err) {
            return "canceled";
        }
    }

    async signTransaction(transactionRequest) {
        const transaction = new Transaction({
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
        });

        try {
            const signedTransactions = await this.signTransactions([transaction]);
            return JSON.stringify(JSON.parse(signedTransactions)[0], null, 4);
        }
        catch (err) {
            return "canceled";
        }
    }

    async signTransactions(transactionsRequest) {
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
            const plainTransactions = transactions.map((tx) => tx.toPlainObject());
            requestMethods.signTransactions[currentPlatform](plainTransactions);

            const signedTransactionsResponse = new Promise((resolve) => {
                window.transactionsSigned = (txs, error) => {
                    txs = JSON.parse(txs);
                    if (error) {
                        window.transactionsSigned = null;
                        throw error;
                    }
                    resolve(txs.map((tx) => Transaction.fromPlainObject(tx)));
                    window.transactionsSigned = null;
                };

                function handleSignResponse(eventData) {
                    const { message, type } = eventData;
                    if (type === WebViewProviderResponseEnums.signTransactionsResponse) {
                        const { transactions, error } = message;

                        try {
                            if (!error) {
                                resolve(transactions.map((tx) => Transaction.fromPlainObject(tx)));
                            } else {
                                throw error;
                            }
                        } catch (err) {
                            throw err;
                        }
                    }
                    if (document) {
                        document.removeEventListener('message', handleSignResponse);
                    }
                }
                handleWaitForMessage(handleSignResponse);
            });
            var signedTransactions = await signedTransactionsResponse;
            return JSON.stringify(signedTransactions.map(transaction => transaction.toSendable()), null, 4);
        }
        catch (err) {
            return "canceled";
        }
    }
}

export const Obj = new WebView();

export const WebViewProviderRequestEnums = {
    signTransactionsRequest: 'SIGN_TRANSACTIONS_REQUEST',
    signMessageRequest: 'SIGN_MESSAGE_REQUEST',
    loginRequest: 'LOGIN_REQUEST',
    logoutRequest: 'LOGOUT_REQUEST'
}
export const WebViewProviderResponseEnums = {
    signTransactionsResponse: 'SIGN_TRANSACTIONS_RESPONSE',
    signMessageResponse: 'SIGN_MESSAGE_RESPONSE',
    loginResponse: 'LOGIN_RESPONSE'
}

export const requestMethods = {
    signTransactions: {
        [PlatformsEnum.ios]: (transactions) =>
            window.webkit.messageHandlers.signTransactions.postMessage(
                transactions,
                targetOrigin
            ),
        [PlatformsEnum.reactNative]: (message) =>
            window.ReactNativeWebView?.postMessage(
                JSON.stringify({
                    type: WebViewProviderRequestEnums.signTransactionsRequest,
                    message
                })
            ),
        [PlatformsEnum.web]: (message) =>
            window.postMessage(
                JSON.stringify({
                    type: WebViewProviderRequestEnums.signTransactionsRequest,
                    message
                }),
                targetOrigin
            )
    },
    signMessage: {
        [PlatformsEnum.ios]: (message) =>
            window.webkit.messageHandlers.signMessage.postMessage(message),
        [PlatformsEnum.reactNative]: (message) =>
            window.ReactNativeWebView?.postMessage(
                JSON.stringify({
                    type: WebViewProviderRequestEnums.signMessageRequest,
                    message
                })
            ),
        [PlatformsEnum.web]: (message) =>
            window.postMessage(
                JSON.stringify({
                    type: WebViewProviderRequestEnums.signMessageRequest,
                    message
                }),
                targetOrigin
            )
    },
    logout: {
        [PlatformsEnum.ios]: () =>
            window.webkit.messageHandlers.logout.postMessage(),
        [PlatformsEnum.reactNative]: () =>
            window.ReactNativeWebView?.postMessage(
                JSON.stringify({
                    type: WebViewProviderRequestEnums.logoutRequest
                })
            ),
        [PlatformsEnum.web]: () =>
            window.postMessage(
                JSON.stringify({
                    type: WebViewProviderRequestEnums.logoutRequest
                }),
                targetOrigin
            )
    },
    login: {
        [PlatformsEnum.ios]: () =>
            window.webkit.messageHandlers.login.postMessage(),
        [PlatformsEnum.reactNative]: () =>
            window.ReactNativeWebView?.postMessage(
                JSON.stringify({
                    type: WebViewProviderRequestEnums.loginRequest
                })
            ),
        [PlatformsEnum.web]: () =>
            window.postMessage(
                JSON.stringify({
                    type: WebViewProviderRequestEnums.loginRequest
                }),
                targetOrigin
            )
    }
};