using Moq;
using SFA.DAS.AODP.Application.Queries.QaaQualification;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;
using System.Reflection;
using System.Text;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.QaaQualification
{
    public class GetQaaQualificationsExportQueryHandlerTests
    {
        private readonly Mock<IQaaQualificationRepository> _qaaQualificationRepositoryMock;
        private readonly Mock<IQaaQualificationDownloadLogRepository> _downloadLogRepositoryMock;
        private readonly GetQaaQualificationsExportQueryHandler _handler;

        public GetQaaQualificationsExportQueryHandlerTests()
        {
            _qaaQualificationRepositoryMock = new Mock<IQaaQualificationRepository>();
            _downloadLogRepositoryMock = new Mock<IQaaQualificationDownloadLogRepository>();
            _handler = new GetQaaQualificationsExportQueryHandler(
                _qaaQualificationRepositoryMock.Object,
                _downloadLogRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_NoQualifications_ReturnsNoDataFailure()
        {
            // Arrange
            _qaaQualificationRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<RegulatedQaaQualification>());

            // Act
            var result = await _handler.Handle(new GetQaaQualificationsExportQuery { CurrentUsername = "tester" }, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(GetQaaQualificationsExportQueryHandler.NoQualificationsFound, result.ErrorMessage);
            _downloadLogRepositoryMock.Verify(
                r => r.CreateAsync(It.IsAny<QaaQualificationDownloadLog>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryThrows_ReturnsUnexpectedErrorFailure()
        {
            // Arrange
            _qaaQualificationRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("boom"));

            // Act
            var result = await _handler.Handle(new GetQaaQualificationsExportQuery { CurrentUsername = "tester" }, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(GetQaaQualificationsExportQueryHandler.UnexpectedErrorGeneratingFile, result.ErrorMessage);
            Assert.IsType<InvalidOperationException>(result.InnerException);
        }

        [Fact]
        public async Task Handle_Success_WritesCorrectHeaderAndStatusPerRow()
        {
            // Arrange
            var newQual = RegulatedQaaQualification.Create(
                new DateTime(2026, 1, 1), "AIM001", "New Qual", "Awarding Body A",
                new DateOnly(2025, 1, 1), new DateOnly(2027, 1, 1), SectorSubjectArea.Science)
                .SetChangeOutcome(QaaImportComparisonOutcome.New, QaaLastDateForRegistrationChangeType.NotChanged);

            var extendedQual = RegulatedQaaQualification.Create(
                new DateTime(2026, 1, 1), "AIM002", "Extended Qual", "Awarding Body B",
                new DateOnly(2025, 1, 1), new DateOnly(2028, 1, 1), SectorSubjectArea.Agriculture)
                .SetChangeOutcome(QaaImportComparisonOutcome.LastDateForRegistrationChanged, QaaLastDateForRegistrationChangeType.Extended);

            var unchangedQual = RegulatedQaaQualification.Create(
                new DateTime(2026, 1, 1), "AIM003", "Unchanged Qual", "Awarding Body C",
                new DateOnly(2025, 1, 1), new DateOnly(2027, 6, 1), SectorSubjectArea.Science)
                .SetChangeOutcome(QaaImportComparisonOutcome.NotChanged, QaaLastDateForRegistrationChangeType.NotChanged);

            var discontinuedQual = RegulatedQaaQualification.Create(
                new DateTime(2026, 1, 1), "AIM004", "Discontinued Qual", "Awarding Body D",
                new DateOnly(2025, 1, 1), new DateOnly(2027, 1, 1), SectorSubjectArea.Science,
                lastDateForCertifications: new DateOnly(2028, 12, 31));
            SetPrivateProperty(discontinuedQual, nameof(RegulatedQaaQualification.IsDiscontinued), true);
            SetPrivateProperty(discontinuedQual, nameof(RegulatedQaaQualification.DiscontinuedDate), new DateOnly(2026, 3, 1));

            _qaaQualificationRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { newQual, extendedQual, unchangedQual, discontinuedQual });

            QaaQualificationDownloadLog? capturedLog = null;
            _downloadLogRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<QaaQualificationDownloadLog>(), It.IsAny<CancellationToken>()))
                .Callback<QaaQualificationDownloadLog, CancellationToken>((log, _) => capturedLog = log)
                .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(new GetQaaQualificationsExportQuery { CurrentUsername = "tester" }, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            var csv = Encoding.UTF8.GetString(result.Value!.FileContent).TrimStart('﻿');
            var lines = csv.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');

            Assert.Equal("Snapshot Date,QualificationID,Status,Awarding Body Name,Qualification Title,SSA,Start Date,Certification End Date,Discontinued Date,Registration End Date", lines[0]);
            Assert.Contains("AIM001", lines[1]);
            Assert.Contains(",New,", lines[1]);
            Assert.Contains(",Extended,", lines[2]);
            Assert.Contains(",Unchanged,", lines[3]);
            Assert.Contains(",Discontinued,", lines[4]);
            Assert.Contains("2028-12-31", lines[4]);
            Assert.Contains("2026-03-01", lines[4]);

            Assert.NotNull(capturedLog);
            Assert.Equal("tester", capturedLog!.UserDisplayName);
        }

        [Fact]
        public async Task Handle_Success_QuotesSectorSubjectAreaNamesContainingCommas()
        {
            // Arrange
            var qual = RegulatedQaaQualification.Create(
                new DateTime(2026, 1, 1), "AIM005", "Sport Qual", "Awarding Body E",
                new DateOnly(2025, 1, 1), new DateOnly(2027, 1, 1), SectorSubjectArea.SportLeisureRecreation)
                .SetChangeOutcome(QaaImportComparisonOutcome.New, QaaLastDateForRegistrationChangeType.NotChanged);

            _qaaQualificationRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { qual });

            _downloadLogRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<QaaQualificationDownloadLog>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(new GetQaaQualificationsExportQuery { CurrentUsername = "tester" }, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            var csv = Encoding.UTF8.GetString(result.Value!.FileContent).TrimStart('﻿');
            var lines = csv.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
            var fields = ParseCsvLine(lines[1]);

            Assert.Equal(10, fields.Count);
            Assert.Equal("Sport, leisure and recreation", fields[5]);
        }

        private static void SetPrivateProperty<T>(RegulatedQaaQualification target, string propertyName, T value)
        {
            var property = typeof(RegulatedQaaQualification).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!;
            property.SetValue(target, value);
        }

        private static List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (inQuotes)
                {
                    if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else if (c == '"')
                    {
                        inQuotes = false;
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            fields.Add(current.ToString());
            return fields;
        }
    }
}
