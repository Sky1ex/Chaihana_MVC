$(document).ready(function () {
    // Генерация временных слотов
    const timeSlotsContainer = document.querySelector('.button-time-text');

    var now = new Date().getHours() + 1;
    if (now >= 22 || now <= 8) timeSlotsContainer.textContent = '8:00';
    else timeSlotsContainer.textContent = (now) % 22 + ':00';

    // Прокрутка времени
    const timePrev = document.getElementById('button-prev');
    const timeNext = document.getElementById('button-next');

    timePrev.addEventListener('click', () => {
        if (timeSlotsContainer.textContent == '8:00') timeSlotsContainer.textContent = '21:00';
        else timeSlotsContainer.textContent = (parseInt(timeSlotsContainer.textContent) - 1) % 22 + ':00';
    });

    timeNext.addEventListener('click', () => {
        if (timeSlotsContainer.textContent == '21:00') timeSlotsContainer.textContent = '8:00';
        else timeSlotsContainer.textContent = (parseInt(timeSlotsContainer.textContent) + 1) % 22 + ':00';
    });

    // Календарь
    const calendarBtn = document.getElementById('calendar-btn');
    const calendarDropdown = document.getElementById('calendar-dropdown');
    const calendarPrev = document.getElementById('calendar-prev');
    const calendarNext = document.getElementById('calendar-next');
    const calendarMonth = document.getElementById('calendar-month');
    const calendarDays = document.getElementById('calendar-days');

    let currentDate = new Date();
    let selectedDate = new Date();

    function updateCalendar() {
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth();

        calendarMonth.textContent = new Intl.DateTimeFormat('ru-RU', {
            month: 'long',
            year: 'numeric'
        }).format(currentDate);

        calendarDays.innerHTML = '';

        // Заголовки дней недели
        const daysOfWeek = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];
        daysOfWeek.forEach(day => {
            const dayElement = document.createElement('div');
            dayElement.className = 'calendar-day';
            dayElement.textContent = day;
            dayElement.style.fontWeight = 'bold';
            calendarDays.appendChild(dayElement);
        });

        // Первый день месяца
        const firstDay = new Date(year, month, 1);
        const firstDayOfWeek = firstDay.getDay() || 7; // 1-7 вместо 0-6

        // Последний день месяца
        const lastDay = new Date(year, month + 1, 0);
        const lastDate = lastDay.getDate();

        // Дни предыдущего месяца
        const prevMonthLastDay = new Date(year, month, 0);
        const prevMonthLastDate = prevMonthLastDay.getDate();

        // Дни следующего месяца
        const nextMonthDays = 7 - (firstDayOfWeek + lastDate - 1) % 7;

        // Добавляем дни предыдущего месяца
        for (let i = firstDayOfWeek - 1; i > 0; i--) {
            const dayElement = document.createElement('div');
            dayElement.className = 'calendar-day other-month';
            dayElement.textContent = prevMonthLastDate - i + 1;
            calendarDays.appendChild(dayElement);
        }

        // Добавляем дни текущего месяца
        for (let i = 1; i <= lastDate; i++) {
            const dayElement = document.createElement('div');
            dayElement.className = 'calendar-day';
            dayElement.textContent = i;

            const date = new Date(year, month, i);
            if (date.toDateString() === selectedDate.toDateString()) {
                dayElement.classList.add('selected');
            }

            dayElement.addEventListener('click', () => {
                selectedDate = new Date(year, month, i);
                document.querySelectorAll('.calendar-day').forEach(d => d.classList.remove('selected'));
                dayElement.classList.add('selected');
                // Здесь можно добавить логику обновления выбранной даты
            });

            calendarDays.appendChild(dayElement);
        }

        // Добавляем дни следующего месяца
        for (let i = 1; i <= nextMonthDays; i++) {
            const dayElement = document.createElement('div');
            dayElement.className = 'calendar-day other-month';
            dayElement.textContent = i;
            calendarDays.appendChild(dayElement);
        }
    }

    calendarBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        calendarDropdown.classList.toggle('active');
    });

    calendarPrev.addEventListener('click', () => {
        currentDate.setMonth(currentDate.getMonth() - 1);
        updateCalendar();
    });

    calendarNext.addEventListener('click', () => {
        currentDate.setMonth(currentDate.getMonth() + 1);
        updateCalendar();
    });

    document.addEventListener('click', (e) => {
        if (calendarBtn.contains(e.target)) {
            calendarDropdown.classList.remove('active');
        }
    });

    var tables = document.querySelectorAll('.table');
    for (var i = 0; i < tables.length; i++)
    {
        const table = tables[i];
        table.addEventListener('click', () => {
            tables.forEach(d => d.classList.remove('selected'));
            table.classList.add('selected');
        })
    }

    const booking = document.querySelector('.button-booking');

    booking.addEventListener('click', () => {

        const monthNames = {
            'январь': 0, 'февраль': 1, 'март': 2, 'апрель': 3,
            'май': 4, 'июнь': 5, 'июль': 6, 'август': 7,
            'сентябрь': 8, 'октябрь': 9, 'ноябрь': 10, 'декабрь': 11
        };

        const monthStr = document.querySelector('#calendar-month').textContent.split(' ')[0].toLowerCase();
        const day = parseInt(document.querySelector('.calendar-day.selected').textContent, 10);
        const year = new Date().getFullYear();
        const timeStr = document.querySelector('.button-time-text').textContent.trim(); // "14:00"
        const tableId = document.querySelector('.table.selected').getAttribute('data-table-id');

        // Получаем номер месяца (0–11)
        const month = monthNames[monthStr];

        // Разбиваем время на часы и минуты
        const [hours, minutes] = timeStr.split(':').map(Number);

        // Создаем Date объект
        const bookingDate = new Date(year, month, day, hours, minutes);

        console.log(bookingDate); // Проверка в консоли

        $.ajax({
            url: '/Api/Booking/Add',
            type: 'POST',
            data: {
                tableId: tableId,
                time: bookingDate.toISOString() // Отправляем в формате ISO
            },
            success: function () {
                console.log("Booking добавлен!");
            }
        });

        /*var month = document.querySelector('#calendar-month').textContent.split(' ')[0];
        var day = document.querySelector('.calendar-day.selected').textContent;
        var time = document.querySelector('.button-time-text').textContent;
        var tableId = document.querySelector('.table.selected').getAttribute('data-table-id');

        const monthNames = {
            'январь': 0, 'февраль': 1, 'март': 2, 'апрель': 3,
            'май': 4, 'июнь': 5, 'июль': 6, 'август': 7,
            'сентябрь': 8, 'октябрь': 9, 'ноябрь': 10, 'декабрь': 11
        };

        var fullMonth = new Intl.DateTimeFormat('ru-RU', { dateStyle: "long" });

        var str = month.toString() + ' ' + day.toString() + ',' + new Date().getFullYear() + ' ' + time.toString();

        fullMonth = new Date(month.toString() + ' ' + day.toString() + ',' + new Date().getFullYear() + ' ' + time.toString());

        $.ajax({
            url: '/Api/Booking/Add',
            type: 'POST',
            data: { tableId: tableId, time: fullMonth },
            success: function () {
                console.log("Booking добавлен!");
            }
        });*/
    });

    // Инициализация календаря
    updateCalendar();

});