using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Specialized;
using JellyJav.Plugin.Util;

namespace JellyJav.Test.Util
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void ExtractCodeFromFilename_Ok()
        {
            string[] filenames = {
                "abp200.mkv",
                "ABP200.mkv",
                "ABP-200.mkv",
                "some random text abp-200 more random text.mkv",
                "abp00200.mkv",
                "abp00200-0.mkv",
                "h_094abp00200-0.mkv",
            };

            foreach (string filename in filenames)
            {
                Utility.ExtractCodeFromFilename(filename).Should().Be("ABP-200");
            }
        }

        [TestMethod]
        public void ExtractCodeFromFilename_WithNumberInCode_Ok()
        {
            string[] filenames = {
                "t28510.mkv",
                "T28510.mkv",
                "T28-510.mkv",
                "some random text t28-510 more random text.mkv",
                "55t2800510.mkv",
                "55t2800510-0.mkv",
                "h_55t2800510-0.mkv",
                "55t2800510.mkv"
            };

            foreach (string filename in filenames)
            {
                Utility.ExtractCodeFromFilename(filename).Should().Be("T28-510");
            }
        }
    }
}
