-- =============================================
-- Notification Service - Seed Data
-- =============================================

-- Notification Templates
INSERT INTO "NotificationTemplates" ("Id", "Code", "Name", "Type", "TitleTemplate", "MessageTemplate", "IsActive", "CreatedAt", "UpdatedAt")
VALUES 
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee01', 'LEAVE_CREATED', 'Leave Request Created', 'LeaveRequestCreated', 'New Leave Request from {EmployeeName}', '{EmployeeName} has submitted a {LeaveType} leave request from {StartDate} to {EndDate} ({TotalDays} days). Reason: {Reason}', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee02', 'LEAVE_APPROVED', 'Leave Request Approved', 'LeaveRequestApproved', 'Your Leave Request has been Approved', 'Your {LeaveType} leave request from {StartDate} to {EndDate} has been approved by {ApproverName}. Comment: {Comment}', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee03', 'LEAVE_REJECTED', 'Leave Request Rejected', 'LeaveRequestRejected', 'Your Leave Request has been Rejected', 'Your {LeaveType} leave request from {StartDate} to {EndDate} has been rejected. Reason: {RejectionReason}', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee04', 'LEAVE_CANCELLED', 'Leave Request Cancelled', 'LeaveRequestCancelled', 'Leave Request Cancelled', 'The leave request from {EmployeeName} ({StartDate} to {EndDate}) has been cancelled.', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee05', 'ATTENDANCE_REMINDER', 'Attendance Reminder', 'AttendanceReminder', 'Don''t Forget to Check In!', 'Reminder: You haven''t checked in today. Please check in before {Deadline}.', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee06', 'ATTENDANCE_ABNORMAL', 'Abnormal Attendance', 'AttendanceAbnormal', 'Attendance Alert for {EmployeeName}', 'Attendance anomaly detected for {EmployeeName} on {Date}: {AbnormalType}. Please review.', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee07', 'OT_CREATED', 'Overtime Request Created', 'OvertimeRequestCreated', 'New Overtime Request from {EmployeeName}', '{EmployeeName} has requested overtime on {Date} from {StartTime} to {EndTime} ({TotalHours} hours). Reason: {Reason}', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee08', 'OT_APPROVED', 'Overtime Request Approved', 'OvertimeRequestApproved', 'Your Overtime Request has been Approved', 'Your overtime request for {Date} has been approved by {ApproverName}.', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee09', 'OT_REJECTED', 'Overtime Request Rejected', 'OvertimeRequestRejected', 'Your Overtime Request has been Rejected', 'Your overtime request for {Date} has been rejected. Reason: {RejectionReason}', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee10', 'BIRTHDAY', 'Birthday Reminder', 'BirthdayReminder', 'Happy Birthday to {EmployeeName}! 🎂', 'Today is {EmployeeName}''s birthday! Don''t forget to wish them well.', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee11', 'ANNIVERSARY', 'Work Anniversary', 'WorkAnniversary', 'Work Anniversary - {EmployeeName}', 'Congratulations! {EmployeeName} celebrates {Years} years with the company today!', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee12', 'ONBOARDING', 'New Employee Onboarding', 'EmployeeOnboarding', 'Welcome {EmployeeName} to the Team!', 'Please welcome {EmployeeName} who is joining {DepartmentName} as {Position} starting {StartDate}.', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee13', 'POLICY_UPDATE', 'Policy Update', 'PolicyUpdate', 'Policy Update: {PolicyName}', 'A new policy update has been published: {PolicyName}. Please review the changes.', true, NOW(), NOW()),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeee14', 'SYSTEM_ANNOUNCEMENT', 'System Announcement', 'SystemAnnouncement', '{Title}', '{Message}', true, NOW(), NOW());

