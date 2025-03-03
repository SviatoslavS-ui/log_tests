using log_analyzer.LogAnalyzer.Core;
using log_analyzer.LogAnalyzer.Files;

namespace LogAnalyzer.Tests
{
    [TestFixture]
    public class LogFileTests
    {
        private LogFileProcessor _processor;
        private string _fileName;

        [SetUp]
        public void SetUp()
        {
            _processor = new LogFileProcessor();
            _fileName = Path.GetTempFileName();
        }

        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }
        }

        [Test, TestCaseSource(typeof(LogParserTestData), nameof(LogParserTestData.ValidLogs))]
        public void WriteLogsTest(string[] logLines, List<LogEntry> logEntries)
        {
            //Act
            _processor.WriteLogs(_fileName, logEntries);
            var writtenLines = File.ReadAllLines(_fileName);

            //Assert
            Assert.That(writtenLines, Has.Length.EqualTo(logLines.Length));

            for (int i = 0; i < logLines.Length; i++)
            {
                Assert.That(writtenLines[i], Is.EqualTo(logLines[i]));
            }
        }

        [Test]
        public void ReadLogsTest()
        {
            // Arrange
            var lines = new[]
            {
            "2025-03-01 12:00:00 | INFO | Test log 1",
            "2025-03-01 12:05:00 | ERROR | Test log 2"
            };
            File.WriteAllLines(_fileName, lines);

            // Act
            var logs = _processor.ReadLogs(_fileName).ToList();

            // Assert
            Assert.That(logs, Has.Count.EqualTo(2));
            Assert.Multiple(() => {
                Assert.That(logs[0].Timestamp, Is.EqualTo(DateTime.Parse("2025-03-01 12:00:00")));
                Assert.That(logs[0].Level, Is.EqualTo("INFO"));
                Assert.That(logs[0].Message, Is.EqualTo("Test log 1"));
            });
        }
    }
}
