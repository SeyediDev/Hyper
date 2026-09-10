/* 
 * Calendar Chart Renderer - Professional Monthly Calendar Display
 * 
 * Exposes: window.renderCalendarChart(selector, rawData, options)
 * 
 * Core Features:
 * - Monthly calendar grid layout (7-column day grid)
 * - Persian (Shamsi) and Gregorian calendar support with real-time conversion
 * - Dynamic color coding: Low (1-5) | Medium (6-15) | High (16-30) | Very-High (31+)
 * - Interactive tooltips with hover effects
 * - Month navigation with arrow buttons
 * - Data aggregation by date with sum and average calculations
 * 
 * Advanced Features:
 * - Export to CSV with Persian text encoding
 * - Keyboard navigation (arrow keys for month navigation)
 * - Full ARIA accessibility support (roles, labels, headers)
 * - Responsive design (Desktop/Tablet/Mobile)
 * - Dark mode support with proper contrast
 * - Tooltip event handling with fade animations
 * - Debounced rendering (100ms) to prevent performance issues
 * - Focus states for keyboard navigation
 * 
 * Options:
 * - calendar: 'persian'|'gregorian' - Initial calendar type
 * - year: number - Initial year
 * - month: number - Initial month
 * - showAggregates: boolean - Show sum/avg/active days (default: true)
 * - showExport: boolean - Show export button (default: true)
 */
