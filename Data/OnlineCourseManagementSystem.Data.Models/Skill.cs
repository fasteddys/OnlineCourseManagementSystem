﻿using OnlineCourseManagementSystem.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCourseManagementSystem.Data.Models
{
    public class Skill : BaseDeletableModel<int>
    {
        public string Text { get; set; }

        public int CourseId { get; set; }

        public virtual Course Course { get; set; }
    }
}
