using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Firework.Abstraction.Data;
using Firework.Core.Settings;
using Firework.Models.Settings;
using Wpf.Ui.Input;

namespace Firework.Desktop.ViewModel;

public sealed class SettingsViewModel : INotifyPropertyChanged
{
    //private readonly Settings _settings;
    private readonly IDataRepository<SettingsItem> _settingsRepository;
    private bool _notifyInfoSend;
    private bool _autoRun;
    private bool _keepSilent;
    private bool _minimizeInTray;
    private bool _runInTray;
    public ICommand UpdateNotifyInfoSendCommand { get; } 
    public ICommand AutoRunCommand { get; }
    public ICommand KeepSilentCommand { get; }
    public ICommand MinimizeInTrayCommand { get; }
    public ICommand RunInTrayCommand { get; }

    public bool NotifyInfoSend
    {
        get => _notifyInfoSend;
        set
        {
            if (value == _notifyInfoSend) return;
            _notifyInfoSend = value;

            UpdateSettingItem(SettingsDefault.Names.NotifyInfoSend, value.ToString());

            OnPropertyChanged();
        }
    }

    public bool AutoRun
    {
        get => _autoRun;
        set
        {
            if (value == _autoRun) return;
            _autoRun = value;

            UpdateSettingItem(SettingsDefault.Names.AppAutoRun, value.ToString());
            OnPropertyChanged();
        }
    }

    public bool KeepSilent
    {
        get => _keepSilent;
        set
        {
            if (value == _keepSilent) return;
            _keepSilent = value;

            UpdateSettingItem(SettingsDefault.Names.KeepSilent, value.ToString());
            OnPropertyChanged();
        }
    }

    public bool MinimizeInTray
    {
        get => _minimizeInTray;
        set
        {
            if (value == _minimizeInTray) return;
            _minimizeInTray = value;

            UpdateSettingItem(SettingsDefault.Names.MinimizeInTray, value.ToString());
            OnPropertyChanged();
        }
    }

    public bool RunInTray
    {
        get => _runInTray;
        set
        {
            if (value == _runInTray) return;
            _runInTray = value;

            UpdateSettingItem(SettingsDefault.Names.RunAppInTray, value.ToString());
            OnPropertyChanged();
        }
    }

    public SettingsViewModel(IDataRepository<SettingsItem> settingsRepository)
    {
        _settingsRepository = settingsRepository;

        var ser = _settingsRepository.GetById(SettingsDefault.Names.NotifyInfoSend);

        NotifyInfoSend = bool.Parse(ser.Value);
        AutoRun = bool.Parse(_settingsRepository.GetById(SettingsDefault.Names.AppAutoRun).Value);
        KeepSilent = bool.Parse(_settingsRepository.GetById(SettingsDefault.Names.KeepSilent).Value);
        MinimizeInTray = bool.Parse(_settingsRepository.GetById(SettingsDefault.Names.MinimizeInTray).Value);
        RunInTray = bool.Parse(_settingsRepository.GetById(SettingsDefault.Names.RunAppInTray).Value);
        
        UpdateNotifyInfoSendCommand = new RelayCommand<bool>(x =>
        {
            NotifyInfoSend = x;
            //service.SetSettings(NotifyInfoSend.GetName(() => NotifyInfoSend), NotifyInfoSend);
        });
        AutoRunCommand = new RelayCommand<bool>(x =>
        {
            AutoRun = x;
            //service.SetSettings(AutoRun.GetName(() => AutoRun), AutoRun);
        });
        KeepSilentCommand = new RelayCommand<bool>(x =>
        {
            KeepSilent = x;
            //service.SetSettings(KeepSilent.GetName(() => KeepSilent), KeepSilent);
        });
        MinimizeInTrayCommand = new RelayCommand<bool>(x =>
        {
            MinimizeInTray = x;
            //service.SetSettings(MinimizeInTray.GetName(() => MinimizeInTray), MinimizeInTray);
        });
        RunInTrayCommand = new RelayCommand<bool>(x =>
        {
            RunInTray = x;
            //service.SetSettings(RunInTray.GetName(() => RunInTray), RunInTray);
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void UpdateSettingItem(string settingName, string value)
    {
        var oldValue = _settingsRepository.GetById(settingName);
        var newValue = (SettingsItem)oldValue.Clone();

        newValue.Value = value;

        _settingsRepository.Change(oldValue, newValue);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}