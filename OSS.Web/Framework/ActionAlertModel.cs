namespace OSS.Web.Framework
{
    public class ActionAlertModel
    {
        public string WindowId { get; set; } = string.Empty;
        public string AlertId { get; set; } = string.Empty;
        public string AlertMessage { get; set; } = string.Empty;
    }
    public class ActionConfirmationModel
    {
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string WindowId { get; set; } = string.Empty;
        public string AdditonalConfirmText { get; set; } = string.Empty;
    }
    public class DeleteConfirmationModel
    {
        public string Id { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string WindowId { get; set; } = string.Empty;
    }
}
