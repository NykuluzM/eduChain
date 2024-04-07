using System;
using System.Threading.Tasks;
using WalletConnectSharp.Core;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Web3Wallet;

namespace eduChain.Models
{
    public class WalletConnectManager
    {
        private static WalletConnectManager _instance;
        private WalletConnectCore _walletConnectCore;
        private Metadata _walletMetadata;
        private Web3WalletClient _web3Client;

        private WalletConnectManager()
        {
            // Initialize WalletConnectCore
            var options = new CoreOptions
            {
                ProjectId = "d7f8a6b9e0bec2ef642d074727689d10", // Your WalletConnect project ID
                Name = "eduChain",
                RelayUrl = "https://relay.walletconnect.org", // WalletConnect relay server URL
            };
            _walletConnectCore = new WalletConnectCore(options);

            // Initialize Metadata
            _walletMetadata = new Metadata
            {
                Description = "An example wallet to showcase WalletConnectSharpv2",
                Icons = new[] { "https://walletconnect.com/meta/favicon.ico" },
                Name = "wallet-csharp-test",
                Url = "https://walletconnect.com",
            };
            InitializeWeb3WalletClient().Wait();

        }

        private async Task InitializeWeb3WalletClient()
        {
            _web3Client = await Web3WalletClient.Init(_walletConnectCore, _walletMetadata, _walletMetadata.Name);
        }
        public static WalletConnectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WalletConnectManager();
                }
                return _instance;
            }
        }
    }
}
