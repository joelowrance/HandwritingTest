var SketchPad2 = /** @class */ (function () {
    function SketchPad2() {
        // Variables to keep track of the mouse position and left-button status 
        this.mouseX = 0;
        this.mouseY = 0;
        this.mouseDown = 0;
        // Variables to keep track of the touch position
        this.touchX = 0;
        this.touchY = 0;
        this.r = 0;
        this.g = 0;
        this.b = 0;
        this.a = 255;
        this.pointerSize = 8;
    }
    // Draws a dot at a specific position on the supplied canvas name
    // Parameters are: A canvas context, the x position, the y position, the size of the dot
    SketchPad2.prototype.drawDot = function (ctx, x, y) {
        // Let's use black by setting RGB values to 0, and 255 alpha (completely opaque)
        //console.profile("begin");
        //let r=0; 
        //let g=0; 
        //let b=0; 
        //let a=255;
        // Select a fill style
        this.ctx.fillStyle = "rgba(" + this.r + "," + this.g + "," + this.b + "," + (this.a / 255) + ")";
        // Draw a filled circle
        ctx.beginPath();
        ctx.arc(x, y, this.pointerSize, 0, Math.PI * 2, true);
        ctx.closePath();
        ctx.fill();
    };
    Object.defineProperty(SketchPad2.prototype, "dotSize", {
        get: function () {
            return this.pointerSize;
        },
        set: function (val) {
            console.log("chaging pointer size to " + val);
            this.pointerSize = val;
        },
        enumerable: true,
        configurable: true
    });
    // Clear the canvas context using the canvas width and height
    SketchPad2.prototype.clearCanvas = function () {
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
    };
    // Keep track of the mouse button being pressed and draw a dot at current location
    SketchPad2.prototype.sketchpadMouseDown = function () {
        this.mouseDown = 1;
        this.drawDot(this.ctx, this.mouseX, this.mouseY);
    };
    // Keep track of the mouse button being released
    SketchPad2.prototype.sketchpadMouseUp = function () {
        this.mouseDown = 0;
    };
    // Keep track of the mouse position and draw a dot if mouse button is currently pressed
    SketchPad2.prototype.sketchpadMouseMove = function (e) {
        // Update the mouse co-ordinates when moved
        this.getMousePos(e);
        // Draw a dot if the mouse button is currently being pressed
        if (this.mouseDown === 1) {
            this.drawDot(this.ctx, this.mouseX, this.mouseY);
        }
    };
    // Get the current mouse position relative to the top-left of the canvas
    SketchPad2.prototype.getMousePos = function (e) {
        if (!e)
            return;
        //e = event;
        if (e.offsetX) {
            this.mouseX = e.offsetX;
            this.mouseY = e.offsetY;
        }
        else if (e.layerX) {
            this.mouseX = e.layerX;
            this.mouseY = e.layerY;
        }
    };
    // Draw something when a touch start is detected
    SketchPad2.prototype.sketchpadTouchStart = function () {
        // Update the touch co-ordinates
        this.getTouchPos(null);
        this.drawDot(this.ctx, this.touchX, this.touchY);
        // Prevents an additional mousedown event being triggered
        event.preventDefault();
    };
    // Draw something and prevent the default scrolling when touch movement is detected
    SketchPad2.prototype.sketchpadTouchMove = function (e) {
        // Update the touch co-ordinates
        this.getTouchPos(e);
        // During a touchmove event, unlike a mousemove event, we don't need to check if the touch is engaged, since there will always be contact with the screen by definition.
        this.drawDot(this.ctx, this.touchX, this.touchY);
        // Prevent a scrolling action as a result of this touchmove triggering.
        event.preventDefault();
    };
    // Get the touch position relative to the top-left of the canvas
    // When we get the raw values of pageX and pageY below, they take into account the scrolling on the page
    // but not the position relative to our target div. We'll adjust them using "target.offsetLeft" and
    // "target.offsetTop" to get the correct values in relation to the top left of the canvas.
    SketchPad2.prototype.getTouchPos = function (e) {
        if (!e)
            e = event;
        if (e.touches) {
            if (e.touches.length === 1) { // Only deal with one finger
                var touch = e.touches[0]; // Get the information for finger #1
                this.touchX = touch.pageX - touch.target.offsetLeft;
                this.touchY = touch.pageY - touch.target.offsetTop;
            }
        }
    };
    // Set-up the canvas and add our event handlers after the page has loaded
    SketchPad2.prototype.init = function () {
        var _this = this;
        // Get the specific canvas element from the HTML document
        this.canvas = document.getElementById('sketchpad');
        // If the browser supports the canvas tag, get the 2d drawing context for this canvas
        if (this.canvas.getContext)
            this.ctx = this.canvas.getContext('2d');
        // Check that we have a valid context to draw on/with before adding event handlers
        if (this.ctx) {
            // React to mouse events on the canvas, and mouseup on the entire document
            //this.canvas.addEventListener('mousedown', this.sketchpad_mouseDown, false);
            //this.canvas.addEventListener('mousemove', this.sketchpad_mouseMove, false);
            //window.addEventListener('mouseup', this.sketchpad_mouseUp, false);
            this.canvas.addEventListener('mousedown', function (e) { return _this.sketchpadMouseDown(); }, false);
            this.canvas.addEventListener('mousemove', function (e) { return _this.sketchpadMouseMove(e); }, false);
            window.addEventListener('mouseup', function (e) { return _this.sketchpadMouseUp(); }, false);
            // React to touch events on the canvas
            //this.canvas.addEventListener('touchstart', this.sketchpad_touchStart, false);
            //this.canvas.addEventListener('touchmove', this.sketchpad_touchMove, false);
            this.canvas.addEventListener('touchstart', function (e) { return _this.sketchpadTouchStart(); }, false);
            this.canvas.addEventListener('touchmove', function (e) { return _this.sketchpadTouchMove(e); }, false);
            //this.canvas.addEventListener("mouseout", (e) => {
            //    this.findxy("out", e);
            //}, false);
        }
    };
    return SketchPad2;
}());
//# sourceMappingURL=Drawing2.js.map