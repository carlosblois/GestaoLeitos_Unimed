//$(document).ready(function () {

//    $("#selEstado").click(function () {
//        $("#hdnestado_Empresa").val($("#selEstado").val());
//        alert($("#hdnestado_Empresa").val());
//    });

//});


function salvaracomodacao(url) {
    $(".input-file").hide();
    $(".loader").show();
    $("#hdnid_Empresa").val($("#selEmpresa").val());
    $("#hdnid_Setor").val($("#selSetor").val());
    $("#hdnid_TipoAcomodacao").val($("#selTipoAcomodacao").val());
    window.document.forms[0].submit();
};


