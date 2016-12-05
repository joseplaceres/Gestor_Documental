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

$(document).ready(function () {
    //VerificarSesion();

    $('.deshabilitado').click(function (e) {
        e.preventDefault();
    });
});

function VerificarSesion() {
    $.post("/Autenticacion/VerificarSesion/", function (data) {
        var json = $.parseJSON(data);
        console.log("json.Abierta: " + json.Abierta);
        if (!json.Abierta) {
            window.location = "/Autenticacion/Index/";
        }
    });
}
