$(document).ready(function () {
    // funcao para validar se a data é uma data válida.
    //$.validator.addMethod(
    //    "dateFormat",
    //    function (value, element) {
    //        var check = false;
    //        var re = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
    //        if (re.test(value)) {
    //            var adata = value.split('/');
    //            var dd = parseInt(adata[0], 10);
    //            var mm = parseInt(adata[1], 10);
    //            var yyyy = parseInt(adata[2], 10);
    //            var xdata = new Date(yyyy, mm - 1, dd);
    //            if ((xdata.getFullYear() === yyyy) && (xdata.getMonth() === mm - 1) && (xdata.getDate() === dd)) {
    //                check = true;
    //            }
    //            else {
    //                check = false;
    //            }
    //        } else {
    //            check = false;
    //        }
    //        return this.optional(element) || check;
    //    },
    //    "Informe uma data v&aacute;lida."
    //);

   
});
// declarando as variáveis para a modal do leito




function validaArquivoImagem(elemento) {

    var allowedExtension = ['jpeg', 'jpg', 'png', 'gif', 'bmp'];
    var fileExtension = document.getElementById(elemento).value.split('.').pop().toLowerCase();
    var isValidFile = false;

    for (var index in allowedExtension) {

        if (fileExtension === allowedExtension[index]) {
            isValidFile = true;
            break;
        }
    }

    if (!isValidFile) {
        alert('As extenções permitidas são: *.' + allowedExtension.join(', *.'));
    }

    return isValidFile;
}


function ValidaData(data) {
    reg = /[^\d\/\.]/gi;                  // Mascara = dd/mm/aaaa | dd.mm.aaaa
    var valida = data.replace(reg, '');    // aplica mascara e valida só numeros
    if (valida && valida.length == 10) {  // é válida, então ;)
        var ano = data.substr(6),
            mes = data.substr(3, 2),
            dia = data.substr(0, 2),
            M30 = ['04', '06', '09', '11'],
            v_mes = /(0[1-9])|(1[0-2])/.test(mes),
            v_ano = /(19[1-9]\d)|(20\d\d)|2100/.test(ano),
            rexpr = new RegExp(mes),
            fev29 = ano % 4 ? 28 : 29;

        if (v_mes && v_ano) {
            if (mes == '02') return (dia >= 1 && dia <= fev29);
            else if (rexpr.test(M30)) return /((0[1-9])|([1-2]\d)|30)/.test(dia);
            else return /((0[1-9])|([1-2]\d)|3[0-1])/.test(dia);
        }
    }
    return false                           // se inválida :(
}


function mascaraData(val) {
    var pass = val.value;
    var expr = /[0123456789]/;

    for (i = 0; i < pass.length; i++) {
        // charAt -> retorna o caractere posicionado no índice especificado
        var lchar = val.value.charAt(i);
        var nchar = val.value.charAt(i + 1);

        if (i == 0) {
            // search -> retorna um valor inteiro, indicando a posição do inicio da primeira
            // ocorrência de expReg dentro de instStr. Se nenhuma ocorrencia for encontrada o método retornara -1
            // instStr.search(expReg);
            if ((lchar.search(expr) != 0) || (lchar > 3)) {
                val.value = "";
            }

        } else if (i == 1) {

            if (lchar.search(expr) != 0) {
                // substring(indice1,indice2)
                // indice1, indice2 -> será usado para delimitar a string
                var tst1 = val.value.substring(0, (i));
                val.value = tst1;
                continue;
            }

            if ((nchar != '/') && (nchar != '')) {
                var tst1 = val.value.substring(0, (i) + 1);

                if (nchar.search(expr) != 0)
                    var tst2 = val.value.substring(i + 2, pass.length);
                else
                    var tst2 = val.value.substring(i + 1, pass.length);

                val.value = tst1 + '/' + tst2;
            }

        } else if (i == 4) {

            if (lchar.search(expr) != 0) {
                var tst1 = val.value.substring(0, (i));
                val.value = tst1;
                continue;
            }

            if ((nchar != '/') && (nchar != '')) {
                var tst1 = val.value.substring(0, (i) + 1);

                if (nchar.search(expr) != 0)
                    var tst2 = val.value.substring(i + 2, pass.length);
                else
                    var tst2 = val.value.substring(i + 1, pass.length);

                val.value = tst1 + '/' + tst2;
            }
        }

        if (i >= 6) {
            if (lchar.search(expr) != 0) {
                var tst1 = val.value.substring(0, (i));
                val.value = tst1;
            }
        }
    }

    if (pass.length > 10)
        val.value = val.value.substring(0, 10);
    return true;
}


function editar(url) {
    $(".input-file").hide();
    $(".loader").show();
    window.location.href = url;
};


function excluir(url,msg) {
    $(".input-file").hide();
    $("#lblQuestion").html(msg);
    $("#urlExclusao").val(url);
    $(".msgbackground").show();
};

function excluir_no() {
    $(".input-file").show();
    $(".msgbackground").hide();
};

function emite_msg(msg) {
    $(".input-file").hide();
    $("#lblMsgOK").html(msg);
    $(".msgokbackground").show();
};

function msg_ok() {
    $(".input-file").show();
    $(".msgokbackground").hide();
};

function excluir_yes() {
    var url = $("#urlExclusao").val();
    $(".msgbackground").hide();
    $(".loader").show();
    window.location.href = url;
};

function salvar(url) {
    $(".input-file").hide();
    $(".loader").show();
    window.document.forms[0].submit();
};

function carregaTela(url) {
    $(".input-file").hide();
    $(".loader").show();
    window.location.href = url;
};

function MostraDetalheAcomodacaoTipoAtividade(url, TipoAtividade, PerfilUsuario, PerfilAdministrador, cod_Acesso) {
    //debugger;
    $(".input-file").hide();
    $(".loader").show();
    $(".box").attr("cursor", "wait");
    clearInterval(IntervaloAtualizacao);
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    getAcomodacao(url, hdntkn_administrativo, TipoAtividade);
    $("#hdnPerfilUsuario").val(PerfilUsuario);
    $("#hdnPerfilAdministrador").val(PerfilAdministrador);
    $("#hdnCodAcesso").val(cod_Acesso);
    $("#modalAcomodacao").show();
    CarregaHistory();
    $(".box").attr("cursor", "default");
    $("#hdnEncaminhaDireto").val('N');
    $(".loader").hide();
}

function EncaminharDiretamente(url, TipoAtividade, PerfilUsuario, PerfilAdministrador, cod_Acesso, TipoSituacao) {
    $(".box").attr("cursor", "wait");
    clearInterval(IntervaloAtualizacao);
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    //alert('EncaminharDiretamente..... ');
    getAcomodacao(url, hdntkn_administrativo, TipoAtividade);
    $("#hdnPerfilUsuario").val(PerfilUsuario);
    $("#hdnPerfilAdministrador").val(PerfilAdministrador);
    $("#hdnCodAcesso").val(cod_Acesso);
    $(".box").attr("cursor", "default");
    $("#hdnEncaminhaDireto").val('S');
    PrepararEncaminhamento(TipoSituacao);
    $("#modalAcomodacao").hide();
    return;
}

function MostraDetalheAcomodacaoAtividade(url, PerfilUsuario, PerfilAdministrador, cod_Acesso) {
    //debugger;
    var TipoAtividade = ''; 
    $(".input-file").hide();
    $(".loader").show();
    $(".box").attr("cursor", "wait");
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    getAcomodacao(url, hdntkn_administrativo, TipoAtividade);
    $("#hdnPerfilUsuario").val(PerfilUsuario);
    $("#hdnPerfilAdministrador").val(PerfilAdministrador);
    $("#hdnCodAcesso").val(cod_Acesso);
    $("#modalAcomodacao").show();
    CarregaHistory();
    $(".box").attr("cursor", "default");
    $(".loader").hide();
    $("#hdnEncaminhaDireto").val('N');
}

