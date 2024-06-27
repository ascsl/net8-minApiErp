using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERP;

[Index("StoreGuid", Name = "IX_StoreGuid")]
public partial class Store
{
    [Key]
    public int StoreId { get; private set; }
    public Guid StoreGuid { get; private set; } //= Guid.NewGuid();

    public string? Name { get; set; }

    [InverseProperty("Store")]
    public virtual ICollection<Raincheck> Rainchecks { get; set; } = new List<Raincheck>();


    public Store()
    {
        StoreGuid = Guid.NewGuid();
    }

}

/*
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERP;

public partial class Store
{
    [Key]
    public int StoreId { get; set; }

    public string? Name { get; set; }

    [InverseProperty("Store")]
    public virtual ICollection<Raincheck> Rainchecks { get; set; } = new List<Raincheck>();
}
*/
