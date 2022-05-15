using Smartstore.Core.Identity;
using Smartstore.Core.Security;

namespace Smartstore.Moving
{
    internal static class MovingPermissions
    {
        public const string Self = "cms.moving";
        public const string Read = $"{Self}.read";
        public const string Update = $"{Self}.update";
        public const string Create = $"{Self}.create";
        public const string Delete = $"{Self}.delete";
        public const string EditComment = $"{Self}.editcomment";
    }

    internal class MovingPermissionProvider : IPermissionProvider
    {
        public IEnumerable<PermissionRecord> GetPermissions()
        {
            // Get all permissions from above static class.
            var permissionSystemNames = PermissionHelper.GetPermissions(typeof(MovingPermissions));
            var permissions = permissionSystemNames.Select(x => new PermissionRecord { SystemName = x });

            return permissions;
        }

        public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            // Allow root permission for admin by default.
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        new PermissionRecord { SystemName = MovingPermissions.Self }
                    }
                }
            };
        }
    }
}
