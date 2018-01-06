$(document).ready(function() {

});

function  bultenKayit() {
    if ($('#takipciler').is(':visible')) {
        $("#takipciler").hide(300);
    } else {
        $("#takipciler").show(300);
    }
}

function emailTakip_fail() {
    alert("Bir seyler ters gitti .....");
}

function emailTakip_success(response) {
    if (response === 'basarili') {
        $("#loadingTakipciEmail").hide();
        $("#emailResponse").show(300);
        setTimeout(function() {
                $("#loadingTakipciEmail").hide();
                $("#email").val("");
                $("#emailResponse").hide(0);
                bultenKayit();
            },
            3000);
    } else if (response === 'zaten var') {

        $("#loadingTakipciEmail").hide();
        $("#email").val("");
        $("#emailResponse").hide();
        alert('Bu mail adresi zaten kayitli');

    } else {
        $("#loadingTakipciEmail").hide();
        alert("Lütfen Düzgün formatta mail adersi girin");
    }
}