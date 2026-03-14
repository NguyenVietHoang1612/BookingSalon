
function confirmDelete(url, NameTitle) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Bạn chắc chắn muốn xóa: " + NameTitle + "?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Vâng, xóa nó!',
        cancelButtonColor: '#3085d6',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {

            fetch(url, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(response => {
                    if (response.ok) {
                        Swal.fire(
                            'Đã xóa!',
                            NameTitle + ' đã được xóa thành công.',
                            'success'
                        ).then(() => {
                            location.reload(); 
                        });
                    } else {
                        Swal.fire('Lỗi!', 'Server từ chối xóa hoặc không tìm thấy dữ liệu.', 'error');
                    }
                })
                .catch(error => {
                    Swal.fire('Lỗi!', 'Đã xảy ra lỗi kết nối.', 'error');
                });
        }
    })
}

function updateBookingStatus(id, status) {
    let title = 'Xác nhận thay đổi?';
    let text = 'Bạn có chắc chắn muốn thực hiện thao tác này?';
    let confirmButtonColor = '#3085d6';

    if (status === 1) { 
        title = 'Xác nhận đơn đặt?';
        text = 'Duyệt đơn này và khóa lịch cho thợ.';
        confirmButtonColor = '#28a745';
    } else if (status === 4) { 
        title = 'Hủy đơn đặt?';
        text = 'Hành động này sẽ giải phóng lịch của thợ và không thể hoàn tác!';
        confirmButtonColor = '#d33';
    } else if (status === 3) { 
        title = 'Hoàn thành dịch vụ?';
        text = 'Xác nhận khách đã thanh toán và hoàn tất.';
        confirmButtonColor = '#6c757d';
    }

    Swal.fire({
        title: title,
        text: text,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        confirmButtonText: 'Đồng ý',
        cancelButtonColor: '#aaa',
        cancelButtonText: 'Quay lại'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.showLoading();
            const url = `/Admin/Booking/Update?id=${id}&status=${status}`;

            fetch(url, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json()) 
                .then(data => {
                    if (data.succeeded) {
                        Swal.fire('Thành công!', 'Trạng thái đã được cập nhật.', 'success')
                            .then(() => location.reload());
                    } else {
                        const errorMsg = data.errors ? data.errors.join('<br>') : 'Không thể cập nhật.';
                        Swal.fire('Thất bại!', errorMsg, 'error');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    Swal.fire('Lỗi!', 'Hệ thống không phản hồi.', 'error');
                });
        }
    });
}

