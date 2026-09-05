using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nethereum.Signer;
using Reminders.Application.Services;

namespace Reminders.Application.Test
{
    /// <summary>
    /// Exercises the blockchain service against a local JSON-RPC stub, so the read
    /// paths are covered without a node. Only successful calls are exercised: the
    /// circuit breaker is static, so injected failures would leak across tests.
    /// </summary>
    [TestClass]
    public class RemindersBlockchainServiceUnitTest
    {
        private const string TransactionHash = "0x" + "1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef";

        private const string OwnerAddress = "0x742d35cc6634c0532925a3b8d4c8fff00000dead";

        private JsonRpcStub stub;
        private RemindersBlockchainService service;

        [TestInitialize]
        public void TestInitialize()
        {
            stub = new JsonRpcStub(reminderCount: 2, reminderText: "Buy milk", owner: OwnerAddress);
            service = new RemindersBlockchainService(
                Mock.Of<ILogger<RemindersBlockchainService>>(),
                Options.Create(BuildSettings(stub.Url)));
        }

        [TestCleanup]
        public void TestCleanup() => stub.Dispose();

        [TestMethod]
        public async Task Should_GetReminderCount()
        {
            var count = await service.GetReminderCountAsync();

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task Should_GetReminder()
        {
            var output = await service.GetReminderAsync(0);

            Assert.AreEqual("Buy milk", output.Text);
            Assert.AreEqual(OwnerAddress, output.Owner.ToLowerInvariant());
        }

        [TestMethod]
        public async Task Should_GetEmptyReminderWhenIdIsOutOfRange()
        {
            var output = await service.GetReminderAsync(5);

            Assert.AreEqual(string.Empty, output.Text);
            Assert.AreEqual(string.Empty, output.Owner);
        }

        [TestMethod]
        public async Task Should_GetEmptyReminderWhenIdIsNegative()
        {
            var output = await service.GetReminderAsync(-1);

            Assert.AreEqual(string.Empty, output.Text);
            Assert.AreEqual(string.Empty, output.Owner);
        }

        [TestMethod]
        public async Task Should_CreateReminder()
        {
            var hash = await service.CreateReminderAsync("Buy milk");

            Assert.AreEqual(TransactionHash, hash);
        }

        [TestMethod]
        public async Task Should_UpdateReminder()
        {
            var hash = await service.UpdateReminderAsync(0, "Buy bread");

            Assert.AreEqual(TransactionHash, hash);
        }

        [TestMethod]
        public async Task Should_DeleteReminder()
        {
            var hash = await service.DeleteReminderAsync(0);

            Assert.AreEqual(TransactionHash, hash);
        }

        private static BlockchainSettings BuildSettings(string nodeUrl) =>
            new BlockchainSettings
            {
                PrivateKey = EthECKey.GenerateKey().GetPrivateKey(),
                NodeUrl = nodeUrl,
                ContractAddress = "0x5fbdb2315678afecb367f032d93f642f64180aa3",
                ChainId = 31337,
                Abi = new List<AbiItem>
                {
                    new AbiItem
                    {
                        Type = "function",
                        Name = "getReminderCount",
                        StateMutability = "view",
                        Outputs = new List<AbiParameter>
                        {
                            new AbiParameter { Name = string.Empty, Type = "uint256", InternalType = "uint256" }
                        }
                    },
                    new AbiItem
                    {
                        Type = "function",
                        Name = "createReminder",
                        StateMutability = "nonpayable",
                        Inputs = new List<AbiParameter>
                        {
                            new AbiParameter { Name = "text", Type = "string", InternalType = "string" }
                        }
                    },
                    new AbiItem
                    {
                        Type = "function",
                        Name = "updateReminder",
                        StateMutability = "nonpayable",
                        Inputs = new List<AbiParameter>
                        {
                            new AbiParameter { Name = "id", Type = "uint256", InternalType = "uint256" },
                            new AbiParameter { Name = "text", Type = "string", InternalType = "string" }
                        }
                    },
                    new AbiItem
                    {
                        Type = "function",
                        Name = "deleteReminder",
                        StateMutability = "nonpayable",
                        Inputs = new List<AbiParameter>
                        {
                            new AbiParameter { Name = "id", Type = "uint256", InternalType = "uint256" }
                        }
                    },
                    new AbiItem
                    {
                        Type = "function",
                        Name = "getReminder",
                        StateMutability = "view",
                        Inputs = new List<AbiParameter>
                        {
                            new AbiParameter { Name = "id", Type = "uint256", InternalType = "uint256" }
                        },
                        Outputs = new List<AbiParameter>
                        {
                            new AbiParameter { Name = "text", Type = "string", InternalType = "string" },
                            new AbiParameter { Name = "owner", Type = "address", InternalType = "address" }
                        }
                    }
                }
            };

        /// <summary>
        /// Minimal JSON-RPC endpoint answering the two eth_call shapes the read paths use.
        /// </summary>
        private sealed class JsonRpcStub : IDisposable
        {
            private readonly HttpListener listener = new HttpListener();
            private readonly string countResult;
            private readonly string reminderResult;

            public JsonRpcStub(int reminderCount, string reminderText, string owner)
            {
                countResult = "0x" + Word(reminderCount.ToString("x"));
                reminderResult = "0x"
                    + Word("40")
                    + Word(owner.Substring(2))
                    + Word(reminderText.Length.ToString("x"))
                    + PadRight(Convert.ToHexString(Encoding.UTF8.GetBytes(reminderText)).ToLowerInvariant());

                Url = $"http://localhost:{FreePort()}/";
                listener.Prefixes.Add(Url);
                listener.Start();
                _ = Task.Run(Listen);
            }

            public string Url { get; }

            public void Dispose() => listener.Close();

            private async Task Listen()
            {
                while (listener.IsListening)
                {
                    HttpListenerContext context;

                    try
                    {
                        context = await listener.GetContextAsync();
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    using var reader = new StreamReader(context.Request.InputStream);
                    var body = await reader.ReadToEndAsync();
                    var payload = Encoding.UTF8.GetBytes(Respond(body));

                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength64 = payload.Length;
                    await context.Response.OutputStream.WriteAsync(payload, 0, payload.Length);
                    context.Response.Close();
                }
            }

            private string Respond(string body)
            {
                using var request = JsonDocument.Parse(body);
                var root = request.RootElement;
                var id = root.GetProperty("id").GetRawText();
                var method = root.GetProperty("method").GetString();

                return $"{{\"jsonrpc\":\"2.0\",\"id\":{id},\"result\":{Result(method, root)}}}";
            }

            private string Result(string method, JsonElement root)
            {
                switch (method)
                {
                    case "eth_call":
                        var data = root.GetProperty("params")[0].GetProperty("data").GetString() ?? string.Empty;

                        // getReminder takes one argument, so its calldata carries an extra word.
                        return Quote(data.Length > 10 ? reminderResult : countResult);

                    case "eth_getTransactionReceipt":
                        return $"{{\"transactionHash\":\"{TransactionHash}\",\"status\":\"0x1\","
                            + "\"blockNumber\":\"0x1\",\"gasUsed\":\"0x5208\","
                            + "\"cumulativeGasUsed\":\"0x5208\",\"logs\":[]}";

                    case "eth_sendRawTransaction":
                        return Quote(TransactionHash);

                    case "eth_chainId":
                        return Quote("0x7a69");

                    case "eth_feeHistory":
                        return "{\"oldestBlock\":\"0x1\",\"reward\":[[\"0x3b9aca00\"]],"
                            + "\"baseFeePerGas\":[\"0x3b9aca00\",\"0x3b9aca00\"],\"gasUsedRatio\":[0.5]}";

                    case "eth_getBlockByNumber":
                        // No baseFeePerGas, so Nethereum signs a legacy transaction.
                        return "{\"number\":\"0x1\",\"transactions\":[]}";

                    default:
                        // Nonce, gas price and the rest of the fee lookups.
                        return Quote("0x1");
                }
            }

            private static string Quote(string value) => $"\"{value}\"";

            private static string Word(string hex) => hex.PadLeft(64, '0');

            private static string PadRight(string hex) => hex.PadRight((hex.Length + 63) / 64 * 64, '0');

            private static int FreePort()
            {
                var probe = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);

                probe.Start();

                var port = ((IPEndPoint)probe.LocalEndpoint).Port;

                probe.Stop();

                return port;
            }
        }
    }
}
