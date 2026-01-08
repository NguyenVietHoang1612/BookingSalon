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
        });
    }
}