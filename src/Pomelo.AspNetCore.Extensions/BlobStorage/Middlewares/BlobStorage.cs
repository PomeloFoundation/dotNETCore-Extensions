using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AspNetCore.Extensions.BlobStorage;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace Microsoft.AspNetCore.Builder
{
    public static class BlobStorage
    {
        public static IApplicationBuilder UseBlobStorage<TModel, TKey>(this IApplicationBuilder self, string path = "/scripts/jquery.pomelo.fileupload.js", string fileFormName="file", string controller = "file", string downloadAction = "download", string uploadAction="upload", string uploadRouteName="FileUpload", string downloadRouteName = "FileDownload")
            where TKey : IEquatable<TKey>
            where TModel : Blob<TKey>, new()
        {
            #region Download

            var endpoint1 = new RouteHandler(async context => {
                var bs = context.RequestServices.GetRequiredService<IBlobStorageProvider<TModel, TKey>>();
                var strId = context.GetRouteValue("id").ToString();
                object id;
                if (typeof(TKey) == typeof(short))
                    id = Convert.ToInt16(strId);
                else if (typeof(TKey) == typeof(int))
                    id = Convert.ToInt32(strId);
                else if (typeof(TKey) == typeof(long))
                    id = Convert.ToInt64(strId);
                else if (typeof(TKey) == typeof(Guid))
                    id = Guid.Parse(strId);
                else if (typeof(TKey) == typeof(string))
                    id = strId;
                else
                    throw new NotSupportedException(typeof(TKey).FullName);
                var blob = bs.Get((TKey)id);
                var auth = context.RequestServices.GetService<IBlobAccessAuthorizationProvider<TKey>>();
                if (blob == null)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Not Found");
                }
                else if (auth != null && !auth.IsAbleToDownload(blob.Id))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden");
                }
                else
                {
                    context.Response.ContentType = blob.ContentType;
                    context.Response.Headers["Content-length"] = blob.Bytes.Length.ToString();
                    if (blob.ContentType.IndexOf("image") < 0)
                        context.Response.Headers["Content-disposition"] = $"attachment; filename={WebUtility.UrlEncode(blob.FileName)}";
                    else
                        context.Response.Headers["Cache-Control"] = $"max-age={ 60 * 24 }";
                    context.Response.Body.Write(blob.Bytes, 0, blob.Bytes.Length);
                }
            });
            var routeBuilder1 = new RouteBuilder(self);
            routeBuilder1.DefaultHandler = endpoint1;
            routeBuilder1.MapRoute(downloadRouteName, controller + "/"+ downloadAction +"/{id}");
            #endregion
            #region Upload
            self.Map("/" + controller + "/" + uploadAction, config =>
            {
                config.Run(async context =>
                {
                    var auth = context.RequestServices.GetService<IBlobUploadAuthorizationProvider>();
                    var handler = context.RequestServices.GetService<IBlobHandler<TModel, TKey>>();
                    if (auth != null && !auth.IsAbleToUpload())
                    {
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Forbidden");
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var bs = context.RequestServices.GetRequiredService<IBlobStorageProvider<TModel, TKey>>();
                        var file = context.Request.Form.Files["file"];
                        if (file != null)
                        {
                            var f = new TModel
                            {
                                Time = DateTime.Now,
                                ContentType = file.ContentType,
                                ContentLength = file.Length,
                                FileName = file.GetFileName(),
                                Bytes = file.ReadAllBytes()
                            };
                            if (handler != null)
                                f = handler.Handle(f, new Base64StringFile(f.Bytes, file.ContentType));
                            var id = bs.Set(f);
                            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(f));
                        }
                        else
                        {
                            var blob = new Base64StringFile(context.Request.Form["file"]);
                            var f = new TModel
                            {
                                Time = DateTime.Now,
                                ContentType = blob.ContentType,
                                ContentLength = blob.Base64String.Length,
                                FileName = "file",
                                Bytes = blob.AllBytes
                            };
                            if (handler != null)
                                f = handler.Handle(f, new Base64StringFile(f.Bytes, f.Bytes.Length.ToString()));
                            var id = bs.Set(f);
                            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(f));
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Bad Request");
                    }
                });
            });
            #endregion
            #region Javascript
            self.Map(path, config =>
            {
                config.Run(async context =>
                {
                    await context.Response.WriteAsync(@"(function ($, window) {
    'use strict';

    var supported = (window.File && window.FileReader && window.FileList);

    /**
     * @options
     * @param action [string] 'Where to submit uploads'
     * @param label [string] <'Drag and drop files or click to select'> 'Dropzone text'
     * @param maxQueue [int] <2> 'Number of files to simultaneously upload'
     * @param maxSize [int] <5242880> 'Max file size allowed'
     * @param postData [object] 'Extra data to post with upload'
     * @param postKey [string] <'file'> 'Key to upload file as'
     */

    var options = {
        action: '',
        label: 'Drag and drop files or click to select',
        maxQueue: 2,
        maxSize: 5242880, // 5 mb
        postData: {},
        postKey: 'file'
    };

    /**
     * @events
     * @event start.dropper ''
     * @event complete.dropper ''
     * @event fileStart.dropper ''
     * @event fileProgress.dropper ''
     * @event fileComplete.dropper ''
     * @event fileError.dropper ''
     */

    var pub = {

        /**
         * @method
         * @name defaults
         * @description Sets default plugin options
         * @param opts [object] <{}> 'Options object'
         * @example $.dropper('defaults', opts);
         */
        defaults: function (opts) {
            options = $.extend(options, opts || {});
            return $(this);
        }
    };

    /**
     * @method private
     * @name _init
     * @description Initializes plugin
     * @param opts [object] 'Initialization options'
     */
    function _init(opts) {
        var $items = $(this);

        if (supported) {
            // Settings
            opts = $.extend({}, options, opts);

            // Apply to each element
            for (var i = 0, count = $items.length; i < count; i++) {
                _build($items.eq(i), opts);
            }
        }

        return $items;
    }

    /**
     * @method private
     * @name _build
     * @description Builds each instance
     * @param $nav [jQuery object] 'Target jQuery object'
     * @param opts [object] <{}> 'Options object'
     */
    function _build($dropper, opts) {
        opts = $.extend({}, opts, $dropper.data('dropper-options'));
        $dropper.addClass('dropper');

        var data = $.extend({
            $dropper: $dropper,
            $input: $dropper.parents().find('.dropper-input'),
            queue: [],
            total: 0,
            uploading: false
        }, opts);

        $dropper.on('click.dropper', data, _onClick)
            .on('dragenter.dropper', data, _onDragEnter)
            .on('dragover.dropper', data, _onDragOver)
            .on('dragleave.dropper', data, _onDragOut)
            .on('drop.dropper', data, _onDrop)
            .data('dropper', data);

        data.$input.on('change.dropper', data, _onChange);
    }

    /**
     * @method private
     * @name _onClick
     * @description Handles click to dropzone
     * @param e [object] 'Event data'
     */
    function _onClick(e) {
        e.stopPropagation();
        e.preventDefault();

        var data = e.data;

        data.$input.trigger('click');
    }

    /**
     * @method private
     * @name _onChange
     * @description Handles change to hidden input
     * @param e [object] 'Event data'
     */
    function _onChange(e) {
        e.stopPropagation();
        e.preventDefault();

        var data = e.data,
            files = data.$input[0].files;

        if (files.length) {
            _handleUpload(data, files);
        }
    }

    /**
     * @method private
     * @name _onDragEnter
     * @description Handles dragenter to dropzone
     * @param e [object] 'Event data'
     */
    function _onDragEnter(e) {
        e.stopPropagation();
        e.preventDefault();

        var data = e.data;

        data.$dropper.addClass('dropping');
    }

    /**
     * @method private
     * @name _onDragOver
     * @description Handles dragover to dropzone
     * @param e [object] 'Event data'
     */
    function _onDragOver(e) {
        e.stopPropagation();
        e.preventDefault();

        var data = e.data;

        data.$dropper.addClass('dropping');
    }

    /**
     * @method private
     * @name _onDragOut
     * @description Handles dragout to dropzone
     * @param e [object] 'Event data'
     */
    function _onDragOut(e) {
        e.stopPropagation();
        e.preventDefault();

        var data = e.data;

        data.$dropper.removeClass('dropping');
    }

    /**
     * @method private
     * @name _onDrop
     * @description Handles drop to dropzone
     * @param e [object] 'Event data'
     */
    function _onDrop(e) {
        e.preventDefault();

        var data = e.data,
            files = e.originalEvent.dataTransfer.files;

        data.$dropper.removeClass('dropping');

        _handleUpload(data, files);
    }

    /**
     * @method private
     * @name _handleUpload
     * @description Handles new files
     * @param data [object] 'Instance data'
     * @param files [object] 'File list'
     */
    function _handleUpload(data, files) {
        var newFiles = [];

        for (var i = 0; i < files.length; i++) {
            var file = {
                index: data.total++,
                file: files[i],
                name: files[i].name,
                size: files[i].size,
                started: false,
                complete: false,
                error: false,
                transfer: null
            };

            newFiles.push(file);
            data.queue.push(file);
        }

        if (!data.uploading) {
            $(window).on('beforeunload.dropper', function () {
                return 'You have uploads pending, are you sure you want to leave this page?';
            });

            data.uploading = true;
        }

        data.$dropper.trigger('start.dropper', [newFiles]);

        _checkQueue(data);
    }

    /**
     * @method private
     * @name _checkQueue
     * @description Checks and updates file queue
     * @param data [object] 'Instance data'
     */
    function _checkQueue(data) {
        var transfering = 0,
            newQueue = [];

        // remove lingering items from queue
        for (var i in data.queue) {
            if (data.queue.hasOwnProperty(i) && !data.queue[i].complete && !data.queue[i].error) {
                newQueue.push(data.queue[i]);
            }
        }

        data.queue = newQueue;

                for (var j in data.queue) {
            if (data.queue.hasOwnProperty(j)) {
                if (!data.queue[j].started) {
                    var formData = function () {
                        var fd = new FormData();
                        fd.append(data.postKey, data.queue[j].file);
                        if (!data.postData)
                            return fd;
                        var pd = data.postData();
                        for (var k in pd) {
                            if (pd.hasOwnProperty(k)) {
                                fd.append(k, pd[k]);
                            }
                        }
                        return fd;
                    };

                    _uploadFile(data, data.queue[j], formData);
                }

                transfering++;

                if (transfering >= data.maxQueue) {
                    return;
                } else {
                    i++;
                }
            }
        }

        if (transfering === 0) {
            $(window).off('beforeunload.dropper');

            data.uploading = false;

            data.$dropper.trigger('complete.dropper');
        }
    }

    /**
     * @method private
     * @name _uploadFile
     * @description Uploads file
     * @param data [object] 'Instance data'
     * @param file [object] 'Target file'
     * @param formData [object] 'Target form'
     */
    function _uploadFile(data, file, formData) {
        if (file.size >= data.maxSize) {
            file.error = true;
            data.$dropper.trigger('fileError.dropper', [file, 'Too large']);

            _checkQueue(data);
        } else {
            file.started = true;
            file.transfer = $.ajax({
                url: data.action,
                data: formData(),
                type: 'POST',
                dataType: 'json',
                contentType: false,
                processData: false,
                cache: false,
                xhr: function () {
                    var $xhr = $.ajaxSettings.xhr();

                    if ($xhr.upload) {
                        $xhr.upload.addEventListener('progress', function (e) {
                            var percent = 0,
                                position = e.loaded || e.position,
                                total = e.total;

                            if (e.lengthComputable) {
                                percent = Math.ceil(position / total * 100);
                            }

                            data.$dropper.trigger('fileProgress.dropper', [file, percent]);
                        }, false);
                    }

                    return $xhr;
                },
                beforeSend: function (e) {
                    data.$dropper.trigger('fileStart.dropper', [file]);
                },
                success: function (response, status, jqXHR) {
                    file.complete = true;
                    data.$dropper.trigger('fileComplete.dropper', [file, response, jqXHR]);

                    _checkQueue(data);
                },
                error: function (jqXHR, status, error) {
                    file.error = true;
                    data.$dropper.trigger('fileError.dropper', [file, error]);

                    _checkQueue(data);
                }
            });
        }
    }

    $.fn.dropper = function (method) {
        if (pub[method]) {
            return pub[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return _init.apply(this, arguments);
        }
        return this;
    };

    $.dropper = function (method) {
        if (method === 'defaults') {
            pub.defaults.apply(this, Array.prototype.slice.call(arguments, 1));
        }
    };
})(jQuery, window);

(function () {
    var $, Paste, createHiddenEditable, dataURLtoBlob;

    $ = window.jQuery;

    $.paste = function (pasteContainer) {
        var pm;
        if (typeof console !== 'undefined' && console !== null) {
            console.log('DEPRECATED: This method is deprecated. Please use $.fn.pastableNonInputable() instead.');
        }
        pm = Paste.mountNonInputable(pasteContainer);
        return pm._container;
    };

    $.fn.pastableNonInputable = function () {
        var el, _i, _len;
        for (_i = 0, _len = this.length; _i < _len; _i++) {
            el = this[_i];
            Paste.mountNonInputable(el);
        }
        return this;
    };

    $.fn.pastableTextarea = function () {
        var el, _i, _len;
        for (_i = 0, _len = this.length; _i < _len; _i++) {
            el = this[_i];
            Paste.mountTextarea(el);
        }
        return this;
    };

    $.fn.pastableContenteditable = function () {
        var el, _i, _len;
        for (_i = 0, _len = this.length; _i < _len; _i++) {
            el = this[_i];
            Paste.mountContenteditable(el);
        }
        return this;
    };

    dataURLtoBlob = function (dataURL, sliceSize) {
        var b64Data, byteArray, byteArrays, byteCharacters, byteNumbers, contentType, i, m, offset, slice, _ref;
        if (sliceSize == null) {
            sliceSize = 512;
        }
        if (!(m = dataURL.match(/^data\:([^\;]+)\;base64\,(.+)$/))) {
            return null;
        }
        _ref = m, m = _ref[0], contentType = _ref[1], b64Data = _ref[2];
        byteCharacters = atob(b64Data);
        byteArrays = [];
        offset = 0;
        while (offset < byteCharacters.length) {
            slice = byteCharacters.slice(offset, offset + sliceSize);
            byteNumbers = new Array(slice.length);
            i = 0;
            while (i < slice.length) {
                byteNumbers[i] = slice.charCodeAt(i);
                i++;
            }
            byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
            offset += sliceSize;
        }
        return new Blob(byteArrays, {
            type: contentType
        });
    };

    createHiddenEditable = function () {
        return $(document.createElement('div')).attr('contenteditable', true).css({
            width: 1,
            height: 1,
            position: 'fixed',
            left: -100,
            overflow: 'hidden'
        });
    };

    Paste = (function () {
        Paste.prototype._target = null;

        Paste.prototype._container = null;

        Paste.mountNonInputable = function (nonInputable) {
            var paste;
            paste = new Paste(createHiddenEditable().appendTo(nonInputable), nonInputable);
            $(nonInputable).on('click', (function (_this) {
                return function () {
                    return paste._container.focus();
                };
            })(this));
            paste._container.on('focus', (function (_this) {
                return function () {
                    return $(nonInputable).addClass('pastable-focus');
                };
            })(this));
            return paste._container.on('blur', (function (_this) {
                return function () {
                    return $(nonInputable).removeClass('pastable-focus');
                };
            })(this));
        };

        Paste.mountTextarea = function (textarea) {
            var ctlDown, paste;
            if (-1 !== navigator.userAgent.toLowerCase().indexOf('chrome')) {
                return this.mountContenteditable(textarea);
            }
            paste = new Paste(createHiddenEditable().insertBefore(textarea), textarea);
            ctlDown = false;
            $(textarea).on('keyup', function (ev) {
                var _ref;
                if ((_ref = ev.keyCode) === 17 || _ref === 224) {
                    return ctlDown = false;
                }
            });
            $(textarea).on('keydown', function (ev) {
                var _ref;
                if ((_ref = ev.keyCode) === 17 || _ref === 224) {
                    ctlDown = true;
                }
                if (ctlDown && ev.keyCode === 86) {
                    return paste._container.focus();
                }
            });
            $(paste._target).on('pasteImage', (function (_this) {
                return function () {
                    return $(textarea).focus();
                };
            })(this));
            $(paste._target).on('pasteText', (function (_this) {
                return function () {
                    return $(textarea).focus();
                };
            })(this));
            $(textarea).on('focus', (function (_this) {
                return function () {
                    return $(textarea).addClass('pastable-focus');
                };
            })(this));
            return $(textarea).on('blur', (function (_this) {
                return function () {
                    return $(textarea).removeClass('pastable-focus');
                };
            })(this));
        };

        Paste.mountContenteditable = function (contenteditable) {
            var paste;
            paste = new Paste(contenteditable, contenteditable);
            $(contenteditable).on('focus', (function (_this) {
                return function () {
                    return $(contenteditable).addClass('pastable-focus');
                };
            })(this));
            return $(contenteditable).on('blur', (function (_this) {
                return function () {
                    return $(contenteditable).removeClass('pastable-focus');
                };
            })(this));
        };

        function Paste(_at__container, _at__target) {
            this._container = _at__container;
            this._target = _at__target;
            this._container = $(this._container);
            this._target = $(this._target).addClass('pastable');
            this._container.on('paste', (function (_this) {
                return function (ev) {
                    var clipboardData, file, item, reader, text, _i, _j, _len, _len1, _ref, _ref1, _ref2, _ref3, _results;
                    if (((_ref = ev.originalEvent) != null ? _ref.clipboardData : void 0) != null) {
                        clipboardData = ev.originalEvent.clipboardData;
                        if (clipboardData.items) {
                            _ref1 = clipboardData.items;
                            for (_i = 0, _len = _ref1.length; _i < _len; _i++) {
                                item = _ref1[_i];
                                if (item.type.match(/^image\//)) {
                                    reader = new FileReader();
                                    reader.onload = function (event) {
                                        return _this._handleImage(event.target.result);
                                    };
                                    reader.readAsDataURL(item.getAsFile());
                                }
                                if (item.type === 'text/plain') {
                                    item.getAsString(function (string) {
                                        return _this._target.trigger('pasteText', {
                                            text: string
                                        });
                                    });
                                }
                            }
                        } else {
                            if (-1 !== Array.prototype.indexOf.call(clipboardData.types, 'text/plain')) {
                                text = clipboardData.getData('Text');
                                _this._target.trigger('pasteText', {
                                    text: text
                                });
                            }
                            _this._checkImagesInContainer(function (src) {
                                return _this._handleImage(src);
                            });
                        }
                    }
                    if (clipboardData = window.clipboardData) {
                        if ((_ref2 = (text = clipboardData.getData('Text'))) != null ? _ref2.length : void 0) {
                            return _this._target.trigger('pasteText', {
                                text: text
                            });
                        } else {
                            _ref3 = clipboardData.files;
                            _results = [];
                            for (_j = 0, _len1 = _ref3.length; _j < _len1; _j++) {
                                file = _ref3[_j];
                                _this._handleImage(URL.createObjectURL(file));
                                _results.push(_this._checkImagesInContainer(function () { }));
                            }
                            return _results;
                        }
                    }
                };
            })(this));
        }

        Paste.prototype._handleImage = function (src) {
            var loader;
            loader = new Image();
            loader.onload = (function (_this) {
                return function () {
                    var blob, canvas, ctx, dataURL;
                    canvas = document.createElement('canvas');
                    canvas.width = loader.width;
                    canvas.height = loader.height;
                    ctx = canvas.getContext('2d');
                    ctx.drawImage(loader, 0, 0, canvas.width, canvas.height);
                    dataURL = null;
                    try {
                        dataURL = canvas.toDataURL('image/png');
                        blob = dataURLtoBlob(dataURL);
                    } catch (_error) { }
                    if (dataURL) {
                        return _this._target.trigger('pasteImage', {
                            blob: blob,
                            dataURL: dataURL,
                            width: loader.width,
                            height: loader.height
                        });
                    }
                };
            })(this);
            return loader.src = src;
        };

        Paste.prototype._checkImagesInContainer = function (cb) {
            var img, timespan, _i, _len, _ref;
            timespan = Math.floor(1000 * Math.random());
            _ref = this._container.find('img');
            for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                img = _ref[_i];
                img['_paste_marked_' + timespan] = true;
            }
            return setTimeout((function (_this) {
                return function () {
                    var _j, _len1, _ref1, _results;
                    _ref1 = _this._container.find('img');
                    _results = [];
                    for (_j = 0, _len1 = _ref1.length; _j < _len1; _j++) {
                        img = _ref1[_j];
                        if (!img['_paste_marked_' + timespan]) {
                            cb(img.src);
                        }
                        _results.push($(img).remove());
                    }
                    return _results;
                };
            })(this), 1);
        };

        return Paste;

    })();

}).call(this);

(function($, undefined) {
    $.fn.getCursorPosition = function() {
        var el = $(this).get(0);
        var pos = 0;
        if ('selectionStart' in el) {
            pos = el.selectionStart;
        } else if ('selection' in document) {
            el.focus();
            var Sel = document.selection.createRange();
            var SelLength = document.selection.createRange().text.length;
            Sel.moveStart('character', -el.value.length);
            pos = Sel.text.length - SelLength;
        }
        return pos;
    }
})(jQuery);

(function($) {
    $.fn.dragDropOrPaste = function(onUploading, onUploaded, postData) {
        var obj = this;
        this.dropper({
            action: '/" + controller + "/" + uploadAction + @"',
            maxQueue: 1,
            postData: postData || null
        })
        .on('fileStart.dropper', function (file) {
            var postargs = { };
            if (postData)
            {
                var pd = postData();
                for (var x in pd)
                    postargs[x] = pd[x];
            }

            if (onUploading)
                onUploading();
        })
        .on('fileComplete.dropper', function (file, res, ret) {
            if (onUploaded)
                onUploaded(ret);
        });
        
        this.pastableTextarea();
        this.on('pasteImage', function (ev, data) {
            var pos = obj.getCursorPosition();
            var str = obj.val();
            var postargs = { };
            if (postData)
            {
                var pd = postData();
                for (var x in pd)
                    postargs[x] = pd[x];
            }
            if (onUploading)
                onUploading();
            postargs.file = data.dataURL;
            $.post('/" + controller + "/" + uploadAction + @"', postargs, function (result) {
                if (onUploaded)
                    onUploaded(result);
            }, 'json');
        });
    }
})(jQuery);");
                });
            });
            #endregion

            return self.UseRouter(routeBuilder1.Build());
        }
        public static IApplicationBuilder UseBlobStorage(this IApplicationBuilder self, string path = "/scripts/jquery.pomelo.fileupload.js", string fileFormName = "file", string controller = "file", string downloadAction = "download", string uploadAction = "upload", string uploadRouteName = "FileUpload", string downloadRouteName = "FileDownload")
        {
            return self.UseBlobStorage<Blob, Guid>(path, fileFormName, controller, downloadAction, uploadAction, uploadRouteName, downloadRouteName);
        }
    }
}
