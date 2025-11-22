using System.Globalization;

namespace Firework.Server.Services;

public class WelcomeService : IWelcomeService
{
    public string GetWelcomeText()
    {
        return CultureInfo.CurrentCulture.TwoLetterISOLanguageName switch
        {
            "ru" => "Добро пожаловать!",
            _ => "Welcome!"
        };
    }
}

public interface IWelcomeService
{
    string GetWelcomeText();
}