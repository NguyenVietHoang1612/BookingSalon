

let customerChoices, branchChoices;
let stylistChoices, skinnerChoices;

let selectedServices = [];

// Xử lý Select2 
function initStaffChoices() {
    stylistChoices = initStaffSelect2('stylistId', 'Chọn stylist...');
    skinnerChoices = initStaffSelect2('skinnerId', 'Chọn skinner...');
}

function initSelect2(elementId, placeholder) {
    return $(`#${elementId}`).select2({
        placeholder: placeholder,
        allowClear: true,
        width: '100%'
    });
}

function initStaffSelect2(elementId, placeholder) {
    return $(`#${elementId}`).select2({
        placeholder: placeholder,
        allowClear: false,
        width: '100%',
        templateResult: formatStaff,
        templateSelection: formatStaff
    });
}

function formatStaff(state) {
    if (!state.id) return state.text;

    if (state.id === 'RANDOM') {
        return $(`<span class="fw-bold text-primary"><i class="fas fa-magic me-2"></i>${state.text}</span>`);
    }

    const img = $(state.element).data('img') || (state.img ? state.img : null);
    const imgUrl = img ? `/media/users/${img}` : '/media/logo/default-user.jpg';

    const $state = $(
        `<span style="display:flex; align-items:center;">
            <img src="${imgUrl}" class="rounded-circle" style="width:24px; height:24px; object-fit:cover; margin-right:10px;" onerror="this.src='/media/logo/default-user.jpg'"/>
            <span class="fw-medium">${state.text}</span>
        </span>`
    );
    return $state;
};

// Load khách hàng, chi nhánh
$(document).ready(function () {
    customerChoices = initSelectSearch('customerId', 'Chọn khách hàng...');
    branchChoices = initSelectSearch('branchId', 'Chọn chi nhánh...');
    initStaffChoices();
    initEvents();

    const currentBranch = $('#branchId').val();
    if (currentBranch) {
        loadStaffAdmin(`/Admin/Booking/GetStaffStylist?branchId=${currentBranch}`, '#stylistId');
        loadStaffAdmin(`/Admin/Booking/GetStaffSkinner?branchId=${currentBranch}`, '#skinnerId');
    }
});

function initEvents() {
    $('#branchId').on('change', function () {
        const bId = $(this).val();
        renderInvoice();

        if (!bId) {
            $('#timeSlotsArea').html('<span class="text-muted small">Vui lòng chọn chi nhánh...</span>');
            return;
        }

        loadStaffAdmin(`/Admin/Booking/GetStaffStylist?branchId=${bId}`, '#stylistId');
        loadStaffAdmin(`/Admin/Booking/GetStaffSkinner?branchId=${bId}`, '#skinnerId');
    });

    $('#stylistId, #skinnerId').on('change', function () {
        const stylistId = $('#stylistId').val();
        const skinnerId = $('#skinnerId').val();

        if (stylistId && skinnerId &&
            stylistId !== 'RANDOM' &&
            skinnerId !== 'RANDOM' &&
            stylistId === skinnerId) {

            alert("Stylist và Skinner đích danh không được là cùng một người!");
            $(this).val(null).trigger('change.select2');
            return;
        }

        loadSlotsIfReady();
    });

    $('#bookingDate').on('change', function () {
        $('#startSlotId').val('');
        loadSlotsIfReady();
    });

    $(document).on('click', '.slot-btn:not(.disabled)', function () {
        $('.slot-btn').removeClass('active');
        $(this).addClass('active');
        $('#startSlotId').val($(this).data('id'));
    });
}

// Load stylist, skinner theo chi nhánh
function loadStaffAdmin(url, selectId) {
    const $select = $(selectId);
    const currentValue = $select.val();

    $.getJSON(url, function (data) {
        $select.empty();
        $select.append(new Option('', '', false, false));
        $select.append(new Option('--- Hệ thống chọn hộ ---', 'RANDOM', false, currentValue === 'RANDOM'));

        data.forEach(staff => {
            const newOption = new Option(staff.name, staff.id.toString(), false, staff.id.toString() === currentValue);
            $(newOption).attr('data-img', staff.img);
            $select.append(newOption);
        });

        $select.trigger('change.select2');
        if (currentValue) loadSlotsIfReady();
    });
}