function FecharDetalheAcomodacao() {
    $(".input-file").show();
    var IntervaloAtualizacao = setInterval(function () { location.reload(); }, 60000); // volta o tempo para o padrão
    location.reload();
    $("#modalAcomodacao").hide();
}

function FecharEncaminhamento() {
    $("#idDivMsgEncaminhamento").hide();
}

function getAcomodacao(caminho_url, token, TipoAtividade) {
    //debugger;
    var objResult;
    var jsonResult = $.ajax({
        url: caminho_url,
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        async: false,
        success: function (data) {
            objResult = data;
        },
        statusCode: {
            404: function () {
                var err = statusCode + ", " + error;
                emite_msg(err);
            }
        }
        })
        .done(function (data) {

            if (data.length > 0) {
                if (data) {
                    if (TipoAtividade != '') {
                        CarregaDadosAcomodacao(data, TipoAtividade);
                    }
                    else {
                        CarregaDadosAcomodacao(data);
                    }

                }
            }
        })
        .fail(function (jqxhr, textStatus, error) {
            console.log("************************************");
            console.log("Erro: " + textStatus + " - " + error);
            console.log(jqxhr.statusText);
            console.log("************************************");
            var err = textStatus + ", " + error;
            emite_msg(err);
        })
        .always(function () {
            console.log("complete");
        });
    
}

function getHistoricoAcomodacao(caminho_url, token) {
    var objResult;
    //debugger;
   // alert(caminho_url);
    var jsonResult = $.ajax({
        url: caminho_url,
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        async: false,
        success: function (data) {
            objResult = data;
        },
        statusCode: {
            404: function () {
                var err = statusCode + ", " + error;
                emite_msg(err);
            }
        }
    })
        .done(function (data) {
            if (data) {
                ////return data;
                var conteudoHistory = '';

                conteudoHistory += '<div class="titleLine">';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 110px !important;">';
                conteudoHistory += '        <title>SLA</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 60px !important;">';
                conteudoHistory += '        <title>Meta</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 150px !important;">';
                conteudoHistory += '       <title>Atividades</title>';
                conteudoHistory += '   </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 100px !important;">';
                conteudoHistory += '       <title>Responsável</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 150px !important;">';
                conteudoHistory += '       <title>Solicitada</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 70px !important;">';
                conteudoHistory += '        <title>Aceita</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 70px !important;">';
                conteudoHistory += '        <title>Iniciada</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 100px !important;">';
                conteudoHistory += '        <title>Finalizada</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn">';
                conteudoHistory += '        <title>Ação</title>';
                conteudoHistory += '   </div>';
                conteudoHistory += '    <div class="cb"></div>';
                conteudoHistory += '</div>';
                if (data.length > 0) {
                    conteudoHistory += '<div class="scrollinghistory">';
                    $.each(data, function (key, val) {
                        var datafim = new Date();
                                                
                        if (trataretorno(val.checkout) != '') {
                            datafim = new Date(val.checkout);
                        }
                        else
                            if (trataretorno(val.semicheckout) != '') {
                                datafim = new Date(val.semicheckout);
                            }
                            else
                                if (trataretorno(val.dt_FimAtividadeAcomodacao) != '') {
                                    datafim = new Date(val.dt_FimAtividadeAcomodacao);
                                }
                           
                        var inicio = new Date(val.solicitado);
                        var pTempoExecucao = difMinutos(inicio, datafim);

                        var strImagem = defineImagemSLAHist(trataretorno(val.solicitado), datafim, val.sla);
                        if (val.now != '') {
                            conteudoHistory += '<div class="line">';
                            conteudoHistory += '<div class="modaltabColumn" style="width: 110px !important; display: flex; padding-left: 10px;">';
                            conteudoHistory += '<div><img class="imgModal" src="' + strImagem + '"></div><div>' + '&nbsp;' + trataretornoHtml(pTempoExecucao) + ' min</div>';
                            conteudoHistory += '</div>';
                            conteudoHistory += '<div class="modaltabColumn" style="width: 60px !important;">';
                            conteudoHistory += '' + trataretornoHtml(val.sla) + ' min';
                            conteudoHistory += '</div>';
                            conteudoHistory += '<div class="modaltabColumn" style="width: 150px !important;">';
                            conteudoHistory += '' + trataretornoHtml(val.nome_TipoAtividadeAcomodacao);
                            conteudoHistory += '</div>';

                            conteudoHistory += '<div class="modaltabColumn" style="width: 100px !important;" title="' + trataretornoHtml(val.nome_Usuario) + '">';
                            conteudoHistory += '' + trataretornoHtml(val.login_Usuario);
                            conteudoHistory += '</div>';

                            conteudoHistory += '<div class="modaltabColumn" style="width: 150px !important;">';
                            conteudoHistory += '' + formataData(trataretornoHtml(val.solicitado), 'C');
                            conteudoHistory += '</div>';
                            conteudoHistory += '<div class="modaltabColumn" style="width: 70px !important;">';
                            //alert('chamado:' + val.chamado);
                            conteudoHistory += '' + formataData(trataretornoHtml(val.aceite), 'H');
                            conteudoHistory += '</div>';
                            conteudoHistory += '<div class="modaltabColumn" style="width: 70px !important;">';
                            //alert('checkin:' + val.checkin);
                            conteudoHistory += '' + formataData(trataretornoHtml(val.checkin), 'H');
                            conteudoHistory += '</div>';
                            conteudoHistory += '<div class="modaltabColumn" style="width: 100px !important;display:flex; padding-left: 10px;"> ';
                            //alert('checkout:' + val.checkout);

                            var strImagemFinalizada = '';

                            if (trataretorno(val.semicheckout) == '' && trataretorno(val.checkout) == '' && trataretorno(val.dt_FimAtividadeAcomodacao) != '') {
                                strImagemFinalizada = defineImagemFinalizada(trataretorno(val.checkout), trataretorno(val.dt_FimAtividadeAcomodacao));
                            }
                            else {
                                strImagemFinalizada = defineImagemFinalizada(trataretorno(val.checkout), trataretorno(val.semicheckout));
                            }

                            if (trataretorno(val.checkout) != '') {
                                conteudoHistory += '<div><img class="imgModal" style="width: 27px !important;" src="' + strImagemFinalizada + '" title="Finalizada Total"></div>';
                                conteudoHistory += '<div>&nbsp;' + formataData(trataretornoHtml(val.checkout), 'H') + '</div>';
                            }
                            else if (trataretorno(val.semicheckout) != '') {
                                conteudoHistory += '<div><img class="imgModal" style="width: 27px !important;" src="' + strImagemFinalizada + '" title="Finalizada Parcial"></div>';
                                conteudoHistory += '<div>&nbsp;' + formataData(trataretornoHtml(val.semicheckout), 'H') + '</div>';
                            }
                            else
                                if (trataretorno(val.dt_FimAtividadeAcomodacao) != '') {
                                    conteudoHistory += '<div><img class="imgModal" style="width: 27px !important;" src="' + strImagemFinalizada + '" title="Finalizada Parcial"></div>';
                                    conteudoHistory += '' + formataData(trataretornoHtml(val.dt_FimAtividadeAcomodacao), 'H');
                                }
                                else {
                                    conteudoHistory += '' + formataData(trataretornoHtml(val.checkout), 'H');
                                }
                            
                            conteudoHistory += '</div>';
                            conteudoHistory += '<div class="modaltabColumn">';

                            if (trataretorno(val.dt_FimAtividadeAcomodacao) != '') {
                                conteudoHistory += 'Encerrado';
                            }
                            else {
                                if ((trataretorno(val.checkout) == '' || trataretorno(val.checkin) == '' || trataretorno(val.aceite) == '') && trataretorno(val.solicitado) != '')
                                    conteudoHistory += '                <img src="../images/icon_stop_modal.png" onclick="EncerrarAtividade(' + val.id_AtividadeAcomodacao + ')">';
                                else
                                    conteudoHistory += '                <img src="../images/icon_modal_action_off.png">';
                            }
                            conteudoHistory += '&nbsp;&nbsp;<a href="#" onclick="PrepararMensagem(' + val.id_AtividadeAcomodacao + ', ' + val.id_Usuario + ');return false;"><i class="fas fa-comments"></i></a>';

                            conteudoHistory += '                    </div>';
                            conteudoHistory += '                <div class="cb"></div>';
                            conteudoHistory += '            </div>';

                        }
                    });
                    conteudoHistory += '</div>';
                }
                $("#divModalHistory").html(conteudoHistory);


            }
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            emite_msg(err);
        })
        .always(function () {
            console.log("complete");
        });

}




