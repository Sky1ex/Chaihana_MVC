
$(document).ready(function () {


    // Увеличение количества товара (делегирование событий)
    $(document).on('click', '.increase-quantity', function () {
        var productId = $(this).data('product-id');
        updateCartItemQuantity(productId, 1);
        console.log("Функция увеличения товара");
    });

    // Уменьшение количества товара (делегирование событий)
    $(document).on('click', '.decrease-quantity', function () {
        var productId = $(this).data('product-id');
        updateCartItemQuantity(productId, -1);
        console.log("Функция уменьшения товара");
    });

    // Обновление количества товара в корзине
    function updateCartItemQuantity(productId, change) {
        $.ajax({
            url: '/Api/Cart/UpdateCartItemCount',
            type: 'POST',
            data: { productId: productId, change: change },
            success: function () {
                loadCart();
                updateButton(productId, change);
            }
        });
    }

    // Оформление выбранных товаров
    $(document).on('click', '#checkout-selected', function () {
        var selectedProducts = [];
        $('.cart-item-checkbox:checked').each(function () {
            selectedProducts.push($(this).data('product-id'));
        });

        var selectedAddressId = $('input[name="selectedAddress"]:checked').val();

        if (selectedProducts.length === 0) {
            alert('Выберите товары для оформления!');
            return;
        }

        if (!selectedAddressId) {
            alert('Выберите адрес доставки!');
            return;
        }

        $.ajax({
            url: '/Api/Cart/CheckoutSelected',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ productIds: selectedProducts, addressId: selectedAddressId }),
            success: function (data) {
                alert('Заказ оформлен!');
                loadCart();
                console.log("Функция оформления выбранной части заказа");
            }
        });
    });

    // Оформление заказа
    /*$('#purshare').on('click', function () {
        var addressId = $('input[name="selectedAddress"]:checked').val();
        $.ajax({
            url: '/Cart/Purshare',
            type: 'POST',
            data: { addressId: addressId },
            success: function (data) {
                alert('Заказ оформлен!');
                loadCart();
                console.log("Функция оформления заказа");
            }
        });
    });*/
});

/*function updateButtons() {
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
}*/