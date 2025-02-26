using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using Moq;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Page;

public class UpdatePageAnswersCommandHandlerTests
{
    private readonly Mock<IApplicationPageRepository> _applicationPageRepository = new Mock<IApplicationPageRepository>();
    private readonly Mock<IApplicationQuestionAnswerRepository> _questionAnswerRepository = new Mock<IApplicationQuestionAnswerRepository>();
    private readonly Mock<IPageRepository> _pageRepository = new Mock<IPageRepository>();
    private readonly Mock<IApplicationRepository> _applicationRepository = new Mock<IApplicationRepository>();
    public UpdatePageAnswersCommandHandler _updatePageAnswersCommandHandler;

    public UpdatePageAnswersCommandHandlerTests()
    {
        _updatePageAnswersCommandHandler = new(_applicationPageRepository.Object,
                _questionAnswerRepository.Object,
                _pageRepository.Object,
                _applicationRepository.Object
            );
    }

    [Fact]
    public async Task Test_Update_Page()
    {
        var request = new UpdatePageAnswersCommand()
        {
            ApplicationId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            Routing = null
        };
        var application = new Data.Entities.Application.Application()
        {
            Submitted = false
        };
        _applicationRepository.Setup(v => v.GetByIdAsync(It.Is<Guid>(v => v == request.ApplicationId)))
            .ReturnsAsync(application);
        _pageRepository.Setup(v => v.GetPageByIdAsync(It.Is<Guid>(v => v == request.PageId)))
            .ReturnsAsync(new Data.Entities.FormBuilder.Page());

        var result = await _updatePageAnswersCommandHandler.Handle(request, default);

        _applicationPageRepository.Verify(v => v.UpsertAsync(It.Is<Data.Entities.Application.ApplicationPage>(v =>
                v.PageId == request.PageId && v.ApplicationId == request.ApplicationId &&
                v.Status == ApplicationPageStatus.Completed.ToString()
            )));
        _questionAnswerRepository.Verify(v => v.UpsertAsync(It.IsAny<List<ApplicationQuestionAnswer>>()));
        _applicationPageRepository.Verify(v => v.UpsertAsync(It.IsAny<List<ApplicationPage>>()));

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Test_Update_Page_Ansers_Errors_When_Submitted()
    {
        var request = new UpdatePageAnswersCommand()
        {
            ApplicationId = Guid.NewGuid(),
            PageId = Guid.NewGuid()
        };
        var application = new Data.Entities.Application.Application()
        {
            Submitted = true
        };
        _applicationRepository.Setup(v => v.GetByIdAsync(It.Is<Guid>(v => v == request.ApplicationId)))
            .ReturnsAsync(application);

        var result = await _updatePageAnswersCommandHandler.Handle(request, default);

        Assert.False(result.Success);
    }
}
