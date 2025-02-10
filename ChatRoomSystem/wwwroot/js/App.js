﻿var app = angular.module('App', ['ngRoute', 'ngCookies']); 

// Configure Routes
app.config(['$routeProvider', '$locationProvider', '$httpProvider', function ($routeProvider, $locationProvider, $httpProvider) {
    /*    $locationProvider.hashPrefix(''); // Removes the "!" from the hash*/

    $routeProvider
        .when('/Login', {
            templateUrl: '/Account/Login',
            controller: 'LoginController',
            resolve: {
                checkUser: ['$cookies', '$location', function ($cookies, $location) {
                    var user = $cookies.getObject('user');
                    if (user) {
                        let redirectPath = '';
                        switch (user.role) {
                            case 0:
                                redirectPath = '/Admin';
                                break;
                        }
                        if (redirectPath && $location.path() !== redirectPath) {
                            $location.path(redirectPath);
                        }
                    }
                }]
            }
        })
        .when('/', {
            templateUrl: '/Main/Index',
            controller: 'ChatController'
        })
        .when('/Admin', {
            templateUrl: '/Admin/Index',
            controller: 'AdminController'
        })
        .otherwise({
            redirectTo: '/'
        });
}]);

//// Toastr Configuration
//app.run(['toastr', function (toastr) { 
//    toastr.options = {
//        "closeButton": true,
//        "debug": false,
//        "newestOnTop": true,
//        "progressBar": true,
//        "positionClass": "toast-bottom-right",
//        "preventDuplicates": false,
//        "onclick": null,
//        "showDuration": "200",
//        "hideDuration": "1000",
//        "timeOut": "5000",
//        "extendedTimeOut": "1000",
//        "showEasing": "swing",
//        "hideEasing": "linear",
//        "showMethod": "fadeIn",
//        "hideMethod": "fadeOut"
//    };
//}]);

app.controller("ChatController", function ($scope, $http) {
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.start().catch(function (err) {
        console.error("Error connecting to SignalR:", err);
    });

    $scope.messages = [];
    $scope.userName = "User1";
    $scope.message = "";

    // Fetch messages from API
    $http.get("/api/chat/messages").then(function (response) {
        if (response.data.success) {
            $scope.messages = response.data.data;
        }
    });

    $scope.sendMessage = function () {
        if ($scope.message.trim() === "") return;

        var messageData = {
            content: $scope.message,
            senderId: 'fd0ae5a1-5540-445f-864a-2db9c3f47162' // Replace with actual sender ID from login session
        };

        // Send message to API
        $http.post("/api/chat/send", messageData).then(function (response) {
            if (response.data.success) {
                console.log("Message sent:", response.data.message);
            }
        });

        // SignalR real-time send
        connection.invoke("SendMessage", $scope.userName, $scope.message)
            .catch(function (err) {
                console.error(err.toString());
            });

        $scope.message = "";
    };

    connection.on("ReceiveMessage", function (user, message) {
        $scope.$apply(function () {
            $scope.messages.push({ sender: { userName: user }, content: message });
        });
    });
});
