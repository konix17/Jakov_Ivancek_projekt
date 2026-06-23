// Hotel Manager shared JavaScript helpers.

function debounce(fn, delay) {
    let timer = null;
    return function () {
        const context = this;
        const args = arguments;
        clearTimeout(timer);
        timer = setTimeout(() => fn.apply(context, args), delay);
    };
}

function createAutocompleteDropdown(container, searchUrl, hiddenInputSelector) {
    const input = $(container).find("input[type='text']");
    const list = $(container).find(".autocomplete-results");
    const hiddenInput = $(container).find(hiddenInputSelector);

    const renderResults = function (items) {
        list.empty();
        if (!items || items.length === 0) {
            list.hide();
            return;
        }

        items.forEach(item => {
            const entry = $(`<div class="autocomplete-item" data-id="${item.id}" data-text="${item.text}">${item.text}</div>`);
            const metaText = item.meta || item.hotel || item.guest || item.city || item.room;
            if (metaText) {
                entry.append(`<span class="autocomplete-meta">${metaText}</span>`);
            }
            list.append(entry);
        });

        list.show();
    };

    const search = debounce(function (force) {
        const query = input.val().trim();
        if (!query && !force) {
            renderResults([]);
            return;
        }

        $.getJSON(searchUrl, { term: query })
            .done(function (data) {
                if (!query) {
                    renderResults(data.slice(0, 10));
                } else {
                    renderResults(data);
                }
            })
            .fail(function () {
                renderResults([]);
            });
    }, 250);

    input.on("input", function () {
        search(false);
    });

    input.on("focus", function () {
        if (!input.val().trim()) {
            search(true);
        }
    });

    list.on("click", ".autocomplete-item", function () {
        const selectedText = $(this).data("text");
        const selectedId = $(this).data("id");
        input.val(selectedText);
        hiddenInput.val(selectedId);
        list.hide();
    });

    $(document).on("click", function (event) {
        if (!$(event.target).closest(container).length) {
            list.hide();
        }
    });
}

function bindAutocompleteWidgets() {
    $(".autocomplete-dropdown").each(function () {
        const searchUrl = $(this).data("search-url");
        const hiddenInputSelector = $(this).data("hidden-selector") || "input[type='hidden']";
        if (searchUrl) {
            createAutocompleteDropdown(this, searchUrl, hiddenInputSelector);
        }
    });
}

function bindAjaxSearchForms() {
    $(".ajax-search-form").each(function () {
        const container = $(this);
        const input = container.find("input[name='q']");
        const targetSelector = container.data("target");
        const url = container.attr("action");

        input.on("input", debounce(function () {
            container.submit();
        }, 350));

        container.on("submit", function (event) {
            event.preventDefault();
            const query = input.val();
            const target = $(targetSelector);
            if (!url || !target.length) {
                return;
            }

            $.get(url, { q: query }, function (html) {
                target.html(html);
                target.find(".fade-in").hide().fadeIn(250);
            });
        });
    });
}

function bindSaveAnimations() {
    $("form").on("submit", function () {
        const form = $(this);
        if (form.data("save-animated")) {
            return;
        }

        if (typeof form.valid === "function" && !form.valid()) {
            return;
        }

        const button = form.find("button[type='submit'], input[type='submit']").filter(":visible").first();
        if (!button.length) {
            return;
        }

        form.data("save-animated", true);
        button.prop("disabled", true).addClass("saving");

        if (button.is("button")) {
            button.data("orig-text", button.html());
            button.html("<span class=\"save-spinner\"></span>Saving...");
        } else {
            button.data("orig-text", button.val());
            button.val("Saving...");
        }
    });
}

