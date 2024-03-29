﻿namespace OnlineCourseManagementSystem.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ITagsService
    {
        IEnumerable<SelectListItem> GetAllAsSelectListItems();

        IEnumerable<T> GetAll<T>();

        IEnumerable<T> GetAllByCourseId<T>(int courseId);
    }
}
