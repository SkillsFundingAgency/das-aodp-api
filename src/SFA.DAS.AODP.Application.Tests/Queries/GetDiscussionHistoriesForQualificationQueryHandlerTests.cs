using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Application.Tests.Queries;

public class GetDiscussionHistoriesForQualificationQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IQualificationDetailsRepository> _detailsRepositoryMock;
    private readonly GetDiscussionHistoriesForQualificationQueryHandler _handler;

    public GetDiscussionHistoriesForQualificationQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _detailsRepositoryMock = _fixture.Freeze<Mock<IQualificationDetailsRepository>>();
        _handler = new GetDiscussionHistoriesForQualificationQueryHandler(_detailsRepositoryMock.Object);
    }

    [Fact]
    public async Task Then_The_Api_Is_Called_With_The_Request_And_QualificationDiscussionHistory_Is_Returned()
    {
        var query = _fixture.CreateMany<QualificationDiscussionHistory>().ToList();

        _detailsRepositoryMock.Setup(x => x.GetDiscussionHistoriesForQualificationRef(It.IsAny<string>()))
                       .ReturnsAsync(query);


        var result = await _handler.Handle(new GetDiscussionHistoriesForQualificationQuery() { QualificationReference = "foobar" }, CancellationToken.None);

        _detailsRepositoryMock.Verify(x => x.GetDiscussionHistoriesForQualificationRef(It.IsAny<string>()), Times.Once);
        Assert.True(result.Success);
        Assert.Equal(query[0].Id, result.Value.QualificationDiscussionHistories[0].Id);
    }

    [Fact]
    public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
    {
        var query = _fixture.CreateMany<QualificationDiscussionHistory>().ToList();

        _detailsRepositoryMock.Setup(x => x.GetDiscussionHistoriesForQualificationRef(It.IsAny<string>()))
            .Throws(new Exception("foobar"));


        var result = await _handler.Handle(new GetDiscussionHistoriesForQualificationQuery() { QualificationReference = "foobar" }, CancellationToken.None);

        _detailsRepositoryMock.Verify(x => x.GetDiscussionHistoriesForQualificationRef(It.IsAny<string>()), Times.Once);
        Assert.False(result.Success);
        Assert.Equal("foobar", result.ErrorMessage);
    }
}
