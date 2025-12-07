using Bladix.Primitives.Core.Rect;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Threading.Tasks;

namespace Bladix.Primitives.Core.Tests.Rect
{
    public class BladixRectTests
    {
        [Fact]
        public void Constructor_InitializesWithJSRuntime()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();

            // Act
            var rect = new BladixRect(jsRuntimeMock.Object);

            // Assert
            Assert.NotNull(rect);
        }

        [Fact]
        public async Task ObserveAsync_ImportsModuleAndInvokesObserveElementRect()
        {
            // Arrange
            var jsModuleMock = new Mock<IJSObjectReference>();
            var cleanupMock = new Mock<IJSObjectReference>();
            var jsRuntimeMock = new Mock<IJSRuntime>();

            jsRuntimeMock
                .Setup(x => x.InvokeAsync<IJSObjectReference>("import", It.IsAny<object[]>()))
                .ReturnsAsync(jsModuleMock.Object);

            jsModuleMock
                .Setup(x => x.InvokeAsync<IJSObjectReference>(
                    "observeElementRect",
                    It.IsAny<object[]>()))
                .ReturnsAsync(cleanupMock.Object);

            var rect = new BladixRect(jsRuntimeMock.Object);
            var elementRef = new ElementReference("test-element");

            // Act
            await rect.ObserveAsync(elementRef);

            // Assert
            jsRuntimeMock.Verify(
                x => x.InvokeAsync<IJSObjectReference>("import", It.Is<object[]>(arr => arr[0].Equals("./_content/Bladix.Primitives/bladix.js"))),
                Times.Once);

            jsModuleMock.Verify(
                x => x.InvokeAsync<IJSObjectReference>(
                    "observeElementRect",
                    It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task ObserveAsync_ThrowsObjectDisposedException_WhenDisposed()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var rect = new BladixRect(jsRuntimeMock.Object);
            await rect.DisposeAsync();

            var elementRef = new ElementReference("test-element");

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => rect.ObserveAsync(elementRef));
        }

        [Fact]
        public void OnRectChanged_InvokesOnChangedEvent()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var rect = new BladixRect(jsRuntimeMock.Object);
            var eventInvoked = false;
            DOMRect? receivedRect = null;

            rect.OnChanged += changedRect =>
            {
                eventInvoked = true;
                receivedRect = changedRect;
            };

            var testRect = new DOMRect(100, 200, 10, 110, 210, 10);

            // Act
            rect.OnRectChanged(testRect);

            // Assert
            Assert.True(eventInvoked);
            Assert.NotNull(receivedRect);
            Assert.Equal(testRect.Width, receivedRect.Width);
            Assert.Equal(testRect.Height, receivedRect.Height);
            Assert.Equal(testRect.Top, receivedRect.Top);
            Assert.Equal(testRect.Right, receivedRect.Right);
            Assert.Equal(testRect.Bottom, receivedRect.Bottom);
            Assert.Equal(testRect.Left, receivedRect.Left);
        }

        [Fact]
        public void OnRectChanged_NoEventWhenNoSubscribers()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var rect = new BladixRect(jsRuntimeMock.Object);
            var testRect = new DOMRect(100, 200, 10, 110, 210, 10);

            // Act & Assert (should not throw)
            rect.OnRectChanged(testRect);
        }

        [Fact]
        public async Task DisposeAsync_CleansUpResources()
        {
            // Arrange
            var cleanupMock = new Mock<IJSObjectReference>();
            var jsModuleMock = new Mock<IJSObjectReference>();
            var jsRuntimeMock = new Mock<IJSRuntime>();

            jsRuntimeMock
                .Setup(x => x.InvokeAsync<IJSObjectReference>("import", It.IsAny<object[]>()))
                .ReturnsAsync(jsModuleMock.Object);

            jsModuleMock
                .Setup(x => x.InvokeAsync<IJSObjectReference>(
                    "observeElementRect",
                    It.IsAny<object[]>()))
                .ReturnsAsync(cleanupMock.Object);

            var rect = new BladixRect(jsRuntimeMock.Object);
            var elementRef = new ElementReference("test-element");
            await rect.ObserveAsync(elementRef);

            // Act
            await rect.DisposeAsync();

            // Assert
            cleanupMock.Verify(x => x.DisposeAsync(), Times.Once);
            jsModuleMock.Verify(x => x.DisposeAsync(), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_CanBeCalledMultipleTimes()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var rect = new BladixRect(jsRuntimeMock.Object);

            // Act & Assert (should not throw)
            await rect.DisposeAsync();
            await rect.DisposeAsync();
        }

        [Fact]
        public async Task ObserveAsync_CachesModuleReference()
        {
            // Arrange
            var jsModuleMock = new Mock<IJSObjectReference>();
            var cleanupMock = new Mock<IJSObjectReference>();
            var jsRuntimeMock = new Mock<IJSRuntime>();

            jsRuntimeMock
                .Setup(x => x.InvokeAsync<IJSObjectReference>("import", It.IsAny<object[]>()))
                .ReturnsAsync(jsModuleMock.Object);

            jsModuleMock
                .Setup(x => x.InvokeAsync<IJSObjectReference>(
                    "observeElementRect",
                    It.IsAny<object[]>()))
                .ReturnsAsync(cleanupMock.Object);

            var rect = new BladixRect(jsRuntimeMock.Object);
            var elementRef = new ElementReference("test-element");

            // Act
            await rect.ObserveAsync(elementRef);
            await rect.ObserveAsync(elementRef);

            // Assert (module should only be imported once)
            jsRuntimeMock.Verify(
                x => x.InvokeAsync<IJSObjectReference>("import", It.IsAny<object[]>()),
                Times.Once);
        }
    }
}
