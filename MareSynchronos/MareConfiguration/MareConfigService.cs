﻿using Dalamud.Plugin;
using MareSynchronos.MareConfiguration.Configurations;
using Microsoft.Extensions.Logging;

namespace MareSynchronos.MareConfiguration;

public class MareConfigService : ConfigurationServiceBase<MareConfig>
{
    public const string ConfigName = "config.json";
    protected override string ConfigurationName => ConfigName;

    public MareConfigService(DalamudPluginInterface pluginInterface) : base(pluginInterface) { }
}
