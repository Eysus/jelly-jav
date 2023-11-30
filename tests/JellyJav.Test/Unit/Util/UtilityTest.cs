using FluentAssertions;
using JellyJav.Plugin.Util;

namespace JellyJav.Test.Unit.Util
{
    [TestClass]
    public class UtilityTest
    {
        /// <summary>
        /// Scenario: Classic JAV Code from file name
        ///     Given the name of the file
        ///     When the code is extracted from the file name
        ///     Then the code is available in a proper format
        /// </summary>
        [DataTestMethod]
        [DataRow("abp200.mkv")]
        [DataRow("ABP200.mkv")]
        [DataRow("some random text abp-200 more random text.mkv")]
        [DataRow("ABP-200.mkv")]
        [DataRow("abp00200.mkv")]
        [DataRow("abp00200-0.mkv")]
        [DataRow("h_094abp00200-0.mkv")]
        public void ClassicJavCodeFromFileName(string filename)
        {
            string? code = Utility.ExtractCodeFromFilename(filename);
            code.Should().NotBeNull();
            code.Should().Be("ABP-200");
        }

        /// <summary>
        /// Scenario: JAV Code with numbers in serie's part from file name
        ///     Given the name of the file
        ///     When the code is extracted from the file name
        ///     Then the code is available in a proper format
        /// </summary>
        [DataTestMethod]
        [DataRow("t28510.mkv")]
        [DataRow("T28510.mkv")]
        [DataRow("T28-510.mkv")]
        [DataRow("some random text t28-510 more random text.mkv")]
        [DataRow("55t2800510.mkv")]
        [DataRow("55t2800510-0.mkv")]
        [DataRow("h_55t2800510-0.mkv")]
        [DataRow("55t2800510.mkv")]
        public void JavCodeWithNumbersInSeriesPartFromFileName(string filename)
        {
            string? code = Utility.ExtractCodeFromFilename(filename);
            code.Should().NotBeNull();
            code.Should().Be("T28-510");
        }

        /// <summary>
        /// Scenario: FC2 JAV Code from file name
        ///     Given the name of the file
        ///     When the code is extracted from the file name
        ///     Then the code is available in a proper format
        /// </summary>
        [DataTestMethod]
        [DataRow("FC2PPV3218133.mkv")]
        [DataRow("FC2-PPV-3218133.mkv")]
        [DataRow("FC2PPV-3218133.mkv")]
        [DataRow("FC2-PPV3218133.mkv")]
        [DataRow("some random text FC2PPV3218133 more random text.mkv")]
        [DataRow("fc2ppv3218133.mkv")]
        public void Fc2JavCodeFromFileName(string filename)
        {
            string? code = Utility.ExtractFC2CodeFromFilename(filename);
            code.Should().NotBeNull();
            code.Should().Be("FC2-PPV-3218133");
        }

        /// <summary>
        /// Scenario: No code available in a string
        ///     Given the name of the file
        ///     And the name doesn't content code
        ///     When trying to extract the code
        ///     Then the result is null
        /// </summary>
        [TestMethod]
        public void ExtractCodeFromFileName_Null_Test()
        {
            string filename = "amzkdjzamf.mp4";
            string result = Utility.ExtractCodeFromFilename(filename);
            result.Should().BeNull();
        }

        //TODO: To evolve
        [TestMethod]
        public void ExtractCodeFromFileName_ForDigit_Test()
        {
            string filename = "abc-1234.mp4";
            string result = Utility.ExtractCodeFromFilename(filename);
            result.Should().Be("ABC-123");
        }

        [TestMethod]
        public void ReverseName_Test()
        {
            Utility.ReverseName("First Last").Should().Be("Last First");
        }
    }
}
