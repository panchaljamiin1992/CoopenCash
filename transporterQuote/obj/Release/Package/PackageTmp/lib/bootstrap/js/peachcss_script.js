var top_menu_height = 0;
jQuery(function ($) {
    $(window).load(function () {
        $('.external-link').unbind('click');
    });

    $(document).ready(function () {

        // load google map
        //var script = document.createElement('script');
        //script.type = 'text/javascript';
        //script.src = 'https://maps.google.co.in/maps?q=Peach+Technovations+Pvt+Ltd.,+Gandhinagar,+Gujarat&hl=en&ll=23.242923,72.627375&spn=0.014807,0.022724&sll=22.416832,71.319661&sspn=7.622071,11.634521&oq=Peach+Technovations&t=m&z=16&iwloc=A';
        //document.body.appendChild(script);

       
            top_menu_height = $('.peachcss-top-menu').height();
            // scroll spy to auto active the nav item
            $('body').scrollspy({ offset:0 });
            $('.external-link').unbind('click');

            // scroll to top
            $('#btn-back-to-top').click(function (e) {
                e.preventDefault();
                scrollTo('#peachcss-top');
            });

            // scroll to specific id when click on menu
            $('.peachcss-top-menu .navbar-nav a').click(function (e) {
                e.preventDefault();
                var linkId = $(this).attr('href');
                scrollTo(linkId);
                if ($('.navbar-toggle').is(":visible") == true) {
                    $('.navbar-collapse').collapse('toggle');
                }
                $(this).blur();
                return false;
            });

            $('#newnav a').click(function (e) {

                e.preventDefault();
                var linkId = $(this).attr('href');
                scrollTo(linkId);
                if ($('.navbar-toggle').is(":visible") == true) {
                    $('.navbar-collapse').collapse('toggle');
                }
                $(this).blur();
                return false;
            });

            // to stick navbar on top
            $('.peachcss-top-menu ').stickUp();

            // gallery category
            $('.peachcss-gallery-category a').click(function (e) {
                e.preventDefault();
                $(this).parent().children('a').removeClass('active');
                $(this).addClass('active');
                var linkClass = $(this).attr('href');
                $('.gallery').each(function () {
                    if ($(this).is(":visible") == true) {
                        $(this).hide();
                    };
                });
                $(linkClass).fadeIn();
            });


            //function fixSpy() {
            //    // grab a copy the scrollspy data for the element
            //    var data = $body.data('bs.scrollspy');
            //    // if there is data, lets fiddle with the offset value
            //    if (data) {
            //        // get the current height of the navbar
            //        offset = $navtop.outerHeight();
            //        // adjust the body's padding top to match
            //        $body.css('padding-top', offset);
            //        // change the data's offset option to match
            //        data.options.offset = offset
            //        // now stick it back in the element
            //        $body.data('bs.scrollspy', data);
            //        // and finally refresh scrollspy
            //        $body.scrollspy('refresh');
            //    }
            //}
            //fixSpy();
            //gallery light box setup
            //$('a.colorbox').colorbox({
            //                            rel: function(){
            //                                return $(this).data('group');

            //                            }
            //});
        });
    });

    //function initialize() {
    //    var mapOptions = {
    //      zoom: 12,
    //      center: new google.maps.LatLng(16.8451789,96.1439764)
    //    };

    //    var map = new google.maps.Map(document.getElementById('map-canvas'),  mapOptions);
    //}

    // scroll animation 
    function scrollTo(selectors) {
        if (!$(selectors).size()) return;
        var selector_top = $(selectors).offset().top - top_menu_height;
        $('html,body').animate({ scrollTop: selector_top }, 'slow');
    }