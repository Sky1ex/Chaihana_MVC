

// Добавление товара в корзину
$(document).ready(function () {

    updateButtons();

    $('#view-cart').on('click', function () {
        $('#cartModal').css('display', 'block');
        loadCart();
        console.log("Функция открытия корзины");
    });

    $(document).on('click', '.close', function () {
        $('#cartModal').css('display', 'none');
        console.log("Функция закрытия корзины (работает для AJAX)");
    });

    $('.rect-btn').off('click').on('click', function () {
        var productCard = $(this).closest('.product-card');
        var productId = productCard.find('.product-id').text().trim();

        // Добавляем анимацию
        $(this).addClass('added');
        setTimeout(() => {
            $(this).removeClass('added');
        }, 500);

        var button = $(this).closest('.rect-btn');
        var quantityElement = button.find('.quantity');
        var quantity = parseInt(quantityElement.text());

        if(quantity == 0) $.ajax({
            url: '/Api/Cart/UpdateCartItemCount',
            type: 'POST',
            data: { productId: productId, change: 1 },
            success: function () {
                // Если количество равно 0, возвращаем кнопку в исходное состояние
                button.find(/*'.add-text'*/'.rect-btn-name').hide();
                button.find('.quantity-controls').show();
                quantityElement.text('1');
                console.log("Функция добавления товара в корзину");
            }
        });
    });

    // Обработка клика на плюс
    $(document).on('click', '.plus-btn', function (event) {
        event.stopPropagation(); // Останавливаем всплытие события

        var button = $(this).closest('.rect-btn');
        var productId = button.data('product-id');
        var quantityElement = button.find('.quantity');
        var quantity = parseInt(quantityElement.text());

        // Увеличиваем количество
        quantity += 1;
        quantityElement.text(quantity);

        // Отправляем запрос на обновление количества товара в корзине
        $.ajax({
            url: '/Api/Cart/UpdateCartItemCount',
            type: 'POST',
            data: { productId: productId, change: 1 },
            success: function () {
                console.log("Количество товара обновлено");
            }
        });
    });

    // Обработка клика на минус
    $(document).on('click', '.minus-btn', function (event) {
        event.stopPropagation(); // Останавливаем всплытие события

        var button = $(this).closest('.rect-btn');
        var productId = button.data('product-id');
        var quantityElement = button.find('.quantity');
        var quantity = parseInt(quantityElement.text());

        // Уменьшаем количество
        quantity -= 1;

        if (quantity <= 0) {
            // Если количество равно 0, возвращаем кнопку в исходное состояние
            button.find('.rect-btn-name').show();
            button.find('.quantity-controls').hide();
            quantityElement.text('0');
        } else {
            // Обновляем количество
            quantityElement.text(quantity);
        }
        // Отправляем запрос на удаление товара из корзины
        $.ajax({
            url: '/Api/Cart/UpdateCartItemCount',
            type: 'POST',
            data: { productId: productId, change: -1 },
            success: function () {
                console.log("Товар удален из корзины");
            }
        });
    });

    $(document).on('click', '.Category', function () {
        var elems = document.getElementsByClassName('visible-category');
        var elem = elems.namedItem($(this).attr('id'));
        elem.scrollIntoView(true);
    });
});

// Загрузка содержимого корзины
function loadCart() {
    $.ajax({
        url: '/Cart/ShowCart',
        type: 'GET',
        success: function (data) {
            $('#cartModal').html(data);
            console.log("Функция загрузки содержимого корзины");
        }
    });
}

function updateButtons() {
    $.ajax({
        url: '/Api/Cart/ShowCart', // URL для получения данных корзины
        type: 'GET',
        contentType: 'application/json',
        success: function (cartProducts) {
            // Проходим по каждому товару в корзине
            cartProducts.forEach(function (productInCart) {
                var productId = productInCart.productId; // Получаем ID товара
                var button = $('.rect-btn[data-product-id="' + productId + '"]'); // Находим кнопку по ID товара

                if (button.length > 0) {
                    // Если кнопка найдена, обновляем её состояние
                    if (productInCart.count > 0) {
                        // Если товар есть в корзине, показываем кнопку "Плюс/Минус"
                        button.find('.rect-btn-name').hide();
                        button.find('.quantity-controls').show();
                        button.find('.quantity').text(productInCart.count);
                    } else {
                        // Если товара нет в корзине, показываем кнопку "Добавить"
                        button.find('.rect-btn-name').show();
                        button.find('.quantity-controls').hide();
                    }
                }
            });
            console.log("Кнопки обновлены на основе данных корзины");
        },
        error: function (xhr, status, error) {
            console.error("Ошибка при получении данных корзины:", error);
        }
    });
}

function updateButton(productId, change) {
    
     // Проходим по каждому товару в корзине
            
    var button = $('.rect-btn[data-product-id="' + productId + '"]'); // Находим кнопку по ID товара

    var val = button.find('.quantity').text();

    val = parseInt(val) + change;

    // Если кнопка найдена, обновляем её состояние
    if (val == 0) {
        // Если товара нет в корзине, показываем кнопку "Добавить"
        button.find('.rect-btn-name').show();
        button.find('.quantity-controls').hide();
        button.find('.quantity').text(val);
    } else
    {
        // Если товара нет в корзине, показываем кнопку "Добавить"
        button.find('.rect-btn-name').hide();
        button.find('.quantity-controls').show();
        button.find('.quantity').text(val);
    }
    console.log("Кнопки обновлены на основе данных корзины");
        
}