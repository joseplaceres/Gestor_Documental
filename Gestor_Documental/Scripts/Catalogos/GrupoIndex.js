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
    $("#btnEliminar").click(function () {
        Eliminar();
    });
});

// Identificador del registro actual.
var idActual = 0;

// Inicia el proceso de "eliminar", muestra una ventana modal para confirmar la operación.
function IniciarEliminar(id) {
    idActual = id;
    $("#mdlEliminar").modal("show");
}

function Eliminar() {
    $("#mdlEliminar").modal("hide");
    if (idActual > 0) {
        var datos = { id: idActual };
        $.post(rutaPost, datos, function (data) {
            var json = $.parseJSON(data);
            if (json.Eliminado == 1) {
                window.location.reload(true);
            } else {
                if (json.Error == "Acceso Denegado") {
                    window.location = "/Autenticacion/Denegado/";
                } else if (json.Error != "") {
                    var mensajeError = "<i class='icon-chevron-right'></i> " + json.Error + "<br/>";
                    $("#errorDetalle").html(mensajeError);
                    $("#mdlError").modal("show");
                }
            }
        });
    }
}

function MostrarMensaje(mensaje) {
    if (mensaje != "") {
        $("#errorDetalle").html(mensajeError);
        $("#mdlError").modal("show");
    }
}
