using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.CrossServiceReporter.Converter;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.BocchiTracker.UnitTests.CrossServiceReporter.Converter
{
    public class AppInfoToCustomFieldsConverterTests
    {
        [Fact]
        public void Convert_ReturnsCorrectCustomFields()
        {
            // Arrange
            var appBasicInfo = new AppBasicInfo 
            {
                ClientID = 1,
                Pid = "1245",
                AppName = "TestApp",
                Args = "TestArgs",
                Platform = "Windows",
                Version = "CL.xxxx"
            };

            var appBundle = new AppStatusBundle(appBasicInfo.ClientID) 
            { 
                AppBasicInfo = appBasicInfo,
                AppStatusDynamics = new Dictionary<string, dynamic>
                {
                    { "Status1", "Running" },
                    { "Status2", "Error" }
                }
            };

            var converter = new AppInfoToCustomFieldsConverter();

            // Act
            var customFields = converter.Convert(appBundle);

            // Assert
            Assert.Equal(7, customFields.Count); // 3 つのカスタムフィールドが期待される

            // Version フィールドのチェック
            Assert.True(customFields.ContainsKey("AppBasicInfo.pid"));
            Assert.Single(customFields["AppBasicInfo.pid"]);
            Assert.Equal("1245", customFields["AppBasicInfo.pid"][0]);

            // Author フィールドのチェック
            Assert.True(customFields.ContainsKey("AppBasicInfo.app_name"));
            Assert.Single(customFields["AppBasicInfo.app_name"]);
            Assert.Equal("TestApp", customFields["AppBasicInfo.app_name"][0]);

            // Status1 フィールドのチェック
            Assert.True(customFields.ContainsKey("AppBasicInfo.args"));
            Assert.Single(customFields["AppBasicInfo.args"]);
            Assert.Equal("TestArgs", customFields["AppBasicInfo.args"][0]);

            // Status2 フィールドのチェック
            Assert.True(customFields.ContainsKey("AppBasicInfo.platform"));
            Assert.Single(customFields["AppBasicInfo.platform"]);
            Assert.Equal("Windows", customFields["AppBasicInfo.platform"][0]);

            // Status2 フィールドのチェック
            Assert.True(customFields.ContainsKey("AppBasicInfo.version"));
            Assert.Single(customFields["AppBasicInfo.version"]);
            Assert.Equal("CL.xxxx", customFields["AppBasicInfo.version"][0]);

            // Status2 フィールドのチェック
            Assert.True(customFields.ContainsKey("Status1"));
            Assert.Single(customFields["Status1"]);
            Assert.Equal("Running", customFields["Status1"][0]);

            // Status2 フィールドのチェック
            Assert.True(customFields.ContainsKey("Status2"));
            Assert.Single(customFields["Status2"]);
            Assert.Equal("Error", customFields["Status2"][0]);
        }
    }
}