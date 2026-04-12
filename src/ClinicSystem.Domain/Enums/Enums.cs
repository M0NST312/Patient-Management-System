namespace ClinicSystem.Domain.Enums;

public enum Gender { Unknown = 0, Male = 1, Female = 2, Other = 3 }
public enum VisitStatus { Scheduled = 0, Completed = 1, Cancelled = 2 }
public enum InvoiceStatus { Unpaid = 0, Partial = 1, Paid = 2 }
public enum UserRole { Admin = 0, Doctor = 1, Receptionist = 2 }
