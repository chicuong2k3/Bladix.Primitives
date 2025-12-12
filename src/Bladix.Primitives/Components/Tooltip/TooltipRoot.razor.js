/Tooltip/TooltipRoot.razor.js
/**
 * Timer utilities for TooltipRoot component using JS isolation
 */

/**
 * Sets a timeout and returns the timer ID
 * @param {DotNetObjectReference} dotnetHelper - .NET callback reference
 * @param {number} ms - Delay in milliseconds
 * @returns {number} Timer ID
 */
export function setTimeout(dotnetHelper, ms) {
    const id = window.setTimeout(async () => {
        try {
            await dotnetHelper.invokeMethodAsync('Invoke');
        } catch (error) {
            console.error('TooltipRoot: Error invoking setTimeout callback:', error);
        }
    }, ms);
    return id;
}

/**
 * Clears a timeout
 * @param {number} id - Timer ID to clear
 */
export function clearTimeout(id) {
    if (id !== null && id !== undefined) {
        window.clearTimeout(id);
    }
}

/**
 * Sets an interval and returns the timer ID
 * @param {DotNetObjectReference} dotnetHelper - .NET callback reference
 * @param {number} ms - Interval in milliseconds
 * @returns {number} Timer ID
 */
export function setInterval(dotnetHelper, ms) {
    const id = window.setInterval(async () => {
        try {
            await dotnetHelper.invokeMethodAsync('Invoke');
        } catch (error) {
            console.error('TooltipRoot: Error invoking setInterval callback:', error);
        }
    }, ms);
    return id;
}

/**
 * Clears an interval
 * @param {number} id - Timer ID to clear
 */
export function clearInterval(id) {
    if (id !== null && id !== undefined) {
        window.clearInterval(id);
    }
}
