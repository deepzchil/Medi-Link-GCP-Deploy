using HospitalMS.Data;
using HospitalMS.Models;
using HospitalMS.ViewModels;

namespace HospitalMS.Services
{
    // ── Interfaces ────────────────────────────────────────────────────────────
    public interface IPatientService
    {
        PatientListViewModel GetPatients(string? search, string? status, int page, int pageSize);
        Patient? GetById(int id);
        PatientDetailViewModel? GetDetail(int id);
        Patient Create(PatientFormViewModel vm);
        Patient? Update(int id, PatientFormViewModel vm);
        bool Delete(int id);
        List<Patient> GetAll();
    }

    public interface IAppointmentService
    {
        AppointmentListViewModel GetAppointments(string? search, string? status, string? date, int page, int pageSize);
        Appointment? GetById(int id);
        Appointment Create(AppointmentFormViewModel vm);
        Appointment? Update(int id, AppointmentFormViewModel vm);
        bool Delete(int id);
        List<Appointment> GetToday();
        ScheduleViewModel GetWeekSchedule(DateTime weekStart);
    }

    public interface IDoctorService
    {
        DoctorListViewModel GetDoctors(string? search, string? dept, string? status);
        Doctor? GetById(int id);
        Doctor Create(DoctorFormViewModel vm);
        Doctor? Update(int id, DoctorFormViewModel vm);
        bool Delete(int id);
        List<Doctor> GetAll();
    }

    public interface IPharmacyService
    {
        PharmacyViewModel GetMedicines(string? search, string? category, string? stock);
        Medicine? GetById(int id);
        Medicine Create(MedicineFormViewModel vm);
        Medicine? Update(int id, MedicineFormViewModel vm);
        bool Delete(int id);
    }

    public interface IBillingService
    {
        BillingViewModel GetInvoices(string? search, string? status, int page);
        Invoice? GetById(int id);
        Invoice Create(InvoiceFormViewModel vm);
        Invoice? Update(int id, InvoiceFormViewModel vm);
        bool Delete(int id);
    }

    public interface IDashboardService
    {
        DashboardViewModel GetDashboard();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    internal static class StoreHelpers
    {
        internal static Patient? Hydrate(this Patient p, InMemoryStore db) => p;
        internal static Appointment Hydrate(this Appointment a, InMemoryStore db)
        {
            a.Patient  = db.Patients.FirstOrDefault(p => p.Id == a.PatientId);
            a.Doctor   = db.Doctors .FirstOrDefault(d => d.Id == a.DoctorId);
            return a;
        }
        internal static Invoice Hydrate(this Invoice inv, InMemoryStore db)
        {
            inv.Patient = db.Patients.FirstOrDefault(p => p.Id == inv.PatientId);
            inv.Items   = db.InvoiceItems.Where(i => i.InvoiceId == inv.Id).ToList();
            return inv;
        }
    }

    // ── Implementations ───────────────────────────────────────────────────────

    public class PatientService : IPatientService
    {
        private readonly InMemoryStore _db;
        public PatientService(InMemoryStore db) => _db = db;

        public PatientListViewModel GetPatients(string? search, string? status, int page, int pageSize)
        {
            var q = _db.Patients.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(p => p.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)
                               || p.LastName .Contains(search, StringComparison.OrdinalIgnoreCase)
                               || p.Phone    .Contains(search)
                               || (p.Email != null && p.Email.Contains(search, StringComparison.OrdinalIgnoreCase)));
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PatientStatus>(status, out var s))
                q = q.Where(p => p.Status == s);
            var list  = q.OrderByDescending(p => p.RegisteredOn).ToList();
            var total = list.Count;
            return new PatientListViewModel
            {
                Patients   = list.Skip((page-1)*pageSize).Take(pageSize).ToList(),
                TotalCount = total, Page = page, PageSize = pageSize,
                SearchTerm = search, StatusFilter = status
            };
        }

        public Patient? GetById(int id) => _db.Patients.FirstOrDefault(p => p.Id == id);

