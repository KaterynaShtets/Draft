let canvas = document.getElementById("canvasSolving");
let backFilter = document.getElementById("backFilter");
let foreFilter = document.getElementById("foreFilter");
let info = document.getElementById("info");

const PLOT_MARGIN = 15, PLOT_MARGIN_2 = PLOT_MARGIN * 2;

// (X0, Y0) - origin point
const X0 = PLOT_MARGIN, Y0 = canvas.height - PLOT_MARGIN;
// coordinate converter
let X = x => x - X0;
let Y = y => -(y - Y0);

// solvByWeeks - решения по неделям
let stepX = 20;
let scaleY = (canvas.height - PLOT_MARGIN_2) / Math.max(...solvByWeeks);



canvas.addEventListener('mousemove', function (e) {
    let x = X(e.clientX - canvas.offsetLeft);
    let y = Y(e.clientY - canvas.offsetTop);

    let point = getNearestPoint(solvByWeeks, x, y);
    if (point !== null) {
        info.innerHTML = `▪ Неделя ${point.x},  задач - ${point.y}`;
        canvas.style.cursor = 'pointer';
    } else {
        info.innerHTML = '▪';
        canvas.style.cursor = '';
    }
});

function getNearestPoint(serie, ax, ay) {        
    for (let i = 0; i < serie.length; i++) {
        let x = (i + 1) * stepX;
        let y = serie[i] * scaleY;
        if (Math.hypot(x - ax, y - ay) < 10)
            return { x: i, y: serie[i] };        
    }
    return null;
}

function draw() 
{
    let ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.save();

    // coordinate transform
    ctx.translate(X0, Y0);
    ctx.scale(1, -1);

    drawAxises(solvByWeeks);
    ctx.lineWidth = 1;
    drawOneSeries(solvByWeeks, 'red');
    ctx.restore();
    
    // inner functions --------------

    function drawAxises(serie) {
        ctx.lineWidth = 0.5;
        ctx.beginPath();
        // axis 0x
        ctx.moveTo(-5, 0);
        ctx.lineTo(canvas.width - PLOT_MARGIN_2, 0);
        for (let i = 0; i < serie.length; i++) {
            let x = (i + 1) * stepX;
            if (i === 0) {
                ctx.fillText("I", x - 3, -5);
            }
            ctx.moveTo(x, 0);
            ctx.lineTo(x, -3);
        }
        // axis 0y
        ctx.moveTo(0, -5);
        ctx.lineTo(0, canvas.height - PLOT_MARGIN_2);
        let maxValue = Math.max(...serie);
        for (let i = 0; i < maxValue; i += 10) {
            let y = i * scaleY;
            if (i === 10) {
                ctx.fillText("I0", -12, y + 5);
            }
            ctx.moveTo(0, y);
            ctx.lineTo(-3, y);
        }
        ctx.stroke();
    }

    function drawOneSeries(serie, color){
        ctx.strokeStyle = ctx.fillStyle = color;
        let xP = null, yP = null;
        ctx.beginPath();
        for (let i = 0; i < serie.length; i++) {
            let x = (i + 1) * stepX;
            let y = serie[i] * scaleY;
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

draw();
