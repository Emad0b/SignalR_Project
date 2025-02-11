var app = angular.module('App', ['ngRoute', 'ngCookies']); 

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
    $scope.userName = ''; // Should be set based on login session
    $scope.message = "";

    // Fetch messages from API
    $http.get("/Chat/GetMessages").then(function (response) {
        if (response.data.success) {
            $scope.messages = response.data.data;
        }
    });

    $scope.sendMessage = function () {
        if ($scope.message.trim() === "") return;

        var messageData = {
            senderId: $scope.userName, // Ensure this is the actual sender ID from session
            content: $scope.message
        };

        // Send message to API
        $http.post('/Chat/SendMessage', messageData)
            .then(function (response) {
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

    // Fix for receiving messages
    connection.on("ReceiveMessage", function (user, message) {
        console.log("Received message from:", user, "Message:", message);
        console.log("Type of user:", typeof user, "Value:", user);

        $scope.$apply(function () {
            // Ensure 'user' is always treated as a string
            let senderName = (typeof user === "object" && user.userName) ? user.userName : String(user);

            $scope.messages.push({ sender: { userName: senderName }, content: message });
        });
    });

});
