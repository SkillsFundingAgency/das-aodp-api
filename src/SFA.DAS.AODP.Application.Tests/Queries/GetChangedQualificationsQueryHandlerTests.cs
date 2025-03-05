using Moq;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Application.Queries.Qualification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Application.Tests.Queries;

public class GetChangedQualificationsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IChangedQualificationsRepository> _qualificationsRepository = new();
    public GetChangedQualificationsQueryHandler _getChangedQualificationsQueryHandler { get; set; }

    public GetChangedQualificationsQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _getChangedQualificationsQueryHandler = new(_qualificationsRepository.Object);
    }

    [Fact]
    public async Task Test_Get_Changed_Qualification()
    {
        //var qualifications = _fixture.Create<Data.Entities.Qualification.ChangedQualification>();
        //_qualificationsRepository.Setup(v => v.GetChangedQualificationsAsync(0,0,new NewQualificationsFilter() { Name="" }))
        //    .ReturnsAsync(qualifications).Single();

        //var response = await _getChangedQualificationsQueryHandler.Handle(new GetChangedQualificationsQuery(), default);

        //Assert.True(response.Success);
        //Assert.NotNull(response.Value);
        //Assert.NotNull(response.Value.Data);
        //Assert.NotEmpty(response.Value.Data);
    }

    [Fact]
    public async Task Test_Get_Changed_Qualification_Throws_Returns_Error()
    {
        var qualifications = _fixture.CreateMany<Data.Entities.Qualification.ChangedQualification>().ToList();
        _qualificationsRepository.Setup(v => v.GetChangedQualificationsAsync(0,0,new NewQualificationsFilter() { Name="" }))
            .Throws(new Exception());

        var response = await _getChangedQualificationsQueryHandler.Handle(new GetChangedQualificationsQuery(), default);

        Assert.False(response.Success);
    }
}
