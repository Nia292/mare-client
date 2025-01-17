﻿using MareSynchronos.API.Data;
using MareSynchronos.FileCache;
using MareSynchronos.Mediator;
using MareSynchronos.Utils;
using MareSynchronos.WebAPI;
using Microsoft.Extensions.Logging;

namespace MareSynchronos.Managers;

public class OnlinePlayerManager : MediatorSubscriberBase, IDisposable
{
    private readonly ApiController _apiController;
    private readonly DalamudUtil _dalamudUtil;
    private readonly FileCacheManager _fileDbManager;
    private readonly PairManager _pairManager;
    private CharacterData? _lastSentData;

    public OnlinePlayerManager(ILogger<OnlinePlayerManager> logger, ApiController apiController, DalamudUtil dalamudUtil,
        FileCacheManager fileDbManager, PairManager pairManager, MareMediator mediator) : base(logger, mediator)
    {
        _logger.LogTrace("Creating " + nameof(OnlinePlayerManager));
        _apiController = apiController;
        _dalamudUtil = dalamudUtil;
        _fileDbManager = fileDbManager;
        _pairManager = pairManager;

        Mediator.Subscribe<PlayerChangedMessage>(this, (msg) => PlayerManagerOnPlayerHasChanged((PlayerChangedMessage)msg));
        Mediator.Subscribe<DelayedFrameworkUpdateMessage>(this, (_) => FrameworkOnUpdate());
        Mediator.Subscribe<CharacterDataCreatedMessage>(this, (msg) =>
        {
            var newData = ((CharacterDataCreatedMessage)msg).CharacterData.ToAPI();
            if (_lastSentData == null || _lastSentData != null && !string.Equals(newData.DataHash.Value, _lastSentData.DataHash.Value, StringComparison.Ordinal))
            {
                _logger.LogDebug("Pushing data for visible players");
                _lastSentData = newData;
                PushCharacterData(_pairManager.VisibleUsers);
            }
            else
            {
                _logger.LogDebug("Not sending data for " + newData.DataHash.Value);
            }
        });
    }

    private void PlayerManagerOnPlayerHasChanged(PlayerChangedMessage msg)
    {
        PushCharacterData(_pairManager.VisibleUsers);
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    private void FrameworkOnUpdate()
    {
        if (!_dalamudUtil.IsPlayerPresent || !_apiController.IsConnected) return;

        var playerCharacters = _dalamudUtil.GetPlayerCharacters();
        var newVisiblePlayers = new List<UserData>();
        foreach (var pChar in playerCharacters)
        {
            var pair = _pairManager.FindPair(pChar);
            if (pair == null) continue;

            if (pair.InitializePair(pChar.Name.ToString()))
            {
                newVisiblePlayers.Add(pair.UserData ?? pair.GroupPair.First().Value.User);
            }
        }

        if (newVisiblePlayers.Any())
        {
            _logger.LogTrace("Has new visible players, pushing character data");
            PushCharacterData(newVisiblePlayers);
        }
    }

    private void PushCharacterData(List<UserData> visiblePlayers)
    {
        if (visiblePlayers.Any() && _lastSentData != null)
        {
            Task.Run(async () =>
            {
                await _apiController.PushCharacterData(_lastSentData, visiblePlayers).ConfigureAwait(false);
            });
        }
    }
}