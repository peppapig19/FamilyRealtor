$('#passwordButton').on('click', function () {
    var passwordInput = $('#Password');
    var passwordFieldType = passwordInput.attr('type');

    if (passwordFieldType === 'password') {
        passwordInput.attr('type', 'text');
        $(this).html('<i class="fa fa-eye-slash"></i>');
    } else {
        passwordInput.attr('type', 'password');
        $(this).html('<i class="fa fa-eye"></i>');
    }
});

var rating = (function () {
    $('#star-rating .star-element').on('click', function () {
        var newRating = $(this).data('rating');
        $('#Rating').val(newRating);

        $('#star-rating .star-element').each(function () {
            if ($(this).data('rating') <= newRating) {
                $(this).children('i').removeClass('fa-regular').addClass('fa text-warning');
            } else {
                $(this).children('i').removeClass('fa text-warning').addClass('fa-regular');
            }
        });
    });
})();

var initPlaceDropdowns = function (countryDropdown, cityDropdown) {
    if (countryDropdown.find(':selected').val() != '') loadCities();
    countryDropdown.on('change', loadCities);

    function loadCities() {
        var selectedCountryId = countryDropdown.find(':selected').val();

        $.ajax({
            url: '/Home/GetCitiesByCountry',
            type: 'GET',
            data: { countryId: selectedCountryId },
            success: function (response) {
                cityDropdown.empty();

                if (response.length > 0) {
                    cityDropdown.append($('<option>').val('').text("Выбрать город..."));

                    $.each(response, function (_, city) {
                        cityDropdown.append($('<option>').val(city.id).text(city.name));
                    });

                    var selectedCityId = $('#requestCity').val();

                    if (selectedCityId != '') {
                        cityDropdown.val(selectedCityId);
                        $('#requestCity').remove();
                    }
                }
                else cityDropdown.append($('<option>').val('').text("Выберите страну..."));
            }
        });
    }
};

initPlaceDropdowns($('#sCountry'), $('#sCity'));
initPlaceDropdowns($('#CountryId'), $('#CityId'));