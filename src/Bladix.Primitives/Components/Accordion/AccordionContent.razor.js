// Small helper to animate accordion content using measured height.
// Exports: setOpen(element), setClosed(element), updateMeasuredHeight(element, height)

import { canUseDOM } from '/_content/Bladix.Primitives/dom.js';

const HANDLER_PROP = '__bladixAccordionAnimate_v3__';

function safeRemoveTransitionEnd(el, handler) {
    try { el.removeEventListener('transitionend', handler); } catch {}
}

function clearAnimationState(el) {
    try {
        const stored = el[HANDLER_PROP];
        if (stored) {
            if (stored.onEnd) safeRemoveTransitionEnd(el, stored.onEnd);
            if (stored.ro) {
                try { stored.ro.disconnect(); } catch {}
            }
            try { delete el[HANDLER_PROP]; } catch {}
        }
    } catch (e) { /* swallow */ }
}

function ensureVisibleForMeasurement(el) {
    const cs = window.getComputedStyle(el);
    const wasDisplayNone = cs.display === 'none';
    if (wasDisplayNone) {
        el.style.display = 'block';
    }
    return wasDisplayNone;
}

export function setOpen(element) {
    if (!canUseDOM || !element) return;
    try {
        clearAnimationState(element);

        const wasDisplayNone = ensureVisibleForMeasurement(element);

        element.style.boxSizing = element.style.boxSizing || 'border-box';
        element.style.overflow = 'hidden';
        element.style.willChange = 'height, opacity';

        element.style.height = '0px';
        element.style.opacity = '0';
        element.getBoundingClientRect();

        let fullHeight = element.scrollHeight;
        element.style.transition = 'height 220ms ease, opacity 180ms ease';

        let ro = null;
        try {
            if (typeof ResizeObserver !== 'undefined') {
                ro = new ResizeObserver(() => {
                    try {
                        const s = element[HANDLER_PROP];
                        if (s && s.animating) {
                            const h = element.scrollHeight;
                            if (Math.abs((parseFloat(element.style.height) || 0) - h) > 1) {
                                element.style.height = h + 'px';
                            }
                        }
                    } catch (err) { /* swallow */ }
                });
                ro.observe(element);
            }
        } catch (err) {
            ro = null;
        }

        requestAnimationFrame(() => {
            element.style.height = fullHeight + 'px';
            element.style.opacity = '1';
        });

        const onEnd = (ev) => {
            if (ev.propertyName !== 'height') return;
            try {
                element.style.height = 'auto';
                element.style.transition = '';
                element.style.willChange = '';
                if (wasDisplayNone) element.style.display = '';
            } catch (e) { /* swallow */ }

            safeRemoveTransitionEnd(element, onEnd);
            try {
                const stored = element[HANDLER_PROP];
                if (stored && stored.ro) {
                    try { stored.ro.disconnect(); } catch {}
                }
            } catch {}
            try { delete element[HANDLER_PROP]; } catch {}
        };

        element.addEventListener('transitionend', onEnd);
        element[HANDLER_PROP] = { onEnd, ro, animating: true };

        setTimeout(() => {
            try {
                const s = element[HANDLER_PROP];
                if (s) s.animating = false;
            } catch {}
        }, 300);
    } catch (err) {
        // best-effort; swallow
    }
}

export function setClosed(element) {
    if (!canUseDOM || !element) return;
    try {
        clearAnimationState(element);

        const cs = window.getComputedStyle(element);
        const wasDisplayNone = cs.display === 'none';
        if (wasDisplayNone) {
            element.style.display = 'block';
        }

        const currentHeight = element.getBoundingClientRect().height;

        element.style.boxSizing = element.style.boxSizing || 'border-box';
        element.style.overflow = 'hidden';
        element.style.willChange = 'height, opacity';
        element.style.height = currentHeight + 'px';
        element.style.opacity = '1';
        element.getBoundingClientRect();

        element.style.transition = 'height 220ms ease, opacity 180ms ease';

        let ro = null;
        try {
            if (typeof ResizeObserver !== 'undefined') {
                ro = new ResizeObserver(() => {
                    try {
                        const s = element[HANDLER_PROP];
                        if (s && s.animating) {
                            const h = element.scrollHeight;
                            element.style.height = h + 'px';
                        }
                    } catch (err) { /* swallow */ }
                });
                ro.observe(element);
            }
        } catch (err) {
            ro = null;
        }

        requestAnimationFrame(() => {
            element.style.height = '0px';
            element.style.opacity = '0';
        });

        const onEnd = (ev) => {
            if (ev.propertyName !== 'height') return;
            try {
                element.style.transition = '';
                element.style.willChange = '';
            } catch (e) { /* swallow */ }

            safeRemoveTransitionEnd(element, onEnd);
            try {
                const stored = element[HANDLER_PROP];
                if (stored && stored.ro) {
                    try { stored.ro.disconnect(); } catch {}
                }
            } catch {}
            try { delete element[HANDLER_PROP]; } catch {}
        };

        element.addEventListener('transitionend', onEnd);
        element[HANDLER_PROP] = { onEnd, ro, animating: true };

        setTimeout(() => {
            try {
                const s = element[HANDLER_PROP];
                if (s) s.animating = false;
            } catch {}
        }, 300);
    } catch (err) {
        // swallow
    }
}

export function updateMeasuredHeight(element, height) {
    if (!canUseDOM || !element) return;
    try {
        const stored = element[HANDLER_PROP];
        const parsed = Number(height) || 0;
        // If an animation is running we must update the target inline height
        if (stored && stored.animating) {
            element.style.height = parsed + 'px';
        } else {
            // If element currently has an explicit pixel height (not 'auto'), update it to avoid visual jumps
            const cur = element.style.height;
            if (cur && cur !== 'auto') {
                element.style.height = parsed + 'px';
            }
        }
    } catch (e) {
        // swallow
    }
}
