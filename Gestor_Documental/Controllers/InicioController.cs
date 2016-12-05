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

using Gestor_Documental.Models.Persistencias;
using Gestor_Documental.Util;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

namespace Gestor_Documental.Controllers
{
    [AutorizarUsuario]
    public class InicioController : Controller
    {
        /// <summary>
        /// Página de inicio.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Borrar archivos temporales viejos.
            string rutaTemp = HttpContext.Server.MapPath("/Temp/");
            Utilerias.BorrarTemporales(rutaTemp);

            var pc = new PerConfiguracion();
            if (pc.Existe())
            {
                Debug.WriteLine("Existe");
                string rutaCopiaTrabajo = UtilSVN.ObtenerRutaCopiaTrabajo();
                rutaTemp = Path.GetFullPath(Path.Combine(rutaCopiaTrabajo, @"..\"));
                rutaTemp += "Temp";
                Utilerias.BorrarTemporales(rutaTemp);
            }

            // Crear carpeta Temp
            string rutaCarpetaTemp = HttpContext.Server.MapPath("/Temp");
            Utilerias.CrearRuta(rutaCarpetaTemp);

            // Crear carpata App_Data
            string rutaCarpetaAppData = HttpContext.Server.MapPath("/App_Data");
            Utilerias.CrearRuta(rutaCarpetaAppData);

            // Crear archivo log.
            //string rutaLog = HttpContext.Server.MapPath("/App_Data/Log.txt");
            //Utilerias.CrearArchivoLog(rutaLog);

            return View();
        }

        /// <summary>
        /// Vista con información del programa.
        /// </summary>
        /// <returns></returns>
        public ActionResult AcercaDe()
        {
            return View();
        }
    }
}
