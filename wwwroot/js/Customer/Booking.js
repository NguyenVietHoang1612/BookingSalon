let selectedServices = [];


// Load Modal Dịch vụ
function toggleService(id, typeId, name, price, duration) {
    const radio = document.getElementById('radio-' + id);
    const card = document.getElementById('svc-' + id);
    const btn = card.querySelector('.btn-select');

    if (card.classList.contains('selected')) {
        radio.checked = false;
        card.classList.remove('selected');
        btn.innerText = "CHỌN";
        selectedServices = selectedServices.filter(s => s.id !== id);
    }
    else {
        $(`input[name="group_${typeId}"]`).each(function () {
            const otherId = this.id.split('-')[1];
            const otherCard = document.getElementById(`svc-${otherId}`);
            if (otherCard) {
                otherCard.classList.remove('selected');
                otherCard.querySelector('.btn-select').innerText = "CHỌN";
            }
            this.checked = false;
        });

        radio.checked = true;
        card.classList.add('selected');
        btn.innerText = "ĐÃ CHỌN";

        selectedServices = selectedServices.filter(s => s.typeId !== typeId);
        selectedServices.push({
            id: id,
            typeId: typeId,
            name: name,
            price: parseFloat(price),
            duration: parseInt(duration)
        });
    }

    resetCouponDisplay();
    renderInvoice();
}

function renderInvoice() {
    let subTotal = 0;
    let totalTime = 0;
    let html = '';
    let hidden = '';
    let names = [];

    selectedServices.forEach((s, i) => {
        subTotal += s.price;
        totalTime += s.duration;
        html += `<div class="invoice-row">
                    <span>${s.name}</span>
                    <span class="fw-bold">${s.price.toLocaleString('vi-VN')}đ</span>
                 </div>`;
        hidden += `<input type="hidden" name="BookingDetails[${i}].Service_Id" value="${s.id}" />`;
        names.push(s.name);
    });

    const rankPercent = parseFloat($('#rankPercent').val()) || 0;
    const rankDiscount = subTotal * (rankPercent / 100);
    const couponDiscount = parseFloat($('#couponDiscountValue').val()) || 0;
    const finalTotal = Math.max(0, subTotal - rankDiscount - couponDiscount);

    $('#invoiceBody').html(html || '<p class="text-center text-muted small">Vui lòng chọn dịch vụ...</p>');
    $('#txtDuration').text(totalTime + ' phút');
    $('#txtRankDiscount').text(`-${rankDiscount.toLocaleString('vi-VN')}đ (${rankPercent}%)`);
    $('#txtCouponDiscount').text(`-${couponDiscount.toLocaleString('vi-VN')}đ`);
    $('#txtTotal').text(finalTotal.toLocaleString('vi-VN') + 'đ');

    $('#totalDuration').val(totalTime);
    $('#totalPriceBeforeDiscount').val(subTotal);
    $('#hiddenDetails').html(hidden);
    $('#displaySelectedServices').text(names.join(' + ') || "Chưa có dịch vụ nào được chọn...");
}

// CHọn chi nhánh load nhân viên
$('#branchId').change(function () {
    const bId = $(this).val();

    selectedServices = [];
    $('#stylistId').val('');
    $('#skinnerId').val('');
    $('#startSlotId').val('');
    $('#bookingDate').val(''); 

    if (!bId) {
        $('#stylistWrapper, #skinnerWrapper, #slotWrapper').addClass('d-none');
        renderInvoice();
        return;
    }

    loadStaffCards(`/Booking/GetStaffStylist?branchId=${bId}`, '#stylistContainer', 'stylist');
    loadStaffCards(`/Booking/GetStaffSkinner?branchId=${bId}`, '#skinnerContainer', 'skinner');

    $('#timeSlotsArea').html('<span class="text-muted small italic">Vui lòng chọn stylist và ngày...</span>');
    $('#slotWrapper').addClass('d-none');
    renderInvoice();
});


