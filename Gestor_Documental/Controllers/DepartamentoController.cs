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
    public class DepartamentoController : Controller
    {
        /// <summary>
        /// Indice de departamentos.
        /// </summary>
        /// <returns></returns>
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Index()
        {
            List<Departamento> lst = new PerDepartamento().ObtenerActivos();

            if (lst != null)
            {
                lst = lst.OrderBy(o => o.Nombre).ToList();
            }

            return View(lst);
        }

        /// <summary>
        /// Nuevo departamento.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ViewResult Nuevo()
        {
            var depa = new Departamento();
            return View(depa);
        }

        /// <summary>
        /// Guardar datos de nuevo departamento.
        /// </summary>
        /// <param name="depa"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Nuevo(Departamento depa)
        {
            ActionResult ar = RedirectToAction("Index");
            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = this.GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Nombre"] = "";
            job["Clave"] = "";

            if (depa != null)
            {
                depa.Nombre = depa.Nombre.Trim();
                depa.Clave = depa.Clave.Trim();

                var error = false;
                var pd = new PerDepartamento();

                // Checar si ya hay un departamento con este nombre.
                List<Departamento> lst = pd.ObtenerActivos(0, depa.Nombre);

                if (lst.Count() > 0)
                {
                    Debug.WriteLine("lst.Count() > 0");
                    error = true;

                    foreach (var departamento in lst)
                    {
                        if (departamento.Nombre.ToUpper() == depa.Nombre.ToUpper())
                        {
                            job["Nombre"] += "El departamento \"" + depa.Nombre + "\" ya existe. ";
                        }
                    }
                }

                // Checar si ya hay un departamento con esta clave.
                List<Departamento> lstClave = pd.ObtenerActivosClave(0, depa.Clave);

                if (lstClave.Count() > 0) 
                {
                    error = true;

                    foreach (var departamento in lstClave)
                    { 
                        if(departamento.Clave.ToUpper() == depa.Clave.ToUpper()) 
                        {
                            job["Clave"] += "La clave \"" + depa.Clave + "\" ya existe. ";
                        }
                    }
                }

                if (error) 
                {
                    ViewBag.validacionDeDatos = job;
                    ar = View(depa);
                }
                else
                {
                    try
                    {
                        pd.Insertar(depa);
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
                        ar = View(depa);
                    }
                }
            }

            return ar;
        }

        /// <summary>
        /// Editar un departamento.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ViewResult Editar(int id)
        {
            Departamento depa = new PerDepartamento().Obtener(id);
            return View(depa);
        }

        /// <summary>
        /// Guardar los cambios de un departamento.
        /// </summary>
        /// <param name="depa"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Editar(Departamento depa)
        {
            ActionResult ar = RedirectToAction("Index");
            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = this.GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Nombre"] = "";
            job["Clave"] = "";
            
            if (depa != null)
            {
                depa.Nombre = depa.Nombre.Trim();
                depa.Clave = depa.Clave.Trim();

                var error = false;
                var pd = new PerDepartamento();

                // Checar si ya hay un departamento con este nombre.
                List<Departamento> lst = pd.ObtenerActivos(depa.IdDepartamento, depa.Nombre);

                if (lst.Count() > 0)
                {
                    Debug.WriteLine("lst.Count() > 0");
                    error = true;
                    foreach (var departamento in lst)
                    {
                        if (departamento.Nombre.ToUpper() == depa.Nombre.ToUpper())
                        {
                            job["Nombre"] += "El departamento \"" + depa.Nombre + "\" ya existe. ";
                        }
                    }
                }

                // Checar si ya hay un departamento con esta clave.
                List<Departamento> lstClave = pd.ObtenerActivosClave(depa.IdDepartamento, depa.Clave);

                if (lstClave.Count() > 0)
                {
                    error = true;

                    foreach (var departamento in lstClave)
                    {
                        if (departamento.Clave.ToUpper() == depa.Clave.ToUpper())
                        {
                            job["Clave"] += "La clave \"" + depa.Clave + "\" ya existe. ";
                        }
                    }
                }

                // Checar si hubo cambio de nombre, y si sí, checar si ya está siendo usado por un documento.
                if (pd.CambioNombre(depa)) 
                {
                    if (pd.EnUsoPorDocumento(depa.IdDepartamento)) 
                    {
                        error = true;
                        job["Nombre"] += "No se le puede cambiar el nombre a un departamento que ya está siendo usado por uno o más documentos. ";
                    }
                }

                if (error) 
                {
                    ViewBag.validacionDeDatos = job;
                    ar = View(depa);
                }
                else
                {
                    try
                    {
                        pd.Editar(depa);
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
                        ar = View(depa);
                    }
                }
            }
            
            return ar;
        }

        /// <summary>
        /// Validación antes de eliminar.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult ValidarParaEliminar(int id)
        {
            var str = new StringBuilder();
            str.Append("{");
            str.Append("\"EnUso\":");

            if (new PerDepartamento().EnUsoPorArea(id))
            {
                str.Append("1");
            }
            else
            {
                str.Append("0");
            }

            str.Append("}");

            return Json(str.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Eliminar (eliminado lógico) un departamento.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Eliminar(int id)
        {
            var pd = new PerDepartamento();
            var error = "";
            var str = new StringBuilder();

            str.Append("{");
            str.Append("\"Eliminado\":");

            try
            {
                if (id > 0)
                {
                    // Checar si no hay un área o un documento que haga referencia a éste departamento.
                    if (pd.EnUsoPorArea(id) || pd.EnUsoPorDocumento(id))
                    {
                        str.Append("0");
                        error = "Departamento en uso.";
                        Debug.WriteLine("Departamento en uso");
                    }
                    else
                    {
                        pd.Eliminar(id);
                        str.Append("1");
                        Debug.WriteLine("Eliminado");
                    }
                }
                else
                {
                    str.Append("0");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                error = ex.Message;
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
