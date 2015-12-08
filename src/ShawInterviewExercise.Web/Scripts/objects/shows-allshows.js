var GlobalTv = (function (parent, $) {
    var globalTv = parent || {};

    var Core = function () {

        var moduleData = {};

        return {
            register: function (moduleId, creator) {
                moduleData[moduleId] = {
                    creator: creator,
                    instance: null
                };
            },

            start: function (moduleId) {
                moduleData[moduleId].instance = moduleData[moduleId].creator(new Sandbox(this, moduleId));
                moduleData[moduleId].instance.init();
            },

            stop: function (moduleId) {
                var data = moduleData[moduleId];
                if (data.instance) {
                    data.instance.destroy();
                    data.instance = null;
                }
            },

            startAll: function () {
                for (var moduleId in moduleData) {
                    if (moduleData.hasOwnProperty(moduleId)) {
                        this.start(moduleId);
                    };
                }
            },

            stopAll: function () {
                for (var moduleId in moduleData) {
                    if (moduleData.hasOwnProperty(moduleId)) {
                        this.stop(moduleId);
                    }
                }
            }
        };
    }();

    var Communication = function () {

        var slice = [].slice, handlers = {};
        var failedNotifications = [];

        return {
            addListener: function (topic, callback, context) {
                var type = topic;
                if (!handlers[type]) {
                    handlers[type] = [];
                }
                handlers[type].push({ context: context, callback: callback });

                if (failedNotifications[type]) {
                    this.notify(topic, failedNotifications[type]);
                    delete failedNotifications[type];
                }
            },

            notify: function (topic) {
                var args = slice.call(arguments, 1),
                type = topic,
                ret = true,
                i,
                len,
                msgList;

                if (!handlers[topic]) {
                    if (!failedNotifications[topic]) {
                        failedNotifications[topic] = args;
                    }
                    return true;
                }

                if (handlers[type] instanceof Array) {
                    msgList = handlers[type];
                    for (i = 0, len = msgList.length; i < len && ret === true; i++) {
                        ret = msgList[i].callback.apply(msgList[i].context, args);
                        if (ret === undefined) {
                            ret = true;
                        }
                    }
                }
                return ret;
            },

            removeListener: function (topic, callbackFunction) {
                var type = topic, callback = callbackFunction, handlersArray = handlers[type], i, len;
                if (handlersArray instanceof Array) {
                    for (i = 0, len = handlersArray.length; i < len; i++) {
                        if (handlersArray[i].callback === callback) {
                            break;
                        }
                    }
                    handlers[type].splice(i, 1);
                }
            },

            removeAllListeners: function () {
                handlers = {};
            },

            removeAllListenersForContext: function (messageContext) {
                //TODO: this method is unfinished
                var context = messageContext, handlersArray, i, j;

                for (j = handlers.length; j >= 0; j++) {
                    handlersArray = handlers[j];
                    if (handlersArray instanceof Array) {
                        for (i = handlersArray.length; i >= 0; i--) {
                            if (handlersArray[i].context === messageContext) {
                                handlersArray.splice(i, 1);
                            }
                        }

                        if (handlers[j].length === 0) {
                            handlers.splice(j, 1);
                        }
                    }
                }
            }
        };
    }();

    /*Modules****************************************************************************/
    var Sandbox = function (core, moduleId) {
        return {
            notify: function () {
                return Communication.notify.apply(Communication, arguments);
            },

            addListener: function (topic, callback, context) {
                Communication.addListener(topic, callback, context);
            },

            removeListener: function (topic, callback) {
                Communication.removeListener(topic, callback);
            },

            removeAllListeners: function (context) {
                Communication.removeAllListenersForContext(context);
            }
        }
    }

    var allShows = [],
        numericFilter = '#';

    Core.register("shows-list", function (sandbox) {

        var allshows_container_id = "section.allShows";
        var show_articles_list_id = "section.allShows .allShows-show";

        return {
            init: function () {
                //constructor
                this.createAllShowsList();
                sandbox.addListener("shows-list-filter-change", this.applyFilter, this);
            },

            destroy: function () {
                //destructor
                sandbox.removeAllListeners(this);
            },

            applyFilter: function (data) {

                function ClearFloat_FirstVisibleElementInLine() {
                    enquire.register(globalTv.GLOBAL_bp_smallToMedium, {
                        match: function () { //mobile
                            $(show_articles_list_id).removeClass('firts-in-line');
                            $(show_articles_list_id + '.is-visible').each(function (i) {
                                if (i % 2 == 0) $(this).addClass('firts-in-line');
                            });
                        },
                        unmatch: function () { }
                    });

                    enquire.register(globalTv.GLOBAL_bp_mediumToExtraLarge, {
                        match: function () { //tablet
                            $(show_articles_list_id).removeClass('firts-in-line');
                            $(show_articles_list_id + '.is-visible').each(function (i) {
                                if (i % 3 == 0) $(this).addClass('firts-in-line');
                            });
                        },
                        unmatch: function () { }
                    });

                    enquire.register(globalTv.GLOBAL_bp_extraLargePlus, {
                        match: function () { //desktop
                            $(show_articles_list_id).removeClass('firts-in-line');
                            $(show_articles_list_id + '.is-visible').each(function (i) {
                                if (i % 4 == 0) $(this).addClass('firts-in-line');
                            });
                        },
                        unmatch: function () { }
                    });
                }

                if (!data['value']) return;

                data['value'] = data['value'].replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\,\<\>\:\;\~\`\\\^\$\|]/g, "\\$&");
                switch (data['type']) {
                    case 'search':
                        if (data['value'] == 'all') {

                            var previousId;
                            for (var i = 0; i < allShows.length; i++) {
                                allShows[i].obj.fadeIn('slow');
                                allShows[i].obj.removeClass('is-visible').removeClass('firts-in-line');
                            }
                            $(allshows_container_id).removeClass("filters-applied");
                            $("#search_error_message").hide();
                        } else {

                            $(show_articles_list_id).hide();
                            $(show_articles_list_id).removeClass('is-visible');

                            if (data['search_by_first_symbol'])
                                var reg = '(^' + data['value'].replace(/ /g, ')|(^') + ')';
                            else
                                var reg = '(' + data['value'] + ')';
                            var has_results = false;
                            for (var i = 0; i < allShows.length; i++) {
                                // The showname can start with a letter or a number/digit
                                if (allShows[i]['title'].match(new RegExp(reg, 'gi')) || (data['value'] === numericFilter && $.isNumeric(allShows[i]['title'].charAt(0)))) {
                                    has_results = true;
                                    allShows[i].obj.fadeIn();
                                    allShows[i].obj.addClass('is-visible');
                                }
                            }

                            $(allshows_container_id).addClass("filters-applied");
                            ClearFloat_FirstVisibleElementInLine();

                            if (!has_results)
                                $("#search_error_message").fadeIn();
                            else
                                $("#search_error_message").hide();
                        }
                        break;
                }
            },

            showsListLoaded: function (data) {
                var data = {};
                data['titlesList'] = [];

                for (var i = 0; i < allShows.length; i++)
                    data['titlesList'].push(allShows[i].title);

                sandbox.notify("shows-list-loaded", data['titlesList']);
            },

            createAllShowsList: function () {
                var self = this;

                //layout overlay
                $(".no-touch " + show_articles_list_id).hover(function () {
                    $(this).find('.overlay').fadeIn(150);
                }, function () {
                    $(this).find('.overlay').fadeOut(150);
                });

                $(show_articles_list_id).each(
                    function (id) {
                        $(this).attr('id', 'show_' + id);

                        var show = {};
                        var orgTitle = $(this).find('h3 a').text();

                        show.title = orgTitle;
                        show.HTML_id = $(this).attr('id');
                        show.obj = $(this);

                        allShows.push(show);

                        if (orgTitle.toLowerCase().indexOf("the") == 0) {
                            var showDuplicate = {};
                            for (var attr in show) {
                                if (show.hasOwnProperty(attr)) showDuplicate[attr] = show[attr];
                            }
                            showDuplicate.title = orgTitle.substring(4);
                            allShows.push(showDuplicate);
                        }
                    }
                );

                self.showsListLoaded();
            }
        };
    });

    Core.register("shows-list-filter", function (sandbox) {
        var filters_container_id = ".filter-alpha";
        var filters_id = ".filter-alpha li a";
        var search_id = ".search_filter input";
        var clear_filters_id = ".filter-alpha li a[data-type='clear-filters']";
        var clear_search_id = ".search_filter .close-button";

        return {
            init: function () {
                //constructor
                this.defineDOMEvents();
                sandbox.addListener("shows-list-loaded", this.disableSearchLetters, this);
            },

            destroy: function () {
                //destructor
                sandbox.removeAllListeners(this);
            },

            changeFilter: function (filter) {
                sandbox.notify("shows-list-filter-change", filter);
            },

            disableSearchLetters: function (data) {

                var firstLettersArray = [],
                    numericTitlesArray = [];

                for (var i = 0; i < data[0].length; i++) {
                    var firstChar = data[0][i].charAt(0);
                    if ($.isNumeric(firstChar))
                        numericTitlesArray.push(firstChar);
                    else
                        firstLettersArray.push(firstChar.toLowerCase());
                };

                $(filters_id).each(
                    function (id) {

                        if ($(this).attr('data-type') == 'clear-filters')
                            return;

                        var at_least_one_valid = false;
                        var letters_list = $(this).attr('data-type').split(' ');

                        for (var key in letters_list) {
                            var curKey = letters_list[key];
                            if ((curKey === numericFilter && numericTitlesArray.length) || jQuery.inArray(curKey, firstLettersArray) != -1) {
                                at_least_one_valid = true;
                                break;
                            }
                        }

                        if (!at_least_one_valid) $(this).addClass('disabled');
                    });
            },

            defineDOMEvents: function () {
                var self = this;

                var selected = false;
                $(filters_id).on('click', function (e) {
                    var target = $(this);
                    e.preventDefault();
                    if (!target.hasClass('disabled')) {
                        // add active state to selected filter
                        $(filters_id).removeClass('active');
                        target.addClass('active');
                        // add active state to filter container to show 'all' button
                        if (!selected) {
                            $(filters_container_id).addClass('active');
                            selected = true;
                        }
                            // remove active state from filter container to hide 'all' button
                        else if (selected && target.hasClass('all')) {
                            $(filters_container_id).removeClass('active');
                            selected = false;
                        }
                        if (target.data('type') != 'clear-filters')
                            ApplyShowsFilter({ letters: target.data('type'), search_by_first_symbol: true });
                    }

                });

                $(search_id).keyup(
                    function (event) {
                        if (event.keyCode == 13) { return false; }
                        $(filters_id).css('text-decoration', 'none');
                        ApplyShowsFilter({ letters: $(this).val(), search_by_first_symbol: false });
                    }
                );

                /*clearing filters*/

                $(clear_filters_id).click(ClearSearchInput);
                $(clear_search_id).click(ClearSearchInput);

                enquire.register(globalTv.GLOBAL_bp_extraLargePlus, {
                    match: function () {
                        ResetFilters();
                        $(filters_container_id).removeClass('active');
                    },
                    unmatch: function () {
                        ResetFilters();
                        $(filters_id + ".all").addClass('active');
                    }
                });

                function ResetFilters() {
                    $(filters_id).removeClass('active');
                    $(filters_id).blur();
                    ClearSearchInput();
                }

                function ClearSearchInput() {
                    $(search_id).val('');
                    ApplyShowsFilter({ letters: '', search_by_first_symbol: false });
                }

                function ApplyShowsFilter(parameters) {
                    var letters = parameters['letters'];
                    var search_by_first_symbol = parameters['search_by_first_symbol'];

                    if (letters.length) {
                        if (!search_by_first_symbol) $(search_id).attr('value', letters);

                        var filter = { type: 'search', search_by_first_symbol: search_by_first_symbol, value: letters };
                        self.changeFilter(filter);

                    } else {

                        var filter = { type: 'search', value: 'all' };
                        self.changeFilter(filter);
                    }
                }

                function stopRKey(evt) {
                    var evt = (evt) ? evt : ((event) ? event : null);
                    var node = (evt.target) ? evt.target :
                                             ((evt.srcElement) ? evt.srcElement : null);

                    if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
                    if ((evt.keyCode == 13) && (node.type == "email")) { return false; }
                    if ((evt.keyCode == 13) && (node.type == "tel")) { return false; }
                    if ((evt.keyCode == 13) && (node.type == "number")) { return false; }
                }

                document.onkeypress = stopRKey;
            }
        };
    });

    $(document).ready(function () {
        Core.startAll();
    });

    return globalTv;
}(GlobalTv || {}, jQuery));
