using log_analyzer.LogAnalyzer.Core;

namespace LogAnalyzer.Tests
{
    [TestFixture]
    public class LogFilterTests
    {
        private LogFilter _filter;

        [SetUp]
        public void SetUp()
        {
            _filter = new LogFilter();
        }

        [Test, TestCaseSource(typeof(LogFilterTestData), nameof(LogFilterTestData.FilterTestCases))]
        public IEnumerable<LogEntry> LogFilterTestDifferentCases(string? level, DateTime? from, DateTime? to, string? keyword)
        {
            return _filter.Filter(LogFilterTestData.Logs, level, keyword, from, to);
        }

        [Test]
        public void FilterByLevelTest()
        {
            //Arrange
            const string level = "INFO";

            //Act
            var result = _filter.Filter(LogFilterTestData.Logs, level: level);

            //Assert
            Assert.Multiple(() => {
                Assert.That(result.Select(log => log.Level), Is.All.EqualTo(level));
                Assert.That(result.Count(), Is.EqualTo(2));
            });
        }

        [Test]
        public void Filter_ByLevel_NotExisting()
        {
            //Arrange
            const string level = "CRITICAL";

            //Act
            var result = _filter.Filter(LogFilterTestData.Logs, level: level);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Filter_ByKeyword_CaseInsensitive()
        {
            //Act
            var result = _filter.Filter(LogFilterTestData.Logs, keyword: "database");

            //Assert
            Assert.Multiple(() => {
                Assert.That(result.Count(), Is.EqualTo(1));
                Assert.That(result.First().Message, Does.Contain("Database"));
            });
        }
    }

    public static class LogFilterTestData
    {
        internal static readonly List<LogEntry> Logs = new()
        {
            new LogEntry(new DateTime(2025, 2, 28), "INFO", "Application started"),
            new LogEntry(new DateTime(2025, 2, 28), "ERROR", "Database connection failed"),
            new LogEntry(new DateTime(2025, 2, 27), "WARNING", "Low disk space"),
            new LogEntry(new DateTime(2025, 2, 26), "DEBUG", "Memory allocation details"),
            new LogEntry(new DateTime(2025, 2, 25), "INFO", "User login success")
        };

        public static IEnumerable<TestCaseData> FilterTestCases()
        {
            yield return new TestCaseData("INFO", null, null, null)
                .SetName("Filter_ByLevel_INFO")
                .Returns(Logs.Where(l => l.Level == "INFO"));

            yield return new TestCaseData(null, new DateTime(2025, 2, 27), null, null)
                .SetName("Filter_FromDate_27Feb")
                .Returns(Logs.Where(l => l.Timestamp >= new DateTime(2025, 2, 27)));

            yield return new TestCaseData(null, null, new DateTime(2025, 2, 27), null)
                .SetName("Filter_ToDate_27Feb")
                .Returns(Logs.Where(l => l.Timestamp <= new DateTime(2025, 2, 27)));

            yield return new TestCaseData(null, null, null, "Database")
                .SetName("Filter_ByKeyword_Database")
                .Returns(Logs.Where(l => l.Message.Contains("Database", StringComparison.OrdinalIgnoreCase)));

            yield return new TestCaseData("ERROR", new DateTime(2025, 2, 27), new DateTime(2025, 2, 28), null)
                .SetName("Filter_ByLevelAndDateRange")
                .Returns(Logs.Where(l => l.Level == "ERROR" && l.Timestamp >= new DateTime(2025, 2, 27) && l.Timestamp <= new DateTime(2025, 2, 28)));

            yield return new TestCaseData(null, null, null, null)
                .SetName("Filter_NoFilters_ReturnAll")
                .Returns(Logs);
        }
    }
}
