using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace Attendance_Managment.Models
{
    [FirestoreData]
    public class Student
    {
        [Key]
        [Required]
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Class { get; set; }
        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public string ContactNumber { get; set; }
        [FirestoreProperty]
        public string Dob { get; set; }
        public string Option { get; set; }
        public string Icard { get; set; }
    }
}