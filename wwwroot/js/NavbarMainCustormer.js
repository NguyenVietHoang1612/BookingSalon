document.addEventListener('DOMContentLoaded', function () {
    const mainMenu = document.getElementById('mainMenu');
    const userDropdown = document.querySelector('.user-dropdown');
    const userIcon = document.getElementById('userIcon');
    const toursDropdown = document.querySelector('.tours-dropdown');

    // --- Hàm đóng tất cả các Dropdown (User và Tours) ---
    function closeAllDropdowns() {
        if (userDropdown) {
            userDropdown.classList.remove('show');
        }
        if (toursDropdown) {
            toursDropdown.classList.remove('show');
        }
    }

    // --- Hàm quản lý class 'active' ---
    function setActiveMenu(targetItem) {
        if (!mainMenu) return;

        const allMenuItems = mainMenu.querySelectorAll('.menu-item');

        // 1. XÓA class 'active' khỏi TẤT CẢ các mục menu chính
        allMenuItems.forEach(item => {
            item.classList.remove('active');
        });

        // 2. THÊM class 'active' vào mục được chỉ định
        if (targetItem) {
            targetItem.classList.add('active');
        }
    }

    // --- Logic cho Tours Dropdown (Gộp cả Mở/Đóng và Active) ---
    if (toursDropdown) {
        toursDropdown.addEventListener('click', function (event) {
            const clickedLink = event.target.closest('a');

            if (!clickedLink) return;

            // 1. Xử lý click vào mục con (Kiểu Tour/Loại Tour)
            if (clickedLink.classList.contains('dropdown-item')) {
                // event.preventDefault(); // Ngăn chuyển trang nếu cần
                closeAllDropdowns(); // Đóng dropdown

                // Gán active cho nút Tours cha
                setActiveMenu(toursDropdown);

                console.log(`Chuyển đến trang con: ${clickedLink.dataset.page || clickedLink.textContent}`);
                return; // Kết thúc xử lý
            }

            // 2. Xử lý click vào nút Tours chính (chỉ để mở/đóng dropdown)
            if (clickedLink.closest('.tours-dropdown') === toursDropdown) {
                event.stopPropagation();

                // Đóng dropdown khác nếu đang mở
                if (userDropdown && userDropdown.classList.contains('show')) {
                    userDropdown.classList.remove('show');
                }

                // Bật/tắt dropdown
                toursDropdown.classList.toggle('show');
            }
        });
    }

    // --- Logic cho Các Menu Chính Khác (Trang Chủ, Giới Thiệu, ...) ---
    if (mainMenu) {
        mainMenu.addEventListener('click', function (event) {
            const clickedLink = event.target.closest('a');

            if (clickedLink) {
                const parentItem = clickedLink.closest('.menu-item');

                // Bỏ qua click vào User và Tours (vì chúng đã được xử lý riêng)
                if (parentItem === toursDropdown || clickedLink.closest('.user-dropdown')) {
                    return;
                }

                // Xử lý click vào các nút Menu Chính khác
                if (parentItem) {
                    event.preventDefault(); // Ngăn chuyển trang nếu cần
                    closeAllDropdowns();
                    setActiveMenu(parentItem); // Đặt active cho nút vừa click

                    console.log(`Chuyển đến trang: ${parentItem.querySelector('a').dataset.page}`);
                }
            }
        });
    }

    // --- Logic cho User Dropdown (Mở/Đóng bằng Click) ---
    if (userIcon && userDropdown) {
        userIcon.addEventListener('click', function (event) {
            event.stopPropagation();

            if (toursDropdown && toursDropdown.classList.contains('show')) {
                toursDropdown.classList.remove('show');
            }

            userDropdown.classList.toggle('show');
        });

        // Ngăn chặn đóng dropdown khi click vào nội dung bên trong user dropdown
        userDropdown.addEventListener('click', function (event) {
            event.stopPropagation();
            // Optional: Đóng sau khi chọn mục con
            // userDropdown.classList.remove('show');
        });
    }

    // --- Đóng tất cả dropdown khi click bất cứ đâu trên document ---
    document.addEventListener('click', function (event) {
        if (toursDropdown && !toursDropdown.contains(event.target)) {
            toursDropdown.classList.remove('show');
        }
        if (userDropdown && !userDropdown.contains(event.target)) {
            userDropdown.classList.remove('show');
        }
    });

    // --- Thiết lập Trang Chủ active ban đầu khi tải trang ---
    const homeItem = mainMenu ? mainMenu.querySelector('[data-page="home"]').closest('.menu-item') : null;
    setActiveMenu(homeItem);
});