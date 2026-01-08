$(document).ready(function () {
    let currentStep = 1;
    let selectedFiles = [];

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

    $('#ImageUploads').on('change', function (e) {
        const files = e.target.files;

        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            if (!file.type.match('image.*')) continue;

            selectedFiles.push(file);

            const reader = new FileReader();

            reader.onload = function (event) {
                const html = `
                            <div class="preview-wrapper position-relative" data-name="${file.name}">
                                <img src="${event.target.result}" class="image-preview img-thumbnail"
                                        style="width: 120px; height: 120px; object-fit: cover;">
                                <span class="remove-img shadow"
                                        style="position: absolute; top: -10px; right: -10px; background: red;
                                        color: white; border-radius: 50%; width: 25px; height: 25px;
                                        text-align: center; cursor: pointer; line-height: 22px; font-weight: bold;">
                                        &times;
                                </span>
                            </div>`;

                $('#imagePreviewContainer').append(html);
            };

            reader.readAsDataURL(file);
        }

        $(this).val('');
    });


    $(document).on('click', '.remove-img', function () {
        const name = $(this).parent().attr('data-name');
        selectedFiles = selectedFiles.filter(file => file.name !== name);
        $(this).parent().remove();
    });

    $('#btnSubmitForm').on("click", function (e) {
        let formData = new FormData();
        let otherData = $('#stylistForm').serializeArray();

        $.each(otherData, function (key, input) {
            formData.append(input.name, input.value);
        });

        for (let i = 0; i < selectedFiles.length; i++) {
            formData.append("ImageUploads", selectedFiles[i]);
        }

        $.ajax({
            url: $('#stylistForm').attr('action'),
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res, status, xhr) {
                window.location.href = 'Index';
            },
            error: function (xhr) {
                if (xhr.status === 400) {
                    alert("Lỗi dữ liệu: " + xhr.responseText);
                } else {
                    alert("Đã có lỗi hệ thống xảy ra.");
                }
            }
        });
    });
});