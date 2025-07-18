﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AdventureWorks.Infrastructure.Models;

/// <summary>
/// Customer reviews of products they have purchased.
/// </summary>
public partial class ProductReview
{
    /// <summary>
    /// Primary key for ProductReview records.
    /// </summary>
    public int ProductReviewID { get; set; }

    /// <summary>
    /// Product identification number. Foreign key to Product.ProductID.
    /// </summary>
    public int ProductID { get; set; }

    /// <summary>
    /// Name of the reviewer.
    /// </summary>
    public string ReviewerName { get; set; }

    /// <summary>
    /// Date review was submitted.
    /// </summary>
    public DateTime ReviewDate { get; set; }

    /// <summary>
    /// Reviewer&apos;s e-mail address.
    /// </summary>
    public string EmailAddress { get; set; }

    /// <summary>
    /// Product rating given by the reviewer. Scale is 1 to 5 with 5 as the highest rating.
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Reviewer&apos;s comments
    /// </summary>
    public string Comments { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public virtual Product Product { get; set; }
}