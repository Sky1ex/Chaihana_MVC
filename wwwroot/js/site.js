// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
/*<script src="~/lib/jquery/dist/jquery.min.js"></script>*/
$(document).ready(function () {
    // Обработка нажатия кнопки "Добавить в корзину"
    $('.add-to-cart-btn').click(function () {
        var productCard = $(this).closest('.product-card');
        var productId = productCard.data('product-id');

        // Добавляем анимацию
        $(this).addClass('added');
        setTimeout(() => {
            $(this).removeClass('added');
        }, 500);

        // Отправляем данные на сервер (AJAX)
        $.ajax({
            url: '/Cart/AddToCart', // Укажите правильный URL
            method: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    alert('Товар добавлен в корзину!');
                } else {
                    alert('Ошибка при добавлении товара.');
                }
            },
            error: function () {
                alert('Ошибка при отправке запроса.');
            }
        });
    });
});
