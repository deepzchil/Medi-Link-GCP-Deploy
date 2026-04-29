using HospitalMS.Models;

namespace HospitalMS.Data
{
    /// <summary>
    /// Thread-safe in-memory data store. Seeded once on startup.
    /// </summary>
    public class InMemoryStore
    {
        private int _nextPatientId    = 1;
        private int _nextDoctorId     = 1;
        private int _nextAppointmentId= 1;
        private int _nextMedicineId   = 1;
        private int _nextInvoiceId    = 1;
        private int _nextInvoiceItemId= 1;
        private int _nextBedId        = 1;

        public List<Patient>     Patients     { get; } = new();
        public List<Doctor>      Doctors      { get; } = new();
        public List<Appointment> Appointments { get; } = new();
        public List<Medicine>    Medicines    { get; } = new();
        public List<Invoice>     Invoices     { get; } = new();
        public List<InvoiceItem> InvoiceItems { get; } = new();
        public List<Bed>         Beds         { get; } = new();

        public int NextPatientId()     => _nextPatientId++;
        public int NextDoctorId()      => _nextDoctorId++;
        public int NextAppointmentId() => _nextAppointmentId++;
        public int NextMedicineId()    => _nextMedicineId++;
        public int NextInvoiceId()     => _nextInvoiceId++;
        public int NextInvoiceItemId() => _nextInvoiceItemId++;
        public int NextBedId()         => _nextBedId++;

        public InMemoryStore() { Seed(); }

