/**
 * Boolean Button Group for Filter Controls
 * Converts radio button boolean controls to modern button groups
 */

$(document).ready(function () {
    initBooleanButtonGroups();
});

function initBooleanButtonGroups() {
    // Find all boolean radio button containers in filter
    $('.modern-filter-wrapper .dWrapper').each(function () {
        var $container = $(this);
        var $radioButtons = $container.find('input[type="radio"]');
        
        if ($radioButtons.length === 0) return;
        
        // Check if this is a boolean field (has values 0, 1, 2, or 3)
        var values = $radioButtons.map(function() { return $(this).val(); }).get();
        var isBooleanField = values.every(function(val) {
            return val === '' || val === '0' || val === '1' || val === '2' || val === '3';
        });
        
        if (!isBooleanField) return;
        
        // Get the field name from first radio button
        var fieldName = $radioButtons.first().attr('name');
        if (!fieldName) return;
        
        // Check if already converted
        if ($container.find('.boolean-button-group').length > 0) return;
        
        // Get labels and current value
        var trueLabel = $container.find('label:contains("بله"), label:contains("true"), label:contains("Yes")').first().text().trim() || 'بله';
        var falseLabel = $container.find('label:contains("خیر"), label:contains("false"), label:contains("No")').first().text().trim() || 'خیر';
        var allLabel = 'همه';
        
        // Get current value
        var currentValue = $container.find('input[type="radio"]:checked').val();
        
        // Hide original radio buttons and their parent labels
        $container.find('label').has('input[type="radio"]').hide();
        // Also hide the btn-group container
        $container.find('.btn-group').hide();
        
        // Create button group
        var $buttonGroup = $('<div class="boolean-button-group"></div>');
        
        // Check which radio buttons exist
        var hasTrue = $radioButtons.filter('[value="1"]').length > 0;
        var hasFalse = $radioButtons.filter('[value="0"]').length > 0;
        var hasAll = $radioButtons.filter('[value="2"]').length > 0;
        
        // Create buttons only if corresponding radio exists (using numeric values: 0=False, 1=True, 2=All)
        var $btnTrue, $btnFalse, $btnAll;
        
        if (hasTrue) {
            $btnTrue = $('<button type="button" class="boolean-btn btn-yes" data-value="1">بله</button>');
        }
        if (hasAll) {
            $btnAll = $('<button type="button" class="boolean-btn btn-all" data-value="2">همه</button>');
        }
        if (hasFalse) {
            $btnFalse = $('<button type="button" class="boolean-btn btn-no" data-value="0">خیر</button>');
        }
        
        // Set active state based on current value
        if (currentValue === '2' && $btnAll) {
            $btnAll.addClass('active');
        } else if (currentValue === '1' && $btnTrue) {
            $btnTrue.addClass('active');
        } else if (currentValue === '0' && $btnFalse) {
            $btnFalse.addClass('active');
        } else if ($btnAll) {
            // Default to All if no value selected and All exists
            $btnAll.addClass('active');
        } else if ($btnTrue) {
            // If no All button, default to True
            $btnTrue.addClass('active');
        }
        
        // Add click handlers
        function handleButtonClick(btn, value) {
            if (!btn) return;
            $(btn).on('click', function () {
                // Remove active from all
                $buttonGroup.find('.boolean-btn').removeClass('active');
                // Add active to clicked
                $(this).addClass('active');
                
                // Update hidden radio buttons
                $radioButtons.prop('checked', false);
                var $targetRadio = $radioButtons.filter('[value="' + value + '"]');
                if ($targetRadio.length > 0) {
                    $targetRadio.prop('checked', true).trigger('change');
                }
            });
        }
        
        handleButtonClick($btnTrue, '1');
        handleButtonClick($btnAll, '2');
        handleButtonClick($btnFalse, '0');
        
        // Append buttons to group (in order: True, All, False)
        if ($btnTrue) $buttonGroup.append($btnTrue);
        if ($btnAll) $buttonGroup.append($btnAll);
        if ($btnFalse) $buttonGroup.append($btnFalse);
        
        // Append to container after label
        $container.find('label').first().after($buttonGroup);
    });
}

// Re-initialize on dynamic content load
$(document).on('DOMContentLoaded', initBooleanButtonGroups);

