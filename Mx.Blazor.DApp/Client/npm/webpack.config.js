const path = require("path");
const webpack = require("webpack");

module.exports = [
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "extensionWallet.js",
            library: "ExtensionWallet"
        },
        name: "ExtensionWallet",
        entry: "./src/extensionWallet.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ['buffer', 'Buffer'],
            })
        ],
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "xPortalWallet.js",
            library: "XPortalWallet"
        },
        name: "XPortalWallet",
        entry: "./src/xPortalWallet.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "webWallet.js",
            library: "WebWallet"
        },
        name: "WebWallet",
        entry: "./src/webWallet.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ['buffer', 'Buffer'],
            })
        ],
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "hardwareWallet.js",
            library: "HardwareWallet"
        },
        name: "HardwareWallet",
        entry: "./src/hardwareWallet.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ['buffer', 'Buffer'],
            })
        ],
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "webView.js",
            library: "WebView"
        },
        name: "WebView",
        entry: "./src/webView.js"
    },
    {
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({
                Buffer: ['buffer', 'Buffer'],
            })
        ],
        output: {
            path: path.resolve(__dirname, '../wwwroot/js/npm'),
            filename: "crossWindowWallet.js",
            library: "CrossWindowWallet"
        },
        name: "CrossWindowWallet",
        entry: "./src/crossWindowWallet.js"
    }
];