
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

//Кусок кода отвечающий за открытие/корзины в виде модального окна

document.addEventListener("DOMContentLoaded", () => {

    const cartButton = document.getElementById("view-cart");
    const cartModal = document.getElementById("cartModal");
    const closeButton = document.querySelector(".close");

    if (!cartModal || !cartButton || !closeButton) {
        console.error("Один из элементов не найден!");
        return;
    }

    cartButton.addEventListener("click", () => {
        cartModal.style.display = "block";
    });

    closeButton.addEventListener("click", () => {
        cartModal.style.display = "none";
    });

    window.addEventListener("click", (event) => {
        if (event.target === cartModal) {
            cartModal.style.display = "none";
        }
    });


});