// Load Nhân viên
function loadStaffCards(url, containerId, type) {
    const wrapperId = (type === 'stylist') ? '#stylistWrapper' : '#skinnerWrapper';
    $(containerId).html('<div class="spinner-border text-warning spinner-border-sm"></div>');

    $.getJSON(url, function (data) {
        if (!data || data.length === 0) {
            $(wrapperId).addClass('d-none');
            return;
        }

        let html = '';
        if (type === 'stylist') {
            data.forEach(staff => {
                const ratingHtml = `
            <div class="staff-rating" style="font-size: 0.8rem; color: #ffc107;">
                <i class="fas fa-star"></i> ${staff.rating}
            </div>`;

                html += `
            <div class="staff-card" data-id="${staff.id}" onclick="selectStaff(this, '${type}')">
                <img src="/media/users/${staff.img || 'default.jpg'}" class="staff-img" onerror="this.src='/media/logo/BranchLogo.jpg'">
                <div class="fw-bold text-white mt-1">${staff.name}</div>
                ${ratingHtml}
            </div>`;
            });
        }
        else {
            data.forEach(staff => {
                html += `
            <div class="staff-card" data-id="${staff.id}" onclick="selectStaff(this, '${type}')">
                <img src="/media/users/${staff.img || 'default.jpg'}" class="staff-img" onerror="this.src='/media/logo/BranchLogo.jpg'">
                <div class="fw-bold text-white mt-1">${staff.name}</div>
            </div>`;
            });
        }
        

        $(containerId).html(html);
        $(wrapperId).removeClass('d-none');
        checkSliderButtons(containerId);
    });
}

function selectStaff(element, type) {
    const container = $(element).parent();
    container.find('.staff-card').removeClass('selected');
    $(element).addClass('selected');

    const id = $(element).data('id');

    if (type === 'stylist') {
        $('#stylistId').val(id);
    } else {
        $('#skinnerId').val(id);
    }
    $('#startSlotId').val('');
    loadSlotsIfReady();
}

// Xử lý chọn Slot
$('#bookingDate').change(function () {
    $('#startSlotId').val('');
    loadSlotsIfReady();
});

function loadSlotsIfReady() {
    const date = $('#bookingDate').val();
    const stylistId = $('#stylistId').val();
    const duration = $('#totalDuration').val();
    const skinnerId = $('#skinnerId').val() || "";

    if (!date || !stylistId) return;

    $('#timeSlotsArea').html('<div class="spinner-border text-warning spinner-border-sm"></div> Đang tìm lịch...');
    $('#slotWrapper').removeClass('d-none');

    $.ajax({
        url: `/Booking/GetAvailableSlots`,
        type: 'GET',
        data: {
            stylistId: stylistId,
            skinnerId: skinnerId,
            date: date,
            durationMinus: duration
        },
        success: function (res) {
            let html = '';

            if (res && res.message) {
                html = `<div class="alert alert-warning w-100 mb-0 py-2 small"><i class="bi bi-info-circle"></i> ${res.message}</div>`;
            }
            else if (!res || res.length === 0) {
                html = `<div class="alert alert-danger w-100 mb-0 py-2 small"><i class="bi bi-x-circle"></i> Không tìm thấy lịch làm việc.</div>`;
            }
            else {
                const now = new Date();
                const todayStr = now.toISOString().split('T')[0];

                const hasAvailableSlot = res.some(slot => {
                    if (!slot.isAvailable) return false;
                    if ($('#bookingDate').val() === todayStr) {
                        const [hour, minute] = slot.timeRange.split(':');
                        const slotTime = new Date();
                        slotTime.setHours(parseInt(hour), parseInt(minute), 0, 0);
                        return slotTime > now;
                    }
                    return true;
                });

                if (!hasAvailableSlot) {
                    html = `<div class="alert alert-danger w-100 mb-0 py-2 small">
                        <i class="bi bi-calendar-x"></i> Rất tiếc, không còn khung giờ nào khả dụng cho thợ này trong ngày đã chọn.
                    </div>`;
                } else {
                    res.forEach(slot => {
                        let disabledClass = slot.isAvailable ? '' : 'disabled';

                        if ($('#bookingDate').val() === todayStr) {
                            const [hour, minute] = slot.timeRange.split(':');
                            const slotTime = new Date();
                            slotTime.setHours(parseInt(hour), parseInt(minute), 0, 0);
                            if (slotTime <= now) disabledClass = 'disabled';
                        }

                        html += `<div class="slot-btn ${disabledClass}" data-id="${slot.slotId}">
                            ${slot.timeRange.substring(0, 5)}
                         </div>`;
                    });
                }
            }

            $('#timeSlotsArea').html(html);
            checkSlotSlider();
        },
        error: function (xhr) {
            let errorText = "Lỗi hệ thống, vui lòng thử lại.";
            if (xhr.status === 400) {
                errorText = xhr.responseText || "Thông tin gửi đi không hợp lệ.";
            }
            $('#timeSlotsArea').html(`<div class="alert alert-danger w-100 mb-0 py-2 small">${errorText}</div>`);
        }
    });
}


