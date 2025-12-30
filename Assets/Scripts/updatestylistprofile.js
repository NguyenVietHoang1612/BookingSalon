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

let newFiles = [];

document.getElementById('imageInput').addEventListener('change', function (e) {
    for (let file of e.target.files) {
        newFiles.push(file);
        previewNewFile(file);
    }
    syncInputFiles();
});

function previewNewFile(file) {
    const reader = new FileReader();
    reader.onload = e => {
        const div = document.createElement('div');
        div.classList.add('image-item', 'new-image');
        div.dataset.name = file.name;

        div.innerHTML = `
                    <img src="${e.target.result}">
                    <button type="button" onclick="removeNewImage('${file.name}')">×</button>
                `;
        document.getElementById('imageList').appendChild(div);
    };
    reader.readAsDataURL(file);
}

function removeNewImage(name) {
    newFiles = newFiles.filter(f => f.name !== name);
    document.querySelector(`.new-image[data-name="${name}"]`).remove();
    syncInputFiles();
}

function syncInputFiles() {
    const dt = new DataTransfer();
    newFiles.forEach(f => dt.items.add(f));
    document.getElementById('imageInput').files = dt.files;
}