var ISOConvertor = {
    exportDate: function (date) {
        var dateTime = new Date(date);
        return dateTime.toISOString();
    },
    exportCycle: function () {
        // var startTime = ;
        // var endTime = ;
        //var isoInterval = startTime + "/" + endTime;
        //return isoInterval;
    },
    exportDuration: function (timespan) { //todo
        var isoDuration = moment.duration({
            seconds: timespan.Seconds,
            minutes: timespan.Minutes,
            hours: timespan.Hours,
            days: timespan.Days,
            weeks: timespan.Weeks,
            months: timespan.Month,
            years: timespan.Years
        }).toISOString();
        console.log(isoDuration);
        return isoDuration;
    },
    importDate: function (isoDate) {
        var dateTime = isoDate.split('T');
        return dateTime;
    },
    importCycle: function (isoCycle) {

        return;
    },
    importDuration: function (isoDuration) {
        console.log(isoDuration);
        if (!isoDuration.startsWith('P'))
            return false;
        else {
            var year = isoDuration.substring(isoDuration.indexOf('P') + 1, isoDuration.indexOf('Y'));
            console.log(year);
            var month = isoDuration.substring(isoDuration.indexOf('Y') + 1, isoDuration.indexOf('M'));
            console.log(month);
            var day = isoDuration.substring(isoDuration.indexOf('M') + 1, isoDuration.indexOf('D'));
            console.log(day);
            var hour = isoDuration.substring(isoDuration.indexOf('T') + 1, isoDuration.indexOf('H'));
            console.log(hour);
            var minute = isoDuration.substring(isoDuration.indexOf('H') + 1, isoDuration.lastIndexOf('M'));
            console.log(minute);
            var second = isoDuration.substring(isoDuration.lastIndexOf('M') + 1, isoDuration.indexOf('S'));
            console.log(second);
            var timespanString = year + ":" + month + ":" + day + ":" + hour + ":" + minute + ":" + second;
            console.log(timespanString);
            return timespanString;
        }
    }
};