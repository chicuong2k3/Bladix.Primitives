using Bladix.Primitives.Core.Primitive;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Threading.Tasks;

namespace Bladix.Primitives.Core.Tests.Primitive
{
    public class BladixDomTests : IAsyncDisposable
    {
        private readonly Mock<IJSRuntime> _jsRuntimeMock;
        private readonly Mock<IJSObjectReference> _jsModuleMock;

        public BladixDomTests()
        {
            _jsRuntimeMock = new Mock<IJSRuntime>();
            _jsModuleMock = new Mock<IJSObjectReference>();

            _jsRuntimeMock
                .Setup(x => x.InvokeAsync<IJSObjectReference>("import", It.IsAny<object[]>()))
                .ReturnsAsync(_jsModuleMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenJSRuntimeIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BladixDom(null!));
        }

        [Fact]
        public void Constructor_InitializesWithJSRuntime()
        {
            // Act
            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Assert
            Assert.NotNull(bladixDom);
        }

        [Fact]
        public async Task CanUseDOM_InvokesJSInterop()
        {
            // Arrange
            _jsModuleMock
                .Setup(x => x.InvokeAsync<bool>("getCanUseDOM", It.IsAny<object[]>()))
                .ReturnsAsync(true);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.CanUseDOM();

            // Assert
            Assert.True(result);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<bool>("getCanUseDOM", It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task CanUseDOM_ReturnsFalse_WhenDOMNotAvailable()
        {
            // Arrange
            _jsModuleMock
                .Setup(x => x.InvokeAsync<bool>("getCanUseDOM", It.IsAny<object[]>()))
                .ReturnsAsync(false);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.CanUseDOM();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanUseDOM_ThrowsObjectDisposedException_WhenDisposed()
        {
            // Arrange
            var bladixDom = new BladixDom(_jsRuntimeMock.Object);
            await bladixDom.DisposeAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => bladixDom.CanUseDOM().AsTask());
        }

        [Fact]
        public async Task GetActiveElement_InvokesJSInterop()
        {
            // Arrange
            var elementRef = new ElementReference("test-element");

            _jsModuleMock
                .Setup(x => x.InvokeAsync<ElementReference?>("getActiveElement", It.IsAny<object[]>()))
                .ReturnsAsync(elementRef);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.GetActiveElement();

            // Assert
            Assert.NotNull(result);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<ElementReference?>("getActiveElement", It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task GetActiveElement_WithActiveDescendantTrue_PassesParameterCorrectly()
        {
            // Arrange
            var elementRef = new ElementReference("test-element");

            _jsModuleMock
                .Setup(x => x.InvokeAsync<ElementReference?>("getActiveElement", It.IsAny<object[]>()))
                .ReturnsAsync(elementRef);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.GetActiveElement(activeDescendant: true);

            // Assert
            Assert.NotNull(result);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<ElementReference?>(
                    "getActiveElement",
                    It.Is<object[]>(arr => arr.Length == 2 && arr[1].Equals(true))),
                Times.Once);
        }

        [Fact]
        public async Task GetActiveElement_ReturnsNull_WhenNoActiveElement()
        {
            // Arrange
            _jsModuleMock
                .Setup(x => x.InvokeAsync<ElementReference?>("getActiveElement", It.IsAny<object[]>()))
                .ReturnsAsync((ElementReference?)null);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.GetActiveElement();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetActiveElement_ThrowsObjectDisposedException_WhenDisposed()
        {
            // Arrange
            var bladixDom = new BladixDom(_jsRuntimeMock.Object);
            await bladixDom.DisposeAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => bladixDom.GetActiveElement().AsTask());
        }

        [Fact]
        public async Task IsFrame_InvokesJSInterop()
        {
            // Arrange
            var elementRef = new ElementReference("iframe-element");
            _jsModuleMock
                .Setup(x => x.InvokeAsync<bool>("isFrame", It.IsAny<object[]>()))
                .ReturnsAsync(true);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.IsFrame(elementRef);

            // Assert
            Assert.True(result);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<bool>("isFrame", It.Is<object[]>(arr => arr.Length == 1)),
                Times.Once);
        }

        [Fact]
        public async Task Contains_InvokesJSInterop()
        {
            // Arrange
            var parent = new ElementReference("parent");
            var child = new ElementReference("child");
            _jsModuleMock
                .Setup(x => x.InvokeAsync<bool>("contains", It.IsAny<object[]>()))
                .ReturnsAsync(true);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.Contains(parent, child);

            // Assert
            Assert.True(result);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<bool>("contains", It.Is<object[]>(arr => arr.Length == 2)),
                Times.Once);
        }

        [Fact]
        public async Task GetTabbableElements_InvokesJSInterop()
        {
            // Arrange
            var container = new ElementReference("container");
            var elements = new[] { new ElementReference("elem1"), new ElementReference("elem2") };
            _jsModuleMock
                .Setup(x => x.InvokeAsync<ElementReference[]>("getTabbableElements", It.IsAny<object[]>()))
                .ReturnsAsync(elements);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.GetTabbableElements(container);

            // Assert
            Assert.Equal(2, result.Length);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<ElementReference[]>("getTabbableElements", It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task Focus_InvokesJSInterop()
        {
            // Arrange
            var element = new ElementReference("element");
            
            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act - Should not throw
            await bladixDom.Focus(element, preventScroll: true);

            // Assert - Verify that module.InvokeAsync was called with "focus"
            _jsModuleMock.Verify(
                x => x.InvokeAsync<It.IsAnyType>("focus", It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task Focus_ThrowsObjectDisposedException_WhenDisposed()
        {
            // Arrange
            var bladixDom = new BladixDom(_jsRuntimeMock.Object);
            var element = new ElementReference("element");
            await bladixDom.DisposeAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => bladixDom.Focus(element).AsTask());
        }

        [Fact]
        public async Task GetOwnerWindow_InvokesJSInterop()
        {
            // Arrange
            var element = new ElementReference("element");
            var windowMock = new Mock<IJSObjectReference>();
            _jsModuleMock
                .Setup(x => x.InvokeAsync<IJSObjectReference?>("getOwnerWindow", It.IsAny<object[]>()))
                .ReturnsAsync(windowMock.Object);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.GetOwnerWindow(element);

            // Assert
            Assert.NotNull(result);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<IJSObjectReference?>("getOwnerWindow", It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task GetOwnerDocument_InvokesJSInterop()
        {
            // Arrange
            var element = new ElementReference("element");
            var documentMock = new Mock<IJSObjectReference>();
            _jsModuleMock
                .Setup(x => x.InvokeAsync<IJSObjectReference?>("getOwnerDocument", It.IsAny<object[]>()))
                .ReturnsAsync(documentMock.Object);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.GetOwnerDocument(element);

            // Assert
            Assert.NotNull(result);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<IJSObjectReference?>("getOwnerDocument", It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task GetComputedStyle_InvokesJSInterop()
        {
            // Arrange
            var element = new ElementReference("element");
            var styleMock = new Mock<IJSObjectReference>();
            _jsModuleMock
                .Setup(x => x.InvokeAsync<IJSObjectReference?>("getComputedStyle", It.IsAny<object[]>()))
                .ReturnsAsync(styleMock.Object);

            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            var result = await bladixDom.GetComputedStyle(element);

            // Assert
            Assert.NotNull(result);
            _jsModuleMock.Verify(
                x => x.InvokeAsync<IJSObjectReference?>("getComputedStyle", It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_DisposesModule()
        {
            // Arrange
            var bladixDom = new BladixDom(_jsRuntimeMock.Object);
            await bladixDom.CanUseDOM(); // Force module load

            // Act
            await bladixDom.DisposeAsync();

            // Assert
            _jsModuleMock.Verify(x => x.DisposeAsync(), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_CanBeCalledMultipleTimes()
        {
            // Arrange
            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act & Assert (should not throw)
            await bladixDom.DisposeAsync();
            await bladixDom.DisposeAsync();
        }

        [Fact]
        public async Task DisposeAsync_DoesNotDisposeModule_WhenModuleNotLoaded()
        {
            // Arrange
            var bladixDom = new BladixDom(_jsRuntimeMock.Object);

            // Act
            await bladixDom.DisposeAsync();

            // Assert
            _jsModuleMock.Verify(x => x.DisposeAsync(), Times.Never);
        }

        public async ValueTask DisposeAsync()
        {
            if (_jsModuleMock?.Object != null)
            {
                await _jsModuleMock.Object.DisposeAsync();
            }
        }
    }
}
