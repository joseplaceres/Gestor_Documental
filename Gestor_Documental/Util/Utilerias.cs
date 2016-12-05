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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace Gestor_Documental.Util
{
    /// <summary>
    /// Diversas funciones útiles.
    /// </summary>
    public static class Utilerias
    {
        /// <summary>
        /// Crea un carpeta en la ruta especificada, si no existía devuelve true, si ya existía devuelve false.
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public static bool CrearRuta(string ruta)
        {
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Borrar los achivos viejos de la carpeta temporal especificada.
        /// </summary>
        /// <param name="ruta"></param>
        public static void BorrarTemporales(string ruta)
        {
            if (!Directory.Exists(ruta))
            {
                return;
            }

            string[] archivos = Directory.GetFiles(ruta);

            foreach (var archivo in archivos)
            {
                FileInfo fi = new FileInfo(archivo);

                if (fi.CreationTime < DateTime.Now.AddDays(-15))
                {
                    fi.Delete();
                }
            }
        }

        /// <summary>
        /// Crea un identificador único para un determinado documento,
        /// usando las claves del departamento y área a que pertenezca.
        /// </summary>
        /// <param name="idDepartamento"></param>
        /// <param name="idArea"></param>
        /// <param name="conta"></param>
        /// <returns></returns>
        public static string CrearUID(int idDepartamento, int idArea, out int conta)
        {
            conta = 0;
            List<Documento> lst = new PerDocumento().ObtenerTodosArea(idArea);
            var contadores = new List<int>();

            foreach (var documento in lst)
            {
                if (documento.ContadorUID.HasValue)
                {
                    contadores.Add(documento.ContadorUID.Value);
                }
            }

            contadores = contadores.OrderByDescending(o => o).ToList();
            conta = contadores.FirstOrDefault();
            conta++;

            Departamento depa = new PerDepartamento().Obtener(idDepartamento);
            Area area = new PerArea().Obtener(idArea);

            var uid = depa.Clave + "-" + area.Clave + "-" + conta.ToString("D4");
            return uid;
        }
    }
}
