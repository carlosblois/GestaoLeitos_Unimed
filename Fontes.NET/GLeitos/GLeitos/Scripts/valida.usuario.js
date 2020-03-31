
$(document).ready(function () {

    $("#selPerfil").click(function () {
        var selecionados = '';

        $('#selPerfil :selected').each(function () {
            selecionados = selecionados + ($(this).val().padStart(2, '0')) + '#';
        });
        
        $("#hdnListaPerfilSel").val(selecionados);
        
    });

});


function salvarusuario(url) {
    $(".input-file").hide();
    $(".loader").show();
    $("#hdnAtivo").val($("#selAtivo").val());
    window.document.forms[0].submit();
};

