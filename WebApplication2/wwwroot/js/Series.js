class Series {
    constructor(data) {
        this.titles = data.titles;
        this.students = data.students;
        this.visits = data.visits;
        this.filters = [rfilter.value, gfilter.value, bfilter.value];

        this.three = [
            this.getSerie(this.filters[0]),
            this.getSerie(this.filters[1]),
            this.getSerie(this.filters[2])];
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
            Math.max(...this.three[0]),
            Math.max(...this.three[1]),
            Math.max(...this.three[2]));
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
        for (let k = 0; k < this.three.length; k++) {
            let serie = this.three[k];
            for (let i = 0; i < serie.length; i++) {
                let x = (i + 1) * stepX;
                let y = serie[i] * scaleY;
                if (Math.hypot(x - ax, y - ay) < 10)
                    return { x: i, y: serie[i], z: k };
            }
        }
        return null;
    }

    getAttendanceList(ax, ay) {
        let point = this.getNearestPoint(ax, ay);
        if (point) {
            let lecNo = point.x;
            let filterNo = point.z;
            let regex = new RegExp(this.filters[filterNo]);
            let userNames = [];
            for (let j of this.visits[lecNo]) {
                let userName = this.students[j][0];
                if (regex.test(userName))
                    userNames.push(userName);
            }
            return userNames;
        }
        return [];
    }

}