function getMensagens(caminho_url, token) {
    var objResult;
    //debugger;
    // alert(caminho_url);
    var jsonResult = $.ajax({
        url: caminho_url,
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        async: false,
        success: function (data) {
            objResult = data;
        },
        statusCode: {
            404: function () {
                var err = statusCode + ", " + error;
                emite_msg(err);
            }
        }
    })
        .done(function (data) {
            if (data) {
                ////return data;
                var conteudoHistory = '';

                conteudoHistory += '<div class="titleLine">';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 120px !important;">';
                conteudoHistory += '        <title>Data Emissão</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 120px !important;">';
                conteudoHistory += '        <title>Recebida</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 100px !important;">';
                conteudoHistory += '       <title>Emissor</title>';
                conteudoHistory += '   </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 100px !important;">';
                conteudoHistory += '       <title>Receptor</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '    <div class="modaltabColumn" style="width: 465px !important; text-align:left !important;">';
                conteudoHistory += '       <title>Mensagem</title>';
                conteudoHistory += '    </div>';
                conteudoHistory += '</div>';
                if (data.length > 0) {
                    conteudoHistory += '<div class="scrollinghistoryMensagem">';
                    $.each(data, function (key, val) {
                        var dataEmissao = '';
                        if (trataretorno(val.dt_EnvioMensagem) != '') {
                            dataEmissao = val.dt_EnvioMensagem;
                        }

                        var Recebimento = '-';                        
                        if (trataretorno(val.dt_RecebimentoMensagem) != '') {
                            Recebimento = formataData(val.dt_RecebimentoMensagem, 'C');
                        }
                        if (val.now != '') {

                            conteudoHistory += '<div class="line">';

                                conteudoHistory += '<div class="modaltabColumn" style="width: 120px !important; display: flex; padding-left: 10px;">';
                                conteudoHistory += '<div>' + formataData(dataEmissao, 'C') + '</div>';
                                conteudoHistory += '</div>';
                                conteudoHistory += '<div class="modaltabColumn" style="width: 120px !important;">';
                                conteudoHistory += '' + Recebimento + '';
                                conteudoHistory += '</div>';

                                conteudoHistory += '<div class="modaltabColumn" style="width: 100px !important;" title="' + trataretornoHtml(val.login_Usuario_Emissor) + '">';
                                conteudoHistory += '' + trataretornoHtml(val.login_Usuario_Emissor);
                                conteudoHistory += '</div>';

                                conteudoHistory += '<div class="modaltabColumn" style="width: 100px !important;" title="' + trataretornoHtml(val.login_Usuario_Destinatario) + '">';
                                conteudoHistory += '' + trataretornoHtml(val.login_Usuario_Destinatario);
                                conteudoHistory += '</div>';

                            conteudoHistory += '<div class="modaltabColumn" style="width: 465px !important; text-align:left !important;">';
                                conteudoHistory += '' + trataretornoHtml(val.textoMensagem) + '';
                                conteudoHistory += '</div>';

                                conteudoHistory += '    <div class="cb"></div>';

                            conteudoHistory += '</div>';
                            
                        }

                    });
                    conteudoHistory += '</div>';
                }
                $("#divModalHistoryMensagem").html(conteudoHistory);


            }
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            emite_msg(err);
        })
        .always(function () {
            console.log("complete");
        });

}


function EnviarMensagem() {
    $(".loader").show();

    var hdntkn_operacional = $("#hdntkn_operacional").val();
    var IdAtividadeAcomodacao = $("#hdnIdAtividadeMsg").val();
    var IdUsuarioEmissor = $("#hdnIdUsuarioEmissor").val();
    var IdUsuarioDest = $("#hdnIdUsuarioDest").val();
    debugger;
    if (IdUsuarioDest == '' || IdUsuarioDest == '0')
        IdUsuarioDest = null;

    var IdEmpresa = $("#hdnIdEmpresa").val();
    var urlsendMensagem = $("#hdnurl_sendMensagem").val();
    var textoMsg = $("#txtMensagemAtividade").val();

    if (textoMsg.trim() == '') {
        emite_msg('Por favor, informe um texto para mensagem.');
        $(".loader").hide();
        return;
    }    
    var d = new Date();
    var ano = d.getFullYear();
    var dia = trataZero(d.getDate(), 2);
    var mes = trataZero(d.getMonth() + 1, 2);
    var hora = trataZero(d.getHours(), 2);
    var minuto = trataZero(d.getMinutes(), 2);
    var segundo = trataZero(d.getSeconds(), 2);

    var dados = {
                id_Mensagem: 0,
                id_AtividadeAcomodacao: IdAtividadeAcomodacao,
                dt_EnvioMensagem: ano + '-' + mes + '-' + dia + 'T' + hora + ':' + minuto + ':' + segundo + '.' + d.getMilliseconds(),
                dt_RecebimentoMensagem: "",
                id_Empresa: IdEmpresa,
                id_Usuario_Emissor: IdUsuarioEmissor,
                id_Usuario_Destinatario: IdUsuarioDest,
                textoMensagem: textoMsg
            }
    var myJSON = JSON.stringify(dados);
    //alert(myJSON);
    postAPIJson(urlsendMensagem, hdntkn_operacional, dados);

    /// ATUALIZA OS DADOS EM TELA
    CarregaMensagem(IdAtividadeAcomodacao);
    emite_msg('Mensagem enviada com sucesso!');
    $("#txtMensagemAtividade").val('');
    $(".box").attr("cursor", "default");
    $(".loader").hide();
}


function EncerrarAtividade(IdAtividadeAcomodacao) {
    ///alert('Encerrado.');

    var PerfilUsuario = $("#hdnPerfilUsuario").val();
    var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    var codAcesso = $("#hdnCodAcesso").val();

    if (!ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso)) {
        return false;
    }

    var msg = 'Confirma a finalização desta atividade?';
    $(".input-file").hide();
    $("#lblQuestionAtividade").html(msg);
    $("#hdnIdAtividadeAcomodacao").val(IdAtividadeAcomodacao);
    //alert($("#hdnIdAtividadeAcomodacao").val());
    $(".msgencerrabackground").show();

}

