//$(document).ready(function () {

//    //

//});

function listarsla(url) {
    
    $(".input-file").hide();
    $(".loader").show();
    //$("#hdnid_TipoSituacaoAcomodacao").val($("#selTipoSituacaoAcomodacao").val());
    //$("#hdnid_TipoAtividadeAcomodacao").val($("#selTipoAtividadeAcomodacao").val());
    //$("#hdnid_TipoAcaoAcomodacao").val($("#selTipoAcaoAcomodacao").val());
    var selecionado;
    $('#selTipoSituacaoAcomodacao :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoSituacaoAcomodacao").val(selecionado);
    selecionado = '';
    $('#selTipoAtividadeAcomodacao :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoAtividadeAcomodacao").val(selecionado);
    selecionado = '';
    $('#selTipoAcaoAcomodacao :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoAcaoAcomodacao").val(selecionado);

    selecionado = '';
    $('#selTipoAcomodacao :selected').each(function () {
        selecionado = ($(this).val());
    });

    $("#hdnid_TipoAcomodacao").val(selecionado);

   // url = "&selTipoAcaoAcomodacao=" + $("#hdnid_TipoAcaoAcomodacao").val() + "&id_tiposituacaoacomodacao=" + $("#hdnid_TipoSituacaoAcomodacao").val() + "&id_tipoatividadeacomodacao=" + $("#hdnid_TipoAtividadeAcomodacao").val() + "&id_tipoacaoacomodacao=" + $("#hdnid_TipoAcomodacao").val();

    //alert(url);
    window.document.forms[0].submit();
};

