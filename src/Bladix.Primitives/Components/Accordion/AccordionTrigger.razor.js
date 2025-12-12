// return an object with a dispose() method so C# can call invokeVoidAsync("dispose")
import { canUseDOM } from '/_content/Bladix.Primitives/dom.js';

/**
 * Sets up keyboard navigation for accordion trigger
 */
export function setupKeyboardNavigation(triggerElement, orientation = 'vertical') {
    if (!canUseDOM || !triggerElement) {
        return { dispose: () => {} };
    }

    // Feature-detect passive option support so we can pass { passive: false } safely
    let supportsPassive = false;
    try {
        const opts = Object.defineProperty({}, 'passive', {
            get() { supportsPassive = true; }
        });
        // use a no-op function instead of null to avoid exceptions in strict browsers
        window.addEventListener('__bladix_test__', () => {}, opts);
        window.removeEventListener('__bladix_test__', () => {}, opts);
    } catch (err) {
        supportsPassive = false;
    }

    const listenerOptions = supportsPassive ? { passive: false } : false;

    const orient = (typeof orientation === 'string' ? orientation.toLowerCase() : 'vertical');
    const keys = orient === 'vertical'
        ? ['ArrowDown', 'ArrowUp', 'Home', 'End']
        : ['ArrowLeft', 'ArrowRight', 'Home', 'End'];

    // Use a private, unlikely-to-collide property on the element so we can clean up reliably
    const HANDLER_PROP = '__bladixAccordionKeydownHandler_v1__';

    // If a previous handler was attached for this element, remove it first (idempotency)
    try {
        const prev = triggerElement[HANDLER_PROP];
        if (prev && prev.handler) {
            triggerElement.removeEventListener('keydown', prev.handler, prev.options || false);
        }
    } catch (err) {
        // swallow: best-effort cleanup
    }

    const handler = (e) => {
        // guard: if event already prevented, bail out
        if (e.defaultPrevented) return;

        // If the actual focused/target element is editable (input/textarea/select or contentEditable),
        // do not prevent default — allow native caret behavior.
        try {
            const tgt = e.target;
            const tag = tgt && tgt.tagName ? String(tgt.tagName).toLowerCase() : null;
            if (tag === 'input' || tag === 'textarea' || tag === 'select') return;
            if (tgt && tgt.isContentEditable) return;
        } catch (err) {
            // ignore and continue; defensive only
        }

        // Only react to the intended keys; preventDefault to avoid page scroll on arrows/Home/End
        if (keys.includes(e.key)) {
            // ensure preventDefault only when appropriate
            if (e.cancelable !== false) {
                try { e.preventDefault(); } catch (err) { /* ignore */ }
            }
        }
    };

    // store handler and options so removal uses same values
    try {
        triggerElement.addEventListener('keydown', handler, listenerOptions);
        triggerElement[HANDLER_PROP] = { handler, options: listenerOptions };
    } catch (err) {
        // Could fail if element is not a real DOM node — return a no-op disposer
        return { dispose: () => {} };
    }

    return {
        dispose: () => {
            try {
                const stored = triggerElement[HANDLER_PROP];
                if (stored && stored.handler) {
                    triggerElement.removeEventListener('keydown', stored.handler, stored.options || false);
                } else {
                    // fallback: try removing the local handler reference
                    triggerElement.removeEventListener('keydown', handler);
                }
            } catch (err) {
                // swallow: element may have been removed or already GC'd
            } finally {
                try { delete triggerElement[HANDLER_PROP]; } catch (e) { /* ignore */ }
            }
        }
    };
}
