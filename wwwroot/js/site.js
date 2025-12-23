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