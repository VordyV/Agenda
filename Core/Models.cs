using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BLite.Bson;

namespace Agenda.Core;

[Table("profiles")]
public class ProfileModel
{
    [Key]
    public ObjectId Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string ModuleId { get; set; }
    [Required]
    public Dictionary<string, string?> Fields { get; set; }
    [Required]
    public DateTime LastSessionDate { get; set; }
}