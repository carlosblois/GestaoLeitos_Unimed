$(document).ready(function () {

    //$("#FormLogin").validate({
    //    rules: {
    //        login: "required",
    //        senha: "required"
    //    },
    //    messages: {
    //        login: "Informe o nome.",
    //        senha: "Informe uma e-mail v&aacute;lido."
    //    }
    //    ,
    //    submitHandler: function (form) {
    //        form.submit();
    //     }
    //});
    $("#login").keypress(function (event) {
        if (event.keyCode === 13) {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            $("#senha").focus();
        }
    });

    $("#senha").keypress(function (event) {
        if (event.keyCode === 13) {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            $("#hdn_id_empresa").val($("#lstEmpresa").val());
            $("#hdn_id_perfil").val($("#lstPerfil").val());
            $(".loader").show();
            window.document.forms[0].submit();
        }
    });

    $("#btSubmit").click(function () {
        ///$(".form-control").hide();
        $("#hdn_id_empresa").val($("#lstEmpresa").val());
        $("#hdn_id_perfil").val($("#lstPerfil").val());
        $(".loader").show();
        window.document.forms[0].submit();
    });

});


