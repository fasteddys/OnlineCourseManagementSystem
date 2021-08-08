﻿namespace OnlineCourseManagementSystem.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using OnlineCourseManagementSystem.Common;
    using OnlineCourseManagementSystem.Data.Models;
    using OnlineCourseManagementSystem.Services.Data;
    using OnlineCourseManagementSystem.Web.ViewModels.Courses;
    using OnlineCourseManagementSystem.Web.ViewModels.Lecturers;
    using OnlineCourseManagementSystem.Web.ViewModels.Reviews;
    using OnlineCourseManagementSystem.Web.ViewModels.Skills;
    using OnlineCourseManagementSystem.Web.ViewModels.Tags;
    using OnlineCourseManagementSystem.Web.ViewModels.Users;

    public class CoursesController : Controller
    {
        private readonly ICoursesService coursesService;
        private readonly ITagsService tagsService;
        private readonly ISubjectsService subjectsService;
        private readonly ILecturersService lecturersService;
        private readonly ISkillsService skillsService;
        private readonly IReviewsService reviewsService;
        private readonly IUsersService usersService;
        private readonly UserManager<ApplicationUser> userManager;

        public CoursesController(
            ICoursesService coursesService,
            ITagsService tagsService,
            ISubjectsService subjectsService,
            ILecturersService lecturersService,
            ISkillsService skillsService,
            IReviewsService reviewsService,
            IUsersService usersService,
            UserManager<ApplicationUser> userManager)
        {
            this.coursesService = coursesService;
            this.tagsService = tagsService;
            this.subjectsService = subjectsService;
            this.lecturersService = lecturersService;
            this.skillsService = skillsService;
            this.reviewsService = reviewsService;
            this.usersService = usersService;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult ById(int id)
        {
            CourseByIdViewModel viewModel = this.coursesService.GetById<CourseByIdViewModel>(id);

            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            CourseDetailsViewModel viewModel = this.coursesService.GetById<CourseDetailsViewModel>(id);

            viewModel.Tags = this.tagsService.GetAllByCourseId<AllTagsByCourseIdViewModel>(id);
            viewModel.Skills = this.skillsService.GetAllByCourseId<AllSkillsByCourseIdViewModel>(id);
            viewModel.Reviews = this.reviewsService.GetAllByCourseId<AllReviewsByCourseIdViewModel>(id);
            viewModel.Lecturers = this.lecturersService.GetAllByCourseId<AllLecturersByCourseIdViewModel>(id);
            viewModel.RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>();

            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult All()
        {
            AllCoursesListViewModel viewModel = new AllCoursesListViewModel
            {
                Courses = this.coursesService.GetAll<AllCoursesViewModel>(),
            };

            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult AllUpcomingAndActive(string name = null, int id = 1)
        {
            if (id <= 0)
            {
                return this.NotFound();
            }

            const int ItemsPerPage = 5;
            UpcomingAndActiveCoursesViewModel viewModel = new UpcomingAndActiveCoursesViewModel
            {
                ListOfActiveCourses = new AllActiveCoursesListViewModel
                {
                    ItemsPerPage = ItemsPerPage,
                    PageNumber = id,
                    ActiveCoursesCount = this.coursesService.GetAllActiveCoursesCount(name),
                    ActiveCourses = this.coursesService.GetAllActive<AllActiveCoursesViewModel>(id, name, ItemsPerPage),
                    Name = name,
                },
                UpcomingCourses = this.coursesService.GetAllUpcoming<AllUpcomingCoursesViewModel>(),
                Tags = this.tagsService.GetAll<AllTagsViewModel>(),
            };

            return this.View(viewModel);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult AllUnapproved()
        {
            AllCoursesListViewModel viewModel = new AllCoursesListViewModel
            {
                Courses = this.coursesService.GetAllUnapproved<AllCoursesViewModel>(),
            };

            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult AllByUser(string id)
        {
            AllCoursesByUserListViewModel viewModel = new AllCoursesByUserListViewModel
            {
                Courses = this.coursesService.GetAllByUser<AllCoursesByUserViewModel>(1, id, 6),
            };

            return this.View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> AllByCurrentUser(int id = 1)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            const int ItemsPerPage = 6;
            AllCoursesByUserListViewModel viewModel = new AllCoursesByUserListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                ActiveCoursesCount = this.coursesService.GetAllCoursesByUserIdCount(user.Id),
                Courses = this.coursesService.GetAllByUser<AllCoursesByUserViewModel>(id, user.Id, ItemsPerPage),
                FeaturedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>(),
                CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id),
            };

            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult AllUpcoming()
        {
            AllCoursesListViewModel viewModel = new AllCoursesListViewModel
            {
                Courses = this.coursesService.GetAllUpcoming<AllCoursesViewModel>(),
            };

            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult AllPast()
        {
            AllCoursesListViewModel viewModel = new AllCoursesListViewModel
            {
                Courses = this.coursesService.GetAllPast<AllCoursesViewModel>(),
            };

            return this.View(viewModel);
        }

        //[Authorize]
        //public IActionResult AllActive()
        //{
        //    AllCoursesListViewModel viewModel = new AllCoursesListViewModel
        //    {
        //        Courses = this.coursesService.GetAllActive<AllCoursesViewModel>(),
        //    };

        //    return this.View(viewModel);
        //}

        [Authorize]
        [HttpPost]
        public IActionResult AllByTag(SearchByTagInputModel input)
        {
            AllCoursesListViewModel viewModel = new AllCoursesListViewModel
            {
                Courses = this.coursesService.GetAllByTag<AllCoursesViewModel>(input),
            };

            return this.View(viewModel);
        }

        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        public IActionResult Create()
        {
            CreateCourseInputModel input = new CreateCourseInputModel
            {
                SubjectsItems = this.subjectsService.GetAllAsSelectListItems(),
                TagItems = this.tagsService.GetAllAsSelectListItems(),
                LecturerItems = this.lecturersService.GetAllAsSelectListItems(),
            };

            return this.View(input);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        public async Task<IActionResult> Create(CreateCourseInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.SubjectsItems = this.subjectsService.GetAllAsSelectListItems();
                input.TagItems = this.tagsService.GetAllAsSelectListItems();
                input.LecturerItems = this.lecturersService.GetAllAsSelectListItems();
                return this.View(input);
            }

            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            input.UserId = user.Id;
            await this.coursesService.CreateAsync(input);
            this.TempData["Message"] = "Course is created successfully!";

            return this.Redirect("/");
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Edit(int id)
        {
            //EditCourseInputModel input = new EditCourseInputModel
            //{
            //    Id = id,
            //    Name = this.coursesService.GetNameById(id),
            //    Description = this.coursesService.GetDescriptionById(id),
            //    Price = this.coursesService.GetPriceById(id),
            //    StartDate = this.coursesService.GetStartDateById(id),
            //    EndDate = this.coursesService.GetEndDateById(id),
            //    SubjectId = this.coursesService.GetSubjectIdById(id),
            //    SubjectsItems = this.subjectsService.GetAllAsSelectListItems(),
            //};

            EditCourseInputModel input = this.coursesService.GetById<EditCourseInputModel>(id);
            input.SubjectsItems = this.subjectsService.GetAllAsSelectListItems();

            return this.View(input);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Edit(EditCourseInputModel input, int id)
        {
            if (!this.ModelState.IsValid)
            {
                input.SubjectsItems = this.subjectsService.GetAllAsSelectListItems();
                return this.View(input);
            }

            input.Id = id;
            await this.coursesService.UpdateAsync(input);
            this.TempData["Message"] = "Course updated successfully!";

            return this.Redirect($"/Courses/AllUnapproved");
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.coursesService.DeleteAsync(id);
            this.TempData["Message"] = "Course deleted successfully!";
            return this.Redirect("/");
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Approve(int id)
        {
            await this.coursesService.ApproveAsync(id);
            this.TempData["Message"] = "Course approved successfully!";

            return this.Redirect("/");
        }
    }
}