function loadSlotsIfReady() {
    const date = $('#bookingDate').val();
    const branchId = $('#branchId').val();
    const stylistId = $('#stylistId').val();
    const skinnerId = $('#skinnerId').val();
    const duration = parseInt($('#totalDuration').val()) || 0;

    if (!date || !branchId || duration <= 0) {
        $('#timeSlotsArea').html('<span class="text-muted small">Vui lòng chọn đủ thông tin...</span>');
        return;
    }

    if (stylistId && skinnerId && stylistId !== 'RANDOM' && skinnerId !== 'RANDOM' && stylistId === skinnerId) {
        $('#timeSlotsArea').html('<div class="alert alert-warning py-1 small">Stylist và Skinner không được trùng nhau!</div>');
        return;
    }

    $('#timeSlotsArea').html('<div class="spinner-border text-primary spinner-border-sm"></div>');

    $.ajax({
        url: `/Admin/Booking/GetSlotsAvailableForBranch`,
        type: 'GET',
        data: { branchId, stylistId, skinnerId, date, duration },
        success: function (data) { renderSlots(data, date); },
        error: function () { $('#timeSlotsArea').html('<span class="text-danger small">Lỗi tải lịch.</span>'); }
    });
}

function selectStaffAdmin(element, type, id) {
    if (type === 'stylist') {
        $('#stylistId').val(id.toString()).trigger('change.select2');
    } else {
        $('#skinnerId').val(id.toString()).trigger('change.select2');
    }
}



function renderSlots(data, selectedDate) {
    let html = '';
    const area = $('#timeSlotsArea');

    if (!data || data.length === 0) {
        area.html('<span class="text-danger small italic">Hết chỗ hoặc chưa có lịch làm việc.</span>');
        return;
    }

    const now = new Date();

    const [year, month, day] = selectedDate.split('-').map(Number);
    const selDate = new Date(year, month - 1, day);
    const isToday = selDate.toDateString() === now.toDateString();

    data.forEach(slot => {
        let available = slot.isAvailable;


        if (available && isToday) {
            const [slotHour, slotMin] = slot.timeRange.split(':').map(Number);
            const slotTime = new Date(year, month - 1, day, slotHour, slotMin, 0);

            if (slotTime <= now) {
                available = false;
            }
        }

        const disabledClass = available ? '' : 'disabled';
        const displayTime = slot.timeRange.substring(0, 5);

        html += `<div class="slot-btn btn btn-outline-primary btn-sm ${disabledClass}" 
                      data-id="${slot.slotId}">${displayTime}</div>`;
    });

    area.html(html);
}

function toggleService(id, typeId, name, price, duration) {
    const card = document.getElementById('svc-' + id);
    const btn = card.querySelector('.btn-select');

    if (card.classList.contains('selected')) {
        card.classList.remove('selected', 'border-primary');
        btn.innerText = 'CHỌN';
        btn.classList.replace('btn-warning', 'btn-outline-warning');
        selectedServices = selectedServices.filter(s => s.id !== id);
    } else {
        $(`input[name="group_${typeId}"]`).each(function () {
            const otherId = this.id.split('-')[1];
            const otherCard = document.getElementById(`svc-${otherId}`);
            if (otherCard) {
                otherCard.classList.remove('selected', 'border-primary');
                const otherBtn = otherCard.querySelector('.btn-select');
                if (otherBtn) {
                    otherBtn.innerText = 'CHỌN';
                    otherBtn.classList.replace('btn-warning', 'btn-outline-warning');
                }
            }
        });


        card.classList.add('selected', 'border-primary');
        btn.innerText = 'ĐÃ CHỌN';
        btn.classList.replace('btn-outline-warning', 'btn-warning');

        selectedServices = selectedServices.filter(s => s.typeId !== typeId);
        selectedServices.push({ id, typeId, name, price: parseFloat(price), duration: parseInt(duration) });
    }
    renderInvoice();
}

let totalBeforeCoupon = 0; 

function renderInvoice() {
    let total = 0, time = 0, html = '', hidden = '', names = [];

    selectedServices.forEach((s, i) => {
        total += s.price;
        time += s.duration;
        html += `<div class="d-flex justify-content-between fs-6 mb-1">
                    <span>${s.name}</span>
                    <span class="fw-bold">${s.price.toLocaleString('vi-VN')}đ</span>
                 </div>`;
        hidden += `<input type="hidden" name="BookingDetails[${i}].Service_Id" value="${s.id}" />`;
        names.push(s.name);
    });

    $('#totalDuration').val(time);
    $('#txtDuration').text(time + ' phút');

    const rankPercent = parseFloat($('#rankPercent').val()) || 0;
    const rankDiscount = total * rankPercent / 100;
    totalBeforeCoupon = total - rankDiscount;
    const couponDiscount = parseFloat($('#couponDiscountValue').val()) || 0;
    const finalTotal = Math.max(0, totalBeforeCoupon - couponDiscount);

    $('#invoiceBody').html(html || '<p class="text-center text-muted small">Chưa chọn dịch vụ</p>');
    $('#txtRankDiscount').text('-' + rankDiscount.toLocaleString('vi-VN') + 'đ');
    $('#txtCouponDiscount').text('-' + couponDiscount.toLocaleString('vi-VN') + 'đ');
    $('#txtTotal').text(finalTotal.toLocaleString('vi-VN') + 'đ');
    $('#totalPriceInput').val(finalTotal);
    $('#hiddenDetails').html(hidden);
    $('#displaySelectedServices').text(names.join(' + ') || "Chưa chọn dịch vụ...");

    const currentStylist = $('#stylistId').val();

    loadSlotsIfReady();
}

