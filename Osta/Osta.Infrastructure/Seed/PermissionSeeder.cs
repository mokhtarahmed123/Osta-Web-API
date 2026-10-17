using Osta.Domain.Entities.Identity;

namespace Osta.Infrastructure.Seed
{
    public static class PermissionSeeder
    {
        public static List<Permission> GetPermissions()
        {
            return new List<Permission>
            {
                // Service
                new Permission
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Code = "SERVICE_CREATE",
                    resource = "Service",
                    action = "Create",
                    description = "Create a service"
                },
                new Permission
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Code = "SERVICE_UPDATE",
                    resource = "Service",
                    action = "Update",
                    description = "Update a service"
                },
                new Permission
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Code = "SERVICE_DELETE",
                    resource = "Service",
                    action = "Delete",
                    description = "Delete a service"
                },
                new Permission
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Code = "SERVICE_READ",
                    resource = "Service",
                    action = "Read",
                    description = "Read services"
                },

                // Category
                new Permission
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Code = "CATEGORY_CREATE",
                    resource = "Category",
                    action = "Create",
                    description = "Create a category"
                },
                new Permission
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Code = "CATEGORY_UPDATE",
                    resource = "Category",
                    action = "Update",
                    description = "Update a category"
                },
                new Permission
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Code = "CATEGORY_DELETE",
                    resource = "Category",
                    action = "Delete",
                    description = "Delete a category"
                },
                new Permission
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Code = "CATEGORY_READ",
                    resource = "Category",
                    action = "Read",
                    description = "Read categories"
                },
                // ServiceArea
                new Permission
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    Code = "SERVICEAREA_CREATE",
                    resource = "ServiceArea",
                    action = "Create",
                    description = "Create a service area"
                },
                new Permission
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Code = "SERVICEAREA_UPDATE",
                    resource = "ServiceArea",
                    action = "Update",
                    description = "Update a service area"
                },
                new Permission
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Code = "SERVICEAREA_DELETE",
                    resource = "ServiceArea",
                    action = "Delete",
                    description = "Delete a service area"
                },
                new Permission
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    Code = "SERVICEAREA_READ",
                    resource = "ServiceArea",
                    action = "Read",
                    description = "Read service areas"
                }
            };
        }
    }
}