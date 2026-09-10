/**
 * دکمه Clear برای کمبوباکس‌ها
 * امکان پاک کردن مقدار کمبو با کلیک روی دکمه ×
 */

/**
 * پاک کردن مقدار کمبو
 * @param {string} fieldName - نام فیلد کمبو
 */
function clearComboValue(fieldName) {
    const comboElement = document.getElementById('field-' + fieldName);
    
    if (!comboElement) {
        console.error('Combo element not found:', fieldName);
        return;
    }
    
    // پاک کردن مقدار
    comboElement.value = '';
    
    // اگر کمبو remote data دارد، باید select2 را هم clear کنیم
    if ($(comboElement).data('select2')) {
        $(comboElement).val('').trigger('change');
    }
    
    // Trigger change event برای اجرای logic ها
    const event = new Event('change', { bubbles: true });
    comboElement.dispatchEvent(event);
    
    // اگر jQuery select2 هست
    $(comboElement).trigger('change');
    
    console.log('Combo cleared:', fieldName);
}

/**
 * Initialize combo clear buttons
 * نمایش/مخفی کردن دکمه clear بر اساس مقدار کمبو
 */
function initComboClearButtons() {
    document.querySelectorAll('select.cBox').forEach(function(select) {
        const wrapper = select.parentElement;
        const clearBtn = wrapper.querySelector('.combo-clear-btn');
        if (!clearBtn) return;
        
        // Function to update button visibility
        function updateClearButton() {
            const hasValue = select.value && 
                           select.value !== '' && 
                           select.value !== 'null';
            
            if (hasValue) {
                wrapper.classList.add('has-value');
            } else {
                wrapper.classList.remove('has-value');
            }
        }
        
        // Initial state
        updateClearButton();
        
        // Update on change
        select.addEventListener('change', updateClearButton);
        
        // For select2
        $(select).on('change', updateClearButton);
        
        // For multi-select
        if (select.multiple) {
            select.addEventListener('input', updateClearButton);
        }
    });
}

// Initialize on page load
$(document).ready(function() {
    initComboClearButtons();
});

// Re-initialize after AJAX updates
$(document).ajaxComplete(function() {
    initComboClearButtons();
});

// Export for manual initialization
window.initComboClearButtons = initComboClearButtons;
window.clearComboValue = clearComboValue;

