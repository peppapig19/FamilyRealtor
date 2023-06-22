$('#DOB-datepicker').datepicker({
    language: 'ru',
    format: 'dd.mm.yyyy',
    endDate: new Date()
});

var initBookingDatepickers = (function (checkInDatepicker, checkOutDatepicker, disableBookedDates) {
    checkInDatepicker.datepicker({
        language: 'ru',
        format: 'dd.mm.yyyy',
        startDate: new Date()
    });

    checkOutDatepicker.datepicker({
        language: 'ru',
        format: 'dd.mm.yyyy'
    });

    if (disableBookedDates) {
        var bookedDates = [];
        var id = $('#rental-id').val();

        $.get('/Booking/GetBookedDates', { rentalId: id }, function (response) {
            bookedDates = response;

            checkInDatepicker.datepicker('setDatesDisabled', bookedDates);
            checkOutDatepicker.datepicker('setDatesDisabled', bookedDates);
        });
    };

    function setDefaultDates() {
        var today = new Date();
        var tomorrow = new Date();
        tomorrow.setDate(today.getDate() + 1);

        checkInDatepicker.datepicker('setDate', today);
        checkOutDatepicker.datepicker('setDate', tomorrow);
        setCheckOutStartDate();
    }

    function setCheckOutStartDate() {
        var cid = checkInDatepicker.datepicker('getDate');
        var cod = checkOutDatepicker.datepicker('getDate');
        var newCid = new Date(cid);
        newCid.setDate(newCid.getDate() + 1);

        checkOutDatepicker.datepicker('setStartDate', newCid);
        if (cid >= cod) checkOutDatepicker.datepicker('setDate', newCid);
    }

    var areDatesSet = checkInDatepicker.datepicker('getDate') && checkOutDatepicker.datepicker('getDate');

    if (areDatesSet) setCheckOutStartDate();
    else setDefaultDates();

    checkInDatepicker.on('change', function () {
        setCheckOutStartDate(checkInDatepicker, checkOutDatepicker);
        $('#CheckOutDate').valid();
    });
});