using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Reminders.Application.Services;

public class BlockchainSettings
{
    public string PrivateKey { get; set; } = string.Empty;
    public string NodeUrl { get; set; } = string.Empty;
    public string ContractAddress { get; set; } = string.Empty;
    public List<AbiItem> Abi { get; set; } = new();
}

public class AbiItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("inputs")]
    public List<AbiParameter> Inputs { get; set; } = new();

    [JsonPropertyName("outputs")]
    public List<AbiParameter> Outputs { get; set; } = new();

    [JsonPropertyName("stateMutability")]
    public string? StateMutability { get; set; }

    [JsonPropertyName("anonymous")]
    public bool? Anonymous { get; set; }
}

public class AbiParameter
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("internalType")]
    public string? InternalType { get; set; }

    [JsonPropertyName("indexed")]
    public bool? Indexed { get; set; }
}

public class ReminderContractFunctions
{
    public const string Create = "createReminder";
    public const string Get = "getReminder";
    public const string Update = "updateReminder";
    public const string Delete = "deleteReminder";
    public const string Count = "getReminderCount";
}

public class RemindersBlockchainService : IRemindersBlockchainService
{
    private readonly BlockchainSettings settings;
    private readonly Web3 web3;
    private readonly Contract contract;

    public RemindersBlockchainService(IOptions<BlockchainSettings> settings)
    {
        this.settings = settings.Value;

        web3 = new Web3(new Account(this.settings.PrivateKey), this.settings.NodeUrl);
        var abiJson = System.Text.Json.JsonSerializer.Serialize(this.settings.Abi, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        contract = web3.Eth.GetContract(abiJson, this.settings.ContractAddress);
    }

    public async Task<string> CreateReminderAsync(string text)
    {
        var createFunction = this.contract.GetFunction(ReminderContractFunctions.Create);

        var receipt = await createFunction.SendTransactionAndWaitForReceiptAsync(
            this.web3.TransactionManager.Account.Address,
            new HexBigInteger(300000),
            null,
            null,
            text
        );

        return receipt.TransactionHash;
    }

    public async Task<GetReminderOutput> GetReminderAsync(int id)
    {
        var count = await GetReminderCountAsync();

        if (id >= count || id < 0)
            return new GetReminderOutput();

        var getFunction = this.contract.GetFunction(ReminderContractFunctions.Get);

        var result = await getFunction.CallDeserializingToObjectAsync<GetReminderOutput>(id);

        return result;
    }

    public async Task<int> GetReminderCountAsync()
    {
        var countFunction = this.contract.GetFunction(ReminderContractFunctions.Count); // Add this to your Solidity contract

        var count = await countFunction.CallAsync<int>();

        return count;
    }

    public async Task<string> DeleteReminderAsync(int id)
    {
        var deleteFunction = this.contract.GetFunction(ReminderContractFunctions.Delete);

        var receipt = await deleteFunction.SendTransactionAndWaitForReceiptAsync(
            this.web3.TransactionManager.Account.Address,
            new HexBigInteger(300000),
            null,
            null,
            id
        );

        return receipt.TransactionHash;
    }

    public async Task<string> UpdateReminderAsync(int id, string text)
    {
        var updateFunction = this.contract.GetFunction(ReminderContractFunctions.Update);

        var receipt = await updateFunction.SendTransactionAndWaitForReceiptAsync(
            this.web3.TransactionManager.Account.Address,
            new HexBigInteger(300000),
            null,
            null,
            id,
            text
        );

        return receipt.TransactionHash;
    }
}