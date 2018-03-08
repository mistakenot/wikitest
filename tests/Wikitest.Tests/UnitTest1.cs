using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Wikitest.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var entity = new Class1().GetEntity("Q513");
            Assert.Equal("Mount Everest", entity.Labels["en"]);
            Assert.Contains("Chumulangma", entity.Aliases["en"]);
        }

        [Fact]
        public void SearchTest()
        {
            var result = new Class1().Search("San").Result;
            
            Assert.NotEmpty(result);
            Assert.Equal("San", result.First().Title);
            Assert.Equal("https://en.wikipedia.org/wiki/San", result.First().Url);
        }

        [Fact]
        public void SearchIdsTests()
        {
            var result = new Class1().SearchIdAsync("New York City").Result;

            Assert.NotEmpty(result);
            Assert.Equal(new[] {"Q60"}, result);
        }

        [Fact]
        public void GetEntityTest()
        {
            var result = new Class1().EntityFromTitle("New York City").Result.First();

            Assert.Equal("Q60", result.Id);
        }

        [Fact]
        public void TestSearch()
        {
            var result = new Class1().SearchPages("New York").Result;
            Assert.NotEmpty(result);
        }

        [Fact]
        public void EntityFromTermTest()
        {
            var result = new Class1().EntityFromKeyword("New York City").Result;
            Assert.NotEmpty(result);
            Assert.Equal(new string[]{"Q60", "Q328473", "Q7733", "Q13361030", "Q283207"}, result.Select(r => r.Id).ToArray());
            Assert.NotEmpty(result.First().Aliases);
            Assert.NotEmpty(result.First().Claims);
            Assert.NotNull(result.First().Claims.FirstOrDefault(c => c.MainSnak.PropertyId == "P31"));
            File.WriteAllText("q60", result.First().Claims.FirstOrDefault(c => c.MainSnak.PropertyId == "P31").MainSnak.RawDataValue.ToString());
        }
    }
}
