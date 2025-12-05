function loginComToken() {
    $("#loginForm").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: '/api/usuario/login',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                email: $('#Email').val(),
                senha: $('#Senha').val()
            }),
            success: function (response) {
                localStorage.setItem('token', response.token);
                $("#msg").html('<span class="text-success">Login realizado! Redirecionando...</span>');
                // Requisição protegida usando o token JWT
                $.ajax({
                    url: '/Usuario/TesteToken',
                    type: 'GET',
                    beforeSend: function(xhr) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + response.token);
                    },
                    success: function(data) {
                        $("#msg").append('<br><span class="text-info">' + data + '</span>');
                    },
                    error: function() {
                        $("#msg").append('<br><span class="text-danger">Token inválido ou acesso negado!</span>');
                    }
                });
                setTimeout(function () {
                    window.location.href = '/Usuario/TesteToken';
                }, 1000);
            },
            error: function () {
                $("#msg").html('<span class="text-danger">Usuário ou senha inválidos!</span>');
            }
        });
    });
}
