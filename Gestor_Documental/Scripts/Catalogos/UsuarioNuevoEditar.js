/*
 * This file is part of Gestor Documental para la Universidad Autónoma de Guadalajara Campus Tabasco.
 * Copyright (c) 2016, José Manuel Placeres Castro
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

$(function () {
    // Evento change de los combos.
    $("#cboxDepartamento").change(function () {
        CargarAreas();
    });

    $("#btnGuardar").click(function () {
        // Si no hay errores envia el formulario.
        if (ValidarInformacion()) {
            $("#frmUsuario").submit();
        }
    });
});

// Carga áreas en el combo correspondiente de acuerdo al departamento.
function CargarAreas() {
    var datos = {idDepartamento: parseInt($("#cboxDepartamento").val())};

    $.post("/Area/CargarAreas", datos, function (data) {
        var html = "<option value='0'>Seleccione...</option>";

        $.each(data, function (index, item) {
            html += "<option value='" + item.Key + "'>" + item.Value + "</option>";
        });

        $("#cboxArea").html(html);
    });
}

// Muestra mensaje de error.
function MensajeError(alias, nombre) {
    // Guarda los mensajes de error.
    var mensajeError = "";

    if (alias) {
        mensajeError += "<i class='icon-chevron-right'></i> " + alias + "<br/>";
    }

    if (nombre) {
        mensajeError += "<i class='icon-chevron-right'></i> " + nombre + "<br/>";
    }

    if (mensajeError != "") {
        $("#errorDetalle").html(mensajeError);
        $("#mdlError").modal("show");
        return false;
    } else {
        return true;
    }
}

// Realiza la validacion de si los datos estan correctamente cargados, tanto para nuevo como para editar.
function ValidarInformacion() {
    // Guarda los mensajes de error.
    var mensajeError = "";

    if ($.trim($("#Alias").val()) == "") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo alias es obligatorio<br/>";
    }

    if ($.trim($("#Nombre").val()) == "") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo nombre es obligatorio<br/>";
    }

    if ($("#cboxGrupo").val() == "0") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo grupo es obligatorio<br/>";
    }

    if ($("#cboxDepartamento").val() == "0") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo departamento es obligatorio<br/>";
    }

    if ($("#cboxArea").val() == "0") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo area es obligatorio<br/>";
    }

    if ($("#cboxGrupoLector").val() == "0") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo grupo lector es obligatorio<br/>";
    }

    if ($("#changePassword").val() == "true") {
        if ($("#Password").val() == "" || $("#ConfirmPassword").val() == "") {
            mensajeError += "<i class='icon-chevron-right'></i> Debe proporcionar un password<br/>";
        }

        if ($("#Password").val() != $("#ConfirmPassword").val()) {
            mensajeError += "<i class='icon-chevron-right'></i> El password no coincide<br/>";
        }
    }

    if (mensajeError != "") {
        $("#errorDetalle").html(mensajeError);
        $("#mdlError").modal("show");
        return false;
    } else {
        return true;
    }
}
