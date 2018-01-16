var sectionSwitch = true;
var currentSubCommnetForm = "#formCevap_0";
var trackCommnet = 0;
var trackSubComment = 0;
$("#moreComments").click(function () {
    if (trackCommnet % 2 === 0) {
        $(".no-display-comment").show(1000);
        $("#moreComments").text("Daha az goster");
        trackCommnet++;
    } else {
        $(".no-display-comment").hide(1000);
        $("#moreComments").text("Daha fazla goster");
        trackCommnet++;
    }
    
});


$("#moreSubComments").click(function() {
    if (trackSubComment % 2 === 0) {
        $(".no-display-subcomment").show(1000);
        $("#moreSubComments").text("Daha az goster");
        trackSubComment++;
    } else {
        $(".no-display-subcomment").hide(1000);
        $("#moreSubComments").text("Daha fazla goster");
        trackSubComment++;
    }
});


$(document).on('focusout',
    '#SubComment',
    function() {
        validateSubCommnet();
    });

$(document).on('click',
    '#btnSubComment',
    function() {
        $(currentSubCommnetForm).ajaxForm({
            beforeSend: function(xhr) {
                var checkCorrrect = validateSubCommnet();
                if (!checkCorrrect) {
                    xhr.abort();
                } else {
                    $("#subCommentLoading").show(300);
                    $("#btnSubComment").attr('disabled', true);
                }
                
            },
            complete: function (xhr) {
                if (xhr.responseText.length > 0) {
                    var insertSection = currentSubCommnetForm.split('_');
                    var subCommentId = $(xhr.responseText).attr('id');
                    console.log(subCommentId);
                    $(".subCommentMenu_" + insertSection[1]).append(xhr.responseText);
                    $("#" + subCommentId).fadeIn(1000);
                    $("#subCommentLoading").hide();
                    $("#btnSubComment").attr('disabled', false);
                    $("#SubComment").val("");
                } else {
                    alert("bir seyler ters gitti");
                    $("#subCommentLoading").hide();
                    $("#btnSubComment").attr('disabled', false);
                }

            },
            error: function() {
                alert("bir seyler ters gitti");
                $("#subCommentLoading").hide();
            }
        });
    }
    );


function validateSubCommnet() {
    var subComment = $("#SubComment").val();
    if (!subComment) {
        $("#SubCommentError").text("");
        $("#SubCommentError").text("Bu alan gereklidir");
        return false;
    }
    else if (subComment.length <= 10) {
        $("#SubCommentError").text("");
        $("#SubCommentError").text("Mesajınızın uzunlugu minimum 10 karakter olmalıdır.");
        return false;
    } else if (subComment.length > 280) {
        $("#SubCommentError").text("");
        $("#SubCommentError").text("Mesajınızın uzunlugu 280 karakteri geçmemelidir.");
        return false;
    }
    $("#SubCommentError").text("");
    return true;
}

function yanitla(parent, replyTo) {
    $(currentSubCommnetForm).remove();
    var form = "<form method='post' action='/Blog/Post/YorumCevap/' id='formCevap_" +
        parent+
        "'" +
        "style='margin-top:15px'>" +
        "<strong id='subCommentLoading' class='text-warning' style='font-size:15px;display:none'>Yukleniyor...</strong>"+
        "<h4><b>Cevap Yazın</b></h4>" +
        "<div class='form-horizontal'>" +
            "<div class='form-group'>" +
                "<textarea class='form-control text-box multi-line'" +
                "id='SubComment' name='SubComment' placeholder= 'Yanıtınız' ></textarea>" +
                "<span class='text-danger' id='SubCommentError'" +
                "style='font-size:14px'></span>" +
                "<input type='hidden' value='"+parent+"' id='parentContainer' name='parentContainer' />"+
                "<input type='hidden' value='"+replyTo+"' id='relpyContainer' name='replyContainer'/>"+
            "</div>" +
            "<div class='form-group'>" +
                "<input type='submit' id='btnSubComment' value='Gonder' class='btn btn-primary'>" +
            "</div>" +
        "</div>" +
        "</form>";
    $(form).insertAfter("#yorum_" + parent);
    currentSubCommnetForm = "#formCevap_" + parent;
    scrollTo("#formCevap_" + parent);
}

function scrollTo(selector) {
    var offset = $(selector).offset();
    var $main = $('#main');
    $main.animate({
        scrollTop: offset.top - ($main.offset().top - $main.scrollTop()) - 200
    }, 1500);
}

function altYorumSilBaslat(response) {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        url: '/Blog/Post/AltYorumSil',
        data: {
            silinecekId: response,
            __RequestVerificationToken: token
        },
        method: "POST",
        beforeSend: function() {

        }
    }).done(function(response) {
        if (response === 'basarisiz') {
            alert("bir seyler ters gitti");
        } else {
            $(currentSubCommnetForm).remove();
            $("#" + response).fadeOut(300,function() {
                $("#" + response).remove();
                alert("Yorum silindi");
            });


        }
    }).fail(function() {
        alert("bir seyler ters gitti");
    }).always(function() {

    });
}


function yorumSilBaslat(Id) {

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        url: "/Blog/Post/YorumSil",
        data: {
            silinecekId: Id,
            __RequestVerificationToken: token
        },
        method: "POST",
        beforeSend: function() {
            $("#successCommentAdd").hide();
        }
    }).done(function(response) {
        if (response === "basarisiz") {
            alert("bir seyler ters gitti");
        } else {
            $(currentSubCommnetForm).remove();
            $("#" + response).fadeOut(300,
                function() {
                    $("#" + response).remove();
                    alert("Yorum(lar) silindi");
                });
        }
    }).fail(function() {
        alert("bir seyler ters gitti");
    }).always(function() {
    });
}

