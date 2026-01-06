// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function confirmDelete(url, Name) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Bạn chắc chắn muốn xóa role: " + Name + "?",
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
                            'Người dùng đã được chuyển sang trạng thái ngừng hoạt động.',
                            'success'
                        ).then(() => {
                            location.reload(); // Load lại trang để cập nhật danh sách
                        });
                    } else {
                        Swal.fire('Lỗi!', 'Không thể thực hiện yêu cầu.', 'error');
                    }
                })
                .catch(error => {
                    Swal.fire('Lỗi!', 'Đã xảy ra lỗi kết nối.', 'error');
                });
        }
    })
}

function updateBookingStatus(id, status) {
    const title = status === 'confirm' ? 'Xác nhận đơn đặt?' : 'Hủy đơn đặt?';
    const text = status === 'confirm' ? 'Bạn muốn xác nhận đơn này?' : 'Hành động này không thể hoàn tác!';
    const confirmButtonColor = status === 'confirm' ? '#28a745' : '#d33';

    Swal.fire({
        title: title,
        text: text,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        confirmButtonText: 'Đồng ý',
        cancelButtonColor: '#3085d6',
        cancelButtonText: 'Quay lại'
    }).then((result) => {
        if (result.isConfirmed) {
  
            const url = `/Admin/Booking/Update?id=${id}&status=${status}`;

            fetch(url, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(response => {
                    if (response.ok) {
                        Swal.fire('Thành công!', 'Trạng thái đã được cập nhật.', 'success')
                            .then(() => location.reload());
                    } else {
                        Swal.fire('Lỗi!', 'Không thể cập nhật trạng thái.', 'error');
                    }
                })
                .catch(error => {
                    Swal.fire('Lỗi!', 'Đã xảy ra lỗi kết nối.', 'error');
                });
        }
    });
}