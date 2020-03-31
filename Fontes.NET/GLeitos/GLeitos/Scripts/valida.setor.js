//$(document).ready(function () {

//    $("#selEstado").click(function () {
//        $("#hdnestado_Empresa").val($("#selEstado").val());
//        alert($("#hdnestado_Empresa").val());
//    });

//});


function salvarsetor(url) {
    $(".input-file").hide();
    $(".loader").show();
    $("#hdnid_Empresa").val($("#selEmpresa").val());
    window.document.forms[0].submit();
};


