﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourseManagementSystem.Services.Data
{
    public interface IOrdersService
    {
        Task CreateAsync(int courseId, string userId);

        Task DeleteAsync(int courseId, string userId);

        IEnumerable<T> GetAllByUserId<T>(string userId);

        int CoursesInCartCount(string userId);

        bool IsOrderAvailable(int courseId, string userId);
    }
}
