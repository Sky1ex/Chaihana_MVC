$(document).ready(function ()
{
    // Обновление количества товара в корзине
    $(document).on('click', '.button-accept', function () {

        const selectedProducts = [];
        document.querySelectorAll('.mini-card').forEach(el => {
            selectedProducts.push(el.id);
        });

        var address = document.querySelector('.address-container').id;

        $.ajax({
            url: '/Api/Cart/Purshare',
            type: 'POST',
            dataType: 'application/json',
            data: { orderElements: selectedProducts, addressId: address },
            success: function (data) {
                alert('Заказ оформлен!');
                window.location.href = '/Menu'
            }
        });
    });

    $(document).on('click', '.button-back', function () {
        window.location.href = '/Menu'
    });
});