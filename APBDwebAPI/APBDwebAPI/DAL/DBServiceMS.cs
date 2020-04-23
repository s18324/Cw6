using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using APBDwebAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace APBDwebAPI.DAL
{
    public class DBServiceMS : IDbService
    {

        [Authorize(Roles = "Employee")]
        public Enrollment EnrollStudent(Student student)
        {
            using (var polaczenie = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18734;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                polaczenie.Open();
                SqlTransaction transaction = polaczenie.BeginTransaction();
                try
                {

                    com.Transaction = transaction;
                    com.Connection = polaczenie;
                    com.CommandText = $"select count(1) from Studies where name=@name";

                    com.Parameters.AddWithValue("name", student.Studies);



                    int id = 0;

                    var dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        int v = (int)dr.GetValue(0);
                        id = v;
                    }
                    if (id == -1) throw new Exception("Student nie istnieje");

                    dr.Close();


                    com.CommandText = $"select idEnrollment from Enrollment where idStudy = @id and Semester = 1";
                    com.Parameters.AddWithValue("id", id);

                    dr = com.ExecuteReader();


                    int idstudy = -1;


                    while (dr.Read())
                    {
                        idstudy = (int)dr.GetValue(0);

                    }
                    int maxid = 1;

                    if (idstudy == -1)
                    {
                        dr.Close();
                        com.CommandText = $"select max(idEnrollment)+1 from Enrollment";



                        dr = com.ExecuteReader();

                        while (dr.Read())
                        {
                            maxid = (int)dr.GetValue(0);

                        }

                        DateTime date = DateTime.Now;
                        dr.Close();
                        com.CommandText = $"insert into Enrollment values (@idEnrollment, 1, @idStudy, @StartDate)";
                        com.Parameters.AddWithValue("idEnrollment", maxid);
                        com.Parameters.AddWithValue("idStudy", idstudy);
                        com.Parameters.AddWithValue("StartDate", date);
                        com.ExecuteNonQuery();

                    }

                    //dodajemy nowego studenta
                    dr.Close();
                    com.CommandText = $"insert into Student values (@id2, @firstname2, @lastname2, @birthdate2, @enrollment2)";
                    com.Parameters.AddWithValue("id2", student.IndexNumber.ToString());
                    com.Parameters.AddWithValue("firstname2", student.FirstName);
                    com.Parameters.AddWithValue("lastname2", student.LastName);
                    com.Parameters.AddWithValue("birthdate2", student.BirthDate);
                    com.Parameters.AddWithValue("enrollment2", maxid);
                    com.ExecuteNonQuery();

                    transaction.Commit();


                    return new Enrollment
                    {
                        IdEnrollment = maxid,
                        Semester = 1,
                        IdStudy = idstudy,
                        StartDate = DateTime.Now


                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    transaction.Rollback();
                }
                throw new Exception("Cos sie stalo sie");
            }



        }
        public IEnumerable<Student> GetStudents()
        {
            return null;
        }

        [Authorize(Roles = "Employee")]
        public Enrollment PromoteStudent(Promotion promotion)
        {
            using (var polaczenie = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18734;Integrated Security=True"))
            using (var com = new SqlCommand())
            {

                polaczenie.Open();
                com.Connection = polaczenie;
                com.CommandText = $"select count(1) from Enrollment a inner join Studies b on a.idstudy = b.idstudy where b.name=@studies10 and a.semester = @semester10";
                com.Parameters.AddWithValue("studies10", promotion.Studies);
                com.Parameters.AddWithValue("semester10", promotion.Semester);

                var dr = com.ExecuteReader();
                int v = -1;
                while (dr.Read())
                {
                    v = (int)dr.GetValue(0);
                }
                if (v == -1) throw new Exception("Student nie istnieje");


                dr.Close();
                com.CommandText = "Exec procedura1 @studies2,@semester2";
                com.Parameters.AddWithValue("studies2", promotion.Studies);
                com.Parameters.AddWithValue("semester2", promotion.Semester);
                com.ExecuteNonQuery();

                dr.Close();
                com.CommandText = $"select max(a.IdEnrollment)+1, a.idstudy from Enrollment a inner join Studies b on a.idstudy = b.idstudy where b.name=@studies3 and a.semester = @semester3 group by a.idstudy";
                com.Parameters.AddWithValue("studies3", promotion.Studies);
                com.Parameters.AddWithValue("semester3", promotion.Semester);
                dr = com.ExecuteReader();
                int idstudy = -1;
                while (dr.Read())
                {
                    v = (int)dr.GetValue(0);
                    idstudy = (int)dr.GetValue(1);
                }
                if (v == -1) throw new Exception("Student nie istnieje");

                return new Enrollment
                {
                    IdEnrollment = v,
                    Semester = 1,
                    IdStudy = idstudy,
                    StartDate = DateTime.Now
                };

            }





        }
        public bool CheckIndex(string index)
        {
            using (var polaczenie = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18734;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                polaczenie.Open();
                com.Connection = polaczenie;
                com.CommandText = $"select count(1) from Student where IndexNumber = @index";
                com.Parameters.AddWithValue("index", index);

                int id = -1;
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    id = (int)dr.GetValue(0);
                }
                if (id == -1) return false;


            }
            return true;
        }
    }
}