function EncerrarAtividade_yes() {
    ///alert('Encerrado.');
    

    //var PerfilUsuario = $("#hdnPerfilUsuario").val();
    //var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    //var codAcesso = $("#hdnCodAcesso").val();
   
    //if (!ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso)) {
    //    return false;
    //}

    $(".msgencerrabackground").hide();
    $(".loader").show();


    var IdAcomodacao = $("#hdnIdAcomodacao").val();
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    var hdntkn_operacional = $("#hdntkn_operacional").val();
    var IdAtividadeAcomodacao = $("#hdnIdAtividadeAcomodacao").val();
    var url = $("#hdnurl_CancelarAtividade").val();
    var hdnIdTipoAtividadeAcomodacao = '';

    url += IdAtividadeAcomodacao;
    if (postAPI(url, hdntkn_operacional)) { }

    /// ATUALIZA OS DADOS EM TELA
    url = $("#hdnurl_getAcomodacao").val();
    url += IdAcomodacao;
    //alert(url);
    getAcomodacao(url, hdntkn_administrativo, hdnIdTipoAtividadeAcomodacao);
    CarregaHistory();
    $(".box").attr("cursor", "default");
    $(".loader").hide();
}

function EncerrarAtividade_no() {
    $(".input-file").show();
    $(".msgencerrabackground").hide();
}

function LimpezaPlus() {


    
    if ($("#linkLimpezaPlus").is("[disabled]")) {
        event.preventDefault();
        //var err = 'Operação inválida para esta atividade.';
        //emite_msg(err);
        return false;
    }

    var PerfilUsuario = $("#hdnPerfilUsuario").val();
    var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    var codAcesso = $("#hdnCodAcesso").val();

    if (!ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso)) {
        return false;
    }
    
    $(".input-file").hide();

    var IdAcomodacao = $("#hdnIdAcomodacao").val();
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    var hdntkn_operacional = $("#hdntkn_operacional").val();
    var IdAtividadeAcomodacao = $("#hdnIdAtividadeAcomodacao").val();
    var url = $("#hdnurl_limpezaPlusAcomodacao").val();
    var hdnIdTipoAtividadeAcomodacao = $("#hdnIdTipoAtividadeAcomodacao").val();
    
    var limpezaPlus = $("#hdncod_Plus").val();
    if (limpezaPlus == 'S')
        limpezaPlus = 'N';
    else 
        limpezaPlus = 'S';
    //S ? idAtividade = 12222
    url += limpezaPlus + "?idAtividade=" + IdAtividadeAcomodacao;
    postAPI(url, hdntkn_operacional);

    /// ATUALIZA OS DADOS EM TELA
    url = $("#hdnurl_getAcomodacao").val();
    url += IdAcomodacao;
    getAcomodacao(url, hdntkn_administrativo, hdnIdTipoAtividadeAcomodacao);
    
}

function Isolar() {

    var PerfilUsuario = $("#hdnPerfilUsuario").val();
    var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    var codAcesso = $("#hdnCodAcesso").val();

    if (!ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso)) {
        return false;
    }

    $(".input-file").hide();
    
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    var IdAcomodacao = $("#hdnIdAcomodacao").val();
    var url = $("#hdnurl_isolaAcomodacao").val();
    var hdnIdTipoAtividadeAcomodacao = $("#hdnIdTipoAtividadeAcomodacao").val();

    var cod_Isolamento = $("#hdncod_Isolamento").val();
    
    if (cod_Isolamento == 'S')
        cod_Isolamento = 'N';
    else
        cod_Isolamento = 'S';

    url += cod_Isolamento + "?idAcomodacao=" + IdAcomodacao;

    postAPI(url, hdntkn_administrativo);

    /// ATUALIZA OS DADOS EM TELA
    url = $("#hdnurl_getAcomodacao").val();
    url += IdAcomodacao;
    getAcomodacao(url, hdntkn_administrativo, hdnIdTipoAtividadeAcomodacao);

    
}

function Priorizar() {

    
    if ($("#imgPriorizar").is("[disabled]")) {
        event.preventDefault();
        //var err = 'Operação inválida para esta atividade.';
        //emite_msg(err);
        return false;
    }

    var PerfilUsuario = $("#hdnPerfilUsuario").val();
    var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    var codAcesso = $("#hdnCodAcesso").val();

    if (!ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso)) {
        return false;
    }

    $(".input-file").hide();
    
    var IdAcomodacao = $("#hdnIdAcomodacao").val();
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    var hdntkn_operacional = $("#hdntkn_operacional").val();
    var IdAtividadeAcomodacao = $("#hdnIdAtividadeAcomodacao").val();
    var url = $("#hdnurl_priorizaAcomodacao").val();
    var prioridadeAtividade = $("#hdnprioridadeAtividade").val();
    var hdnIdTipoAtividadeAcomodacao = $("#hdnIdTipoAtividadeAcomodacao").val();

    if (prioridadeAtividade == 'S')
        prioridadeAtividade = 'N';
    else
        prioridadeAtividade = 'S';

    //S ? idAtividade = 12222
    url += prioridadeAtividade + "?idAtividade=" + IdAtividadeAcomodacao;

    postAPI(url, hdntkn_operacional);

    /// ATUALIZA OS DADOS EM TELA
    url = $("#hdnurl_getAcomodacao").val();
    url += IdAcomodacao;
    getAcomodacao(url, hdntkn_administrativo, hdnIdTipoAtividadeAcomodacao);

    
}

function PrepararMensagem(idAtividade, idUsuarioDest) {
    if (idAtividade == null) {
        idAtividade = '';
    }
    $(".loader").show();
    CarregaMensagem(idAtividade, idUsuarioDest);
    $(".loader").hide();
}

function FecharMensagem() {
    $("#idDivMensagem").hide();
}


function PrepararEncaminhamento(TipoSituacao) {

    if (TipoSituacao == null) {
        TipoSituacao = '';
    }

    var PerfilUsuario = $("#hdnPerfilUsuario").val();
    var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    var codAcesso = $("#hdnCodAcesso").val();
    var hdnIdTipoSituacaoAcomodacao = $("#hdnIdTipoSituacaoAcomodacao").val();

    if (!ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso)) {
        return false;
    }

    $(".loader").show();
    if (TipoSituacao != '') {
        hdnIdTipoSituacaoAcomodacao = TipoSituacao;
    }

    CarregaCheckTipoAtividade();
    CarregaComboTipoSituacao(hdnIdTipoSituacaoAcomodacao);
    
    $("#idDivMsgEncaminhamento").show();   
    
    //alert('passei aqui no PreparaEncaminhamento... ');
    $(".loader").hide();

    //if ($("#hdnEncaminhaDireto").val() == 'S') {
    //    return false;
    //}
}

function CarregaComboTipoSituacao(TipoSituacaoAtual) {

    var PerfilUsuario = $("#hdnPerfilUsuario").val();
    var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    var codAcesso = $("#hdnCodAcesso").val();

    if (!ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso)) {
        return false;
    }

    var token = $("#hdntkn_configuracao").val();
    var caminho_url = $("#hdnurl_listatiposituacao").val();
    var txthtml = '';
    var objResult;
    var jsonResult = $.ajax({
        url: caminho_url,
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        async: false,
        success: function (data) {
            objResult = data;
        },
        statusCode: {
            404: function () {
                var err = statusCode + ", " + error;
                emite_msg(err);
            }
        }
    })
        .done(function (data) {
            if (data) {
                //$('#selTipoSituacaoEncaminha').find('option').remove();
                //$('#selTipoSituacaoEncaminha').append('<option value="">SELECIONE</option>');
                //$.each(data, function (key, val) {

                //    if (val.now != '') {
                //        if (TipoSituacaoAtual != val.id_TipoSituacaoAcomodacao) {
                //            $('#selTipoSituacaoEncaminha').append('<option value="' + val.id_TipoSituacaoAcomodacao + '">' + val.nome_TipoSituacaoAcomodacao + '</option>');
                //        }
                //    }
                //});
                // novo padrão de tela.
                //$('#selTipoSituacaoEncaminha').find('a').remove();
                $('#selTipoSituacaoEncaminha').html('');
                $.each(data, function (key, val) {

                    if (val.now != '') {
                        if (TipoSituacaoAtual != val.id_TipoSituacaoAcomodacao) {
                            txthtml += '<a class="bt_situacao" name="btnSituacao" onclick="SelecionarSituacaoEncaminha(' + val.id_TipoSituacaoAcomodacao + ', this)"><img src="../images/' + val.imagem + '"></img><span>' + val.nome_TipoSituacaoAcomodacao + '</span></a>';
                        }
                    }
                });
                $('#selTipoSituacaoEncaminha').html(txthtml);
            }
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            emite_msg(err);
        })
        .always(function () {
            console.log("complete");
        });


}