        public PatientDetailViewModel? GetDetail(int id)
        {
            var p = GetById(id);
            if (p == null) return null;
            return new PatientDetailViewModel
            {
                Patient            = p,
                RecentAppointments = _db.Appointments
                    .Where(a => a.PatientId == id)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Take(5)
                    .Select(a => a.Hydrate(_db))
                    .ToList(),
                RecentInvoices     = _db.Invoices
                    .Where(i => i.PatientId == id)
                    .OrderByDescending(i => i.InvoiceDate)
                    .Take(5)
                    .Select(i => i.Hydrate(_db))
                    .ToList(),
                CurrentBed         = _db.Beds.FirstOrDefault(b => b.OccupiedByPatientId == id)
            };
        }

        public Patient Create(PatientFormViewModel vm)
        {
            var p = new Patient {
                Id=_db.NextPatientId(), FirstName=vm.FirstName, LastName=vm.LastName,
                DateOfBirth=vm.DateOfBirth, Gender=vm.Gender, Phone=vm.Phone,
                Email=vm.Email, BloodGroup=vm.BloodGroup, Address=vm.Address,
                MedicalHistory=vm.MedicalHistory, Allergies=vm.Allergies,
                Status=vm.Status, RegisteredOn=DateTime.Now
            };
            _db.Patients.Add(p);
            return p;
        }

        public Patient? Update(int id, PatientFormViewModel vm)
        {
            var p = GetById(id);
            if (p == null) return null;
            p.FirstName=vm.FirstName; p.LastName=vm.LastName; p.DateOfBirth=vm.DateOfBirth;
            p.Gender=vm.Gender; p.Phone=vm.Phone; p.Email=vm.Email;
            p.BloodGroup=vm.BloodGroup; p.Address=vm.Address;
            p.MedicalHistory=vm.MedicalHistory; p.Allergies=vm.Allergies; p.Status=vm.Status;
            return p;
        }

        public bool Delete(int id)
        {
            var p = GetById(id);
            if (p == null) return false;
            _db.Patients.Remove(p);
            return true;
        }

        public List<Patient> GetAll() => _db.Patients.OrderBy(p => p.FirstName).ToList();
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly InMemoryStore _db;
        public AppointmentService(InMemoryStore db) => _db = db;

        public AppointmentListViewModel GetAppointments(string? search, string? status, string? date, int page, int pageSize)
        {
            var q = _db.Appointments.Select(a => a.Hydrate(_db)).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(a =>
                    (a.Patient?.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Patient?.LastName .Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Doctor ?.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Doctor ?.LastName .Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<AppointmentStatus>(status, out var s))
                q = q.Where(a => a.Status == s);
            if (date == "today")    q = q.Where(a => a.AppointmentDate.Date == DateTime.Today);
            else if (date == "upcoming") q = q.Where(a => a.AppointmentDate.Date >= DateTime.Today);
            else if (date == "past")     q = q.Where(a => a.AppointmentDate.Date <  DateTime.Today);
            var list = q.OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime).ToList();
            return new AppointmentListViewModel
            {
                Appointments=list.Skip((page-1)*pageSize).Take(pageSize).ToList(),
                TotalCount=list.Count, Page=page, PageSize=pageSize,
                SearchTerm=search, StatusFilter=status, DateFilter=date
            };
        }

        public Appointment? GetById(int id)
        {
            var a = _db.Appointments.FirstOrDefault(x => x.Id == id);
            return a == null ? null : a.Hydrate(_db);
        }

        public Appointment Create(AppointmentFormViewModel vm)
        {
            var a = new Appointment {
                Id=_db.NextAppointmentId(), PatientId=vm.PatientId, DoctorId=vm.DoctorId,
                AppointmentDate=vm.AppointmentDate,
                AppointmentTime=TimeSpan.Parse(vm.AppointmentTime),
                Department=vm.Department, Type=vm.Type, Status=vm.Status,
                Notes=vm.Notes, CreatedOn=DateTime.Now
            };
            _db.Appointments.Add(a);
            return a;
        }

        public Appointment? Update(int id, AppointmentFormViewModel vm)
        {
            var a = _db.Appointments.FirstOrDefault(x => x.Id == id);
            if (a == null) return null;
            a.PatientId=vm.PatientId; a.DoctorId=vm.DoctorId;
            a.AppointmentDate=vm.AppointmentDate;
            a.AppointmentTime=TimeSpan.Parse(vm.AppointmentTime);
            a.Department=vm.Department; a.Type=vm.Type; a.Status=vm.Status; a.Notes=vm.Notes;
            return a.Hydrate(_db);
        }

