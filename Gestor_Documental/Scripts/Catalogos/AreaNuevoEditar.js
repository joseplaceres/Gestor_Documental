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
    $("#btnGuardar").click(function () {      
        // Si no hay errores envia el formulario.
        if (ValidarInformacion()) {
            $("#frmArea").submit();
        }
    });
});

// Muestra mensaje de error.
function MensajeError(nombre, clave, departamento) {
    // Guarda los mensajes de error.
    var mensajeError = "";
    
    if (nombre) {
        mensajeError += "<i class='icon-chevron-right'></i> " + nombre + "<br/>";
    }

    if (clave) {
        mensajeError += "<i class='icon-chevron-right'></i> " + clave + "<br/>";
    }

    if (departamento) {
        mensajeError += "<i class='icon-chevron-right'></i> " + departamento + "<br/>";
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

    if ($.trim($('#Nombre').val()) == "") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo nombre es obligatorio<br/>";
    }

    if ($.trim($("#Clave").val()) == "") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo clave es obligatorio<br/>";
    }

    if ($("#cboxDepartamento").val() == "0") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo departamento es obligatorio<br/>";
    }

    if (mensajeError != "") {
        $("#errorDetalle").html(mensajeError);
        $("#mdlError").modal("show");
        return false;
    } else {
        return true;
    }
}
