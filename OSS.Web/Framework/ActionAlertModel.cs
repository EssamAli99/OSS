namespace OSS.Web.Framework
{
    public class ActionAlertModel
    {
        public string WindowId { get; set; }
        public string AlertId { get; set; }
        public string AlertMessage { get; set; }
    }
    public class ActionConfirmationModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string WindowId { get; set; }
        public string AdditonalConfirmText { get; set; }
    }
    public class DeleteConfirmationModel
    {
        public string Id { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string WindowId { get; set; }
    }
}