$('#customerId').change(function () {
    const cId = $(this).val();
    if (!cId) {
        $('#rankPercent').val(0);
        $('#customerRankNote').text('');
        renderInvoice();
        return;
    }

    $.getJSON(`/Admin/Booking/GetCustomerRank?customerId=${cId}`, function (data) {
        if (data && data.succeeded) {
            const percent = data.data.rank.discountPercent;
            const rankName = data.data.rank.rankName;

            $('#rankPercent').val(percent);
            $('#customerRankNote').text(`Hạng: ${rankName} (Giảm ${percent}%)`);
        } else {
            $('#rankPercent').val(0);
            $('#customerRankNote').text('Hạng: Thành viên mới (0%)');
        }
        renderInvoice();
    });
});

function resetBookingProgress() {
    $('#stylistId').val(null).trigger('change.select2');
    $('#skinnerId').val(null).trigger('change.select2');

    $('#startSlotId').val('');
    $('#timeSlotsArea').html('<span class="text-muted small">Vui lòng chọn stylist...</span>');
}

$('#formBooking').submit(function (e) {
    const requiredFields = {
        '#branchId': 'vui lòng chọn chi nhánh',
        'select[name="NewBooking.Customer_Id"]': 'vui lòng chọn khách hàng',
        '#stylistId': 'vui lòng chọn stylist',
        '#skinnerId': 'vui lòng chọn skinner',
        '#bookingDate': 'vui lòng chọn ngày',
        '#startSlotId': 'vui lòng chọn khung giờ'
    };

    for (let selector in requiredFields) {
        if (!$(selector).val()) {
            alert(requiredFields[selector].toUpperCase());
            e.preventDefault();
            return false;
        }
    }

    if (selectedServices.length === 0) {
        alert("VUI LÒNG CHỌN ÍT NHẤT 1 DỊCH VỤ");
        e.preventDefault();
        return false;
    }
});

$('#btnApplyCoupon').click(function () {
    let code = $('#couponCodeInput').val();
    let customerId = $('#customerId').val(); 

    let rawAmount = 0;
    selectedServices.forEach(s => {
        rawAmount += s.price;
    });

    if (!customerId) {
        showCouponMessage("Vui lòng chọn khách hàng trước khi áp mã", false);
        return;
    }

    let token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Admin/Coupon/ValidateCouponAjax',
        type: 'POST',
        data: {
            __RequestVerificationToken: token,
            code: code,
            orderAmount: rawAmount,
            userId: customerId 
        },
        success: function (res) {
            if (!res.succeeded) {
                showCouponMessage(res.errors[0], false);
                $('#couponDiscountValue').val(0);
            } else {
                $('#couponDiscountValue').val(res.discount);
                showCouponMessage("Áp dụng mã thành công!", true);
            }
            updateOnlyPrice();
        }
    });
});

function updateOnlyPrice() {
    let total = 0;
    selectedServices.forEach(s => {
        total += s.price;
    });

    const rankPercent = parseFloat($('#rankPercent').val()) || 0;
    const rankDiscount = total * rankPercent / 100;

    const couponDiscount = parseFloat($('#couponDiscountValue').val()) || 0;

    const finalTotal = Math.max(0, (total - rankDiscount) - couponDiscount);

    $('#txtRankDiscount').text('-' + rankDiscount.toLocaleString('vi-VN') + 'đ');
    $('#txtCouponDiscount').text('-' + couponDiscount.toLocaleString('vi-VN') + 'đ');
    $('#txtTotal').text(finalTotal.toLocaleString('vi-VN') + 'đ');
    $('#totalPriceInput').val(finalTotal);
}

function showCouponMessage(message, isSuccess) {
    const el = $('#couponMessage');
    el.text(message).removeClass('text-danger text-success').addClass(isSuccess ? 'text-success' : 'text-danger');
}