-- Notification Preferences for sample users
INSERT INTO "NotificationPreferences" ("Id", "UserId", "Type", "EmailEnabled", "PushEnabled", "InAppEnabled", "CreatedAt", "UpdatedAt")
VALUES 
-- Admin preferences
('ffffffff-ffff-ffff-ffff-ffffffffffff01', '44444444-4444-4444-4444-444444444444', 'LeaveRequestCreated', true, true, true, NOW(), NOW()),
('ffffffff-ffff-ffff-ffff-ffffffffffff02', '44444444-4444-4444-4444-444444444444', 'SystemAnnouncement', true, true, true, NOW(), NOW()),
-- HR Manager preferences
('ffffffff-ffff-ffff-ffff-ffffffffffff03', '44444444-4444-4444-4444-444444444445', 'LeaveRequestCreated', true, true, true, NOW(), NOW()),
('ffffffff-ffff-ffff-ffff-ffffffffffff04', '44444444-4444-4444-4444-444444444445', 'AttendanceAbnormal', true, true, true, NOW(), NOW()),
('ffffffff-ffff-ffff-ffff-ffffffffffff05', '44444444-4444-4444-4444-444444444445', 'EmployeeOnboarding', true, true, true, NOW(), NOW()),
-- Engineering Manager preferences  
('ffffffff-ffff-ffff-ffff-ffffffffffff06', '44444444-4444-4444-4444-444444444446', 'LeaveRequestCreated', true, true, true, NOW(), NOW()),
('ffffffff-ffff-ffff-ffff-ffffffffffff07', '44444444-4444-4444-4444-444444444446', 'OvertimeRequestCreated', true, true, true, NOW(), NOW()),
-- Team Lead preferences
('ffffffff-ffff-ffff-ffff-ffffffffffff08', '44444444-4444-4444-4444-444444444447', 'LeaveRequestCreated', true, true, true, NOW(), NOW()),
('ffffffff-ffff-ffff-ffff-ffffffffffff09', '44444444-4444-4444-4444-444444444447', 'OvertimeRequestCreated', true, true, true, NOW(), NOW()),
-- Employee preferences
('ffffffff-ffff-ffff-ffff-ffffffffffff10', '44444444-4444-4444-4444-444444444448', 'LeaveRequestApproved', true, true, true, NOW(), NOW()),
('ffffffff-ffff-ffff-ffff-ffffffffffff11', '44444444-4444-4444-4444-444444444448', 'LeaveRequestRejected', true, true, true, NOW(), NOW()),
('ffffffff-ffff-ffff-ffff-ffffffffffff12', '44444444-4444-4444-4444-444444444448', 'AttendanceReminder', false, true, true, NOW(), NOW());

-- Sample Notifications
INSERT INTO "Notifications" ("Id", "UserId", "Title", "Message", "Type", "Priority", "Data", "ActionUrl", "IsRead", "IsArchived", "CreatedAt", "ReadAt", "ExpiresAt")
VALUES 
-- For HR Manager (pending approval notifications)
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa001', '44444444-4444-4444-4444-444444444445', 'New Leave Request from Linh Hoang', 'Linh Hoang has submitted an Annual leave request from Dec 30, 2025 to Jan 02, 2026 (4 days). Reason: New Year holiday', 'LeaveRequestCreated', 'Normal', '{"requestId": "cccccccc-cccc-cccc-cccc-ccccccccc003", "employeeId": "44444444-4444-4444-4444-444444444454"}', '/approvals', false, false, '2025-12-15 10:00:00', NULL, '2025-12-30 00:00:00'),

('11111111-aaaa-aaaa-aaaa-aaaaaaaaa002', '44444444-4444-4444-4444-444444444445', 'Pending HR Approval: Phong Dang', 'Leave request from Phong Dang is awaiting your approval. Period: Dec 20-22, 2025', 'LeaveRequestCreated', 'High', '{"requestId": "cccccccc-cccc-cccc-cccc-ccccccccc004", "employeeId": "44444444-4444-4444-4444-444444444455"}', '/approvals', false, false, '2025-12-16 11:05:00', NULL, '2025-12-20 00:00:00'),

-- For Team Lead (manager approval notifications)
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa003', '44444444-4444-4444-4444-444444444447', 'New Leave Request from Linh Hoang', 'Linh Hoang has submitted an Annual leave request from Dec 30, 2025 to Jan 02, 2026 (4 days).', 'LeaveRequestCreated', 'Normal', '{"requestId": "cccccccc-cccc-cccc-cccc-ccccccccc003", "employeeId": "44444444-4444-4444-4444-444444444454"}', '/approvals', false, false, '2025-12-15 10:00:00', NULL, NULL),

