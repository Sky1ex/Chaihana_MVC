var currentId;

var currentBookingId;

var currentPhone;

var started = false;

function Timer(time) {
    if (started) return;

    started = true;
    document.querySelector('#send-again').className = 'send-code-again denied';
    var timer = setInterval(() => {
        document.querySelector('.code-timer').textContent = time;
        if (time <= 0) {
            document.querySelector('#send-again').className = 'send-code-again';
            clearInterval(timer)
            started = false;
        }
        else time--;
    }, 1000);
}

$(document).ready(function () {

    $('.change-button').on('click', function () {
        $('.map-modal').css('display', 'grid');
        currentId = $(this).attr('id');
        var button = document.querySelector('#send-address-data');
        if (button != null) button.id = 'update-address-data';
        var fullAddress = $(this).closest('.full-address');
        fullAddress = fullAddress.text().split(', ');
        fullAddress = fullAddress.map(x => x.trimStart() );
        fullAddress[3] = fullAddress[3].replace('\n\t\t\t\tИзменить\n\t\t\t', '');
        document.querySelector('#city').value = fullAddress[0];
        document.querySelector('#street').value = fullAddress[1];
        document.querySelector('#house').value = fullAddress[2];
        document.querySelector('#apartment').value = fullAddress[3];
    });

    $(document).on('click', '.add-button', function () {
        $('.map-modal').css('display', 'grid');
        var button = document.querySelector('#update-address-data');
        if (button != null) button.id = 'send-address-data';
        document.querySelector('#city').value = '';
        document.querySelector('#street').value = '';
        document.querySelector('#house').value = '';
        document.querySelector('#apartment').value = '';
    });

    $(document).on('click', '#send-address-data', function () {

        var city = document.querySelector('#city').value;
        var street = document.querySelector('#street').value;
        var house = document.querySelector('#house').value;
        var apartment = document.querySelector('#apartment').value;

        $.ajax({
            url: '/Api/Account/AddAddress',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ City: city, Street: street, House: house, Apartment: apartment }),
            success: function () {
                console.log('Адрес сохранен!');
                window.location.reload();
            },
            error: function () {
                alert('Укажите правильный адресс доставки!');
            }
        });
    });

    $(document).on('click', '#update-address-data', function () {

        var city = document.querySelector('#city').value;
        var street = document.querySelector('#street').value;
        var house = document.querySelector('#house').value;
        var apartment = document.querySelector('#apartment').value;

        $.ajax({
            url: '/Api/Account/PutAddress',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify({ City: city, Street: street, House: house, Apartment: apartment, AddressId: currentId }),
            success: function () {
                console.log('Адрес обновлен!');
                window.location.reload();
            },
            error: function () {
                alert('Укажите правильный адресс доставки!');
            }
        });
    });

    $(document).on('click', '.delete-button', function () {

        currentId = $(this).closest('.delete-button').attr('id');

        $.ajax({
            url: '/Api/Account/DeleteAddress',
            type: 'DELETE',
            data: { addressId: currentId },
            success: function () {
                console.log('Адрес удален!');
                window.location.reload();
            },
        });
    });

    $(document).on('click', '.booking-delete-button', function () {

        currentBookingId = $(this).attr('id');

        $.ajax({
            url: '/Api/Booking/Delete',
            type: 'DELETE',
            data: { bookingId: currentBookingId },
            success: function () {
                console.log('бронирование удалено!');
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
        $('.map-modal').css('display', 'none');
        $('.phone-modal').css('display', 'none');
    });

    $(document).on('click', '#send-phone', function () {
        $('.phone-modal').css('display', 'grid');
    });

    $(document).on('click', '.send-phone-data', function () {

        currentPhone = document.querySelector("#phone-input").value;

        $.ajax({
            url: '/Api/Account/GetCode',
            type: 'GET',
            data: { userNumber: currentPhone },
            success: function () {
                $('#content-send-phone').css('display', 'none');
                Timer(119);
                $('#content-send-code').css('display', 'block');
                console.log('телефон отправлен!');
            },
        });
    });

    // Нужно в будущем удалить
    $(document).on('click', '.send-code-again', function () {

        $.ajax({
            url: '/Api/Account/GetCode',
            type: 'GET',
            data: { userNumber: currentPhone },
            success: function () {
                Timer(119);
                console.log('код отправлен еще раз!');
            },
        });
    });

    $(document).on('click', '.send-code-data', function () {

        var number = document.querySelector("#code-input").value;

        $.ajax({
            url: '/Api/Account/CheckCode',
            type: 'POST',
            data: { code: number },
            success: function (answer) {
                if (answer == true)
                {
                    $('#content-send-code').css('display', 'none');
                    $('#content-send-name').css('display', 'block');
                }
                else if (answer == false) alert("Неправильный код");
                else {
                    alert("Вы уже были зарегистрированы");
                    $('#content-send-code').css('display', 'none');
                    window.location.reload();
                }
                console.log('телефон сохранен!');
            },
        });
    });

    $(document).on('click', '.send-name-data', function () {

        var number = document.querySelector("#name-input").value;

        $.ajax({
            url: '/Api/Account/AddName',
            type: 'POST',
            data: { name: number },
            success: function () {
                $('#content-send-name').css('display', 'none');
                console.log('имя сохранено!');
                window.location.reload();
            },
        });
    });
});

