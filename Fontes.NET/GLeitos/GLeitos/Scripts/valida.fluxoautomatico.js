//$(document).ready(function () {

//    //

//});

function salvarfluxo(url) {
    
    $(".input-file").hide();
    $(".loader").show();
    //$("#hdnid_TipoSituacaoAcomodacao").val($("#selTipoSituacaoAcomodacao").val());
    //$("#hdnid_TipoAtividadeAcomodacao").val($("#selTipoAtividadeAcomodacao").val());
    //$("#hdnid_TipoAcaoAcomodacao").val($("#selTipoAcaoAcomodacao").val());
    var selecionado;
    $('#selTipoSituacaoAcomodacaoOrigem :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoSituacaoAcomodacaoOrigem").val(selecionado);
    selecionado = '';
    $('#selTipoAtividadeAcomodacaoOrigem :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoAtividadeAcomodacaoOrigem").val(selecionado);
    selecionado = '';
    $('#selTipoAcaoAcomodacaoOrigem :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoAcaoAcomodacaoOrigem").val(selecionado);
    selecionado = '';;
    $('#selTipoSituacaoAcomodacaoDestino :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoSituacaoAcomodacaoDestino").val(selecionado);
    selecionado = '';
    $('#selTipoAtividadeAcomodacaoDestino :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoAtividadeAcomodacaoDestino").val(selecionado);

    //url = "&selTipoAcaoAcomodacao=" + $("#hdnid_TipoAcaoAcomodacao").val() + "&id_tiposituacaoacomodacao=" + $("#hdnid_TipoSituacaoAcomodacao").val() + "&id_tipoatividadeacomodacao=" + $("#hdnid_TipoAtividadeAcomodacao").val();

    //alert(url);
    window.document.forms[0].submit();
};