('11111111-aaaa-aaaa-aaaa-aaaaaaaaa004', '44444444-4444-4444-4444-444444444447', 'Overtime Request from Khanh Nguyen', 'Khanh Nguyen has requested 3 hours overtime on Dec 18, 2025 for bug fixes.', 'OvertimeRequestCreated', 'Normal', '{"requestId": "dddddddd-dddd-dddd-dddd-ddddddddd003", "employeeId": "44444444-4444-4444-4444-444444444453"}', '/approvals', false, false, '2025-12-17 09:00:00', NULL, NULL),

-- For Employees (approval result notifications)
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa005', '44444444-4444-4444-4444-444444444448', 'Your Leave Request has been Approved', 'Your Annual leave request from Dec 24-26, 2025 has been approved by Hung Tran and Lan Pham. Enjoy your vacation!', 'LeaveRequestApproved', 'Normal', '{"requestId": "cccccccc-cccc-cccc-cccc-ccccccccc001"}', '/leave', true, false, '2025-12-10 14:00:00', '2025-12-10 15:30:00', NULL),

('11111111-aaaa-aaaa-aaaa-aaaaaaaaa006', '44444444-4444-4444-4444-444444444456', 'Your Leave Request has been Rejected', 'Your Annual leave request from Dec 01-05, 2025 has been rejected. Reason: Critical project period, please reschedule', 'LeaveRequestRejected', 'High', '{"requestId": "cccccccc-cccc-cccc-cccc-ccccccccc005"}', '/leave', true, false, '2025-11-25 09:00:00', '2025-11-25 09:15:00', NULL),

('11111111-aaaa-aaaa-aaaa-aaaaaaaaa007', '44444444-4444-4444-4444-444444444448', 'Your Overtime Request has been Approved', 'Your overtime request for Dec 04, 2025 (2 hours) has been approved by Hung Tran.', 'OvertimeRequestApproved', 'Normal', '{"requestId": "dddddddd-dddd-dddd-dddd-ddddddddd001"}', '/attendance', true, false, '2025-12-04 10:00:00', '2025-12-04 10:30:00', NULL),

-- Attendance reminders
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa008', '44444444-4444-4444-4444-444444444453', 'Don''t Forget to Check In!', 'Reminder: You haven''t checked in today. Please check in before 10:00 AM.', 'AttendanceReminder', 'Normal', '{}', '/dashboard', false, false, '2025-12-17 09:30:00', NULL, '2025-12-17 12:00:00'),

-- System announcements
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa009', '44444444-4444-4444-4444-444444444444', 'System Maintenance Notice', 'The HRM system will undergo scheduled maintenance on Dec 20, 2025 from 10 PM to 2 AM. Please save your work before this time.', 'SystemAnnouncement', 'High', '{}', NULL, false, false, '2025-12-15 08:00:00', NULL, '2025-12-21 00:00:00'),
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa010', '44444444-4444-4444-4444-444444444445', 'System Maintenance Notice', 'The HRM system will undergo scheduled maintenance on Dec 20, 2025 from 10 PM to 2 AM.', 'SystemAnnouncement', 'High', '{}', NULL, true, false, '2025-12-15 08:00:00', '2025-12-15 10:00:00', '2025-12-21 00:00:00'),
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa011', '44444444-4444-4444-4444-444444444446', 'System Maintenance Notice', 'The HRM system will undergo scheduled maintenance on Dec 20, 2025 from 10 PM to 2 AM.', 'SystemAnnouncement', 'High', '{}', NULL, false, false, '2025-12-15 08:00:00', NULL, '2025-12-21 00:00:00'),
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa012', '44444444-4444-4444-4444-444444444447', 'System Maintenance Notice', 'The HRM system will undergo scheduled maintenance on Dec 20, 2025 from 10 PM to 2 AM.', 'SystemAnnouncement', 'High', '{}', NULL, false, false, '2025-12-15 08:00:00', NULL, '2025-12-21 00:00:00'),
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa013', '44444444-4444-4444-4444-444444444448', 'System Maintenance Notice', 'The HRM system will undergo scheduled maintenance on Dec 20, 2025 from 10 PM to 2 AM.', 'SystemAnnouncement', 'High', '{}', NULL, false, false, '2025-12-15 08:00:00', NULL, '2025-12-21 00:00:00'),

