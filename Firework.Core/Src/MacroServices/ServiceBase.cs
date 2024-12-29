using System.Reflection;
using Firework.Abstraction.Services;
using Firework.Core.Exceptions;
using Firework.Dto.Instructions;

namespace Firework.Core.MacroServices;

public abstract class ServiceBase : IServiceBase
{
    private readonly IServiceManager _serviceManager;

    public ServiceBase(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    public virtual string Start(InstructionInfo instruction)
    {
        var service = _serviceManager.GetAllServices()[instruction.ServiceName];
        var module = service.ActionInfo.Find(item => item.Name == instruction.ActionInfo.Name);

        if (module != null)
        {
            //var serviceObj = _serviceManager.CreateService(service);
            var actionParams = ParamFilter(module.Method.GetParameters(), instruction.Parameters);

            try
            {
                var result = (string?)module.Method.Invoke(this, actionParams);
                return result ?? string.Empty;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        throw new StartServiceException("Такого модуля не существует");
    }

    private object[] ParamFilter(ParameterInfo[] parameters, List<ActionParameterInfo> parameterInfos)
    {
        if (parameters.Length == 0)
            return [];

        var result = new List<ActionParameterInfo>();

        foreach (ParameterInfo parameter in parameters.OrderBy(p => p.Position))
        {
            var param = parameterInfos
                .FirstOrDefault(x => string.Equals(x.Name, parameter.Name, StringComparison.CurrentCultureIgnoreCase));

            if (parameter.HasDefaultValue && param == null)
            {
                result.Add(new ActionParameterInfo
                {
                    Name = parameter.Name,
                    Value = parameter.DefaultValue.ToString()
                });

                continue;
            }

            result.Add(param);
        }

        var requiredParams = parameters.Where(x => !x.HasDefaultValue);

        if (requiredParams.ExceptBy(result.Select(x => x.Name), x => x.Name).Any())
        {
            throw new StartServiceException("Не правильное сопоставление параметров, не указаны все обязательные параметры");
        }



        return result.Select(x => x.Value).ToArray<object>();
    }
}