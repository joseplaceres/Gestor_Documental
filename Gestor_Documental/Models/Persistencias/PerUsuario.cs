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
using Gestor_Documental.Util;
using System.Diagnostics;

namespace Gestor_Documental.Models.Persistencias
{
    public class PerUsuario
    {
        /// <summary>
        /// Obtiene un listado de todos los usuarios.
        /// </summary>
        /// <returns></returns>
        public List<Usuario> ObtenerTodos()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Usuario.ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los usuarios, indicando un filtro.
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<Usuario> ObtenerTodos(string filtro)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Usuario.Where(o => o.Nombre.Contains(filtro)).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los usuarios activos.
        /// </summary>
        /// <returns></returns>
        public List<Usuario> ObtenerActivos()
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Usuario.Include("Grupo").Include("GrupoLector").Include("Departamento").Include("Area").Where(o => o.Activo == true).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los usuarios activos, indicando un filtro.
        /// </summary>
        /// <returns></returns>
        public List<Usuario> ObtenerActivos(string filtro)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var lst = modelo.Usuario.Where(o => o.Activo == true && o.Nombre.Contains(filtro)).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Obtiene un listado de todos los usuarios activos, indicando un filtro por nombre y alias.
        /// Si se le pasa un id distinto de 0 se traen todos los registros que no coincidan con ese id.
        /// </summary>
        /// <returns></returns>
        public List<Usuario> ObtenerActivos(int id, string alias, string nombre)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                List<Usuario> lst;
                if (id == 0)
                {
                    lst = modelo.Usuario.Include("Grupo").Where(o => o.Activo == true && (o.Nombre == nombre || o.Alias == alias)).ToList();
                }
                else
                {
                    lst = modelo.Usuario.Include("Grupo").Where(o => o.Activo == true && (o.Nombre == nombre || o.Alias == alias) && !(o.IdUsuario == id)).ToList();
                }

                return lst;
            }
        }

        /// <summary>
        /// Obtiene un usuario mediante su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Usuario Obtener(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var usr = modelo.Usuario.Include("Grupo").Include("GrupoLector").Where(o => o.IdUsuario == id).FirstOrDefault();
                return usr;
            }
        }

        /// <summary>
        /// Obtiene un usuario mediante su alias.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public Usuario Obtener(string alias)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                var usr = modelo.Usuario.Include("Grupo").Where(o => o.Alias == alias && o.Activo == true).FirstOrDefault();
                return usr;
            }
        }

        /// <summary>
        /// Inserta un nuevo usuario.
        /// </summary>
        /// <param name="usr"></param>
        public void Insertar(Usuario usr)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                SaltHash sh = Crypto.CrearHash(usr.Password);
                usr.Password = Convert.ToBase64String(sh.hash);
                usr.Salt = Convert.ToBase64String(sh.salt);
                usr.Activo = true;
                usr.FechaCreacion = DateTime.Now;
                usr.IdUsuarioCreacion = Util.Sesion.Current.IdUsuario;
                modelo.Usuario.Add(usr);
                modelo.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un usuario existente.
        /// </summary>
        /// <param name="usr">Datos del usuario.</param>
        /// <param name="changePassword">Indica si se le cambió el password.</param>
        public void Editar(Usuario usr, bool changePassword)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Usuario usrEditar = modelo.Usuario.Where(o => o.IdUsuario == usr.IdUsuario).FirstOrDefault();
                if (usrEditar != null)
                {
                    usrEditar.Alias = usr.Alias;
                    usrEditar.Nombre = usr.Nombre;
                    usrEditar.IdGrupo = usr.IdGrupo;
                    usrEditar.IdDepartamento = usr.IdDepartamento;
                    usrEditar.IdArea = usr.IdArea;
                    usrEditar.IdGrupoLector = usr.IdGrupoLector;
                    usrEditar.FechaEdicion = DateTime.Now;
                    usrEditar.IdUsuarioEdicion = Util.Sesion.Current.IdUsuario;

                    if (changePassword)
                    {
                        SaltHash sh = Crypto.CrearHash(usr.Password);
                        usrEditar.Password = Convert.ToBase64String(sh.hash);
                        usrEditar.Salt = Convert.ToBase64String(sh.salt);
                    }

                    modelo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Eliminado lógico del usuario indicado.
        /// </summary>
        /// <param name="id"></param>
        public void Eliminar(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Usuario usrEliminar = modelo.Usuario.Where(o => o.IdUsuario == id).FirstOrDefault();
                if (usrEliminar != null)
                {
                    usrEliminar.Activo = false;
                    usrEliminar.FechaEliminacion = DateTime.Now;
                    usrEliminar.IdUsuarioEliminacion = Util.Sesion.Current.IdUsuario;
                    modelo.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Retorna true si el usuario existe y coincide el password, de lo contrario false.
        /// </summary>
        /// <param name="usr"></param>
        /// <returns></returns>
        public bool Validar(Usuario usr)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                if (modelo.Usuario.Where(o => o.Alias == usr.Alias && o.Activo == true).Count() > 0)
                {
                    var usrAutenticar = modelo.Usuario.Where(o => o.Alias == usr.Alias && o.Activo == true).FirstOrDefault();
                    var pass = Convert.FromBase64String(usrAutenticar.Password);
                    var salt = Convert.FromBase64String(usrAutenticar.Salt);
                    var sh = new SaltHash(salt, pass);

                    return Crypto.VerificarPassword(sh, usr.Password);
                }

                return false;
            }
        }
    }
}
