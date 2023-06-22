function GetMap() {
    var latitude = parseFloat($('#map').data('latitude').replace(',', '.'));
    var longitude = parseFloat($('#map').data('longitude').replace(',', '.'));
    var street = $('#map').data('street');

    var map = new Microsoft.Maps.Map('#map', {
        center: new Microsoft.Maps.Location(latitude, longitude),
        zoom: 13
    });

    var center = map.getCenter();

    var pin = new Microsoft.Maps.Pushpin(center, {
        title: street
    });

    map.entities.push(pin);
}

var booking = (function () {
    function initBookingForm() {
        var checkInDatepicker = $('#booking-datepicker-from');
        var checkOutDatepicker = $('#booking-datepicker-to');

        initBookingDatepickers(checkInDatepicker, checkOutDatepicker, true);
        $.validator.unobtrusive.parse($('#booking-card'));

        $('#booking-datepicker-from, #booking-datepicker-to').on('change', function () {
            var cid = checkInDatepicker.datepicker('getDate');
            var cod = checkOutDatepicker.datepicker('getDate');
            var days = (cod - cid) / (1000 * 60 * 60 * 24);

            var finalPrice = parseInt($('#final-price').val());
            var totalPrice = Math.ceil(finalPrice * days);

            var price = parseInt($('#price').val());
            var oldTotalPrice = Math.ceil(price * days);

            $('#total-price').html(totalPrice + ' <span>&#8381;</span>');
            $('#old-total-price').html(oldTotalPrice + ' <span>&#8381;</span>');
        });
    }

    function loadBookingForm(e) {
        if (e.matches) {
            $('#booking-desktop-card').empty();
            $('#booking-mobile-card').load('/Booking/GetBookingPartial', { rentalId: $('#Id').val() }, function () {
                initBookingForm()
            });
        }
        else {
            $('#booking-mobile-card').empty();
            $('#booking-desktop-card').load('/Booking/GetBookingPartial', { rentalId: $('#Id').val() }, function () {
                initBookingForm()
            });
        }
    }

    var isNarrow = window.matchMedia("(max-width: 991px)");
    loadBookingForm(isNarrow);
    isNarrow.addEventListener('change', loadBookingForm);

    $('#booking-mobile-card, #booking-desktop-card').on('submit', '#booking-form', function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.valid) window.location.href = response.content;
                else {
                    $('#booking-card').parent().html(response.content);
                    initBookingForm();
                }
            },
            error: function () {
                alert('Во время бронирования произошла ошибка.');
            }
        });
    });
})();

var commenting = (function () {
    $('#review-form').on('submit', function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                $('#reviews-card').html(response);
                $.validator.unobtrusive.parse('#review-form');
            },
            error: function () {
                alert('Во время публикации отзыва произошла ошибка.');
            }
        });
    });
})();

var scrolling = (function () {
    var $anchor = $('a[href^="#"]');

    $anchor.on('click', function () {
        $(this).attr('href').scrollIntoView({
            behavior: 'smooth'
        });
    });
})();