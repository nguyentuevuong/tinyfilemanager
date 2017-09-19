$(document).ready(function () {
    if (!top.tinyfile || !top.tinyfile.fillTo || !top.tinyfile.fillToFolder) {
        $('.btn-apply-folder').addClass('disabled');
    } else {
        $('.btn-apply-folder').removeClass('disabled');
    }

    $('div.row.ff-filters').on('click', '.btn-upload', function () {
        var _this = $(this);
        if (_this.parents('.ff-filters').hasClass('disabled')) {
            return false;
        }
        $('div.row.ff-filters, div.row.ff-breadcrumb').addClass('disabled');
        $('div.row.uploader').removeClass('hidden');
        $('div.row.ff-find-box, div.row.ff-files').addClass('hidden');
    });

    $('.close-uploader').click(function () {
        $('div.row.uploader').addClass('hidden');
        $('div.row.ff-filters, div.row.ff-breadcrumb').removeClass('disabled');
        $('div.row.uploader form').removeClass('dz-started').children('.dz-preview').remove();
        $('div.row.ff-find-box, div.row.ff-files').removeClass('hidden');
        $('div.row.ff-find-box button.btn-search').trigger('click');
    });

    $('div.row.ff-filters').on('click', '.btn-new-folder', function () {
        var _this = $(this);
        if (_this.parents('.ff-filters').hasClass('disabled')) {
            return false;
        }
        bootbox.prompt("Nhập tên thư mục:", function (result) {
            if (result !== null) {
                $.ajax({
                    type: "POST",
                    url: "Ajax/CreateFolder.ashx",
                    data: {
                        folderName: result,
                        folderPath: $('input[name=folder]').val()
                    },
                    success: function (msg) {
                        $('div.row.ff-find-box button.btn-search').trigger('click');
                    },
                    error: function (msg) {
                    }
                });
            }
        });
    });

    /// Làm nổi bật tập tin
    $('div.row.ff-filters').on('change', 'input[name=radio-sort]', function () {
        var _this = $(this);
        if (_this.parents('.ff-filters').hasClass('disabled')) {
            return false;
        }
        $("label[class*='ff-label-type-']")
            .addClass('btn-info')
            .not('label[id=' + _this.data('item') + ']')
            .removeClass('btn-info');

        if (_this.data('item') == 'ff-item-type-all') {
            $("div[class*='ff-item-type-']").fadeTo(500, 1).removeClass('disabled');
        }
        else {
            $("div[class*='ff-item-type-']").not("div." + _this.data('item')).fadeTo(500, 0.1).addClass('disabled');
            $("div." + _this.data('item')).fadeTo(500, 1).removeClass('disabled');
        }
    });

    // Nhảy đến thư mục
    $('div.row.ff-breadcrumb').on('click', 'a', function () {
        var _this = $(this);
        if (_this.parents('.ff-breadcrumb').hasClass('disabled')) {
            return false;
        }
        $('input[name=folder]').val($(this).data('path'));
        $('div.row.ff-find-box button.btn-search').trigger('click');
    });

    /// Nhập liệu
    $('div.row.ff-find-box').on('click', '.btn-back', function () {
        $('input[name=folder]').val($('input[name=folder]').val().substring(0, $('input[name=folder]').val().lastIndexOf('/')));
        $('div.row.ff-find-box button.btn-search').trigger('click');
    });

    $('div.row.ff-find-box').on('click', '.btn-apply-folder', function () {
        var _this = $(this);
        if (_this.hasClass('disabled')) {
            return false;
        }
        if (top.tinyfile && top.tinyfile.fillTo) {
            top.tinyfile.fillTo.val(_tinypath + '/' + $('input[name=folder]').val());
            $(window.parent.document).find('.close-button:first').trigger('click');
            return;
        }
    });

    $('div.row.ff-find-box').on('click', '.btn-search', function () {
        var filter = $('div.row.ff-find-box input[type=text].form-control').val() || '';
        $.ajax({
            method: "POST",
            url: "Ajax/GetFiles.ashx",
            data: {
                filter: filter,
                folderPath: $('input[name=folder]').val()
            },
            success: function (msg) {
                $('div.row.ff-files').empty();
                $(JSON.parse(msg)).each(function (i, item) {
                    var html = "<div class='col-lg-2 col-md-3 col-sm-3 col-xs-6 ff-item-type-" + item.Type + "'>" +
                               "<div class='thumbnail' title='" + item.Title + "' data-url='" + item.Url + "' data-name='" + item.Title + "' data-type='" + item.Type + "'>" +
                               "<img src='" + item.Image + "' alt='" + item.Title + "' style='height: 125px; width: 185px;'/>" +
                               "<div class='caption text-center'><span>" + item.Label + "</span></div>" +
                               "<div class='row'><div class='col-md-12 text-center'><div class='btn-group btn-group-sm toolbox'>" +
                               "<button type='button' class='btn btn-default btn-delete' title='Xóa'><i class='glyphicon glyphicon-trash'></i></button>" +
                               "<a href='" + item.Url + "' download='" + item.Title + "' class='btn btn-default btn-download " + (item.Type == 0 ? "disabled" : "") + "' title='Tải xuống'><i class='glyphicon glyphicon-download'></i></a>" +
                               "<button type='button' class='btn btn-default btn-preview " + (item.Type != 2 ? "disabled" : "") + "' title='Xem trước ảnh'><i class='glyphicon glyphicon-eye-open'></i></button>" +
                               "<button type='button' class='btn btn-default btn-copy-url' title='Sao chép đường link'><i class='glyphicon glyphicon-copy'></i></button>" +
                               "<button type='button' class='btn btn-default btn-apply " + (item.Type == 0 ? (!top.tinyfile || !top.tinyfile.fillTo ? "disabled" : !top.tinyfile.fillToFolder ? "disabled" : "") : (top.tinyfile && top.tinyfile.fillToFolder ? "disabled" : "")) + "' title='Chèn vào trình soạn thảo'><i class='glyphicon glyphicon-check'></i></button>" +
                               "</div></div></div></div></div>";
                    $('div.row.ff-files').append(html);
                });
                $.ajax({
                    method: "POST",
                    url: "ajax/breadcrumb.ashx",
                    success: function (data) {
                        $('div.row.ff-breadcrumb > div.col-md-12').html(data);
                    },
                    error: function (msg) {
                    }
                })

                if ($('input[name=folder]').val() == '' || $('input[name=folder]').val() == '/') {
                    $('div.row.ff-find-box .btn-group').addClass('hidden');
                } else {
                    $('div.row.ff-find-box .btn-group').removeClass('hidden');
                }
                $('input[name=radio-sort]:checked').trigger('change');
            },
            error: function (msg) {
            }
        });
        $('body').removeAttr('style');
    });

    $('div.row.ff-find-box button.btn-search').trigger('click');

    $('div.row.ff-find-box input[type=text].form-control').keyup(function () {
        $('div.row.ff-find-box button.btn-search').trigger('click');
    });

    $('div.ff-files').on('click', 'img:not(".disabled")', function () {
        var _this = $(this);
        if (_this.parents('[class*=ff-item-type-]').hasClass('disabled')) {
            return false;
        }
        if (_this.parents('.thumbnail').data('type') == 0) {
            $('input[name=folder]').val($('input[name=folder]').val() + ($('input[name=folder]').val() == '/' ? '' : '/') + _this.parents('.thumbnail').data('name'));
            $('div.row.ff-find-box button.btn-search').trigger('click');
        } else {
            _this.parents('.thumbnail').find('.btn-preview').trigger('click');
        }
    });

    $('div.ff-files').on('click', '.btn-delete:not(".disabled")', function () {
        var _this = $(this);
        if (_this.parents('[class*=ff-item-type-]').hasClass('disabled')) {
            return false;
        }
        bootbox.confirm("Bạn có chắc chắn muốn xóa " + (_this.parents('.thumbnail').data('type') == 0 ? "thư mục" : "tập tin") + " này không?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "Ajax/Delete.ashx",
                    data: {
                        folderPath: $('input[name=folder]').val(),
                        fileName: _this.parents('.thumbnail').data('name')
                    },
                    success: function (msg) {
                        if (msg == "true") {
                            bootbox.alert('Xóa thành công ' + (_this.parents('.thumbnail').data('type') == 0 ? "thư mục" : "tập tin") + ' có tên: ' + _this.parents('.thumbnail').data('name') + '!');
                            $('div.row.ff-find-box button.btn-search').trigger('click');
                            $('body').removeAttr('style');
                        }
                        else {
                            bootbox.alert('Thư mục đang có tập tin hoặc tập tin bị cấm xóa!');
                            $('body').removeAttr('style');
                        }
                    },
                    error: function (msg) {
                    }
                });
            }
            $('body').removeAttr('style');
        });
    });

    $('div.ff-files').on('click', '.btn-download:not(".disabled")', function () {
        if ($(this).parents('[class*=ff-item-type-]').hasClass('disabled')) {
            return false;
        }
    });

    $('div.ff-files').on('click', '.btn-preview:not(".disabled")', function () {
        var _this = $(this);
        if (_this.parents('[class*=ff-item-type-]').hasClass('disabled')) {
            return false;
        }
        var _dir = $('<div>', { 'class': 'col-md-12 text-center' }).append($('<div>', { 'class': 'thumbnail' }).append($('<img>', { 'src': _this.parents('.thumbnail').data('url'), 'alt': '', 'style': 'max-height: 420px; max-width: 548px;' })));
        $('.modal .modal-footer > button:nth-child(1)').text('Đóng');
        $('.modal .modal-header, .modal .modal-footer > button:nth-child(2)').addClass('hidden');
        $('.modal .modal-body').empty().append($('<div>', { 'class': 'row' }).append(_dir));
        $('.modal').modal();
    });

    $('div.ff-files').on('click', '.btn-copy-url:not(".disabled")', function () {
        var _this = $(this);
        if (_this.parents('[class*=ff-item-type-]').hasClass('disabled')) {
            return false;
        }
        $('body').append($('<input>', { 'type': 'text', 'class': 'copy-text', 'value': _this.parents('.thumbnail').data('url') }));
        document.querySelector('.copy-text').select();
        document.execCommand('copy');
        $('.copy-text').remove();
    });

    $('div.ff-files').on('click', '.btn-apply:not(".disabled")', function () {
        var _this = $(this);
        if (_this.parents('[class*=ff-item-type-]').hasClass('disabled')) {
            return false;
        }

        if (top.tinyfile && top.tinyfile.fillTo) {
            top.tinyfile.fillTo.val(_this.parents('.thumbnail').data('url'));
            $(window.parent.document).find('.close-button:first').trigger('click');
            return;
        }
        if (top.tinyMCE && top.tinyMCE.activeEditor) {
            if (_this.parents('.thumbnail').data('type') == 3) {
                var image = $('<div>').append($('<a>', { 'href': _this.parents('.thumbnail').data('url'), }).text(_this.parents('.thumbnail').data('name'))).html();
                top.tinyMCE.activeEditor.execCommand('mceInsertContent', false, image);
                $(window.parent.document).find('.mce-tinyfile:first .mce-close:first').trigger('click');
            } else if (_this.parents('.thumbnail').data('type') == 2) {
                $('.modal .modal-footer > button:nth-child(1)').text('Hủy chèn');
                $('.modal .modal-header, .modal .modal-footer > button').removeClass('hidden');
                $('.modal .modal-body').empty()
                    .append($('<div>', { 'class': 'row' })
                        .append($('<div>', { 'class': 'col-md-12' })
                            .append($('<div>', { 'class': 'thumbnail' })
                                .append($('<img>', { 'src': _this.parents('.thumbnail').data('url'), 'style': 'max-height: 250px; max-width: 560px;' }))
                            )
                        )
                    )
                    .append($('<div>', { 'class': 'row' })
                        .append($('<div>', { 'class': 'col-md-6 col-sm-6' })
                            .append($('<div>', { 'class': 'form-group' })
                                .append($('<label>', { 'class': 'label-control' }).text('Chiều rộng:'))
                                .append($('<div>', { 'class': 'input-group' })
                                    .append($('<input>', { 'class': 'form-control img-width', 'type': 'text', 'val': '530' }))
                                    .append($('<span>', { 'class': 'input-group-addon' }).text('px'))
                                )
                            )
                        )
                        .append($('<div>', { 'class': 'col-md-6 col-sm-6' })
                            .append($('<div>', { 'class': 'form-group' })
                                .append($('<label>', { 'class': 'label-control' }).text('Chiều cao:'))
                                .append($('<div>', { 'class': 'input-group' })
                                    .append($('<input>', { 'class': 'form-control img-height', 'type': 'text', 'val': '230' }))
                                    .append($('<span>', { 'class': 'input-group-addon' }).text('px'))
                                )
                            )
                        )
                    );
                $('.modal').modal();
                $('.img-width').focus();

                $('.modal .modal-footer').on('click', 'button:nth-child(2)', function () {
                    var image = $('<div>').append($('<img>', { 'src': _this.parents('.thumbnail').data('url'), 'width': ($('.img-width').val() || 530), 'height': ($('.img-height').val() || 230) })).html();
                    top.tinyMCE.activeEditor.execCommand('mceInsertContent', false, image);
                    $(window.parent.document).find('.mce-tinyfile:first .mce-close:first').trigger('click');
                });
            }
        }
    });
});