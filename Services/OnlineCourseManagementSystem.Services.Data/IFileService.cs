﻿namespace OnlineCourseManagementSystem.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using OnlineCourseManagementSystem.Web.ViewModels.Files;

    public interface IFileService
    {
        Task UploadImage(UploadImageInputModel uploadImageInputModel);

        IEnumerable<ImageViewModel> GetAllImagesForUser(string userId);

        Task DeleteImageFromGallery(int fileId, string userId);
    }
}
