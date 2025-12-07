using Bladix.Primitives.Core.Rect;

namespace Bladix.Primitives.Core.Tests.Rect
{
    public class DOMRectTests
    {
        [Fact]
        public void DOMRect_CreatesRecordWithAllProperties()
        {
            // Arrange & Act
            var domRect = new DOMRect(100, 200, 10, 110, 210, 10);

            // Assert
            Assert.Equal(100, domRect.Width);
            Assert.Equal(200, domRect.Height);
            Assert.Equal(10, domRect.Top);
            Assert.Equal(110, domRect.Right);
            Assert.Equal(210, domRect.Bottom);
            Assert.Equal(10, domRect.Left);
        }

        [Fact]
        public void DOMRect_SupportsEquality()
        {
            // Arrange
            var rect1 = new DOMRect(100, 200, 10, 110, 210, 10);
            var rect2 = new DOMRect(100, 200, 10, 110, 210, 10);

            // Act & Assert
            Assert.Equal(rect1, rect2);
        }

        [Fact]
        public void DOMRect_NotEqual_WhenPropertiesDiffer()
        {
            // Arrange
            var rect1 = new DOMRect(100, 200, 10, 110, 210, 10);
            var rect2 = new DOMRect(150, 200, 10, 110, 210, 10);

            // Act & Assert
            Assert.NotEqual(rect1, rect2);
        }

        [Fact]
        public void DOMRect_HasCorrectHashCode()
        {
            // Arrange
            var rect1 = new DOMRect(100, 200, 10, 110, 210, 10);
            var rect2 = new DOMRect(100, 200, 10, 110, 210, 10);

            // Act & Assert
            Assert.Equal(rect1.GetHashCode(), rect2.GetHashCode());
        }

        [Fact]
        public void DOMRect_WithZeroValues()
        {
            // Arrange & Act
            var domRect = new DOMRect(0, 0, 0, 0, 0, 0);

            // Assert
            Assert.Equal(0, domRect.Width);
            Assert.Equal(0, domRect.Height);
            Assert.Equal(0, domRect.Top);
            Assert.Equal(0, domRect.Right);
            Assert.Equal(0, domRect.Bottom);
            Assert.Equal(0, domRect.Left);
        }

        [Fact]
        public void DOMRect_WithNegativeValues()
        {
            // Arrange & Act
            var domRect = new DOMRect(-100, -200, -10, -110, -210, -10);

            // Assert
            Assert.Equal(-100, domRect.Width);
            Assert.Equal(-200, domRect.Height);
            Assert.Equal(-10, domRect.Top);
            Assert.Equal(-110, domRect.Right);
            Assert.Equal(-210, domRect.Bottom);
            Assert.Equal(-10, domRect.Left);
        }
    }
}