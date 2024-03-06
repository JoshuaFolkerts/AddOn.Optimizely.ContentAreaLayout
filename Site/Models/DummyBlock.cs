using AddOn.Optimizely.ContentAreaLayout.Attributes;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Site.Models
{
    /// <summary>
    /// </summary>
    [ContentType(GUID = "E946BA2B-5DDC-443F-8D1F-7E0DC96B6315")]
    public class DummyBlock : BlockData
    {
        [Display(Name = "Text")]
        [BlockRenderingMetadataAttribute]
        public virtual string Text { get; set; }
    }
}
