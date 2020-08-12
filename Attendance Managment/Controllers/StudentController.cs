using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Attendance_Managment.Models;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Attendance_Managment.Controllers
{
    public class StudentController : Controller
    {
        string projectId;
        FirestoreDb firestoreDb;
        public StudentController()
        {
            string filepath = "C:\\Users\\DC\\Desktop\\Sem6\\Attendance Managment\\Attendance Managment\\Content\\attendancemanagment-1c187.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "attendancemanagment-1c187";
            firestoreDb = FirestoreDb.Create(projectId);
        }
        // GET: Student
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                return View();
            }
        }
        public async Task<ActionResult> ViewStudent()
        {
            string institute = Session["user"].ToString();
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                List<Student> list = new List<Student>();
                try
                {
                    list.Clear();
                    Query faculty = firestoreDb.Collection("Institute").Document(institute).Collection("Student");
                    QuerySnapshot getfaculty = await faculty.GetSnapshotAsync();
                    foreach (DocumentSnapshot documentSnapshot in getfaculty.Documents)
                    {
                        if (documentSnapshot.Exists)
                        {
                            Dictionary<string, object> city = documentSnapshot.ToDictionary();
                            string json = JsonConvert.SerializeObject(city);
                            Student newuser = JsonConvert.DeserializeObject<Student>(json);
                            newuser.Id = documentSnapshot.Id;
                            list.Add(newuser);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Source);
                    throw new Exception(e.Source);
                }
                ViewBag.Faculty = list;
                return View();
            }
        }
        [HttpPost]
        public async Task<ActionResult> RemoveStudent(string Id)
        {
            string institute = Session["user"].ToString();
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                CollectionReference cr = firestoreDb.Collection("Institute").Document(institute).Collection("Student");
                try
                {
                    cr.Document(Id.ToString());
                    await cr.Document(Id.ToString()).DeleteAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Source);
                    throw new Exception(e.Source);
                }
                return RedirectToAction("../Student/ViewStudent");
            }
        }
        public async Task<ActionResult> EditStudent(string Id)
        {
            string institute = Session["user"].ToString();
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                Student data = new Student();
                try
                {
                    Query student = firestoreDb.Collection("Institute").Document(institute).Collection("Student");
                    QuerySnapshot getstudent = await student.GetSnapshotAsync();
                    foreach (DocumentSnapshot documentSnapshot in getstudent.Documents)
                    {
                        if (documentSnapshot.Exists && documentSnapshot.Id == Id.ToString())
                        {
                            Dictionary<string, object> city = documentSnapshot.ToDictionary();
                            string json = JsonConvert.SerializeObject(city);
                            Student newuser = JsonConvert.DeserializeObject<Student>(json);
                            newuser.Id = documentSnapshot.Id;
                            data = newuser;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Source);
                    throw new Exception(e.Source);
                }
                ViewBag.Student = data;
                return View();
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditStudent([Bind(Include = "Id,Name,Email,ContactNumber,Class,Dob")] Student st)
        {
            string institute = Session["user"].ToString();
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                try
                {
                    CollectionReference cr = firestoreDb.Collection("Institute").Document(institute).Collection("Student");
                    cr.Document(st.Id.ToString());
                    await cr.Document(st.Id.ToString()).SetAsync(st, SetOptions.Overwrite);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Source);
                    throw new Exception(e.Source);
                }
                return RedirectToAction("../Student/ViewStudent");
            }
        }
    }
}