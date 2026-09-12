namespace Pistachio.Api.Authorization;

public static class Permissions
{
    public const string ServicesRead = "services:read";
    public const string ServicesWrite = "services:write";

    public const string SchedulingRead = "scheduling:read";
    public const string SchedulingWrite = "scheduling:write";
    public const string SchedulingStatus = "scheduling:status";
    public const string SchedulingAssign = "scheduling:assign";
    public const string SchedulingDelete = "scheduling:delete";

    public const string PaymentsRead = "payments:read";
    public const string PaymentsWrite = "payments:write";

    public const string UsersRead = "users:read";

    public static readonly string[] All =
    [
        ServicesRead, ServicesWrite,
        SchedulingRead, SchedulingWrite, SchedulingStatus, SchedulingAssign, SchedulingDelete,
        PaymentsRead, PaymentsWrite,
        UsersRead
    ];
}
