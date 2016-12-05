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
using System.Linq;
using System.Collections.Generic;
using System;

namespace Gestor_Documental.Models.Persistencias
{
    public class PerArea
    {
        /// <summary>
        /// Obtiene un listado con todas las áreas.
        /// </summary>
        /// <returns></returns>
        public List<Area> ObtenerTodas()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Area.Include("Departamento").ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado con todas las áreas, indicando un filtro.
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<Area> ObtenerTodas(string filtro)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Area.Include("Departamento").Where(o => o.Nombre.Contains(filtro)).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos las areas activas.
        /// </summary>
        /// <returns></returns>
        public List<Area> ObtenerActivas()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos las areas activas, indicando un filtro.
        /// </summary>
        /// <returns></returns>
        public List<Area> ObtenerActivas(string filtro)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && o.Nombre.Contains(filtro)).ToList();
                return lst;
            }
        }

        // <summary>
        /// Obtiene un listado de todos las areas activas, indicando un filtro por nombre.
        /// Si se le pasa un id distinto de 0 se traen todos los registros que no coincidan con ese id.
        /// </summary>
        /// <returns></returns>
        public List<Area> ObtenerActivas(int id, string nombre)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Area> lst;
                if (id == 0)
                {
                    lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && o.Nombre == nombre).ToList();
                }
                else
                {
                    lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && o.Nombre == nombre && !(o.IdArea == id)).ToList();
                }

                return lst;
            }
        }

        // <summary>
        /// Obtiene un listado de todos las areas activas de un departamento.
        /// Si se le pasa un idArea distinto de 0 se traen todos los registros que no coincidan con ese id.
        /// </summary>
        /// <returns></returns>
        public List<Area> ObtenerActivasDepartamento(int idArea, int idDepartamento)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Area> lst;

                if (idArea == 0)
                {
                    lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && o.IdDepartamento == idDepartamento).ToList();
                }
                else
                {
                    lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && (o.IdDepartamento == idDepartamento) && !(o.IdArea == idArea)).ToList();
                }

                return lst;
            }
        }

        // <summary>
        /// Obtiene un listado de todas las áreas activas de un departamento, indicando un filtro por nombre.
        /// Si se le pasa un idArea distinto de 0 se traen todos los registros que no coincidan con ese id.
        /// </summary>
        /// <returns></returns>
        public List<Area> ObtenerActivasDepartamento(int idArea, string nombreArea, int idDepartamento)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Area> lst;
                if (idArea == 0)
                {
                    lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && o.Nombre == nombreArea && o.IdDepartamento == idDepartamento).ToList();
                }
                else
                {
                    lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && o.Nombre == nombreArea && (o.IdDepartamento == idDepartamento) && !(o.IdArea == idArea)).ToList();
                }

                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todas las áreas activas de un departamento, indicando un filtro por clave.
        /// Si se le pasa un idArea distinto de 0 se traen todos los registros que no coincidan con ese id.
        /// </summary>
        /// <param name="idArea"></param>
        /// <param name="claveArea"></param>
        /// <param name="idDepartamento"></param>
        /// <returns></returns>
        public List<Area> ObtenerActivasDepartamentoClave(int idArea, string claveArea, int idDepartamento)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Area> lst;
                if (idArea == 0)
                {
                    lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && o.Clave == claveArea && o.IdDepartamento == idDepartamento).ToList();
                }
                else
                {
                    lst = modelo.Area.Include("Departamento").Where(o => o.Activo == true && o.Clave == claveArea && (o.IdDepartamento == idDepartamento) && !(o.IdArea == idArea)).ToList();
                }

                return lst;
            }
        }

        /// <summary>
        /// Obtiene un área mediante su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Area Obtener(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var area = modelo.Area.Include("Departamento").Where(o => o.IdArea == id).FirstOrDefault();
                return area;
            }
        }

        /// <summary>
        /// Inserta una nueva área.
        /// </summary>
        /// <param name="area"></param>
        public void Insertar(Area area)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                area.Activo = true;
                area.FechaCreacion = DateTime.Now;
                area.IdUsuarioCreacion = Util.Sesion.Current.IdUsuario;
                modelo.Area.Add(area);
                modelo.SaveChanges();
            }
        }

        /// <summary>
        /// Edita los datos de un área existente.
        /// </summary>
        /// <param name="area"></param>
        public void Editar(Area area)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Area areaEditar = modelo.Area.Where(o => o.IdArea == area.IdArea).FirstOrDefault();
                if (areaEditar != null)
                {
                    areaEditar.Nombre = area.Nombre;
                    areaEditar.Clave = area.Clave;
                    areaEditar.IdDepartamento = area.IdDepartamento;
                    areaEditar.FechaEdicion = DateTime.Now;
                    areaEditar.IdUsuarioEdicion = Util.Sesion.Current.IdUsuario;
                    modelo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina (eliminado lógico) un área.
        /// </summary>
        /// <param name="id"></param>
        public void Eliminar(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Area areaEliminar = modelo.Area.Where(o => o.IdArea == id).FirstOrDefault();
                if (areaEliminar != null)
                {
                    areaEliminar.Activo = false;
                    areaEliminar.FechaEliminacion = DateTime.Now;
                    areaEliminar.IdUsuarioEliminacion = Util.Sesion.Current.IdUsuario;
                    modelo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Retorna true si esta área tiene un nombre distino con respecto al que esta guardado actualmente en la base.
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool CambioNombre(Area area)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Area areaEditar = modelo.Area.Where(o => o.IdArea == area.IdArea).FirstOrDefault();
                return areaEditar.Nombre != area.Nombre;
            }
        }

        /// <summary>
        /// Retorna true si esta área pertenece a un departamento distino con respecto al que esta guardado actualmente en la base.
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool CambioDepartamento(Area area)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Area areaEditar = modelo.Area.Where(o => o.IdArea == area.IdArea).FirstOrDefault();
                return areaEditar.IdDepartamento != area.IdDepartamento;
            }
        }

        /// <summary>
        /// Retorna true si hay algún documento que esté usando esta área, de lo contrario false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EnUsoPorDocumento(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Where(o => o.IdArea == id).ToList();
                return lst.Count > 0;
            }
        }
    }
}
