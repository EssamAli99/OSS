using System.Collections.Generic;

namespace OSS.Web
{
    public enum NotifyType
    {
        Success,
        Error,
        Warning
    }
    public enum LogLevel
    {
        Debug = 10,
        Information = 20,
        Warning = 30,
        Error = 40,
        Fatal = 50
    }
    public struct PermissionIds
    {
        public const string READ = "1";
        public const string INSERT = "2";
        public const string UPDATE = "3";
        public const string DELET = "4";
        public const string EXPORT = "5";
        public const string REJECT = "6";
    }
    public struct Permission
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public string HtmlElementId { get; set; }
    };

    public class OSSConfig
    {
        public static string PreferedLanguage => "PreferedLangauge";
        /// <summary>
        /// Gets the name of a request item that stores the value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        public static string IsPostBeingDoneRequestItem => "OSS.IsPOSTBeingDone";
        /// <summary>
        /// Gets the name of X-FORWARDED-FOR header
        /// </summary>
        public static string XForwardedForHeader => "X-FORWARDED-FOR";
        public static string DefaultHttpClient => "default";
        public static string NotificationListKey => "NotificationList";
        public static int RestartTimeout => 3000;
        public static int DefaultGridPageSize => 15;
        public static int PopupGridPageSize => 7;
        public static string GridPageSizes = "7, 15, 20, 50, 100";
        public static string ScheduleTaskPath => "https://localhost:44391/scheduletask/runtask";
        public static int ScheduleTaskRunTimeout => 30 * 60;
        public struct NotifyData
        {
            public NotifyType Type { get; set; }
            public string Message { get; set; }
            public bool Encode { get; set; }
        }

        public static Dictionary<string, string> PermissionNames => new()
        {
            { "1", "READ" },
            { "2", "INSERT" },
            { "3", "UPDATE" },
            { "4", "DELETE" },
            { "5", "EXPORT" },
            { "6", "REJECT" },
        };

        public static List<Permission> Permissions => new()
        {
            new Permission
            {
                Id = PermissionIds.READ,
                Name = "READ",
                ActionName = "Index",
                HtmlElementId = "btnShow"
            },
            new Permission
            {
                Id = PermissionIds.READ,
                Name = "READ",
                ActionName = "GetList",
                HtmlElementId = "btnShow"
            },
            new Permission
            {
                Id = PermissionIds.INSERT,
                Name = "INSERT",
                ActionName = "Create",
                HtmlElementId = "btnAdd"
            },
            new Permission
            {
                Id = PermissionIds.UPDATE,
                Name = "UPDATE",
                ActionName = "Edit",
                HtmlElementId = "btnEdit"
            },
            new Permission
            {
                Id = PermissionIds.DELET,
                Name = "DELETE",
                ActionName = "Delete",
                HtmlElementId = "btnDelete"
            },
            new Permission
            {
                Id = PermissionIds.EXPORT,
                Name = "EXPORT",
                ActionName = "Export",
                HtmlElementId = "btnExport"
            },
            new Permission
            {
                Id = PermissionIds.REJECT,
                Name = "REJECT",
                ActionName = "Reject",
                HtmlElementId = "btnReject"
            },
        };
    }

}
