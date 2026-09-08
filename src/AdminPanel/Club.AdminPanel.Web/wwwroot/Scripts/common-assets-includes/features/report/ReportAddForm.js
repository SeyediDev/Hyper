/**
 * Report Add Form - Visual Type Selection
 * تبدیل ComboBox نوع گزارش به انتخاب دیداری با Radio Buttons
 */

(function() {
    'use strict';

    // Report type definitions
    const reportTypes = {
        'Chart': {
            icon: 'fa fa-bar-chart',
            title: 'نمودار',
            description: 'گزارش به صورت نمودار (Chart)',
            englishTitle: 'Chart'
        },
        'ReportList': {
            icon: 'fa fa-list',
            title: 'لیست گزارش',
            description: 'گزارش به صورت لیست (Report List)',
            englishTitle: 'Report List'
        },
        'GroupByList': {
            icon: 'fa fa-table',
            title: 'لیست گروه‌بندی شده',
            description: 'گزارش به صورت جدول گروه‌بندی شده (Group By List)',
            englishTitle: 'Group By List'
        }
    };

    /**
     * Initialize visual report type selection
     */
    function initializeVisualSelection() {
        // Check if already initialized
        if (document.querySelector('.report-type-selection-container')) {
            return; // Already initialized
        }

        // Find the report type select/combobox - try multiple selectors
        const selectors = [
            'select[name*="Type"]',
            'select[name*="type"]',
            'select[id*="Type"]',
            'select[id*="type"]',
            '#ReportType',
            '#reportType',
            'select[name="ReportType"]',
            'select[name="reportType"]',
            'select[name="Type"]',
            'select[name="type"]'
        ];
        
        let reportTypeSelect = null;
        for (let i = 0; i < selectors.length; i++) {
            reportTypeSelect = document.querySelector(selectors[i]);
            if (reportTypeSelect) {
                break;
            }
        }
        
        if (!reportTypeSelect) {
            console.log('Report type select not found. Form might be loaded differently. Selectors tried:', selectors);
            return;
        }

        // Hide the original select
        reportTypeSelect.classList.add('report-type-combobox-hidden');
        reportTypeSelect.style.display = 'none';

        // Get the form container
        const formContainer = reportTypeSelect.closest('form') || 
                             reportTypeSelect.closest('.modal-body') || 
                             reportTypeSelect.closest('.form-container') ||
                             document.body;

        // Find or create the selection container
        let selectionContainer = document.querySelector('.report-type-selection-container');
        
        if (!selectionContainer) {
            selectionContainer = document.createElement('div');
            selectionContainer.className = 'report-type-selection-container';
            
            // Insert before the select element
            reportTypeSelect.parentNode.insertBefore(selectionContainer, reportTypeSelect);
        }

        // Create label
        const label = document.createElement('label');
        label.className = 'report-type-selection-label';
        label.textContent = window.tetaI18n && window.tetaI18n.t ? 
            window.tetaI18n.t('Report Type') || 'نوع گزارش' : 'نوع گزارش';
        selectionContainer.appendChild(label);

        // Create options container
        const optionsContainer = document.createElement('div');
        optionsContainer.className = 'report-type-options';
        selectionContainer.appendChild(optionsContainer);

        // Create visual cards for each report type
        Object.keys(reportTypes).forEach(function(type) {
            const typeInfo = reportTypes[type];
            const card = createReportTypeCard(type, typeInfo, reportTypeSelect);
            optionsContainer.appendChild(card);
        });

        // Set initial selection
        const currentValue = reportTypeSelect.value;
        if (currentValue) {
            selectReportType(currentValue, reportTypeSelect);
        }
    }

    /**
     * Create a report type card
     */
    function createReportTypeCard(type, typeInfo, selectElement) {
        const card = document.createElement('div');
        card.className = 'report-type-card';
        card.setAttribute('data-type', type);
        card.setAttribute('role', 'button');
        card.setAttribute('tabindex', '0');
        card.setAttribute('aria-label', typeInfo.title);

        // Radio input (hidden)
        const radio = document.createElement('input');
        radio.type = 'radio';
        radio.name = selectElement.name || 'ReportType';
        radio.value = type;
        radio.id = 'report-type-' + type.toLowerCase();
        card.appendChild(radio);

        // Icon
        const icon = document.createElement('div');
        icon.className = 'report-type-icon';
        const iconElement = document.createElement('i');
        iconElement.className = typeInfo.icon;
        icon.appendChild(iconElement);
        card.appendChild(icon);

        // Title
        const title = document.createElement('div');
        title.className = 'report-type-title';
        title.textContent = typeInfo.title;
        card.appendChild(title);

        // Description
        const description = document.createElement('div');
        description.className = 'report-type-description';
        description.textContent = typeInfo.description;
        card.appendChild(description);

        // Click handler
        card.addEventListener('click', function(e) {
            e.preventDefault();
            selectReportType(type, selectElement);
        });

        // Keyboard handler
        card.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                selectReportType(type, selectElement);
            }
        });

        // Update when radio changes
        radio.addEventListener('change', function() {
            if (this.checked) {
                selectReportType(type, selectElement);
            }
        });

        return card;
    }

    /**
     * Select a report type
     */
    function selectReportType(type, selectElement) {
        // Update select value
        selectElement.value = type;
        
        // Trigger change event on select
        const changeEvent = new Event('change', { bubbles: true });
        selectElement.dispatchEvent(changeEvent);

        // Update visual selection
        document.querySelectorAll('.report-type-card').forEach(function(card) {
            card.classList.remove('selected');
            const radio = card.querySelector('input[type="radio"]');
            if (radio) {
                radio.checked = false;
            }
        });

        const selectedCard = document.querySelector('.report-type-card[data-type="' + type + '"]');
        if (selectedCard) {
            selectedCard.classList.add('selected');
            const radio = selectedCard.querySelector('input[type="radio"]');
            if (radio) {
                radio.checked = true;
            }
        }
    }

    /**
     * Initialize when DOM is ready
     */
    function init() {
        // Try to initialize immediately
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', initializeVisualSelection);
        } else {
            initializeVisualSelection();
        }

        // Also try after a short delay (for dynamically loaded content)
        setTimeout(initializeVisualSelection, 500);
        setTimeout(initializeVisualSelection, 1000);
        setTimeout(initializeVisualSelection, 2000);
    }

    // Initialize
    init();

    // Expose function for manual initialization
    window.initializeReportTypeVisualSelection = initializeVisualSelection;

    // Listen for iframe content loaded (in case form is in iframe)
    window.addEventListener('message', function(event) {
        if (event.data && (event.data.type === 'iframe-loaded' || event.data.type === 'initializeReportTypeSelection')) {
            setTimeout(initializeVisualSelection, 100);
        }
    });
    
    // Also listen for MutationObserver to detect dynamically added selects
    if (typeof MutationObserver !== 'undefined') {
        var observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                if (mutation.addedNodes.length > 0) {
                    // Check if a select element was added
                    for (var i = 0; i < mutation.addedNodes.length; i++) {
                        var node = mutation.addedNodes[i];
                        if (node.nodeType === 1) { // Element node
                            if (node.tagName === 'SELECT' || node.querySelector('select[name*="Type"], select[name*="type"]')) {
                                setTimeout(initializeVisualSelection, 200);
                                break;
                            }
                        }
                    }
                }
            });
        });
        
        // Start observing
        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

})();

