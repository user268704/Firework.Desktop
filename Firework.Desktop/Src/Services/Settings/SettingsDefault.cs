using System.ComponentModel.DataAnnotations;

namespace Firework.Desktop.Services.Settings;

public static class SettingsDefault
{
    public static class Fields
    {
        [Display(Name = "Автозапуск при включении ОС")]
        public static string AppAutoRun => "false";

        [Display(Name = "Сидеть тихо")]
        public static string KeepSilent => "false";

        [Display(Name = "Запуск в трее")]
        public static string RunAppInTray => "false";

        [Display(Name = "Сворачивать в трей")]
        public static string MinimizeInTray => "false";

        [Display(Name = "Уведомлять о отправленной нагрузке")]
        public static string NotifyInfoSend => "false";

        [Display(Name = "Хост сервера")]
        public static string LocalHost => "*";

        [Display(Name = "Порт сервера")]
        public static string LocalPort => "5062";

        [Display(Name = "Включить логирование")]
        public static string IsLoggingEnabled => "true";

        [Display(Name = "Путь к папке логов")]
        public static string LogPathDirectory => "D:/Firework.Txt/Logs/";
    }

    public static class Names
    {
        public static string ApplicationName => "Application";
        public static string AppAutoRun => "AppAutoRun";
        public static string KeepSilent => "KeepSilent";
        public static string RunAppInTray => "RunAppInTray";
        public static string MinimizeInTray => "MinimizeInTray";
        public static string NotifyInfoSend => "NotifyInfoSend";
        public static string LocalHost => "LocalHost";
        public static string LocalPort => "LocalPort";
        public static string IsLoggingEnabled => "IsLoggingEnabled";
        public static string LogPathDirectory => "LogPathDirectory";
    }

}