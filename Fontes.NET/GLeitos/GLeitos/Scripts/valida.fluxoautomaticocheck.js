$(document).ready(function () {

    $("#selCheckList").change(function () {

        var selecionados = '';

        $('#selCheckList :selected').each(function () {
            selecionados = selecionados + $(this).val();
        });
        
        if (selecionados != '') { 
            CarregaComboItemCheckList(selecionados); 
        }
        
    });

});

function salvarfluxocheck(url) {
    
    $(".input-file").hide();
    $(".loader").show();

    var selecionado;
    $('#selTipoSituacaoAcomodacao :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoSituacaoAcomodacao").val(selecionado);

    var selecionado;
    $('#selCheckList :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_Checklist").val(selecionado);

    selecionado = '';
    $('#selItemCheckList :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_ItemChecklist").val(selecionado);

    selecionado = '';
    $('#SelTipoAtividadeAcomodacao :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdnid_TipoAtividadeAcomodacao").val(selecionado);

    selecionado = '';;
    $('#selTipoResposta :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdncod_Resposta").val(selecionado);

    selecionado = '';
    $('#selTipoFinalizacao :selected').each(function () {
        selecionado = ($(this).val());
    });
    $("#hdncod_PermiteTotal").val(selecionado);

    //url = "&selTipoAcaoAcomodacao=" + $("#hdnid_TipoAcaoAcomodacao").val() + "&id_tiposituacaoacomodacao=" + $("#hdnid_TipoSituacaoAcomodacao").val() + "&id_tipoatividadeacomodacao=" + $("#hdnid_TipoAtividadeAcomodacao").val();

    //alert(url);
    window.document.forms[0].submit();
};



