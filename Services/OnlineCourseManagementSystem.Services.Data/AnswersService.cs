﻿using OnlineCourseManagementSystem.Data.Common.Repositories;
using OnlineCourseManagementSystem.Data.Models;
using OnlineCourseManagementSystem.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineCourseManagementSystem.Services.Data
{
    public class AnswersService : IAnswersService
    {
        private readonly IDeletableEntityRepository<Answer> answersRepository;

        public AnswersService(IDeletableEntityRepository<Answer> answersRepository)
        {
            this.answersRepository = answersRepository;
        }

        public IEnumerable<T> GetAllByExamIdAndUserId<T>(int examId, string userId)
        {
            return this.answersRepository
                .All()
                .Where(a => a.Question.ExamId == examId && a.UserId == userId)
                .To<T>()
                .ToList();
        }
    }
}
