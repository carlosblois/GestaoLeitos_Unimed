
$(document).ready(function () {
    $('#tblacomodacao').DataTable({
        "language": {
            "lengthMenu": "Mostrando _MENU_ registros por p&aacute;gina",
            "zeroRecords": "Nada encontrado",
            "info": "Mostrando p&aacute;gina _PAGE_ de _PAGES_",
            "infoEmpty": "Nenhum registro dispon&iacute;vel",
            "infoFiltered": "(filtrado de _MAX_ registros no total)"
        }
    });
    $("#btn_novo").click(function () {
        $(".input-file").hide();
        $(".loader").show();
    });

    $("#btn_atualizar").click(function () {
        $(".input-file").hide();
        $(".loader").show();
    });
    
});

function atualizar(url) {
    $(".input-file").hide();
    $(".loader").show();
    $("#hdnid_TipoSituacaoAcomodacao").val($("#selTipoSituacao").val());
    window.document.location = url + "?id_tiposituacaoacomodacao=" + $("#selTipoSituacao").val();
};

function geraratividade(url) {
    $(".input-file").hide();
    $(".loader").show();
    $("#hdnid_TipoSituacaoAcomodacao").val($("#selTipoSituacao").val());
    $("#hdnid_TipoAtividadeAcomodacao").val($("#selTipoAtividade").val());
    window.document.location = url + "&id_tiposituacaoacomodacao=" + $("#selTipoSituacao").val() + "&id_tipoatividadeacomodacao=" + $("#selTipoAtividade").val();
};