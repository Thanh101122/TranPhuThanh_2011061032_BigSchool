using BigSchool.Models;
using BigSchool.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public CoursesController()
        {
            dbContext = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new CourseViewModel
            {
                Categories = dbContext.Categories.ToList(),
                Heading = "Add Course0"
            };
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = dbContext.Categories.ToList();
                return View("Create", viewModel);
            }
            var course = new Course
            {
                LecturerId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                CategoryId = viewModel.Category,
                Place = viewModel.Place
            };
            dbContext.Courses.Add(course);
            dbContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult Attending()
        {
            var userID = User.Identity.GetUserId();
            var course = dbContext.Attendances
                .Where(a => a.AttendeeId == userID)
                .Select(a => a.Course)
                .Include(a => a.Lecturer)
                .Include(a => a.Category)
                .ToList();
            var viewModel = new CourseViewModel
            {
                UpcomingCourse = course,
                ShowAction = User.Identity.IsAuthenticated
            };
            return View(viewModel);
        }
        public ActionResult Mine()
        {
            var userID = User.Identity.GetUserId();
            var courses = dbContext.Courses
                .Where(a => a.LecturerId == userID && a.DateTime > DateTime.Now)
                .Include(l => l.Lecturer)
                .Include(c => c.Category)
                .ToList();
            return View(courses);
        }
        public ActionResult Edit(int id)
        {
            var userID = User.Identity.GetUserId();
            var course = dbContext.Courses.Single(c => c.Id == id && c.LecturerId == userID);
            var viewModel = new CourseViewModel
            {
                Categories = dbContext.Categories.ToList(),
                Date = course.DateTime.ToString("MM/dd/yyyy"),
                Time = course.DateTime.ToString("HH:mm"),
                Category = course.CategoryId,
                Place = course.Place,
                Heading = "Edit Course",
                Id = course.Id
            };
            return View("Create", viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = dbContext.Categories.ToList();
                return View("Create", viewModel);
            }
            var userId = User.Identity.GetUserId();
            var course = dbContext.Courses.Single(c => c.Id == viewModel.Id && c.LecturerId == userId);
            course.Place = viewModel.Place;
            course.DateTime = viewModel.GetDateTime();
            course.CategoryId = viewModel.Category;
            dbContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}