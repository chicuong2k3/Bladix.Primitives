export const canUseDOM = !!(
    typeof window !== 'undefined' &&
    window.document &&
    window.document.createElement
);

export function composeEventHandlers(originalEventHandler, ourEventHandler, options) {
    const check = options?.checkForDefaultPrevented ?? true;
    return function handle(event) {
        if (originalEventHandler) originalEventHandler(event);
        if (check === false || !event.defaultPrevented) {
            if (ourEventHandler) ourEventHandler(event);
        }
    };
}

export function getOwnerWindow(element) {
    if (!canUseDOM) throw new Error("Cannot access window outside DOM");
    return element?.ownerDocument?.defaultView ?? window;
}

export function getOwnerDocument(element) {
    if (!canUseDOM) throw new Error("Cannot access document outside DOM");
    return element?.ownerDocument ?? document;
}

export function getActiveElement(node, activeDescendant = false) {
    const { activeElement } = getOwnerDocument(node);
    if (!activeElement?.nodeName) return null;

    if (isFrame(activeElement) && activeElement.contentDocument) {
        return getActiveElement(activeElement.contentDocument.body, activeDescendant);
    }

    if (activeDescendant) {
        const id = activeElement.getAttribute("aria-activedescendant");
        if (id) {
            const el = getOwnerDocument(activeElement).getElementById(id);
            if (el) return el;
        }
    }
    return activeElement;
}

export function isFrame(el) {
    return el.tagName === "IFRAME";
}

window.bladix = window.bladix || {};
window.bladix.dom = {
    canUseDOM,
    composeEventHandlers,
    getOwnerWindow,
    getOwnerDocument,
    getActiveElement,
    isFrame
};

window.bladix.timer = {
    setTimeout(fn, ms) {
        const id = setTimeout(fn, ms);
        return id;
    },
    clearTimeout(id) {
        clearTimeout(id);
    },
    setInterval(fn, ms) {
        const id = setInterval(fn, ms);
        return id;
    },
    clearInterval(id) {
        clearInterval(id);
    },
    setImmediate(fn) {
        const id = setImmediate(fn);
        return id;
    },
    clearImmediate(id) {
        clearImmediate(id);
    }
};

