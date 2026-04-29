using HospitalMS.Services;
using HospitalMS.ViewModels;
using HospitalMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalMS.Controllers
{
    // ── Dashboard ─────────────────────────────────────────────────────────────
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboard;
        public HomeController(IDashboardService d) => _dashboard = d;
        public IActionResult Index() => View(_dashboard.GetDashboard());
    }

    // ── Patient ───────────────────────────────────────────────────────────────
    public class PatientController : Controller
    {
        private readonly IPatientService _patients;
        public PatientController(IPatientService p) => _patients = p;

        public IActionResult Index(string? search, string? status, int page = 1)
            => View(_patients.GetPatients(search, status, page, 10));

        public IActionResult Detail(int id)
        {
            var vm = _patients.GetDetail(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        public IActionResult Create() => View(new PatientFormViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(PatientFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var p = _patients.Create(vm);
            TempData["Success"] = $"Patient {p.FullName} registered successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var p = _patients.GetById(id);
            if (p == null) return NotFound();
            return View(new PatientFormViewModel {
                Id=p.Id, FirstName=p.FirstName, LastName=p.LastName,
                DateOfBirth=p.DateOfBirth, Gender=p.Gender, Phone=p.Phone,
                Email=p.Email, BloodGroup=p.BloodGroup, Address=p.Address,
                MedicalHistory=p.MedicalHistory, Allergies=p.Allergies, Status=p.Status
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PatientFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var p = _patients.Update(id, vm);
            if (p == null) return NotFound();
            TempData["Success"] = "Patient record updated successfully.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _patients.Delete(id);
            TempData["Success"] = "Patient record deleted.";
            return RedirectToAction(nameof(Index));
        }
    }

    // ── Appointment ───────────────────────────────────────────────────────────
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointments;
        private readonly IPatientService     _patients;
        private readonly IDoctorService      _doctors;
        public AppointmentController(IAppointmentService a, IPatientService p, IDoctorService d)
        { _appointments=a; _patients=p; _doctors=d; }

        public IActionResult Index(string? search, string? status, string? date, int page = 1)
            => View(_appointments.GetAppointments(search, status, date, page, 10));

        public IActionResult Detail(int id)
        {
            var a = _appointments.GetById(id);
            if (a == null) return NotFound();
            return View(a);
        }

        public IActionResult Create()
            => View(new AppointmentFormViewModel {
                Patients=_patients.GetAll(), Doctors=_doctors.GetAll(),
                AppointmentDate=DateTime.Today, AppointmentTime="09:00"
            });

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            { vm.Patients=_patients.GetAll(); vm.Doctors=_doctors.GetAll(); return View(vm); }
            _appointments.Create(vm);
            TempData["Success"] = "Appointment booked successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var a = _appointments.GetById(id);
            if (a == null) return NotFound();
            return View(new AppointmentFormViewModel {
                Id=a.Id, PatientId=a.PatientId, DoctorId=a.DoctorId,
                AppointmentDate=a.AppointmentDate,
                AppointmentTime=a.AppointmentTime.ToString(@"hh\:mm"),
                Department=a.Department, Type=a.Type, Status=a.Status, Notes=a.Notes,
                Patients=_patients.GetAll(), Doctors=_doctors.GetAll()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AppointmentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            { vm.Patients=_patients.GetAll(); vm.Doctors=_doctors.GetAll(); return View(vm); }
            _appointments.Update(id, vm);
            TempData["Success"] = "Appointment updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _appointments.Delete(id);
            TempData["Success"] = "Appointment deleted.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Schedule(string? date)
        {
            var weekStart = date != null
                ? DateTime.Parse(date)
                : DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            if (weekStart.DayOfWeek == DayOfWeek.Sunday) weekStart = weekStart.AddDays(1);
            return View(_appointments.GetWeekSchedule(weekStart));
        }
    }

    // ── Doctor ────────────────────────────────────────────────────────────────
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctors;
        public DoctorController(IDoctorService d) => _doctors = d;

        public IActionResult Index(string? search, string? dept, string? status)
            => View(_doctors.GetDoctors(search, dept, status));

        public IActionResult Create() => View(new DoctorFormViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(DoctorFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var d = _doctors.Create(vm);
            TempData["Success"] = $"{d.FullName} added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var d = _doctors.GetById(id);
            if (d == null) return NotFound();
            return View(new DoctorFormViewModel {
                Id=d.Id, FirstName=d.FirstName, LastName=d.LastName,
                Specialization=d.Specialization, Department=d.Department,
                Phone=d.Phone, Email=d.Email, ExperienceYears=d.ExperienceYears,
                Qualification=d.Qualification, LicenseNumber=d.LicenseNumber,
                Status=d.Status, AvatarColor=d.AvatarColor
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DoctorFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            _doctors.Update(id, vm);
            TempData["Success"] = "Doctor record updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _doctors.Delete(id);
            TempData["Success"] = "Doctor removed.";
            return RedirectToAction(nameof(Index));
        }
    }

    // ── Pharmacy ──────────────────────────────────────────────────────────────
    public class PharmacyController : Controller
    {
        private readonly IPharmacyService _pharmacy;
        public PharmacyController(IPharmacyService p) => _pharmacy = p;

        public IActionResult Index(string? search, string? category, string? stock)
            => View(_pharmacy.GetMedicines(search, category, stock));

        public IActionResult Create() => View(new MedicineFormViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(MedicineFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            _pharmacy.Create(vm);
            TempData["Success"] = "Medicine added to inventory.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var m = _pharmacy.GetById(id);
            if (m == null) return NotFound();
            return View(new MedicineFormViewModel {
                Id=m.Id, Name=m.Name, Category=m.Category, Manufacturer=m.Manufacturer,
                BatchNumber=m.BatchNumber, Form=m.Form, StockQuantity=m.StockQuantity,
                MinimumStock=m.MinimumStock, MaximumStock=m.MaximumStock,
                UnitPrice=m.UnitPrice, ExpiryDate=m.ExpiryDate
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MedicineFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            _pharmacy.Update(id, vm);
            TempData["Success"] = "Medicine updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _pharmacy.Delete(id);
            TempData["Success"] = "Medicine removed.";
            return RedirectToAction(nameof(Index));
        }
    }

    // ── Billing ───────────────────────────────────────────────────────────────
    public class BillingController : Controller
    {
        private readonly IBillingService _billing;
        private readonly IPatientService _patients;
        public BillingController(IBillingService b, IPatientService p) { _billing=b; _patients=p; }

        public IActionResult Index(string? search, string? status, int page = 1)
            => View(_billing.GetInvoices(search, status, page));

        public IActionResult Detail(int id)
        {
            var inv = _billing.GetById(id);
            if (inv == null) return NotFound();
            return View(inv);
        }

        public IActionResult Create()
            => View(new InvoiceFormViewModel { Patients=_patients.GetAll() });

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(InvoiceFormViewModel vm)
        {
            if (!ModelState.IsValid)
            { vm.Patients=_patients.GetAll(); return View(vm); }
            var inv = _billing.Create(vm);
            TempData["Success"] = $"Invoice #{inv.InvoiceNumber} created.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var inv = _billing.GetById(id);
            if (inv == null) return NotFound();
            return View(new InvoiceFormViewModel {
                Id=inv.Id, PatientId=inv.PatientId, Services=inv.Services,
                SubTotal=inv.SubTotal, Discount=inv.Discount,
                PaidAmount=inv.PaidAmount, PaymentMethod=inv.PaymentMethod,
                Status=inv.Status, Notes=inv.Notes, DueDate=inv.DueDate,
                Patients=_patients.GetAll()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, InvoiceFormViewModel vm)
        {
            if (!ModelState.IsValid)
            { vm.Patients=_patients.GetAll(); return View(vm); }
            _billing.Update(id, vm);
            TempData["Success"] = "Invoice updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _billing.Delete(id);
            TempData["Success"] = "Invoice deleted.";
            return RedirectToAction(nameof(Index));
        }
    }

    // ── Reports ───────────────────────────────────────────────────────────────
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            var months   = new[] {"May","Jun","Jul","Aug","Sep","Oct","Nov","Dec","Jan","Feb","Mar","Apr"};
            var revenues = new decimal[] {1800000,2100000,1950000,2550000,2440000,2700000,2510000,2325000,2950000,3060000,2900000,3150000};
            var patients = new[] {380,420,395,510,488,540,502,465,590,612,580,630};
            var vm = new ReportsViewModel
            {
                MonthlyRevenue=3150000, MonthlyPatients=630,
                RevenueBreakdown = new RevenueBreakdown {
                    Consultations=8400000, Surgeries=9800000, Pharmacy=3200000, LabImaging=3200000
                },
                MonthlyRevenues  = months.Select((m,i) => new MonthlyRevenue { Month=m, Revenue=revenues[i], Patients=patients[i] }).ToList(),
                DepartmentStats  = new List<DepartmentStats> {
                    new() {Department="Cardiology",PatientCount=284,Revenue=9200000,LoadLevel="High load"},
                    new() {Department="Orthopedics",PatientCount=218,Revenue=12400000,LoadLevel="Medium"},
                    new() {Department="Neurology",PatientCount=196,Revenue=7800000,LoadLevel="Medium"},
                    new() {Department="Gynecology",PatientCount=178,Revenue=4200000,LoadLevel="Normal"},
                    new() {Department="Pediatrics",PatientCount=145,Revenue=2900000,LoadLevel="Normal"},
                }
            };
            return View(vm);
        }
    }

    // ── Settings ──────────────────────────────────────────────────────────────
    public class SettingsController : Controller
    {
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult SaveHospitalInfo()
        {
            TempData["Success"] = "Hospital information saved successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdatePassword()
        {
            TempData["Success"] = "Password updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
