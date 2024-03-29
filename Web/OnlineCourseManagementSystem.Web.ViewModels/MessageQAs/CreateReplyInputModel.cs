﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnlineCourseManagementSystem.Web.ViewModels.MessageQAs
{
    public class CreateReplyInputModel
    {
        [Required]
        [MaxLength(160)]
        public string Content { get; set; }

        public string CreatorId { get; set; }

        public int ChannelId { get; set; }

        public int ParentId { get; set; }
    }
}
