/**
 * SBVR Business Rules Tooltip and Modal Initialization
 * This script initializes Bootstrap tooltips and modals for SBVR icons
 */

(function() {
    'use strict';

    /**
     * Move SBVR modals to body to prevent positioning issues
     */
    function moveSBVRModalsToBody() {
        $('.sbvr-modal').each(function() {
            var $modal = $(this);
            // Only move if not already in body
            if ($modal.parent()[0] !== document.body) {
                $modal.appendTo('body');
            }
        });
    }

    /**
     * Initialize SBVR tooltips
     */
    function initializeSBVRTooltips() {
        // Initialize all SBVR icons with tooltips (not modals)
        $('[data-toggle="tooltip"].sbvr-icon').tooltip({
            html: true,
            trigger: 'hover focus',
            placement: 'auto',
            container: 'body',
            template: '<div class="tooltip sbvr-tooltip" role="tooltip">' +
                      '<div class="arrow"></div>' +
                      '<div class="tooltip-inner"></div>' +
                      '</div>'
        });

        // Close tooltip when clicking outside (but not for modal icons)
        $(document).on('click', function(e) {
            if (!$(e.target).closest('.sbvr-icon').length && 
                !$(e.target).closest('.tooltip').length) {
                $('[data-toggle="tooltip"].sbvr-icon').tooltip('hide');
            }
        });
    }

    /**
     * Initialize SBVR modals
     */
    function initializeSBVRModals() {
        // Move all modals to body
        moveSBVRModalsToBody();
        
        // Ensure modal is at body level when showing
        $(document).on('show.bs.modal', '.sbvr-modal', function() {
            var $modal = $(this);
            if ($modal.parent()[0] !== document.body) {
                $modal.appendTo('body');
            }
        });
        
        // Cleanup modal-open class if no modals are visible
        $(document).on('hidden.bs.modal', '.sbvr-modal', function() {
            if ($('.modal:visible').length === 0) {
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }
        });
    }

    /**
     * Initialize all SBVR features
     */
    function initializeSBVR() {
        initializeSBVRTooltips();
        initializeSBVRModals();
    }

    /**
     * Initialize on document ready
     */
    $(document).ready(function() {
        initializeSBVR();
    });

    /**
     * Reinitialize after dynamic content loads
     */
    $(document).ajaxComplete(function() {
        // Destroy old tooltips
        $('[data-toggle="tooltip"].sbvr-icon').tooltip('dispose');
        // Reinitialize
        initializeSBVR();
    });

    /**
     * Export for manual initialization if needed
     */
    window.SBVR = {
        init: initializeSBVR,
        initTooltips: initializeSBVRTooltips,
        initModals: initializeSBVRModals
    };

})();

