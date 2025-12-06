const observedElements = new Map();
let rafId = null;

export function observeElementRect(element, dotNetRef) {
    if (!observedElements.has(element)) {
        observedElements.set(element, {
            rect: element.getBoundingClientRect(),
            dotNetRef
        });

        if (observedElements.size === 1) {
            rafId = requestAnimationFrame(runLoop);
        }
    }

    return () => {
        observedElements.delete(element);

        if (observedElements.size === 0 && rafId !== null) {
            cancelAnimationFrame(rafId);
            rafId = null;
        }
    };
}

function rectEquals(a, b) {
    return a.width === b.width &&
        a.height === b.height &&
        a.top === b.top &&
        a.right === b.right &&
        a.bottom === b.bottom &&
        a.left === b.left;
}

function runLoop() {
    observedElements.forEach((data, element) => {
        const newRect = element.getBoundingClientRect();

        if (!rectEquals(data.rect, newRect)) {
            data.rect = newRect;

            data.dotNetRef.invokeMethodAsync("OnRectChanged", {
                width: newRect.width,
                height: newRect.height,
                top: newRect.top,
                right: newRect.right,
                bottom: newRect.bottom,
                left: newRect.left
            });
        }
    });

    rafId = requestAnimationFrame(runLoop);
}
