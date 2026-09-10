/**
 * Report Tree Cleanup
 * حذف اعداد count از node های jstree در Report
 * 
 * مشکل: اعداد مثل (5) در کنار نام folder ها نمایش داده میشن که گیج کننده است
 * راه حل: حذف اعداد و اتکا به آیکن folder/arrow برای نمایش وجود children
 */

(function() {
    'use strict';

    /**
     * حذف اعداد count از text های jstree
     * مثال: "فولدر اصلی (5)" => "فولدر اصلی"
     */
    function removeCountFromTreeNodes(treeSelector) {
        const tree = $(treeSelector);
        if (!tree.length) return;

        // منتظر میمونیم تا tree بارگذاری بشه
        tree.on('ready.jstree refresh.jstree', function() {
            tree.find('.jstree-anchor').each(function() {
                const $anchor = $(this);
                let text = $anchor.text().trim();
                
                // حذف اعداد داخل پرانتز از انتهای متن
                // الگو: هر چیزی که با فاصله + (عدد) تمام بشه
                // مثال: "Reports (12)" => "Reports"
                // مثال: "گزارشات (۵)" => "گزارشات"
                text = text.replace(/\s*\(\d+\)\s*$/g, ''); // انگلیسی
                text = text.replace(/\s*\([۰-۹]+\)\s*$/g, ''); // فارسی
                
                // فقط text node رو عوض کنیم، نه icon ها
                const textNode = $anchor.contents().filter(function() {
                    return this.nodeType === Node.TEXT_NODE;
                }).first();
                
                if (textNode.length && textNode[0].nodeValue !== text) {
                    textNode[0].nodeValue = text;
                }
            });
        });
    }

    /**
     * اطمینان از RTL بودن tree برای فارسی
     */
    function ensureRTLDirection(treeSelector) {
        const tree = $(treeSelector);
        if (!tree.length) return;

        // اضافه کردن کلاس jstree-rtl برای RTL
        if (!tree.hasClass('jstree-rtl')) {
            tree.addClass('jstree-rtl');
        }
        
        // اطمینان از direction: rtl
        tree.css('direction', 'rtl');
    }

    /**
     * Initialize tree cleanup
     */
    function initTreeCleanup() {
        // Configs Tree
        if ($('#configs-tree').length) {
            ensureRTLDirection('#configs-tree');
            removeCountFromTreeNodes('#configs-tree');
        }

        // Scheduled Reports Tree
        if ($('#scheduledReport-tree').length) {
            ensureRTLDirection('#scheduledReport-tree');
            removeCountFromTreeNodes('#scheduledReport-tree');
        }
    }

    // Initialize on document ready
    $(document).ready(function() {
        initTreeCleanup();
    });

    // Re-initialize after AJAX content loads
    $(document).ajaxComplete(function() {
        initTreeCleanup();
    });

    // Export for manual initialization if needed
    window.ReportTreeCleanup = {
        init: initTreeCleanup,
        removeCount: removeCountFromTreeNodes,
        ensureRTL: ensureRTLDirection
    };
})();

