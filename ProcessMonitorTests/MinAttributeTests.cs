using Xunit;
using ProcessMonitor;

namespace ProcessMonitorTests
{
    public class MinAttributeTests
    {
        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(10, true)]
        [InlineData(-1, false)]
        [InlineData(-10, false)]
        public void CheckIntValidation(int value, bool expected)
        {
            var attribute = new MinAttribute(0);
            Assert.Equal(expected, attribute.IsValid(value));
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(0.1, true)]
        [InlineData(10.0, true)]
        [InlineData(-0.1, false)]
        [InlineData(-10.0, false)]
        public void CheckDoubleValidation(double value, bool expected)
        {
            var attribute = new MinAttribute(0);
            Assert.Equal(expected, attribute.IsValid(value));
        }
    }
}
