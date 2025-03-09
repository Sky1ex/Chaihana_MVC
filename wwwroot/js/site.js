$(document).ready(function () {
    // Открытие модального окна корзины
    $('#view-cart').on('click', function () {
        $('#cartModal').css('display', 'block');
        loadCart();
    });

    // Закрытие модального окна корзины
    $('.close').on('click', function () {
        $('#cartModal').css('display', 'none');
    });

    // Загрузка содержимого корзины
    function loadCart() {
        $.ajax({
            url: 'https://localhost:7008/Cart/ShowCart',
            type: 'GET',
            success: function (data) {
                $('#cartContent').html(data);
            }
        });
    }

    // Добавление товара в корзину
    $('.add-to-cart-btn').click(function () {
        var productCard = $(this).closest('.product-card');

        var productId = productCard.find('.product-id').text().trim();

        // Добавляем анимацию
        $(this).addClass('added');
        setTimeout(() => {
            $(this).removeClass('added');
        }, 500);

        $.ajax({
            url: 'https://localhost:7008/Cart/AddToCart',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ productId: productId, count: 1 }),
            success: function () {
                loadCart();
            }
        });
    });

    // Оформление заказа
    $('#purshare').on('click', function () {
        var addressId = 1; // Здесь должен быть выбранный адрес
        $.ajax({
            url: 'https://localhost:7008/Cart/Purshare',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ addressId: addressId }),
            success: function (data) {
                alert('Заказ оформлен!');
                loadCart();
            }
        });
    });
});

initMap();

async function initMap() {
    // Промис `ymaps3.ready` будет зарезолвлен, когда загрузятся все компоненты основного модуля API
    await ymaps3.ready;

    const { YMap, YMapDefaultSchemeLayer } = ymaps3;

    // Иницилиазируем карту
    const map = new YMap(
        // Передаём ссылку на HTMLElement контейнера
        document.getElementById('map'),
        // Передаём параметры инициализации карты
        {
            // Координаты центра карты
            center: [37.588144, 55.733842],
            // Уровень масштабирования
            zoom: 10,
            controls: ['searchControl']
        },
        {
        searchControlProvider: 'yandex#search'
        }
    );

    // Добавляем слой для отображения схематической карты
    map.addChild(new YMapDefaultSchemeLayer());
}