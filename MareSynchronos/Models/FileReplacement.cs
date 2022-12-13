﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MareSynchronos.API;
using System.Text.RegularExpressions;
using MareSynchronos.FileCache;
using MareSynchronos.Managers;
using System;

namespace MareSynchronos.Models;

public class FileReplacement
{
    private readonly FileCacheManager fileDbManager;
    private readonly IpcManager ipcManager;

    public FileReplacement(FileCacheManager fileDbManager, IpcManager ipcManager)
    {
        this.fileDbManager = fileDbManager;
        this.ipcManager = ipcManager;
    }

    public bool Computed => IsFileSwap || !HasFileReplacement || !string.IsNullOrEmpty(Hash);

    public List<string> GamePaths { get; set; } = new();

    public bool HasFileReplacement => GamePaths.Count >= 1 && GamePaths.Any(p => !string.Equals(p, ResolvedPath, System.StringComparison.Ordinal));

    public bool IsFileSwap => !Regex.IsMatch(ResolvedPath, @"^[a-zA-Z]:(/|\\)", RegexOptions.ECMAScript) && !string.Equals(GamePaths.First(), ResolvedPath, System.StringComparison.Ordinal);

    public string Hash { get; private set; } = string.Empty;

    public string ResolvedPath { get; set; } = string.Empty;

    private void SetResolvedPath(string path)
    {
        ResolvedPath = path.ToLowerInvariant().Replace('\\', '/');
        if (!HasFileReplacement || IsFileSwap) return;

        _ = Task.Run(() =>
        {
            var cache = fileDbManager.GetFileCacheByPath(ResolvedPath)!;
            Hash = cache.Hash;
        });
    }

    public bool Verify()
    {
        if (!IsFileSwap)
        {
            var cache = fileDbManager.GetFileCacheByPath(ResolvedPath);
            if (cache == null) return false;
            Hash = cache.Hash;
            return true;
        }

        var resolvedPath = fileDbManager.ResolveFileReplacement(GamePaths.First());
        ResolvedPath = resolvedPath.ToLowerInvariant();

        return IsFileSwap;
    }

    public FileReplacementDto ToFileReplacementDto()
    {
        return new FileReplacementDto
        {
            GamePaths = GamePaths.ToArray(),
            Hash = Hash,
            FileSwapPath = IsFileSwap ? ResolvedPath : string.Empty
        };
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.AppendLine($"Modded: {HasFileReplacement} - {string.Join(",", GamePaths)} => {ResolvedPath}");
        return builder.ToString();
    }

    internal void ReverseResolvePath(string path)
    {
        GamePaths = ipcManager.PenumbraReverseResolvePlayer(path).ToList();
        SetResolvedPath(path);
    }

    internal void ResolvePath(string path)
    {
        GamePaths = new List<string> { path };
        SetResolvedPath(ipcManager.PenumbraResolvePath(path));
    }
}