function newCommentSuccess(response) {
    if (response === '') {
        alert("yorum uygun formatta degil");
    } else {
        $("#noComment").remove();
        $("#Description").val("");
        $("#successCommentAdd").show(1000,
            function() {
                scrollTo("#endOfComments");
            });

    }
    
}
function newCommentComplete() {
    $("#yorumEkleBtn").attr('disabled', false);
}

function newCommentBegin() {
    $("#successCommentAdd").hide();
    $("#yorumEkleBtn").attr('disabled', true);
}
function newCommentFail() {
    alert("Bir seyler ters gitti");
}

function bultenKayit() {
    if ($('#takipciler').is(':visible')) {
        $("#takipciler").hide(300);
    } else {
        $("#takipciler").show(300);
    }
}

function emailTakip_fail() {
    alert("Bir seyler ters gitti .....");
}

function openSection1() {
    $('.home-page-posts').removeClass("hide");
    $('.home-page-categories').addClass("hide");
    $('.home-page-update').addClass("hide");

    $('.select-posts').addClass("active");
    $('.select-categories').removeClass("active");
    $('.select-update').removeClass("active");

    $('.home-footer').toggleClass("hide");
}

function openSection2() {
    $('.home-page-categories').removeClass("hide");
    $('.home-page-posts').addClass("hide");
    $('.home-page-update').addClass("hide");

    $('.select-categories').addClass("active");
    $('.select-posts').removeClass("active");
    $('.select-update').removeClass("active");

    $('.home-footer').toggleClass("hide");
}

function openSection3() {
    $('.home-page-update').removeClass("hide");
    $('.home-page-categories').addClass("hide");
    $('.home-page-posts').addClass("hide");
    $('.select-update').addClass("active");
    $('.select-categories').removeClass("active");
    $('.select-posts').removeClass("active");

    $('.home-footer').toggleClass("hide");
}

function emailTakip_success(response) {
    if (response === 'basarili') {
        $("#loadingTakipciEmail").hide();
        $("#emailResponse").show(300);
        setTimeout(function() {
                $("#loadingTakipciEmail").hide();
                $("#emailResponse").hide();
                bultenKayit();
            },
            3000);
    } else if (response === 'zaten var') {

        $("#loadingTakipciEmail").hide();
        $("#emailResponse").hide();
        alert('Bu mail adresi zaten kayitli');

    } else if (response === 'Cok fazla deneme yaptınız biraz dinlenin') {
        $("#loadingTakipciEmail").hide();
        $("#emailResponse").hide();
        alert("Cok fazla deneme yaptınız biraz dinlenin");
    }
    else {
        $("#loadingTakipciEmail").hide();
        alert("Lütfen Düzgün formatta mail adersi girin");
    }
}

var s,
    app = {

        settings : {
            jpm: {}
        },
        init: function() {
            //Global settings
            s = this.settings;

            // initalize
            this.initalizers();
            this.bindUiActions();
        },
        bindUiActions: function (){
            // Should include all JS user interactions
            var self = this;

            $('.select-posts').on('click',
                function () {
                    self.homePostsSwitch();
                });
            $('.select-categories').on('click',
                function () {
                    self.homeCategoriesSwitch();
                });
            $('.select-update').on('click',
                function () {
                    self.homeUpdateSwitch();
                });

            $('.social-icon').on('click', function(){
                self.socialIconClick( $(this) );
            });
        },
        initalizers: function (){
            // Initalize any plugins for functions when page loads

            // JPanel Menu Plugin -
            this.jpm();

            // Fast Click for Mobile - removes 300ms delay - https://github.com/ftlabs/fastclick
            FastClick.attach(document.body);

            // Add Bg colour from JS so jPanel has time to initalize
            $('body').css({"background-color":"#333337"});
        }, homePostsSwitch: function () {
            if (sectionSwitch) {
                openSection1();
            }
        },homeCategoriesSwitch: function() {
            if (sectionSwitch) {
                openSection2();
            }
        },
        homeUpdateSwitch: function(){
            if (sectionSwitch) {
                openSection3();
            }
        },
        socialIconClick: function(el) {
            // Post page social Icons
            // When Clicked pop up a share dialog

            var platform = el.data('platform');
            var message = el.data('message');
            var url = el.data('url');

            if (platform == 'mail'){
                // Let mail use default browser behaviour
                return true;
            } else {
                this.popItUp(platform, message, url);
                return false;
            }
        },
        popItUp : function (platform, message, url) {
            // Create the popup with the correct location URL for sharing
            var popUrl,
                newWindow;

            if( platform == 'twitter'){
                popUrl = 'http://twitter.com/home?status=' + encodeURI(message) + '+' + url;

            } else if(platform == 'facebook'){
                popUrl = 'http://www.facebook.com/share.php?u=' + url + '&amp;title=' + encodeURI(message);
            }
            newWindow = window.open(popUrl,'name','height=500,width=600');
            if (window.focus) { newWindow.focus(); }
            return false;

        },
        jpm: function(){
            // Off Screen Navigation Plugin

            s.jpm = $.jPanelMenu({
                menu : '#menu-target',
                trigger: '.menu-trigger',
                animated: false,
                beforeOpen : ( function() {
                    if (matchMedia('only screen and (min-width: 992px)').matches) {
                        $('.sidebar').css("left", "250px");
                    }
                }),
                beforeClose : ( function() {
                    $('.sidebar').css("left", "0");
                    $('.writer-icon, .side-writer-icon').removeClass("fadeOutUp");
                })
            });

            s.jpm.on();
        }
    };

$(document).ready(function(){
    app.init();
});
