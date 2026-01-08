let bookingPayload = { branchId: null, stylistId: null, date: null, slotId: null };

// Hàm mở Modal dịch vụ
function openServiceModal() {
    var modalElem = document.getElementById('serviceModal');
    var modal = bootstrap.Modal.getInstance(modalElem) || new bootstrap.Modal(modalElem);
    modal.show();
}

$('#branchSelect').change(function () {
    const val = $(this).val();
    $('#hiddenBranchId').val(val);
    if (val) {
        bookingPayload.branchId = val;

        $('#step-2').removeClass('opacity-50 pe-none');
        $('#step-2').find('.badge').addClass('bg-warning text-dark').removeClass('bg-secondary');
        $('#servicePlaceholder').text('Mời anh chọn dịch vụ...');

        if (selectedServices.length > 0) {
            loadStylists(val);
            bookingPayload.stylistId = null;
            bookingPayload.slotId = null;
            $('#timeSlotContainer').addClass('d-none');
            $('#btnSubmit').addClass('disabled btn-secondary').removeClass('btn-warning text-dark shadow');
        }
    } else {
        resetFrom(2);
        $('#servicePlaceholder').text('Anh vui lòng chọn salon trước...');
    }
});

let selectedServices = [];

function toggleService(id, name, price, duration) {
    const index = selectedServices.findIndex(s => s.id === id);
    if (index > -1) {
        selectedServices.splice(index, 1);
        $(`#check-${id}`).prop('checked', false);
        $(`#item-${id}`).removeClass('selected');
    } else {
        selectedServices.push({ id, name, price, duration });
        $(`#check-${id}`).prop('checked', true);
        $(`#item-${id}`).addClass('selected');
    }
    updateModalSummary();
}

function updateModalSummary() {
    const count = selectedServices.length;
    const total = selectedServices.reduce((sum, item) => sum + item.price, 0);

    $('#modalCount').text(`Đã chọn ${count} dịch vụ`);
    $('#modalTotal').text(`${total.toLocaleString('vi-VN')} VNĐ`);
}

// hàm chức năng cho nút xong trong chọn service
function confirmServices() {
    if (selectedServices.length === 0) {
        alert("Vui lòng chọn ít nhất 1 dịch vụ");
        return;
    }
    let totalTime = 0;
    selectedServices.forEach(s => {
        totalTime += s.duration;
    });
    let hiddenHtml = '';
    selectedServices.forEach((s, index) => {
        hiddenHtml += `<input type="hidden" name="BookingDetails[${index}].Service_Id" value="${s.id}" />`;
    });
    $('#hiddenServiceInputs').html(hiddenHtml);

    let htmlBadges = '';
    selectedServices.forEach(s => {
        htmlBadges += `<span class="badge bg-warning text-dark p-2 animate__animated animate__fadeIn">${s.name}</span>`;
    });
    $('#selectedServicesList').html(htmlBadges);

    const finalTotal = selectedServices.reduce((sum, item) => sum + item.price, 0);
    $('input[name="NewBooking.TotalPrice"]').val(finalTotal);

    $('#totalPriceDisplay').html(`
                <div>Tổng thanh toán: <span class="fs-7">${finalTotal.toLocaleString('vi-VN')} VNĐ</span></div>
                <div class="text-info small">Dự kiến thực hiện: ${totalTime} phút</div>
            `);


    $('#step-3').removeClass('opacity-50 pe-none');
    $('#step-3').find('.badge').addClass('bg-warning text-dark').removeClass('bg-secondary');

    bootstrap.Modal.getInstance(document.getElementById('serviceModal')).hide();

    loadStylists(bookingPayload.branchId);
}

