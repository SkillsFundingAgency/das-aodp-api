using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Moq;
using SFA.DAS.AODP.Application.Commands.Import;
using SFA.DAS.AODP.Data.Entities.Import;
using SFA.DAS.AODP.Data.Repositories.Import;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Import;

public class ImportPldnsCommandHandlerTests
{
    [Fact]
    public async Task Handle_FileNull_ReturnsZero()
    {
        // Arrange
        var repoMock = new Mock<IImportRepository>(MockBehavior.Strict);
        var handler = new ImportPldnsCommandHandler(repoMock.Object);

        var request = new ImportPldnsCommand { File = null };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(0, result.Value.ImportedCount);
        repoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_SheetNotFound_ReturnsZero()
    {
        var repoMock = new Mock<IImportRepository>(MockBehavior.Strict);
        var headers = new List<string> { "Text QAN" };
        var data = new List<IList<object>>();
        var bytes = CreateExcelBytes("SomeOtherSheet", headers, data);

        var file = CreateMockFormFile(bytes);

        var handler = new ImportPldnsCommandHandler(repoMock.Object);
        var request = new ImportPldnsCommand { File = file, FileName = "file.xlsx" };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(0, result.Value.ImportedCount);
        repoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ImportsRows_CallsRepositoryAndReturnsCount()
    {
        var repoMock = new Mock<IImportRepository>();

        List<Pldns>? capturedItems = null;
        repoMock
            .Setup(r => r.BulkInsertAsync(It.IsAny<IEnumerable<Pldns>>(), It.IsAny<CancellationToken>()))
            .Returns<IEnumerable<Pldns>, CancellationToken>((items, ct) =>
            {
                capturedItems = items.ToList();
                return Task.CompletedTask;
            });

        repoMock
            .Setup(r => r.DeleteDuplicateAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var headers = new List<string>
            {
                "Text QAN",
                "Date PLDNS list updated",
                "NOTE",
                "PLDNS 14-16"
            };

        var oaDate = DateTime.Parse("2020-01-01").ToOADate();
        var dataRows = new List<IList<object>>
            {
                new List<object> { "QAN-001", "01/08/2019", "some note", oaDate }
            };

        var bytes = CreateExcelBytes("PLDNS V12F", headers, dataRows);
        var file = CreateMockFormFile(bytes);

        var handler = new ImportPldnsCommandHandler(repoMock.Object);
        var request = new ImportPldnsCommand { File = file, FileName = "file.xlsx" };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(1, result.Value.ImportedCount);
        repoMock.Verify(r => r.BulkInsertAsync(It.IsAny<IEnumerable<Pldns>>(), It.IsAny<CancellationToken>()), Times.Once);
        repoMock.Verify(r => r.DeleteDuplicateAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(capturedItems);
        var item = capturedItems!.Single();
        Assert.Equal("QAN-001", item.Qan);
        Assert.Equal(new DateTime(2019, 8, 1), item.ListUpdatedDate);
        Assert.Equal(new DateTime(2020, 1, 1), item.Pldns14To16);
        Assert.Equal("some note", item.Notes);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_ReturnsFailureAndSetsInnerException()
    {
        var repoMock = new Mock<IImportRepository>();

        repoMock
            .Setup(r => r.BulkInsertAsync(It.IsAny<IEnumerable<Pldns>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("insert failed"));

        var headers = new List<string> { "Text QAN" };
        var dataRows = new List<IList<object>> { new List<object> { "QAN-002" } };

        var bytes = CreateExcelBytes("PLDNS V12F", headers, dataRows);
        var file = CreateMockFormFile(bytes);

        var handler = new ImportPldnsCommandHandler(repoMock.Object);
        var request = new ImportPldnsCommand { File = file, FileName = "file.xlsx" };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("insert failed", result.ErrorMessage);
        Assert.NotNull(result.InnerException);
        Assert.IsType<InvalidOperationException>(result.InnerException);
        repoMock.Verify(r => r.BulkInsertAsync(It.IsAny<IEnumerable<Pldns>>(), It.IsAny<CancellationToken>()), Times.Once);
        repoMock.Verify(r => r.DeleteDuplicateAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Never);
    }

    private static byte[] CreateExcelBytes(string sheetName, IList<string> headers, IList<IList<object>> dataRows)
    {
        using var ms = new MemoryStream();
        using (var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, true))
        {
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var sstPart = workbookPart.AddNewPart<SharedStringTablePart>();
            sstPart.SharedStringTable = new SharedStringTable();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet(sheetData);

            int AddSharedString(string text)
            {
                var sst = sstPart.SharedStringTable;
                var si = new SharedStringItem(new Text(text));
                sst.AppendChild(si);
                sst.Save();
                return sst.ChildElements.Count - 1;
            }

            string ColLetter(int index)
            {
                const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                if (index < 26) return letters[index].ToString();
                // simple support for up to ZZ
                var first = (index / 26) - 1;
                var second = index % 26;
                return letters[first] + letters[second].ToString();
            }

            uint rowIndex = 1;
            var initialRow = new Row { RowIndex = rowIndex++ };
            sheetData.Append(initialRow);

            var headerRow = new Row { RowIndex = rowIndex++ };
            for (int i = 0; i < headers.Count; i++)
            {
                var col = ColLetter(i);
                var text = headers[i] ?? string.Empty;
                var idx = AddSharedString(text);
                var cell = new Cell
                {
                    CellReference = $"{col}{headerRow.RowIndex}",
                    DataType = CellValues.SharedString,
                    CellValue = new CellValue(idx.ToString())
                };
                headerRow.Append(cell);
            }
            sheetData.Append(headerRow);

            foreach (var dataRow in dataRows)
            {
                var r = new Row { RowIndex = rowIndex++ };
                for (int i = 0; i < dataRow.Count; i++)
                {
                    var col = ColLetter(i);
                    var value = dataRow[i];
                    Cell cell;
                    if (value is string s)
                    {
                        var idx = AddSharedString(s);
                        cell = new Cell
                        {
                            CellReference = $"{col}{r.RowIndex}",
                            DataType = CellValues.SharedString,
                            CellValue = new CellValue(idx.ToString())
                        };
                    }
                    else if (value is double d)
                    {
                        cell = new Cell
                        {
                            CellReference = $"{col}{r.RowIndex}",
                            CellValue = new CellValue(d.ToString(System.Globalization.CultureInfo.InvariantCulture))
                        };
                    }
                    else if (value is DateTime dt)
                    {
                        var idx = AddSharedString(dt.ToString("dd/MM/yyyy"));
                        cell = new Cell
                        {
                            CellReference = $"{col}{r.RowIndex}",
                            DataType = CellValues.SharedString,
                            CellValue = new CellValue(idx.ToString())
                        };
                    }
                    else
                    {
                        var idx = AddSharedString(value?.ToString() ?? string.Empty);
                        cell = new Cell
                        {
                            CellReference = $"{col}{r.RowIndex}",
                            DataType = CellValues.SharedString,
                            CellValue = new CellValue(idx.ToString())
                        };
                    }
                    r.Append(cell);
                }
                sheetData.Append(r);
            }

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            var sheetId = 1U;
            var sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);

            workbookPart.Workbook.Save();
        }
        return ms.ToArray();
    }

    private static IFormFile CreateMockFormFile(byte[] bytes)
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.Length).Returns(bytes.Length);
        mock.Setup(f => f.FileName).Returns("test.xlsx");
        mock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>(async (target, ct) =>
            {
                using var src = new MemoryStream(bytes);
                src.Position = 0;
                await src.CopyToAsync(target, ct).ConfigureAwait(false);
            });
        return mock.Object;
    }
}
