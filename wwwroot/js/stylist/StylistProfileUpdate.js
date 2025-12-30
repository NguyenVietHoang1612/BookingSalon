$(document).ready(function () {
    let currentStep = 1;

    $('.next-step').click(function () {
        $(`#step${currentStep}`).addClass('d-none');
        currentStep++;
        $(`#step${currentStep}`).removeClass('d-none');
        updateStepper();
    });

    $('.prev-step').click(function () {
        $(`#step${currentStep}`).addClass('d-none');
        currentStep--;
        $(`#step${currentStep}`).removeClass('d-none');
        updateStepper();
    });

    function updateStepper() {
        $('.step-item').removeClass('active');
        $(`.step-item[data-step="${currentStep}"]`).addClass('active');
    }
});

function removeImage(btn) {
    const wrapper = btn.closest('.image-item');
    wrapper.querySelector('input[name$=".IsRemoved"]').value = "true";
    wrapper.style.display = "none";
}

let selectedFiles = [];

//Xử lý khi chọn file mới
document.getElementById('imageInput').addEventListener('change', function (e) {
    const files = Array.from(e.target.files);

    files.forEach(file => {

        if (!selectedFiles.some(f => f.name === file.name && f.size === file.size)) {
            selectedFiles.push(file);
            previewNewFile(file);
        }
    });

    syncInputFiles();
});

//Render giao diện preview cho ảnh mới 
function previewNewFile(file) {
    const reader = new FileReader();
    reader.onload = e => {
        const div = document.createElement('div');
        div.className = 'image-item position-relative new-image-preview';

        const fileId = btoa(file.name).substring(0, 10);
        div.setAttribute('data-file-id', fileId);

        div.innerHTML = `
                    <img src="${e.target.result}"
                         style="width:120px;height:120px;object-fit:cover;border-radius:6px; border: 2px border-style: dashed; border-color: #0d6efd" />
                    <button type="button"
                            class="btn btn-danger btn-sm position-absolute top-0 end-0"
                            onclick="removeNewImage('${fileId}', '${file.name}')">
                        ×
                    </button>
                    <small class="d-block text-center text-primary">Ảnh mới</small>
                `;
        document.getElementById('imageList').appendChild(div);
    };
    reader.readAsDataURL(file);
}

// Xóa ảnh trong Db
function removeImage(btn) {
    const wrapper = btn.closest('.image-item');

    const inputRemoved = wrapper.querySelector('.is-removed-input');
    if (inputRemoved) {
        inputRemoved.value = "true";
        wrapper.style.display = "none";
    }
}

//Xóa ảnh mới vừa Upload 
function removeNewImage(fileId, fileName) {

    selectedFiles = selectedFiles.filter(f => btoa(f.name).substring(0, 10) !== fileId);

    const element = document.querySelector(`.new-image-preview[data-file-id="${fileId}"]`);
    if (element) element.remove();

    syncInputFiles();
}

// Hàm đồng bộ mảng selectedFiles vào input[type=file] để gửi lên Server
function syncInputFiles() {
    const dt = new DataTransfer();
    selectedFiles.forEach(file => dt.items.add(file));
    document.getElementById('imageInput').files = dt.files;
}