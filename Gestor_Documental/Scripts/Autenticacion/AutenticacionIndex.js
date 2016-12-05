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
    $("#btnLogIn").click(function () {
        if (ValidarInformacion()) {
            $("#frmLogIn").submit();
        }
    });

    $("#Alias").keypress(function (event) {
        runScript(event);
    });

    $("#Password").keypress(function (event) {
        runScript(event);
    });
});

function runScript(event) {
    if (event.keyCode == 13) {
        if (ValidarInformacion()) {
            $("#frmLogIn").submit();
        }
    }
}

// Realiza la validacion de si los datos estan correctamente cargados.
function ValidarInformacion() {
    // Guarda los mensajes de error.
    var mensajeError = "";

    if ($("#Alias").val() == "") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo Alias es obligatorio<br/>";
    }

    if ($("#Password").val() == "") {
        mensajeError += "<i class='icon-chevron-right'></i> El campo Password es obligatorio<br/>";
    }

    if (mensajeError != "") {
        $("#errorDetalle").html(mensajeError);
        $("#mdlError").modal("show");
        return false;
    } else {
        return true;
    }
}
