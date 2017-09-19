# tinyfilemanager
file manager for tinymce


## required

dotnet version 4.6.2

## using

tiny file for tinymce

## Hướng dẫn sử dụng

Publish project này, nhúng vào một thư mục bất kỳ trong project chính (webform, mvc). Tạo một iframe trỏ đường link vào thư mục chứa phần publish của project này để sử dụng quản lý file (kết hợp với modal của bootstrap là một phương án tốt).

`
tinymce.PluginManager.add('tinyfile', function (editor) {
    function openmanager() {
        var win, data, dom = editor.dom, tinyfile_path,
            imgElm = editor.selection.getNode();
        var width, height, imageListCtrl;
        win = editor.windowManager.open({
            title: 'Quản lý tập tin',
            file: (tinyfile_path || "/Scripts/tinymce/plugins/tinyfile") + '/dialog.aspx?editor=' + editor.id,
            filetype: 'all',
            classes: 'tinyfile',
            width: 900,
            height: 600,
            inline: 1
        }, {
            editor: editor
        })
    }
    editor.addButton('tinyfile', {
        icon: 'browse',
        tooltip: 'Chèn tập tin',
        onclick: openmanager,
        stateSelector: 'img:not([data-mce-object])'
    });
    editor.addMenuItem('tinyfile', {
        icon: 'browse',
        text: 'Chèn tập tin',
        onclick: openmanager,
        context: 'insert',
        prependToContext: true
    })
});
`

trên đây là ví dụ về cách nhúng (tạo plugin) vào tinymce
