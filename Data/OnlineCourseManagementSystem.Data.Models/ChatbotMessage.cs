﻿using OnlineCourseManagementSystem.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCourseManagementSystem.Data.Models
{
    public class ChatbotMessage : BaseDeletableModel<int>
    {
        public string Content { get; set; }

        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }

        public bool IsMessageFromChatbot { get; set; }
    }
}
