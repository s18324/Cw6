using System.Collections.Generic;
using APBDwebAPI.Models;

namespace APBDwebAPI.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public Enrollment EnrollStudent(Student student);
        public Enrollment PromoteStudent(Promotion promotion);
        public bool CheckIndex(string index);
    }
}