using ProcessMonitor;
using Xunit;

namespace ProcessMonitorTests
{
    public class IntegerAttributeTests
    {
        [Theory]
        [InlineData("1", true)]
        [InlineData("-1", true)]
        [InlineData("0", true)]
        [InlineData("01", true)]
        [InlineData("0000", true)]
        [InlineData("11", true)]
        [InlineData("-11", true)]
        [InlineData("", false)]
        [InlineData("1.0", false)]
        [InlineData("1.1", false)]
        [InlineData("a", false)]
        [InlineData("a1", false)]
        [InlineData("1a", false)]
        [InlineData("999999999999999999999999999999", false)]
        public void CheckValidation(string value, bool expected)
        {
            var attribute = new IntegerAttribute();
            Assert.Equal(expected, attribute.IsValid(value));
        }
    }
}
