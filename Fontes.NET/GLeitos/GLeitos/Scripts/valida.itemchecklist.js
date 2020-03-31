
//$(document).ready(function () {

//    $("#selEstado").click(function () {
//        $("#hdnestado_Empresa").val($("#selEstado").val());
//        alert($("#hdnestado_Empresa").val());
//    });

//});


function salvaritemchecklist(url) {
    $(".input-file").hide();
    $(".loader").show();
    window.document.forms[0].submit();
};

function editar_itemchecklist(id, nome) {
    $("#hdnid_ItemCheckList").val(id);
    $("#nome_ItemCheckList").val(nome);
};