function CarregaCheckTipoAtividade() {

    var PerfilUsuario = $("#hdnPerfilUsuario").val();
    var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    var codAcesso = $("#hdnCodAcesso").val();

    if (!ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso)) {
        return false;
    }

    var token = $("#hdntkn_configuracao").val();
    var caminho_url = $("#hdnurl_listatipoatividade").val();
    var txthtml = '';
    var objResult;
    var jsonResult = $.ajax({
        url: caminho_url,
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        async: false,
        success: function (data) {
            objResult = data;
        },
        statusCode: {
            404: function () {
                var err = statusCode + ", " + error;
                emite_msg(err);
            }
        }
    })
        .done(function (data) {
            if (data) {

                //$.each(data, function (key, val) {

                //    if (val.now != '') {
                //        txthtml += '<div><input type="checkbox" id="idChkEncaminharAtividade" name="chkEncaminharAtividade" value="' + val.id_TipoAtividadeAcomodacao + '">&nbsp;' + val.nome_TipoAtividadeAcomodacao + '<br /></div>';
                //    }
                //});

                //$("#divListaEncaminhamento").html(txthtml + '<br>');

                $('#selTipoAtividadeEncaminha').html('');
                $.each(data, function (key, val) {

                    if (val.now != '') {
                        txthtml += '<a class="bt_atividade"><spam class="button_off" onclick="SelecionarAtividadeEncaminha(' + val.id_TipoAtividadeAcomodacao + ',this)"></spam>' + val.nome_TipoAtividadeAcomodacao + '</a>';
                        txthtml += '<input type="checkbox" id="idChkEncaminharAtividade" name="chkEncaminharAtividade" value="' + val.id_TipoAtividadeAcomodacao + '" style="display: none;">';
                    }
                });
                $('#selTipoAtividadeEncaminha').html(txthtml);

            }
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            emite_msg(err);
        })
        .always(function () {
            console.log("complete");
        });

    
}

function SelecionarAtividadeEncaminha(TipoAtividade, item) {
    var hdnIdTipoAtividadeSelEncaminha = $("#hdnIdTipoAtividadeSelEncaminha").val();
    var encontrou = '';     
    $('input[id="idChkEncaminharAtividade"]:checked').each(function () {
        if (this.value == TipoAtividade) {
            this.checked = false;
            item.removeAttribute("class");
            item.setAttribute("class", "button_off");
            encontrou = 'S';
            return false;
        }
    });
    if (encontrou != 'S') {
        $('input[id="idChkEncaminharAtividade"]').each(function () {
            if (this.value == TipoAtividade) {
                this.checked = true;
                item.removeAttribute("class");
                item.setAttribute("class", "button_on");
                return false;
            }
        });
    }
}

function SelecionarSituacaoEncaminha(TipoSituacao, item) {

    var hdnIdTipoSituacaoSelEncaminha = $("#hdnIdTipoSituacaoSelEncaminha").val();
   
    $('.bt_situacao').each(function () {
            this.removeAttribute("class");
            this.setAttribute("class", "bt_situacao");
    });
    $('.bt_situacao_selected').each(function () {
        this.removeAttribute("class");
        this.setAttribute("class", "bt_situacao");
    });

    if (hdnIdTipoSituacaoSelEncaminha != TipoSituacao) {
        $("#hdnIdTipoSituacaoSelEncaminha").val(TipoSituacao);
        item.removeAttribute("class");
        item.setAttribute("class", "bt_situacao_selected");
    }
    else {
        $("#hdnIdTipoSituacaoSelEncaminha").val('');
    }
}

function EncaminharAcomodacao() {
    $(".input-file").hide();
    $(".loader").show();
    var hdnIdTipoSituacaoSelEncaminha = $("#hdnIdTipoSituacaoSelEncaminha").val();
    var indRealizado = 0;
    var ids = [];
    $('input[id="idChkEncaminharAtividade"]:checked').each(function () {
        ids.push(this.value);
    });
    var situacaoSel = hdnIdTipoSituacaoSelEncaminha;
    //$('#selTipoSituacaoEncaminha :selected').each(function () {
    //    situacaoSel = situacaoSel + $(this).val()
    //});    
    if (ids.length <= 0 && (situacaoSel == '' || situacaoSel == '0')) {
        $(".loader").hide();
        emite_msg('Escolha uma atividade ou uma situação.');
    }
    else {

        //if (situacaoSel.length > 0) {
        if (situacaoSel != '') {
            EncaminharSituacao(situacaoSel);
            //alert('Encaminha Situacao' + situacaoSel);
            indRealizado = 1;
        }

        if (ids.length > 0) {
            //alert('Encaminha atividade ' + ids.toString());
            EncaminharAtividade();
            indRealizado = 1;
        }

        if (indRealizado == 1) {
            $(".loader").hide();
            emite_msg('Encaminhamento realizado.');
            $("#idDivMsgEncaminhamento").hide();
        }
        //$("#idDivMsgEncaminhamento").hide();
        ///// ATUALIZA OS DADOS EM TELA
        //url = $("#hdnurl_getAcomodacao").val();
        //url += IdAcomodacao;
        //getAcomodacao(url, hdntkn_administrativo, '');
    }
    $(".loader").hide();
}

function EncaminharSituacao(situacaoDestino) {
    $(".input-file").hide();
    //clearInterval(IntervaloAtualizacao);
    var hdntkn_operacional = $("#hdntkn_operacional").val();
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    var url = $("#hdnurl_EncaminharSituacao").val();
    var IdAcomodacao = $("#hdnIdAcomodacao").val();
    var hdncodExterno_Acomodacao = $("#hdncodExterno_Acomodacao").val();
    
    url += '/codexternoacomodacao/' + hdncodExterno_Acomodacao + '/situacaodestino/' + situacaoDestino;

    if (postAPI(url, hdntkn_operacional)) {}
    
    /// ATUALIZA OS DADOS EM TELA
    if ($("#hdnEncaminhaDireto").val() != 'S') {

        url = $("#hdnurl_getAcomodacao").val();
        url += IdAcomodacao;
        getAcomodacao(url, hdntkn_administrativo, '');
        CarregaHistory();

    }
   
}

