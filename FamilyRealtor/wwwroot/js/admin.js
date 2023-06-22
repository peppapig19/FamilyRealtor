$('#PriceADay, #Discount').on('change', function () {
    var price = parseFloat($('#PriceADay').val());
    var discount = parseInt($('#Discount').val());

    if (isNaN(price)) {
        $('#final-price').text('');
        return;
    }

    if (isNaN(discount)) $('#final-price').text(price);
    else {
        var finalPrice = Math.ceil(price - (price * (discount / 100)));
        $('#final-price').text(finalPrice);
    }

    $('#final-price').append(' <span>&#8381;</span>');
});

$('#FormFiles').on('change', function () {
    $('#image-previews').empty();

    var files = this.files;

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var reader = new FileReader();

        reader.onload = (function (file) {
            return function (e) {
                var img = $('<img>');
                img.attr('class', 'image-preview');
                img.attr('src', e.target.result);
                img.attr('alt', file.name);

                $('#image-previews').append(img);
            };
        })(file);

        reader.readAsDataURL(file);
    }
});

var deleting = (function () {
    var path;

    $('.deleteButton').each(function () {
        $(this).on('click', function () {
            var name = $(this).data('name');
            path = $(this).data('path');

            $.get('/Admin/Home/GetDeletePartial', { Name: name }, function (response) {
                $('#deleteModal .modal-dialog').html(response);
                $('#deleteModal').modal('show');
            });
        });
    });

    $('#deleteModal').on('submit', '#deleteForm', function (e) {
        e.preventDefault();

        $.post(path, function (response) {
            window.location.replace(response);
        });
    });
})();

var datepickers = (function () {
    $('#datepicker-from').datepicker({
        language: 'ru',
        format: 'dd.mm.yyyy'
    }).datepicker('setDate', new Date());

    $('#datepicker-to').datepicker({
        language: 'ru',
        format: 'dd.mm.yyyy',
        defaultDate: new Date()
    }).datepicker('setDate', new Date());

    $('#datepicker-from').on('change', function () {
        var datepickerFrom = $(this);
        var datepickerTo = $('#datepicker-to');

        var dateFrom = datepickerFrom.datepicker('getDate');
        var dateTo = datepickerTo.datepicker('getDate');
        var newDateTo = new Date(dateFrom);

        datepickerTo.datepicker('setStartDate', newDateTo);
        if (dateFrom > dateTo) datepickerTo.datepicker('setDate', newDateTo);
    });
})();