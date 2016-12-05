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
    public class PerDocumento
    {
        /// <summary>
        /// Obtiene un listado de todos los documentos.
        /// </summary>
        /// <returns></returns>
        public List<Documento> ObtenerTodos()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Include("Area").Include("Departamento").Include("Usuario").Include("Usuario2").ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los documentos que coincidan con un filtro.
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<Documento> ObtenerTodos(string filtro)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Include("Area").Include("Departamento").Include("Usuario").Include("Usuario2")
                    .Where(o => o.Nombre.Contains(filtro) || o.Descripcion.Contains(filtro) || o.UID.Contains(filtro)).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los documentos de un determinado departamento.
        /// </summary>
        /// <param name="idDepartamento"></param>
        /// <returns></returns>
        public List<Documento> ObtenerTodosDepartamento(int idDepartamento)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Include("Area").Include("Departamento").Where(o => o.IdDepartamento == idDepartamento).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los documentos de un determinado departamento que coincidan con un filtro.
        /// </summary>
        /// <param name="idDepartamento"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<Documento> ObtenerTodosDepartamento(int idDepartamento, string filtro)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Include("Area").Include("Departamento")
                    .Where(o => o.IdDepartamento == idDepartamento && (o.Nombre.Contains(filtro) || o.Descripcion.Contains(filtro) || o.UID.Contains(filtro))).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los documentos de una determinada area.
        /// </summary>
        /// <param name="idArea"></param>
        /// <returns></returns>
        public List<Documento> ObtenerTodosArea(int idArea)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Include("Area").Include("Departamento").Where(o => o.IdArea == idArea).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los documentos de una determinada área que coincidan con un filtro.
        /// </summary>
        /// <param name="idArea"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<Documento> ObtenerTodosArea(int idArea, string filtro)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Include("Area").Include("Departamento")
                    .Where(o => o.IdArea == idArea && (o.Nombre.Contains(filtro) || o.Descripcion.Contains(filtro) || o.UID.Contains(filtro))).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los documentos activos.
        /// </summary>
        /// <returns></returns>
        public List<Documento> ObtenerActivos()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Documento.Include("Area").Include("Departamento").Where(o => o.Activo == true).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un documento mediante su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Documento Obtener(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var doc = modelo.Documento.Include("Area").Include("Departamento").Where(o => o.IdDocumento == id).FirstOrDefault();
                return doc;
            }
        }

        /// <summary>
        /// Guarda un nuevo registro de documento en la base.
        /// </summary>
        /// <param name="doc"></param>
        public void Insertar(Documento doc)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                doc.Activo = true;
                doc.FechaCreacion = DateTime.Now;
                doc.IdUsuarioCreacion = Util.Sesion.Current.IdUsuario;
                modelo.Documento.Add(doc);
                modelo.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un registro de documento en la base.
        /// </summary>
        /// <param name="doc"></param>
        public void Editar(Documento doc)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Documento docEditar = modelo.Documento.Where(o => o.IdDocumento == doc.IdDocumento).FirstOrDefault();
                if (docEditar != null)
                {
                    docEditar.Activo = true;
                    docEditar.Nombre = doc.Nombre;
                    docEditar.Descripcion = doc.Descripcion;
                    docEditar.Ruta = doc.Ruta;
                    docEditar.IdDepartamento = doc.IdDepartamento;
                    docEditar.IdArea = doc.IdArea;
                    docEditar.FechaEdicion = DateTime.Now;
                    docEditar.IdUsuarioEdicion = Util.Sesion.Current.IdUsuario;
                    modelo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Marca como "eliminado" (eliminado lógico) un documento, para que se pinte en rojo.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rutaEliminado"></param>
        /// <param name="revEliminado"></param>
        public void Eliminar(int id, string rutaEliminado, long revEliminado)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Documento docEliminar = modelo.Documento.Where(o => o.IdDocumento == id).FirstOrDefault();
                if (docEliminar != null)
                {
                    docEliminar.Activo = false;
                    docEliminar.RutaEliminado = rutaEliminado;
                    docEliminar.RevisionEliminacion = revEliminado;
                    docEliminar.FechaEliminacion = DateTime.Now;
                    docEliminar.IdUsuarioEliminacion = Util.Sesion.Current.IdUsuario;
                    modelo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Borra todos los registros de documentos de la base (eliminado físico).
        /// </summary>
        public void EliminarTodos()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Documento> lst = modelo.Documento.ToList();

                foreach (var doc in lst)
                {
                    modelo.Documento.Remove(doc);
                }

                modelo.SaveChanges();
            }
        }
    }
}
