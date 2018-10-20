class Drawing {
    private canvas: HTMLCanvasElement;
    private ctx: CanvasRenderingContext2D;
    private flag: boolean = false;
    private prevX: number = 0;
    private currX: number = 0;
    private prevY: number = 0.01;
    private currY: number = 0;
    private dot_flag: boolean = false;
    private x: string = "black";
    private y: number = 2;
    private w: number;
    private h: number;

    constructor(canvas: HTMLCanvasElement) {
        console.log({ canvas });
        this.canvas = canvas;
        this.init();
    }

    public init() {
        //this.canvas = document.getElementById("can") as HTMLCanvasElement;
        this.ctx = this.canvas.getContext("2d");
        this.w = this.canvas.width;
        this.h = this.canvas.height;

        this.canvas.addEventListener("mousemove", (e) => {
            this.findxy("move", e);
        }, false);

        this.canvas.addEventListener("mousedown", (e) => {
            this.findxy("down", e);
        }, false);

        this.canvas.addEventListener("mouseup", (e) => {
            this.findxy("up", e);
        }, false);

        this.canvas.addEventListener("mouseout", (e) => {
            this.findxy("out", e);
        }, false);
    }

    public draw() {
        this.ctx.beginPath();
        this.ctx.moveTo(this.prevX, this.prevY);
        this.ctx.lineTo(this.currX, this.currY);
        this.ctx.strokeStyle = this.x;
        this.ctx.lineWidth = this.y;
        this.ctx.stroke();
        this.ctx.closePath();
    }

    public save() {
        let data = this.canvas.toDataURL("image/png")
        data = data.replace('data:image/png;base64,', '');
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        return data;
    }
    public findxy(res, e) {
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
    }
}