function EncaminharAtividade() {
    $(".input-file").hide();
    //clearInterval(IntervaloAtualizacao);
    var hdntkn_operacional = $("#hdntkn_operacional").val();
    var hdntkn_administrativo = $("#hdntkn_administrativo").val();
    var url = $("#hdnurl_EncaminharAtividade").val();
    var url_GerarAtividade = $("#hdnurl_GerarAtividade").val();
    var IdAcomodacao = $("#hdnIdAcomodacao").val();
    var hdnIdAtividadeAcomodacao = $("#hdnIdAtividadeAcomodacao").val();
    
    var ids = [];
    url += 'IdAtividadeAcomodacao=' + hdnIdAtividadeAcomodacao;
    $('input[id="idChkEncaminharAtividade"]:checked').each(function () {
        ids.push(this.value);
    });

    if (ids.length <= 0) {
        emite_msg('Escolha uma atividade.');
    }
    else {
        
        if (hdnIdAtividadeAcomodacao != '' && hdnIdAtividadeAcomodacao != '0') {
            for (i = 0; i <= ids.length - 1; i++) {              
                //postAPI(url + '&LstTipoAtividadeToSave=' + ids[i], hdntkn_operacional)
                url += '&LstTipoAtividadeToSave=' + ids[i];
            }
            postAPI(url, hdntkn_operacional);
        }
        else {
            for (i = 0; i <= ids.length - 1; i++) {               
                var d = new Date();
                var ano = d.getFullYear();
                var dia = trataZero(d.getDate(), 2);
                var mes = trataZero(d.getMonth() + 1, 2);
                var hora = trataZero(d.getHours(), 2);
                var minuto = trataZero(d.getMinutes(), 2);
                var segundo = trataZero(d.getSeconds(), 2);

                var dados = {                                       
                    id_AtividadeAcomodacao: 0,
                    id_SituacaoAcomodacao: $("#hdnIdSituacaoAcomodacao").val(),
                    id_TipoSituacaoAcomodacao: $("#hdnIdTipoSituacaoAcomodacao").val(),
                    id_TipoAtividadeAcomodacao: ids[i],
                    dt_InicioAtividadeAcomodacao: ano + '-' + mes + '-' + dia + 'T' + hora + ':' + minuto + ':' + segundo + '.' + d.getMilliseconds(),
                    dt_FimAtividadeAcomodacao: "",
                    id_UsuarioSolicitante: $("#hdnIdUsuario").val(),
                    cod_Prioritario: "N",
                    cod_Plus: "N"                
                }
                var myJSON = JSON.stringify(dados);
                //alert(myJSON);
                postAPIJson(url_GerarAtividade, hdntkn_operacional, dados);
            }
        }

        $("#idDivMsgEncaminhamento").hide();
        /// ATUALIZA OS DADOS EM TELA
        var hdnEncaminhaDireto = $("#hdnEncaminhaDireto").val();

        if (hdnEncaminhaDireto != 'S') {
            url = $("#hdnurl_getAcomodacao").val();
            url += IdAcomodacao;
            getAcomodacao(url, hdntkn_administrativo, '');
            CarregaHistory();
            $(".box").attr("cursor", "default");
        }
    }
}

function postAPI(caminho_url, token) {
    var objResult;
    var jsonResult = $.ajax({
        url: caminho_url,
        method: "POST",
        async: false,
        headers: {
            "Authorization": "Bearer " + token
        },
        success: function (data) {
            objResult = data;
            //if (objResult == "OK") {
                //emite_msg('Ação realizada com sucesso');
            //}
            return true;
        },
        statusCode: {
            404: function () {
                var err = statusCode + ", " + error;
                emite_msg(err);
            }
        }
    })
        .done(function (data) {
            if (data) {
                //CarregaDadosAcomodacao(data);
            }
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            emite_msg(err);
        })
        .always(function () {
            console.log("complete");
        });

}

function postAPIJson(caminho_url, token, dados) {
    var objResult;
    var jsonResult = $.ajax({
        url: caminho_url,
        method: "POST",
        data: JSON.stringify(dados),
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: {
            "Authorization": "Bearer " + token
        },
        success: function (data) {
            objResult = data;
            emite_msg('Ação realizada com sucesso');
            return true;
        },
        statusCode: {
            404: function () {
                var err = statusCode + ", " + error;
                emite_msg(err);
            }
        }
    })
        .done(function (data) {
            if (data) {
                //CarregaDadosAcomodacao(data);
            }
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            emite_msg(err);
        })
        .always(function () {
            console.log("complete");
        });

}

function CarregaHistory() {
    var hdntkn_operacional = $("#hdntkn_operacional").val();
    var IdSituacaoAcomodacao = $("#hdnIdSituacaoAcomodacao").val();
    var url = $("#hdnurl_getHistoricoAcomodacao").val();
    if (IdSituacaoAcomodacao != '') {
        url += IdSituacaoAcomodacao;
        var data = getHistoricoAcomodacao(url, hdntkn_operacional);
    }
}


function CarregaMensagem(IdAtividadeAcomodacao, idUsuarioDest) {

    $("#hdnIdAtividadeMsg").val(IdAtividadeAcomodacao);
    var hdntkn_operacional = $("#hdntkn_operacional").val();
    var url = $("#hdnurl_getMensagens").val();
    $("#idDivMensagem").show();
    $("#hdnIdUsuarioDest").val(idUsuarioDest); //alterar funcao que carregahistorico para passar como parametro o id do usuario destinatario.
    //$("#hdnIdUsuarioDest").val('21');
    if (IdAtividadeAcomodacao != '') {
        url += IdAtividadeAcomodacao;
        //alert(url);
        var data = getMensagens(url, hdntkn_operacional);
    }

}


