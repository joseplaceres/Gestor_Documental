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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestor_Documental.Models.Persistencias
{
    public class PerGrupo
    {
        /// <summary>
        /// Obtiene un listado de todos los grupos.
        /// </summary>
        /// <returns></returns>
        public List<Grupo> ObtenerTodos()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Grupo.ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los grupos, indicando un filtro por nombre.
        /// Si se le pasa un id distinto de 0 se traen todos los registros que no coincidan con ese id.
        /// </summary>
        /// <returns></returns>
        public List<Grupo> ObtenerTodos(int id, string nombre)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Grupo> lst;
                if (id == 0)
                {
                    lst = modelo.Grupo.Where(o => o.Nombre == nombre).ToList();
                }
                else
                {
                    lst = modelo.Grupo.Where(o => o.Nombre == nombre && !(o.IdGrupo == id)).ToList();
                }

                return lst;
            }
        }

        /// <summary>
        /// Obtiene un grupo, mediante su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Grupo Obtener(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var grupo = modelo.Grupo.Where(o => o.IdGrupo == id).FirstOrDefault();
                return grupo;
            }
        }

        /// <summary>
        /// Retorna true si hay algún usuario que pertenezca a este grupo, de lo contrario false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EnUsoPorUsr(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Usuario.Where(o => o.Activo == true && o.IdGrupo == id).ToList();
                return lst.Count > 0;
            }
        }
    }
}
