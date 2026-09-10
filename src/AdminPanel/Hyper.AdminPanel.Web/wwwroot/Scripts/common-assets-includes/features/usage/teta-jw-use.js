$('[data-video-stream]').each(function (idx, item) {
    var fileUrl = $(item).data('video-stream');
    jwplayer(item.id).setup({
        file: fileUrl,
        playbackRateControls: [0.25, 0.5, 0.75, 1, 1.5, 2, 4]
    });
});