function CarregaDadosAcomodacao(data, TipoAtividade) {

    //debugger;
    if (TipoAtividade == null) {
        TipoAtividade = '';
    }

    if (data.length > 0) {

        $.each(data, function (key, val) {

            if (val.now != '') {
                //if (TipoAtividade == val.id_TipoAtividadeAcomodacao | TipoAtividade == '' | TipoAtividade == '0' | val.id_TipoAtividadeAcomodacao == '0') {

                $("#lblModalAcomodacao").text(trataretorno(val.nome_Acomodacao));
                $("#hdnIdAcomodacao").val(trataretorno(val.id_Acomodacao));
                $("#hdncodExterno_Acomodacao").val(trataretorno(val.codExterno_Acomodacao));
                $("#lblModalSetor").text(trataretorno(val.nome_Setor));
                $("#lblModalPaciente").text(trataretorno(val.nome_Paciente));
                if (trataretorno(val.generoPaciente) != '') {
                    if (trataretorno(val.generoPaciente) == 'F') {
                        $("#lblModalGenero").text('Feminino');
                    }
                    else {
                        $("#lblModalGenero").text('Masculino');
                    }                    
                }                
                $("#lblModalIdade").text(idadePaciente(val.dt_NascimentoPaciente));
                $("#lblModalSituacao").text(trataretorno(val.nome_TipoSituacaoAcomodacao));
                $("#hdnIdSituacaoAcomodacao").val(trataretorno(val.id_SituacaoAcomodacao));
                $("#hdnIdTipoSituacaoAcomodacao").val(trataretorno(val.id_TipoSituacaoAcomodacao));
                $("#lblModalAtividade").text(trataretorno(val.nome_TipoAtividadeAcomodacao));
                $("#hdnIdAtividadeAcomodacao").val(trataretorno(val.id_AtividadeAcomodacao));
                $("#hdnIdTipoAtividadeAcomodacao").val(trataretorno(val.id_TipoAtividadeAcomodacao));

                var hoje = new Date();
                if (trataretorno(val.dt_FimAcaoAtividade) == '') {
                    hoje = new Date();
                }
                else {
                    hoje = new Date(trataretorno(val.dt_FimAcaoAtividade));
                }
                var inicio = new Date();
                var pTempoExecucao = 0;
                if (trataretorno(val.dt_InicioAcaoAtividade) != '') {
                    inicio = new Date(trataretorno(val.dt_InicioAcaoAtividade));
                    pTempoExecucao = difMinutos(inicio, hoje);
                }
                else {
                    pTempoExecucao = 0;
                }
                $("#imgModalSLA").attr("src", defineImagemSLAHist(trataretorno(val.dt_InicioAcaoAtividade), trataretorno(val.dt_FimAcaoAtividade), trataretorno(val.slaAtividade)));

                $("#lblModalTempoExecucaoSLA").text(pTempoExecucao);
                $("#imgModalMeta").attr("src", "../images/icon_modal_target.png");
                
                $("#lblModalTempoExecucao").text(trataretorno(val.slaAtividade));                
                $("#lblModalAcaoAtividade").text(trataretorno(val.nome_Status));
                $("#lblModalInicioAcao").text(trataretorno(val.dt_InicioAcaoAtividade));
                $("#lblModalTipoAcomodacao").text(trataretorno(val.nome_TipoAcomodacao));

                var HoraInicio = '';
                if (trataretorno(val.dt_InicioAcaoAtividade) != '') {
                    //HoraInicio = retornaHoraMinuto(val.dt_InicioAcaoAtividade);
                    HoraInicio = formataData(val.dt_InicioAcaoAtividade, 'H')
                }

                $("#lblInicioAcao").text(HoraInicio);

                $("#hdncod_Isolamento").val(trataretorno(val.cod_Isolamento));
                $("#hdnprioridadeAtividade").val(trataretorno(val.prioridadeAtividade));
                $("#hdncod_Plus").val(trataretorno(val.cod_Plus));


                $("#imgLimpezaPlus").attr("src", defineImagemLimpezaPlus(trataretorno(val.cod_Plus), trataretorno(val.id_TipoAtividadeAcomodacao)));

                if (trataretorno(val.id_TipoAtividadeAcomodacao) == '2') // HIGIENIZACAO
                {
                    $('#linkLimpezaPlus').removeClass('disabled')
                    $("#linkLimpezaPlus").removeAttr("disabled");
                }
                else {
                    $("#linkLimpezaPlus").attr("disabled", "disabled");
                    $('#linkLimpezaPlus').addClass('disabled')
                }

                $("#imgIsolar").attr("src", defineImagemIsolamento(trataretorno(val.cod_Isolamento)));
                $("#imgPriorizar").attr("src", defineImagemPrioridade(trataretorno(val.prioridadeAtividade), trataretorno(val.id_TipoAtividadeAcomodacao)));


                if (trataretorno(val.id_TipoAtividadeAcomodacao) == '' | trataretorno(val.id_TipoAtividadeAcomodacao) == '0') {
                    $("#imgPriorizar").attr("disabled", "disabled");
                    $('#imgPriorizar').addClass('disabled')
                }
                else {
                    $('#imgPriorizar').removeClass('disabled')
                    $("#imgPriorizar").removeAttr("disabled");
                }

                if (trataretorno(val.pendenciaFinanceira) == 'S') {
                    $("#lblModalPendencia").text('Sim');
                }
                else {
                    $("#lblModalPendencia").text('Não');
                }



                //}
            }
        });
    }
}

function trataretorno(pValor) {
    if (pValor == null)
        return '';
    else
        return pValor;
}

function trataretornoHtml(pValor) {
    if (pValor == null)
        return '&nbsp;';
    else
        return pValor;
}

function idadePaciente(pDataNascimento) {
    if (pDataNascimento == null)
        return '';
    else
        return calculaidade(pDataNascimento) + ' anos';
}

function calculaidade(pDataNascimento) {

    var dataNascimento = new Date(pDataNascimento);
    var hoje = new Date();
    var anos = hoje.getFullYear() - dataNascimento.getFullYear();
    return anos;
}

function defineImagemPrioridade(pPrioridade, pTipoAtividade) {
    if (pTipoAtividade == '') // 2
    {
        return '../images/icon_modal_bell_off.png';
    }
    else {
        if (pPrioridade == 'S')
            return '../images/icon_modal_bell_on.png';
        else
            return '../images/icon_modal_bell.png';
    }
}

function defineImagemLimpezaPlus(pLimpeza, pTipoAtividade) {
    if (pTipoAtividade != '2') // 2
    {
        return '../images/icon_modal_plus_off.png';
    }
    else 
        if (pLimpeza == 'S')
            return '../images/icon_modal_plus_on.png';
        else
            return '../images/icon_modal_plus.png';
}

function defineImagemIsolamento(pIsolamento) {
    if (pIsolamento == 'S')
        return '../images/icon_modal_alert_on.png';
    else
        return '../images/icon_modal_alert.png';
}

function formataData(pData, pTipo) {
    // PDpDataFormato: 2019-06-21T13:01:35
    // pTipo = 'C' - Completa (Data e Hora)
    // pTipo = 'D' - Somente a Data
    // pTipo = 'H' - Somente a Hora
    // alert(pData);
    var pRetorno = '';
    if (pData != '' && pData != '&nbsp;') {
        var Ano = pData.substring(0, 4);
        var Mes = pData.substring(5, 7);
        var Dia = pData.substring(8, 10);
        var Hora = pData.substring(11, 13);
        var Minuto = pData.substring(14, 16);
        switch (pTipo) {
            case 'C':
                pRetorno = Dia + '/' + Mes + '/' + Ano + ' ' + Hora + ':' + Minuto;
                break;
            case 'D':
                pRetorno = Dia + '/' + Mes + '/' + Ano;
                break;
            case 'H':
                pRetorno = Hora + ':' + Minuto;
                break;
            default:
                pRetorno = '';
                break;
        }
    }
    else {
        pRetorno = pData;
    }
    return pRetorno;
}

function defineImagemSLA(pDataInicioAcao, pTempoMinutosSLA)
{
    var strImagem = "";

    if (pDataInicioAcao == '') {
        strImagem = "../images/clock_green.png";
        return strImagem;
    }

    var decPercentSLA = 0;
    var pPercentualAtencao = $('#hdnpercentualAtencaoSLA').val();
    var hoje = new Date();
    var inicio = new Date(pDataInicioAcao);
    var pTempoExecucao = difMinutos(inicio, hoje);
    try {
        
        decPercentSLA = pTempoMinutosSLA - ((pTempoMinutosSLA / pPercentualAtencao));
        if (pTempoMinutosSLA <= 0) {
            strImagem = "../images/clock_green.png";
        }
        else
            if (pTempoExecucao > pTempoMinutosSLA) {
                strImagem = "../images/clock_red.png";
            }
            else {
                if (pTempoExecucao < decPercentSLA) {
                    strImagem = "../images/clock_green.png";
                }
                else {
                    strImagem = "../images/clock_yelow.png";
                }
            }

        return strImagem;

    }
    catch (ex)
    {
        throw ex;
    }
}

function defineImagemSLAHist(pDataInicioAcao, pDataFimAcao, pTempoMinutosSLA) {
    var strImagem = "";
    
    if (pDataInicioAcao == '') {
        strImagem = "../images/clock_green.png";
        return strImagem;
    }
    var hoje = new Date();
    if (pDataFimAcao == '') {
        hoje = new Date();
    }
    else {
        hoje = new Date(pDataFimAcao); 
    }
    var decPercentSLA = 0;
    var pPercentualAtencao = $('#hdnpercentualAtencaoSLA').val();
    var inicio = new Date(pDataInicioAcao);
    var pTempoExecucao = difMinutos(inicio, hoje);
    try {

        decPercentSLA = pTempoMinutosSLA - ((pTempoMinutosSLA / pPercentualAtencao));
        if (pTempoMinutosSLA <= 0) {
            strImagem = "../images/clock_green.png";
        }
        else
            if (pTempoExecucao > pTempoMinutosSLA) {
                strImagem = "../images/clock_red.png";
            }
            else {
                if (pTempoExecucao < decPercentSLA) {
                    strImagem = "../images/clock_green.png";
                }
                else {
                    strImagem = "../images/clock_yelow.png";
                }
            }

        return strImagem;

    }
    catch (ex) {
        throw ex;
    }


}

