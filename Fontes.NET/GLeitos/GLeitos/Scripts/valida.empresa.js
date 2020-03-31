//$(document).ready(function () {

//    $("#selEstado").click(function () {
//        $("#hdnestado_Empresa").val($("#selEstado").val());
//        alert($("#hdnestado_Empresa").val());
//    });

//});


function salvarempresa(url) {
    $(".input-file").hide();
    $(".loader").show();
    $("#hdnestado_Empresa").val($("#selEstado").val());
    window.document.forms[0].submit();
};


