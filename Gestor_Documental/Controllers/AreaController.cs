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

using System.Linq;
using System.Web.Mvc;
using Gestor_Documental.Models.Entidades;
using System.Collections.Generic;
using Gestor_Documental.Models.Persistencias;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Diagnostics;
using Gestor_Documental.Util;

namespace Gestor_Documental.Controllers
{
    [AutorizarUsuario]
    public class AreaController : Controller
    {
        /// <summary>
        /// Indice de áreas.
        /// </summary>
        /// <returns></returns>
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Index()
        {
            List<Area> lst = new PerArea().ObtenerActivas();

            if (lst != null)
            {
                lst = lst.OrderBy(o => o.Nombre).ToList();
            }

            return View(lst);
        }

        /// <summary>
        /// Nueva área.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ViewResult Nuevo()
        {
            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            var area = new Area();
            return View(area);
        }

        /// <summary>
        /// Guardar nueva área.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Nuevo(Area ar)
        {
            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            ActionResult aresult = RedirectToAction("Index");
            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = this.GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Nombre"] = "";
            job["Clave"] = "";
            job["Departamento"] = ""; // Este se pone por paridad con el metodo editar.

            if (ar != null)
            {
                ar.Nombre = ar.Nombre.Trim();
                ar.Clave = ar.Clave.Trim();

                var error = false;
                var pa = new PerArea();

                // Checar si ya hay una área con este nombre en este departamento.
                List<Area> lst = pa.ObtenerActivasDepartamento(0, ar.Nombre, ar.IdDepartamento.GetValueOrDefault());

                if (lst.Count() > 0)
                {
                    error = true;

                    foreach (var area in lst)
                    {
                        if (area.Nombre.ToUpper() == ar.Nombre.ToUpper())
                        {
                            job["Nombre"] += "El área \"" + ar.Nombre + "\" ya existe en este departamento. ";
                        }
                    }
                }

                // Checar si ya hay un área con esta clave en este departamento.
                List<Area> lstClave = pa.ObtenerActivasDepartamentoClave(0, ar.Clave, ar.IdDepartamento.GetValueOrDefault());

                if (lstClave.Count > 0) 
                {
                    error = true;

                    foreach (var area in lstClave)
                    {
                        if (area.Clave.ToUpper() == ar.Clave.ToUpper())
                        {
                            job["Clave"] += "La clave \"" + ar.Clave + "\" ya existe en este departamento. ";
                        }
                    }
                }

                if (error) 
                {
                    ViewBag.validacionDeDatos = job;
                    aresult = View(ar);
                }
                else
                {
                    try
                    {
                        pa.Insertar(ar);
                        job["Error"] = false;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        job["Nombre"] += "Error al guardar el registro: " + ex.Message;
                        if (ex.InnerException != null)
                        {
                            job["Nombre"] += " " + ex.InnerException.Message;
                        }

                        ViewBag.validacionDeDatos = job;
                        aresult = View(ar);
                    }
                }
            }

            return aresult;
        }

        /// <summary>
        /// Editar un área.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Editar(int id)
        {
            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            Area area = new PerArea().Obtener(id);
            return View(area);
        }

        /// <summary>
        /// Guardar los cambios a un área.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Editar(Area ar)
        {
            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            ActionResult aresult = RedirectToAction("Index");
            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = this.GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Nombre"] = "";
            job["Departamento"] = "";

            if (ar != null)
            {
                ar.Nombre = ar.Nombre.Trim();
                ar.Clave = ar.Clave.Trim();

                var error = false;
                var pa = new PerArea();
                
                // Checar si ya hay una área con este nombre en este departamento.
                List<Area> lst = pa.ObtenerActivasDepartamento(ar.IdArea, ar.Nombre.Trim(), ar.IdDepartamento.GetValueOrDefault());

                if (lst.Count() > 0)
                {
                    error = true;
                    foreach (var area in lst)
                    {
                        if (area.Nombre.ToUpper() == ar.Nombre.ToUpper().Trim())
                        {
                            job["Nombre"] += "El área \"" + ar.Nombre.Trim() + "\" ya existe en este departamento. ";
                        }
                    }
                }

                // Checar si ya hay un área con esta clave en este departamento.
                List<Area> lstClave = pa.ObtenerActivasDepartamentoClave(ar.IdArea, ar.Clave, ar.IdDepartamento.GetValueOrDefault());

                if (lstClave.Count > 0)
                {
                    error = true;

                    foreach (var area in lstClave)
                    {
                        if (area.Clave.ToUpper() == ar.Clave.ToUpper())
                        {
                            job["Clave"] += "La clave \"" + ar.Clave + "\" ya existe en este departamento. ";
                        }
                    }
                }

                // Checar si hubo cambio de nombre, y si sí, checar si ya está siendo usada por un documento.
                if (pa.CambioNombre(ar)) 
                {
                    if (pa.EnUsoPorDocumento(ar.IdArea)) 
                    {
                        error = true;
                        job["Nombre"] += "No se le puede cambiar el nombre a un área que ya está siendo usada por uno o más documentos. ";
                    }
                }

                // Checar si hubo cambio de departamento y si sí, checar si ya está siendo usada por un documento.
                if (pa.CambioDepartamento(ar)) 
                {
                    if (pa.EnUsoPorDocumento(ar.IdArea))
                    {
                        error = true;
                        job["Departamento"] += "No se le puede cambiar el departamento a un área que ya está siendo usada por uno o más documentos. ";
                    }
                }

                if (error) 
                {
                    ViewBag.validacionDeDatos = job;
                    aresult = View(ar);
                }
                else
                {
                    try
                    {
                        pa.Editar(ar);
                        job["Error"] = false;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        job["Nombre"] += "Error al guardar el registro: " + ex.Message;
                        if (ex.InnerException != null)
                        {
                            job["Nombre"] += " " + ex.InnerException.Message;
                        }

                        ViewBag.validacionDeDatos = job;
                        aresult = View(ar);
                    }
                }
            }

            return aresult;
        }

        /// <summary>
        /// Eliminar (eliminado lógico) un área.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Eliminar(int id)
        {
            var pa = new PerArea();
            var str = new StringBuilder();
            var error = "";

            str.Append("{");
            str.Append("\"Eliminado\":");
            try
            {
                if (id > 0)
                {
                    if (pa.EnUsoPorDocumento(id))
                    {
                        str.Append("0");
                        error = "Área en uso.";
                    }
                    else 
                    {
                        pa.Eliminar(id);
                        str.Append("1");
                    }
                    
                }
                else
                {
                    str.Append("0");
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Debug.WriteLine(ex.ToString());
                str.Append("0");
            }

            str.Append(",");
            str.Append("\"Error\":");
            str.Append("\"" + error + "\"");
            str.Append("}");

            return Json(str.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Obtener en un JSON las áreas correspondientes a un departamento.
        /// </summary>
        /// <param name="idDepartamento"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CargarAreas(int idDepartamento)
        {
            List<Area> lst = new PerArea().ObtenerActivasDepartamento(0, idDepartamento);
            IEnumerable<KeyValuePair<int, string>> dic = new List<KeyValuePair<int, string>>();

            if (lst != null && lst.Count > 0)
            {
                dic = from item in lst select new KeyValuePair<int, string>(item.IdArea, item.Nombre);
            }

            return Json(dic, JsonRequestBehavior.AllowGet);
        }
    }
}
