using System.ComponentModel.DataAnnotations;

namespace HospitalMS.Models
{
    public class Patient
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string FirstName { get; set; } = "";
        [Required, StringLength(100)] public string LastName  { get; set; } = "";
        public string FullName => $"{FirstName} {LastName}";
        [Required] public DateTime DateOfBirth { get; set; }
        public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);
        [Required] public string Gender     { get; set; } = "Male";
        [Required] public string Phone      { get; set; } = "";
        public string? Email        { get; set; }
        [Required] public string BloodGroup { get; set; } = "O+";
        public string? Address        { get; set; }
        public string? MedicalHistory { get; set; }
        public string? Allergies      { get; set; }
        public PatientStatus Status   { get; set; } = PatientStatus.Active;
        public string PatientCode => $"PT-{Id:D6}";
        public DateTime RegisteredOn  { get; set; } = DateTime.Now;
        public List<Appointment> Appointments { get; set; } = new();
        public List<Invoice>     Invoices     { get; set; } = new();
    }
    public enum PatientStatus { Active, Admitted, FollowUp, Discharged, Cancelled }

    public class Doctor
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string FirstName      { get; set; } = "";
        [Required, StringLength(100)] public string LastName       { get; set; } = "";
        public string FullName => $"Dr. {FirstName} {LastName}";
        public string Initials => $"{FirstName[0]}{LastName[0]}";
        [Required] public string Specialization { get; set; } = "";
        [Required] public string Department     { get; set; } = "";
        [Required] public string Phone          { get; set; } = "";
        public string? Email           { get; set; }
        public int    ExperienceYears  { get; set; }
        public string? Qualification   { get; set; }
        public string? LicenseNumber   { get; set; }
        public DoctorStatus Status     { get; set; } = DoctorStatus.Available;
        public string AvatarColor      { get; set; } = "#00b4d8";
        public DateTime JoinedOn       { get; set; } = DateTime.Now;
        public List<Appointment> Appointments { get; set; } = new();
    }
    public enum DoctorStatus { Available, Busy, InSurgery, OffDuty, OnLeave }

    public class Appointment
    {
        public int Id          { get; set; }
        public int PatientId   { get; set; }
        public Patient? Patient { get; set; }
        public int DoctorId    { get; set; }
        public Doctor? Doctor  { get; set; }
        [Required] public DateTime AppointmentDate { get; set; }
        [Required] public TimeSpan AppointmentTime { get; set; }
        public DateTime AppointmentDateTime => AppointmentDate.Date + AppointmentTime;
        [Required] public string Department { get; set; } = "";
        public AppointmentType   Type   { get; set; } = AppointmentType.Consultation;
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Notes        { get; set; }
        public string? Diagnosis    { get; set; }
        public string? Prescription { get; set; }
        public DateTime CreatedOn   { get; set; } = DateTime.Now;
    }
    public enum AppointmentType   { Consultation, FollowUp, Surgery, Checkup, Emergency }
    public enum AppointmentStatus { Pending, Confirmed, InProgress, Completed, Cancelled, NoShow }

    public class Medicine
    {
        public int Id { get; set; }
        [Required, StringLength(200)] public string Name { get; set; } = "";
        [Required] public string Category     { get; set; } = "";
        public string? Manufacturer { get; set; }
        public string? BatchNumber  { get; set; }
        public MedicineForm Form    { get; set; } = MedicineForm.Tablet;
        public int StockQuantity    { get; set; }
        public int MinimumStock     { get; set; } = 50;
        public int MaximumStock     { get; set; } = 1000;
        public decimal UnitPrice    { get; set; }
        public DateTime ExpiryDate  { get; set; }
        public StockStatus StockStatus
        {
            get
            {
                if (StockQuantity <= 0) return StockStatus.OutOfStock;
                if (StockQuantity <= MinimumStock * 0.5) return StockStatus.Critical;
                if (StockQuantity <= MinimumStock)       return StockStatus.LowStock;
                return StockStatus.InStock;
            }
        }
        public bool IsExpiringSoon => ExpiryDate <= DateTime.Today.AddDays(30);
        public double StockPercentage =>
            MaximumStock > 0 ? Math.Min(100, StockQuantity * 100.0 / MaximumStock) : 0;
    }
    public enum MedicineForm { Tablet, Capsule, Syrup, Injection, Cream, Drops, Inhaler }
    public enum StockStatus  { InStock, LowStock, Critical, OutOfStock }

    public class Invoice
    {
        public int Id { get; set; }
        public int PatientId    { get; set; }
        public Patient? Patient { get; set; }
        public string InvoiceNumber => $"INV-{Id:D4}";
        public decimal SubTotal     { get; set; }
        public decimal Discount     { get; set; }
        public decimal TaxAmount    { get; set; }
        public decimal TotalAmount  { get; set; }
        public decimal PaidAmount   { get; set; }
        public decimal BalanceAmount => TotalAmount - PaidAmount;
        public PaymentMethod  PaymentMethod { get; set; } = PaymentMethod.Cash;
        public InvoiceStatus  Status        { get; set; } = InvoiceStatus.Pending;
        public string? Services { get; set; }
        public string? Notes    { get; set; }
        public DateTime  InvoiceDate { get; set; } = DateTime.Now;
        public DateTime? DueDate     { get; set; }
        public List<InvoiceItem> Items { get; set; } = new();
    }

    public class InvoiceItem
    {
        public int Id         { get; set; }
        public int InvoiceId  { get; set; }
        public Invoice? Invoice { get; set; }
        [Required] public string Description { get; set; } = "";
        public int Quantity      { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Total     => Quantity * UnitPrice;
    }
    public enum PaymentMethod  { Cash, UPI, Card, Insurance, NetBanking }
    public enum InvoiceStatus  { Pending, Partial, Paid, Overdue, Cancelled }

    public class Bed
    {
        public int Id { get; set; }
        [Required] public string BedNumber { get; set; } = "";
        [Required] public string Ward      { get; set; } = "";
        public string WardType  { get; set; } = "General";
        public BedStatus Status { get; set; } = BedStatus.Available;
        public int? OccupiedByPatientId    { get; set; }
        public Patient? OccupiedByPatient  { get; set; }
        public DateTime? OccupiedSince     { get; set; }
        public decimal DailyRate           { get; set; }
    }
    public enum BedStatus { Available, Occupied, Reserved, Maintenance }
}
