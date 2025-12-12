// Map to store observed elements with their last known rect and .NET reference
const observedElements = new Map();

// Single ResizeObserver used for size changes
let resizeObserver = null;

// Window events that can change element position
let needsPositionCheck = false;

// Helper: create ResizeObserver lazily
function ensureResizeObserver() {
    if (resizeObserver) return;
    if (typeof ResizeObserver === "undefined") return;

    resizeObserver = new ResizeObserver(entries => {
        for (const entry of entries) {
            const element = entry.target;
            const data = observedElements.get(element);
            if (!data) continue;

            // Use contentRect for size, but still compute full bounding rect
            const newRect = element.getBoundingClientRect();
            if (!rectEquals(data.rect, newRect)) {
                data.rect = newRect;
                safeInvokeOnRectChanged(data.dotNetRef, newRect, element);
            }
        }
    });
}

// Compare two DOMRect-like objects
function rectEquals(a, b) {
    return a &&
        b &&
        a.width === b.width &&
        a.height === b.height &&
        a.top === b.top &&
        a.right === b.right &&
        a.bottom === b.bottom &&
        a.left === b.left;
}

function safeInvokeOnRectChanged(dotNetRef, rect, element) {
    if (!dotNetRef) return;
    dotNetRef.invokeMethodAsync("OnRectChanged", {
        width: rect.width,
        height: rect.height,
        top: rect.top,
        right: rect.right,
        bottom: rect.bottom,
        left: rect.left
    }).catch(error => {
        console.error("Error invoking OnRectChanged:", error);
        // If the dotnet target is gone, remove the element from observation.
        observedElements.delete(element);
        try {
            if (resizeObserver && element) resizeObserver.unobserve(element);
        } catch { /* swallow */ }
    });
}

// Position check: on scroll/resize/DOM mutations we re-check positions (cheap)
function checkPositions() {
    if (observedElements.size === 0) return;
    observedElements.forEach((data, element) => {
        if (!element.isConnected) {
            observedElements.delete(element);
            try { if (resizeObserver) resizeObserver.unobserve(element); } catch {}
            return;
        }

        const newRect = element.getBoundingClientRect();
        if (!rectEquals(data.rect, newRect)) {
            data.rect = newRect;
            safeInvokeOnRectChanged(data.dotNetRef, newRect, element);
        }
    });
    needsPositionCheck = false;
}

// Listen to global events that may change layout/position
if (typeof window !== "undefined") {
    window.addEventListener("scroll", () => { if (!needsPositionCheck) { needsPositionCheck = true; requestAnimationFrame(checkPositions); } }, { passive: true });
    window.addEventListener("resize", () => { if (!needsPositionCheck) { needsPositionCheck = true; requestAnimationFrame(checkPositions); } }, { passive: true });

    // MutationObserver to catch DOM changes that could shift elements (attributes, subtree)
    try {
        const mo = new MutationObserver(() => { if (!needsPositionCheck) { needsPositionCheck = true; requestAnimationFrame(checkPositions); } });
        mo.observe(document, { attributes: true, childList: true, subtree: true });
    } catch (e) {
        // If MutationObserver isn't available, we still have scroll/resize as fallbacks
        console.warn("MutationObserver not available:", e);
    }
}

/**
 * Observes changes to an element's bounding rectangle and notifies .NET when changes occur.
 * Uses ResizeObserver for size changes and window events for position changes.
 * @param {HTMLElement} element - The DOM element to observe
 * @param {Object} dotNetRef - Reference to .NET object for invoking callbacks
 * @returns {Object} Cleanup object with dispose() and async dispose support
 */
export function observeElementRect(element, dotNetRef) {
    if (!element || !dotNetRef) {
        throw new Error("Element and dotNetRef are required");
    }

    // Initialize ResizeObserver if supported
    ensureResizeObserver();

    if (!observedElements.has(element)) {
        const initialRect = element.getBoundingClientRect();
        observedElements.set(element, {
            rect: initialRect,
            dotNetRef
        });

        // Start observing size if ResizeObserver exists
        try {
            if (resizeObserver) resizeObserver.observe(element);
        } catch (e) {
            console.warn("ResizeObserver.observe failed:", e);
        }

        // Notify initial rect to .NET
        safeInvokeOnRectChanged(dotNetRef, initialRect, element);
    }

    let disposed = false;

    return {
        dispose: () => {
            if (disposed) return;
            disposed = true;
            observedElements.delete(element);
            try { if (resizeObserver) resizeObserver.unobserve(element); } catch {}
        },
        [Symbol.asyncDispose]: async () => {
            if (disposed) return;
            disposed = true;
            observedElements.delete(element);
            try { if (resizeObserver) resizeObserver.unobserve(element); } catch {}
        }
    };
}
