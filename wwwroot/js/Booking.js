$(document).ready(function () {

    // Календарь
    const calendarBtn = document.getElementById('calendar-btn');
    const calendarDropdown = document.getElementById('calendar-dropdown');
    const calendarPrev = document.getElementById('calendar-prev');
    const calendarNext = document.getElementById('calendar-next');
    const calendarMonth = document.getElementById('calendar-month');
    const calendarDays = document.getElementById('calendar-days');

    // Прокрутка времени (8:00 - 21:00) с проверкой доступности
    const timePrev = document.getElementById('button-prev');
    const timeNext = document.getElementById('button-next');

    // Бронирование
    const booking = document.querySelector('.button-booking');

    let currentDate = new Date();
    let selectedDate = new Date();
    let bookedTimeSlots = []; // Массив для хранения занятых временных слотов
    let currentTableId = null; // ID текущего выбранного стола

    // Получаем текущее время
    const nowHours = new Date().getHours();
    const nowMinutes = new Date().getMinutes();

    // Устанавливаем начальное время - текущее время + 1 час
    let initialHour = nowHours + 1;
    let initialDayOffset = 0;

    // Если время между 21:00 и 8:00, переходим на следующий день
    if (nowHours >= 21 || nowHours < 8) {
        initialDayOffset = 1;
        initialHour = 8; // Устанавливаем начало рабочего дня
    }

    // Обновляем текущую и выбранную дату
    currentDate.setDate(currentDate.getDate() + initialDayOffset);
    selectedDate = new Date(currentDate);

    // Рассчитываем максимальную доступную дату (7 дней вперед)
    const maxAvailableDate = new Date();
    maxAvailableDate.setDate(maxAvailableDate.getDate() + 7);
    maxAvailableDate.setHours(23, 59, 59, 999); // Конец дня

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

        // Добавляем дни предыдущего месяца (неактивные)
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
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            // Проверяем, прошел ли день или он дальше, чем 7 дней вперед
            if (date < today || date > maxAvailableDate) {
                dayElement.classList.add('other-month');
            }

            // Выделяем выбранную дату
            if (date.toDateString() === selectedDate.toDateString()) {
                dayElement.classList.add('selected');
            }

            // Добавляем обработчик клика только для доступных дней
            if (!dayElement.classList.contains('other-month')) {
                dayElement.addEventListener('click', () => {
                    selectedDate = new Date(year, month, i);
                    if (currentTableId != null) {
                        loadBookedTimeSlots(currentTableId)
                    }
                    document.querySelectorAll('.calendar-day').forEach(d => d.classList.remove('selected'));
                    dayElement.classList.add('selected');
                    updateTimeSlots();
                });
            }

            calendarDays.appendChild(dayElement);
        }

        // Добавляем дни следующего месяца (неактивные, если не входят в 7 дней)
        for (let i = 1; i <= nextMonthDays; i++) {
            const dayElement = document.createElement('div');
            dayElement.className = 'calendar-day other-month';
            dayElement.textContent = i;
            calendarDays.appendChild(dayElement);
        }

        // Проверяем, нужно ли разрешить переключение на следующий месяц
        const lastDayOfCurrentMonth = new Date(year, month + 1, 0);
        calendarNext.style.display = lastDayOfCurrentMonth < maxAvailableDate ? 'block' : 'none';

        // Кнопка "назад" скрыта, если текущий месяц - текущий
        const currentMonth = new Date().getMonth();
        calendarPrev.style.display = month > currentMonth ? 'block' : 'none';
    }

    // Проверяет, доступно ли выбранное время
    function isTimeSlotAvailable(time) {
        const selectedDateTime = new Date(selectedDate);
        const selectedDateTimeWithInterval = new Date(selectedDate);
        const [hours, minutes] = time.split(':').map(Number);
        const selectedInterval = parseInt(document.querySelector('.time-container').textContent[0]);
        selectedDateTime.setHours(hours, minutes, 0, 0);
        selectedDateTimeWithInterval.setHours(hours + selectedInterval, minutes, 0, 0);

        if (selectedDateTimeWithInterval.getHours() > 22 || selectedDateTimeWithInterval.getHours() == 0) {
            document.querySelector('.button-booking').className = 'button-booking denied';
            return false;
        }

        // Проверяем, не попадает ли выбранное время в занятый промежуток
        for (const booking of bookedTimeSlots) {
            const bookingStart = new Date(booking.time);
            const bookingEnd = new Date(bookingStart);
            bookingEnd.setHours(bookingStart.getHours() + booking.interval);

            if ((selectedDateTime >= bookingStart && selectedDateTime < bookingEnd) ||
                (selectedDateTimeWithInterval > bookingStart && selectedDateTimeWithInterval <= bookingEnd) ||
                (selectedDateTime <= bookingStart && selectedDateTimeWithInterval >= bookingEnd))
                {
                document.querySelector('.button-booking').className = 'button-booking denied';
                return false;
            }
            else document.querySelector('.button-booking').className = 'button-booking';
        }
        document.querySelector('.button-booking').className = 'button-booking';
        return true;
    }


    // Обновление временных слотов с учетом занятых промежутков
    function updateTimeSlots() {
        const timeSlotsContainer = document.querySelector('.button-time-text');
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        // Если выбран сегодняшний день
        if (selectedDate.toDateString() === today.toDateString()) {
            // Устанавливаем время на час вперед от текущего, но не менее 8:00 и не более 21:00
            let hour = nowHours + 1;
            if (hour < 8) hour = 8;
            if (hour > 21) hour = 8; // если уже позже 21:00, переходим на 8:00 следующего дня

            // Проверяем, доступно ли это время
            let timeStr = hour + ':00';

            isTimeSlotAvailable(timeStr);

            timeSlotsContainer.textContent = timeStr;
        } else {
            // Для других дней устанавливаем начало рабочего дня
            let timeStr = '8:00';
            timeSlotsContainer.textContent = timeStr;
            isTimeSlotAvailable(timeStr);
        }
    }
    function UpdateBookingReserved() {
        var content = document.querySelector('.booking-reserved-array');
        content.innerHTML = ''
        if (bookedTimeSlots.length == 0) {
            var element = document.createElement('div');
            element.className = 'booking-reserved-element';
            element.textContent = 'Все время свободно!';
            content.appendChild(element);
            return;
        }
        for (const booking of bookedTimeSlots) {
            var element = document.createElement('div');
            element.className = 'booking-reserved-element';
            var start = new Date(booking.time);
            var end = new Date(booking.time);
            end.setHours(start.getHours() + booking.interval)
            element.textContent = start.toLocaleTimeString() + " - " + end.toLocaleTimeString();
            content.appendChild(element);
        }
    }

    // Загружает занятые временные слоты для выбранного стола
    function loadBookedTimeSlots(tableId) {
        var tempDate = new Date(selectedDate);
        tempDate.setHours(12);
        var date = new Date(tempDate).toISOString();
        $.ajax({
            /*url: '/Api/Booking/GetAll',*/
            url: '/Api/Booking/GetByDate',
            type: 'GET',
            data: { tableId: tableId, time: date },
            success: function (bookings) {
                
                bookedTimeSlots = bookings;
                updateTimeSlots(); // Обновляем временные слоты после загрузки данных
                UpdateBookingReserved(); // добавляем все брони в таблицу
            },
            error: function () {
                console.error('Ошибка при загрузке бронирований');
            }
        });
    }

    // Инициализация календаря
    updateCalendar();
    updateTimeSlots();

    // Выбор стола
    const tables = document.querySelectorAll('.table');
    tables.forEach(table => {
        table.addEventListener('click', () => {
            tables.forEach(d => d.classList.remove('selected'));
            table.classList.add('selected');

            currentTableId = table.getAttribute('data-table-id');
            if (currentTableId) {
                loadBookedTimeSlots(currentTableId);
            }

            
        });
    });

    

    
    booking.addEventListener('click', () => {
        if (booking.className != 'button-booking denied') {
            const monthNames = {
                'январь': 0, 'февраль': 1, 'март': 2, 'апрель': 3,
                'май': 4, 'июнь': 5, 'июль': 6, 'август': 7,
                'сентябрь': 8, 'октябрь': 9, 'ноябрь': 10, 'декабрь': 11
            };

            const monthStr = document.querySelector('#calendar-month').textContent.split(' ')[0].toLowerCase();
            const day = parseInt(document.querySelector('.calendar-day.selected').textContent, 10);
            const year = new Date().getFullYear();
            const timeStr = document.querySelector('.button-time-text').textContent.trim();
            const tableId = document.querySelector('.table.selected');

            if (tableId == null) {
                alert('Выберите стол!');
                return;
            }

            const month = monthNames[monthStr];
            const [hours, minutes] = timeStr.split(':').map(Number);
            const bookingDate = new Date(year, month, day, hours, minutes);
            const interval = parseInt(document.querySelector('.time-container').textContent[0]);

            isTimeSlotAvailable(timeStr);


            $.ajax({
                url: '/Api/Booking/Add',
                type: 'POST',
                data: {
                    tableId: tableId.getAttribute('data-table-id'),
                    time: bookingDate.toISOString(),
                    interval: interval
                },
                success: function () {
                    console.log("Booking добавлен!");
                    // Обновляем список бронирований после успешного добавления
                    if (currentTableId) {
                        loadBookedTimeSlots(currentTableId);
                    }
                },
                error: function () {
                    alert('Ошибка при бронировании. Пожалуйста, попробуйте еще раз.');
                }
            });
        }
        
    });

    // Переключение месяцев (только если есть доступные дни в следующем месяце)
    calendarPrev.addEventListener('click', () => {
        currentDate.setMonth(currentDate.getMonth() - 1);
        updateCalendar();
    });

    calendarNext.addEventListener('click', () => {
        currentDate.setMonth(currentDate.getMonth() + 1);
        updateCalendar();
    });

    // Открытие/закрытие календаря
    calendarBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        calendarDropdown.classList.toggle('active');

        const timeSlotsContainer = document.querySelector('.button-time-text');
        isTimeSlotAvailable(timeSlotsContainer.textContent);
    });

    document.addEventListener('click', (e) => {
        if (!calendarBtn.contains(e.target) && !calendarDropdown.contains(e.target)) {
            calendarDropdown.classList.remove('active');
        }
    });

    timePrev.addEventListener('click', () => {
        const timeSlotsContainer = document.querySelector('.button-time-text');
        let currentTime = parseInt(timeSlotsContainer.textContent);

        if (currentTime > 8) {
            timeSlotsContainer.textContent = (currentTime - 1) + ':00';
        } else {
            timeSlotsContainer.textContent = '21:00';
        }

        isTimeSlotAvailable(timeSlotsContainer.textContent);
    });

    timeNext.addEventListener('click', () => {
        const timeSlotsContainer = document.querySelector('.button-time-text');
        let currentTime = parseInt(timeSlotsContainer.textContent);

        if (currentTime < 21) {
            timeSlotsContainer.textContent = (currentTime + 1) + ':00';
        } else {
            timeSlotsContainer.textContent = '8:00';
        }

        isTimeSlotAvailable(timeSlotsContainer.textContent);

    });

    $(document).on('click', '#interval-next', function () {
        const cont = document.querySelector('.time-container');
        if (cont.textContent[0] < 3) cont.textContent = (parseInt(cont.textContent[0]) + 1) + ' ч';
        const timeStr = document.querySelector('.button-time-text').textContent.trim();
        const timeSlotsContainer = document.querySelector('.button-time-text');

        isTimeSlotAvailable(timeSlotsContainer.textContent);
    });

    $(document).on('click', '#interval-prev', function () {
        const cont = document.querySelector('.time-container');
        if (cont.textContent[0] > 1) cont.textContent = (parseInt(cont.textContent[0]) - 1) + ' ч';
        const timeStr = document.querySelector('.button-time-text').textContent.trim();
        const timeSlotsContainer = document.querySelector('.button-time-text');

        isTimeSlotAvailable(timeSlotsContainer.textContent);
    });
});