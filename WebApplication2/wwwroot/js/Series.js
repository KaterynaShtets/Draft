class Series {
    constructor(titles, students, visits) {
        this.titles = titles;
        this.students = students;
        this.visits = visits;

        this.data = [this.getSerie(rfilter.value),
            this.getSerie(gfilter.value),
            this.getSerie(bfilter.value)];
    }

    getSerie(sample) {
        if (!sample)
            return [];
        let regex = new RegExp(sample);
        let result = [];
        for (let i = 0; i < this.titles.length; i++) {
            let count = 0;
            for (let j of this.visits[i]) {
                let userName = this.students[j][0];
                if (regex.test(userName))
                    count++;
            }
            result.push(count);
        }
        return result;
    }

    get maxValue() {
        return Math.max(
            Math.max(...this.data[0]), 
            Math.max(...this.data[1]), 
            Math.max(...this.data[2]));
    }

    get stepX() {
        return (canvas.width - PLOT_MARGIN_2) / this.titles.length | 0;
    }

    get scaleY() {
        return (canvas.height - PLOT_MARGIN_2) / this.maxValue;
    }

    getNearestPoint(ax, ay) {
        let stepX = this.stepX;
        let scaleY = this.scaleY;
        for (let k = 0; k < this.data.length; k++) {
            let serie = this.data[k];
            for (let i = 0; i < serie.length; i++) {
                let x = (i + 1) * stepX;
                let y = serie[i] * scaleY;
                if (Math.hypot(x - ax, y - ay) < 10)
                    return {x:i, y:serie[i]};
            }
        }
        return null;
    }

}
