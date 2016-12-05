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
using System.Linq;
using Gestor_Documental.Models.Entidades;

namespace Gestor_Documental.Models.Persistencias
{
    public class PerConfiguracion
    {
        /// <summary>
        /// Obtiene la configuración, si existe.
        /// </summary>
        /// <returns></returns>
        public Configuracion Obtener()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                return (Existe()) ? modelo.Configuracion.FirstOrDefault() : new Configuracion();
            }
        }

        /// <summary>
        /// Retorna true si existe la configración, de lo contrario false.
        /// </summary>
        /// <returns></returns>
        public bool Existe()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                int cuenta = modelo.Configuracion.Count();
                return cuenta > 0;
            }
        }

        /// <summary>
        /// Inserta la configuración.
        /// </summary>
        /// <param name="conf"></param>
        public void Insertar(Configuracion conf)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                conf.FechaCreacion = DateTime.Now;
                conf.IdUsuarioCreacion = Util.Sesion.Current.IdUsuario;
                modelo.Configuracion.Add(conf);
                modelo.SaveChanges();
            }
        }

        /// <summary>
        /// Edita la configuración.
        /// </summary>
        /// <param name="conf"></param>
        public void Editar(Configuracion conf)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Configuracion confEditar = modelo.Configuracion.Where(o => o.IdConfiguracion == conf.IdConfiguracion).FirstOrDefault();

                if (confEditar != null)
                {
                    confEditar.RutaRaiz = conf.RutaRaiz;
                    confEditar.RutaRepositorio = conf.RutaRepositorio;
                    confEditar.RutaCopiaTrabajo = conf.RutaCopiaTrabajo;
                    confEditar.FechaEdicion = DateTime.Now;
                    confEditar.IdUsuarioEdicion = Util.Sesion.Current.IdUsuario;
                    modelo.SaveChanges();
                }
            }
        }
    }
}
