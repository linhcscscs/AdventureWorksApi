﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AdventureWorks.Infrastructure.Models;

/// <summary>
/// Bicycle assembly diagrams.
/// </summary>
public partial class Illustration
{
    /// <summary>
    /// Primary key for Illustration records.
    /// </summary>
    public int IllustrationID { get; set; }

    /// <summary>
    /// Illustrations used in manufacturing instructions. Stored as XML.
    /// </summary>
    public string Diagram { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public virtual ICollection<ProductModelIllustration> ProductModelIllustrations { get; set; } = new List<ProductModelIllustration>();
}