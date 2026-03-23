function initSelectSearch(elementId, customPlaceholder = "Tìm kiếm...") {
    const element = document.getElementById(elementId);

    if (element) {
        return new Choices(element, {
            searchEnabled: true,          
            itemSelectText: '',           
            removeItemButton: true,      
            noResultsText: 'Không tìm thấy kết quả',
            noChoicesText: 'Hết lựa chọn',
            placeholder: true,
            placeholderValue: customPlaceholder,
            searchPlaceholderValue: 'Gõ để tìm...',
            allowHTML: true,
        });
    }
}

function previewImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('previewAvatar').src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    }
}