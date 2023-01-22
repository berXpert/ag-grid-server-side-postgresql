const PROXY_CONFIG = [
    {
        context: [
            "/api"
        ],
        target: "http://localhost:5049",
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    }
]

module.exports = PROXY_CONFIG