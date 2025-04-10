
// Добавление товара в корзину
$(document).ready(function () {

    updateButtons();
    $(/*'.add-to-cart-btn'*/'.rect-btn').off('click').on('click', function () {
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
            url: '/Cart/UpdateCartItemCount',
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
            url: '/Cart/UpdateCartItemCount',
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
            url: '/Cart/UpdateCartItemCount',
            type: 'POST',
            data: { productId: productId, change: -1 },
            success: function () {
                console.log("Товар удален из корзины");
            }
        });
    });
});