using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Infrastructure;
using SFA.DAS.AODP.Models.Settings;
using System.Text;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification
{
    public class GetQualificationOutputFileQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationOutputFileRepository> _repo;
        private readonly Mock<IQualificationOutputFileLogRepository> _logRepo;
        private readonly Mock<IBlobStorageService> _blob;
        private readonly OutputFileBlobStorageSettings _settings;
        private readonly GetQualificationOutputFileQueryHandler _handler;

        private const string ContainerName = "unit-test-container";
        private const string CsvContentType = "text/csv";
        private const string ErrorNoQualifications = "No qualifications found for the output file.";
        private const string ErrorGeneric = "Exception message";
        private const string ErrorUnexpected = "An unexpected error occurred while generating the output file.";
        private const string FileSuffix = "-AOdPOutputFile.csv";
        private const string CsvHeaderPrefixLong = "DateOfOfqualDataSnapshot,QualificationName,AwardingOrganisation,QualificationNumber,Level";
        private const string CsvHeaderPrefixShort = "DateOfOfqualDataSnapshot,QualificationName";

        #region Test data
        private GetQualificationOutputFileQuery _testRequest = new()
        {
            CurrentUsername = "Alaam Adams",
            PublicationDate = DateTime.UtcNow
        };
        #endregion

        public GetQualificationOutputFileQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

            _repo = _fixture.Freeze<Mock<IQualificationOutputFileRepository>>();
            _blob = _fixture.Freeze<Mock<IBlobStorageService>>();
            _logRepo = _fixture.Freeze<Mock<IQualificationOutputFileLogRepository>>();

            _settings = _fixture.Freeze<OutputFileBlobStorageSettings>();
            _settings.ContainerName = ContainerName;

            _handler = _fixture.Create<GetQualificationOutputFileQueryHandler>();
        }

        [Fact]
        public async Task Then_Repository_Is_Called_Uploads_CSV_And_Returns_CSV_With_Two_Entries()
        {
            // Arrange
            var publicationDate = DateTime.UtcNow.Date;
            var active = new QualificationOutputFile
            {
                QualificationName = "Active Q",
                Age1619_FundingApprovalEndDate = publicationDate.AddDays(2)
            };
            var archived = new QualificationOutputFile
            {
                QualificationName = "Archived Q",
                Age1619_FundingApprovalEndDate = publicationDate.AddDays(-2)
            };

            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ReturnsAsync(new List<QualificationOutputFile> { active, archived });

            var datePrefix = publicationDate.ToString("yyyy-MM-dd");
            var expectedFile = $"{datePrefix}{FileSuffix}";

            // capture uploaded streams for later assertions
            var uploaded = new List<(string Container, string FileName, string ContentType, MemoryStream Copy)>();

            _blob.Setup(x => x.UploadFileAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<Stream>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                 .Callback<string, string, Stream, string, CancellationToken>((container, name, stream, contentType, _) =>
                 {
                     var ms = new MemoryStream();
                     stream.Position = 0;
                     stream.CopyTo(ms);
                     ms.Position = 0;
                     uploaded.Add((container, name, contentType, ms));
                 })
                 .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(_testRequest, CancellationToken.None);

            // Assert – repository called
            _repo.Verify(x => x.GetQualificationOutputFile(), Times.Once);

            // Assert – high level response
            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.StartsWith(datePrefix, result.Value!.FileName);
                Assert.EndsWith(FileSuffix, result.Value.FileName);
                Assert.NotNull(result.Value.FileContent);
                Assert.NotEmpty(result.Value.FileContent);
            });

            // Assert – blob upload (one call, correct name, container, content type)
            _blob.Verify(x => x.UploadFileAsync(_settings.ContainerName, expectedFile,
                                                It.IsAny<Stream>(), CsvContentType, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.Single(uploaded);
                Assert.All(uploaded, u => Assert.Equal(ContainerName, u.Container));
                Assert.All(uploaded, u => Assert.Equal(CsvContentType, u.ContentType));
                Assert.Contains(uploaded, u => u.FileName == expectedFile);
                Assert.All(uploaded, u => Assert.True(u.Copy.Length > 0));
            });

            // Assert – CSV content
            var csvText = Encoding.UTF8.GetString(result.Value.FileContent);
            var lines = csvText.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);


            Assert.Multiple(() =>
            {
                Assert.True(lines.Length >= 3, "CSV should contain header + two rows");
                Assert.StartsWith(CsvHeaderPrefixLong, lines[0]);

                // Row checks: names present and statuses correct
                var body = string.Join("\n", lines.Skip(1));
                Assert.Contains("Active Q", body);
                Assert.Contains(",Approved,", body);

                Assert.Contains("Archived Q", body);
                Assert.Contains(",Archived,", body);
            });

            _logRepo.Verify(x => x.CreateAsync(
                It.Is<QualificationOutputFileLog>(h =>
                    h.UserDisplayName == _testRequest.CurrentUsername &&
                    h.FileName == expectedFile &&
                    h.PublicationDate == _testRequest.PublicationDate &&
                    h.DownloadDate <= DateTime.UtcNow.AddSeconds(5) &&
                    h.DownloadDate >= DateTime.UtcNow.AddMinutes(-1)
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Then_No_Qualifications_Returns_Failure_And_No_Uploads()
        {
            // Arrange
            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ReturnsAsync(new List<QualificationOutputFile>());

            // Act
            var result = await _handler.Handle(_testRequest, CancellationToken.None);

            // Assert
            _repo.Verify(x => x.GetQualificationOutputFile(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.Equal(ErrorNoQualifications, result.ErrorMessage);
                Assert.Null(result.InnerException);          
                Assert.NotNull(result.Value);                  
            });

            _blob.Verify(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _logRepo.Verify(x => x.CreateAsync(It.IsAny<QualificationOutputFileLog>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Then_Exception_Is_Handled_And_Failure_Returned_No_Uploads()
        {
            // Arrange
            var ex = new Exception(ErrorGeneric);
            _repo.Setup(x => x.GetQualificationOutputFile()).ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(_testRequest, CancellationToken.None);

            // Assert
            _repo.Verify(x => x.GetQualificationOutputFile(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.Equal(ErrorUnexpected, result.ErrorMessage);
                Assert.NotNull(result.InnerException);      
                Assert.Equal(ex, result.InnerException);
                Assert.NotNull(result.Value);                   
            });

            _blob.Verify(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _logRepo.Verify(x => x.CreateAsync(It.IsAny<QualificationOutputFileLog>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Then_Null_From_Repository_Is_Treated_As_Fault()
        {
            // Arrange
            _repo.Setup(x => x.GetQualificationOutputFile()).ReturnsAsync((List<QualificationOutputFile>?)null!);

            // Act
            var result = await _handler.Handle(_testRequest, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.UnexpectedError, result.ErrorCode);
            Assert.NotNull(result.InnerException); 
            _blob.Verify(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _logRepo.Verify(x => x.CreateAsync(It.IsAny<QualificationOutputFileLog>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Then_Record_With_EndDate_Today_Set_To_PublicationStatus_Archive()
        {
            // Arrange: exactly today should be archived (since active is strictly > today)
            var todayUtc = DateTime.UtcNow.Date;
            var edge = new QualificationOutputFile
            {
                QualificationName = "Edge Q",
                Age1619_FundingApprovalEndDate = todayUtc
            };

            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ReturnsAsync(new List<QualificationOutputFile> { edge });

            _blob.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            var datePrefix = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var expectedFilename = $"{datePrefix}{FileSuffix}";

            // Act
            var result = await _handler.Handle(_testRequest, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.StartsWith(datePrefix, result.Value!.FileName);
            Assert.EndsWith("-AOdPOutputFile.csv", result.Value.FileName);
            Assert.NotEmpty(result.Value.FileContent);

            _blob.Verify(x => x.UploadFileAsync(
                _settings.ContainerName,
                expectedFilename,
                It.IsAny<Stream>(),
                "text/csv",
                It.IsAny<CancellationToken>()),
                Times.Once);

            // Assert – CSV has header + one data row, and row is Archived (not Approved)
            var csv = Encoding.UTF8.GetString(result.Value.FileContent);
            var lines = csv.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Header check: use whichever prefix your suite already uses
            Assert.True(lines.Length >= 2, "CSV should contain header + one row");
            Assert.StartsWith(CsvHeaderPrefixShort, lines[0]); // or CsvHeaderPrefixLong, whichever you use

            var data = lines.Skip(1).ToArray();
            Assert.Single(data);

            var row = data[0];
            Assert.Contains("Edge Q", row);
            Assert.Contains(",Archived,", row);   // status should be Archived
            Assert.DoesNotContain(",Approved,", row); // must not be Approved

        }

        [Fact]
        public async Task Then_Record_With_EndDate_After_PublicationDate_Is_Set_To_Approved()
        {
            // Arrange: future end date -> should be Approved
            var todayUtc = DateTime.UtcNow.Date;
            var future = new QualificationOutputFile
            {
                QualificationName = "Future Q",
                Age1619_FundingApprovalEndDate = todayUtc.AddDays(5)
            };

            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ReturnsAsync(new List<QualificationOutputFile> { future });

            _blob.Setup(x => x.UploadFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            var datePrefix = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var expectedFilename = $"{datePrefix}{FileSuffix}";

            // Act
            var result = await _handler.Handle(_testRequest, CancellationToken.None);

            // Assert – success and file basics
            Assert.True(result.Success);
            Assert.EndsWith(FileSuffix, result.Value!.FileName);

            // Assert – CSV content
            var csv = Encoding.UTF8.GetString(result.Value.FileContent);
            var lines = csv.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);

            Assert.True(lines.Length >= 2, "CSV should contain header + one row");
            var row = lines.Skip(1).Single();

            Assert.Contains("Future Q", row);
            Assert.Contains(",Approved,", row);
            Assert.DoesNotContain(",Archived,", row);
        }

        [Fact]
        public async Task Then_PublicationDate_Different_From_RunDate_Is_Handled_Correctly()
        {
            // Arrange
            var publicationDate = DateTime.UtcNow.AddDays(-7).Date;
            var request = new GetQualificationOutputFileQuery
            {
                CurrentUsername = "Tester",
                PublicationDate = publicationDate
            };

            var qualification = new QualificationOutputFile
            {
                QualificationName = "Test Q",
                Age1619_FundingApprovalEndDate = publicationDate.AddDays(1)
            };

            _repo.Setup(x => x.GetQualificationOutputFile())
                 .ReturnsAsync(new List<QualificationOutputFile> { qualification });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.StartsWith(publicationDate.ToString("yyyy-MM-dd"), result.Value!.FileName);
            Assert.EndsWith("-AOdPOutputFile.csv", result.Value.FileName);

            _logRepo.Verify(x => x.CreateAsync(
                It.Is<QualificationOutputFileLog>(h =>
                    h.PublicationDate == publicationDate &&
                    h.DownloadDate.Date == DateTime.UtcNow.Date),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