function defineImagemFinalizada(pDataFimTotal, pDataFimParcial) {
    var strImagem = "";

    if (pDataFimTotal == '' && pDataFimParcial == '') {
        strImagem = "";
        return strImagem;
    }
    else {
        if (pDataFimTotal != '') {
            strImagem = "../images/icon_finalizado_total.png";
        }
        else {
            if (pDataFimParcial != '') {
                strImagem = "../images/icon_finalizado_parcial.png";
            }
        }
        return strImagem;
    }
}


function difMinutos(dt1, dt2) {
    var diff = (dt2.getTime() - dt1.getTime()) / (1000 * 60);
    //diff /= (60);
    return Math.abs(Math.round(diff));
}

function retornaHoraMinuto(pData) {
    var strHora = '';
    var dataConvertida = new Date(pData);
    strHora = dataConvertida.getHours() + ':' + dataConvertida.getMinutes();
    return strHora;
}

function trataZero(valor, tamanho) {
    var strRet = '0' + valor;
    if (strRet.length > tamanho) {
        strRet = strRet.substring(strRet.length - tamanho, strRet.length)
    }
    return strRet;
}


function FiltrarAtividade(url) {
    $(".input-file").hide();
    $(".loader").show();
    var hdnIdTipoAtividadeAcomodacao = $("#hdnIdTipoAtividadeAcomodacaoFiltro").val();
    var rdnFiltroSetor = [];
    var rdnFiltroTipoAtividade = [];
    var SetorSel = '';
    
    $('input[id="rdnFiltroSetor"]:checked').each(function () {
        rdnFiltroSetor.push(this.value);
    });
    
    if (rdnFiltroSetor.length > 0) {
        SetorSel = rdnFiltroSetor[0];
    }
    
    $('input[name="rdnFiltroTipoAtividade"]:checked').each(function () {
        rdnFiltroTipoAtividade.push(this.value);
    });

    if (rdnFiltroTipoAtividade.length > 0) {
        hdnIdTipoAtividadeAcomodacao = rdnFiltroTipoAtividade[0];
    }
    else {
        if (SetorSel == '') {
            emite_msg('Por favor, selecione um dos critérios de filtro.');
            $(".loader").hide();
            return;
        }
    }

    url += '?id_tipoatividadeacomodacao=' + hdnIdTipoAtividadeAcomodacao;    
    if (SetorSel != '') {
        url += '&pId_Setor=' + SetorSel;    
    }
    window.location.href = url;
    $(".loader").hide();
};



function FiltrarSituacao(url) {
    $(".input-file").hide();
    $(".loader").show();
    var hdnIdTipoSituacaoAcomodacao = $("#hdnIdTipoSituacaoAcomodacaoFiltro").val();
    var rdnFiltroSetor = [];
    var rdnFiltroTipoAtividade = [];
    var SetorSel = '';
    var AtividadeSel = '';

    $('input[id="rdnFiltroSetor"]:checked').each(function () {
        rdnFiltroSetor.push(this.value);
    });

    if (rdnFiltroSetor.length > 0) {
        SetorSel = rdnFiltroSetor[0];
    }

    $('input[name="rdnFiltroTipoAtividade"]:checked').each(function () {
        rdnFiltroTipoAtividade.push(this.value);
    });

    if (rdnFiltroTipoAtividade.length > 0) {
        AtividadeSel = rdnFiltroTipoAtividade[0];
    }
    else {
        if (SetorSel == '') {
            emite_msg('Por favor, selecione um dos critérios de filtro.');
            $(".loader").hide();
            return;
        }
    }

    url += '?id_tiposituacaoacomodacao=' + hdnIdTipoSituacaoAcomodacao;
    if (SetorSel != '') {
        url += '&pId_Setor=' + SetorSel;
    }
    if (AtividadeSel != '') {
        url += '&id_tipoatividadeacomodacao=' + AtividadeSel;
    }
    window.location.href = url;
    $(".loader").hide();
};


function FiltrarSituacaoAtividade(url) {
    $(".input-file").hide();
    $(".loader").show();
    var hdnIdTipoAtividadeAcomodacao = $("#hdnIdTipoAtividadeAcomodacaoFiltro").val();
    var hdnIdTipoSituacaoAcomodacao = $("#hdnIdTipoSituacaoAcomodacaoFiltro").val();
    var rdnFiltroSetor = [];
    var rdnFiltroTipoAtividade = [];
    var SetorSel = '';

    $('input[id="rdnFiltroSetor"]:checked').each(function () {
        rdnFiltroSetor.push(this.value);
    });

    if (rdnFiltroSetor.length > 0) {
        SetorSel = rdnFiltroSetor[0];
    }

    $('input[name="rdnFiltroTipoAtividade"]:checked').each(function () {
        rdnFiltroTipoAtividade.push(this.value);
    });

    if (rdnFiltroTipoAtividade.length > 0) {
        hdnIdTipoAtividadeAcomodacao = rdnFiltroTipoAtividade[0];
    }
    else {
        if (SetorSel == '') {
            emite_msg('Por favor, selecione um dos critérios de filtro.');
            $(".loader").hide();
            return;
        }
    }
    //alert('hdnIdTipoSituacaoAcomodacao:' + hdnIdTipoSituacaoAcomodacao)
    url += '?id_tiposituacaoacomodacao=' + hdnIdTipoSituacaoAcomodacao;
    url += '&id_tipoatividadeacomodacao=' + hdnIdTipoAtividadeAcomodacao;
    if (SetorSel != '') {
        url += '&pId_Setor=' + SetorSel;
    }
    window.location.href = url;
    $(".loader").hide();
};

function ValidarOperacaoUsuario(PerfilUsuario, PerfilAdministrador, codAcesso){

    var PerfilUsuario = $("#hdnPerfilUsuario").val();
    var PerfilAdministrador = $("#hdnPerfilAdministrador").val();
    var codAcesso = $("#hdnCodAcesso").val();
  
    if (PerfilAdministrador == "G") {
        return true;
    }
    if (codAcesso != "E") {
            var err = "Operação não autorizada para este usuário.";
            emite_msg(err);
            return false;
        }
    

    return true;
}


function CarregaComboItemCheckList(id_CheckList) {
    

    var token = $("#hdntkn_configuracao").val();
    var caminho_url = $("#hdnurl_listaCheckList").val() + id_CheckList;
    //alert(caminho_url);
    var txthtml = '';
    var objResult;
    var jsonResult = $.ajax({
        url: caminho_url,
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
        async: false,
        success: function (data) {
            objResult = data;
        },
        statusCode: {
            404: function () {
                var err = statusCode + ", " + error;
                emite_msg(err);
            }
        }
    })
        .done(function (data) {
            if (data) {
                $('#selItemCheckList').find('option').remove();
                $('#selItemCheckList').append('<option value="">SELECIONE</option>');
                $.each(data, function (key, val) {

                    if (val.now != '') {
                        $('#selItemCheckList').append('<option value="' + val.id_ItemChecklist + '">' + val.nome_ItemChecklist + '</option>');
                    }
                });

            }
        })
        .fail(function (jqxhr, textStatus, error) {
            var err = textStatus + ", " + error;
            emite_msg(err);
        })
        .always(function () {
            console.log("complete");
        });


}