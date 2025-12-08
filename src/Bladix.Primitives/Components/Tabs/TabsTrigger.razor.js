import { canUseDOM } from '/_content/Bladix.Primitives/dom.js';

/**
 * Sets up keyboard navigation for tabs trigger
 */
export function setupKeyboardNavigation(triggerElement, orientation = 'horizontal') {
    if (!canUseDOM || !triggerElement) {
        return () => {};
    }

    const keys = orientation === 'vertical'
        ? ['ArrowDown', 'ArrowUp', 'Home', 'End']
        : ['ArrowLeft', 'ArrowRight', 'Home', 'End'];

    const handler = (e) => {
        if (keys.includes(e.key)) {
            e.preventDefault();
        }
    };

    triggerElement.addEventListener('keydown', handler, { passive: false });

    return () => {
        triggerElement.removeEventListener('keydown', handler);
    };
}
