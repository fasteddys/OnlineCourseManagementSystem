﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace OnlineCourseManagementSystem.Web.Infrastructure.ValidationAttributes
{
    public class WordDocumentExtensionAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is IFormFile formFile)
            {
                string extension = Path.GetExtension(formFile.FileName);

                if (extension == ".docx")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
