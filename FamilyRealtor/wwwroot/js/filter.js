function initFilterModal() {
    initPlaceDropdowns($('#CountryId'), $('#CityId'));
    initBookingDatepickers($('#filter-datepicker-from'), $('#filter-datepicker-to'));
    $.validator.unobtrusive.parse('#filterModal');

    $('#MinPriceADay').on('change', function () {
        $('#MaxPriceADay').valid();
    });
}

$('#filterButton').on('click', function () {
    var selectedCountryId = $('#sCountry').find(':selected').val();
    var selectedCityId = $('#sCity').find(':selected').val();

    $('#filterModal .modal-dialog').load('/Home/GetFilterPartial', { countryId: selectedCountryId, cityId: selectedCityId }, function () {
        initFilterModal();
        $('#filterModal').modal('show');
    });
});

$('#filterModal').on('submit', '#filterForm', function (e) {
    e.preventDefault();

    $.ajax({
        url: $(this).attr('action'),
        type: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            if (response.valid) window.location.href = response.content;
            else {
                $('#filterModal .modal-dialog').html(response.content);
                initFilterModal();
            }
        }
    });
});