-- Birthday notification
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa014', '44444444-4444-4444-4444-444444444447', 'Happy Birthday to Khanh Nguyen! 🎂', 'Today is Khanh Nguyen''s birthday! Don''t forget to wish them well.', 'BirthdayReminder', 'Low', '{"employeeId": "44444444-4444-4444-4444-444444444453"}', NULL, false, false, '2025-07-14 08:00:00', NULL, '2025-07-15 00:00:00'),

-- Work anniversary
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa015', '44444444-4444-4444-4444-444444444446', 'Work Anniversary - Mai Nguyen', 'Congratulations! Mai Nguyen celebrates 6 years with the company today!', 'WorkAnniversary', 'Low', '{"employeeId": "44444444-4444-4444-4444-444444444449", "years": 6}', NULL, true, false, '2025-07-01 08:00:00', '2025-07-01 09:00:00', NULL),

-- Policy update
('11111111-aaaa-aaaa-aaaa-aaaaaaaaa016', '44444444-4444-4444-4444-444444444448', 'Policy Update: Remote Work Policy', 'A new policy update has been published: Remote Work Policy 2025. Please review the changes in the HR portal.', 'PolicyUpdate', 'Normal', '{"policyId": "policy-001"}', '/policies', false, false, '2025-12-01 09:00:00', NULL, NULL);

-- Notification Logs
INSERT INTO "NotificationLogs" ("Id", "NotificationId", "UserId", "Channel", "Status", "Error", "SentAt", "DeliveredAt")
VALUES 
('22222222-bbbb-bbbb-bbbb-bbbbbbbbb001', '11111111-aaaa-aaaa-aaaa-aaaaaaaaa001', '44444444-4444-4444-4444-444444444445', 'InApp', 'Delivered', NULL, '2025-12-15 10:00:00', '2025-12-15 10:00:01'),
('22222222-bbbb-bbbb-bbbb-bbbbbbbbb002', '11111111-aaaa-aaaa-aaaa-aaaaaaaaa001', '44444444-4444-4444-4444-444444444445', 'Email', 'Delivered', NULL, '2025-12-15 10:00:00', '2025-12-15 10:00:15'),
('22222222-bbbb-bbbb-bbbb-bbbbbbbbb003', '11111111-aaaa-aaaa-aaaa-aaaaaaaaa005', '44444444-4444-4444-4444-444444444448', 'InApp', 'Delivered', NULL, '2025-12-10 14:00:00', '2025-12-10 14:00:01'),
('22222222-bbbb-bbbb-bbbb-bbbbbbbbb004', '11111111-aaaa-aaaa-aaaa-aaaaaaaaa005', '44444444-4444-4444-4444-444444444448', 'Email', 'Delivered', NULL, '2025-12-10 14:00:00', '2025-12-10 14:00:12'),
('22222222-bbbb-bbbb-bbbb-bbbbbbbbb005', '11111111-aaaa-aaaa-aaaa-aaaaaaaaa005', '44444444-4444-4444-4444-444444444448', 'Push', 'Delivered', NULL, '2025-12-10 14:00:00', '2025-12-10 14:00:03'),
('22222222-bbbb-bbbb-bbbb-bbbbbbbbb006', '11111111-aaaa-aaaa-aaaa-aaaaaaaaa006', '44444444-4444-4444-4444-444444444456', 'InApp', 'Delivered', NULL, '2025-11-25 09:00:00', '2025-11-25 09:00:01'),
('22222222-bbbb-bbbb-bbbb-bbbbbbbbb007', '11111111-aaaa-aaaa-aaaa-aaaaaaaaa006', '44444444-4444-4444-4444-444444444456', 'Email', 'Failed', 'SMTP connection timeout', '2025-11-25 09:00:00', NULL);
