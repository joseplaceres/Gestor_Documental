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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Gestor_Documental.Controllers
{
    [AutorizarUsuario]
    public class DocumentoController : Controller
    {
        /// <summary>
        /// Indice de documentos.
        /// </summary>
        /// <returns></returns>
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores, Lectores")]
        public ActionResult Index()
        {
            List<Documento> lst;

            // Obtener documentos de acuerdo al grupo de lectores al que pertenezca el usuario.
            Usuario usr = Sesion.Current.Usuario;

            switch (usr.GrupoLector.Nombre)
            {
                case "SuperLector":
                    lst = new PerDocumento().ObtenerTodos();
                    break;
                case "LectorDepartamento":
                    lst = new PerDocumento().ObtenerTodosDepartamento(usr.IdDepartamento.GetValueOrDefault());
                    break;
                default:
                    lst = new PerDocumento().ObtenerTodosArea(usr.IdArea.GetValueOrDefault());
                    break;
            }

            ViewBag.msj = null;

            if (lst != null)
            {
                lst = lst.OrderBy(o => o.Departamento.Nombre).ThenBy(o => o.Area.Nombre).ThenBy(o => o.Nombre).ToList();
            }

            return View(lst);
        }

        /// <summary>
        /// Indice de documentos, obtener los que cumplan con un filtro.
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores, Lectores")]
        public ActionResult Index(string filtro)
        {
            List<Documento> lst;

            // Obtener documentos de acuerdo al grupo de lectores al que pertenezca el usuario.
            Usuario usr = Sesion.Current.Usuario;

            switch (usr.GrupoLector.Nombre)
            {
                case "SuperLector":
                    lst = new PerDocumento().ObtenerTodos(filtro);
                    break;
                case "LectorDepartamento":
                    lst = new PerDocumento().ObtenerTodosDepartamento(usr.IdDepartamento.GetValueOrDefault(), filtro);
                    break;
                default:
                    lst = new PerDocumento().ObtenerTodosArea(usr.IdArea.GetValueOrDefault(), filtro);
                    break;
            }

            ViewBag.msj = null;

            if (lst != null)
            {
                lst = lst.OrderBy(o => o.Departamento.Nombre).ThenBy(o => o.Area.Nombre).ThenBy(o => o.Nombre).ToList();
            }

            return View(lst);
        }

        /// <summary>
        /// Vista de nuevo documento.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores")]
        public ViewResult Nuevo()
        {
            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            ViewBag.Area = null;

            var doc = new Documento();
            return View(doc);
        }

        /// <summary>
        /// Guardar un nuevo documento en el repositorio y en la base de datos.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores")]
        public ActionResult Nuevo(Documento doc, HttpPostedFileBase archivo)
        {
            string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");
            //Utilerias.EscribirLog(rutaLog, "Usuario: " + Sesion.Current.Usuario.Nombre + " quiere crear un nuevo archivo.");

            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            ViewBag.Area = null;

            ActionResult ar = RedirectToAction("Index");

            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = this.GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Mensaje"] = "";

            if (archivo == null || archivo.ContentLength == 0)
            {
                job["Mensaje"] += "Error al subir el archivo: El archivo esta vacío";
                ViewBag.validacion = job;
                ar = View(doc);

                return ar;
            }

            string nombreArchivo = Path.GetFileName(archivo.FileName);

            if (doc != null)
            {
                var error = false;

                // Checar si no existe un documento con el mismo nombre en esta area y departamento.
                var lst = new PerDocumento().ObtenerTodosArea(doc.IdArea.GetValueOrDefault());

                foreach (var documento in lst)
                {
                    if (documento.Nombre == nombreArchivo)
                    {
                        error = true;
                        break;
                    }
                }

                if (error)
                {
                    job["Mensaje"] += "El nombre de archivo ya existe en esta área.";
                    ViewBag.validacion = job;
                    ar = View(doc);

                    return ar;
                }

                try
                {
                    // Crear carpetas si no existen.
                    Departamento depa = new PerDepartamento().Obtener(doc.IdDepartamento.GetValueOrDefault());
                    Area area = new PerArea().Obtener(doc.IdArea.GetValueOrDefault());

                    string depaArea = @"\" + depa.Nombre + @"\" + area.Nombre;
                    string ruta = UtilSVN.ObtenerRutaCopiaTrabajo() + depaArea;
                    Utilerias.CrearRuta(ruta);

                    // Guardar en el servidor.
                    ruta = Path.Combine(ruta, nombreArchivo);
                    Debug.WriteLine("ruta al guardar archivo: " + ruta);
                    archivo.SaveAs(ruta);

                    // Obtener el usuario logueado
                    var usr = Sesion.Current.Usuario;

                    // Agregar archivo en el SVN.
                    string msjAgregarArchivo = UtilSVN.AgregarArchivo(depaArea + nombreArchivo, usr.Nombre);

                    if (msjAgregarArchivo == "Exito")
                    {
                        doc.Nombre = nombreArchivo;
                        doc.Ruta = ruta;

                        // Crear UID.
                        int conta;
                        string uid = Utilerias.CrearUID(doc.IdDepartamento.GetValueOrDefault(), doc.IdArea.GetValueOrDefault(), out conta);

                        doc.UID = uid;
                        doc.ContadorUID = conta;

                        // Agregar registro a la BD.
                        new PerDocumento().Insertar(doc);

                        job["Error"] = false;
                    }
                    else
                    {
                        job["Mensaje"] += msjAgregarArchivo;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    //Utilerias.EscribirLog(rutaLog, ex.ToString());

                    job["Mensaje"] += "Error al subir el archivo: " + ex.Message;

                    if (ex.InnerException != null)
                    {
                        job["Mensaje"] += " " + ex.InnerException.Message;
                    }

                    ViewBag.validacion = job;
                    ar = View(doc);
                }
            }

            return ar;
        }

        /// <summary>
        /// Vista de editar un documento.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores")]
        public ActionResult Editar(int id)
        {
            Documento doc = new PerDocumento().Obtener(id);

            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            ViewBag.Area = new PerArea().ObtenerActivasDepartamento(0, doc.IdDepartamento.GetValueOrDefault());

            return View(doc);
        }

        /// <summary>
        /// Guardar los cambios de un documento en el repositorio y en la base de datos.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores")]
        public ActionResult Editar(Documento doc, HttpPostedFileBase archivo)
        {
            string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");
            //Utilerias.EscribirLog(rutaLog, "Usuario: " + Sesion.Current.Usuario.Nombre + " quiere editar archivo con id: " + doc.IdDocumento + ".");

            ViewBag.Departamento = new PerDepartamento().ObtenerActivos();
            ViewBag.Area = new PerArea().ObtenerActivasDepartamento(0, doc.IdDepartamento.GetValueOrDefault());

            ActionResult ar = RedirectToAction("Index");

            var job = new JObject();
            job["Error"] = true;
            job["Clase"] = this.GetType().Name;
            job["Metodo"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            job["Mensaje"] = "";

            if (archivo == null || archivo.ContentLength == 0)
            {
                job["Mensaje"] += "Error al subir el archivo: El archivo esta vacío";
                ViewBag.validacion = job;
                ar = View(doc);

                return ar;
            }

            if (doc != null)
            {
                // Insertar físicamente el archivo en la carpeta correspondiente y hacer commit.
                try
                {
                    // Se supone que las carpetas ya están creadas.
                    Departamento depa = new PerDepartamento().Obtener(doc.IdDepartamento.GetValueOrDefault());
                    Area area = new PerArea().Obtener(doc.IdArea.GetValueOrDefault());

                    string depaArea = @"\" + depa.Nombre + @"\" + area.Nombre;
                    string ruta = UtilSVN.ObtenerRutaCopiaTrabajo() + depaArea;

                    string nombreArchivo = Path.GetFileName(archivo.FileName);
                    ruta = Path.Combine(ruta, nombreArchivo);

                    // Obtener el usuario logueado.
                    var usr = Sesion.Current.Usuario;

                    // Borrar el archivo para poder poner el reemplazo.
                    System.IO.File.Delete(ruta);

                    // Guardar en el servidor.
                    archivo.SaveAs(ruta);

                    string msjActualizarArchivo = UtilSVN.ActualizarArchivo(depaArea + nombreArchivo, usr.Nombre);

                    if (msjActualizarArchivo == "Exito")
                    {
                        doc.Nombre = nombreArchivo;
                        doc.Ruta = ruta;

                        // Editar registro en la BD.
                        new PerDocumento().Editar(doc);

                        job["Error"] = false;
                    }
                    else
                    {
                        job["Mensaje"] += msjActualizarArchivo;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    //Utilerias.EscribirLog(rutaLog, ex.ToString());

                    job["Mensaje"] += "Error al subir el archivo: " + ex.Message;

                    if (ex.InnerException != null)
                    {
                        job["Mensaje"] += " " + ex.InnerException.Message;
                    }

                    ViewBag.validacion = job;
                    ar = View(doc);
                }
            }

            return ar;
        }

        /// <summary>
        /// Eliminar (eliminado lógico) un documento de la base de datos, mandarlo a la carpeta Eliminados en el repositorio.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores")]
        public ActionResult Eliminar(int id)
        {
            string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");
            //Utilerias.EscribirLog(rutaLog, "Usuario: " + Sesion.Current.Usuario.Nombre + " quiere eliminar archivo con id: " + id + ".");

            var str = new StringBuilder();
            var error = "";
            str.Append("{");
            str.Append("\"Eliminado\":");

            try
            {
                // Crear carpeta eliminados si no existe.
                string rutaEliminados = UtilSVN.ObtenerRutaCopiaTrabajo() + @"\Eliminados";
                bool creado = Utilerias.CrearRuta(rutaEliminados);

                // Obtener el usuario logueado
                var usr = Sesion.Current.Usuario;

                // Si se acaba de crear hacer un commit indicándolo.
                if (creado)
                {
                    UtilSVN.AgregarArchivo(@"\Eliminados", usr.Nombre);
                }

                if (id > 0)
                {
                    var pd = new PerDocumento();
                    Documento doc = pd.Obtener(id);
                    string rutaCarpetas = @"\" + doc.Departamento.Nombre + @"\" + doc.Area.Nombre;
                    string rutaEliminado = rutaEliminados + rutaCarpetas;

                    // Crear carpetas de departamento y area dentro de eliminados, y agregarlas al SVN.
                    Utilerias.CrearRuta(rutaEliminado);
                    UtilSVN.AgregarArchivo(@"\Eliminados" + rutaCarpetas, usr.Nombre);

                    rutaEliminado = rutaEliminado + @"\" + doc.Nombre;

                    // Commit eliminación.
                    string msjEliminarArchivo = UtilSVN.EliminarArchivo(doc.Ruta, rutaEliminado, rutaCarpetas + @"\" + doc.Nombre, usr.Nombre);

                    if (msjEliminarArchivo == "Exito")
                    {
                        long revEliminado = UtilSVN.ObtenerUltimaRevision();
                        Debug.WriteLine("revEliminado: " + revEliminado);

                        // Desactivar el registro de la base.
                        pd.Eliminar(id, rutaEliminado, revEliminado);

                        str.Append("1");
                    }
                    else
                    {
                        str.Append("0");
                        error += msjEliminarArchivo;
                    }
                }
                else
                {
                    str.Append("0");
                    error += "Error al eliminar el archivo: Id = 0";
                }
            }
            catch (Exception ex)
            {
                error += "Error al eliminar el archivo: " +  ex.Message;
                Debug.WriteLine(ex.ToString());
                //Utilerias.EscribirLog(rutaLog, ex.ToString());

                str.Append("0");
            }

            str.Append(",");
            str.Append("\"Error\":");
            str.Append("\"" + error + "\"");
            str.Append("}");

            Debug.WriteLine("str: " + str.ToString());

            return Json(str.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Descargar un documento.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores, Lectores")]
        public ActionResult Descargar(int id)
        {
            string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");
            //Utilerias.EscribirLog(rutaLog, "Usuario: " + Sesion.Current.Usuario.Nombre + " quiere descargar archivo con id: " + id + ".");

            Documento doc = new PerDocumento().Obtener(id);
            var rutaEnCopiaTrabajo = doc.Ruta;

            if (!System.IO.File.Exists(rutaEnCopiaTrabajo))
            {
                ViewBag.msj = "No se encontró el archivo.";
                return View("Index");
            }

            return File(rutaEnCopiaTrabajo, "application/octet-stream", Server.UrlEncode(rutaEnCopiaTrabajo));
        }

        /// <summary>
        /// Descargar una versión específica de un documento.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rev"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores")]
        public ActionResult DescargarRevision(int id, long rev)
        {
            string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");
            //Utilerias.EscribirLog(rutaLog, "Usuario: " + Sesion.Current.Usuario.Nombre + " quiere descargar archivo con id: " + id + " en su revisión: " + rev + ".");

            ViewBag.msj = null;

            Documento doc = new PerDocumento().Obtener(id);
            var rutaEnCopiaTrabajo = (doc.RevisionEliminacion.GetValueOrDefault() > 0) ? ((rev < doc.RevisionEliminacion.GetValueOrDefault()) ? doc.Ruta : doc.RutaEliminado) : doc.Ruta;

            string rutaCopiaTrabajo = UtilSVN.ObtenerRutaCopiaTrabajo();

            string rutaTemp = Path.GetFullPath(Path.Combine(rutaCopiaTrabajo, @"..\"));
            rutaTemp += "Temp";
            Utilerias.CrearRuta(rutaTemp);

            var rutaArchivoTemp = rutaTemp + @"\" +  doc.Nombre;
            Debug.WriteLine("rutaArchivoTemp: " + rutaArchivoTemp);

            string msj = UtilSVN.ObtenerRevision(rev, rutaArchivoTemp, rutaEnCopiaTrabajo);
            Debug.WriteLine("msj: " + msj);

            if (msj == "Exito")
            {
                if (System.IO.File.Exists(rutaArchivoTemp))
                {
                    return File(rutaArchivoTemp, "application/octet-stream", Server.UrlEncode(doc.Nombre));
                }
                else
                {
                    ViewBag.msj = "No se encontró el archivo.";
                    return View("Versiones");
                }
            }
            else
            {
                ViewBag.msj = msj;
                return View("Versiones");
            }
        }

        /// <summary>
        /// Ver un documento PDF mediante el visor integrado en la aplicación.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores, Lectores")]
        public ActionResult Ver(int id)
        {
            string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");
            //Utilerias.EscribirLog(rutaLog, "Usuario: " + Sesion.Current.Usuario.Nombre + " quiere ver archivo con id: " + id + ".");

            Documento doc = new PerDocumento().Obtener(id);
            var extension = Path.GetExtension(doc.Nombre);

            // Checar que sea un PDF.
            if (extension.ToLower() != ".pdf")
            {
                ViewBag.msj = "Sólo se pueden visualizar archivos PDF.";
                ViewBag.metodo = "Index";
                return View();
            }

            if (!System.IO.File.Exists(doc.Ruta))
            {
                ViewBag.msj = "No se encontró el archivo.";
                ViewBag.metodo = "Index";
                return View();
            }

            Debug.WriteLine("Navegador: " + Request.Browser.Type);
            Debug.WriteLine("Versión del navegador: " + Request.Browser.MajorVersion);

            if (Request.Browser.Type.ToUpper().Contains("IE"))
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    ViewBag.msj = "El visor de PDFs no soporta Internet Explorer 9 o inferior.";
                    ViewBag.metodo = "Index";
                    return View();
                }
            }

            string rutaTemp = HttpContext.Server.MapPath("/Temp/" + doc.Nombre);
            string rutaUrl = HttpUtility.UrlEncode("/Temp/" + HttpUtility.UrlPathEncode(doc.Nombre));

            // Copiar el archivo a la carpeta Temp.
            System.IO.File.Copy(doc.Ruta, rutaTemp, true);
            //Utilerias.EscribirLog(rutaLog, "Archivo copiado");

            // Mandar a la página del viewer con el documento seleccionado.
            return Redirect("~/Content/PDF.js/web/viewer.html?file=" + rutaUrl);
        }

        /// <summary>
        /// Ver una versión específica de un documento PDF mediante el visor integrado en la aplicación.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rev"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores")]
        public ActionResult VerRevision(int id, long rev)
        {
            string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");

            Documento doc = new PerDocumento().Obtener(id);
            var extension = Path.GetExtension(doc.Nombre);

            // Checar que sea un PDF.
            if (extension.ToLower() != ".pdf")
            {
                ViewBag.msj = "Sólo se pueden visualizar archivos PDF.";
                ViewBag.metodo = "Versiones";
                ViewBag.id = id;
                return View("Ver");
            }

            if (Request.Browser.Type.ToUpper().Contains("IE"))
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    ViewBag.msj = "El visor de PDFs no soporta Internet Explorer 8 o inferior.";
                    ViewBag.metodo = "Versiones";
                    ViewBag.id = id;
                    return View("Ver");
                }
            }

            var rutaEnCopiaTrabajo = (doc.RevisionEliminacion.GetValueOrDefault() > 0) ? ((rev < doc.RevisionEliminacion.GetValueOrDefault()) ? doc.Ruta : doc.RutaEliminado) : doc.Ruta;

            string rutaTemp = HttpContext.Server.MapPath("/Temp/" + rev + doc.Nombre);
            string rutaUrl = HttpUtility.UrlEncode("/Temp/" + rev + HttpUtility.UrlPathEncode(doc.Nombre));

            string msj = UtilSVN.ObtenerRevision(rev, rutaTemp, rutaEnCopiaTrabajo);

            if (msj == "Exito")
            {
                // Mandar a la página del viewer con el documento seleccionado.
                return Redirect("~/Content/PDF.js/web/viewer.html?file=" + rutaUrl);
            }
            else
            {
                ViewBag.msj = msj;
                return View();
            }
        }

        /// <summary>
        /// Indice de las distintas versiones de un documento.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AutorizarGrupos(Roles = "SuperUsuarios, Administradores")]
        public ActionResult Versiones(int id)
        {
            string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");
            //Utilerias.EscribirLog(rutaLog, "Usuario: " + Sesion.Current.Usuario.Nombre + " quiere ver las versiones del archivo con id: " + id + ".");

            Documento doc = new PerDocumento().Obtener(id);
            ViewBag.documento = doc;
            ActionResult ar = View(doc);
            ViewBag.colVersiones = null;
            ViewBag.msj = null;

            string msj;

            // Obtener lista de versiones anteriores.
            System.Collections.ObjectModel.Collection<SharpSvn.SvnFileVersionEventArgs> colFileVersionEventArgs = UtilSVN.ObtenerLstVersionesAnteriores(doc, out msj);

            Debug.WriteLine(msj);

            if (String.IsNullOrEmpty(msj))
            {
                ViewBag.colVersiones = colFileVersionEventArgs;
            }
            else
            {
                ViewBag.msj = msj;
            }

            return ar;
        }
    }
}
