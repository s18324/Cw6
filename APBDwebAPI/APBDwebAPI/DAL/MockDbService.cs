/*using System.Collections.Generic;
using APBDwebAPI.Models;

namespace APBDwebAPI.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student {IdStudent = 1, FirstName = "Jan", LastName = "Kowalski"},
                new Student {IdStudent = 2, FirstName = "Anna", LastName = "Malewski"},
                new Student {IdStudent = 3, FirstName = "Jacek", LastName = "Chlebek"}
            };
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}*/