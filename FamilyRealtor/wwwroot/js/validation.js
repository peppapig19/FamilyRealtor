function checkType(str) {
    if (!isNaN(parseFloat(str)) && isFinite(str)) return 'number';
    if (/^\d{2}\.\d{2}\.\d{4}$/.test(str)) return 'date';
    return 'unknown';
}

function createDate(str) {
    var dateParts = str.split('.');
    return new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
}

$.validator.addMethod('ageRange', function (value, _, _) {
    var DOB = createDate(value);
    if (isNaN(DOB)) return false;

    var min = 18;
    var max = 100;
    var now = new Date();

    var olderThan = new Date();
    var minYear = now.getFullYear() - min;
    olderThan.setFullYear(minYear);

    var youngerThan = new Date();
    var maxYear = now.getFullYear() - max;
    youngerThan.setFullYear(maxYear);

    if (DOB < olderThan && DOB > youngerThan) return true;
    else return false;
});

$.validator.unobtrusive.adapters.addBool('ageRange');

$.validator.addMethod('greaterThan', function (value, _, params) {
    var target = $(params).val();

    if (checkType(value) == 'date' && checkType(target) == 'date') return createDate(value) >= createDate(target);
    if (checkType(value) == 'number' && checkType(target) == 'number') return Number(value) >= Number(target);
    return true;
});

$.validator.unobtrusive.adapters.add('greaterThan', ['property'], function (options) {
    options.rules['greaterThan'] = '#' + options.params['property'];
    options.messages['greaterThan'] = options.message;
});