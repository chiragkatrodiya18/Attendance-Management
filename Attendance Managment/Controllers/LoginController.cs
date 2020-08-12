using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Attendance_Managment.Models;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Attendance_Managment.Controllers
{
    public class LoginController : Controller
    {
        string projectId;
        FirestoreDb firestoreDb;
        public LoginController()
        {
            string filepath = "C:\\Attendance Managment\\Attendance Managment\\Content\\attendancemanagment-1c187.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "attendancemanagment-1c187";
            firestoreDb = FirestoreDb.Create(projectId);
        }

        private Login db = new Login();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            if (Session["id"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Index([Bind(Include ="Id,Password")] Login lo)
        {
            Login s = new Login();
            Query student = firestoreDb.Collection("Admin");
            QuerySnapshot getfaculty = await student.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in getfaculty.Documents)
            {
                if (documentSnapshot.Exists && documentSnapshot.Id == lo.Id.ToString())
                {
                    Dictionary<string, object> city = documentSnapshot.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    Login newuser = JsonConvert.DeserializeObject<Login>(json);
                    newuser.Id = documentSnapshot.Id;
                    s = newuser;
                }
            }
            if (s!=null){
                if (lo.Password == s.Password)
                {
                    HttpCookie admin = new HttpCookie("Login");
                    admin.Value = "hallo";
                    admin.Expires = DateTime.Now.AddHours(1);
                    Response.Cookies.Add(admin);
                    Session["user"] = lo.Id;
                    return RedirectToAction("../Home/Index");
                }
                else
                {
                    TempData["password"] = "Password is Wrong";
                    return View();
                }
            }
            else
            {
                TempData["user"] = "Admin Not Found";
                return View();
            }
        }

    }
}