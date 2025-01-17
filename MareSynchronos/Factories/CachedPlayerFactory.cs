﻿using MareSynchronos.API.Dto.User;
using MareSynchronos.FileCache;
using MareSynchronos.Managers;
using MareSynchronos.Mediator;
using MareSynchronos.Utils;
using MareSynchronos.WebAPI;
using Microsoft.Extensions.Logging;

namespace MareSynchronos.Factories;

public class CachedPlayerFactory
{
    private readonly IpcManager _ipcManager;
    private readonly DalamudUtil _dalamudUtil;
    private readonly FileCacheManager _fileCacheManager;
    private readonly GameObjectHandlerFactory _gameObjectHandlerFactory;
    private readonly MareMediator _mediator;
    private readonly ILoggerFactory _loggerFactory;

    public CachedPlayerFactory(IpcManager ipcManager, DalamudUtil dalamudUtil, FileCacheManager fileCacheManager,
        GameObjectHandlerFactory gameObjectHandlerFactory,
        MareMediator mediator, ILoggerFactory loggerFactory)
    {
        _ipcManager = ipcManager;
        _dalamudUtil = dalamudUtil;
        _fileCacheManager = fileCacheManager;
        _gameObjectHandlerFactory = gameObjectHandlerFactory;
        _mediator = mediator;
        _loggerFactory = loggerFactory;
    }

    public CachedPlayer Create(OnlineUserIdentDto dto, ApiController apiController)
    {
        return new CachedPlayer(_loggerFactory.CreateLogger<CachedPlayer>(), dto, _gameObjectHandlerFactory, _ipcManager, apiController, _dalamudUtil, _fileCacheManager, _mediator);
    }
}
