export const canUseDOM = !!(
    typeof window !== 'undefined' &&
    window.document &&
    window.document.createElement
);

/**
 * Returns whether DOM is available (wrapper for C# interop)
 * @returns {boolean} True if DOM is available
 */
export function getCanUseDOM() {
    return canUseDOM;
}

/**
 * Composes multiple event handlers together
 * @param {Function} originalEventHandler - The original event handler
 * @param {Function} ourEventHandler - The new event handler to compose
 * @param {Object} options - Configuration options
 * @param {boolean} options.checkForDefaultPrevented - Whether to check if default is prevented
 * @returns {Function} Composed event handler
 */
export function composeEventHandlers(originalEventHandler, ourEventHandler, options) {
    const checkForDefaultPrevented = options?.checkForDefaultPrevented ?? true;
    return function handleComposedEvent(event) {
        originalEventHandler?.call(this, event);
        if (checkForDefaultPrevented === false || !event.defaultPrevented) {
            ourEventHandler?.call(this, event);
        }
    };
}

/**
 * Gets the owner window of an element
 * @param {HTMLElement} element - The element to get the owner window from
 * @returns {Window} The owner window
 */
export function getOwnerWindow(element) {
    if (!canUseDOM) {
        throw new Error("Cannot access window outside DOM environment");
    }
    return element?.ownerDocument?.defaultView ?? window;
}

/**
 * Gets the owner document of an element
 * @param {HTMLElement} element - The element to get the owner document from
 * @returns {Document} The owner document
 */
export function getOwnerDocument(element) {
    if (!canUseDOM) {
        throw new Error("Cannot access document outside DOM environment");
    }
    return element?.ownerDocument ?? document;
}

/**
 * Gets the currently active element, with support for iframes and aria-activedescendant
 * @param {HTMLElement} node - The node to get the active element from
 * @param {boolean} activeDescendant - Whether to check for aria-activedescendant
 * @returns {HTMLElement|null} The active element or null
 */
export function getActiveElement(node, activeDescendant = false) {
    const doc = getOwnerDocument(node);
    let { activeElement } = doc;
    
    if (!activeElement?.nodeName) {
        return null;
    }

    // Handle iframes recursively
    if (isFrame(activeElement) && activeElement.contentDocument) {
        return getActiveElement(activeElement.contentDocument.body, activeDescendant);
    }

    // Handle aria-activedescendant
    if (activeDescendant) {
        const descendantId = activeElement.getAttribute("aria-activedescendant");
        if (descendantId) {
            const descendantElement = doc.getElementById(descendantId);
            if (descendantElement) {
                return descendantElement;
            }
        }
    }

    return activeElement;
}

/**
 * Checks if an element is an iframe
 * @param {HTMLElement} element - The element to check
 * @returns {boolean} True if the element is an iframe
 */
export function isFrame(element) {
    return element?.tagName === "IFRAME";
}

/**
 * Checks if an element contains another element
 * @param {HTMLElement} parent - The parent element
 * @param {HTMLElement} child - The child element
 * @returns {boolean} True if parent contains child
 */
export function contains(parent, child) {
    if (!canUseDOM) return false;
    return parent === child || parent?.contains?.(child);
}

/**
 * Gets all tabbable elements within a container
 * @param {HTMLElement} container - The container element
 * @returns {HTMLElement[]} Array of tabbable elements
 */
export function getTabbableElements(container) {
    if (!canUseDOM || !container) return [];
    
    const selector = [
        'a[href]',
        'area[href]',
        'input:not([disabled]):not([type="hidden"]):not([aria-hidden])',
        'select:not([disabled]):not([aria-hidden])',
        'textarea:not([disabled]):not([aria-hidden])',
        'button:not([disabled]):not([aria-hidden])',
        'iframe',
        'object',
        'embed',
        '[contenteditable]',
        '[tabindex]:not([tabindex^="-"])'
    ].join(',');

    return Array.from(container.querySelectorAll(selector));
}

/**
 * Focus an element with optional prevention of scroll
 * @param {HTMLElement} element - The element to focus
 * @param {Object} options - Focus options
 * @param {boolean} options.preventScroll - Whether to prevent scroll on focus
 */
export function focus(element, options = {}) {
    if (!canUseDOM || !element) return;
    element.focus(options);
}

/**
 * Gets the computed style of an element
 * @param {HTMLElement} element - The element
 * @returns {CSSStyleDeclaration} The computed style
 */
export function getComputedStyle(element) {
    if (!canUseDOM) return null;
    return getOwnerWindow(element).getComputedStyle(element);
}

/**
 * Set native checkbox indeterminate property on an input element.
 * This is necessary because `indeterminate` is not an attribute and must be set on the DOM object.
 * @param {HTMLInputElement} element 
 * @param {boolean} value
 */
export function setIndeterminate(element, value) {
    if (!canUseDOM || !element) return;
    try {
        element.indeterminate = !!value;
    } catch (e) {
        // swallow for environments without full DOM during prerendering
        console.warn("setIndeterminate failed:", e);
    }
}

