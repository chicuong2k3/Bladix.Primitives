
import { canUseDOM } from '/_content/Bladix.Primitives/dom.js';

/**
 * Sets up keyboard navigation for accordion trigger
 */
export function setupKeyboardNavigation(triggerElement, orientation = 'vertical') {
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
