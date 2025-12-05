using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.Internal;
using Moq;
using SFA.DAS.AODP.Application.Commands.Import;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Repositories.Import;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Import;

public class ImportDefundingListCommandHandlerTests
{
    private const string TargetSheetName = "Approval not extended";
    private readonly Mock<IImportRepository> mockRepo = new();
    private readonly ImportDefundingListCommandHandler importHandler;

    private static readonly string[] HeaderCols_AB = new[] { "A", "B" };
    private static readonly string[] HeaderTexts_Q_Title = new[] { "Qualification number", "Title" };
    private static readonly string[] HeaderCols_A = new[] { "A" };
    private static readonly string[] HeaderTexts_Q = new[] { "Qualification number"};

    public ImportDefundingListCommandHandlerTests() => 
        importHandler = new(mockRepo.Object);

    [Fact]
    public async Task Handle_WhenFileContentsEmpty_ShouldReturnsZero()
    {
        // Arrange
        var emptyBytes = Array.Empty<byte>();
        var ms = new MemoryStream(emptyBytes);
        var formFile = new FormFile(ms, 0, ms.Length, "file", "anything.xlsx");

        var command = new ImportDefundingListCommand
        {
            File = formFile,
            FileName = "anything.xlsx"
        };

        // Act
        var result = await importHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(0, result.Value.ImportedCount);
        mockRepo.Verify(r => r.BulkInsertAsync(It.IsAny<List<DefundingList>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOnUnsupportedFileType_ShouldReturnsError()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };
        var ms = new MemoryStream(bytes);
        var formFile = new FormFile(ms, 0, ms.Length, "file", "notexcel.txt");

        var command = new ImportDefundingListCommand
        {
            File = formFile,
            FileName = "notexcel.txt"
        };

        // Act
        var result = await importHandler.Handle(command, CancellationToken.None);

        // Act
        Assert.False(result.Success);
        Assert.Contains("Unsupported file type", result.ErrorMessage ?? string.Empty);
        mockRepo.Verify(r => r.BulkInsertAsync(It.IsAny<List<DefundingList>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTargetSheetMissing_ShouldReturnsZero()
    {
        // Arrange
        var bytes = CreateExcel(
            sheetName: "Other sheet",
            headerRowIndexOneBased: 1,
            headerColumns: HeaderCols_AB,
            headerTexts: HeaderTexts_Q_Title,
            dataRows: Array.Empty<Dictionary<string, string>>()
        );

        var ms = new MemoryStream(bytes);
        var formFile = new FormFile(ms, 0, ms.Length, "file", "file.xlsx");

        var command = new ImportDefundingListCommand
        {
            File = formFile,
            FileName = "file.xlsx"
        };

        // Act
        var result = await importHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Value.ImportedCount);
        mockRepo.Verify(r => r.BulkInsertAsync(It.IsAny<List<DefundingList>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSheetHasOneOrNoRows_ShouldReturnsZero()
    {
        // Arrange
        var bytes = CreateExcel(
            sheetName: TargetSheetName,
            headerRowIndexOneBased: 1,
            headerColumns: HeaderCols_A,
            headerTexts: HeaderTexts_Q,
            dataRows: Array.Empty<Dictionary<string, string>>()
        );

        var ms = new MemoryStream(bytes);
        var formFile = new FormFile(ms, 0, ms.Length, "file", "file.xlsx");

        var command = new ImportDefundingListCommand
        {
            File = formFile,
            FileName = "file.xlsx"
        };

        // Act
        var result = await importHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Value.ImportedCount);
        mockRepo.Verify(r => r.BulkInsertAsync(It.IsAny<List<DefundingList>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenWithExpectedValues_ShouldImportsItems_And_CallsBulkInsert()
    {
        // Arrange
        var headerCols = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
        var headerTexts = new[]
        {
            "Qualification number", "Title", "Awarding organisation", "Guided Learning Hours",
            "Sector Subject Area Tier 2", "Relevant route", "Funding offer", "In Scope", "Comments"
        };

        var dataRows = new[]
        {
                new Dictionary<string, string> {
                    ["A"] = "QAN001",
                    ["B"] = "Qualification One",
                    ["C"] = "Org A",
                    ["D"] = "10",
                    ["E"] = "SSA1",
                    ["F"] = "Route1",
                    ["G"] = "OfferA",
                    ["H"] = "No",
                    ["I"] = "First comment"
                },
                new Dictionary<string, string> {
                    ["A"] = "QAN002",
                    ["B"] = "Qualification Two",
                    ["C"] = "Org B",
                    ["D"] = "20",
                    ["E"] = "SSA2",
                    ["F"] = "Route2",
                    ["G"] = "OfferB",
                    ["H"] = "",
                    ["I"] = "Second comment"
                }
            };

        var bytes = CreateExcel(TargetSheetName, 1, headerCols, headerTexts, dataRows);

        IEnumerable<DefundingList>? captured = null;
        mockRepo.Setup(r => r.BulkInsertAsync(It.IsAny<List<DefundingList>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<DefundingList>, CancellationToken>((items, ct) => captured = items)
            .Returns(Task.CompletedTask)
            .Verifiable();
        mockRepo.Setup(r => r.DeleteDuplicateAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var ms = new MemoryStream(bytes);
        var formFile = new FormFile(ms, 0, ms.Length, "file", "file.xlsx");

        var command = new ImportDefundingListCommand
        {
            File = formFile,
            FileName = "file.xlsx"
        };

        // Act
        var result = await importHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Value.ImportedCount);
        mockRepo.Verify(r => r.BulkInsertAsync(It.IsAny<List<DefundingList>>(), It.IsAny<CancellationToken>()), Times.Once);
        mockRepo.Verify(r => r.DeleteDuplicateAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(captured);
        var list = captured!.ToList();
        Assert.Equal(2, list.Count);

        var first = list[0];
        Assert.Equal("QAN001", first.Qan);
        Assert.Equal("Qualification One", first.Title);
        Assert.Equal("Org A", first.AwardingOrganisation);
        Assert.Equal("10", first.GuidedLearningHours);
        Assert.Equal("SSA1", first.SectorSubjectArea);
        Assert.Equal("Route1", first.RelevantRoute);
        Assert.Equal("OfferA", first.FundingOffer);
        Assert.False(first.InScope);
        Assert.Equal("First comment", first.Comments);
        Assert.True((DateTime.UtcNow - first.ImportDate).TotalMinutes < 5);

        var second = list[1];
        Assert.Equal("QAN002", second.Qan);
        Assert.True(second.InScope);
    }

    [Fact]
    public async Task Handle_WhenImportsRows_ShouldHeaderLocatedAtRow7()
    {
        // Arrange
        var headerCols = new[] { "A", "B", "C", "H" };
        var headerTexts = new[] { "Qualification number", "Title", "Awarding organisation", "In Scope" };

        var dataRows = new[]
        {
                new Dictionary<string,string> {
                    ["A"] = "Q7-1",
                    ["B"] = "Title7-1",
                    ["C"] = "Org7",
                    ["H"] = "1"
                },
                new Dictionary<string,string> {
                    ["A"] = "Q7-2",
                    ["B"] = "Title7-2",
                    ["C"] = "Org7",
                    ["H"] = "excluded"
                }
            };

        var bytes = CreateExcel(TargetSheetName, 7, headerCols, headerTexts, dataRows);

        IEnumerable<DefundingList>? captured = null;
        mockRepo.Setup(r => r.BulkInsertAsync(It.IsAny<List<DefundingList>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<DefundingList>, CancellationToken>((items, ct) => captured = items)
            .Returns(Task.CompletedTask);

        var ms = new MemoryStream(bytes);
        var formFile = new FormFile(ms, 0, ms.Length, "file", "file.xlsx");

        var command = new ImportDefundingListCommand
        {
            File = formFile,
            FileName = "file.xlsx"
        };

        // Act
        var result = await importHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Value.ImportedCount);
        var list = captured!.ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("Q7-1", list[0].Qan);
        Assert.True(list[0].InScope);
        Assert.Equal("Q7-2", list[1].Qan);
        Assert.False(list[1].InScope); 
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldReturnsError()
    {
        // Arrange
        var headerCols = new[] { "A" };
        var headerTexts = new[] { "Qualification number" };
        var dataRows = new[]
        {
                new Dictionary<string,string> { ["A"] = "QEX-1" }
            };

        var bytes = CreateExcel(TargetSheetName, 1, headerCols, headerTexts, dataRows);

        mockRepo.Setup(r => r.BulkInsertAsync(It.IsAny<List<DefundingList>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var ms = new MemoryStream(bytes);
        var formFile = new FormFile(ms, 0, ms.Length, "file", "file.xlsx");

        var command = new ImportDefundingListCommand
        {
            File = formFile,
            FileName = "file.xlsx"
        };

        // Act
        var result = await importHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("DB error", result.ErrorMessage ?? string.Empty);
        Assert.NotNull(result.InnerException);
    }

    private static byte[] CreateExcel(string sheetName, int headerRowIndexOneBased, string[] headerColumns, string[] headerTexts, IEnumerable<Dictionary<string, string>> dataRows)
    {
        using var ms = new MemoryStream();
        using (var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            var sharedStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
            sharedStringPart.SharedStringTable = new SharedStringTable();
            var sst = sharedStringPart.SharedStringTable;

            int AddSharedString(string text)
            {
                if (text == null) text = string.Empty;
                var existing = sst.Elements<SharedStringItem>().Select((item, idx) => new { item, idx })
                    .FirstOrDefault(x => (x.item.Text?.Text ?? x.item.InnerText ?? string.Empty) == text);
                if (existing != null)
                    return existing.idx;

                var ssi = new SharedStringItem(new Text(text));
                sst.AppendChild(ssi);
                return sst.Elements<SharedStringItem>().Count() - 1;
            }

            for (int r = 1; r <= headerRowIndexOneBased; r++)
            {
                var row = new Row() { RowIndex = (UInt32)r };
                if (r == headerRowIndexOneBased)
                {
                    for (int c = 0; c < headerColumns.Length && c < headerTexts.Length; c++)
                    {
                        var col = headerColumns[c];
                        var txt = headerTexts[c] ?? string.Empty;
                        var cell = CreateSharedStringCell(col + r, txt, AddSharedString);
                        row.Append(cell);
                    }
                }
                sheetData.Append(row);
            }

            int currentRow = headerRowIndexOneBased + 1;
            foreach (var data in dataRows)
            {
                var row = new Row() { RowIndex = (UInt32)currentRow };
                foreach (var kv in data)
                {
                    var col = kv.Key;
                    var val = kv.Value ?? string.Empty;

                    var cell = CreateSharedStringCell(col + currentRow, val, AddSharedString);
                    row.Append(cell);
                }
                sheetData.Append(row);
                currentRow++;
            }

            var sheet = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = sheetName
            };
            sheets.Append(sheet);

            workbookPart.Workbook.Save();
        }

        return ms.ToArray();
    }

    private static Cell CreateSharedStringCell(string cellReference, string text, Func<string, int> addSharedString)
    {
        var idx = addSharedString(text);
        var cell = new Cell()
        {
            CellReference = cellReference,
            DataType = CellValues.SharedString,
            CellValue = new CellValue(idx.ToString())
        };
        return cell;
    }
}
