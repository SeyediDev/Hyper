//dependent on ControlBindingsManager from '../Form.js'
//dependent on '../../I18n' window.tetaI18n
//dependent on window.FormFileManager
//dependent on $
//dependent on fineUploader

(function() {
    window.ControlBindingsManager.registerJsControlIO('FineUploader',
        {
            obtainOutputData: function(controlId) {
                var uploads = $('[id="' + controlId + '"]').fineUploader('getUploads');
                if (Array.isArray(uploads) && uploads.length > 0) {
                    var upload = uploads[0];
                    if (upload.status === "upload successful")
                        return {
                            uuid: upload.uuid,
                            action: window.FormFileManager.eSubmitAction.Move
                        };
                    else throw window.tetaI18n.t('UnableToSubmitWhileUploading');
                }
                return null;
            },
            provideInitialData: function(initialData) {
                return null;
            }
        });
})();