using Assignment08June.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Assignment08June.Controllers
{
    public class EmployeeController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MVC_PrishusoftContext"].ConnectionString;

        
        public ActionResult List()
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employee e JOIN Designation d ON e.DesignationIdRef = d.Id JOIN DesignationGrade dg ON e.GradeIdref = dg.id";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Employee employee = new Employee
                    {
                        Id = (int)reader["Id"],
                        Firstname = reader["Firstname"].ToString(),
                        Lastname = reader["Lastname"].ToString(),
                        EmailAddress = reader["EmailAddress"].ToString(),
                        Phonenumber = reader["Phonenumber"].ToString(),
                        DesignationIdRef = (int)reader["DesignationIdRef"],
                        GradeIdref = (int)reader["GradeIdref"],
                        Designation = new Designation
                        {
                            Id = (int)reader["DesignationIdRef"],
                            DesignationName = reader["DesignationName"].ToString(),
                            IsActive = (bool)reader["IsActive"]
                        },
                        DesignationGrade = new DesignationGrade
                        {
                            Id = (int)reader["GradeIdref"],
                            Gradename = reader["Gradename"].ToString(),
                            IsActive = (bool)reader["IsActive"]
                        }
                    };
                    employees.Add(employee);
                }
            }
            return View(employees);
        }


        public ActionResult Add()
        {
            ViewBag.Designations = GetDesignations();
            ViewBag.Grades = new SelectList(new List<SelectListItem>());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Employee model)
        {
            
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    //string query = "INSERT INTO Employee (Firstname, Lastname, EmailAddress, Phonenumber, DesignationIdRef, GradeIdref) VALUES (@Firstname, @Lastname, @EmailAddress, @Phonenumber, @DesignationIdRef, @GradeIdref)";
                    SqlCommand cmd = new SqlCommand("InsertEmployee", con);
                cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Firstname", model.Firstname);
                    cmd.Parameters.AddWithValue("@Lastname", model.Lastname);
                    cmd.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                    cmd.Parameters.AddWithValue("@Phonenumber", model.Phonenumber);
                    cmd.Parameters.AddWithValue("@DesignationIdRef", model.DesignationIdRef);
                    cmd.Parameters.AddWithValue("@GradeIdref", model.GradeIdref);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("List");
                
                ViewBag.Designations = GetDesignations();
                ViewBag.Grades = GetGrades(model.DesignationIdRef);
                return View(model);
          
           
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = GetEmployeeById(id.Value);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.Designations = GetDesignations();
            ViewBag.Grades = GetGrades(employee.DesignationIdRef);
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("UpdateEmployee", con);
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Firstname", model.Firstname);
                    cmd.Parameters.AddWithValue("@Lastname", model.Lastname);
                    cmd.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                    cmd.Parameters.AddWithValue("@Phonenumber", model.Phonenumber);
                    cmd.Parameters.AddWithValue("@DesignationIdRef", model.DesignationIdRef);
                    cmd.Parameters.AddWithValue("@GradeIdref", model.GradeIdref);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("List");
            }
            ViewBag.Designations = GetDesignations();
            ViewBag.Grades = GetGrades(model.DesignationIdRef);
            return View(model);
        }

        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = GetEmployeeById(id.Value);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Employee WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        public JsonResult GetGradesJson(int designationId)
        {
            var grades = GetGrades(designationId);
            return Json(grades, JsonRequestBehavior.AllowGet);
        }

        private SelectList GetDesignations()
        {
            List<SelectListItem> designations = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, DesignationName FROM Designation WHERE IsActive = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    designations.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["DesignationName"].ToString()
                    });
                }
            }

            return new SelectList(designations, "Value", "Text");
        }

        private SelectList GetGrades(int designationId)
        {
            List<SelectListItem> grades = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Gradename FROM DesignationGrade WHERE DesignationIdref = @DesignationId AND IsActive = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DesignationId", designationId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    grades.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Gradename"].ToString()
                    });
                }
            }

            return new SelectList(grades, "Value", "Text");
        }


        private Employee GetEmployeeById(int id)
        {
            Employee employee = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employee WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    employee = new Employee
                    {
                        Id = (int)reader["Id"],
                        Firstname = reader["Firstname"].ToString(),
                        Lastname = reader["Lastname"].ToString(),
                        EmailAddress = reader["EmailAddress"].ToString(),
                        Phonenumber = reader["Phonenumber"].ToString(),
                        DesignationIdRef = (int)reader["DesignationIdRef"],
                        GradeIdref = (int)reader["GradeIdref"]
                    };
                }
            }

            return employee;
        }
    }
}
