﻿namespace OnlineCourseManagementSystem.Data.Models
{
    using OnlineCourseManagementSystem.Data.Common.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Album : BaseDeletableModel<int>
    {
        public Album()
        {
            this.Images = new HashSet<File>();
        }

        public string Name { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<File> Images { get; set; }
    }
}
