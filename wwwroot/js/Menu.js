// Добавление товара в корзину
$(document).ready(function () {
    $('.add-to-cart-btn').on('click', function () {
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
                /*loadCart();*/
                console.log("Функция добавления товара в корзину");
            }
        });
    });
});