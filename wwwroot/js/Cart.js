$(document).ready(function ()
{
    // Оформление заказа (пока без оплаты)
    $(document).on('click', '.button-accept', function () {

        const selectedProducts = [];
        document.querySelectorAll('.mini-card').forEach(el => {
            selectedProducts.push(el.id);
        });

        var address = document.querySelector('.address-container').id;
        if (address == "") {
            alert('Укажите адрес!');
            return;
        }

        $.ajax({
            url: '/Api/Cart/Purshare',
            type: 'POST',
            dataType: 'application/json',
            data: { orderElements: selectedProducts, addressId: address },
            success: function (data) {
                alert('Заказ оформлен!');
                window.location.href = '/Menu'
            },
            error: function (message) {
                alert(message.responseText);
            }
        });
    });

    // Получение всех адресов пользователя
    $(document).on('click', '.button-change-address', function () {
        $('.map-modal').css('display', 'grid');

        GetAddresses();
    });

    // Добавление функции выбора адреса
    $(document).on('click', '.address-add', function () {

        const addressContainer = $('.address-data');

        // Очищаем контейнер перед добавлением (если нужно)
        addressContainer.empty();

        // Добавляем каждый адрес в DOM
        addresses = $(`
                <div class="name-address">
					Город:
					<input class="input-address" id="city">
				</div>

				<div class="name-address">
					Улица:
					<input class="input-address" id="street">
				</div>

				<div class="name-address">
					Дом:
					<input class="input-address" id="house">
				</div>

				<div class="name-address">
					Квартира:
					<input class="input-address" id="apartment">
				</div>`);

        addressContainer.append(addresses);

        var button = document.querySelector('#send-address-data');

        button.textContent = 'Сохранить';

        button.id = 'save-address-data'

        document.querySelector('.address-add').setAttribute('hidden', '');
    });

    // Сохранение адреса
    $(document).on('click', '#save-address-data', function () {

        var city = document.querySelector('#city').value;
        var street = document.querySelector('#street').value;
        var house = document.querySelector('#house').value;
        var apartment = document.querySelector('#apartment').value;

        if (city == '' || street == '' || house == '' || apartment == '') alert('Укажите полный адресс!');
        else

        $.ajax({
            url: '/Api/Account/AddAddress',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ City: city, Street: street, House: house, Apartment: apartment }),
            success: function () {
                console.log('Адрес сохранен!');
                GetAddresses();
            },
            error: function () {
                alert('Укажите правильный адресс доставки!');
            }
        });
    });

    // Выбор адреса для заказа
    $(document).on('click', '#send-address-data', function () {
        // Получаем выбранный radio-элемент
        const selectedAddress = $('input[name="address"]:checked');

        // Проверяем, что адрес выбран
        if (selectedAddress.length === 0) {
            alert('Пожалуйста, выберите адрес');
            return;
        }

        // Получаем ID выбранного адреса (чистый DOM-элемент)
        const addressId = selectedAddress.attr('id'); // или selectedAddress[0].id

        // Находим соответствующий label
        const addressLabel = $(`label[for="address-${addressId}"]`);

        // Проверяем, что label найден
        if (addressLabel.length === 0) {
            console.error('Label для выбранного адреса не найден');
            return;
        }

        // Разбиваем текст адреса
        const addressText = addressLabel.text().split(", ");

        // Обновляем данные в интерфейсе
        $('#city-element').text('Город: ' + (addressText[0] || ''));
        $('#street-element').text('Улица: ' + (addressText[1] || ''));
        $('#house-element').text('Дом: ' + (addressText[2] || ''));
        $('#appartment-element').text('Квартира: ' + (addressText[3] || ''));

        document.querySelector('.address-container').id = addressId;

        // Скрываем модальное окно
        $('.map-modal').hide();
    });

    // Закрытие карты
    $(document).on('click', '.close', function () {
        $('.map-modal').css('display', 'none');
    });

    // Назад в меню
    $(document).on('click', '.button-back', function () {
        window.location.href = '/Menu'
    });

    function GetAddresses() {
        $.ajax({
            url: '/Api/Account/GetAddresses',
            method: 'GET',
            success: function (addresses) {
                const addressContainer = $('.address-data');

                // Очищаем контейнер перед добавлением (если нужно)
                addressContainer.empty();

                // Добавляем каждый адрес в DOM
                addresses.forEach(address => {
                    const addressPoint = $(`
                <div class="address-point">
                    <input type="radio" name="address" id="${address.addressId}" ${address.isDefault ? 'checked' : ''}>
                    <label for="address-${address.addressId}">
                        ${address.city}, ${address.street}, ${address.house}${address.apartment ? ', кв. ' + address.apartment : ''}
                    </label>
                </div>
                    `);

                    addressContainer.append(addressPoint);
                });

                document.querySelector('.address-add').removeAttribute('hidden');

                var button = document.querySelector('#save-address-data');

                button.textContent = 'Выбрать';

                button.id = 'send-address-data'
            },
            error: function (error) {
                console.error('Ошибка при загрузке адресов:', error);
            }
        }
        )
    };
});