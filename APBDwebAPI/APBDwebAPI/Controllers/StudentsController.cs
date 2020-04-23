using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using APBDwebAPI.DAL;
using APBDwebAPI.DTOs.Requests;
using APBDwebAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
//using System.Data.SqlClient;

namespace APBDwebAPI.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        public string dbName = "Data Source=db-mssql; Initial Catalog=s18977; Integrated Security=True";
        public IConfiguration Configuration { get; set; }
        public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /*[HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //add to database
            //generating index nr
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }*/

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {

            using (var con = new SqlConnection(dbName))
            {
                using (var com = new SqlCommand(dbName))
                {
                    var pass = CreateHash(request.Haslo, CreateSalt());

                    com.Connection = con;
                    com.CommandText = "SELECT * FROM student s WHERE s.IndexNumber = @index AND s.Password = @pass";
                    com.Parameters.AddWithValue("@index", request.Login);
                    com.Parameters.AddWithValue("@pass", pass);

                    con.Open();
                    SqlDataReader reader = com.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return Unauthorized("Podany login lub hasło nie wystepuja w bazie!");
                    }
                }
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, request.Login),
                new Claim(ClaimTypes.Role, "Employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds

            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
        }

        //WYKLAD 8 59.07 FUNKCJA HASHUJACA
        public static string CreateHash(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                );

            return Convert.ToBase64String(valueBytes);
        }

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static bool Validate(string value, string salt, string hash)
            => CreateHash(value, salt) == hash;

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string refToken)
        {

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "jo"),
                new Claim(ClaimTypes.Role, "Employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds

            );

            return Ok();
        }

        /*[HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokończona");
        }*/

        [HttpPut]
        public IActionResult PutStudent(string id)
        {
            return Ok($"Aktualizacja dokończona {id}");
        }

        [HttpDelete]
        public IActionResult DeleteStudent(string id)
        {
            return Ok($"Usuwanie ukończone {id}");
        }

        /*[HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }*/
        
        //=============================================
        private readonly IDbService _dbService;

        

        private void Open()
        {
            throw new NotImplementedException();
        }

        //=============================================

        [HttpGet("{id}")]
        public IActionResult GetStudent(string Id)
        {
            List<Object> list = new List<object>();
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18734;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "select a.FirstName, a.LastName, a.BirthDate, b.Semester, c.Name" +
                    "               from Student a inner join Enrollment b on a.IdEnrollment = b.IdEnrollment" +
                    "                              inner join Studies c on b.IdStudy = c.IdStudy where" +
                    "                              a.IndexNumber = @id";
                com.Parameters.AddWithValue("id", Id);

                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {

                    var st = new
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = DateTime.Parse(dr["BirthDate"].ToString()),
                        Semester = dr["Semester"].ToString(),
                        Name = dr["Name"].ToString()
                    };
                    list.Add(st);


                }

            }
            return Ok(list);

        }
        [HttpGet]
        public IActionResult GetStudent()
        {

            
            var id = "s18324";

            List<Object> list = new List<object>();
            //using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18734;Integrated Security=True"))
            using (var con = new SqlConnection(dbName))
            {
                using (var com = new SqlCommand())
                {

                    com.Connection = con;
                    /*com.CommandText = "select a.FirstName, a.LastName, a.BirthDate, b.Semester, c.Name" +
                        "               from Student a inner join Enrollment b on a.IdEnrollment = b.IdEnrollment" +
                        "                              inner join Studies c on b.IdStudy = c.IdStudy";
    */

                    com.CommandText = "SELECT * FROM Student WHERE IndexNumber = @id";
                    com.Parameters.AddWithValue("@id", id);

                    con.Open();
                    var dr = com.ExecuteReader();
                    while (dr.Read())
                    {

                        var st = new
                        {
                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                            BirthDate = DateTime.Parse(dr["BirthDate"].ToString()),
                            Semester = dr["Semester"].ToString(),
                            Name = dr["Name"].ToString()
                        };
                        list.Add(st);

                        //...
                    }
                }

            }
            return Ok(list);

        }
    }
}