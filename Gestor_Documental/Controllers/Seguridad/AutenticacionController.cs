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

using System;
using System.Diagnostics;
using System.Web.Mvc;
using Gestor_Documental.Models.Entidades;
using Gestor_Documental.Models.Persistencias;
using Gestor_Documental.Util;
using System.Web.Security;
using WebMatrix.WebData;
using System.Text;

namespace Gestor_Documental.Controllers.Seguridad
{
    public class AutenticacionController : Controller
    {
        /// <summary>
        /// Inicio de sesión.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Usuario usr = new Usuario();
            return View(usr);
        }

        /// <summary>
        /// Validación de usuario.
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(Usuario usr, string returnUrl)
        {
            var pu = new PerUsuario();
            Usuario usuario = pu.Obtener(usr.Alias);

            if (pu.Validar(usr))
            {
                FormsAuthentication.SetAuthCookie(usuario.Nombre, false);
                Sesion.Current.IdUsuario = usuario.IdUsuario;

                if (!String.IsNullOrWhiteSpace(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Inicio");
                }
            }
            else
            {
                return View(usr);
            }
        }

        /// <summary>
        /// Cerrar la sesión, limpiar la caché y regresar a inicio de sesión.
        /// </summary>
        /// <returns></returns>
        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Sesion.Current.IdUsuario = 0;
            Sesion.Current.Abandon();
            return RedirectToAction("index", "Autenticacion");
        }

        /// <summary>
        /// Consultar si hay una sesión iniciada.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerificarSesion() 
        {
            Debug.WriteLine("VerificarSesion");
            var str = new StringBuilder();
            str.Append("{");
            str.Append("\"Abierta\":");
            string sesionIniciada = UtilSeguridad.SesionIniciada().ToString().ToLower();
            str.Append(sesionIniciada);
            str.Append("}");

            return Json(str.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ir a la vista de acción denegada.
        /// </summary>
        /// <returns></returns>
        public ActionResult Denegado()
        {
            return View();
        }

        /// <summary>
        /// Manda el json con el error Acceso Denegado, ya que desde el atributo no se puede mandar.
        /// </summary>
        /// <returns></returns>
        public ActionResult DenegadoJson() 
        {
            var str = "{\"Eliminado\":0,\"Error\":\"Acceso Denegado\"}";
            return Json(str, JsonRequestBehavior.AllowGet);
        }
    }
}
