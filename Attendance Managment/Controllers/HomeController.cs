using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using Attendance_Managment.Models;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Attendance_Managment.Controllers
{
    public class HomeController : Controller
    {
        string projectId;
        FirestoreDb firestoreDb;
        public HomeController()
        {
            string filepath = "C:\\Users\\DC\\Desktop\\Sem6\\Attendance Managment\\Attendance Managment\\Content\\attendancemanagment-1c187.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "attendancemanagment-1c187";
            firestoreDb = FirestoreDb.Create(projectId);
        }
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
        [HttpPost]
        public async Task<ActionResult> Search(string searchString)
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
                    Query user = firestoreDb.Collection("Institute").Document(institute).Collection("Faculty");
                    QuerySnapshot getfaculty = await user.GetSnapshotAsync();
                    foreach (DocumentSnapshot documentSnapshot in getfaculty.Documents)
                    {
                        if (documentSnapshot.Exists)
                        {
                            Dictionary<string, object> city = documentSnapshot.ToDictionary();
                            string json = JsonConvert.SerializeObject(city);
                            Student newuser = JsonConvert.DeserializeObject<Student>(json);
                            newuser.Id = documentSnapshot.Id;
                            newuser.Option = "Faculty";
                            list.Add(newuser);
                        }
                    }
                    user = firestoreDb.Collection("Institute").Document(institute).Collection("Student");
                    QuerySnapshot getstudent = await user.GetSnapshotAsync();
                    foreach (DocumentSnapshot documentSnapshot in getstudent.Documents)
                    {
                        if (documentSnapshot.Exists)
                        {
                            Dictionary<string, object> city = documentSnapshot.ToDictionary();
                            string json = JsonConvert.SerializeObject(city);
                            Student newuser = JsonConvert.DeserializeObject<Student>(json);
                            newuser.Id = documentSnapshot.Id;
                            newuser.Option = "Student";
                            list.Add(newuser);
                        }
                    }
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i].Id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 || list[i].Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)//list[i].Id.Contains(searchString) || list[i].Name.Contains(searchString))
                        {
                            continue;
                        }
                        else
                        {
                            list.RemoveAt(i);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Source);
                    throw new Exception(e.Source);
                }
                ViewBag.User = list;
                return View();
            }
        }
    }
}