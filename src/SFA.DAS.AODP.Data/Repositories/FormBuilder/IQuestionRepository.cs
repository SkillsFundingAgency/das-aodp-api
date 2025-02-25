using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IQuestionRepository
    {
        Task Archive(Guid questionId);
        Task<Dictionary<Guid, Guid>> CopyQuestionsForNewFormVersion(Dictionary<Guid, Guid> oldNewPageIds);
        Task<Question> Create(Question question);
        int GetMaxOrderByPageId(Guid pageId);
        Task<Question> GetQuestionByIdAsync(Guid id);
        Task<Question> GetQuestionDetailForRoutingAsync(Guid questionId);
        Task<string?> GetQuestionTypeById(Guid questionid);

        /// <summary>
        /// Finds a question with a given Id, and finds the next section with a lower Order (so will appear higher in the list) and switches them. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> MoveQuestionOrderDown(Guid id);

        /// <summary>
        /// Finds a question with a given Id, and finds the next section with a higher Order (so will appear lower in the list) and switches them. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> MoveQuestionOrderUp(Guid id);
        Task<bool> IsQuestionEditable(Guid id);
        Task<Question> Update(Question question);
        Task<List<Section>> GetSectionsWithPagesAndQuestionsByFormVersionIdAsync(Guid formVersionId);
    }
}