$(document).ready(function () {

    $("#selItemCheckList").click(function () {
        var selecionados = '';

        $('#selItemCheckList :selected').each(function () {
            selecionados = selecionados + ($(this).val().padStart(4, '0')) + '#';
        });

        $("#hdnListaItemCheckListSel").val(selecionados);

    });


    $("#btn_adicionar").click(function () {
        var selecionados = '';
        $('#selItemCheckList :selected').each(function () {
            $('#selItemCheckListSel').append('<option value="' + $(this).val() + '">' + $(this).text() + '</option>');
            $(this).remove();
            //selecionados = selecionados + $(this).val() + '#'
        });

        $('#selItemCheckListSel option').each(function () {
            selecionados = selecionados + $(this).val() + '#'
        });

        $("#hdnItemCheckListSel").val(selecionados);
        //alert($("#hdnItemCheckListSel").val());
    });

    $("#btn_remover").click(function () {
        var selecionados = '';

        $('#selItemCheckListSel :selected').each(function () {
            $('#selItemCheckList').append('<option value="' + $(this).val() + '">' + $(this).text() + '</option>');
            $(this).remove();
        });

        $('#selItemCheckListSel option').each(function () {
            selecionados = selecionados + $(this).val() + '#'
        });

        $("#hdnItemCheckListSel").val(selecionados);
        //alert($("#hdnItemCheckListSel").val());
    });

});

function salvarchecklist(url) {
    $(".input-file").hide();
    $(".loader").show();
    window.document.forms[0].submit();
};

function adicionaRelacionamento(url) {
    
    $(".input-file").hide();
    $(".loader").show();
    var selecionados = '';

    $('#selTipoSituacaoAcomodacao :selected').each(function () {
        selecionados = selecionados + $(this).val() + '#'
    });
    //$("#hdnid_TipoSituacaoAcomodacao").val($("#selTipoSituacaoAcomodacao").val());
    $("#hdnid_TipoSituacaoAcomodacao").val(selecionados);

    // ATIVIDADES
    selecionados = '';
    $('#selTipoAtividadeAcomodacao :selected').each(function () {
        selecionados = selecionados + $(this).val() + '#'
    });
    //$("#hdnid_TipoAtividadeAcomodacao").val($("#selTipoAtividadeAcomodacao").val());
    $("#hdnid_TipoAtividadeAcomodacao").val(selecionados);


    // TIPO ACOMODACAO
    selecionados = '';
    $('#selTipoAcomodacao :selected').each(function () {
        selecionados = selecionados + $(this).val() + '#'
    });
    //$("#hdnid_TipoAcomodacao").val($("#selTipoAcomodacao").val());
    $("#hdnid_TipoAcomodacao").val(selecionados);

    url += "?id_checklist=" + $("#htnid_CheckList").val() + "&pTipoacomodacao=" + $("#selTipoAcomodacao").val() + "&pTiposituacaoacomodacao=" + $("#selTipoSituacaoAcomodacao").val() + "&pTipoatividadeacomodacao=" + $("#selTipoAtividadeAcomodacao").val();
    //url += "?id_checklist=" + $("#htnid_CheckList").val() + "&id_tipoacomodacao=" + $("#selTipoAcomodacao").val() + "&id_tiposituacaoacomodacao=" + $("#selTipoSituacaoAcomodacao").val() + "&id_tipoatividadeacomodacao=" + $("#selTipoAtividadeAcomodacao").val();
    window.document.location = url;
};

