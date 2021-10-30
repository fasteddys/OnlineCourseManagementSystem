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
    using OnlineCourseManagementSystem.Web.ViewModels.Lectures;
    using OnlineCourseManagementSystem.Web.ViewModels.Reviews;
    using OnlineCourseManagementSystem.Web.ViewModels.Skills;
    using OnlineCourseManagementSystem.Web.ViewModels.Tags;
    using OnlineCourseManagementSystem.Web.ViewModels.Users;
    using SmartBreadcrumbs.Attributes;

    public class CoursesController : Controller
    {
        private readonly ICoursesService coursesService;
        private readonly ITagsService tagsService;
        private readonly ISubjectsService subjectsService;
        private readonly ILecturersService lecturersService;
        private readonly ISkillsService skillsService;
        private readonly IReviewsService reviewsService;
        private readonly IUsersService usersService;
        private readonly IFilesService filesService;
        private readonly ILecturesService lecturesService;
        private readonly ICompletitionsService completitionsService;
        private readonly UserManager<ApplicationUser> userManager;

        public CoursesController(
            ICoursesService coursesService,
            ITagsService tagsService,
            ISubjectsService subjectsService,
            ILecturersService lecturersService,
            ISkillsService skillsService,
            IReviewsService reviewsService,
            IUsersService usersService,
            IFilesService filesService,
            ILecturesService lecturesService,
            ICompletitionsService completitionsService,
            UserManager<ApplicationUser> userManager)
        {
            this.coursesService = coursesService;
            this.tagsService = tagsService;
            this.subjectsService = subjectsService;
            this.lecturersService = lecturersService;
            this.skillsService = skillsService;
            this.reviewsService = reviewsService;
            this.usersService = usersService;
            this.filesService = filesService;
            this.lecturesService = lecturesService;
            this.completitionsService = completitionsService;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Student,Lecturer,Administrator")]
        [Breadcrumb(" Course Details ", FromAction = "AllByCurrentUser")]
        public IActionResult ById(int id)
        {
            CourseByIdViewModel viewModel = this.coursesService.GetById<CourseByIdViewModel>(id);

            viewModel.Skills = this.skillsService.GetAllByCourseId<AllSkillsByCourseIdViewModel>(id);
            viewModel.Lecturers = this.lecturersService.GetAllByCourseId<AllLecturersByIdViewModel>(id);
            viewModel.Lectures = this.lecturesService.GetAllById<AllLecturesByIdViewModel>(id);

            return this.View(viewModel);
        }

        [Authorize]
        [Breadcrumb(" Course Details ", FromAction = "AllUpcomingAndActive")]
        public async Task<IActionResult> Details(int id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            CourseDetailsViewModel viewModel = this.coursesService.GetById<CourseDetailsViewModel>(id);

            viewModel.Tags = this.tagsService.GetAllByCourseId<AllTagsByCourseIdViewModel>(id);
            viewModel.Skills = this.skillsService.GetAllByCourseId<AllSkillsByCourseIdViewModel>(id);
            viewModel.Reviews = this.reviewsService.GetAllByCourseId<AllReviewsByCourseIdViewModel>(id);
            viewModel.Lecturers = this.lecturersService.GetAllByCourseId<AllLecturersByCourseIdViewModel>(id);
            viewModel.RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>();
            viewModel.CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id);

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
        [Breadcrumb(" Upcoming and Active Courses ", FromAction = "Index", FromController = typeof(HomeController))]
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

            foreach (var course in viewModel.Courses)
            {
                course.CompletedLecturesCount = this.completitionsService.GetAllCompletitionsCountByCourseIdAndUserId(course.CourseId, id);
            }

            return this.View(viewModel);
        }

        [Authorize(Roles = GlobalConstants.StudentRoleName)]
        [Breadcrumb("My Courses", FromAction = "Index", FromController = typeof(HomeController))]
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

            foreach (var course in viewModel.Courses)
            {
                course.CompletedLecturesCount = this.completitionsService.GetAllCompletitionsCountByCourseIdAndUserId(course.CourseId, user.Id);
            }

            this.ViewData["CurrentUserHeading"] = "Messages";

            return this.View(viewModel);
        }

        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        public async Task<IActionResult> AllByCurrentLecturer(int id = 1)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            const int ItemsPerPage = 6;
            AllCoursesByCurrentLecturerListViewModel viewModel = new AllCoursesByCurrentLecturerListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                LecturerCoursesCount = this.coursesService.GetAllCoursesByCreatorIdCount(user.Id),
                Courses = this.coursesService.GetAllByCreatorId<AllCoursesByCurrentLecturerViewModel>(id, user.Id, ItemsPerPage),
                FeaturedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>(),
                CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id),
            };

            this.ViewData["CurrentUserHeading"] = "Messages";

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
        [Breadcrumb("Add Meta", FromAction = "Create")]
        public async Task<IActionResult> Meta()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            CreateMetaInputModel input = new CreateMetaInputModel
            {
                SubjectsItems = this.subjectsService.GetAllAsSelectListItems(),
                TagsItems = this.tagsService.GetAllAsSelectListItems(),
                LecturersItems = this.lecturersService.GetAllAsSelectListItems(),
                RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>(),
                CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id),
                CoursesItems = this.coursesService.GetAllAsSelectListItemsByCreatorId(user.Id),
            };

            this.ViewData["CurrentUserHeading"] = "Messages";

            return this.View(input);
        }

        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        [HttpPost]
        public async Task<IActionResult> Meta(CreateMetaInputModel input)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            if (!this.ModelState.IsValid)
            {
                input.SubjectsItems = this.subjectsService.GetAllAsSelectListItems();
                input.TagsItems = this.tagsService.GetAllAsSelectListItems();
                input.LecturersItems = this.lecturersService.GetAllAsSelectListItems();
                input.RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>();
                input.CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id);
                input.CoursesItems = this.coursesService.GetAllAsSelectListItemsByCreatorId(user.Id);
                return this.View(input);
            }

            input.UserId = user.Id;
            await this.coursesService.CreateMetaAsync(input);
            this.TempData["Message"] = "Meta information about the course successfully created!";
            this.ViewData["CurrentUserHeading"] = "Messages";

            return this.RedirectToAction("AllLecturesByCreatorId", "Lectures");
        }

        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        [Breadcrumb("Create a Course", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Create()
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            CreateCourseInputModel input = new CreateCourseInputModel
            {
                RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>(),
                CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id),
            };

            this.ViewData["CurrentUserHeading"] = "Messages";

            return this.View(input);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        public async Task<IActionResult> Create(CreateCourseInputModel input)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            if (!this.ModelState.IsValid)
            {
                input.RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>();
                input.CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id);
                return this.View(input);
            }

            input.UserId = user.Id;
            input.CreatorId = user.Id;
            await this.coursesService.CreateAsync(input);
            this.TempData["Message"] = "Course is created successfully!";
            this.ViewData["CurrentUserHeading"] = "Messages";

            return this.RedirectToAction(nameof(this.Meta));
        }

        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        public async Task<IActionResult> Edit(int id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            EditCourseInputModel input = this.coursesService.GetById<EditCourseInputModel>(id);
            input.CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id);
            input.RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>();

            return this.View(input);
        }

        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        [HttpPost]
        public async Task<IActionResult> Edit(EditCourseInputModel input, int id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            if (!this.ModelState.IsValid)
            {
                input.CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id);
                input.RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>();
                return this.View(input);
            }

            input.Id = id;
            await this.coursesService.UpdateAsync(input);
            this.TempData["Message"] = "Course updated successfully!";

            return this.RedirectToAction("EditMeta", "Courses", new { id });
        }

        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        public async Task<IActionResult> EditMeta(int id)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            EditMetaInputModel input = this.coursesService.GetById<EditMetaInputModel>(id);
            input.CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id);
            input.RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>();
            input.SubjectsItems = this.subjectsService.GetAllAsSelectListItems();

            return this.View(input);
        }

        [Authorize(Roles = GlobalConstants.LecturerRoleName)]
        [HttpPost]
        public async Task<IActionResult> EditMeta(int id, int fileId, EditMetaInputModel input)
        {
            ApplicationUser user = await this.userManager.GetUserAsync(this.User);
            if (!this.ModelState.IsValid)
            {
                input.CurrentUser = this.usersService.GetById<CurrentUserViewModel>(user.Id);
                input.RecommendedCourses = this.coursesService.GetAllRecommended<AllRecommendedCoursesByIdViewModel>();
                input.SubjectsItems = this.subjectsService.GetAllAsSelectListItems();
                input.FileRemoteUrl = this.filesService.GetRemoteUrlById(fileId);
                return this.View(input);
            }

            input.Id = id;
            input.FileId = fileId;
            await this.coursesService.UpdateMetaAsync(input);
            this.TempData["Message"] = "Course meta data updated successfully!";

            return this.RedirectToAction("AllLecturesByCreatorId", "Lectures");
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
