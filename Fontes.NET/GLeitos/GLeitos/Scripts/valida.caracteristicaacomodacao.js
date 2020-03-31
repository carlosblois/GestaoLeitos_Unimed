//$(document).ready(function () {

//    $("#selEstado").click(function () {
//        $("#hdnestado_Empresa").val($("#selEstado").val());
//        alert($("#hdnestado_Empresa").val());
//    });

//});


function salvarcaracteristica(url) {
    $(".input-file").hide();
    $(".loader").show();
    window.document.forms[0].submit();
};

function editar_caracteristica(id, nome) {
    $("#hdnid_CaracteristicaAcomodacao").val(id);
    $("#nome_CaracteristicaAcomodacao").val(nome);
};