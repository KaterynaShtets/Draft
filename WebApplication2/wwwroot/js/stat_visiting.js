let canvas = document.getElementById("canvas");
let rfilter = document.getElementById("rfilter");
let gfilter = document.getElementById("gfilter");
let bfilter = document.getElementById("bfilter");
let info = document.getElementById("info");
let series = new Series(titles, students, visits);
const PLOT_MARGIN = 10, PLOT_MARGIN_2 = 20;

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
        info.innerHTML = `${titles[point.x]} (${point.y})`;
    }
});

// canvas.addEventListener('mousedown', function (e) {
//     let x = X(e.clientX - canvas.offsetLeft);
//     let y = Y(e.clientY - canvas.offsetTop);
//     info.innerHTML = `${x}   ${y} `;
// });

rfilter.addEventListener('change', filterChange);
gfilter.addEventListener('change', filterChange);
bfilter.addEventListener('change', filterChange);

function filterChange() {
    series = new Series(titles, students, visits);
    draw();
}


function draw() 
{
    let ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.save();

    // coordinate transform
    ctx.translate(X0, Y0);
    ctx.scale(1, -1);

    drawAxises();
    ctx.lineWidth = 1;
    drawOneSeries(series.data[0], 'red');
    drawOneSeries(series.data[1], 'green');
    drawOneSeries(series.data[2], 'blue');

    ctx.restore();
    
    // inner functions --------------
    
    function drawAxises() {
        ctx.lineWidth = 0.5;
        ctx.beginPath();
        // axis 0x
        ctx.moveTo(-5, 0);
        ctx.lineTo(canvas.width - PLOT_MARGIN_2, 0);
        for (let i = 0; i < series.maxValue + 5; i += 10 ) {
            let y = (i) * series.scaleY;
            ctx.moveTo(0, y);
            ctx.lineTo(-3, y);
        }
        // axis 0y
        ctx.moveTo(0, -5);
        ctx.lineTo(0, canvas.height - PLOT_MARGIN_2);
        for (let i = 0; i < titles.length; i++) {
            let x = (i + 1) * series.stepX;
            ctx.moveTo(x, 0);
            ctx.lineTo(x, -3);
        }
        ctx.stroke();
    }


    function drawOneSeries(serie, color){
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
            [xP, yP] = [x, y]
        }
        ctx.stroke();
    }
    

}

draw(series.data);
