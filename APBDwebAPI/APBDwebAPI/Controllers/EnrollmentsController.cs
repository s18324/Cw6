using APBDwebAPI.DAL;
using APBDwebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBDwebAPI.Controllers
{
    [ApiController]
    public class EnrollmentsController
    {
        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [Route("api/enrollments")]
        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            try
            {
                return Created("", _dbService.EnrollStudent(student));
            }
            catch (Exception e)
            {
                Console.WriteLine("Cos poszlo nie tak!!!");
                Console.WriteLine(e.StackTrace);
                return StatusCode(400);
            }
        }

        private IActionResult StatusCode(int v)
        {
            throw new NotImplementedException();
        }

        [Route("api/enrollments/promotions")]
        [HttpPost]
        public IActionResult PromoteStudent(Promotion promotion)
        {
            return Created("", _dbService.PromoteStudent(promotion));
        }

        private IActionResult Created(string v, Enrollment enrollment)
        {
            throw new NotImplementedException();
        }
    }
}
