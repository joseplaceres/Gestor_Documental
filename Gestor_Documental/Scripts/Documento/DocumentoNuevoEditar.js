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
        if (ValidarInformacion()) {
            $("#frmDocumento").submit();
        }
    });
});

// Realiza la validación de los datos.
function ValidarGuardado(mensaje) {
    // Guarda los mensajes de error.
    var mensajeError = "";

    if (mensaje) {
        mensajeError += "<i class='icon-chevron-right'></i> " + mensaje + "<br/>";
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

    if ($("#archivo").val() == "") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo Archivo es obligatorio<br/>";
    }

    if ($("#cboxDepartamento").val() == 0) {
        mensajeError += "<i class='icon-chevron-right'></i> El campo Departamento es obligatorio<br/>";
    }

    if ($("#cboxArea").val() == 0) {
        mensajeError += "<i class='icon-chevron-right'></i> El campo Area es obligatorio<br/>";
    }

    if (editar) {
        // Checar que el nombre del nuevo archivo sea igual al anterior.
        var archivo = $("#archivo").val();
        archivo = archivo.split(/(\\|\/)/g).pop();

        if (archivo != archivoAnterior) {
            mensajeError += "<i class='icon-chevron-right'></i> No se le puede cambiar el nombre al archivo.<br/>";
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
