using System.Collections.Generic;

namespace OSS.Web
{
    public interface IPermissionProvider
    {
        IEnumerable<Permission> GetPermissions();
    }

    public class PermissionProvider : IPermissionProvider
    {
        public IEnumerable<Permission> GetPermissions()
        {
            return new List<Permission>
            {
                new Permission { Id = PermissionIds.READ, Name = "READ", ActionName = "Index", HtmlElementId = "btnShow" },
                new Permission { Id = PermissionIds.READ, Name = "READ", ActionName = "GetList", HtmlElementId = "btnShow" },
                new Permission { Id = PermissionIds.INSERT, Name = "INSERT", ActionName = "Create", HtmlElementId = "btnAdd" },
                new Permission { Id = PermissionIds.UPDATE, Name = "UPDATE", ActionName = "Edit", HtmlElementId = "btnEdit" },
                new Permission { Id = PermissionIds.DELET, Name = "DELETE", ActionName = "Delete", HtmlElementId = "btnDelete" },
                new Permission { Id = PermissionIds.EXPORT, Name = "EXPORT", ActionName = "Export", HtmlElementId = "btnExport" },
                new Permission { Id = PermissionIds.REJECT, Name = "REJECT", ActionName = "Reject", HtmlElementId = "btnReject" },
            };
        }
    }
}
