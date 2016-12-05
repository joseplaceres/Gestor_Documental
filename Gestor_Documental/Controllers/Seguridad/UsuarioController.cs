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
using Gestor_Documental.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Gestor_Documental.Controllers.Seguridad
{
    [AutorizarUsuario]
    public class UsuarioController : Controller
    {
        /// <summary>
        /// Indice de usuarios.
        /// </summary>
        /// <returns></returns>
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Index()
        {
            var pu = new PerUsuario();
            List<Usuario> lst = pu.ObtenerActivos();

            if (lst != null)
            {
                lst = lst.OrderBy(o => o.Alias).ToList();
            }

            return View(lst);
        }

        /// <summary>
        /// Nuevo usuario.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ViewResult Nuevo()
        {
            ViewBag.Grupo = new PerGrupo().ObtenerTodos();
            ViewBag.GrupoLector = new PerGrupoLector().ObtenerTodos();
            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            ViewBag.Area = null;

            var usr = new Usuario();
            return View(usr);
        }

        /// <summary>
        /// Insertar un nuevo usuario.
        /// </summary>
        /// <param name="usr"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Nuevo(Usuario usr)
        {
            ViewBag.Grupo = new PerGrupo().ObtenerTodos();
            ViewBag.GrupoLector = new PerGrupoLector().ObtenerTodos();

            ActionResult ar = RedirectToAction("Index");
            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Alias"] = "";
            job["Nombre"] = "";

            if (usr != null)
            {
                var pu = new PerUsuario();
                List<Usuario> lst = pu.ObtenerActivos(0, usr.Alias.Trim(), usr.Nombre.Trim());

                if (lst.Count() > 0)
                {
                    foreach (var usuario in lst)
                    {
                        if (usuario.Alias.ToUpper() == usr.Alias.ToUpper().Trim())
                        {
                            job["Alias"] += "El alias \"" + usr.Alias.Trim() + "\" ya existe. ";
                        }

                        if (usuario.Nombre.ToUpper() == usr.Nombre.ToUpper().Trim())
                        {
                            job["Nombre"] += "El nombre \"" + usr.Nombre.Trim() + "\" ya existe. ";
                        }
                    }

                    ViewBag.validacionDeDatos = job;
                    ar = View(usr);
                }
                else
                {
                    try
                    {
                        pu.Insertar(usr);
                        job["Error"] = false;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        job["Nombre"] += "Error al guardar el registro: " + ex.Message;
                        if (ex.InnerException != null)
                        {
                            job["Nombre"] += " " + ex.InnerException.Message;
                        }

                        ViewBag.validacionDeDatos = job;
                        ar = View(usr);
                    }
                }
            }

            return ar;
        }

        /// <summary>
        /// Editar usuario.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ViewResult Editar(int id)
        {
            ViewBag.Grupo = new PerGrupo().ObtenerTodos();
            ViewBag.GrupoLector = new PerGrupoLector().ObtenerTodos();
            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();

            Usuario usr = new PerUsuario().Obtener(id);
            ViewBag.Area = new PerArea().ObtenerActivasDepartamento(0, usr.IdDepartamento.GetValueOrDefault());
            
            return View(usr);
        }

        /// <summary>
        /// Guardar los cambios a un usuario.
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Editar(Usuario usr, bool changePassword)
        {
            ViewBag.Grupo = new PerGrupo().ObtenerTodos();
            ViewBag.GrupoLector = new PerGrupoLector().ObtenerTodos();

            ActionResult ar = RedirectToAction("Index");
            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = this.GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Nombre"] = "";
            job["Clave"] = "";

            if (usr != null)
            {
                var pu = new PerUsuario();
                List<Usuario> lst = pu.ObtenerActivos(usr.IdUsuario, usr.Alias.Trim(), usr.Nombre.Trim());

                if (lst.Count() > 0)
                {
                    foreach (var usuario in lst)
                    {
                        if (usuario.Alias.ToUpper() == usr.Alias.ToUpper().Trim())
                        {
                            job["Alias"] += "El alias \"" + usr.Alias.Trim() + "\" ya existe. ";
                        }

                        if (usuario.Nombre.ToUpper() == usr.Nombre.ToUpper().Trim())
                        {
                            job["Nombre"] += "El nombre \"" + usr.Nombre.Trim() + "\" ya existe. ";
                        }
                    }

                    ViewBag.validacionDeDatos = job;
                    ar = View(usr);
                }
                else
                {
                    try
                    {
                        pu.Editar(usr, changePassword);
                        job["Error"] = false;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        job["Nombre"] += "Error al guardar el registro: " + ex.Message;
                        if (ex.InnerException != null)
                        {
                            job["Nombre"] += " " + ex.InnerException.Message;
                        }

                        ViewBag.validacionDeDatos = job;
                        ar = View(usr);
                    }
                }
            }

            return ar;
        }

        /// <summary>
        /// Eliminar (eliminado lógico) un usuario.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Eliminar(int id)
        {
            var str = new StringBuilder();
            var error = "";
            str.Append("{");
            str.Append("\"Eliminado\":");
            try
            {
                if (id > 0)
                {
                    new PerUsuario().Eliminar(id);
                    str.Append("1");
                }
                else
                {
                    str.Append("0");
                }
            }
            catch (Exception ex)
            {
                error += "Error al eliminar el usuario: " + ex.Message;
                Debug.WriteLine(ex.ToString());
                str.Append("0");
            }
            str.Append(",");
            str.Append("\"Error\":");
            str.Append("\"" + error + "\"");
            str.Append("}");

            return Json(str.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}
