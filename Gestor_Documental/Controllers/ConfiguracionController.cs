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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestor_Documental.Models.Entidades;
using Gestor_Documental.Models.Persistencias;
using Newtonsoft.Json.Linq;
using SharpSvn;
using Gestor_Documental.Util;

namespace Gestor_Documental.Controllers
{
    [AutorizarUsuario]
    public class ConfiguracionController : Controller
    {
        /// <summary>
        /// Vista de configuración.
        /// </summary>
        /// <returns></returns>
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Index()
        {
            bool existe = new PerConfiguracion().Existe();
            ViewBag.existe = existe;

            Configuracion conf = new PerConfiguracion().Obtener();
            return View(conf);
        }

        /// <summary>
        /// Guardar los datos de configuración.
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios")]
        public ActionResult Index(Configuracion conf)
        {
            ActionResult ar = RedirectToAction("Index");

            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = this.GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Mensaje"] = "";

            var rutaRepositorio = conf.RutaRaiz + @"\Repositorio";
            var rutaCopiaTrabajo = conf.RutaRaiz + @"\CopiaTrabajo";

            conf.RutaRepositorio = rutaRepositorio;
            conf.RutaCopiaTrabajo = rutaCopiaTrabajo;

            var pc = new PerConfiguracion();

            // Ya había una configuración.
            if (pc.Existe())
            {
                try
                {
                    // Obtener las rutas antiguas.
                    Configuracion config = pc.Obtener();
                    var rutaRepoAntigua = config.RutaRepositorio;
                    var rutaCopiaAntigua =  config.RutaCopiaTrabajo;

                    string msjRepositorio = UtilSVN.ActualizarRepositorio(rutaRepositorio, rutaRepoAntigua);
                    string msjCopiaTrabajo = UtilSVN.ActualizarCopiaTrabajo(rutaRepositorio, rutaCopiaTrabajo, rutaRepoAntigua, rutaCopiaAntigua);

                    if (msjRepositorio == "Exito" && msjCopiaTrabajo == "Exito")
                    {
                        pc.Editar(conf);

                        // Borrar todos los documentos que existan, para que no se mezclen los del anterior repositorio con los del nuevo en la base.
                        new PerDocumento().EliminarTodos();

                        job["Error"] = false;
                    }
                    else
                    {
                        if (msjRepositorio != "Exito")
                        {
                            job["Mensaje"] += msjRepositorio;
                        }

                        if (msjCopiaTrabajo != "Exito")
                        {
                            job["Mensaje"] += msjCopiaTrabajo;
                        }

                        ViewBag.validacion = job;
                        ar = View(conf);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    job["Mensaje"] += "Error al guardar el registro: " + ex.Message;

                    if (ex.InnerException != null)
                    {
                        job["Mensaje"] += " " + ex.InnerException.Message;
                    }

                    ViewBag.validacion = job;
                    ar = View(conf);
                }
            }
            else // Se creará la configuración por primera vez.
            {
                try
                {
                    string msjRepositorio = UtilSVN.CrearRepositorio(rutaRepositorio);
                    string msjCopiaTrabajo = UtilSVN.CrearCopiaTrabajo(rutaRepositorio, rutaCopiaTrabajo);

                    if (msjRepositorio == "Exito" && msjCopiaTrabajo == "Exito")
                    {
                        pc.Insertar(conf);
                        job["Error"] = false;
                    }
                    else
                    {
                        if (msjRepositorio != "Exito")
                        {
                            job["Mensaje"] += msjRepositorio;
                        }

                        if (msjCopiaTrabajo != "Exito")
                        {
                            job["Mensaje"] += msjCopiaTrabajo;
                        }

                        ViewBag.validacion = job;
                        ar = View(conf);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    job["Mensaje"] += "Error al guardar el registro: " + ex.Message;

                    if (ex.InnerException != null)
                    {
                        job["Mensaje"] += " " + ex.InnerException.Message;
                    }

                    ViewBag.validacion = job;
                    ar = View(conf);
                }
            }

            return ar;
        }
    }
}
