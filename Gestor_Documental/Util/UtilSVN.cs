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
using SharpSvn;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Gestor_Documental.Util
{
    /// <summary>
    /// Funciones para el manejo del repositorio SVN.
    /// </summary>
    public static class UtilSVN
    {
        /// <summary>
        /// Obtiene la ruta de la copia de trabajo.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerRutaCopiaTrabajo()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Configuracion configuracion = modelo.Configuracion.FirstOrDefault();
                return configuracion.RutaCopiaTrabajo;
            }
        }

        /// <summary>
        /// Obtiene la ruta del repositorio SVN.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerRutaRepositorio()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Configuracion configuracion = modelo.Configuracion.FirstOrDefault();
                return configuracion.RutaRepositorio;
            }
        }

        /// <summary>
        /// Crea un repositorio SVN en la ruta indicada.
        /// </summary>
        /// <param name="rutaRepositorio"></param>
        /// <returns></returns>
        public static string CrearRepositorio(string rutaRepositorio)
        {
            var msj = "";

            using (var svnRepoClient = new SvnRepositoryClient())
            {
                svnRepoClient.LoadConfiguration(rutaRepositorio);

                if (svnRepoClient.CreateRepository(rutaRepositorio))
                {
                    msj = "Exito";
                }
                else
                {
                    msj = "Error al crear el repositorio.";
                }
            }

            return msj;
        }

        /// <summary>
        /// Actualiza la ubicacion del repositorio.
        /// </summary>
        /// <param name="rutaRepositorio"></param>
        /// <param name="rutaRepoAntigua"></param>
        /// <returns></returns>
        public static string ActualizarRepositorio(string rutaRepositorio, string rutaRepoAntigua)
        {
            var msj = "";

            // Checar si ya se había creado el repositorio en la ruta proporcionada.
            if (rutaRepositorio == rutaRepoAntigua)
            {
                return "Exito";
            }

            // TODO Checar si no hay algún otro repositorio en la ruta proporcionada.
            using (var svnRepoClient = new SvnRepositoryClient())
            {
                svnRepoClient.LoadConfiguration(rutaRepositorio);

                if (svnRepoClient.CreateRepository(rutaRepositorio))
                {
                    msj = "Exito";
                }
                else
                {
                    msj = "Error al crear el repositorio.";
                }
            }

            return msj;
        }

        /// <summary>
        /// Crea una copia de trabajo en la ruta indicada.
        /// </summary>
        /// <param name="rutaRepositorio"></param>
        /// <param name="rutaCopiaTrabajo"></param>
        /// <returns></returns>
        public static string CrearCopiaTrabajo(string rutaRepositorio, string rutaCopiaTrabajo)
        {
            var msj = "";

            if (rutaRepositorio == rutaCopiaTrabajo)
            {
                return "Error al crear la copia de trabajo: La ruta del repositorio no puede ser la misma que la de la ruta de trabajo.";
            }

            using (var client = new SvnClient())
            {
                try
                {
                    // SvnUriTarget es una clase que envuelve las URIs de los repositorios SVN.
                    var target = new SvnUriTarget(rutaRepositorio);

                    // Hacemos el "svn checkout".
                    if (client.CheckOut(target, rutaCopiaTrabajo))
                    {
                        msj = "Exito";
                    }
                    else
                    {
                        msj += "Error al crear la copia de trabajo.";
                    }
                }
                catch (SvnException se)
                {
                    Debug.WriteLine(se.ToString());
                    msj += "Error al crear la copia de trabajo: " + se.Message;

                    if (se.InnerException != null)
                    {
                        msj += " " + se.InnerException.Message;
                    }
                }
                catch (UriFormatException ufe)
                {
                    Debug.WriteLine(ufe.ToString());
                    msj += "Error al con la ruta proporcionada: " + ufe.Message;

                    if (ufe.InnerException != null)
                    {
                        msj += " " + ufe.InnerException.Message;
                    }
                }
            }

            return msj;
        }

        /// <summary>
        /// Cambiar de ubicacion la copia de trabajo.
        /// </summary>
        /// <param name="rutaRepositorio"></param>
        /// <param name="rutaCopiaTrabajo"></param>
        /// <param name="rutaRepoAntigua"></param>
        /// <param name="rutaCopiaAntigua"></param>
        /// <returns></returns>
        public static string ActualizarCopiaTrabajo(string rutaRepositorio, string rutaCopiaTrabajo, string rutaRepoAntigua, string rutaCopiaAntigua)
        {
            var msj = "";

            if (rutaRepositorio == rutaCopiaTrabajo)
            {
                return "Error al crear la copia de trabajo: La ruta del repositorio no puede ser la misma que la de la ruta de trabajo.";
            }

            if (rutaCopiaTrabajo == rutaCopiaAntigua)
            {
                return "Exito";
            }

            // TODO Checar si ya hay un repositorio o copia en las rutas proporcionadas.
            using (var client = new SvnClient())
            {
                try
                {
                    // SvnUriTarget es una clase que envuelve las URIs de los repositorios SVN.
                    var target = new SvnUriTarget(rutaRepositorio);

                    // Hacemos el "svn checkout".
                    if (client.CheckOut(target, rutaCopiaTrabajo))
                    {
                        msj = "Exito";
                    }
                    else
                    {
                        msj += "Error al crear la copia de trabajo.";
                    }
                }
                catch (SvnException se)
                {
                    Debug.WriteLine(se.ToString());
                    msj += "Error al crear la copia de trabajo: " + se.Message;

                    if (se.InnerException != null)
                    {
                        msj += " " + se.InnerException.Message;
                    }
                }
                catch (UriFormatException ufe)
                {
                    Debug.WriteLine(ufe.ToString());
                    msj += "Error al con la ruta proporcionada: " + ufe.Message;

                    if (ufe.InnerException != null)
                    {
                        msj += " " + ufe.InnerException.Message;
                    }
                }
            }

            return msj;
        }

        /// <summary>
        /// Agrega un nuevo archivo o carpeta al SVN.
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public static string AgregarArchivo(string nombre, string usuario)
        {
            var msj = "";

            try
            {
                string rutaCopiaTrabajo = ObtenerRutaCopiaTrabajo();

                // Argumentos del agregado.
                var aa = new SvnAddArgs();
                aa.Depth = SvnDepth.Infinity;
                aa.Force = true;

                // Argumentos del commit.
                var args = new SvnCommitArgs();
                args.LogMessage = "Agregar: " + nombre + ".\nUsuario: " + usuario + ".\nFecha: " + DateTime.Now;
                args.Depth = SvnDepth.Infinity;

                using (var client = new SvnClient())
                {
                    // Agregar archivo a la copia de trabajo SVN.
                    client.Add(rutaCopiaTrabajo, aa);

                    // Hacer commit repositorio SVN.
                    client.Commit(rutaCopiaTrabajo, args);
                }

                msj = "Exito";
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                msj += "Error al subir el archivo: " + ex.Message;

                if (ex.InnerException != null)
                {
                    msj += " " + ex.InnerException.Message;
                }
            }

            return msj;
        }

        /// <summary>
        /// Actualiza un archivo en el SVN.
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public static string ActualizarArchivo(string nombre, string usuario)
        {
            var msj = "";

            try
            {
                string rutaCopiaTrabajo = ObtenerRutaCopiaTrabajo();

                // Argumentos del agregado.
                var aa = new SvnAddArgs();
                aa.Depth = SvnDepth.Infinity;
                aa.Force = true;

                // Argyumentos del commit.
                var args = new SvnCommitArgs();
                args.LogMessage = "Editar: " + nombre + ".\nUsuario: " + usuario + ".\nFecha: " + DateTime.Now;
                args.Depth = SvnDepth.Infinity;

                using (var client = new SvnClient())
                {
                    // Agregar archivo a la copia de trabajo SVN.
                    client.Add(rutaCopiaTrabajo, aa);

                    // Hacer commit repositorio SVN.
                    client.Commit(rutaCopiaTrabajo, args);
                }

                msj = "Exito";
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                msj += "Error al subir el archivo: " + ex.Message;

                if (ex.InnerException != null)
                {
                    msj += " " + ex.InnerException.Message;
                }
            }

            return msj;
        }

        /// <summary>
        /// Mueve un archivo a la carpeta eliminados, preservando la historia del SVN.
        /// </summary>
        /// <param name="rutaOrigen"></param>
        /// <param name="rutaEliminado"></param>
        /// <returns></returns>
        public static string EliminarArchivo(string rutaOrigen, string rutaEliminado, string nombre, string usuario)
        {
            var msj = "";

            try
            {
                string rutaCopiaTrabajo = ObtenerRutaCopiaTrabajo();

                // Argumentos del movido.
                var aa = new SvnMoveArgs();
                aa.Force = true;

                // Argumentos del commit.
                var args = new SvnCommitArgs();
                args.LogMessage = "Mover " + nombre + " a la carpeta Eliminados.\nUsuario: " + usuario + ".\nFecha: " + DateTime.Now;
                args.Depth = SvnDepth.Infinity;

                using (var client = new SvnClient())
                {
                    // Agregar archivo a la copia de trabajo SVN.
                    client.Move(rutaOrigen, rutaEliminado, aa);

                    // Hacer commit repositorio SVN.
                    client.Commit(rutaCopiaTrabajo, args);
                }

                msj = "Exito";
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                msj += "Error al eliminar el archivo: " + ex.Message;

                if (ex.InnerException != null)
                {
                    msj += " " + ex.InnerException.Message;
                }
            }

            return msj;
        }

        /// <summary>
        /// Obtiene una lista con todas las versiones de un archivo.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="msj"></param>
        /// <returns></returns>
        public static Collection<SvnFileVersionEventArgs> ObtenerLstVersionesAnteriores(Documento doc, out string msj)
        {
            var colFileVersionEventArgs = new Collection<SvnFileVersionEventArgs>();
            msj = "";

            try
            {
                string rutaEnRepo = (doc.Activo.GetValueOrDefault()) ? doc.Ruta : doc.RutaEliminado;
                Debug.WriteLine("rutaEnRepo: " + rutaEnRepo);

                using (var client = new SvnClient())
                {
                    var target = new SvnPathTarget(rutaEnRepo);
                    client.GetFileVersions(target, out colFileVersionEventArgs);

                    foreach (var fileVersionEventArgs in colFileVersionEventArgs)
                    {
                        Debug.WriteLine("-------------------------");
                        Debug.WriteLine("Autor: " + fileVersionEventArgs.Author);
                        Debug.WriteLine("Revision: " + fileVersionEventArgs.Revision);
                        Debug.WriteLine("VersionFile: " + fileVersionEventArgs.VersionFile);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                msj += "Error al consultar las versiones del archivo: " + ex.Message;

                if (ex.InnerException != null)
                {
                    msj += " " + ex.InnerException.Message;
                }
            }

            return colFileVersionEventArgs;
        }

        /// <summary>
        /// Obtiene la versión indicada de un archivo y la guarda en la carpeta Temp.
        /// </summary>
        /// <param name="rev"></param>
        /// <param name="rutaArchivoTemp"></param>
        /// <param name="rutaEnCopiaTrabajo"></param>
        /// <returns></returns>
        public static string ObtenerRevision(long rev, string rutaArchivoTemp, string rutaEnCopiaTrabajo)
        {
            var msj = "";

            try
            {
                using (var client = new SvnClient())
                using (Stream to = System.IO.File.Create(rutaArchivoTemp))
                {
                    client.Write(new SvnPathTarget(rutaEnCopiaTrabajo, new SvnRevision(rev)), to);
                }

                msj = "Exito";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                msj += "Error al descargar la revisión del archivo: " + ex.Message;

                if (ex.InnerException != null)
                {
                    msj += " " + ex.InnerException.Message;
                }
            }

            return msj;
        }

        /// <summary>
        /// Obtiene el número de la última revisión de la copia de trabajo.
        /// </summary>
        /// <returns></returns>
        public static long ObtenerUltimaRevision()
        {
            SvnWorkingCopyVersion version;

            using(var copiaDeTrabajo = new SvnWorkingCopyClient())
            {
                copiaDeTrabajo.GetVersion(ObtenerRutaCopiaTrabajo(), out version);
            }

            return version.End;
        }
    }
}
