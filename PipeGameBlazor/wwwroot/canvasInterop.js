window.addEventListener = (element, event, callback) => {
    element.addEventListener(event, (e) => {
        callback();
    });
};

window.removeEventListener = (element, event, callback) => {
    element.removeEventListener(event, (e) => {
        callback();
    });
};

window.preventDefault = (e) => {
    e.preventDefault();
};

window.getCanvasSize = (canvasElement) => {
    return {
        width: canvasElement.width,
        height: canvasElement.height
    };
};

window.getBoundingClientRect = (element) => {
    return element.getBoundingClientRect();
};

window.drawGridAndCells = (canvasElement, map, stepX, stepY) => {
    // Implement your drawing logic here
};

window.drawWinMessage = (canvasElement) => {
    // Implement your win message drawing logic here
};