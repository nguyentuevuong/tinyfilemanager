Dropzone.autoDiscover = false;
angular.module('tinyfile', ['ui.tree', 'ui.bootstrap', 'ngStorage', 'angular-clipboard'])
    .constant('rootUrl', 'Default.aspx')
    .config(['$httpProvider', '$localStorageProvider', function ($httpProvider, $localStorageProvider) {
        $httpProvider.defaults.headers.post = {};
        $httpProvider.defaults.headers.post["Content-Type"] = "application/json; charset=utf-8";
        $localStorageProvider.setKeyPrefix('tinyfile.');
    }])
    .directive('dropzone', function () {
        return function (scope, element, attrs) {
            var config, dropzone;
            config = scope[attrs.dropzone];
            dropzone = new Dropzone(element[0], config.options);
            angular.forEach(config.eventHandlers, function (handler, event) {
                dropzone.on(event, handler);
            });
        };
    })
    .filter("selectedNode", function () {
        return function (id, data) {
            if (!id || !data) {
                return undefined;
            }

            var _filter = function (id, d) {
                var _r = undefined;
                angular.forEach(d, function (v, k) {
                    if (id == v.path) {
                        _r = v;
                    }
                    if (!_r && v.children && angular.isArray(v.children)) {
                        _r = _filter(id, v.children);
                    }
                });
                return _r;
            };

            return _filter(id, data);
        }
    })
    .controller('fileManagerController', ['$scope', '$http', '$filter', '$window', '$uibModal', '$localStorage', 'rootUrl',
        function ($scope, $http, $filter, $window, $uibModal, $localStorage, rootUrl) {
            $scope.config = {};

            $http.post(rootUrl + '/GetConfigs', {})
                    .success(function (data) {
                        if (data.d) {
                            $scope.config = data.d;
                        }
                    });

            $scope.$keyword = '';
            $scope.treeNode = {};
            $scope.treeData = [];
            $scope.$messages = [];

            if ($localStorage.settings) {
                $scope.$settings = $localStorage.settings;
            } else {
                $scope.$settings = $localStorage.settings = { view: 0, sort: 0, path: '/', tree: true };
            }

            $scope.$newFolder = function () {
                var $folderName = $scope.$folderName;
                if ($folderName) {
                    $http.post(rootUrl + '/NewFolder', { folderName: $folderName, folderPath: $scope.$settings.path })
                    .success(function (response) {
                        $scope.$messages.push({ type: 'success', msg: 'Tạo thư mục [' + $folderName + '] thành công!' });
                        $scope.$folderName = '';
                        $scope.$getTree();
                    });
                }
                else {
                    $scope.$messages.push({ type: 'warning', msg: 'Bạn chưa nhập tên cho thư mục!' });
                }
            };

            $scope.$delete = function (file) {
                bootbox.confirm("Bạn có chắc chắn muốn xóa " + (file.Type != 1 ? "tập tin [" : "thư mục [") + file.Title + "] này không?", function (result) {
                    if (result) {
                        $http.post(rootUrl + '/Delete', { fileName: file.Title, folderPath: $scope.$settings.path })
                        .success(function (data) {
                            if (data.d) {
                                $scope.$messages.push({ type: 'success', msg: 'Xóa ' + (file.Type != 1 ? 'tập tin [' : 'thư mục [') + file.Title + '] thành công!' });
                                $scope.$getTree();
                            } else {
                                $scope.$messages.push({ type: 'danger', msg: 'Xóa ' + (file.Type != 1 ? 'tập tin [' : 'thư mục [') + file.Title + '] không thành công!' });
                            }
                        });
                    }
                });
            };

            $scope.$treeSelect = function (branch) {
                $scope.$settings.path = branch.path || '/';
                $scope.$getFiles();
            };

            $scope.$go = function (path) {
                $scope.$settings.path = path || $scope.model.back || "/";
                $scope.$expand(0);
            };

            $scope.$expand = function (timeOut) {
                setTimeout(function () {
                    $scope.treeNode.select_branch($filter('selectedNode')($scope.$settings.path, $scope.treeData));
                    $scope.treeNode.expand_all();
                }, (timeOut || 0));
            };

            $scope.$getTree = function () {
                $http.post(rootUrl + '/GetTree', {})
                .success(function (data) {
                    $scope.treeData = data.d;
                    $scope.$expand(250);
                });
            };
            $scope.$getTree();

            $scope.$getFiles = function () {
                $http.post(rootUrl + '/GetFiles', { filter: ($scope.$keyword || ''), folderPath: ($scope.$settings.path || '/') })
                .success(function (data) {
                    $scope.model = data.d;
                    $scope.$folder = false;
                    $scope.$uploader = false;
                    $scope.$folderName = '';
                });
            };

            $scope.dzOptions = {
                'options': {
                    'method': 'POST',
                    'paramName': 'file',
                    'addRemoveLinks': true,
                    'maxFilesize': $scope.config.size,
                    'dictRemoveFile': 'Xóa tập tin',
                    'dictResponseError': 'Không kết nối được tới máy chủ!',
                    'dictFileTooBig': 'Kích thước tập tin vượt quá mức cho phép!',
                    'accept': function (file, done) {
                        var extension = file.name.split('.').pop();
                        if ($.inArray(extension.toLowerCase(), $scope.config.extensions) > -1) {
                            done();
                        } else {
                            done("Tập tin không được chấp nhận");
                        }
                    },
                    'removedfile': function (file) {
                        var name = file.name;
                        bootbox.confirm("Bạn có chắc chắn muốn xóa tập tin này không?", function (result) {
                            if (result) {
                                $http.post(rootUrl + '/Delete', { fileName: name, folderPath: $scope.$settings.path })
                                .success(function (data) {
                                    if (data) {
                                        $scope.$messages.push({ type: 'success', msg: 'Xóa tập tin [' + file.name + '] thành công!' });
                                    }
                                });
                                var _ref;
                                return (_ref = file.previewElement) != null ? _ref.parentNode.removeChild(file.previewElement) : void 0;
                            }
                        });
                    }
                }
            };

            $scope.$preview = function (file) {
                $uibModal.open({
                    templateUrl: 'preview.html',
                    controller: function ($scope, $uibModalInstance, $file) {
                        $scope.file = $file;
                        $scope.$action = {
                            cancel: function () {
                                $uibModalInstance.dismiss('cancel');
                            }
                        }
                    },
                    resolve: {
                        $file: file
                    }
                });
            };

            $scope.$checked = function (file) {
                file.Checked = !(file.Checked || false);
            };

            $scope.$fillTo = function (file) {
                if ($window.top.tinyfile && $window.top.tinyfile.fillTo) {

                    $window.top.tinyfile.fillTo.val(file.Type == 1 ? file.Path : file.Url);
                    angular.element($window.parent.document).find('.ff-close-button:first').trigger('click');
                    return;
                }

                var $insertModal = $uibModal.open({
                    templateUrl: 'insert.html',
                    controller: function ($scope, $uibModalInstance, $file) {
                        $scope.file = $file;
                        $scope.$action = {
                            insert: function () {
                                $uibModalInstance.close($scope.model || { x: 0, y: 0 });
                            },
                            cancel: function () {
                                $uibModalInstance.dismiss('cancel');
                            }
                        }
                    },
                    resolve: {
                        $file: file
                    }
                });

                $insertModal.result.then(function (model) {
                    if ($window.top.tinyMCE && $window.top.tinyMCE.activeEditor) {
                        var image = $('<div>').append($('<a>', { 'class': 'ff-file thumbnail' }).append($('<img>', { 'src': file.Url, 'style': ((model.x && 'max-width: ' + model.x + 'px;') + (model.y && 'max-height: ' + model.y + 'px;')) }))).html();
                        $window.top.tinyMCE.activeEditor.execCommand('mceInsertContent', false, image);
                        angular.element($window.parent.document).find('.mce-tinyfile:first .mce-close:first').trigger('click');
                        return;
                    }
                });
            };

            $scope.login = function () {
                var loginData = $scope.loginData;
                $http.post(rootUrl + '/LogIn', loginData)
                    .success(function (data) {
                        if (data.d) {
                            $scope.config.auth = true;
                            $scope.$getTree();
                        } else {
                            $scope.config.auth = false;
                            $scope.loginData.message = 'Tên đăng nhập hoặc mật khẩu không đúng!';
                        }
                    });
            };

            $scope.$disabled = {
                file: $window.top.tinyfile ? ($window.top.tinyfile.fillTo && $window.top.tinyfile.fillToFolder) : ($window.top.tinyMCE && $window.top.tinyMCE.activeEditor),
                folder: !$window.top.tinyfile || !$window.top.tinyfile.fillTo || !$window.top.tinyfile.fillToFolder
            }
        }]);