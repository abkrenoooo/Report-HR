using Report_HR.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Report_HR.Controllers
{
    public class HomeController : Controller
    {
        ModelDB DB = new ModelDB();

        #region Create
        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            try
            {
                DB.Entry(employee).State = EntityState.Added;
                DB.SaveChanges();
                return RedirectToAction("Create");
            }
            catch
            {
                return View("Error");
            }
        }
        #endregion

        #region Settings
        // GET: Home/Settings
        public ActionResult Settings()
        {
            ViewBag.Employee = DB.Employees.ToList();
            string emp = null;
            return View(emp);
        }

        // POST: Home/Settings
        [HttpPost]
        public ActionResult Settings(int EmployeeID, string Pons, string Mins, string Absence, double PonsValue, double MinsValue, double AbsenceValue)
        {
            EmpSetting emp = new EmpSetting();
            var empSetting = DB.EmpSettings.Where(w => w.EmployeeId == EmployeeID).Count();
            try
            {
                if (empSetting > 0)
                {
                    return View("SetError");
                }
                else
                {
                    emp.EmployeeId = EmployeeID;

                    if (Pons == "Pons/Hour")
                    {
                        emp.HPonsH = PonsValue;
                        emp.HPonsP = 0;
                    }
                    else
                    {
                        emp.HPonsP = PonsValue;
                        emp.HPonsH = 0;
                    }
                    if (Mins != "Mins/Hour")
                    {
                        emp.HMinsP = MinsValue;
                        emp.HMinsH = 0;
                    }
                    else
                    {
                        emp.HMinsH = MinsValue;
                        emp.HMinsP = 0;
                    }
                    if (Absence == "Absence/Day")
                    {
                        emp.DAbsentD = AbsenceValue;
                        emp.DAbsentP = 0;
                    }
                    else
                    {
                        emp.DAbsentP = AbsenceValue;
                        emp.DAbsentD = 0;
                    }
                    DB.Entry(emp).State = EntityState.Added;
                    DB.SaveChanges();
                    return RedirectToAction("Settings");

                }
            }
            catch
            {
                return View("Error");
            }
        }
        #endregion

        #region Absence
        // GET: Home/Absence
        public ActionResult Absence()
        {
            ViewBag.Employee = DB.Employees.ToList();

            return View();
        }
        [HttpPost]
        public ActionResult Absence(Absence absence)
        {
            try
            {
                var employee = DB.Employees.Find(absence.EmployeeId);
                var absenc = DB.Absences.Where(w => w.Date == absence.Date && w.EmployeeId == absence.EmployeeId && w.state == "Absence").ToList();
                var Attend = DB.Absences.Where(w => w.Date == absence.Date && w.EmployeeId == absence.EmployeeId && w.state == "Attend").ToList();
                var leave = DB.Absences.Where(w => w.Date == absence.Date && w.EmployeeId == absence.EmployeeId && w.state == "leave").ToList();
                if (absence.state == "Attend" && (absenc.Count() > 0 || Attend.Count() > 0 || absence.Time < employee.TFrom))
                {
                    return View("repetition");
                }
                else if (absence.state == "leave" && (absenc.Count() > 0 || leave.Count() > 0 || Attend.Count() == 0 || Attend.LastOrDefault().Time > absence.Time))
                {
                    return View("repetition");
                }
                else if (absence.state == "Absent" && (Attend.Count() > 0 ))
                {
                    return View("repetition");
                }
                else
                {
                    DB.Entry(absence).State = EntityState.Added;
                    DB.SaveChanges();
                    return RedirectToAction("Absence");
                }
            }
            catch
            {
                return View("Error");
            }

        }
        #endregion

        #region AbsenceSalary
        // GET: Home/Report
        public ActionResult AbsenceReport()
        {
            ViewBag.Employee = DB.Employees.ToList();
            return View();
        }
        #endregion

        #region AbsenceReportShow
        public ActionResult AbsenceReportShow(int EmployeeId, DateTime DateFrom, DateTime DateTo)
        {
            var emp = DB.Absences.Where(w => w.EmployeeId == EmployeeId && w.Date >= DateFrom && w.Date <= DateTo).OrderBy(w => w.Date).ToList();

            return View(emp);
        }
        #endregion

        #region ReportSalary 
        // GET: Home/Report
        public ActionResult SalaryReport()
        {
            ViewBag.Employee = DB.Employees.ToList();
            return View();
        }
        #endregion

        #region SalaryReportShow
        public ActionResult SalaryReportShow(int EmployeeId, DateTime Month)
        {
            try
            {
                var employee = DB.Employees.Find(EmployeeId);
                var STA = employee.TFrom;
                var STL = employee.TTo;
                var ST0 = new TimeSpan(0, 0, 0);
                var PTime = new TimeSpan(0, 0, 0);
                var MTime = new TimeSpan(0, 0, 0);
                var empID = DB.EmpSettings.Where(w => w.EmployeeId == EmployeeId).ToList();
                var emp = empID.LastOrDefault();
                var Attend = DB.Absences.Where(w => w.EmployeeId == EmployeeId && w.Date.Month == Month.Month && w.Date.Year <= Month.Year && w.state == "Attend").Distinct().ToList();
                var leave = DB.Absences.Where(w => w.EmployeeId == EmployeeId && w.Date.Month == Month.Month && w.Date.Year <= Month.Year && w.state == "leave").Distinct().ToList();
                var AbsentCount = DB.Absences.Where(w => w.EmployeeId == EmployeeId && w.Date.Month == Month.Month && w.Date.Year <= Month.Year && w.state == "Absent").Distinct().Count();
                var AttendP = DB.EmpSettings.Where(w => w.EmployeeId == EmployeeId).Select(w => w.HPonsH + w.HPonsP);
                double AttendPC = 0.0f, leavePC = 0.0f, DayPC = 0.0f;
                var AttendDays = Attend.Count();
                var salary = employee.Salary;
                var MainSalary = (salary / (double)30) * AttendDays;
                if (emp.HPonsH == 0)
                {
                    AttendPC = emp.HPonsP;
                }
                else
                {
                    var OneHour = salary / (double)30 / (double)8;
                    AttendPC = emp.HPonsH * OneHour;
                }
                if (emp.HMinsH == 0)
                {
                    leavePC = emp.HMinsP;
                }
                else
                {
                    var OneHour = salary / (double)30 / (double)8;
                    leavePC = emp.HMinsH * OneHour;
                }
                if (emp.DAbsentD == 0)
                {
                    DayPC = emp.DAbsentP * AbsentCount;
                }
                else
                {
                    var OneDay = salary / (double)30;
                    DayPC = emp.DAbsentD * AbsentCount * OneDay;
                }
                foreach (var item in Attend)
                {
                    var time = item.Time - STA;
                    if (time > ST0)
                        MTime += ((TimeSpan)time);
                    else if (time < ST0)
                    {
                        PTime += (-(TimeSpan)time);
                    }
                }
                foreach (var item in leave)
                {
                    var time = item.Time - STL;
                    if (time > ST0)
                        PTime += ((TimeSpan)time);
                    else if (time < ST0)
                    {
                        MTime += (-(TimeSpan)time);
                    }
                }
                ViewBag.PTime = PTime;
                ViewBag.MTime = MTime;
                ViewBag.Absent = AbsentCount;
                double MHour = MTime.TotalHours;
                ViewBag.MTimeS = MHour * leavePC;
                MainSalary -= (MHour * leavePC);
                double PHour = PTime.TotalHours;
                ViewBag.PTimeS = PHour * AttendPC;
                MainSalary += (PHour * AttendPC);
                ViewBag.AbsentS = DayPC;
                MainSalary -= DayPC;
                ViewBag.Salary = MainSalary;
                return View();
            }
            catch
            {
                return View("Error");
            }
        }
        #endregion
    }
}
