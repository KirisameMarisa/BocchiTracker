using BocchiTracker.ServiceClientAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.ServiceClientAdapters
{
    public class DescriptionParserTests
    {
        [Fact]
        public void ParserTest_1()
        {
            // Arrange
            string inputText = "(< PositionX:10.0 >, < PositionY:20.0 >, < PositionZ:30.0 >)";

            var parser = new DescriptionParser();
            
            // Act
            var result = parser.Parse(inputText);

            // Assert
            Assert.Equal("10.0", result["PositionX"][0]);
            Assert.Equal("20.0", result["PositionY"][0]);
            Assert.Equal("30.0", result["PositionZ"][0]);
        }

        [Fact]
        public void ParserTest_2()
        {
            // Arrange
            string inputText = "- < PositionX:10.0 >";

            var parser = new DescriptionParser();

            // Act
            var result = parser.Parse(inputText);

            // Assert
            Assert.Equal("10.0", result["PositionX"][0]);
        }

        [Fact]
        public void ParserTest_3()
        {
            // Arrange
            string inputText = "- < Labels:label1, label2, label3 >";

            var parser = new DescriptionParser();

            // Act
            var result = parser.Parse(inputText);

            // Assert
            Assert.Equal("label1", result["Labels"][0]);
            Assert.Equal("label2", result["Labels"][1]);
            Assert.Equal("label3", result["Labels"][2]);
        }

        [Fact]
        public void ParserTest_DescriptionNull()
        {
            // Arrange
            string? inputText = null;

            var parser = new DescriptionParser();

            // Act
            var result = parser.Parse(inputText);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsEmpty());
        }
    }
}
