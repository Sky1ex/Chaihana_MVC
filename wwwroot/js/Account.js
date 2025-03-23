$(document).on('click', '#send-address-data', function () {

    const elem = document.querySelector('.address-data');

    array = elem.textContent.split(',');

    $.ajax({
        url: '/Account/AddAddress',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ City: array[1], Street: array[2], House: array[3] }),
        success: function () {
            console.log('Адрес сохранен!');
            window.location.reload();
        },
        error: function () {
            alert('Укажите правильный адресс доставки!');
        }
    });
});

$(document).on('click', '#delete-address-data', function () {

    const elem = document.querySelector('.Adress-id');

    $.ajax({
        url: '/Account/DeleteAddress',
        type: 'DELETE',
        data: { addressId: elem.textContent },
        success: function () {
            console.log('Адрес удален!');
            window.location.reload();
        },
    });
});

$(document).on('click', '#send-data', function () {
    $('#phoneModal').css('display', 'block');
    console.log("Функция сохранения телефона");
});

$(document).on('click', '.close', function () {
    $('#phoneModal').css('display', 'none');
    $('#codeModal').css('display', 'none');
    $('#nameModal').css('display', 'none');
});

$(document).on('click', '#send-phone', function () {

    var number = document.querySelector(".number").value;

    $('#phoneModal').css('display', 'none');
    $('#codeModal').css('display', 'block');

    $.ajax({
        url: '/Account/GetCode',
        type: 'GET',
        data: { userNumber: number },
        success: function () {
            console.log('телефон отправлен!');
        },
    });
});

$(document).on('click', '#send-code', function () {

    var number = document.querySelector(".code").value;

    $('#codeModal').css('display', 'none');

    $.ajax({
        url: '/Account/CheckCode',
        type: 'POST',
        data: { code: number },
        success: function (answer) {
            if (answer == true) $('#nameModal').css('display', 'block');
            else if (answer == false) alert("Неправильный код");
            else {
                alert("Вы уже были зарегистрированы");
                window.location.reload();
            }
            console.log('телефон сохранен!');
        },
    });
});

$(document).on('click', '#send-name', function () {

    var number = document.querySelector(".name").value;

    $('#nameModal').css('display', 'none');

    $.ajax({
        url: '/Account/AddName',
        type: 'POST',
        data: { name: number },
        success: function () {
            console.log('имя сохранено!');
            window.location.reload();
        },
    });
});