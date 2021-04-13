﻿namespace OnlineCourseManagementSystem.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using OnlineCourseManagementSystem.Data.Common.Repositories;
    using OnlineCourseManagementSystem.Data.Models;
    using OnlineCourseManagementSystem.Services.Mapping;
    using OnlineCourseManagementSystem.Web.ViewModels.Students;

    public class StudentsService : IStudentsService
    {
        private readonly IDeletableEntityRepository<Student> studentsRepository;

        public StudentsService(IDeletableEntityRepository<Student> studentsRepository)
        {
            this.studentsRepository = studentsRepository;
        }

        public async Task AddParent(AddParentInputModel input)
        {
            Student student = this.studentsRepository.All().FirstOrDefault(s => s.Id == input.StudentId);
            student.ParentId = input.ParentId;
            await this.studentsRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return this.studentsRepository
                .All()
                .OrderBy(s => s.User.FirstName + ' ' + s.User.LastName)
                .ThenBy(s => s.User.UserName)
                .Where(s => s.User.Roles.FirstOrDefault().RoleId.EndsWith("Student"))
                .To<T>()
                .ToList();
        }

        public IEnumerable<SelectListItem> GetAllAsSelectListItems()
        {
            return this.studentsRepository
                .All()
                .OrderBy(s => s.User.FirstName + ' ' + s.User.LastName)
                .ThenBy(s => s.User.UserName)
                .Where(l => l.User.Roles.FirstOrDefault().RoleId.EndsWith("Student"))
                .Select(s => new SelectListItem
                {
                    Text = s.User.FirstName + ' ' + s.User.LastName,
                    Value = s.Id,
                })
                .ToList();
        }

        public IEnumerable<SelectListItem> GetAllAsSelectListItems(int courseId)
        {
            return this.studentsRepository
                .All()
                .OrderBy(s => s.User.FirstName + ' ' + s.User.LastName)
                .ThenBy(s => s.User.UserName)
                .Where(s => s.User.Courses.Any(c => c.Id == courseId) && s.User.Roles.FirstOrDefault().RoleId.EndsWith("Student"))
                .Select(s => new SelectListItem
                {
                    Text = s.User.FirstName + ' ' + s.User.LastName,
                    Value = s.Id,
                }).ToList();
        }

        public IEnumerable<T> GetAllById<T>(string parentId)
        {
            return this.studentsRepository
                .All()
                .OrderBy(s => s.User.FirstName + ' ' + s.User.LastName)
                .ThenBy(s => s.User.UserName)
                .Where(s => s.ParentId == parentId)
                .To<T>()
                .ToList();
        }

    }
}
