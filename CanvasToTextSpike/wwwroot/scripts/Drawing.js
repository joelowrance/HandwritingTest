var Drawing = /** @class */ (function () {
    function Drawing(canvas) {
        this.flag = false;
        this.prevX = 0;
        this.currX = 0;
        this.prevY = 0.01;
        this.currY = 0;
        this.dot_flag = false;
        this.x = "black";
        this.y = 2;
        console.log({ canvas: canvas });
        this.canvas = canvas;
        this.init();
    }
    Drawing.prototype.init = function () {
        var _this = this;
        //this.canvas = document.getElementById("can") as HTMLCanvasElement;
        this.ctx = this.canvas.getContext("2d");
        this.w = this.canvas.width;
        this.h = this.canvas.height;
        this.canvas.addEventListener("mousemove", function (e) {
            _this.findxy("move", e);
        }, false);
        this.canvas.addEventListener("mousedown", function (e) {
            _this.findxy("down", e);
        }, false);
        this.canvas.addEventListener("mouseup", function (e) {
            _this.findxy("up", e);
        }, false);
        this.canvas.addEventListener("mouseout", function (e) {
            _this.findxy("out", e);
        }, false);
    };
    Drawing.prototype.draw = function () {
        this.ctx.beginPath();
        this.ctx.moveTo(this.prevX, this.prevY);
        this.ctx.lineTo(this.currX, this.currY);
        this.ctx.strokeStyle = this.x;
        this.ctx.lineWidth = this.y;
        this.ctx.stroke();
        this.ctx.closePath();
    };
    Drawing.prototype.save = function () {
        var data = this.canvas.toDataURL("image/png");
        data = data.replace('data:image/png;base64,', '');
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        return data;
    };
    Drawing.prototype.findxy = function (res, e) {
        if (res == 'down') {
            this.prevX = this.currX;
            this.prevY = this.currY;
            this.currX = e.clientX - this.canvas.offsetLeft;
            this.currY = e.clientY - this.canvas.offsetTop;
            this.flag = true;
            this.dot_flag = true;
            if (this.dot_flag) {
                this.ctx.beginPath();
                this.ctx.fillStyle = this.x;
                this.ctx.fillRect(this.currX, this.currY, 2, 2);
                this.ctx.closePath();
                this.dot_flag = false;
            }
        }
        if (res == 'up' || res == "out") {
            this.flag = false;
        }
        if (res == 'move') {
            if (this.flag) {
                this.prevX = this.currX;
                this.prevY = this.currY;
                this.currX = e.clientX - this.canvas.offsetLeft;
                this.currY = e.clientY - this.canvas.offsetTop;
                this.draw();
            }
        }
    };
    return Drawing;
}());
//# sourceMappingURL=Drawing.js.map