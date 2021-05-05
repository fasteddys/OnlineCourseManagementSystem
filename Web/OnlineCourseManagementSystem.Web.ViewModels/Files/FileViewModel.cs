﻿using AutoMapper;
using OnlineCourseManagementSystem.Data.Models;
using OnlineCourseManagementSystem.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCourseManagementSystem.Web.ViewModels.Files
{
    public class FileViewModel : IMapFrom<File>, IHaveCustomMappings
    {
        public string RemoteUrl { get; set; }

        public int FileId { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<File, FileAssignmentViewModel>()
                .ForMember(x => x.FileId, y => y.MapFrom(f => f.Id));
        }
    }
}