        private void Seed()
        {
            // ── Doctors ──────────────────────────────────────────────────────
            var docData = new[]
            {
                ("Ananya","Sharma","Cardiologist","Cardiology","+91 98765 11001","ananya@medicare.in",8,"MD, DM Cardiology","MCI-2016-001",DoctorStatus.Available,"#00b4d8"),
                ("Rajiv","Menon","Neurologist","Neurology","+91 98765 11002","rajiv@medicare.in",12,"MD, DM Neurology","MCI-2012-002",DoctorStatus.Busy,"#8b5cf6"),
                ("Vikram","Rao","Orthopedic Surgeon","Orthopedics","+91 98765 11003","vikram@medicare.in",15,"MS Orthopedics","MCI-2009-003",DoctorStatus.InSurgery,"#10b981"),
                ("Priya","Venkat","Dermatologist","Dermatology","+91 98765 11004","priya.v@medicare.in",6,"MD Dermatology","MCI-2018-004",DoctorStatus.Available,"#f59e0b"),
                ("Sanjay","Gupta","Gynecologist","Gynecology","+91 98765 11005","sanjay@medicare.in",10,"MS Gynecology","MCI-2014-005",DoctorStatus.Available,"#ef4444"),
                ("Deepa","Nair","Pediatrician","Pediatrics","+91 98765 11006","deepa@medicare.in",9,"MD Pediatrics","MCI-2015-006",DoctorStatus.OffDuty,"#0ea5e9"),
                ("Kavitha","Pillai","General Physician","General Medicine","+91 98765 11007","kavitha@medicare.in",11,"MD General Medicine","MCI-2013-007",DoctorStatus.Available,"#6366f1"),
                ("Arun","Kumar","Radiologist","Radiology","+91 98765 11008","arun@medicare.in",7,"MD Radiology","MCI-2017-008",DoctorStatus.Busy,"#14b8a6"),
            };
            foreach (var (fn,ln,spec,dept,ph,em,exp,qual,lic,stat,col) in docData)
            {
                Doctors.Add(new Doctor {
                    Id=NextDoctorId(), FirstName=fn, LastName=ln, Specialization=spec,
                    Department=dept, Phone=ph, Email=em, ExperienceYears=exp,
                    Qualification=qual, LicenseNumber=lic, Status=stat, AvatarColor=col,
                    JoinedOn=DateTime.Now.AddYears(-exp+2)
                });
            }

            // ── Patients ─────────────────────────────────────────────────────
            var patData = new[]
            {
                ("Priya","Reddy",new DateTime(1990,3,15),"Female","+91 98765 21001","priya@email.com","B+","45 MG Road, Hyderabad",PatientStatus.Active),
                ("Mohammed","Khan",new DateTime(1972,7,22),"Male","+91 98765 21002","m.khan@email.com","O+","12 Banjara Hills, Hyderabad",PatientStatus.FollowUp),
                ("Sunita","Nair",new DateTime(1983,11,8),"Female","+91 98765 21003","sunita@email.com","A+","78 Jubilee Hills, Hyderabad",PatientStatus.Admitted),
                ("Arjun","Joshi",new DateTime(1996,5,20),"Male","+91 98765 21004","arjun@email.com","AB+","23 Madhapur, Hyderabad",PatientStatus.Active),
                ("Lakshmi","Iyer",new DateTime(1986,9,12),"Female","+91 98765 21005","lakshmi@email.com","O-","56 Kondapur, Hyderabad",PatientStatus.Active),
                ("Ravi","Shankar",new DateTime(2000,2,28),"Male","+91 98765 21006","ravi@email.com","B-","89 Gachibowli, Hyderabad",PatientStatus.Active),
                ("Anita","Pillai",new DateTime(1978,6,3),"Female","+91 98765 21007","anita@email.com","A-","34 Kukatpally, Hyderabad",PatientStatus.Active),
                ("Deepak","Verma",new DateTime(1991,12,17),"Male","+91 98765 21008","deepak@email.com","O+","67 HITEC City, Hyderabad",PatientStatus.Active),
            };
            int pOffset = 30;
            foreach (var (fn,ln,dob,gender,ph,em,bg,addr,stat) in patData)
            {
                Patients.Add(new Patient {
                    Id=NextPatientId(), FirstName=fn, LastName=ln, DateOfBirth=dob,
                    Gender=gender, Phone=ph, Email=em, BloodGroup=bg, Address=addr,
                    Status=stat, RegisteredOn=DateTime.Now.AddDays(-pOffset)
                });
                pOffset += 10;
            }

            // ── Appointments ─────────────────────────────────────────────────
            var apptData = new[]
            {
                (1,1,0,new TimeSpan(9,0,0),"Cardiology",AppointmentType.Consultation,AppointmentStatus.Confirmed,"Chest pain follow-up"),
                (2,2,0,new TimeSpan(10,30,0),"Neurology",AppointmentType.FollowUp,AppointmentStatus.Pending,""),
                (3,3,0,new TimeSpan(11,0,0),"Orthopedics",AppointmentType.Surgery,AppointmentStatus.Confirmed,""),
                (4,4,0,new TimeSpan(14,15,0),"Dermatology",AppointmentType.Consultation,AppointmentStatus.InProgress,""),
                (5,5,1,new TimeSpan(9,30,0),"Gynecology",AppointmentType.Checkup,AppointmentStatus.Confirmed,""),
                (6,6,1,new TimeSpan(11,0,0),"Pediatrics",AppointmentType.Consultation,AppointmentStatus.Cancelled,""),
                (7,7,2,new TimeSpan(10,0,0),"General Medicine",AppointmentType.Checkup,AppointmentStatus.Confirmed,""),
                (1,1,-7,new TimeSpan(9,0,0),"Cardiology",AppointmentType.Consultation,AppointmentStatus.Completed,"Hypertension Grade 2"),
            };
            foreach (var (pid,did,dayOffset,time,dept,type,stat,notes) in apptData)
            {
                Appointments.Add(new Appointment {
                    Id=NextAppointmentId(), PatientId=pid, DoctorId=did,
                    AppointmentDate=DateTime.Today.AddDays(dayOffset),
                    AppointmentTime=time, Department=dept, Type=type, Status=stat,
                    Notes=string.IsNullOrEmpty(notes)?null:notes, CreatedOn=DateTime.Now
                });
            }

            // ── Medicines ────────────────────────────────────────────────────
            var medData = new[]
            {
                ("Amoxicillin 500mg","Antibiotic","Cipla","AMX-2024-001",MedicineForm.Capsule,850,100,1000,12m,new DateTime(2026,12,31)),
                ("Metformin 1000mg","Anti-diabetic","Sun Pharma","MET-2024-002",MedicineForm.Tablet,120,200,500,28m,new DateTime(2026,6,30)),
                ("Atorvastatin 20mg","Statin","Lupin","ATV-2024-003",MedicineForm.Tablet,680,100,1000,45m,new DateTime(2027,3,31)),
                ("Omeprazole 20mg","Antacid","Dr. Reddy's","OMP-2024-004",MedicineForm.Capsule,45,150,500,18m,new DateTime(2026,5,31)),
                ("Paracetamol 500mg","Analgesic","GSK","PCM-2024-005",MedicineForm.Tablet,1200,200,1200,8m,new DateTime(2027,1,31)),
                ("Amlodipine 5mg","Anti-hypertensive","Abbott","AML-2024-006",MedicineForm.Tablet,400,100,600,22m,new DateTime(2026,11,30)),
                ("Azithromycin 500mg","Antibiotic","Cipla","AZT-2024-007",MedicineForm.Tablet,80,100,300,65m,new DateTime(2026,8,31)),
                ("Insulin Glargine","Anti-diabetic","Novo Nordisk","INS-2024-008",MedicineForm.Injection,60,50,200,850m,new DateTime(2026,7,31)),
            };
            foreach (var (name,cat,mfr,batch,form,qty,minQ,maxQ,price,exp) in medData)
            {
                Medicines.Add(new Medicine {
                    Id=NextMedicineId(), Name=name, Category=cat, Manufacturer=mfr,
                    BatchNumber=batch, Form=form, StockQuantity=qty,
                    MinimumStock=minQ, MaximumStock=maxQ, UnitPrice=price, ExpiryDate=exp
                });
            }

            // ── Invoices ─────────────────────────────────────────────────────
            var invData = new[]
            {
                (1,2000m,0m,400m,2400m,2400m,PaymentMethod.UPI,InvoiceStatus.Paid,"Cardiology Consultation + ECG",0),
                (2,7200m,0m,1300m,8500m,0m,PaymentMethod.Insurance,InvoiceStatus.Pending,"MRI Brain + Neurologist Consultation",7),
                (3,35593m,2000m,6407m,42000m,20000m,PaymentMethod.Insurance,InvoiceStatus.Partial,"Knee Replacement Surgery + 3 days Ward",14),
                (6,678m,0m,122m,800m,0m,PaymentMethod.Cash,InvoiceStatus.Overdue,"Pediatric Consultation",-1),
            };
            foreach (var (pid,sub,disc,tax,total,paid,pm,stat,svc,dueDays) in invData)
            {
                var inv = new Invoice {
                    Id=NextInvoiceId(), PatientId=pid, SubTotal=sub, Discount=disc,
                    TaxAmount=tax, TotalAmount=total, PaidAmount=paid,
                    PaymentMethod=pm, Status=stat, Services=svc,
                    InvoiceDate=DateTime.Today.AddDays(-Math.Abs(dueDays)),
                    DueDate=DateTime.Today.AddDays(dueDays)
                };
                Invoices.Add(inv);
            }

            // ── Beds ──────────────────────────────────────────────────────────
            var wards = new[] { ("General",500m,30), ("ICU",3000m,10), ("Private",2000m,20), ("Semi-Private",1000m,20) };
            int bedNum = 1;
            foreach (var (ward, rate, count) in wards)
            {
                for (int b = 1; b <= count; b++)
                {
                    Beds.Add(new Bed {
                        Id=NextBedId(),
                        BedNumber=$"{ward[0]}{bedNum:D3}",
                        Ward=ward, WardType=ward,
                        Status = b <= count/2 ? BedStatus.Occupied : BedStatus.Available,
                        DailyRate=rate
                    });
                    bedNum++;
                }
            }
        }
    }
}
