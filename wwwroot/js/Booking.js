$(document).ready(function () {
    // Генерация временных слотов
    const timeSlotsContainer = document.querySelector('.button-time-text');
    const hours = Array.from({ length: 24 }, (_, i) => i);

    hours.forEach(hour => {
        const timeSlot = document.createElement('div');
        timeSlot.className = 'time-slot';
        timeSlot.textContent = `${hour}:00`;
        timeSlot.addEventListener('click', () => {
            document.querySelectorAll('.time-slot').forEach(s => s.classList.remove('active'));
            timeSlot.classList.add('active');
        });

        if (hour === 17) {
            timeSlot.classList.add('active');
        }

        timeSlotsContainer.appendChild(timeSlot);
    });

    // Прокрутка времени
    const timeScroll = document.getElementById('time-scroll');
    const timePrev = document.getElementById('button-prev');
    const timeNext = document.getElementById('button-next');

    timePrev.addEventListener('click', () => {
        timeScroll.scrollBy({ left: -100, behavior: 'smooth' });
    });

    timeNext.addEventListener('click', () => {
        timeScroll.scrollBy({ left: 100, behavior: 'smooth' });
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

    // Инициализация календаря
    updateCalendar();

});