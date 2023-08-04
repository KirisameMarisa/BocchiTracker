using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ProcessLinkQuery;
using BocchiTracker.ProcessLinkQuery.Queries;

namespace BocchiTracker.BocchiTracker.UnitTests.ProcessLinkQuery
{
    public class VariableDumpTests
    {
        [Fact]
        public void VariableDumpTest()
        {
            VariableDump variableDump = new VariableDump("Query.schema.json");

            Assert.True(variableDump.ClassAndPropertyNames.ContainsKey("PlayerPosition"));
            Assert.Equal("x",           variableDump.ClassAndPropertyNames["PlayerPosition"][0]);
            Assert.Equal("y",           variableDump.ClassAndPropertyNames["PlayerPosition"][1]);
            Assert.Equal("z",           variableDump.ClassAndPropertyNames["PlayerPosition"][2]);
            Assert.Equal("stage",       variableDump.ClassAndPropertyNames["PlayerPosition"][3]);

            Assert.True(variableDump.ClassAndPropertyNames.ContainsKey("ScreenshotData"));
            Assert.Equal("width",       variableDump.ClassAndPropertyNames["ScreenshotData"][0]);
            Assert.Equal("height",      variableDump.ClassAndPropertyNames["ScreenshotData"][1]);
            Assert.Equal("data",        variableDump.ClassAndPropertyNames["ScreenshotData"][2]);

            Assert.True(variableDump.ClassAndPropertyNames.ContainsKey("AppBasicInfo"));
            Assert.Equal("pid",         variableDump.ClassAndPropertyNames["AppBasicInfo"][0]);
            Assert.Equal("app_name",    variableDump.ClassAndPropertyNames["AppBasicInfo"][1]);
            Assert.Equal("args",        variableDump.ClassAndPropertyNames["AppBasicInfo"][2]);
            Assert.Equal("platform",    variableDump.ClassAndPropertyNames["AppBasicInfo"][3]);

            Assert.True(variableDump.ClassAndPropertyNames.ContainsKey("RequestQuery"));
            Assert.Equal("query_id",    variableDump.ClassAndPropertyNames["RequestQuery"][0]);
        }
    }
}