function loadStylists(branchId) {

    $('#stylistSlider').html('<div class="text-center w-100 py-3"><div class="spinner-border spinner-border-sm text-warning"></div></div>');

    $.get('/Booking/GetStylists', { branchId: branchId }, function (data) {
        let html = '';
        if (data && data.length > 0) {
            html += `
                        <div class="stylist-item" onclick="onStylistSelect('auto', this)">
                            <img src="/media/users/default-stylist.png" class="stylist-avatar mb-1" style="filter: grayscale(1);">
                            <div class="small fw-bold">Chọn hộ anh</div>
                        </div>`;

            data.forEach(s => {
                html += `
                            <div class="stylist-item" onclick="onStylistSelect('${s.id}', this)">
                                <img src="/media/users/${s.img}" class="stylist-avatar mb-1" onerror="this.src='/media/users/default-stylist.png'">
                                <div class="small">${s.name}</div>
                            </div>`;
            });
        } else {
            html = '<div class="text-secondary small ps-2 py-3">Rất tiếc, chi nhánh này hiện chưa có stylist sẵn sàng.</div>';
        }
        $('#stylistSlider').html(html);
    }).fail(function () {
        $('#stylistSlider').html('<div class="text-danger small">Lỗi nạp dữ liệu stylist.</div>');
    });
}

// 4. Lazy Loading Khung giờ
function onStylistSelect(id, el) {
    $('.stylist-item').removeClass('active');
    $(el).addClass('active');
    bookingPayload.stylistId = id;
    bookingPayload.date = $('#bookingDate').val();

    $('#timeSlotContainer').removeClass('d-none');
    $('#slotGrid').html('<div class="col-12 text-center py-3"><div class="spinner-border spinner-border-sm text-warning"></div></div>');

    $.get('/Booking/GetSlots', { stylistId: bookingPayload.stylistId, dateStr: bookingPayload.date }, function (slots) {
        let html = '';
        if (slots && slots.length > 0) {
            slots.forEach(slot => {
                const disabled = slot.isAvailable ? '' : 'disabled';
                const opacity = slot.isAvailable ? '' : 'opacity-25';
                html += `
                <div style="width: calc(25% - 8px);">
                    <button type="button" class="btn slot-btn ${disabled} ${opacity}" onclick="onSlotSelect(${slot.slotId}, this)">
                        ${slot.time}
                    </button>
                </div>`;
            });
        } else {
            html = '<div class="col-12 text-center small text-danger">Hết khung giờ trống trong ngày này.</div>';
        }
        $('#slotGrid').html(html);
    });

    $('#hiddenStylistId').val(id);

    let dateVal = $('#bookingDate').val();
    if (!dateVal) {
        dateVal = new Date().toISOString().split('T')[0];
    }
    $('#hiddenDate').val(dateVal);
}

function onSlotSelect(id, el) {
    $('.slot-btn').removeClass('active');
    $(el).addClass('active');
    bookingPayload.slotId = id;
    $('#hiddenSlotId').val(id);
    $('#btnSubmit').removeClass('disabled btn-secondary').addClass('btn-warning text-dark shadow');
}

function resetFrom(step) {
    for (let i = step; i <= 3; i++) {
        $(`#step-${i}`).addClass('opacity-50 pe-none');
        $(`#step-${i}`).find('.badge').removeClass('bg-warning text-dark').addClass('bg-secondary');
    }

    if (step <= 2) {
        selectedServices = [];
        $('#selectedServicesList').empty();
        $('#totalPriceDisplay').empty();
        $('#modalCount').text(`Đã chọn 0 dịch vụ`);
        $('#modalTotal').text(`0 VNĐ`);
        $('.service-checkbox').prop('checked', false);
        $('.service-item').removeClass('selected');
    }

    if (step <= 3) {
        bookingPayload.stylistId = null;
        bookingPayload.slotId = null;
        $('#stylistSlider').empty();
        $('#timeSlotContainer').addClass('d-none');
        $('#btnSubmit').addClass('disabled btn-secondary').removeClass('btn-warning text-dark shadow');
    }


    $('#btnSubmit').removeClass('disabled btn-secondary').addClass('btn-warning text-dark shadow');
}