function GetFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

function isFormDirty(e)
{
    //Enter	13
    //Up arrow	38
    //Down arrow	40
    //Left arrow	37
    //Right arrow	39
    //Escape	27
    //Spacebar	32
    //Ctrl	17
    //Alt	18
    //Tab	9
    //Shift	16
    //Caps - lock	20
    //Windows key	91
    //Windows option key	93
    //Backspace	8
    //Home	36
    //End	35
    //Insert	45
    //Delete	46
    //Page Up	33
    //Page Down	34
    //Numlock	144
    //F1 - F12	112 - 123
    //Print - screen ??
    //Scroll - lock	145
    //Pause -break	19

    var result = false;
    var excludedKeys = [13, 38, 40, 37, 39, 27, 32, 17, 18, 9, 16, 20, 91, 93, 8, 36, 35, 45, 46, 33, 34, 144, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 145, 19];

    for (var i = 0; i < excludedKeys.length; i++) {
        if (excludedKeys[i] != e.keyCode)
        {
            result = true;
            break;
        }
    }

    return result;
}

function cdbl($number)
{
    return parseFloat($number.replace(/,/g, ''))
}