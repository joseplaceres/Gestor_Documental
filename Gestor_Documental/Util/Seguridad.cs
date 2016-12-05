/*
 * Portions based on the library password-hashing, Copyright (c) 2016 Taylor Hornby, see LICENSES.txt.
 *
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

using CryptSharp;
using CryptSharp.Utility;
using Gestor_Documental.Models.Entidades;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gestor_Documental.Util
{
    /// <summary>
    /// Estructura que contiene el salt y el hash, que se guardan en la base de datos.
    /// </summary>
    public struct SaltHash
    {
        public byte[] salt;
        public byte[] hash;

        public SaltHash(byte[] salt, byte[] hash)
        {
            this.salt = salt;
            this.hash = hash;
        }
    }

    /// <summary>
    /// Proporciona funciones de criptografía.
    /// </summary>
    /// <remarks>Usa CryptSharp y código de 'password-hashing'</remarks>
    public static class Crypto
    {
        private const int cost = 262144;
        private const int blockSize = 8;
        private const int parallel = 1;
        private const int derivedKeyLength = 128;

        /// <summary>
        /// Crear el hash y el salt que se guardaran en la BD a partir del password proporcionado.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SaltHash CrearHash(string password)
        {
            // Creamos el salt.
            string salt = Crypter.Blowfish.GenerateSalt();

            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            var maxThreads = (int?)null;

            // Creamos el hash del password.
            byte[] bytes = SCrypt.ComputeDerivedKey(keyBytes, saltBytes, cost, blockSize, parallel, maxThreads, derivedKeyLength);

            return new SaltHash(saltBytes, bytes);
        }

        /// <summary>
        /// Compara el password proporionado contra el hash proporcionado.
        /// </summary>
        /// <param name="sh"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool VerificarPassword(SaltHash sh, string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = sh.salt;
            var maxThreads = (int?)null;

            // Creamos el hash del password.
            byte[] bytes = SCrypt.ComputeDerivedKey(keyBytes, saltBytes, cost, blockSize, parallel, maxThreads, derivedKeyLength);

            return SlowEquals(sh.hash, bytes);
        }

        /// <summary>
        /// Comparacion lenta de password contra hash.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>Basado en el método del mismo nombre de 'password-hashing' de Taylor Hornby, bajo la licencia que se indica en el archivo LICENSES.txt</remarks>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;

            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0;
        }
    }

    /// <summary>
    /// Atributo para validar que exista un usuario logueado.
    /// </summary>
    public class AutorizarUsuarioAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Retorna true si hay un usuario actualmente logueado, de lo contrario false.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return Sesion.Current.IdUsuario > 0;
        }

        /// <summary>
        /// En caso de no haber ningún usuario logueado redirecciona a la vista de inicio de sesión.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Autenticacion" }, { "action", "Index" } });
        }
    }

    /// <summary>
    /// Atributo para validar los permisos de los usuarios, mediante el grupo al que pertenezcan.
    /// </summary>
    public class AutorizarGruposAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Retorna true si el usuario actualmente logueado pertenece a uno de los grupos indicados en la función usada por este atributo,
        /// de lo contrario false.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            Usuario usr = Sesion.Current.Usuario;
            var usrGrupo = UtilSeguridad.obtenerNombreGrupo(usr.IdGrupo.GetValueOrDefault());
            string[] grupos = Roles.Split(',');

            foreach (var grupo in grupos)
            {
                if (grupo.Trim() == usrGrupo)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Si el usuario no pertenece a ninguno de los grupos indicados, redirecciona a la vista de acceso denegado.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Checar si es un eliminar, ya que esos esperan un JSON.
            var action = (filterContext.ActionDescriptor.ActionName == "Eliminar") ? "DenegadoJson" : "Denegado";

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Autenticacion" }, { "action", action } });
        }
    }

    public static class UtilSeguridad
    {
        /// <summary>
        /// Obtiene el id de un grupo mediante su nombre.
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        public static int obtenerIdGrupo(string nombre)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Grupo grupo = modelo.Grupo.Where(o => o.Nombre == nombre).FirstOrDefault();
                return grupo.IdGrupo;
            }
        }

        /// <summary>
        /// Obtiene el nombre de un grupo mediante su id.
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        public static string obtenerNombreGrupo(int id)
        {
            using (var modelo = new GESTOR_DOCUMENTAL())
            {
                Grupo grupo = modelo.Grupo.Where(o => o.IdGrupo == id).FirstOrDefault();
                return grupo.Nombre;
            }
        }

        /// <summary>
        /// Retorna true si hay un usuario logueado, de lo contrario, false.
        /// </summary>
        /// <returns></returns>
        public static bool SesionIniciada()
        {
            return Sesion.Current.IdUsuario > 0;
        }
    }
}
