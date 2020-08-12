using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using Attendance_Managment.Models;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;

namespace Attendance_Managment.Controllers
{
    public class GenrateController : Controller
    {
        string projectId;
        FirestoreDb firestoreDb;
        public GenrateController()
        {
            string filepath = "C:\\Users\\DC\\Desktop\\Sem6\\Attendance Managment\\Attendance Managment\\Content\\attendancemanagment-1c187.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            projectId = "attendancemanagment-1c187";
            firestoreDb = FirestoreDb.Create(projectId);
        }
        // GET: Genrate        
        public async Task<ActionResult> Icard()
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
                        if (list[i].Icard == "No")
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
                ViewBag.Icard = list;
                return View();
            }
        }
        public async Task<ActionResult> GenrateIcard(string id,string type)
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
                    Query student = firestoreDb.Collection("Institute").Document(institute).Collection(type);
                    QuerySnapshot getstudent = await student.GetSnapshotAsync();
                    foreach (DocumentSnapshot documentSnapshot in getstudent.Documents)
                    {
                        if (documentSnapshot.Exists && documentSnapshot.Id == id.ToString())
                        {
                            Dictionary<string, object> city = documentSnapshot.ToDictionary();
                            string json = JsonConvert.SerializeObject(city);
                            Student newuser = JsonConvert.DeserializeObject<Student>(json);
                            newuser.Id = documentSnapshot.Id;
                            data = newuser;
                            data.Option = type;
                            break;
                        }
                    }
                    CollectionReference cr = firestoreDb.Collection("Institute").Document(institute).Collection("Student");
                    cr.Document(data.Id.ToString());
                    await cr.Document(data.Id.ToString()).SetAsync(data, SetOptions.Overwrite);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Source);
                    throw new Exception(e.Source);
                }
                string code = data.Id.ToString();
                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;
                var result = QCwriter.Write(code);
                var barcodeBitmap = new Bitmap(result);
                System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                imgBarCode.Height = 125;
                imgBarCode.Width = 125;

                using (MemoryStream memory = new MemoryStream())
                {
                    /*using (FileStream fs = new FileStream(path,
                       FileMode.Create, FileAccess.ReadWrite))
                    {*/
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(memory.ToArray());
                    //fs.Write(bytes, 0, bytes.Length);
                    //}
                }
                string HTMLContent = "<html><head><meta http-equiv='Content-Type' Content='text/html; charset=UTF-8'>" +
                    "<style type='text/css'></style></head>" +
                                        "<body>" +
                                            "<div style='background:white;color:black;height:565px;width:360px;border:5px solid rgb(115,151,201);border-top:9px solid rgb(115,151,201)'>" +
                                                "<div style = 'border-top:8px solid rgb(62,76,102);width:100.1%;'>" +
                                                    "<div>" +
                                                        "<h2 style = 'color:rgb(15,15,15);text-align:center;'>Dharmsinh Desai University</h2>" +
                                                        "<p style = 'color:rgb(15,15,15);text-align:center;margin-top:-5%;font-size:20px;'>Faculty Of Technology</p>" +
                                                     "</div>" +
                                                     "<div style = 'width:100.1%;'>" +
                                                        "<div style = 'background:rgb(64,79,102);color:White;height:6%;border-top-right-radius:30px;font-size:25px;text-align:center;width:75.5%;border: 1px solid rgb(115,151,201);'>" + data.Name + "</div>" +
                                                        "<div style = 'text-align:center;'><img src =" + "'C:\\Users\\DC\\Desktop\\img_girl.jpg'" + "width='187' height='225'></div>" +
                                                        "<div style = 'background:rgb(64,79,102);color:White;height:6%;border-bottom-left-radius:30px;float:right;font-size:25px;text-align:center;width:75.4%;border: 1px solid rgb(115,151,201);'>" + data.Option + "</div>" +
                                                     "</div>" +
                                                     "<div style = 'margin-top:15%;text-align:center;overflow-x:auto;'>" +
                                                        "<div style = 'display:flex;'>" +
                                                            "<table style = 'margin-left:7%;margin-top:2%;display:inline-block;font-size:18px;'>" +
                                                                "<tr><th style='text-align:left;'>"+type+ " Id</th><td>:</td><td style='text-align:left;'>" + data.Id + "</td></tr>" +
                                                                "<tr><th style='text-align:left;'>Class</th><td>:</td><td style='text-align:left;'>" + data.Class + "</td></tr>" +
                                                                "<tr><th style='text-align:left;'>Contact.</th><td>:</td><td style='text-align:left;'>" + data.ContactNumber + "</td></tr>" +
                                                                "<tr><th style='text-align:left;'>Dob</th><td>:</td><td style='text-align:left;'>" + data.Dob + "</td></tr>" +
                                                            "</table>" +
                                                            "<div style = 'float:right;margin-right:0.5%;display:inline-block;'><img src = " + imgBarCode.ImageUrl + " width='125' height='125'></div>" +
                                                        "</div>" +
                                                     "</div>" +
                                                "</div>" +
                                           "</div>" +
                                       "</body>" +
                                  "</html>";
                var Renderer = new IronPdf.HtmlToPdf();
                var PDF = Renderer.RenderHtmlAsPdf(HTMLContent);

                string path = "C:\\Users\\DC\\Desktop\\AM\\" + DateTime.Now.ToString("dd-MM-yyyy");
                bool folderExists = Directory.Exists(path);//Here i don't understand what is server i think this will work in ASP.NET
                if (!folderExists)
                    Directory.CreateDirectory(path);

                var OutputPath = path + "\\" + data.Id.ToString() + ".pdf";
                PDF.SaveAs(OutputPath);
                System.Diagnostics.Process.Start(OutputPath);
                return RedirectToAction("../Genrate/Icard");
                //return View();
            }
        }
        public void DownloadPDF(string id)
        {
            /*string code = "17ceuos106";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
            imgBarCode.Height = 110;
            imgBarCode.Width = 110;
            Bitmap bitMap = qrCode.GetGraphic(20);
            System.Drawing.Image img = (System.Drawing.Image)bitMap;
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            Byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(HTMLContent, PdfSharp.PageSize.A4);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(HTMLContent.ToString());
            StringReader sr = new StringReader(sb.ToString());
            byte[] bytes = null;
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();
                bytes = memoryStream.ToArray();
                memoryStream.Close();
            }
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + "PDFfile.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(GetPDF(HTMLContent));
            Response.End();*/
        }
    }
}