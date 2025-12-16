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

        allMenuItems.forEach(item => {
            item.classList.remove('active');
        });

        if (targetItem) {
            targetItem.classList.add('active');
        }
    }

    if (toursDropdown) {
        toursDropdown.addEventListener('click', function (event) {
            const clickedLink = event.target.closest('a');

            if (!clickedLink) return;


            if (clickedLink.classList.contains('dropdown-item')) {
  
                closeAllDropdowns(); 

                setActiveMenu(toursDropdown);

                console.log(`Chuyển đến trang con: ${clickedLink.dataset.page || clickedLink.textContent}`);
                return; 
            }

            if (clickedLink.closest('.tours-dropdown') === toursDropdown) {
                event.stopPropagation();

                if (userDropdown && userDropdown.classList.contains('show')) {
                    userDropdown.classList.remove('show');
                }

                toursDropdown.classList.toggle('show');
            }
        });
    }

    if (mainMenu) {
        mainMenu.addEventListener('click', function (event) {
            const clickedLink = event.target.closest('a');

            if (clickedLink) {
                const parentItem = clickedLink.closest('.menu-item');

                if (parentItem === toursDropdown || clickedLink.closest('.user-dropdown')) {
                    return;
                }

                if (parentItem) {
                    event.preventDefault(); 
                    closeAllDropdowns();
                    setActiveMenu(parentItem); 

                    console.log(`Chuyển đến trang: ${parentItem.querySelector('a').dataset.page}`);
                }
            }
        });
    }

    if (userIcon && userDropdown) {
        userIcon.addEventListener('click', function (event) {
            event.stopPropagation();

            if (toursDropdown && toursDropdown.classList.contains('show')) {
                toursDropdown.classList.remove('show');
            }

            userDropdown.classList.toggle('show');
        });

        userDropdown.addEventListener('click', function (event) {
            event.stopPropagation();
        });
    }

    document.addEventListener('click', function (event) {
        if (toursDropdown && !toursDropdown.contains(event.target)) {
            toursDropdown.classList.remove('show');
        }
        if (userDropdown && !userDropdown.contains(event.target)) {
            userDropdown.classList.remove('show');
        }
    });

    const homeItem = mainMenu ? mainMenu.querySelector('[data-page="home"]').closest('.menu-item') : null;
    setActiveMenu(homeItem);
});