
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

    /*// Оформление выбранных товаров
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
    });*/

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

        var queryParams = selectedProducts.map(id => 'products=' + encodeURIComponent(id)).join('&');

        window.location.href = '/Cart/CheckoutSelected?' + queryParams;
    });
});