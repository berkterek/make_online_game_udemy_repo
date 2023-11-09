using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;

public class MultiplayAllocationService : System.IDisposable
{
    private IMultiplayService multiplayService;
    private MultiplayEventCallbacks serverCallbacks;
    private IServerQueryHandler serverCheckManager;
    private IServerEvents serverEvents;
    private CancellationTokenSource serverCheckCancel;
    string allocationId;

    public MultiplayAllocationService()
    {
        try
        {
            multiplayService = MultiplayService.Instance;
            serverCheckCancel = new CancellationTokenSource();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Error creating Multiplay allocation service.\n{ex}");
        }
    }

    public async UniTask<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation()
    {
        if (multiplayService == null) { return null; }

        allocationId = null;
        serverCallbacks = new MultiplayEventCallbacks();
        serverCallbacks.Allocate += OnMultiplayAllocation;
        serverEvents = await multiplayService.SubscribeToServerEventsAsync(serverCallbacks);

        string allocationID = await AwaitAllocationID();
        MatchmakingResults matchmakingPayload = await GetMatchmakerAllocationPayloadAsync();

        return matchmakingPayload;
    }

    private async UniTask<string> AwaitAllocationID()
    {
        ServerConfig config = multiplayService.ServerConfig;
        Debug.Log($"Awaiting Allocation. Server Config is:\n" +
            $"-ServerID: {config.ServerId}\n" +
            $"-AllocationID: {config.AllocationId}\n" +
            $"-Port: {config.Port}\n" +
            $"-QPort: {config.QueryPort}\n" +
            $"-logs: {config.ServerLogDirectory}");

        while (string.IsNullOrEmpty(allocationId))
        {
            string configID = config.AllocationId;

            if (!string.IsNullOrEmpty(configID) && string.IsNullOrEmpty(allocationId))
            {
                Debug.Log($"Config had AllocationID: {configID}");
                allocationId = configID;
            }

            await UniTask.Delay(100);
        }

        return allocationId;
    }

    private async UniTask<MatchmakingResults> GetMatchmakerAllocationPayloadAsync()
    {
        MatchmakingResults payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
        string modelAsJson = JsonConvert.SerializeObject(payloadAllocation, Formatting.Indented);
        Debug.Log(nameof(GetMatchmakerAllocationPayloadAsync) + ":" + System.Environment.NewLine + modelAsJson);
        return payloadAllocation;
    }

    private void OnMultiplayAllocation(MultiplayAllocation allocation)
    {
        Debug.Log($"OnAllocation: {allocation.AllocationId}");

        if (string.IsNullOrEmpty(allocation.AllocationId)) { return; }

        allocationId = allocation.AllocationId;
    }

    public async UniTask BeginServerCheck()
    {
        if (multiplayService == null) { return; }

        serverCheckManager = await multiplayService.StartServerQueryHandlerAsync((ushort)20, "", "", "0", "");

        ServerCheckLoop(serverCheckCancel.Token);
    }

    public void SetServerName(string name)
    {
        serverCheckManager.ServerName = name;
    }
    public void SetBuildID(string id)
    {
        serverCheckManager.BuildId = id;
    }

    public void SetMaxPlayers(ushort players)
    {
        serverCheckManager.MaxPlayers = players;
    }

    public void AddPlayer()
    {
        serverCheckManager.CurrentPlayers++;
    }

    public void RemovePlayer()
    {
        serverCheckManager.CurrentPlayers--;
    }

    public void SetMap(string newMap)
    {
        serverCheckManager.Map = newMap;
    }

    public void SetMode(string mode)
    {
        serverCheckManager.GameType = mode;
    }

    private async void ServerCheckLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            serverCheckManager.UpdateServerCheck();
            await UniTask.Delay(100,DelayType.DeltaTime,PlayerLoopTiming.Update,cancellationToken);
        }
    }

    private void OnMultiplayDeAllocation(MultiplayDeallocation deallocation)
    {
        Debug.Log(
                $"Multiplay Deallocated : ID: {deallocation.AllocationId}\nEvent: {deallocation.EventId}\nServer{deallocation.ServerId}");
    }

    private void OnMultiplayError(MultiplayError error)
    {
        Debug.Log($"MultiplayError : {error.Reason}\n{error.Detail}");
    }

    public void Dispose()
    {
        if (serverCallbacks != null)
        {
            serverCallbacks.Allocate -= OnMultiplayAllocation;
            serverCallbacks.Deallocate -= OnMultiplayDeAllocation;
            serverCallbacks.Error -= OnMultiplayError;
        }

        if (serverCheckCancel != null)
        {
            serverCheckCancel.Cancel();
        }

        serverEvents?.UnsubscribeAsync();
    }
}