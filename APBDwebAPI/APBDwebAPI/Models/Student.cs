using System;
using System.ComponentModel.DataAnnotations;

namespace APBDwebAPI.Models
{
    public class Student
    {
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]

        public string Studies { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string IndexNumber { get; set; }
    }
}