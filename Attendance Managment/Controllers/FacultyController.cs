using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Attendance_Managment.Models;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;

namespace Attendance_Managment.Controllers
{
    public class FacultyController : Controller
    {
        string projectId;
        FirestoreDb firestoreDb;
        public FacultyController()
        {
            string filepath = "C:\\Users\\DC\\Desktop\\Sem6\\Attendance Managment\\Attendance Managment\\Content\\attendancemanagment-1c187.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "attendancemanagment-1c187";
            firestoreDb = FirestoreDb.Create(projectId);
        }
        // GET: Faculty
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
        public async Task<ActionResult> AddFaculty([Bind(Include ="Id,Name,Email,ContactNumber,Class,Dob")] Faculty fa)
        {
            string institute = Session["user"].ToString();
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                string date = "";
                fa.Dob = fa.Dob.ToString();
                date = date + fa.Dob[8] + fa.Dob[9] + "/" + fa.Dob[5] + fa.Dob[6] + "/" + fa.Dob[0] + fa.Dob[1] + fa.Dob[2] + fa.Dob[3];
                fa.Dob = date;
                fa.Icard = "No";
                CollectionReference cr = firestoreDb.Collection("Institute").Document(institute).Collection("Faculty");
                try
                {
                    cr.Document(fa.Id.ToString());
                    await cr.Document(fa.Id.ToString()).SetAsync(fa);

                    SmtpClient client = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("managementattandance@gmail.com", "qrcode123")
                    };
                    MailMessage mailMessage = new MailMessage("managementattandance@gmail.com", fa.Email);
                    mailMessage.Subject = "Attendance Managment";
                    mailMessage.Body = "This Mail From Attendance Managment.\nYour Account is Added\n Your id : " + fa.Id + "\nYour Password : " + fa.Dob;
                    client.Send(mailMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Source);
                    throw new Exception(e.Source);
                }
                return RedirectToAction("../Faculty/Index");
            }
        }
        [HttpPost]
        public async Task<ActionResult> RemoveFaculty(string Id)
        {
            string institute = Session["user"].ToString();
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                CollectionReference cr = firestoreDb.Collection("Institute").Document(institute).Collection("Faculty");
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
                return RedirectToAction("../Faculty/Index");
            }
        }
        public ActionResult Edit()
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
        public async Task<ActionResult> EditFaculty(string Id)
        {
            string institute = Session["user"].ToString();
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                Faculty data = new Faculty();
                try
                {
                    Query faculty = firestoreDb.Collection("Institute").Document(institute).Collection("Faculty");
                    QuerySnapshot getfaculty = await faculty.GetSnapshotAsync();
                    foreach (DocumentSnapshot documentSnapshot in getfaculty.Documents)
                    {
                        if (documentSnapshot.Exists && documentSnapshot.Id == Id.ToString())
                        {
                            Dictionary<string, object> city = documentSnapshot.ToDictionary();
                            string json = JsonConvert.SerializeObject(city);
                            Faculty newuser = JsonConvert.DeserializeObject<Faculty>(json);
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
                ViewBag.Faculty = data;
                return View();
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditFaculty([Bind(Include = "Id,Name,Email,ContactNumber,Class,Dob")] Faculty fa)
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
                    CollectionReference cr = firestoreDb.Collection("Institute").Document(institute).Collection("Faculty");
                    cr.Document(fa.Id.ToString());
                    await cr.Document(fa.Id.ToString()).SetAsync(fa, SetOptions.Overwrite);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Source);
                    throw new Exception(e.Source);
                }
                return RedirectToAction("../Faculty/Index");
            }
        }
            public async Task<ActionResult> ViewEmployee()
        {
            string institute = Session["user"].ToString();
            if (Session["user"] == null)
            {
                return RedirectToAction("../Login/Index");
            }
            else
            {
                List<Faculty> list = new List<Faculty>();
                try
                {
                    list.Clear();
                    Query faculty = firestoreDb.Collection("Institute").Document(institute).Collection("Faculty");
                    QuerySnapshot getfaculty = await faculty.GetSnapshotAsync();
                    foreach (DocumentSnapshot documentSnapshot in getfaculty.Documents)
                    {
                        if (documentSnapshot.Exists)
                        {
                            Dictionary<string, object> city = documentSnapshot.ToDictionary();
                            string json = JsonConvert.SerializeObject(city);
                            Faculty newuser = JsonConvert.DeserializeObject<Faculty>(json);
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
    }
}