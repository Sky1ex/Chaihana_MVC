
$(document).ready(function () {
    // Обработка нажатия кнопки "Добавить в корзину"
    $('.add-to-cart-btn').click(function () {
        var productCard = $(this).closest('.product-card');

        var productId = productCard.find('.product-id').text().trim();

        // Добавляем анимацию
        $(this).addClass('added');
        setTimeout(() => {
            $(this).removeClass('added');
        }, 500);



        // Отправляем данные на сервер (AJAX)
        $.ajax({
            url: 'https://localhost:7008/Cart/AddToCart', // Укажите правильный URL
            method: 'POST',
            contentType: 'application/json', // Указываем, что отправляем JSON
            data: JSON.stringify({ // Преобразуем данные в JSON
                ProductId: productId,
                Count: 1
            }),
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

/*document.addEventListener("DOMContentLoaded", () => {

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


});*/

document.addEventListener("DOMContentLoaded", () => {
    const cartButton = document.getElementById("view-cart");
    const cartModal = document.getElementById("cartModal");
    const closeButton = document.querySelector(".close");
    const cartContent = document.getElementById("cartContent"); // Элемент для отображения содержимого корзины

    if (!cartModal || !cartButton || !closeButton || !cartContent) {
        console.error("Один из элементов не найден!");
        return;
    }

    cartButton.addEventListener("click", () => {
        // Запрашиваем данные с сервера (AJAX)
        $.ajax({
            url: 'https://localhost:7008/Cart/ShowCart', // Укажите правильный URL
            method: 'GET',
            success: function (response) {
                if (response.products && response.products.length > 0) {
                    // Очищаем содержимое корзины
                    cartContent.innerHTML = "";

                    // Добавляем каждый продукт в корзину
                    response.products.forEach(product => {
                        const productElement = document.createElement("div");
                        productElement.className = "cart-item";
                        productElement.innerHTML = `
                            <span>${product.name}</span>
                            <span>${product.price} ₽</span>
                            <span>Количество: ${product.count}</span>
                        `;
                        cartContent.appendChild(productElement);
                    });

                    // Показываем модальное окно
                    cartModal.style.display = "block";
                } else {
                    alert("Корзина пуста.");
                }
            },
            error: function () {
                alert('Ошибка при загрузке корзины.');
            }
        });
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




