using HospitalMS.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalMS.ViewModels
{
    // ── Dashboard ─────────────────────────────────────────────────────────────
    public class DashboardViewModel
    {
        public int     TotalPatients        { get; set; }
        public int     TodayAppointments    { get; set; }
        public int     AvailableBeds        { get; set; }
        public int     TotalBeds            { get; set; }
        public decimal MonthlyRevenue       { get; set; }
        public int     NewPatientsThisMonth { get; set; }
        public int     PendingAppointments  { get; set; }
        public int     LowStockMedicines    { get; set; }
        public double  BedOccupancyPercentage =>
            TotalBeds > 0 ? Math.Round((TotalBeds - AvailableBeds) * 100.0 / TotalBeds, 1) : 0;
        public List<Appointment>    TodayAppointmentList { get; set; } = new();
        public List<WeeklyAdmission> WeeklyAdmissions   { get; set; } = new();
        public List<DepartmentLoad>  DepartmentLoads     { get; set; } = new();
        public List<AlertItem>       Alerts              { get; set; } = new();
    }
    public class WeeklyAdmission { public string Day { get; set; } = ""; public int Count { get; set; } }
    public class DepartmentLoad
    {
        public string Department  { get; set; } = "";
        public int    PatientCount{ get; set; }
        public int    Capacity    { get; set; }
        public double LoadPercentage => Capacity > 0 ? Math.Min(100, PatientCount * 100.0 / Capacity) : 0;
        public string LoadColor => LoadPercentage >= 80 ? "#ef4444" : LoadPercentage >= 60 ? "#f59e0b" : "#10b981";
    }
    public class AlertItem { public string Message { get; set; } = ""; public string Color { get; set; } = "#00b4d8"; public string TimeAgo { get; set; } = ""; }

    // ── Patient ───────────────────────────────────────────────────────────────
    public class PatientListViewModel
    {
        public List<Patient> Patients     { get; set; } = new();
        public string?       SearchTerm   { get; set; }
        public string?       StatusFilter { get; set; }
        public int TotalCount { get; set; }
        public int Page       { get; set; } = 1;
        public int PageSize   { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
    public class PatientFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="First name is required")]
        [Display(Name="First Name")]   public string FirstName { get; set; } = "";
        [Required(ErrorMessage="Last name is required")]
        [Display(Name="Last Name")]    public string LastName  { get; set; } = "";
        [Required(ErrorMessage="Date of birth is required")]
        [Display(Name="Date of Birth")][DataType(DataType.Date)]
        public DateTime DateOfBirth    { get; set; } = DateTime.Today.AddYears(-30);
        [Required] public string Gender     { get; set; } = "Male";
        [Required(ErrorMessage="Phone is required")]
        public string Phone            { get; set; } = "";
        [EmailAddress] public string?  Email      { get; set; }
        [Required][Display(Name="Blood Group")] public string BloodGroup { get; set; } = "O+";
        public string? Address        { get; set; }
        [Display(Name="Medical History")] public string? MedicalHistory { get; set; }
        public string? Allergies      { get; set; }
        public PatientStatus Status   { get; set; } = PatientStatus.Active;
    }
    public class PatientDetailViewModel
    {
        public Patient             Patient            { get; set; } = null!;
        public List<Appointment>   RecentAppointments { get; set; } = new();
        public List<Invoice>       RecentInvoices     { get; set; } = new();
        public Bed?                CurrentBed         { get; set; }
    }

    // ── Appointment ───────────────────────────────────────────────────────────
    public class AppointmentListViewModel
    {
        public List<Appointment> Appointments { get; set; } = new();
        public string? SearchTerm   { get; set; }
        public string? StatusFilter { get; set; }
        public string? DateFilter   { get; set; }
        public int TotalCount { get; set; }
        public int Page       { get; set; } = 1;
        public int PageSize   { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
    public class AppointmentFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Patient is required")]
        public int PatientId   { get; set; }
        [Required(ErrorMessage="Doctor is required")]
        public int DoctorId    { get; set; }
        [Required][DataType(DataType.Date)]
        [Display(Name="Appointment Date")] public DateTime AppointmentDate { get; set; } = DateTime.Today;
        [Required][Display(Name="Time")]   public string  AppointmentTime  { get; set; } = "09:00";
        [Required] public string Department { get; set; } = "";
        public AppointmentType   Type   { get; set; } = AppointmentType.Consultation;
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Notes            { get; set; }
        public List<Patient> Patients   { get; set; } = new();
        public List<Doctor>  Doctors    { get; set; } = new();
    }

    // ── Doctor ────────────────────────────────────────────────────────────────
    public class DoctorListViewModel
    {
        public List<Doctor> Doctors          { get; set; } = new();
        public string?      SearchTerm       { get; set; }
        public string?      DepartmentFilter { get; set; }
        public string?      StatusFilter     { get; set; }
    }
    public class DoctorFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="First name is required")]
        [Display(Name="First Name")] public string FirstName { get; set; } = "";
        [Required(ErrorMessage="Last name is required")]
        [Display(Name="Last Name")]  public string LastName  { get; set; } = "";
        [Required] public string Specialization { get; set; } = "";
        [Required] public string Department     { get; set; } = "";
        [Required] public string Phone          { get; set; } = "";
        [EmailAddress] public string? Email     { get; set; }
        [Display(Name="Experience (Years)")][Range(0,60)]
        public int ExperienceYears  { get; set; }
        public string? Qualification  { get; set; }
        [Display(Name="License Number")] public string? LicenseNumber { get; set; }
        public DoctorStatus Status    { get; set; } = DoctorStatus.Available;
        public string AvatarColor     { get; set; } = "#00b4d8";
    }

    // ── Schedule ──────────────────────────────────────────────────────────────
    public class ScheduleViewModel
    {
        public DateTime         WeekStart { get; set; }
        public DateTime         WeekEnd   { get; set; }
        public List<DaySchedule> Days     { get; set; } = new();
    }
    public class DaySchedule
    {
        public DateTime          Date         { get; set; }
        public string            DayName      => Date.ToString("ddd");
        public bool              IsToday      => Date.Date == DateTime.Today;
        public List<Appointment> Appointments { get; set; } = new();
    }

    // ── Pharmacy ──────────────────────────────────────────────────────────────
    public class PharmacyViewModel
    {
        public List<Medicine> Medicines         { get; set; } = new();
        public int    TotalItems        { get; set; }
        public int    LowStockCount     { get; set; }
        public int    ExpiringCount     { get; set; }
        public int    TodayPrescriptions{ get; set; }
        public string? SearchTerm       { get; set; }
        public string? CategoryFilter   { get; set; }
        public string? StockFilter      { get; set; }
    }
    public class MedicineFormViewModel
    {
        public int Id { get; set; }
        [Required] public string Name     { get; set; } = "";
        [Required] public string Category { get; set; } = "";
        public string? Manufacturer { get; set; }
        public string? BatchNumber  { get; set; }
        public MedicineForm Form    { get; set; } = MedicineForm.Tablet;
        [Range(0,int.MaxValue)] public int StockQuantity { get; set; }
        public int MinimumStock  { get; set; } = 50;
        public int MaximumStock  { get; set; } = 1000;
        [Range(0,double.MaxValue)] public decimal UnitPrice { get; set; }
        [Required][DataType(DataType.Date)]
        [Display(Name="Expiry Date")] public DateTime ExpiryDate { get; set; } = DateTime.Today.AddYears(1);
    }

    // ── Billing ───────────────────────────────────────────────────────────────
    public class BillingViewModel
    {
        public List<Invoice> Invoices       { get; set; } = new();
        public decimal TotalCollected       { get; set; }
        public decimal TotalPending         { get; set; }
        public decimal TotalOverdue         { get; set; }
        public int     InsuranceClaims      { get; set; }
        public string? SearchTerm           { get; set; }
        public string? StatusFilter         { get; set; }
        public int TotalCount { get; set; }
        public int Page       { get; set; } = 1;
        public int TotalPages => (int)Math.Ceiling(TotalCount / 10.0);
    }
    public class InvoiceFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Patient is required")]
        public int PatientId         { get; set; }
        public string? Services      { get; set; }
        [Range(0,double.MaxValue)] public decimal SubTotal  { get; set; }
        [Range(0,double.MaxValue)] public decimal Discount  { get; set; }
        [Range(0,100)] public decimal TaxRate                { get; set; } = 18;
        public PaymentMethod  PaymentMethod { get; set; } = PaymentMethod.Cash;
        public InvoiceStatus  Status        { get; set; } = InvoiceStatus.Pending;
        [Range(0,double.MaxValue)] public decimal PaidAmount { get; set; }
        public string? Notes         { get; set; }
        [DataType(DataType.Date)] public DateTime? DueDate  { get; set; }
        public List<Patient>        Patients { get; set; } = new();
        public List<InvoiceItemForm> Items   { get; set; } = new() { new InvoiceItemForm() };
    }
    public class InvoiceItemForm
    {
        public string  Description { get; set; } = "";
        public int     Quantity    { get; set; } = 1;
        public decimal UnitPrice   { get; set; }
    }

    // ── Reports ───────────────────────────────────────────────────────────────
    public class ReportsViewModel
    {
        public decimal MonthlyRevenue         { get; set; }
        public int     MonthlyPatients        { get; set; }
        public double  PatientSatisfaction    { get; set; } = 94;
        public double  AverageStayDays        { get; set; } = 4.2;
        public int     AverageWaitMinutes     { get; set; } = 18;
        public double  BedAvailabilityUptime  { get; set; } = 99.2;
        public List<MonthlyRevenue>  MonthlyRevenues  { get; set; } = new();
        public List<DepartmentStats> DepartmentStats  { get; set; } = new();
        public RevenueBreakdown      RevenueBreakdown { get; set; } = new();
    }
    public class MonthlyRevenue  { public string Month { get; set; }=""; public decimal Revenue { get; set; } public int Patients { get; set; } }
    public class DepartmentStats { public string Department { get; set; }=""; public int PatientCount { get; set; } public decimal Revenue { get; set; } public string LoadLevel { get; set; }="Normal"; }
    public class RevenueBreakdown
    {
        public decimal Consultations { get; set; }
        public decimal Surgeries     { get; set; }
        public decimal Pharmacy      { get; set; }
        public decimal LabImaging    { get; set; }
        public decimal Total => Consultations + Surgeries + Pharmacy + LabImaging;
        public double ConsultationsPercent => Total>0 ? Math.Round((double)Consultations*100.0/(double)Total,1):0;
        public double SurgeriesPercent     => Total>0 ? Math.Round((double)Surgeries*100.0/(double)Total,1):0;
        public double PharmacyPercent      => Total>0 ? Math.Round((double)Pharmacy*100.0/(double)Total,1):0;
        public double LabPercent           => Total>0 ? Math.Round((double)LabImaging*100.0/(double)Total,1):0;
    }
}