function bindHotelRoomFilters() {
    $("select[data-rooms-url]").each(function () {
        const hotelSelect = $(this);
        const roomSelect = $("#room-selection");
        const roomUrl = hotelSelect.data("rooms-url");
        const initialSelectedRoom = roomSelect.val();

        const renderRoomOptions = function (rooms, selectedRoomId) {
            roomSelect.empty();
            roomSelect.append($('<option>').attr('value', '').text('Select a room...'));
            rooms.forEach(function (room) {
                roomSelect.append($('<option>').attr('value', room.id).text(room.text));
            });
            if (selectedRoomId) {
                roomSelect.val(selectedRoomId);
            }
        };

        const loadRooms = function (hotelId) {
            if (!hotelId) {
                roomSelect.empty();
                roomSelect.append($('<option>').attr('value', '').text('Select a hotel first...'));
                return;
            }

            $.getJSON(roomUrl, { hotelId: hotelId })
                .done(function (data) {
                    renderRoomOptions(data, initialSelectedRoom);
                })
                .fail(function () {
                    renderRoomOptions([], null);
                });
        };

        hotelSelect.on("change", function () {
            loadRooms(hotelSelect.val());
        });

        if (hotelSelect.val()) {
            loadRooms(hotelSelect.val());
        } else {
            roomSelect.empty();
            roomSelect.append($('<option>').attr('value', '').text('Select a hotel first...'));
        }
    });
}

function initializeDateTimePickers() {
    // Detect browser language for localization
    const browserLang = (navigator.language || navigator.userLanguage || 'en').split('-')[0];
    const isHrLocale = browserLang === 'hr';
    
    $(".flatpickr-datetime").each(function () {
        const input = $(this);
        
        // Configure Flatpickr with proper localization
        flatpickr(this, {
            mode: "single",
            enableTime: false,
            dateFormat: isHrLocale ? "d.m.Y" : "Y-m-d",
            locale: isHrLocale ? "hr" : "en",
            allowInput: true,
            onClose: function (selectedDates, dateStr, instance) {
                if (typeof input.valid === "function") {
                    input.valid();
                }
            }
        });

        input.on("blur", function () {
            if (typeof input.valid === "function") {
                input.valid();
            }
        });
    });
}

function bindGlobalSearch() {
    const input = $("#globalSearchInput");
    const results = $("#globalSearchResults");

    if (!input.length) return;

    const performGlobalSearch = debounce(function () {
        const query = input.val().trim();
        if (!query) {
            results.empty().hide();
            return;
        }

        $.getJSON("/api/search", { q: query })
            .done(function (data) {
                results.empty();
                if (!data || data.length === 0) {
                    results.append('<div class="autocomplete-item text-muted">Nema rezultata</div>');
                } else {
                    data.forEach(item => {
                        const row = $(`<div class="autocomplete-item" style="display: flex; align-items: center; gap: 8px; justify-content: start;">
                            <span class="badge bg-secondary">${item.category}</span>
                            <span>${item.title}</span>
                        </div>`);
                        row.on("click", function () {
                            window.location.href = item.url;
                        });
                        results.append(row);
                    });
                }
                results.show();
            })
            .fail(function () {
                results.empty().hide();
            });
    }, 300);

    input.on("input", performGlobalSearch);

    $(document).on("click", function (e) {
        if (!$(e.target).closest("#globalSearchInput, #globalSearchResults").length) {
            results.hide();
        }
    });

    input.on("focus", function() {
        if (input.val().trim()) {
            results.show();
        }
    });
}

function bindMobileNavigation() {
    const sidebar = $(".app-sidebar");
    const backdrop = $("#mobileMenuBackdrop");
    const toggleBtn = $("#mobileMenuToggle");
    const closeBtn = $("#mobileMenuClose");

    if (!toggleBtn.length) return;

    toggleBtn.on("click", function () {
        sidebar.addClass("show");
        backdrop.addClass("show");
    });

    const closeMenu = function () {
        sidebar.removeClass("show");
        backdrop.removeClass("show");
    };

    closeBtn.on("click", closeMenu);
    backdrop.on("click", closeMenu);
    
    // Also close menu when clicking on any nav link on mobile
    sidebar.find(".nav-link").on("click", function () {
        if ($(window).width() <= 940) {
            closeMenu();
        }
    });
}

$(function () {
    bindAutocompleteWidgets();
    bindHotelRoomFilters();
    bindAjaxSearchForms();
    bindSaveAnimations();
    initializeDateTimePickers();
    bindGlobalSearch();
    bindMobileNavigation();
});

