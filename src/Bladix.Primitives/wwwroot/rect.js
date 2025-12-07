// Map to store observed elements with their last known rect and .NET reference
const observedElements = new Map();
// RequestAnimationFrame ID for cancellation
let rafId = null;
// Throttle flag to prevent excessive updates
let isProcessing = false;

/**
 * Observes changes to an element's bounding rectangle and notifies .NET when changes occur.
 * @param {HTMLElement} element - The DOM element to observe
 * @param {Object} dotNetRef - Reference to .NET object for invoking callbacks
 * @returns {Object} Cleanup object with DisposeAsync method for C# interop
 */
export function observeElementRect(element, dotNetRef) {
    if (!element || !dotNetRef) {
        throw new Error("Element and dotNetRef are required");
    }

    if (!observedElements.has(element)) {
        // Store the element's current rect and .NET reference
        observedElements.set(element, {
            rect: element.getBoundingClientRect(),
            dotNetRef
        });

        // Start the animation frame loop only when the first element is added
        if (observedElements.size === 1) {
            rafId = requestAnimationFrame(runLoop);
        }
    }

    // Return a cleanup object that matches IJSObjectReference pattern
    let disposed = false;
    
    return {
        // Blazor calls DisposeAsync() which maps to dispose() in JS
        dispose: () => {
            if (disposed) return;
            disposed = true;

            observedElements.delete(element);

            // Stop the animation frame loop when no elements are being observed
            if (observedElements.size === 0 && rafId !== null) {
                cancelAnimationFrame(rafId);
                rafId = null;
                isProcessing = false;
            }
        },
        // Symbol for async disposal (optional but good practice)
        [Symbol.asyncDispose]: async () => {
            if (disposed) return;
            disposed = true;

            observedElements.delete(element);

            if (observedElements.size === 0 && rafId !== null) {
                cancelAnimationFrame(rafId);
                rafId = null;
                isProcessing = false;
            }
        }
    };
}

/**
 * Compares two DOMRect objects for equality
 * @param {DOMRect} a - First rectangle
 * @param {DOMRect} b - Second rectangle
 * @returns {boolean} True if rectangles are equal
 */
function rectEquals(a, b) {
    return a.width === b.width &&
        a.height === b.height &&
        a.top === b.top &&
        a.right === b.right &&
        a.bottom === b.bottom &&
        a.left === b.left;
}

/**
 * Main loop that checks all observed elements for rect changes.
 * Runs continuously via requestAnimationFrame while elements are being observed.
 */
function runLoop() {
    // Prevent concurrent execution
    if (isProcessing) {
        rafId = requestAnimationFrame(runLoop);
        return;
    }

    isProcessing = true;

    try {
        observedElements.forEach((data, element) => {
            // Check if element is still in the DOM
            if (!element.isConnected) {
                observedElements.delete(element);
                return;
            }

            const newRect = element.getBoundingClientRect();

            if (!rectEquals(data.rect, newRect)) {
                data.rect = newRect;

                // Use invokeMethodAsync with error handling
                data.dotNetRef.invokeMethodAsync("OnRectChanged", {
                    width: newRect.width,
                    height: newRect.height,
                    top: newRect.top,
                    right: newRect.right,
                    bottom: newRect.bottom,
                    left: newRect.left
                }).catch(error => {
                    console.error("Error invoking OnRectChanged:", error);
                    // Remove the element if the callback fails
                    observedElements.delete(element);
                });
            }
        });
    } finally {
        isProcessing = false;
    }

    // Schedule the next frame if there are still elements to observe
    if (observedElements.size > 0) {
        rafId = requestAnimationFrame(runLoop);
    } else {
        // Cleanup when no elements remain
        rafId = null;
    }
}
