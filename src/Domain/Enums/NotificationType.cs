namespace NotificationService.Domain.Entities;

public enum NotificationType
{
    LeaveRequestCreated,
    LeaveRequestApproved,
    LeaveRequestRejected,
    LeaveRequestCancelled,
    AttendanceReminder,
    AttendanceAbnormal,
    OvertimeRequestCreated,
    OvertimeRequestApproved,
    OvertimeRequestRejected,
    EmployeeOnboarding,
    EmployeeOffboarding,
    BirthdayReminder,
    WorkAnniversary,
    PolicyUpdate,
    SystemAnnouncement,
    General
}
