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

using Gestor_Documental.Models.Entidades;
using Gestor_Documental.Models.Persistencias;
using System;
using System.Web;

namespace Gestor_Documental.Util
{
    /// <summary>
    /// Clase que encapsula la sesión.
    /// </summary>
    [Serializable]
    public class Sesion
    {
        /// <summary>
        /// Constructor privado.
        /// </summary>
        private Sesion()
        {
            IdUsuario = 0;
        }

        public int IdUsuario { get; set; }

        /// <summary>
        /// Obtiene el objeto Usuario correspondiente al usuario actual.
        /// </summary>
        public Usuario Usuario
        {
            get
            {
                return new PerUsuario().Obtener(IdUsuario);
            }
        }

        /// <summary>
        /// Abandona la sesión actual.
        /// </summary>
        public void Abandon()
        {
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// Obtener la sesión actual.
        /// </summary>
        public static Sesion Current
        {
            get
            {
                Sesion sesion = (Sesion)HttpContext.Current.Session["Sesion"];

                if (sesion == null)
                {
                    sesion = new Sesion();
                    HttpContext.Current.Session["Sesion"] = sesion;
                }

                return sesion;
            }
        }
    }
}