$(document).on('click', '.slot-btn:not(.disabled)', function () {
    $('.slot-btn').removeClass('active');
    $(this).addClass('active');
    $('#startSlotId').val($(this).data('id'));
});

// Coupon Logic
$('#btnApplyCoupon').click(function () {
    let code = $('#couponCode').val();

    let orderAmount = 0;
    selectedServices.forEach(s => {
        orderAmount += s.price;
    });

    if (orderAmount === 0) {
        showCouponMessage("Vui lòng chọn dịch vụ trước khi áp mã", false);
        return;
    }

    if (!code) return;

    $.post('/Coupon/ValidateCouponAjax', {
        code: code,
        orderAmount: orderAmount
    }, function (res) {
        if (!res.succeeded) {
            showCouponMessage(res.errors[0], false);
            $('#couponDiscountValue').val(0);
        } else {
            $('#couponDiscountValue').val(res.discount);
            showCouponMessage("Áp dụng mã thành công!", true);
        }
        renderInvoice();
    });
});

function resetCouponDisplay() {
    $('#couponDiscountValue').val(0);
    $('#couponMessage').text('');
}

function showCouponMessage(message, isSuccess) {
    const el = $('#couponMessage');
    el.text(message).removeClass('text-danger text-success').addClass(isSuccess ? 'text-success' : 'text-danger');
}

// Scroll Logic
function scrollStaff(containerId, direction) {
    document.getElementById(containerId).scrollBy({ left: 200 * direction, behavior: 'smooth' });
}

function scrollSlot(direction) {
    document.getElementById('timeSlotsArea').scrollBy({ left: 200 * direction, behavior: 'smooth' });
}

function checkSliderButtons(containerId) {
    const container = document.querySelector(containerId);
    if (!container) return;
    const wrapper = container.parentElement;
    const btns = $(wrapper).find('.slider-btn');
    container.scrollWidth <= container.clientWidth ? btns.hide() : btns.show();
}

function checkSlotSlider() {
    const container = document.getElementById('timeSlotsArea');
    const btns = $('#slotWrapper .slider-btn');
    container.scrollWidth <= container.clientWidth ? btns.hide() : btns.show();
}

// Form Validation
$('#formBooking').submit(function (e) {
    if (selectedServices.length === 0) { alert("Vui lòng chọn dịch vụ"); e.preventDefault(); return; }
    if (!$('#branchId').val()) { alert("Vui lòng chọn chi nhánh"); e.preventDefault(); return; }
    if (!$('#stylistId').val()) { alert("Vui lòng chọn stylist"); e.preventDefault(); return; }
    if (!$('#bookingDate').val()) { alert("Vui lòng chọn ngày"); e.preventDefault(); return; }
    if (!$('#startSlotId').val()) { alert("Vui lòng chọn khung giờ"); e.preventDefault(); return; }

    $(this).find('button[type="submit"]').prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Đang đặt lịch...');
});