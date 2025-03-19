﻿using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationQuestionAnswerRepository : IApplicationQuestionAnswerRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationQuestionAnswerRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Data.Entities.Application.ApplicationQuestionAnswer> Create(Data.Entities.Application.ApplicationQuestionAnswer application)
        {
            application.Id = Guid.NewGuid();
            await _context.ApplicationQuestionAnswers.AddAsync(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<List<ApplicationQuestionAnswer>> GetAnswersByApplicationAndPageId(Guid applicationId, Guid pageId)
        {
            return await _context.ApplicationQuestionAnswers.Where(a => a.ApplicationPage.PageId == pageId && a.ApplicationPage.ApplicationId == applicationId).ToListAsync();
        }

        public async Task<Data.Entities.Application.ApplicationQuestionAnswer> Update(Data.Entities.Application.ApplicationQuestionAnswer application)
        {
            _context.ApplicationQuestionAnswers.Update(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task UpsertAsync(List<ApplicationQuestionAnswer> questionAnswers)
        {
            foreach (var answer in questionAnswers)
            {
                if (answer.Id == default)
                {
                    answer.Id = Guid.NewGuid();
                    _context.ApplicationQuestionAnswers.Add(answer);

                }
                else
                {
                    _context.ApplicationQuestionAnswers.Update(answer);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<ApplicationQuestionAnswersDTO>> GetAnswersByApplicationId(Guid applicationId)
        {
            return await _context.ApplicationQuestionAnswers
                .Where(aqa => aqa.ApplicationPage.ApplicationId == applicationId)
                .AsSplitQuery()
                .Select(aqa => new ApplicationQuestionAnswersDTO
                {
                    QuestionId = aqa.Question.Id,
                    QuestionTitle = aqa.Question.Title,
                    QuestionType = aqa.Question.Type,
                    QuestionRequired = aqa.Question.Required,
                    AnswerText = aqa.TextValue,
                    AnswerChoice = aqa.OptionsValue,
                    AnswerDate = aqa.DateValue.ToString(),
                    AnswerNumber = aqa.NumberValue
                })
                .ToListAsync();
        }

        public class ApplicationQuestionAnswersDTO
        {
            public Guid QuestionId { get; set; }
            public string QuestionTitle { get; set; }
            public string QuestionType { get; set; }
            public bool QuestionRequired { get; set; }

            public string? AnswerText { get; set; }
            public string? AnswerDate { get; set; }
            public string? AnswerChoice { get; set; }
            public decimal? AnswerNumber { get; set; }
        }
    }
}
