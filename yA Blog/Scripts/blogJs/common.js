var sectionSwitch = true;

function newComment(response) {
    if (response === 'yorum uygun formatta degil') {
        alert("yorum uygun formatta degil");
    } else {
        $("#Description").val("");
        $("#successCommentAdd").show(300);
    }
    
}
function newCommentBegin() {
    $("#successCommentAdd").hide();
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
