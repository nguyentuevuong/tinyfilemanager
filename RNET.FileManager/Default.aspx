<%@ Page Title="RNET File Manager" Language="C#" AutoEventWireup="true" Inherits="RNET.FileManager.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Quản lý tập tin</title>
    <meta name="robots" content="noindex,nofollow" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="content/animate.min.css" rel="stylesheet" />
    <link href="Content/cerulean.min.css" rel="stylesheet" />
    <link href="Scripts/ui-tree/ui-tree.css" rel="stylesheet" />
    <link href="Content/bootstrap.custom.css" rel="stylesheet" />
    <link href="Scripts/dropzone/dropzone.min.css" rel="stylesheet" />
</head>
<body ng-app="tinyfile" class="ff-manager">
    <div class="container-fluid" ng-controller="fileManagerController">
        <div ng-show="config.auth" class="animated fadeInDown">
            <div class="row ff-filters">
                <div class="col-md-12">
                    <div class="btn-toolbar">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-primary" ng-show="config.upload" ng-click="$folder = false; $uploader = true;">
                                <i class="glyphicon glyphicon-upload glyphicon-0x hidden-lg hidden-md hidden-sm"></i>
                                <i class="glyphicon glyphicon-upload hidden-xs"></i>
                                <span class="hidden-xs">Tải lên</span>
                            </button>
                            <button class="btn btn-success" ng-show="config.upload" ng-click="$folder = true; $uploader = false;">
                                <i class="glyphicon glyphicon-folder-open glyphicon-0x hidden-lg hidden-md hidden-sm"></i>
                                <i class="glyphicon glyphicon-folder-open hidden-xs"></i>
                                <span class="hidden-xs">Tạo thư mục</span>
                            </button>
                        </div>
                        <div class="btn-group btn-group-sm pull-right">
                            <label class="btn btn-default" ng-class="{'btn-success' : $settings.sort == 0}" ng-click="$settings.sort = 0;">
                                <i class="glyphicon glyphicon-blackboard glyphicon-0x hidden-lg hidden-md"></i>
                                <i class="glyphicon glyphicon-blackboard hidden-sm hidden-xs"></i>
                                <span class="hidden-sm hidden-xs">Tất cả</span>
                            </label>
                            <label class="btn btn-default" ng-class="{'btn-success' : $settings.sort == 1}" ng-click="$settings.sort = 1">
                                <i class="glyphicon glyphicon-folder-open glyphicon-0x hidden-lg hidden-md"></i>
                                <i class="glyphicon glyphicon-folder-open hidden-sm hidden-xs"></i>
                                <span class="hidden-sm hidden-xs">Thư mục</span>
                            </label>
                            <label class="btn btn-default" ng-class="{'btn-success' : $settings.sort == 2}" ng-click="$settings.sort = 2">
                                <i class="glyphicon glyphicon-picture glyphicon-0x hidden-lg hidden-md"></i>
                                <i class="glyphicon glyphicon-picture hidden-sm hidden-xs"></i>
                                <span class="hidden-sm hidden-xs">Hình ảnh</span>
                            </label>
                            <label class="btn btn-default" ng-class="{'btn-success' : $settings.sort == 3}" ng-click="$settings.sort = 3">
                                <i class="glyphicon glyphicon-file glyphicon-0x hidden-lg hidden-md"></i>
                                <i class="glyphicon glyphicon-file hidden-sm hidden-xs"></i>
                                <span class="hidden-sm hidden-xs">Tập tin</span>
                            </label>
                            <label class="btn btn-default" ng-class="{'btn-success' : $settings.sort == 4}" ng-click="$settings.sort = 4">
                                <i class="glyphicon glyphicon-film glyphicon-0x hidden-lg hidden-md"></i>
                                <i class="glyphicon glyphicon-film hidden-sm hidden-xs"></i>
                                <span class="hidden-sm hidden-xs">Đoạn phim</span>
                            </label>
                            <label class="btn btn-default" ng-class="{'btn-success' : $settings.sort == 5}" ng-click="$settings.sort = 5">
                                <i class="glyphicon glyphicon-music glyphicon-0x hidden-lg hidden-md"></i>
                                <i class="glyphicon glyphicon-music hidden-sm hidden-xs"></i>
                                <span class="hidden-sm hidden-xs">Âm nhạc</span>
                            </label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row ff-breadcrumb">
                <div class="col-md-12">
                    <ol class="breadcrumb">
                        <li><a ng-click="$go('/')"><i class='glyphicon glyphicon-home'></i></a></li>
                        <li ng-repeat="bre in model.breadcrumbs" ng-class="{'active' : $index == model.breadcrumbs.length - 1}"><a ng-if="$index < model.breadcrumbs.length - 1" ng-click="$go(bre.path)">{{bre.name}}</a><span ng-if="$index == model.breadcrumbs.length - 1">{{bre.name}}</span></li>
                        <li class="pull-right" ng-show="$messages.length"><a title="Xóa thông báo" ng-click="$messages = []"><i class="glyphicon glyphicon-erase glyphicon-0x"></i></a></li>
                    </ol>
                </div>
            </div>
            <div class="row ff-messagebox animated fadeInDown">
                <div class="col-md-12">
                    <uib-alert ng-repeat="alert in $messages" type="{{alert.type}}" close="$messages.splice($index, 1)">{{alert.msg}}</uib-alert>
                </div>
            </div>
            <div class="row ff-uploader animated fadeInLeft" ng-show="$uploader">
                <div class="col-md-12">
                    <form action="<%= Request.Url.AbsolutePath %>" method="post" enctype="multipart/form-data" dropzone="dzOptions" class="dropzone">
                        <input type="hidden" name="folder" value="{{$settings.path}}" />
                        <div class="fallback">
                            <input type="hidden" name="fback" value="true" />
                            <input name="file" type="file" multiple="multiple" />
                        </div>
                    </form>
                    <div class="row">
                        <div class="col-md-12 text-center">
                            <button class="btn btn-primary btn-lg close-uploader" ng-click="$getFiles()">
                                <i class="glyphicon glyphicon-chevron-left"></i>
                                <span>Quay lại danh mục</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row ff-new-folder animated fadeInDown" ng-show="$folder">
                <div class="col-md-4 col-md-offset-4 col-sm-6 col-sm-offset-3">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h4 class="panel-title"><i class="glyphicon glyphicon-bell"></i>Thông báo&hellip;</h4>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <label class="control-label">Nhập tên thư mục:</label>
                                <div class="input-group input-group-sm">
                                    <input type="text" class="form-control" ng-model="$folderName" ng-keypress="$event.keyCode == 13 ? $newFolder() : void(0); " />
                                    <div class="input-group-btn">
                                        <button type="button" class="btn btn-default" ng-click="$newFolder()"><i class="glyphicon glyphicon-ok glyphicon-0x"></i></button>
                                        <button type="button" class="btn btn-default" ng-click="$getFiles()"><i class="glyphicon glyphicon-remove glyphicon-0x"></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row ff-items" ng-show="!$uploader && !$folder">
                <div class="col-md-12">
                    <div class="row">
                        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                            <div class="btn-group btn-group-sm">
                                <button class="btn btn-default" type="button" ng-disabled="model.path == '/'" ng-click="$go(model.back)"><i class="glyphicon glyphicon-chevron-left"></i><span class="hidden-xs">Quay lại</span></button>
                                <button class="btn btn-default" type="button" ng-disabled="$disabled.folder && !(model.items | filter : true).length" ng-click="$fillTo({ Url: (config.root + $settings.path) })"><i class="glyphicon glyphicon-check"></i>({{(model.items | filter : true).length}})</button>
                            </div>
                            <div class="btn-group btn-group-sm">
                                <label class="btn btn-default" ng-class="{'btn-success' : $settings.view == 1}" ng-click="$settings.view = 1"><i class="glyphicon glyphicon-list glyphicon-0x hidden-lg hidden-md"></i><i class="glyphicon glyphicon-list hidden-sm hidden-xs"></i><span class="hidden-sm hidden-xs">Danh sách</span></label>
                                <label class="btn btn-default" ng-class="{'btn-success' : $settings.view == 0}" ng-click="$settings.view = 0"><i class="glyphicon glyphicon-th glyphicon-0x hidden-lg hidden-md"></i><i class="glyphicon glyphicon-th hidden-sm hidden-xs"></i><span class="hidden-sm hidden-xs">Hình thu nhỏ</span></label>
                                <label class="btn btn-default" ng-class="{'btn-success' : $settings.tree}" ng-click="$settings.tree = !$settings.tree"><i class="glyphicon glyphicon-object-align-left glyphicon-0x"></i></label>
                            </div>
                        </div>
                        <div class="col-md-4 col-md-offset-2 col-sm-6 col-xs-6">
                            <div class="input-group input-group-sm pull-right">
                                <input type="text" class="form-control" ng-model="$keyword" placeholder="Tìm kiếm..." ng-change="$getFiles()" />
                                <span class="input-group-btn">
                                    <button class="btn btn-default" type="button" ng-click="$getFiles()"><i class="glyphicon glyphicon-search glyphicon-0x"></i></button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div ng-class="{ 'col-md-3': $settings.tree, 'hidden' : !$settings.tree }">
                            <div class="well animated fadeInDown tree-view" ng-show="treeData.length">
                                <ui-tree
                                    tree-data="treeData"
                                    tree-control="treeNode"
                                    icon-leaf="glyphicon glyphicon-briefcase text-danger"
                                    icon-expand="glyphicon glyphicon-folder-close text-warning"
                                    icon-collapse="glyphicon glyphicon-folder-open text-warning"
                                    expand-level="10"
                                    on-select="$treeSelect(branch)">
                            </ui-tree>
                            </div>
                        </div>
                        <div ng-class="{ 'col-md-9' : $settings.tree, 'col-md-12' : !$settings.tree}">
                            <div class="row" ng-show="$settings.view">
                                <div class="col-md-12">
                                    <table class="table table-hover">
                                        <thead>
                                            <tr>
                                                <th class="index">#</th>
                                                <th>Name</th>
                                                <th class="hidden-sm hidden-xs">Url</th>
                                                <th class="hidden-sm hidden-xs hidden">Date</th>
                                                <th class="action-toolbox">Action</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr class="animated fadeInUp" ng-repeat="file in model.items" ng-show="(!$settings.sort || ($settings.sort == file.Type))">
                                                <td>{{$index+ 1}}</td>
                                                <td><a ng-click="file.Type == 1 ? $go(file.Url) : !$disabled.folder ? $checked(file) :  file.Type == 2 ? $preview(file) : void(0)">{{(file.Title | limitTo : 25) + (file.Title.length > 25 ? '&hellip;' : '')}}</a></td>
                                                <td class="hidden-sm hidden-xs">{{(file.Url | limitTo : 55) + (file.Url.length > 55 ? '&hellip;' : '')}}</td>
                                                <td class="hidden-sm hidden-xs hidden">{{file.Date}}</td>
                                                <td class="text-center">
                                                    <div class="btn-group btn-group-sm toolbox">
                                                        <button type="button" class="btn btn-default" title="Xóa" ng-disabled="!config.delete" ng-click="$delete(file)">
                                                            <i class="glyphicon glyphicon-trash"></i>
                                                        </button>
                                                        <button type="button" class="btn btn-default hidden" title="Đổi tên" ng-click="$rename(file)">
                                                            <i class="glyphicon glyphicon-console"></i>
                                                        </button>
                                                        <a href="{{file.Type != 1 ? file.Url : 'javascript:;'}}" download="{{file.Title}}" class="btn btn-default" ng-disabled="file.Type == 1" title="Tải xuống">
                                                            <i class="glyphicon glyphicon-download"></i>
                                                        </a>
                                                        <button type="button" class="btn btn-default" title="Xem trước ảnh" ng-click="$preview(file)" ng-disabled="file.Type == 1">
                                                            <i class="glyphicon glyphicon-eye-open"></i>
                                                        </button>
                                                        <button type="button" class="btn btn-default" title="Sao chép đường link" clipboard text="file.Url" ng-disabled="file.Type == 1">
                                                            <i class="glyphicon glyphicon-copy"></i>
                                                        </button>
                                                        <button type="button" class="btn btn-default" title="Chèn vào trình soạn thảo" ng-click="$fillTo(file)" ng-disabled="file.Type == 1 ? $disabled.folder : (file.Type != 1 ? $disabled.file : false)">
                                                            <i class="glyphicon glyphicon-check"></i>
                                                        </button>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="row" ng-show="!$settings.view">
                                <div class="col-sm-3 col-xs-4 animated fadeInRight" ng-class="{'col-md-3' : $settings.tree, 'col-md-2' : !$settings.tree }" ng-repeat="file in model.items" ng-show="(!$settings.sort || ($settings.sort == file.Type))">
                                    <div class="thumbnail" ng-class="{'checked': file.Checked}" title="{{file.Title}}">
                                        <i class="icon-checked"></i>
                                        <img ng-src="{{file.Image}}" alt="{{file.Title}}" ng-click="file.Type == 1 ? $go(file.Url) : !$disabled.folder ? $checked(file) : file.Type == 2 ? $preview(file) : void(0)" />
                                        <div class="caption text-center"><span>{{(file.Title | limitTo : 20) + (file.Title.length > 20 ? '&hellip;' : '')}}</span></div>
                                        <div class="row">
                                            <div class="col-md-12 text-center">
                                                <div class="btn-group btn-group-sm toolbox">
                                                    <button type="button" class="btn btn-default" title="Xóa" ng-disabled="!config.delete" ng-click="$delete(file)">
                                                        <i class="glyphicon glyphicon-trash"></i>
                                                    </button>
                                                    <button type="button" class="btn btn-default hidden" title="Đổi tên" ng-click="$rename(file)">
                                                        <i class="glyphicon glyphicon-console"></i>
                                                    </button>
                                                    <a href="{{file.Type != 1 ? file.Url : 'javascript:;'}}" class="btn btn-default" ng-disabled="file.Type == 1" download="{{file.Title}}" title="Tải xuống">
                                                        <i class="glyphicon glyphicon-download"></i>
                                                    </a>
                                                    <button type="button" class="btn btn-default" title="Xem trước ảnh" ng-click="$preview(file)" ng-disabled="file.Type == 1">
                                                        <i class="glyphicon glyphicon-eye-open"></i>
                                                    </button>
                                                    <button type="button" class="btn btn-default" title="Sao chép đường link" ng-disabled="!file.Type" clipboard text="file.Url">
                                                        <i class="glyphicon glyphicon-copy"></i>
                                                    </button>
                                                    <button type="button" class="btn btn-default" title="Chèn vào trình soạn thảo" ng-click="$fillTo(file)" ng-disabled="file.Type == 1 ? $disabled.folder : (file.Type != 1 ? $disabled.file : false)">
                                                        <i class="glyphicon glyphicon-check"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row animated swing" ng-show="!config.auth">
            <div class="col-md-4 col-md-offset-4 col-sm-6 col-sm-offset-3">
                <div class="panel panel-default panel-login">
                    <div class="panel-heading clearfix">
                        <h4 class="panel-title pull-left">
                            <i class="glyphicon glyphicon-user"></i>
                            <span>Đăng nhập</span>
                        </h4>
                    </div>
                    <div class="panel-body">
                        <form method="post">
                            <div class="form-group">
                                <label class="control-label" for="UserName">Tên đăng nhập:</label>
                                <div class="input-group input-group-sm">
                                    <span class="input-group-addon">
                                        <i class="glyphicon glyphicon-user"></i>
                                    </span>
                                    <input class="form-control" data-val="true" id="UserName" required type="text" ng-model="loginData.userName" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label" for="Password">Mật khẩu bảo vệ:</label>
                                <div class="input-group input-group-sm">
                                    <span class="input-group-addon">
                                        <i class="glyphicon glyphicon-tags"></i>
                                    </span>
                                    <input class="form-control" id="Password" required type="password" ng-model="loginData.passWord" />
                                </div>
                                <span class="help-block with-errors" ng-if="loginData.message">{{loginData.message}}</span>
                            </div>
                            <div class="text-right">
                                <hr />
                                <button type="reset" class="btn btn-default btn-sm">
                                    <i class="glyphicon glyphicon-refresh"></i>
                                    <span>Làm lại</span>
                                </button>
                                <button type="submit" class="btn btn-success btn-sm" ng-click="login()">
                                    <i class="glyphicon glyphicon-ok"></i>
                                    <span>Đăng nhập</span>
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/ng-template" id="insert.html">        
        <div class="modal-header">
            <h4 class="modal-title">Chỉnh kích thước</h4>
        </div>
        <div class="modal-body" style="padding: 0">
            <div class="thumbnail" style="margin: 0">
                <img ng-src="{{file.Url}}" style="min-height: 300px;" alt="{{file.Title}}" />
            </div>
        </div>
        <div class="modal-footer">
            <form class="form-inline text-center">
                <div class="form-group form-group-sm">
                    <label class="control-label">Rộng:</label>
                    <input type="text" class="form-control" ng-model="model.x" ng-pattern="/^\d+$/" maxlength="4" placeholder="px" />
                </div>
                <div class="form-group form-group-sm">
                    <label class="control-label">Cao:</label>
                    <input type="text" class="form-control" ng-model="model.y" ng-pattern="/^\d+$/" maxlength="4" placeholder="px" />
                </div>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-primary btn-sm" type="button" ng-click="$action.insert()"><i class="glyphicon glyphicon-check glyphicon-0x"></i></button>
                    <button class="btn btn-warning btn-sm" type="button" ng-click="$action.cancel()"><i class="glyphicon glyphicon-remove glyphicon-0x"></i></button>
                </div>
            </form>
        </div>
    </script>
    <script type="text/ng-template" id="preview.html">        
        <div class="modal-body" style="padding: 5px 0px 0px 0px">
            <div class="text-right hidden" style="padding-right: 5px">
                <button class="btn btn-warning btn-xs" type="button" ng-click="$action.cancel()"><i class="glyphicon glyphicon-remove glyphicon-0x"></i></button>
            </div>
            <hr class="hidden" />
            <div class="thumbnail" style="margin: 0">
                <img ng-src="{{file.Url}}" style="min-height: 300px;" alt="{{file.Title}}" />
            </div>
        </div>
    </script>

    <script type="text/javascript" src="Scripts/jquery-2.1.4.min.js"></script>
    <script type="text/javascript" src="Scripts/angular.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script src="Scripts/angular-ui/ui-bootstrap.min.js"></script>
    <script src="Scripts/angular-ui/ui-bootstrap-tpls.min.js"></script>
    <script src="Scripts/ui-tree/ui-tree.js"></script>
    <script src="Scripts/ngStorage.min.js"></script>
    <script src="Scripts/angular-clipboard.js"></script>
    <script type="text/javascript" src="Scripts/bootbox.js"></script>
    <script type="text/javascript" src="Scripts/dropzone/dropzone.min.js"></script>
    <script type="text/javascript" src="Scripts/filemanager.js"></script>
</body>
</html>