(function(window){
    if (typeof $ === 'undefined') {
        console.error('chart-calendar.js: jQuery is required');
        return;
    }

    // ============================================================================
    // Persian Calendar Converter
    // ============================================================================
    var PersianCalendar = {
        toPersian: function(gd) {
            var gy = gd.getFullYear();
            var gm = gd.getMonth() + 1;
            var gday = gd.getDate();
            
            var g_d_n = 365*gy + Math.floor((gy+3)/4) - Math.floor((gy+99)/100) + Math.floor((gy+399)/400);
            for(var i=0; i<gm; i++) {
                switch(i) {
                    case 0: case 2: case 4: case 6: case 7: case 9: case 11:
                        g_d_n += 31; break;
                    case 1: 
                        g_d_n += (gy%4===0 && (gy%100!==0 || gy%400===0)) ? 29 : 28; break;
                    case 3: case 5: case 8: case 10:
                        g_d_n += 30; break;
                }
            }
            g_d_n += gday;
            
            var j_d_n = g_d_n - 79;
            var j_np = Math.floor(j_d_n / 12053);
            j_d_n %= 12053;
            
            var py = 400*j_np + 100*Math.floor(j_d_n/36524);
            j_d_n %= 36524;
            if(j_d_n >= 36524) j_d_n = 36524;
            py += 4*Math.floor(j_d_n/1461);
            j_d_n %= 1461;
            if(j_d_n >= 1461) j_d_n = 1461;
            py += Math.floor(j_d_n/365);
            j_d_n %= 365;
            
            var pm = 0;
            for(var i = 0; i < 12; i++) {
                var djm = (i < 6) ? 31 : 30;
                if(i === 11 && (gy % 4 === 0) && ((gy % 100 !== 0) || (gy % 400 === 0))) djm = 29;
                if(j_d_n < djm) break;
                j_d_n -= djm;
            }
            pm = i + 1;
            var pd = j_d_n + 1;
            return {y: py, m: pm, d: pd};
        },
        fromPersian: function(py, pm, pd) {
            var jy = py, jm = pm, jd = pd;
            var jy1 = jy - 979;
            var jm1 = jm - 1;
            var jd1 = jd - 1;
            
            var j_np = Math.floor(jy1 / 33);
            jy1 %= 33;
            
            var gy = 1600 + 33 * j_np + 4 * Math.floor(jy1 / 4) + Math.floor((jy1 % 4) * 365.25);
            var gm = 1, gd = 1;
            
            var dayOfYear = 0;
            for(var i = 0; i < jm1; i++) {
                dayOfYear += (i < 6) ? 31 : 30;
            }
            dayOfYear += jd1;
            
            gy += Math.floor(dayOfYear / 365.25);
            dayOfYear %= 365.25;
            
            var monthDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
            if ((gy % 4 === 0 && gy % 100 !== 0) || gy % 400 === 0) monthDays[1] = 29;
            
            for(var i = 0; i < 12; i++) {
                if(dayOfYear < monthDays[i]) {
                    gm = i + 1;
                    gd = dayOfYear + 1;
                    break;
                }
                dayOfYear -= monthDays[i];
            }
            
            return new Date(gy, gm - 1, gd);
        },
        getMonthName: function(m) {
            var names = ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور', 'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'];
            return names[m-1] || '';
        },
        getDaysInMonth: function(y, m) {
            if(m <= 6) return 31;
            if(m <= 11) return 30;
            var isLeap = ((y % 33) % 4) === 1;
            return isLeap ? 30 : 29;
        }
    };

    // ============================================================================
    // Helper Functions
    // ============================================================================
    function parseDateItem(item){
        if(!item) return null;
        if(item.Date) return new Date(item.Date);
        if(item.date) return new Date(item.date);
        if(item.x) return new Date(item.x);
        if(item.Year && item.Month && item.Day) return new Date(item.Year, item.Month - 1, item.Day);
        for(var k in item){
            if(typeof item[k] === 'string' && /\d{4}-\d{2}-\d{2}/.test(item[k])) return new Date(item[k]);
        }
        return null;
    }

    function getValue(item) {
        return (typeof item.value === 'number') ? item.value : 
               (typeof item.Value === 'number' ? item.Value : 
               (typeof item.Count === 'number'? item.Count : 
               (typeof item.Sum === 'number'? item.Sum:1)));
    }

    function getColorRange(value) {
        if (!value || value === 0) return '';
        if (value >= 1 && value <= 5) return 'low';
        if (value >= 6 && value <= 15) return 'medium';
        if (value >= 16 && value <= 30) return 'high';
        return 'very-high';
    }

    function buildCalendarData(data, year, month, usePersian) {
        var dataMap = {};
        data.forEach(function(it){
            var date = parseDateItem(it);
            if(!date) return;
            var key;
            if(usePersian) {
                var pd = PersianCalendar.toPersian(date);
                if(pd.y !== year || pd.m !== month) return;
                key = pd.d;
            } else {
                if(date.getFullYear() !== year || date.getMonth() + 1 !== month) return;
                key = date.getDate();
            }
            dataMap[key] = (dataMap[key] || 0) + getValue(it);
        });
        return dataMap;
    }

    function getMonthInfo(year, month, usePersian) {
        if(usePersian) {
            var daysInMonth = PersianCalendar.getDaysInMonth(year, month);
            var firstDay = PersianCalendar.fromPersian(year, month, 1);
            var startDayOfWeek = firstDay.getDay();
            return {
                daysInMonth: daysInMonth,
                startDayOfWeek: startDayOfWeek,
                monthName: PersianCalendar.getMonthName(month),
                year: year,
                month: month
            };
        } else {
            var firstDay = new Date(year, month - 1, 1);
            var daysInMonth = new Date(year, month, 0).getDate();
            var startDayOfWeek = firstDay.getDay();
            var monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
            return {
                daysInMonth: daysInMonth,
                startDayOfWeek: startDayOfWeek,
                monthName: monthNames[month - 1],
                year: year,
                month: month
            };
        }
    }

    // ============================================================================
    // Render Functions
    // ============================================================================
    function renderCalendar(selector, dataMap, monthInfo, options) {
        var usePersian = options.calendar === 'persian';
        var year = options.year || monthInfo.year;
        // monthInfo now contains numeric month property thanks to getMonthInfo
        var month = options.month || monthInfo.month;
        var weekdays = usePersian ? 
            ['شنبه', 'یکشنبه', 'دوشنبه', 'سه‌شنبه', 'چهارشنبه', 'پنجشنبه', 'جمعه'] : 
            ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        
        var html = '<div class="calendar-container" role="application" aria-label="تقویم ماهانه">';
        
        // Header with navigation
        html += '<div class="calendar-header" role="banner">';
        html += '<button class="calendar-nav prev-month" aria-label="ماه قبل" title="ماه قبل"><i class="fa fa-chevron-right"></i></button>';
        html += '<h3 role="heading" aria-level="1">' + monthInfo.monthName + ' <span class="year-display">' + monthInfo.year + '</span></h3>';
        html += '<button class="calendar-nav next-month" aria-label="ماه بعد" title="ماه بعد"><i class="fa fa-chevron-left"></i></button>';
        html += '</div>';
        
        // Calendar switch
        html += '<div class="calendar-switch" role="group" aria-label="انتخاب نوع تقویم">';
        html += '<label><input type="radio" name="calendar-type" value="gregorian" ' + (usePersian ? '' : 'checked') + ' aria-label="تقویم میلادی"> میلادی</label>';
        html += '<label><input type="radio" name="calendar-type" value="persian" ' + (usePersian ? 'checked' : '') + ' aria-label="تقویم شمسی"> شمسی</label>';
        html += '</div>';
        
        // Weekdays header
        html += '<div class="calendar-weekdays" role="row">';
        weekdays.forEach(function(day) {
            html += '<div class="weekday" role="columnheader">' + day + '</div>';
        });
        html += '</div>';
        
        // Calendar grid
        html += '<div class="calendar-grid" role="grid">';
        
        var totalCells = 42;
        var startOffset = monthInfo.startDayOfWeek;
        if(usePersian) {
            startOffset = (startOffset + 1) % 7;
        }
        
        for(var i = 0; i < totalCells; i++) {
            var dayNumber = i - startOffset + 1;
            var isCurrentMonth = dayNumber >= 1 && dayNumber <= monthInfo.daysInMonth;
            var value = isCurrentMonth ? dataMap[dayNumber] || 0 : 0;
            var colorRange = getColorRange(value);
            var ariaLabel = isCurrentMonth ? dayNumber + ': ' + value + ' مورد' : '';
            
            html += '<div class="calendar-day" ' +
                   'data-day="' + dayNumber + '" ' +
                   'data-value="' + value + '" ' +
                   'data-range="' + colorRange + '" ' +
                   'data-outside="' + (!isCurrentMonth) + '" ' +
                   'role="gridcell" ' +
                   'aria-label="' + ariaLabel + '">';
            
            if(isCurrentMonth) {
                if(value > 0) {
                    html += '<div class="day-value" aria-label="' + dayNumber + ': ' + value + ' رویداد">' + value + '</div>';
                }
                html += '<div class="calendar-tooltip" role="tooltip"></div>';
            }
            html += '</div>';
        }
        
        html += '</div>';
        
        // Aggregates
        if(options.showAggregates !== false) {
            var total = Object.values(dataMap).reduce(function(sum, val) { return sum + val; }, 0);
            var activedays = Object.values(dataMap).filter(function(val) { return val > 0; }).length;
            var avg = activedays > 0 ? (total / activedays).toFixed(2) : 0;
            
            html += '<div class="calendar-aggregates">';
            html += '<div><strong>' + total + '</strong><div>مجموع ماه</div></div>';
            html += '<div><strong>' + activedays + '</strong><div>روز فعال</div></div>';
            html += '<div><strong>' + avg + '</strong><div>میانگین روزانه</div></div>';
            html += '</div>';
        }

        // Export Button
        if(options.showExport !== false) {
            html += '<button class="calendar-export-btn">📥 خروجی اکسل</button>';
        }
        
        html += '</div>';
        
        $(selector).html(html);
        
        // Event handlers
        $(selector).find('.calendar-nav.prev-month').click(function() {
            var newMonth = month - 1;
            var newYear = year;
            if(newMonth < 1) {
                newMonth = 12;
                newYear--;
            }
            renderCalendarChart(selector, options.rawData, $.extend({}, options, {year: newYear, month: newMonth}));
        });
        
        $(selector).find('.calendar-nav.next-month').click(function() {
            var newMonth = month + 1;
            var newYear = year;
            if(newMonth > 12) {
                newMonth = 1;
                newYear++;
            }
            renderCalendarChart(selector, options.rawData, $.extend({}, options, {year: newYear, month: newMonth}));
        });
        
        $(selector).find('input[name="calendar-type"]').change(function(e) {
            e.stopPropagation();
            var newCalendar = $(this).val();
            var newYear = year;
            var newMonth = month;
            
            if(newCalendar === 'persian' && !usePersian) {
                var pd = PersianCalendar.toPersian(new Date(year, month - 1, 1));
                newYear = pd.y;
                newMonth = pd.m;
            } else if(newCalendar === 'gregorian' && usePersian) {
                var gd = PersianCalendar.fromPersian(year, month, 1);
                newYear = gd.getFullYear();
                newMonth = gd.getMonth() + 1;
            }
            
            var newOptions = $.extend({}, options, {calendar: newCalendar, year: newYear, month: newMonth});
            renderCalendarChart(selector, options.rawData, newOptions);
        });

        // Export functionality
        $(selector).find('.calendar-export-btn').click(function() {
            exportToExcel(dataMap, monthInfo, options);
        });

        // Add tooltip event handlers for better UX
        $(selector).find('.calendar-day').on('mouseenter', function() {
            var value = $(this).data('value');
            if(value > 0) {
                var tooltip = $(this).find('.calendar-tooltip');
                if(tooltip.length) {
                    tooltip.text(value + ' مورد').stop(true, true).fadeIn(200);
                }
            }
        }).on('mouseleave', function() {
            $(this).find('.calendar-tooltip').stop(true, true).fadeOut(200);
        });

        // Keyboard navigation
        $(document).on('keydown.calendar-' + Date.now(), function(e) {
            var activeElement = document.activeElement;
            if(!$(activeElement).closest(selector).length) return;
            
            if(e.key === 'ArrowLeft' || e.key === 'ArrowRight') {
                e.preventDefault();
                if(e.key === 'ArrowLeft') {
                    $(selector).find('.calendar-nav.prev-month').click();
                } else {
                    $(selector).find('.calendar-nav.next-month').click();
                }
            }
        });
    }

    function exportToExcel(dataMap, monthInfo, options) {
        var usePersian = options.calendar === 'persian';
        var rows = [
            ['تاریخ', 'تعداد'],
            [monthInfo.monthName + ' ' + monthInfo.year, '']
        ];

        Object.keys(dataMap).sort(function(a, b) { return parseInt(a) - parseInt(b); }).forEach(function(day) {
            if(dataMap[day] > 0) {
                var date = usePersian ? 
                    monthInfo.year + '/' + monthInfo.monthName + '/' + day :
                    day + '/' + monthInfo.monthName + '/' + monthInfo.year;
                rows.push([date, dataMap[day]]);
            }
        });

        var csv = rows.map(function(row) {
            return row.map(function(cell) {
                return '"' + cell + '"';
            }).join(',');
        }).join('\n');

        var link = document.createElement('a');
        link.href = 'data:text/csv;charset=utf-8,%EF%BB%BF' + encodeURIComponent(csv);
        link.download = 'calendar-' + monthInfo.year + '-' + monthInfo.monthName + '.csv';
        link.click();
    }

    // ============================================================================
    // Main Render Function with Debouncing
    // ============================================================================
    var calendarRenderTimeouts = {};
    
    window.renderCalendarChart = function(selector, rawData, options){
        options = options || {};
        options.rawData = rawData;
        options.showAggregates = options.showAggregates !== false;
        options.showExport = options.showExport !== false;
        
        var $el = $(selector);
        if(!$el.length) return;
        
        // Debounce rendering to prevent rapid re-renders
        var selectorKey = $(selector).attr('id') || selector;
        if(calendarRenderTimeouts[selectorKey]) {
            clearTimeout(calendarRenderTimeouts[selectorKey]);
        }
        
        calendarRenderTimeouts[selectorKey] = setTimeout(function() {
        
        var now = new Date();
        var usePersian = options.calendar === 'persian';
        var year = options.year || (usePersian ? PersianCalendar.toPersian(now).y : now.getFullYear());
        var month = options.month || (usePersian ? PersianCalendar.toPersian(now).m : now.getMonth() + 1);
        
        // Load CSS if not already loaded
        if (!$('#calendar-chart-css').length) {
            $('head').append('<link id="calendar-chart-css" rel="stylesheet" href="/Scripts/common-assets-includes/features/report/chart-calendar.css">');
        }
        
        var dataMap = buildCalendarData(rawData || [], year, month, usePersian);
        var monthInfo = getMonthInfo(year, month, usePersian);
        
        renderCalendar(selector, dataMap, monthInfo, options);
        
        }, 100);  // 100ms debounce delay
    };
    
    window.PersianCalendar = PersianCalendar;
})(window);
