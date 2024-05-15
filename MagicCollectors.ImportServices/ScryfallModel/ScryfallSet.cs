using MagicCollectors.Core.Model;
using System;
using System.Collections.Generic;

namespace MagicCollectors.ImportServices
{
    public class ScryfallSet
    {
        public Guid id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public DateTime released_at { get; set; }
        public string set_type { get; set; }
        public int card_count { get; set; }
        public bool digital { get; set; }
        public bool nonfoil_only { get; set; }
        public bool foil_only { get; set; }
        public string icon_svg_uri { get; set; }

        public Set MapToCore()
        {
            Enum.TryParse(set_type, out SetType setType);
            return new Set()
            {
                Id = id,
                Code = code,
                Name = name,
                ReleaseDate = released_at,
                CardCount = card_count,
                Digital = digital,
                NonFoilOnly = nonfoil_only,
                FoilOnly = foil_only,
                IconSvgUri = icon_svg_uri,
                Type = setType
            };
        }
    }

    public class ScryfallSetCollection
    {
        public bool has_more { get; set; }
        public List<ScryfallSet> data { get; set; }
    }
}