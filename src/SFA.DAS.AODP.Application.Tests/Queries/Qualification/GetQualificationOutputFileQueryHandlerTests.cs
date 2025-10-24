using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Infrastructure;
using SFA.DAS.AODP.Models.Settings;
using System.IO.Compression;
using System.Text;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification
{
    public class GetQualificationOutputFileQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationOutputFileRepository> _repo;
        private readonly Mock<IBlobStorageService> _blob;
        private readonly OutputFileBlobStorageSettings _settings;
        private readonly GetQualificationOutputFileQueryHandler _handler;
        private static readonly string[] LineSeparators = { "\r\n", "\n" };

        public GetQualificationOutputFileQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

            _repo = _fixture.Freeze<Mock<IQualificationOutputFileRepository>>();
            _blob = _fixture.Freeze<Mock<IBlobStorageService>>();

            // Freeze settings so handler receives a concrete instance with a known container name
            _settings = _fixture.Freeze<OutputFileBlobStorageSettings>();
            _settings.ContainerName = "unit-test-container";

            _handler = _fixture.Create<GetQualificationOutputFileQueryHandler>();
        }

        [Fact]
        public async Task Then_Repository_Is_Called_Uploads_Both_CSVs_And_Returns_Zip_With_Two_Entries()
        {
            // Arrange
            var todayUtc = DateTime.UtcNow.Date;
            var active = new QualificationOutputFile
            {
                QualificationName = "Active Q",
                Age1619_FundingApprovalEndDate = todayUtc.AddDays(2)
            };
            var archived = new QualificationOutputFile
            {
                QualificationName = "Archived Q",
                Age1619_FundingApprovalEndDate = todayUtc.AddDays(-2)
            };

            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ReturnsAsync(new List<QualificationOutputFile> { active, archived });

            var datePrefix = DateTime.Now.ToString("yy-MM-dd");
            var expectedApproved = $"{datePrefix}-AOdPApprovedOutputFile.csv";
            var expectedArchived = $"{datePrefix}-AOdPArchivedOutputFile.csv";

            // Capture what was uploaded so we can do a light content assertion
            var uploaded = new List<(string FileName, MemoryStream Copy)>();

            _blob.Setup(x => x.UploadFileAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Stream>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                 .Callback<string, string, Stream, string, CancellationToken>((container, name, stream, contentType, _) =>
                 {
                     // copy the stream to inspect outside the handler's using scope
                     var ms = new MemoryStream();
                     stream.Position = 0;
                     stream.CopyTo(ms);
                     ms.Position = 0;
                     uploaded.Add((name, ms));

                     // quick guards on each call
                     Assert.Equal("unit-test-container", container);
                     Assert.Equal("text/csv", contentType);
                     Assert.True(ms.Length > 0);
                 })
                 .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileQuery(), CancellationToken.None);

            // Assert – repository
            _repo.Verify(x => x.GetQualificationOutputFile(), Times.Once);

            // Assert – high level response
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.StartsWith(datePrefix, result.Value!.FileName);
            Assert.EndsWith("_qualifications_export.zip", result.Value.FileName);
            Assert.NotNull(result.Value.ZipFileContent);
            Assert.NotEmpty(result.Value.ZipFileContent);

            // Assert – blob uploads (two calls, correct names)
            _blob.Verify(x => x.UploadFileAsync(_settings.ContainerName, expectedApproved,
                                                It.IsAny<Stream>(), "text/csv", It.IsAny<CancellationToken>()), Times.Once);
            _blob.Verify(x => x.UploadFileAsync(_settings.ContainerName, expectedArchived,
                                                It.IsAny<Stream>(), "text/csv", It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(2, uploaded.Count);
            Assert.Contains(uploaded, u => u.FileName == expectedApproved);
            Assert.Contains(uploaded, u => u.FileName == expectedArchived);

            // Assert – zip has exactly the two entries with expected names
            using var ms = new MemoryStream(result.Value.ZipFileContent);
            using var zip = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: false, Encoding.UTF8);
            var names = zip.Entries.Select(e => e.FullName).ToList();
            Assert.Contains(expectedApproved, names);
            Assert.Contains(expectedArchived, names);
            Assert.Equal(2, names.Count);

            // Light CSV sanity: first line is header for both
            foreach (var entry in zip.Entries)
            {
                using var s = entry.Open();
                using var r = new StreamReader(s, Encoding.UTF8, leaveOpen: false);
                var firstLine = await r.ReadLineAsync();
                Assert.StartsWith("DateOfOfqualDataSnapshot,QualificationName,AwardingOrganisation,QualificationNumber,Level", firstLine);
            }
        }

        [Fact]
        public async Task Then_No_Qualifications_Returns_Failure_And_No_Uploads()
        {
            // Arrange
            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ReturnsAsync(new List<QualificationOutputFile>());

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(x => x.GetQualificationOutputFile(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("No qualifications found for output file.", result.ErrorMessage);
            _blob.Verify(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Then_Exception_Is_Handled_And_Failure_Returned_No_Uploads()
        {
            // Arrange
            var ex = new Exception("boom");
            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(x => x.GetQualificationOutputFile(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("boom", result.ErrorMessage);
            _blob.Verify(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Then_Record_With_EndDate_Today_Goes_To_Neither_List_But_Zip_Still_Has_Both_Headers()
        {
            // Arrange: exactly today is neither > nor < (edge case)
            var todayUtc = DateTime.UtcNow.Date;
            var edge = new QualificationOutputFile
            {
                QualificationName = "Edge Q",
                Age1619_FundingApprovalEndDate = todayUtc
            };

            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ReturnsAsync(new List<QualificationOutputFile> { edge });

            // Accept uploads but we only care that they exist
            _blob.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileQuery(), CancellationToken.None);

            // Assert: success and both CSVs present with only header lines
            Assert.True(result.Success);
            using var ms = new MemoryStream(result.Value!.ZipFileContent);
            using var zip = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: false, Encoding.UTF8);

            foreach (var entry in zip.Entries)
            {
                using var s = entry.Open();
                using var r = new StreamReader(s, Encoding.UTF8, leaveOpen: false);
                var all = await r.ReadToEndAsync();
                var lines = all.Split(LineSeparators, StringSplitOptions.None);

                // one header + maybe trailing empty line from newline at end => <= 2 lines
                Assert.StartsWith("DateOfOfqualDataSnapshot,QualificationName", lines[0]);
                Assert.True(lines.Length <= 2);
            }
        }
    }
}
