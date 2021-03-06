//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gestor_Documental.Models.Entidades
{
    using System;
    using System.Collections.Generic;
    
    public partial class Documento
    {
        public int IdDocumento { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public Nullable<int> IdDepartamento { get; set; }
        public Nullable<int> IdArea { get; set; }
        public Nullable<int> IdUsuarioCreacion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<int> IdUsuarioEdicion { get; set; }
        public Nullable<System.DateTime> FechaEdicion { get; set; }
        public Nullable<int> IdUsuarioEliminacion { get; set; }
        public Nullable<System.DateTime> FechaEliminacion { get; set; }
        public Nullable<bool> Activo { get; set; }
        public string RutaEliminado { get; set; }
        public Nullable<long> RevisionEliminacion { get; set; }
        public string UID { get; set; }
        public string Descripcion { get; set; }
        public Nullable<int> ContadorUID { get; set; }
    
        public virtual Area Area { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario Usuario1 { get; set; }
        public virtual Usuario Usuario2 { get; set; }
    }
}
