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
    public class PerDepartamento
    {
        /// <summary>
        /// Obtiene un listado de todos los departamentos.
        /// </summary>
        /// <returns></returns>
        public List<Departamento> ObtenerTodos()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Departamento.ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los departamentos, indicando un filtro.
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<Departamento> ObtenerTodos(string filtro)
        {
            using (GESTOR_DOCUMENTAL modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Departamento.Where(o => o.Nombre.Contains(filtro)).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los departamentos activos.
        /// </summary>
        /// <returns></returns>
        public List<Departamento> ObtenerActivos()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Departamento.Where(o => o.Activo == true).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los departamentos activos, indicando un filtro.
        /// </summary>
        /// <returns></returns>
        public List<Departamento> ObtenerActivos(string filtro)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Departamento.Where(o => o.Activo == true && o.Nombre.Contains(filtro)).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los departamentos activos, indicando un filtro por nombre.
        /// Si se le pasa un id distinto de 0 se traen todos los registros que no coincidan con ese id.
        /// </summary>
        /// <returns></returns>
        public List<Departamento> ObtenerActivos(int id, string nombre)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Departamento> lst;
                if (id == 0)
                {
                    lst = modelo.Departamento.Where(o => o.Activo == true && o.Nombre == nombre).ToList();
                }
                else
                {
                    lst = modelo.Departamento.Where(o => o.Activo == true && o.Nombre == nombre && !(o.IdDepartamento == id)).ToList();
                }

                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los departamentos activos, indicando un filtro por clave.
        /// Si se le pasa un id distinto de 0 se traen todos los registros que no coincidan con ese id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clave"></param>
        /// <returns></returns>
        public List<Departamento> ObtenerActivosClave(int id, string clave)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Departamento> lst;
                if (id == 0)
                {
                    lst = modelo.Departamento.Where(o => o.Activo == true && o.Clave == clave).ToList();
                }
                else
                {
                    lst = modelo.Departamento.Where(o => o.Activo == true && o.Clave == clave && !(o.IdDepartamento == id)).ToList();
                }

                return lst;
            }
        }

        /// <summary>
        /// Obtiene un departamento, mediante su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Departamento Obtener(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var depa = modelo.Departamento.Where(o => o.IdDepartamento == id).FirstOrDefault();
                return depa;
            }
        }

        /// <summary>
        /// Inserta un nuevo departamento.
        /// </summary>
        /// <param name="depa"></param>
        public void Insertar(Departamento depa)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                depa.Activo = true;
                depa.FechaCreacion = DateTime.Now;
                depa.IdUsuarioCreacion = Util.Sesion.Current.IdUsuario;
                modelo.Departamento.Add(depa);
                modelo.SaveChanges();
            }
        }

        /// <summary>
        /// Edita los datos de un departamento existente.
        /// </summary>
        /// <param name="depa"></param>
        public void Editar(Departamento depa)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Departamento depaEditar = modelo.Departamento.Where(o => o.IdDepartamento == depa.IdDepartamento).FirstOrDefault();

                if (depaEditar != null)
                {
                    depaEditar.Nombre = depa.Nombre;
                    depaEditar.Clave = depa.Clave;
                    depaEditar.FechaEdicion = DateTime.Now;
                    depaEditar.IdUsuarioEdicion= Util.Sesion.Current.IdUsuario;
                    modelo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina (eliminado lógico) un departamento.
        /// </summary>
        /// <param name="id"></param>
        public void Eliminar(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Departamento depaEliminar = modelo.Departamento.Where(o => o.IdDepartamento == id).FirstOrDefault();
                if (depaEliminar != null)
                {
                    depaEliminar.Activo = false;
                    depaEliminar.FechaEliminacion = DateTime.Now;
                    depaEliminar.IdUsuarioEliminacion = Util.Sesion.Current.IdUsuario;
                    modelo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Retorna true si este departamento tiene un nombre distino con respecto al que esta guardado actualmente en la base.
        /// </summary>
        /// <param name="depa"></param>
        /// <returns></returns>
        public bool CambioNombre(Departamento depa)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Departamento depaEditar = modelo.Departamento.Where(o => o.IdDepartamento == depa.IdDepartamento).FirstOrDefault();
                return depaEditar.Nombre != depa.Nombre;
            }
        }

        /// <summary>
        /// Retorna true si hay algún área que esté usando este departamento, de lo contrario false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EnUsoPorArea(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Area.Where(o => o.Activo == true && o.IdDepartamento == id).ToList();
                return lst.Count > 0;
            }
        }

        /// <summary>
        /// Retorna true si hay algún documento que esté usando este departamento, de lo contrario false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EnUsoPorDocumento(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Where(o => o.IdDepartamento == id).ToList();
                return lst.Count > 0;
            }
        }
    }
}
