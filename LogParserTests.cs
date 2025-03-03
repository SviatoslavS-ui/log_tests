using log_analyzer.LogAnalyzer.Core;

namespace LogAnalyzer.Tests
{
    [TestFixture]
    internal class LogParserTests
    {
        private LogParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new LogParser();
        }

        [Test, TestCaseSource(typeof(LogParserTestData), nameof(LogParserTestData.ValidLogs))]
        public void ParseValidLogFile(string[] logLines, List<LogEntry> expectedEntries)
        {
            //Arrange            
            //var filePath = CreateTempFile(logLines);

            //Act
            var result = _parser.Parse(logLines).ToList();

            //Assert
            Assert.That(result, Has.Count.EqualTo(expectedEntries.Count));

            for (int i = 0; i < expectedEntries.Count; i++)
            {
                Assert.Multiple(() => {
                    Assert.That(result[i].Timestamp, Is.EqualTo(expectedEntries[i].Timestamp));
                    Assert.That(result[i].Level, Is.EqualTo(expectedEntries[i].Level));
                    Assert.That(result[i].Message, Is.EqualTo(expectedEntries[i].Message));
                });
            }
        }

        [Test, TestCaseSource(typeof(LogParserTestData), nameof(LogParserTestData.InvalidLogs))]
        public void ParseInvalidLinesAreSkipped(string[] logLines, int expectedCount)
        {
            // Arrange            
            //var filePath = CreateTempFile(logLines);

            //Act
            var result = _parser.Parse(logLines).ToList();

            //Assert
            Assert.That(result, Has.Count.EqualTo(expectedCount));

            Assert.Multiple(() => {
                Assert.That(result[0].Timestamp, Is.EqualTo(new DateTime(2025, 2, 28, 12, 0, 0)));
                Assert.That(result[0].Level, Is.EqualTo("INFO"));
                Assert.That(result[0].Message, Is.EqualTo("Valid entry"));
            });
        }

        [Test]
        public void ParseEmptyLogFile()
        {
            //Arrange
            //var filePath = CreateTempFile(Array.Empty<string>());

            //Act
            var result = _parser.Parse(Array.Empty<string>()).ToList();

            //Assert
            Assert.That(result, Is.Empty);
        }

        private string CreateTempFile(IEnumerable<string> logLines)
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, logLines);
            return tempFile;
        }
    }

    public static class LogParserTestData
    {
        public static IEnumerable<TestCaseData> ValidLogs()
        {
            yield return new TestCaseData(new string[]
                {
                    "2025-02-28 12:00:00 | INFO | System started",
                    "2025-02-28 12:05:00 | ERROR | Connection failed",
                    "2025-02-28 12:07:10 | INFO | Database connected"
                },
            new List<LogEntry>
                {
                    new LogEntry(new DateTime(2025, 2, 28, 12, 0, 0), "INFO", "System started"),
                    new LogEntry(new DateTime(2025, 2, 28, 12, 5, 0), "ERROR", "Connection failed"),
                    new LogEntry(new DateTime(2025, 2, 28, 12, 7, 10), "INFO", "Database connected")
                }
            );
        }

        public static IEnumerable<TestCaseData> InvalidLogs()
        {
            yield return new TestCaseData(new string[]
                {
                    "Invalid log line",
                    "2025-02-28 12:00:00 | INFO | Valid entry",
                    "Another bad line"
                }, 1); // only 1 correct line
        }
    }
}
