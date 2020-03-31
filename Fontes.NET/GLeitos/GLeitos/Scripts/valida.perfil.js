
//$(document).ready(function () {

//    $("#selEstado").click(function () {
//        $("#hdnestado_Empresa").val($("#selEstado").val());
//        alert($("#hdnestado_Empresa").val());
//    });

//});


function salvarperfil(url) {
    $(".input-file").hide();
    $(".loader").show();
    $("#hdncodTipoPerfil").val($("#selTipoPerfil").val());
    window.document.forms[0].submit();
};

function editar_perfil(id, nome) {
    $("#hdnid_Perfil").val(id);
    $("#nome_Perfil").val(nome);
};

function adicionaRelacionamento(url) {

    $(".input-file").hide();
    $(".loader").show();
    url += "&cod_tipo=" + $("#selTipoAcesso").val() + "&id_tiposituacaoacomodacao=" + $("#selTipoSituacaoAcomodacao").val() + "&id_tipoatividadeacomodacao=" + $("#selTipoAtividadeAcomodacao").val();
    window.document.location = url;
};