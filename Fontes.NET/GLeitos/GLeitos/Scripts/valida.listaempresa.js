$(document).ready(function () {
      

    $("#btSubmit").click(function () {
        ///$(".form-control").hide();
        $("#hdn_id_empresa").val($("#lstEmpresa").val());
        $("#hdn_id_perfil").val($("#lstPerfil").val());
        $(".loader").show();
        window.document.forms[0].submit();
    });

});

