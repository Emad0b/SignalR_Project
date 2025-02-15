var app = angular.module('App', ['ngRoute', 'ngCookies']); 

// Configure Routes
app.config(['$routeProvider', '$locationProvider', '$httpProvider', function ($routeProvider, $locationProvider, $httpProvider) {
        $locationProvider.hashPrefix(''); // Removes the "!" from the hash

    $routeProvider
        .when('/Login', {
            templateUrl: '/Auth/Login',
            controller: 'LoginController',
            resolve: {
                checkUser: ['$cookies', '$location', function ($cookies, $location) {
                    var user = $cookies.getObject('user');
                    if (user) {
                        let redirectPath = '/';
                        if (redirectPath && $location.path() !== redirectPath) {
                            $location.path(redirectPath);
                        }
                    }
                }]
            }
        })
        .when('/Signup', {
            templateUrl: '/Auth/Signup',
            controller: 'SignUpController'
        })
        .when('/', {
            templateUrl: '/Main/Index',
            controller: 'ChatController'
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
app.controller('LoginController', function ($scope, $http, $cookies, $location, $httpParamSerializerJQLike ) {


    $scope.userName='';
    $scope.password='';
    // Login Function
    $scope.login = function () {
        if (!$scope.userName || !$scope.password) {
            alert("Please enter both username and password.");
            return;
        }

        $http({
            method: 'POST',
            url: '/Account/Login',
            data: $httpParamSerializerJQLike({
                username: $scope.userName,
                password: $scope.password
            }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {

            if (response.data.success) {
                var userInfo = {
                    Id: response.data.id,         
                    Username: response.data.username, 
                    Name: response.data.name     
                };



                // Store user in cookies (expires in 7 days)
                var expireDate = new Date();
                expireDate.setDate(expireDate.getDate() + 7);

                $cookies.putObject('user', userInfo, {
                    expires: expireDate,
                    path: '/'
                });

                console.log("User stored in cookies:", userInfo);
                $location.path('/');
            } else {
                $scope.message = response.data.message || "Invalid username or password.";
                alert($scope.message);
            }
        }, function (error) {
            console.error("Login error:", error);
            $scope.message = "Error during login.";
            alert($scope.message);
        });
    };




});

app.controller('SignUpController', function ($scope, $http, $httpParamSerializerJQLike, $location) {
    $scope.user = {};

    $scope.submit = function () {
        if (validateUser($scope.user)) {
            $http({
                method: 'POST',
                url: '/Account/SignUp',
                data: $httpParamSerializerJQLike({
                    obj: $scope.user,

                }),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).then(function (response) {
                const result = response.data;
                if (result.success) {
                    alert(result.message);
                    $scope.user = {};
                    $location.path('/Login');

                } else {
                    alert(result.message);
                }
            }).catch(function (error) {
                console.error('Error creating user:', error);
            });
        }
    };

    // Validate user input
    function validateUser(user) {
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        var UserNameRegex = /^[^\s]{6,}$/;

        if (!emailRegex.test(user.Email)) {
            alert('Invalid email format.');
            return false;
        }

        if (!UserNameRegex.test(user.UserName)) {
            alert('Invalid username format. \n\nUsername must: \nBe at least 6 characters long \nContain no spaces.');
            return false;
        }

        if (user.Name == null || user.Name.trim() === "") {
            alert('Please enter your name.');
            return false;
        }
        return true;
    }

});


app.controller("ChatController", function ($scope, $http, $httpParamSerializerJQLike, $location, $cookies) {

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.start().catch(function (err) {
        console.error("Error connecting:", err.toString());
    });

    const userCookie = $cookies.getObject('user');
    if (userCookie == null) {
        $location.path('/Login');

    }
    $scope.userName = userCookie?.Name;
    $scope.LoggedId = userCookie?.Id;

    $scope.messages = [];
    $scope.message = "";

    $http.get("/Chat/GetMessages").then(function (response) {
        if (response.data.success) {
            $scope.messages = response.data.data;
        }
    });





    $scope.sendMessage = function () {
        if ($scope.message.trim() === "") return;

        $http({
            method: 'POST',
            url: '/Chat/SendMessage',
            data: $httpParamSerializerJQLike({
                senderId: $scope.LoggedId,
                content: $scope.message
            }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' } 
        }).then(function (response) {
                if (response.data.success) {
                    console.log("Message sent:", response.data.message);
                }
            });

        // SignalR send
        connection.invoke("SendMessage", $scope.LoggedId, $scope.message)
            .catch(function (err) {
                console.error(err.toString());
            });

        $scope.message = "";
    };

    // SignalR receive
    connection.on("ReceiveMessage", function (senderId, userName, message) {
        $scope.$apply(function () {
            $scope.messages.push({
                senderId: senderId,   
                senderName: userName,
                content: message
            });
        });
    });


    $scope.EnterButton = function (event) {
        if (event.which === 13 || event.keyCode === 13) {
            event.preventDefault();
            $scope.sendMessage();
        }
    };



    $scope.Date = function (timestamp) {
        return moment(timestamp).format('h:mm A, MMM D'); 
    };

});
