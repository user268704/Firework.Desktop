using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firework.Abstraction.Connection;
using Firework.Abstraction.Data;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher; 
using Firework.Dto.Instructions;
using Firework.Dto.Results;
using Firework.Models.Data;
using Firework.Models.Server;

namespace Firework.Desktop.ViewModel;

public sealed partial class NetworkManagerViewModel : ObservableObject
{
    private readonly IMacroLauncher _launcher;
    private readonly IInstructionService _instructionService;
    private readonly IConnectionManager _connectionManager;
    private readonly IDataRepository<SettingsItem> _settingsRepository;
    public string FullHost { get; set; }

    [ObservableProperty] private string commandText;
    [ObservableProperty] private string commandResult;

    public NetworkManagerViewModel(IMacroLauncher launcher,
        IInstructionService instructionService,
        /*IConnectionManager connectionManager*/
        IDataRepository<SettingsItem> settingsRepository)
    {
        _launcher = launcher;
        _instructionService = instructionService;
        //_connectionManager = connectionManager;
        _settingsRepository = settingsRepository;

        //_connectionManager.OnConnectionChanged += OnConnectionInfoChanged;
        FullHost = GetFullHost();
    }

    private void OnConnectionInfoChanged(object? sender, ConnectionInfo e)
    {
        FullHost = GetFullHost();
    }

    private string GetFullHost()
    {
        /*string host = _settingsRepository
            .GetAll()
            .First(x => x.UniqueKey == SettingsDefault.Names.LocalHost)
            .Value;

        string port = _settingsRepository
            .GetAll()
            .First(x => x.UniqueKey == SettingsDefault.Names.LocalPort)
            .Value;
            */


        return $"undefined";
    }

    [RelayCommand]
    public void StopReceivingCommand()
    {
        throw new NotImplementedException();

        //_apiManager.StopReceivingCommand();
    }

    /// <summary>
    ///     Stop info flow
    /// </summary>
    [RelayCommand]
    public void StopSendingStatus()
    {
        throw new NotImplementedException();

        //_apiManager.StopSendingStatus();
    }

    [RelayCommand]
    public void ClearResult()
    {
        CommandResult = "";
    }

    [RelayCommand]
    public void Send(string commandText)
    {
        try
        {
            List<string> commands = commandText.Split(";").ToList();

            StringBuilder result = new StringBuilder();

            List<InstructionInfo> instructions = [];
            foreach (string command in commands)
            {
                var instruction = _instructionService.CreateInstruction(command);

                instructions.Add(instruction);
            }

            List<InstructionResult> resultInstruction = _launcher.StartRange(instructions);
            resultInstruction.ForEach(i => result.AppendLine(i.Value));

            CommandResult += result.ToString();
            //return result.ToString();
        }
        catch (Exception ex)
        {
            CommandResult += ex.Message;
            //return ex.Message;
        }
    }
}