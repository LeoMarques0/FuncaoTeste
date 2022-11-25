$(document).ready(function () {
    $('#incluirBeneficiario').click(function () {

        var cpfNumeros = $("#CPFBeneficiario").val().toString().replace(/\D/g, "");
        if (!testCpf(cpfNumeros)) {
            ModalDialog("Ocorreu um erro", "CPF Invalido");
        }
        else if (!$("#NomeBeneficiario").val()) {
            ModalDialog("Ocorreu um erro", "Insira um Nome");
        }
        else {
            $.ajax({
                url: urlBeneficiarioIncluir,
                method: "POST",
                data: {
                    "NOME": $("#NomeBeneficiario").val(),
                    "CPF": $("#CPFBeneficiario").val()
                },
                error:
                    function (r) {
                        if (r.status == 400)
                            ModalDialog("Ocorreu um erro", r.responseJSON);
                        else if (r.status == 500)
                            ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                    },
                success:
                    function (r) {
                        UpdateList(r);
                        $('#CPFBeneficiario').val("");
                        $('#NomeBeneficiario').val("");
                    }
            })
        }    
    })

    $('#alterarBeneficiario').click(function () {
        $('#IncluirBtn').show();
        $('#AlterarBtn').hide();

        Alterar($("#NomeBeneficiario").val(), $("#CPFBeneficiario").val(), indexAlterar);

        $('#CPFBeneficiario').val("");
        $('#NomeBeneficiario').val("");
    })
})

var modal = document.getElementById("beneficiario");
var indexAlterar = 0;

function LoadBeneficiadores() {
    if (document.getElementById("gridBeneficiarios"))

        $.ajax({
            url: urlBeneficiarioList,
            method: "POST",
            error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
            success:
                function (r) {
                    UpdateList(r);
                }
        })
}

function AlterarGet(nome, cpf, i) {
    $('#IncluirBtn').hide();
    $('#AlterarBtn').show();

    $('#CPFBeneficiario').val(cpf);
    $('#NomeBeneficiario').val(nome);
    indexAlterar = i;
}

function Alterar(nome, cpf, i) {
    $.ajax({
        url: urlAlteracao,
        method: "POST",
        data: {
            "NOME": nome,
            "CPF": cpf,
            "index": i
        },
        error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
        success:
            function (r) {
                UpdateList(r);
            }
    })
}

function Cancelar() {
    $('#IncluirBtn').show();
    $('#AlterarBtn').hide();

    $('#CPFBeneficiario').val("");
    $('#NomeBeneficiario').val("");
    indexAlterar = 0;
}

function Deletar(i) {
    Cancelar();
    $.ajax({
        url: urlDeletar,
        method: "DELETE",
        data: {
            "index": i
        },
        error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
        success:
            function (r) {
                UpdateList(r);
            }
    })
}

function UpdateList(r) {
    var table = $('#gridBeneficiarios');
    var texto = '<tr>' +
        '           <td> <label for="CPF">CPF</label></td>' +
        '           <td><label for="Nome">Nome</label></td>' +
        '           <td></td>' +
        '           <td></td>' +
        '        </tr >';
    for (var i = 0; i < r.length; i++) {
        texto += '<tr>' +
            '        <td>' + r[i].CPF + '</td>' +
            '        <td>' + r[i].Nome + '</td>' +
            '        <td> <button type="button" onclick="AlterarGet(\'' + r[i].Nome + '\',\'' + r[i].CPF + '\',' + i + ')" class="btn btn-sm btn-primary">Alterar</button> </td>' +
            '        <td> <button type="button" onclick="Deletar(' + i + ')" class="btn btn-sm btn-primary">Excluir</button> </td>' +
            '     </tr>'
    }
    table.empty();
    table.append(texto);
}

function OpenModal() {
    LoadBeneficiadores();
    modal.style.display = "block";
}

function CloseModal() {
    modal.style.display = "none";
}

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}