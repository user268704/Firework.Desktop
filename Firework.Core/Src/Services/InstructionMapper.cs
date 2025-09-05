using Firework.Dto.Instructions;

namespace Firework.Core.Services;

public static class InstructionMapper
{
    /// <summary>
    /// Маппит InstructionInfoDto в InstructionInfo
    /// </summary>
    /// <param name="dto">DTO инструкции</param>
    /// <returns>Модель инструкции</returns>
    public static InstructionInfo MapToInstructionInfo(InstructionInfoDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var actionInfo = new ActionInfo
        {
            Name = dto.ActionName,
            Parameters = new List<ActionParameterInfo>()
        };

        // Маппим параметры из Dictionary в List<ActionParameterInfo>
        if (dto.Parameters != null)
        {
            foreach (var param in dto.Parameters)
            {
                actionInfo.Parameters.Add(new ActionParameterInfo
                {
                    Name = param.Key,
                    Value = param.Value
                });
            }
        }

        return new InstructionInfo
        {
            ServiceName = dto.ServiceName,
            ActionInfo = actionInfo,
            Parameters = actionInfo.Parameters,
            Title = $"{dto.ServiceName}.{dto.ActionName}",
            Description = $"Выполнение действия {dto.ActionName} в сервисе {dto.ServiceName}"
        };
    }

    /// <summary>
    /// Маппит список InstructionInfoDto в список InstructionInfo
    /// </summary>
    /// <param name="dtos">Список DTO инструкций</param>
    /// <returns>Список моделей инструкций</returns>
    public static List<InstructionInfo> MapToInstructionInfoList(List<InstructionInfoDto> dtos)
    {
        if (dtos == null)
            throw new ArgumentNullException(nameof(dtos));

        return dtos.Select(MapToInstructionInfo).ToList();
    }
}

