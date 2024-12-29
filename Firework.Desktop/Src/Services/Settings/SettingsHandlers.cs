using System;
using System.Collections.Generic;
using System.Linq;
using Firework.Abstraction.Data;
using Firework.Core.Consts;
using Firework.Core.Settings;
using Firework.Models.Metadata;
using Firework.Models.Settings;

namespace Firework.Desktop.Services.Settings;

public class SettingsHandlers
{
    private readonly IDataRepository<Metadata> _metaRepository;
    private static Dictionary<string, Action> _handlers = new();

    public SettingsHandlers(IDataRepository<Metadata> metaRepository, Dictionary<string, SettingsItem> settings)
    {
        _metaRepository = metaRepository;
        SetSettingHandlers(settings);
    }

    public Action? GetSettingsHandler(string key) =>
        _handlers.TryGetValue(key, out var handler) ? handler : null;

    private void SetSettingHandlers(Dictionary<string, SettingsItem> settings)
    {
        var appName = _metaRepository.FindBy(x => x.Name == MetadataNames.APP_NAME);

        settings[nameof(SettingsDefault.Fields.AppAutoRun)].Handler = (arg) =>
        {
            var value = bool.Parse(arg);
            var handler = new AutoRunHandler(appName.Value);

            if (value)
            {
                handler.On();
            }
            else
            {
                handler.Off();
            }
        };
    }

}