        public bool Delete(int id)
        {
            var a = _db.Appointments.FirstOrDefault(x => x.Id == id);
            if (a == null) return false;
            _db.Appointments.Remove(a);
            return true;
        }

        public List<Appointment> GetToday() =>
            _db.Appointments
               .Where(a => a.AppointmentDate.Date == DateTime.Today)
               .OrderBy(a => a.AppointmentTime)
               .Select(a => a.Hydrate(_db))
               .ToList();

        public ScheduleViewModel GetWeekSchedule(DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(6);
            var appts   = _db.Appointments
                .Where(a => a.AppointmentDate.Date >= weekStart.Date && a.AppointmentDate.Date <= weekEnd.Date)
                .OrderBy(a => a.AppointmentTime)
                .Select(a => a.Hydrate(_db))
                .ToList();
            var days = Enumerable.Range(0,7).Select(i => new DaySchedule
            {
                Date         = weekStart.AddDays(i),
                Appointments = appts.Where(a => a.AppointmentDate.Date == weekStart.AddDays(i).Date).ToList()
            }).ToList();
            return new ScheduleViewModel { WeekStart=weekStart, WeekEnd=weekEnd, Days=days };
        }
    }

    public class DoctorService : IDoctorService
    {
        private readonly InMemoryStore _db;
        public DoctorService(InMemoryStore db) => _db = db;

        public DoctorListViewModel GetDoctors(string? search, string? dept, string? status)
        {
            var q = _db.Doctors.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(d => d.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)
                               || d.LastName .Contains(search, StringComparison.OrdinalIgnoreCase)
                               || d.Specialization.Contains(search, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(dept))   q = q.Where(d => d.Department == dept);
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<DoctorStatus>(status, out var s))
                q = q.Where(d => d.Status == s);
            return new DoctorListViewModel
            {
                Doctors=q.OrderBy(d => d.FirstName).ToList(),
                SearchTerm=search, DepartmentFilter=dept, StatusFilter=status
            };
        }

        public Doctor? GetById(int id) => _db.Doctors.FirstOrDefault(d => d.Id == id);

        public Doctor Create(DoctorFormViewModel vm)
        {
            var d = new Doctor {
                Id=_db.NextDoctorId(), FirstName=vm.FirstName, LastName=vm.LastName,
                Specialization=vm.Specialization, Department=vm.Department,
                Phone=vm.Phone, Email=vm.Email, ExperienceYears=vm.ExperienceYears,
                Qualification=vm.Qualification, LicenseNumber=vm.LicenseNumber,
                Status=vm.Status, AvatarColor=vm.AvatarColor, JoinedOn=DateTime.Now
            };
            _db.Doctors.Add(d);
            return d;
        }

        public Doctor? Update(int id, DoctorFormViewModel vm)
        {
            var d = GetById(id);
            if (d == null) return null;
            d.FirstName=vm.FirstName; d.LastName=vm.LastName;
            d.Specialization=vm.Specialization; d.Department=vm.Department;
            d.Phone=vm.Phone; d.Email=vm.Email; d.ExperienceYears=vm.ExperienceYears;
            d.Qualification=vm.Qualification; d.LicenseNumber=vm.LicenseNumber;
            d.Status=vm.Status; d.AvatarColor=vm.AvatarColor;
            return d;
        }

        public bool Delete(int id)
        {
            var d = GetById(id);
            if (d == null) return false;
            _db.Doctors.Remove(d);
            return true;
        }

        public List<Doctor> GetAll() => _db.Doctors.OrderBy(d => d.FirstName).ToList();
    }

    public class PharmacyService : IPharmacyService
    {
        private readonly InMemoryStore _db;
        public PharmacyService(InMemoryStore db) => _db = db;

        public PharmacyViewModel GetMedicines(string? search, string? category, string? stock)
        {
            var q = _db.Medicines.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(m => m.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                               || m.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(category)) q = q.Where(m => m.Category == category);
            var all = q.OrderBy(m => m.Name).ToList();
            if (stock == "low")      all = all.Where(m => m.StockStatus == StockStatus.LowStock || m.StockStatus == StockStatus.Critical).ToList();
            else if (stock == "expiring") all = all.Where(m => m.IsExpiringSoon).ToList();
            var total = _db.Medicines.ToList();
            return new PharmacyViewModel
            {
                Medicines=all, SearchTerm=search, CategoryFilter=category, StockFilter=stock,
                TotalItems=total.Count,
                LowStockCount=total.Count(m => m.StockStatus==StockStatus.LowStock || m.StockStatus==StockStatus.Critical),
                ExpiringCount=total.Count(m => m.IsExpiringSoon),
                TodayPrescriptions=64
            };
        }

        public Medicine? GetById(int id) => _db.Medicines.FirstOrDefault(m => m.Id == id);

        public Medicine Create(MedicineFormViewModel vm)
        {
            var m = new Medicine {
                Id=_db.NextMedicineId(), Name=vm.Name, Category=vm.Category,
                Manufacturer=vm.Manufacturer, BatchNumber=vm.BatchNumber, Form=vm.Form,
                StockQuantity=vm.StockQuantity, MinimumStock=vm.MinimumStock,
                MaximumStock=vm.MaximumStock, UnitPrice=vm.UnitPrice, ExpiryDate=vm.ExpiryDate
            };
            _db.Medicines.Add(m);
            return m;
        }

        public Medicine? Update(int id, MedicineFormViewModel vm)
        {
            var m = GetById(id);
            if (m == null) return null;
            m.Name=vm.Name; m.Category=vm.Category; m.Manufacturer=vm.Manufacturer;
            m.Form=vm.Form; m.StockQuantity=vm.StockQuantity; m.MinimumStock=vm.MinimumStock;
            m.MaximumStock=vm.MaximumStock; m.UnitPrice=vm.UnitPrice; m.ExpiryDate=vm.ExpiryDate;
            return m;
        }

        public bool Delete(int id)
        {
            var m = GetById(id);
            if (m == null) return false;
            _db.Medicines.Remove(m);
            return true;
        }
    }

    public class BillingService : IBillingService
    {
        private readonly InMemoryStore _db;
        public BillingService(InMemoryStore db) => _db = db;

        public BillingViewModel GetInvoices(string? search, string? status, int page)
        {
            const int pageSize = 10;
            var q = _db.Invoices.Select(i => i.Hydrate(_db)).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(i => (i.Patient?.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                               || (i.Patient?.LastName .Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<InvoiceStatus>(status, out var s))
                q = q.Where(i => i.Status == s);
            var list  = q.OrderByDescending(i => i.InvoiceDate).ToList();
            var all   = _db.Invoices.ToList();
            return new BillingViewModel
            {
                Invoices=list.Skip((page-1)*pageSize).Take(pageSize).ToList(),
                TotalCount=list.Count, Page=page,
                TotalCollected =all.Where(i => i.Status==InvoiceStatus.Paid).Sum(i => i.PaidAmount),
                TotalPending   =all.Where(i => i.Status==InvoiceStatus.Pending||i.Status==InvoiceStatus.Partial).Sum(i => i.TotalAmount-i.PaidAmount),
                TotalOverdue   =all.Where(i => i.Status==InvoiceStatus.Overdue).Sum(i => i.TotalAmount-i.PaidAmount),
                InsuranceClaims=all.Count(i => i.PaymentMethod==PaymentMethod.Insurance && i.Status!=InvoiceStatus.Paid),
                SearchTerm=search, StatusFilter=status
            };
        }

        public Invoice? GetById(int id)
        {
            var inv = _db.Invoices.FirstOrDefault(i => i.Id == id);
            return inv == null ? null : inv.Hydrate(_db);
        }

        public Invoice Create(InvoiceFormViewModel vm)
        {
            var sub   = vm.Items.Sum(x => x.Quantity * x.UnitPrice);
            var tax   = sub * vm.TaxRate / 100;
            var total = sub - vm.Discount + tax;
            var inv   = new Invoice {
                Id=_db.NextInvoiceId(), PatientId=vm.PatientId, Services=vm.Services,
                SubTotal=sub, Discount=vm.Discount, TaxAmount=tax, TotalAmount=total,
                PaidAmount=vm.PaidAmount, PaymentMethod=vm.PaymentMethod,
                Status = vm.PaidAmount >= total ? InvoiceStatus.Paid
                       : vm.PaidAmount > 0      ? InvoiceStatus.Partial
                       : vm.Status,
                Notes=vm.Notes, DueDate=vm.DueDate, InvoiceDate=DateTime.Now
            };
            _db.Invoices.Add(inv);
            foreach (var item in vm.Items.Where(i => !string.IsNullOrWhiteSpace(i.Description)))
                _db.InvoiceItems.Add(new InvoiceItem {
                    Id=_db.NextInvoiceItemId(), InvoiceId=inv.Id,
                    Description=item.Description, Quantity=item.Quantity, UnitPrice=item.UnitPrice
                });
            return inv;
        }

        public Invoice? Update(int id, InvoiceFormViewModel vm)
        {
            var inv = _db.Invoices.FirstOrDefault(i => i.Id == id);
            if (inv == null) return null;
            inv.PaidAmount=vm.PaidAmount; inv.Status=vm.Status; inv.Notes=vm.Notes;
            return inv;
        }

        public bool Delete(int id)
        {
            var inv = _db.Invoices.FirstOrDefault(i => i.Id == id);
            if (inv == null) return false;
            _db.Invoices.Remove(inv);
            _db.InvoiceItems.RemoveAll(ii => ii.InvoiceId == id);
            return true;
        }
    }

    public class DashboardService : IDashboardService
    {
        private readonly InMemoryStore _db;
        public DashboardService(InMemoryStore db) => _db = db;

        public DashboardViewModel GetDashboard()
        {
            var today      = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var todayAppts = _db.Appointments
                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentTime)
                .Select(a => a.Hydrate(_db))
                .ToList();
            var totalBeds     = _db.Beds.Count;
            var availableBeds = _db.Beds.Count(b => b.Status == BedStatus.Available);
            var monthRevenue  = _db.Invoices.Where(i => i.InvoiceDate >= monthStart).Sum(i => i.PaidAmount);
            var newPatients   = _db.Patients.Count(p => p.RegisteredOn >= monthStart);
            var pending       = _db.Appointments.Count(a => a.Status == AppointmentStatus.Pending);
            var lowStock      = _db.Medicines.Count(m => m.StockQuantity <= m.MinimumStock);

            var depts   = new[] { "Cardiology","Neurology","Orthopedics","Pediatrics","Gynecology" };
            var caps    = new[] { 50, 40, 40, 35, 35 };
            var defLoad = new[] { 82, 67, 54, 41, 58 };
            var deptLoads = depts.Select((d, i) =>
            {
                var cnt = _db.Appointments.Count(a => a.Department == d && a.AppointmentDate.Date == today);
                return new DepartmentLoad { Department=d, PatientCount=cnt>0?cnt:defLoad[i], Capacity=caps[i] };
            }).ToList();

            var weeklyAdmissions = Enumerable.Range(0,7).Select(i =>
            {
                var d = today.AddDays(-6+i);
                var cnt = _db.Appointments.Count(a => a.AppointmentDate.Date == d.Date);
                return new WeeklyAdmission { Day=d.ToString("ddd"), Count=cnt>0?cnt:new Random(d.DayOfYear+i).Next(20,75) };
            }).ToList();

            return new DashboardViewModel
            {
                TotalPatients      = _db.Patients.Count,
                TodayAppointments  = todayAppts.Count,
                AvailableBeds      = availableBeds,
                TotalBeds          = totalBeds,
                MonthlyRevenue     = monthRevenue,
                NewPatientsThisMonth = newPatients,
                PendingAppointments  = pending,
                LowStockMedicines    = lowStock,
                TodayAppointmentList = todayAppts,
                WeeklyAdmissions     = weeklyAdmissions,
                DepartmentLoads      = deptLoads,
                Alerts = new List<AlertItem>
                {
                    new() { Message="Omeprazole stock critically low (45 units). Reorder immediately.", Color="#ef4444", TimeAgo="10 min ago" },
                    new() { Message="Mohammed Khan's appointment pending confirmation.", Color="#f59e0b", TimeAgo="32 min ago" },
                    new() { Message="New patient registered: Deepak Verma.", Color="#00b4d8", TimeAgo="1 hour ago" },
                    new() { Message="Invoice #INV-0001 paid by Priya Reddy · ₹2,400.", Color="#10b981", TimeAgo="2 hours ago" },
                }
            };
        }
    }
}
