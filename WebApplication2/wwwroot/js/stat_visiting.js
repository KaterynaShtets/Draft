let canvas = document.getElementById("canvas2");
let rfilter = document.getElementById("rfilter");
let gfilter = document.getElementById("gfilter");
let bfilter = document.getElementById("bfilter");
let info = document.getElementById("info");
let info2 = document.getElementById("info2");

const PLOT_MARGIN = 15, PLOT_MARGIN_2 = PLOT_MARGIN * 2;

let series = new Series(data);

// (X0, Y0) - origin point
const X0 = PLOT_MARGIN, Y0 = canvas.height - PLOT_MARGIN;
// coordinate converter
let X = x => x - X0;
let Y = y => -(y - Y0);


canvas.addEventListener('mousemove', function (e) {
    let x = X(e.clientX - canvas.offsetLeft);
    let y = Y(e.clientY - canvas.offsetTop);

    let point = series.getNearestPoint(x, y);
    if (point) {
        info.innerHTML = `▪ ${data.titles[point.x]} (${point.y})`;
    }
});

canvas.addEventListener('mousedown', function (e) {
    let x = X(e.clientX - canvas.offsetLeft);
    let y = Y(e.clientY - canvas.offsetTop);
    let userNames = series.getAttendanceList(x, y);
    if (userNames.length) {
        info2.innerHTML = userNames.join(', ');
        info2.style.visibility = "visible";
    } else {
        info2.style.visibility = "hidden";
    }
});

document.body.addEventListener('keydown', function (e) {
    if (e.key === "Escape") {
        info2.style.visibility = "hidden";
    }
});

rfilter.addEventListener('change', filterChange);
gfilter.addEventListener('change', filterChange);
bfilter.addEventListener('change', filterChange);

function filterChange() {
    series = new Series(data);
    draw();
}


function draw() {
    let ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.save();

    // coordinate transform
    ctx.translate(X0, Y0);
    ctx.scale(1, -1);

    drawAxises();
    ctx.lineWidth = 1;
    drawOneSeries(series.three[0], 'red');
    drawOneSeries(series.three[1], 'green');
    drawOneSeries(series.three[2], 'blue');

    ctx.restore();

    // inner functions --------------

    function drawAxises() {
        ctx.lineWidth = 0.5;
        ctx.beginPath();
        // axis Ox
        ctx.moveTo(-5, 0);
        ctx.lineTo(canvas.width - PLOT_MARGIN_2, 0);
        for (let i = 0; i < data.titles.length; i++) {
            let x = (i + 1) * series.stepX;
            if (i === 0) {
                ctx.fillText("I", x - 3, -5);
            }
            ctx.moveTo(x, 0);
            ctx.lineTo(x, -3);
        }
        // axis Oy
        ctx.moveTo(0, -5);
        ctx.lineTo(0, canvas.height - PLOT_MARGIN_2);
        for (let i = 0; i < series.maxValue; i += 10) {
            let y = i * series.scaleY;
            if (i === 10) {
                ctx.fillText("I0", -12, y + 5);
            }
            ctx.moveTo(0, y);
            ctx.lineTo(-3, y);
        }
        ctx.stroke();
    }


    function drawOneSeries(serie, color) {
        ctx.strokeStyle = ctx.fillStyle = color;
        let xP = null, yP = null;
        ctx.beginPath();
        for (let i = 0; i < serie.length; i++) {
            let x = (i + 1) * series.stepX;
            let y = serie[i] * series.scaleY;
            ctx.fillRect(x - 2, y - 2, 4, 4);
            if (i) {
                ctx.moveTo(xP, yP);
                ctx.lineTo(x, y);
            }
            [xP, yP] = [x, y];
        }
        ctx.stroke();
    }


}

draw(series.three);
