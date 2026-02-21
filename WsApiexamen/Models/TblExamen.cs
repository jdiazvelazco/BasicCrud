using System;
using System.Collections.Generic;

namespace WsApiexamen.Models;

public partial class TblExamen
{
    public int IdExamen { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }
}
