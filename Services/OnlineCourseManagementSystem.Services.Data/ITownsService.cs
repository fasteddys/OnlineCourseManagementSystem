﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCourseManagementSystem.Services.Data
{
    public interface ITownsService
    {
        IEnumerable<SelectListItem> GetAllAsSelectListItems();
    }
}
