﻿using Dalamud.Interface.Internal.Notifications;
using MareSynchronos.Models;

namespace MareSynchronos.Mediator;

#pragma warning disable MA0048 // File name must match type name
public record SwitchToIntroUiMessage : IMessage;
public record SwitchToMainUiMessage : IMessage;
public record OpenSettingsUiMessage : IMessage;
public record DalamudLoginMessage : IMessage;
public record DalamudLogoutMessage : IMessage;
public record FrameworkUpdateMessage : IMessage;
public record ClassJobChangedMessage : IMessage;
public record DelayedFrameworkUpdateMessage : IMessage;
public record ZoneSwitchStartMessage : IMessage;
public record ZoneSwitchEndMessage : IMessage;
public record CutsceneStartMessage : IMessage;
public record GposeStartMessage : IMessage;
public record GposeEndMessage : IMessage;
public record CutsceneEndMessage : IMessage;
public record CutsceneFrameworkUpdateMessage : IMessage;
public record ConnectedMessage : IMessage;
public record DisconnectedMessage : IMessage;
public record PenumbraModSettingChangedMessage : IMessage;
public record PenumbraInitializedMessage : IMessage;
public record PenumbraDisposedMessage : IMessage;
public record PenumbraRedrawMessage(IntPtr Address, int ObjTblIdx, bool WasRequested) : IMessage;
public record HeelsOffsetMessage(float Offset) : IMessage;
public record PenumbraResourceLoadMessage(IntPtr GameObject, string GamePath, string FilePath) : IMessage;
public record CustomizePlusMessage(string? Data) : IMessage;
public record PalettePlusMessage(string? Data) : IMessage;
public record PlayerChangedMessage(API.Data.CharacterData Data) : IMessage;
public record CharacterChangedMessage(GameObjectHandler GameObjectHandler) : IMessage;
public record TransientResourceChangedMessage(IntPtr Address) : IMessage;
public record AddWatchedGameObjectHandler(GameObjectHandler Handler) : IMessage;
public record RemoveWatchedGameObjectHandler(GameObjectHandler Handler) : IMessage;
public record HaltScanMessage(string Source) : IMessage;
public record ResumeScanMessage(string Source) : IMessage;
public record NotificationMessage
    (string Title, string Message, NotificationType Type, uint TimeShownOnScreen = 3000) : IMessage;
public record CreateCacheForObjectMessage(GameObjectHandler ObjectToCreateFor) : IMessage;
public record ClearCacheForObjectMessage(GameObjectHandler ObjectToCreateFor) : IMessage;
public record CharacterDataCreatedMessage(CharacterData CharacterData) : IMessage;
public record PenumbraStartRedrawMessage(IntPtr Address) : IMessage;
public record PenumbraEndRedrawMessage(IntPtr Address) : IMessage;
public record HubReconnectingMessage(Exception? Exception) : IMessage;
public record HubReconnectedMessage(string? Arg) : IMessage;
public record HubClosedMessage(Exception? Exception) : IMessage;
#pragma warning restore MA0048 // File name must match type name